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
    public class CustomMiniArena : BattleStateMachine
    {
        public override string name => this.GetType().Name;
        public virtual string FSMName => "Battle Control";
        public virtual string InitStateName => "Init";
        public virtual string StartBattleStateName => "Detect";

        public RNG rng;
        protected virtual int typeSeedOffset => 0;
        public int seedOffset = 0;
        public int SeedOffset => seedOffset + typeSeedOffset;//change this to mix up the arena rng

        public EntitySpawner spawner;
        public EntitySpawner Spawner
        {
            get
            {
                if (spawner == null)
                {
                    spawner = FSM.gameObject.GetOrAddComponent<EntitySpawner>();
                    spawner.maxChildren = 100;
                }
                return spawner;
            }
        }

        public virtual Vector2 hazardRespawnPos => FSM.transform.position;//not a good one

        public int StateIndex => stateIndex;
        protected int stateIndex = 0;
        protected HutongGames.PlayMaker.FsmState currentState => states == null || states.Count <= 0 ? null : states[stateIndex];
        protected List<HutongGames.PlayMaker.FsmState> states = new List<HutongGames.PlayMaker.FsmState>();

        public HashSet<string> spawnedObjects = new HashSet<string>();

        public Dictionary<string, //wave name
               Dictionary<string, //enemy dict key
                   Vector2 //spawn point
                   >> arenaWaves = new Dictionary<string, Dictionary<string, Vector2>>();

        public virtual bool ArenaCleared
        {
            get => FSM.GetComponent<PersistentBoolItem>().persistentBoolData.activated;
            set { 
                FSM.GetComponent<PersistentBoolItem>().persistentBoolData.activated = value;
                global::SceneData.instance.SaveMyState(FSM.GetComponent<PersistentBoolItem>().persistentBoolData);
            }
        }

        protected virtual void BuildWaves(PlayMakerFSM fsm)
        {
            Spawner.topLeft = topLeft;
            Spawner.botRight = botRight;

            Dev.Log($"{Dev.FunctionHeader(0)}");
            fsm.gameObject.GetComponentsInChildren<HealthManager>(true).Where(x => x.gameObject.activeInHierarchy && x.gameObject.IsVisible()).ToList().ForEach(x => AddExternalObjectToArena(x.gameObject));

            if (arenaWaves == null)
                arenaWaves = new Dictionary<string, Dictionary<string, Vector2>>();

            foreach (var wave in fsm.gameObject.GetDirectChildren())
            {
                if(!arenaWaves.TryGetValue(wave.name, out var waveData ))
                {
                    waveData = new Dictionary<string, Vector2>();

                    arenaWaves.Add(wave.name, waveData);
                }

                foreach(var go in wave.GetComponentsInChildren<HealthManager>(true))
                {
                    if (!go.gameObject.IsDatabaseObject())
                        continue;

                    string goPath = go.gameObject.GetSceneHierarchyPath();
                    string dbKey = go.gameObject.GetDatabaseKey();

                    if (spawnedObjects.Contains(goPath))
                    {
                        continue;
                    }

                    var spawnPos = GetSpawnPosition(SceneName, dbKey, wave.name, go.name, go.gameObject);
                    waveData.Add(goPath + "+" + dbKey, spawnPos);

                    go.gameObject.SetActive(false);

                    SpawnerExtensions.DestroyObject(go.gameObject);
                }
            }
        }

        protected virtual Vector2 GetSpawnPosition(string scene, string dbKey, string wave, string sourceName, GameObject source)
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            return source.transform.position;
        }

        public override void Dispose()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            battleStarted = false;
            battleEnded = false;
            isCustomArena = false;
            isMiniArena = false;
            ((IDisposable)disposables).Dispose();
        }

        public override void Setup(Scene scene, PlayMakerFSM fsm)
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                if (fsm.name != FSMName)
                {
                    FSM = fsm.gameObject.LocateMyFSM(FSMName);
                }
                else
                {
                    fsm = FSM;
                }

                seedOffset = scene.name.GetHashCode();

                int seedToUse = EnemyRandomizerDatabase.GetPlayerSeed();

                rng = new RNG(seedToUse + SeedOffset);
                preKilledEnemies = 0;
                battleStarted = false;
                battleEnded = false;
                BattleScene = scene;
                isMiniArena = true;
                isCustomArena = false;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            try
            {
                HookStartBattle();
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            try
            {
                BuildWaves(FSM);
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void HookStartBattle()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                FSM.GetState(StartBattleStateName).AddCustomAction(() => { StartBattle(); });
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void StartBattle()
        {
            if (battleStarted)
                return;

            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                if (currentState == null)
                {
                    Dev.LogError($"currentState is null and this should never happen! statesCount:{states.Count} currentIndex:{stateIndex}");
                }
                OnBattleStarted();
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        public bool IsHeroInStartArea()
        {
            return false;
        }

        protected override void OnBattleStarted()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                CloseGates(true);
                BattleManager.Instance.Value.StartCoroutine(DoBattle());
                battleStarted = true;
                battleEnded = false;
                GameManager.instance.playerData.hazardRespawnLocation = hazardRespawnPos;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void EndBattle()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            OnBattleEnded();
        }

        protected override void OnBattleEnded()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                base.OnBattleEnded();
                EndArena();
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        public void EndArena()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                ArenaCleared = true;
                OpenGates();
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual void SetFSMToNextState()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                if (stateIndex >= states.Count)
                {
                    Dev.LogError("Somehow this was invoked beyond the end? Trying to end again...");
                    EndBattle();
                    return;
                }

                stateIndex++;
                if (stateIndex >= states.Count)
                {
                    EndBattle();
                }
                else
                {
                    SetFSMToCurrentState();
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
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
            Dev.Log($"{Dev.FunctionHeader(0)}");
            if (Spawner.Children.Count > 0)
                return false;

            bool stateChanged = true;
            try
            {
                if (currentState.Name.Contains("Pause"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Wave "))
                {
                    if (Spawner.Children.Count <= 0)
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
                else if (currentState.Name.Contains("Spike Pit"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Ceiling"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Garpedes"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("GC"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Walls"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Shake"))
                {
                    {
                        SetFSMToNextState();
                    }
                }
                else if (currentState.Name.Contains("Spikes"))
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

        protected virtual float GetWaveDelay()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            //var waitAction = currentState.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SPAWN").FirstOrDefault();

            return 1f;
        }

        public GameObject Spawn(string originalPath, Vector2 pos, string enemy = null, string originalEnemy = null)
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                var spawnedObject = Spawner.SpawnCustomArenaEnemy(pos, enemy, originalEnemy, rng);
                spawnedObjects.Add(originalPath);
                return spawnedObject;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            return null;
        }

        public void AddExternalObjectToArena(GameObject enemy)
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                spawnedObjects.Add(enemy.GetSceneHierarchyPath());

                Spawner.TrackObject(enemy);
                enemy.SafeSetActive(true);
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        protected virtual float SpawnWave()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                var waveName = currentState.Name;
                Dictionary<string, Vector2> waveData = null;
                if (arenaWaves.ContainsKey(waveName))
                {
                    waveData = arenaWaves[waveName];
                }

                //nothing to spawn
                if (waveData == null || waveData.Count <= 0)
                    return 0f;

                float waveDelay = GetWaveDelay();

                foreach (var spawn in waveData)
                {
                    Vector2 spawnPos = spawn.Value;
                    string goPath = spawn.Key.Split('+')[0];
                    string databaseKey = spawn.Key.Split('+')[1];
                    Dev.Log($"{name}: Spawning Random Battle Arena Enemy for {waveName} to replace {spawn} at {spawnPos}");
                    Spawn(goPath, spawnPos, null, databaseKey);
                }

                return waveDelay;
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }

            return 0f;
        }

        protected float CheckWaitStates()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            float waitTime = 1f;
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
                else if (currentState.Name.Contains("Cheer"))
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
                else if (currentState.Name.Contains("Hopper Arena"))
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
                else if (currentState.Name.Contains("Garpedes"))
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
                else if (currentState.Name.Contains("Spike Pit"))
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
                else if (currentState.Name.Contains("Ceiling"))
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
                else if (currentState.Name.Contains("GC"))
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
                else if (currentState.Name.Contains("Walls"))
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
                else if (currentState.Name.Contains("Shake"))
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
                else if (currentState.Name.Contains("Spikes"))
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
                else if (currentState.Name.Contains("Wave "))
                {
                    waitTime = 1f;
                }

            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
            return waitTime;
        }

        protected virtual void SetFSMToCurrentState()
        {
            try
            {
                Dev.Log($"{Dev.FunctionHeader(0)}");
                Spawner.RemoveDeadChildren();
                FSM.SetState(currentState.Name);
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        IEnumerator DoBattle()
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            SetFSMToCurrentState();
            while (stateIndex < states.Count)
            {
                bool stateChanged = CheckNextState();

                yield return new WaitForSeconds(1f);

                //make sure battle hasn't ended
                if (battleEnded)
                {
                    break;
                }

                //now handle the next state

                if (stateChanged)
                {
                    if (currentState.Name.Contains("Wave "))
                    {
                        float waitTime = SpawnWave();
                        if (waitTime > 0f)
                            yield return new WaitForSeconds(waitTime);
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


    public class Crossroads_08Arena : CustomMiniArena
    {
        public override Vector2 hazardRespawnPos => new Vector2(20.5f, 12.2f);

        public override Vector2 topLeft => new Vector2(21f, 27f);
        public override Vector2 botRight => new Vector2(40f, 7f);

        protected override void BuildWaves(PlayMakerFSM fsm)
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            base.BuildWaves(fsm);

            try
            {
                HutongGames.PlayMaker.FsmState current = fsm.GetState(InitStateName);
                while (current.Name != "End")
                {
                    HutongGames.PlayMaker.FsmState prev = current;
                    current = fsm.GetState(current.Transitions[0].ToState);

                    if (prev.Name.Contains("Wave ") ||
                        prev.Name.Contains("Init") ||
                        prev.Name.Contains("End Wait ") ||
                        //prev.Name.Contains("Reset") ||
                        //prev.Name.Contains("Respawn") ||
                        prev.Name.Contains("Pause")
                        //prev.Name.Contains("Gruz")
                        )
                    {
                        prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SUMMON").ToList().ForEach(x => x.Enabled = false);
                        prev.GetActions<SetIntValue>().ToList().ForEach(x => x.Enabled = false);
                        prev.GetActions<IntCompare>().ToList().ForEach(x => x.Enabled = false);
                        states.Add(prev);
                        prev.Transitions = new HutongGames.PlayMaker.FsmTransition[0];
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }
    }



    public class Crossroads_22Arena : CustomMiniArena
    {
        public override Vector2 hazardRespawnPos => new Vector2(72.0f, 23.0f);

        public override Vector2 topLeft => new Vector2(63f, 30f);
        public override Vector2 botRight => new Vector2(81f, 5f);

        protected override void BuildWaves(PlayMakerFSM fsm)
        {
            Dev.Log($"{Dev.FunctionHeader(0)}");
            base.BuildWaves(fsm);

            Dev.Log($"{Dev.FunctionHeader(0)}");
            try
            {
                HutongGames.PlayMaker.FsmState current = fsm.GetState(InitStateName);
                while (current.Name != "End")
                {
                    current.Transitions = current.Transitions.Where(x => x.EventName != "ACTIVATE").ToArray();

                    HutongGames.PlayMaker.FsmState prev = current;
                    current = fsm.GetState(current.Transitions[0].ToState);

                    if (prev.Name.Contains("Wave ") ||
                        prev.Name.Contains("Init") ||
                        prev.Name.Contains("Blob Open ") ||
                        //prev.Name.Contains("Reset") ||
                        //prev.Name.Contains("Respawn") ||
                        prev.Name.Contains("Pause")
                        //prev.Name.Contains("Gruz")
                        )
                    {
                        prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SUMMON").ToList().ForEach(x => x.Enabled = false);
                        prev.GetActions<SetIntValue>().ToList().ForEach(x => x.Enabled = false);
                        prev.GetActions<IntCompare>().ToList().ForEach(x => x.Enabled = false);
                        states.Add(prev);
                        prev.Transitions = new HutongGames.PlayMaker.FsmTransition[0];
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }
    }
}

