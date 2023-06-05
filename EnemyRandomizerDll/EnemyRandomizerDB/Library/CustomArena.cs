using UnityEngine;
using UnityEngine.SceneManagement;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker.Actions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{
    public class CustomArena : BattleStateMachine
    {
        public PlayMakerFSM manager;

        public RNG rng;
        public int seedOffset = 0;//change this to mix up the arena rng
        public EntitySpawner spawner;

        public GameObject audioPlayer;
        public AudioClip colCageAppear;
        public AudioClip colCageOpen;
        public AudioClip colCageDown;

        protected HashSet<ArenaCageControl> queue = new HashSet<ArenaCageControl>();



        protected int stateIndex = 0;
        protected HutongGames.PlayMaker.FsmState currentState => states == null || states.Count <= 0 ? null : states[stateIndex];
        protected List<HutongGames.PlayMaker.FsmState> states = new List<HutongGames.PlayMaker.FsmState>();

        public void CrowdCheer()
        {
            PlayMakerFSM.BroadcastEvent("CROWD CHEER");
        }

        public void CrowdLaugh()
        {
            PlayMakerFSM.BroadcastEvent("CROWD LAUGH");
        }

        public void EndColo()
        {
            PlayerData.instance.IntAdd("killsDummy", 1);
            manager.SendEvent("WAVES COMPLETED");
            OpenGates();
        }

        protected virtual void BuildWaves(PlayMakerFSM fsm)
        {
            //??
        }

        public override void Dispose()
        {
            battleStarted = false;
            battleEnded = false;
            isCustomArena = false;
            ((IDisposable)disposables).Dispose();
        }

        public override void Setup(Scene scene, PlayMakerFSM fsm)
        {
            FSM = fsm.gameObject.LocateMyFSM("Battle Control");
            fsm = FSM;
            seedOffset += PlayerData.instance.GetInt("killsDummy");
            rng = new RNG(EnemyRandomizerDatabase.GetPlayerSeed() + seedOffset);
            preKilledEnemies = 0;
            battleStarted = false;
            battleEnded = false;
            BattleScene = scene;
            isCustomArena = true;

            SetupManager();
            SetupAudio();

            try
            {
                BuildWaves(FSM);
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            try
            {
                //should be no enemies alive on scene entry
                SpawnedObjectControl.GetAllEnemies.ToList().ForEach(x => SpawnerExtensions.DestroyObject(x.gameObject,true));
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void SetupManager()
        {
            try
            {
                manager = FSM.gameObject.LocateMyFSM("Manager");
                {
                    manager.GetState("Waves Start").AddCustomAction(() => { StartBattle(); });
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void SetupAudio()
        {
            try
            {
                SetupAudioCageUp();
                SetupAudioCageOpen();
                SetupAudioCageDown();
            }
            catch(Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void SetupAudioCageUp()
        {
            var audioCageUp = FSM.gameObject.LocateMyFSM("Audio Cage Up");
            {
                var state2 = audioCageUp.GetState("State 2");
                audioPlayer = state2.GetAction<AudioPlayerOneShotSingle>(0).audioPlayer.Value;
                colCageAppear = state2.GetAction<AudioPlayerOneShotSingle>(0).audioClip.Value as AudioClip;
                //audioCageUp.enabled = false;
            }
        }

        protected virtual void SetupAudioCageOpen()
        {
            var audioCageOpen = FSM.gameObject.LocateMyFSM("Audio Cage Open");
            {
                var state2 = audioCageOpen.GetState("State 2");
                colCageOpen = state2.GetAction<AudioPlayerOneShotSingle>(0).audioClip.Value as AudioClip;
                //audioCageOpen.enabled = false;
            }
        }

        protected virtual void SetupAudioCageDown()
        {
            var audioCageDown = FSM.gameObject.LocateMyFSM("Audio Cage Open");
            {
                var state3 = audioCageDown.GetState("State 3");
                colCageDown = state3.GetAction<AudioPlayerOneShotSingle>(1).audioClip.Value as AudioClip;
                //audioCageDown.enabled = false;
            }
        }

        public void PlayCageUp()
        {
            PlayMakerFSM.BroadcastEvent("AUDIO CAGE UP");
            //PlaySound(colCageAppear);
        }

        public void PlayCageOpen()
        {
            PlayMakerFSM.BroadcastEvent("AUDIO CAGE OPEN");
            //PlaySound(colCageOpen);
        }

        public void PlayCageDown()
        {
            //PlayMakerFSM.BroadcastEvent("AUDIO CAGE OPEN");
            //PlaySound(colCageDown);
        }

        public void PlaySound(AudioClip clip)
        {
            audioPlayer.Spawn(HeroController.instance.transform.position, Quaternion.Euler(Vector3.up));
            var audio = audioPlayer.GetComponent<AudioSource>();
            audio.PlayOneShot(clip);
        }

        public GameObject Spawn(Vector2 pos, string enemy = null, string originalEnemy = null)
        {
            return spawner.SpawnCustomArenaEnemy(pos, enemy, originalEnemy, rng);            
        }

        protected virtual void StartBattle()
        {
            if(currentState == null)
            {
                Dev.LogError($"currentState is null and this should never happen! statesCount:{states.Count} currentIndex:{stateIndex}");
            }

            spawner = FSM.gameObject.GetOrAddComponent<EntitySpawner>();
            spawner.maxChildren = 100;

            CloseGates();
            BattleManager.Instance.Value.StartCoroutine(DoBattle());
            OnBattleStarted();
        }

        protected override void OnBattleStarted()
        {
            battleStarted = true;
            battleEnded = false;
        }

        protected virtual void EndBattle()
        {
            OnBattleEnded();
        }

        protected override void OnBattleEnded()
        {
            base.OnBattleEnded();
            EndColo();
        }

        protected virtual void SetFSMToNextState()
        {
            if (stateIndex >= states.Count)
            {
                Dev.LogError("Somehow this was invoked beyond the end? Trying to end again...");
                EndBattle();
                return;
            }

            stateIndex++;
            if(stateIndex >= states.Count)
            {
                EndBattle();
            }
            else
            {
                SetFSMToCurrentState();
            }
        }

        public override void DecrementBattleEnemies()
        {
            //do nothing for custom arenas, we're in full control
        }

        public override void RegisterEnemyDeath(SpawnedObjectControl soc)
        {
            //do nothing for now, this should be automatically handled by the child controller
        }

        protected virtual bool CheckNextState()
        {
            if (waitingOnZote)
                return false;

            bool stateChanged = true;
            try
            {
                if (currentState.Name.Contains("Wave ") || currentState.Name.Contains("Wave 29 Zote"))
                {
                    if (spawner.Children.Count <= 0 && queue.Count <= 0)
                    {
                        SetFSMToNextState();
                    }
                    else
                    {
                        stateChanged = false;
                    }
                }
                else if (currentState.Name.Contains("Init"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Arena "))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Respawn"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Reset"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Pause"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Gruz Arena"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else
                {
                    Dev.LogError("Unknown state in custom arena state list!");
                    stateChanged = false;
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            return stateChanged;
        }

        public int zotesSpawned = 0;
        protected virtual void OnZoteSpawn(ArenaCageControl cage, GameObject zote)
        {
            var zotec = zote.GetComponent<ZoteBossControl>();
            if (zotec != null)
            {
                zotec.SetColor(zotesSpawned);

                float scale = rng.Rand(0.5f, 1.2f);
                zotec.gameObject.ScaleObject(scale);
                zotec.gameObject.ScaleAudio(scale);
            }

            zotesSpawned++;
            if(zotesSpawned >= 57)
            {
                cage.onSpawn -= OnZoteSpawn;
            }
        }

        public virtual void RemoveFromQueue(ArenaCageControl cage, GameObject spawnedObject)
        {
            queue.Remove(cage);
            cage.onSpawn -= RemoveFromQueue;
        }

        public void AddToQueue(ArenaCageControl cage)
        {
            cage.onSpawn += RemoveFromQueue;
            queue.Add(cage);
        }

        public bool isZoteWave = false;

        protected virtual bool IsZoteWave()
        {
            if (SceneName == "Room_Colosseum_Bronze" && currentState.Name.Contains("Wave 29 Zote"))
            {
                isZoteWave = true;
            }

            return isZoteWave;
        }

        protected virtual int GetWaveIndex()
        {
            int index = 0;
            string nameToParse = currentState.Name.Split(' ').Last();

            if (!IsZoteWave())
            {
                index = int.Parse(nameToParse);
            }
            else
            {
                index = 29;
            }
            return index;
        }

        protected virtual List<GameObject> GetCages(string goName)
        {
            return FSM.gameObject.FindGameObjectInDirectChildren("Waves").FindGameObjectInDirectChildren(goName).GetDirectChildren();
        }

        protected virtual float GetWaveDelay()
        {
            return currentState.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SPAWN").FirstOrDefault().delay.Value;
        }

        protected virtual void SpawnZoteWave(GameObject zoteCage, float waveDelay)
        {
            var c = SpawnerExtensions.SpawnEntityAt("Arena Cage Zote", zoteCage.transform.position, true, false);
            var cc = c.GetComponent<ArenaCageControl>();
            if (cc != null)
            {
                cc.GetComponent<ArenaCageControl>().onSpawn += OnZoteSpawn;
                cc.owner = this;
                cc.flingRandomly = true;
                cc.Spawn("Zote Boss", waveDelay);
            }
        }

        protected virtual void SpawnWave()
        {
            try
            {
                int index = GetWaveIndex();
                string goName = "Wave " + index;

                List<GameObject> cages = GetCages(goName);
                float waveDelay = GetWaveDelay();

                if (isZoteWave)
                {
                    waitingOnZote = true;
                    SpawnZoteWave(cages.First(), waveDelay);
                }
                else
                {
                    var smCages = cages.Where(x => x.name.Contains("Small"));
                    var lgCages = cages.Where(x => x.name.Contains("Large"));

                    foreach (var cage in smCages)
                    {
                        var c = SpawnerExtensions.SpawnEntityAt("Arena Cage Small", cage.transform.position, true, false);
                        var cc = c.GetComponent<ArenaCageControl>();
                        if (cc != null)
                        {
                            cc.owner = this;
                            cc.Spawn(null, waveDelay);
                        }
                    }

                    foreach (var cage in lgCages)
                    {
                        var c = SpawnerExtensions.SpawnEntityAt("Arena Cage Large", cage.transform.position, true, false);
                        var cc = c.GetComponent<ArenaCageControl>();
                        if (cc != null)
                        {
                            cc.owner = this;
                            cc.Spawn(null, waveDelay);
                        }
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        public bool waitingOnZote = false;

        protected float CheckWaitStates()
        {
            float waitTime = 0f;
            try
            {
                if (currentState.Name.Contains("Arena "))
                {
                    //TODO: wait
                    {
                        var wait = currentState.GetFirstActionOfType<Wait>();
                        if (wait != null)
                        {
                            waitTime = wait.time.Value;
                        }
                    }
                }
                else if (currentState.Name.Contains("Respawn"))
                {
                    //TODO: wait
                    {
                        var wait = currentState.GetFirstActionOfType<Wait>();
                        if (wait != null)
                        {
                            waitTime = wait.time.Value;
                        }
                    }
                }
                else if (currentState.Name.Contains("Reset"))
                {
                    //TODO: wait
                    {
                        var wait = currentState.GetFirstActionOfType<Wait>();
                        if (wait != null)
                        {
                            waitTime = wait.time.Value;
                        }
                    }
                }
                else if (currentState.Name.Contains("Pause"))
                {
                    //TODO: wait
                    {
                        var wait = currentState.GetFirstActionOfType<Wait>();
                        if (wait != null)
                        {
                            waitTime = wait.time.Value;
                        }
                    }
                }
                else if (currentState.Name.Contains("Gruz Arena"))
                {
                    //TODO: wait?
                    {
                        var wait = currentState.GetFirstActionOfType<Wait>();
                        if (wait != null)
                        {
                            waitTime = wait.time.Value;
                        }
                    }
                }

            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            return waitTime;
        }

        protected virtual void SetFSMToCurrentState()
        {
            spawner.RemoveDeadChildren();
            FSM.SetState(currentState.Name);
        }

        IEnumerator DoBattle()
        {
            SetFSMToCurrentState();
            while (stateIndex < states.Count)
            {
                //if(stateIndex == 2)
                //{
                //    var gruz = states.FirstOrDefault(x => x.Name.Contains("Gruz"));
                //    stateIndex = states.IndexOf(gruz) - 1;
                //}

                bool stateChanged = CheckNextState();

                //make sure battle hasn't ended
                if(battleEnded)
                {
                    break;
                }

                //now handle the next state

                if (stateChanged)
                {
                    if (currentState.Name.Contains("Wave "))
                    {
                        SpawnWave();
                    }
                    else
                    {
                        float waitTime = CheckWaitStates();
                        if (waitTime > 0f)
                            yield return new WaitForSeconds(waitTime);
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            if (battleEnded)
                yield break;
        }
    }

    public class ColoBronze : CustomArena
    {
        public override string name => "Bronze Arena";

        protected override void BuildWaves(PlayMakerFSM fsm)
        {
            HutongGames.PlayMaker.FsmState current = fsm.GetState("Init");
            while (current.Name != "End")
            {
                HutongGames.PlayMaker.FsmState prev = current;
                current = fsm.GetState(current.Transitions[0].ToState);

                if (prev.Name.Contains("Wave ") ||
                    prev.Name.Contains("Init") ||
                    prev.Name.Contains("Arena ") ||
                    prev.Name.Contains("Reset") ||
                    prev.Name.Contains("Respawn") ||
                    prev.Name.Contains("Pause") ||
                    prev.Name.Contains("Gruz"))
                {
                    prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SPAWN").ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<SetIntValue>().ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<IntCompare>().ToList().ForEach(x => x.Enabled = false);
                    states.Add(prev);
                    prev.Transitions = new HutongGames.PlayMaker.FsmTransition[0];
                }
            }
        }
    }

    public class ColoSilver : CustomArena
    {
        public override string name => "Silver Arena";
    }

    public class ColoGold : CustomArena
    {
        public override string name => "Gold Arena";
    }
}
