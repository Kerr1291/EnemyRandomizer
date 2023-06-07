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
        public bool IsPreloading { get; protected set; }
        public PlayMakerFSM manager;

        public RNG rng;
        protected virtual int typeSeedOffset => 0;
        public int seedOffset = 0;
        public int SeedOffset => seedOffset + typeSeedOffset;//change this to mix up the arena rng

        public EntitySpawner spawner;
        public EntitySpawner Spawner
        {
            get
            {
                if(spawner == null)
                {
                    spawner = FSM.gameObject.GetOrAddComponent<EntitySpawner>();
                    spawner.maxChildren = 100;
                }
                return spawner;
            }
        }

        public GameObject audioPlayer;
        public AudioClip colCageAppear;
        public AudioClip colCageOpen;
        public AudioClip colCageDown;

        public Dictionary<string, Queue<GameObject>> preloads = new Dictionary<string, Queue<GameObject>>();
        protected HashSet<ArenaCageControl> queue = new HashSet<ArenaCageControl>();


        public int StateIndex => stateIndex;
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

        public void RetractAllPlats()
        {
            manager.GetComponentsInChildren<PlayMakerFSM>(true).Where(x => x.name.Contains("Colosseum Platform")).ToList().ForEach(x => x.SendEvent("PLAT RETRACT"));
        }

        public void Bronze_ResetWallC()
        {
            if(this is ColoBronze)
            {
                string wallName = "Colosseum Wall C";

                var wall = manager.gameObject.FindGameObjectInChildrenWithName(wallName);
                var wallfsm = wall.GetComponent<PlayMakerFSM>();
                wallfsm.SendEvent("RESET");
            }
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
            seedOffset = PlayerData.instance.GetInt("killsDummy");

            int seedToUse = 0;
            if (EnemyRandomizerDatabase.GetCustomColoSeed() > -1)
            {
                seedToUse = EnemyRandomizerDatabase.GetCustomColoSeed();
            }
            else
            {
                seedToUse = EnemyRandomizerDatabase.GetPlayerSeed();
            }

            rng = new RNG(seedToUse + SeedOffset);
            preKilledEnemies = 0;
            battleStarted = false;
            battleEnded = false;
            BattleScene = scene;
            isCustomArena = true;

            SetupManager();

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

            if(manager != null)
            {
                try
                {
                    if (name == "Bronze Arena")
                    {
                        var goName = "Wave 29";
                        List<GameObject> cages = GetCages(goName);
                        PreloadZoteWave(cages.First());
                    }
                }
                catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
                
            }
        }

        public void PlayCageUp()
        {
            PlayMakerFSM.BroadcastEvent("AUDIO CAGE UP");
        }

        public void PlayCageOpen()
        {
            PlayMakerFSM.BroadcastEvent("AUDIO CAGE OPEN");
        }

        public void PreloadCageEntity(Vector2 pos, string cage, string cageThing, int numCages = 1, int numThings = 1)
        {
            manager.StartCoroutine(DoPreloadCageEntity(pos, cage, cageThing, numCages, numThings, 5));
        }

        protected virtual IEnumerator DoPreloadCageEntity(Vector2 pos, string cage, string cageThing, int numCages, int numThings, int batchsize = 5)
        {
            //needs "IsPreloading" to be true when spawning enemies that have special colo logic
            IsPreloading = true;
            if (!preloads.TryGetValue(cage, out var preloadedCages))
            {
                preloadedCages = new Queue<GameObject>();
                preloads.Add(cage, preloadedCages);
            }

            for (int i = 0; i < numCages; i += batchsize)
            {
                for (int j = 0; j < batchsize; ++j)
                {
                    if ((i + j) >= numCages)
                        break;

                    var c = SpawnerExtensions.SpawnEntityAt(cage, pos, true, false);
                    c.GetComponent<PlayMakerFSM>().enabled = false;
                    preloadedCages.Enqueue(c);
                    var cc = c.GetComponent<ArenaCageControl>();
                    if (cc != null)
                    {
                        cc.owner = this;
                        yield return cc.DoPreload(cageThing, numThings);
                    }
                    c.SetActive(false);
                    c.GetComponent<PlayMakerFSM>().enabled = true;
                    yield return new WaitForEndOfFrame();
                }
            }
            IsPreloading = false;
        }

        public GameObject SpawnCageEntity(Vector2 pos, string cage, string cageThing, int thingsToSpawn = 1, float waveDelay = 1f, bool fling = false)
        {
            if (preloads.TryGetValue(cage, out var preloadedCages))
            {
                GameObject c = null;
                if (preloadedCages.Count > 0)
                {
                    c = preloadedCages.Dequeue();
                    c.gameObject.SafeSetActive(true);
                }
                else
                {
                    c = SpawnerExtensions.SpawnEntityAt(cage, pos, true, false);
                }

                if (preloadedCages.Count <= 0)
                    preloads.Remove(cage);

                var cc = c.GetComponent<ArenaCageControl>();
                if (cc != null)
                {
                    if (cageThing == "Zote Boss")
                    {
                        cc.GetComponent<ArenaCageControl>().onSpawn += OnZoteSpawn;
                    }
                    cc.owner = this;
                    cc.flingRandomly = fling;
                    cc.SpawnBatch(cageThing, null, waveDelay, thingsToSpawn);
                }

                return c;
            }
            else
            {
                var c = SpawnerExtensions.SpawnEntityAt(cage, pos, true, false);
                var cc = c.GetComponent<ArenaCageControl>();
                if (cc != null)
                {
                    if (cageThing == "Zote Boss")
                    {
                        cc.GetComponent<ArenaCageControl>().onSpawn += OnZoteSpawn;
                    }
                    cc.owner = this;
                    cc.flingRandomly = fling;

                    if (thingsToSpawn == 1)
                        cc.Spawn(cageThing, null, waveDelay);
                    else
                        cc.SpawnBatch(cageThing, null, waveDelay, thingsToSpawn);
                }
                return c;
            }
        }

        public GameObject Spawn(Vector2 pos, string enemy = null, string originalEnemy = null, bool preload = false)
        {
            var spawnedObject = Spawner.SpawnCustomArenaEnemy(pos, enemy, originalEnemy, rng);            
            if(preload)
            {
                Spawner.UntrackObject(spawnedObject);
                spawnedObject.SafeSetActive(false); //disable object for now
            }
            else
            {

            }
            return spawnedObject;
        }

        public void AddExternalObjectToArena(GameObject enemy)
        {
            Spawner.TrackObject(enemy);
            enemy.SafeSetActive(true);
        }

        protected virtual void StartBattle()
        {
            if(currentState == null)
            {
                Dev.LogError($"currentState is null and this should never happen! statesCount:{states.Count} currentIndex:{stateIndex}");
            }

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

            if (waitingOnLancer)
                return false;

            bool stateChanged = true;
            try
            {
                if (currentState.Name.Contains("Wave ") 
                    || currentState.Name.Contains("Wave 29 Zote")
                    || currentState.Name.Contains("Lancer Shake 1"))
                {
                    if (Spawner.Children.Count <= 0 && queue.Count <= 0)
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
                else if (currentState.Name.Contains("Cheer"))
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
                else if (currentState.Name.Contains("Hopper Arena"))
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
                else if (currentState.Name.Contains("Gruz Arena"))
                {
                    {
                        RetractAllPlats();
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

        protected virtual int IsObbleWave()
        {
            if (SceneName == "Room_Colosseum_Silver")
            {
                if (currentState.Name.Contains("Wave 26 Obble"))
                    return 26;

                if (currentState.Name.Contains("Wave 27 Obble"))
                    return 27;

                if (currentState.Name.Contains("Wave 28 Obble"))
                    return 28;

                if (currentState.Name.Contains("Wave 29 Obble"))
                    return 29;

                if (currentState.Name.Contains("Wave 30 Obble"))
                    return 30;
            }

            return -1;
        }

        protected virtual bool IsLancerWave()
        {
            if(SceneName == "Room_Colosseum_Gold")
            {
                if (currentState.Name.Contains("Lancer Shake 1"))
                    return true;
            }
            return false;
        }


        protected virtual int GetWaveIndex()
        {
            string nameToParse = currentState.Name.Split(' ').Last();

            if (IsZoteWave())
                return 29;

            if (Spawner.Children.Count <= 0)
            {
                waitingOnLancer = IsLancerWave();
                if (waitingOnLancer)
                    return 51;
            }

            if (currentState.Name.Contains("Wave 24 Loop"))
                return 24;

            int obbleIndex = IsObbleWave();
            if (obbleIndex > 0)
            {
                return obbleIndex;
            }

            int waveIndex = int.Parse(nameToParse);
            return waveIndex;
        }

        protected virtual List<GameObject> GetCages(string goName)
        {
            return FSM.gameObject.FindGameObjectInDirectChildren("Waves").FindGameObjectInDirectChildren(goName).GetDirectChildren();
        }

        protected virtual float GetWaveDelay()
        {
            //check the wake events for the mage waves
            if (SceneName == "Room_Colosseum_Gold")
            {
                if(currentState.Name.Contains("Wave 25")
                    || currentState.Name.Contains("Wave 26")
                    || currentState.Name.Contains("Wave 27")
                    || currentState.Name.Contains("Wave 28"))
                {
                    return currentState.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "WAKE").FirstOrDefault().delay.Value;
                }
            }

            return currentState.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SPAWN").FirstOrDefault().delay.Value;
        }

        protected virtual void PreloadZoteWave(GameObject zoteCage)
        {
            PreloadCageEntity(zoteCage.transform.position, "Arena Cage Zote", "Zote Boss", 1, 57);
        }

        protected virtual void SpawnZoteWave(GameObject zoteCage, float waveDelay)
        {
            SpawnCageEntity(zoteCage.transform.position, "Arena Cage Zote", "Zote Boss", 57, waveDelay, true);
        }

        List<GameObject> empty = new List<GameObject>();

        protected virtual void SpawnWave()
        {
            try
            {
                int index = GetWaveIndex();
                string goName = "Wave " + index;

                if (currentState.Name.Contains("Wave 24 Loop"))
                {
                    goName = "Wave 24a";
                }

                if(waitingOnLancer)
                {
                    goName = "Lobster Lancer";
                }

                List<GameObject> cages = empty;
                float waveDelay = 0f;

                if (!waitingOnLancer)
                {
                    cages = GetCages(goName);
                    waveDelay = GetWaveDelay();
                }

                if (isZoteWave)
                {
                    waitingOnZote = true;
                    SpawnZoteWave(cages.First(), waveDelay);
                }
                else if(waitingOnLancer)
                {
                    //spawn is taken by the FSM of the state
                }
                else
                {
                    var smCages = cages.Where(x => x.name.Contains("Small"));
                    var lgCages = cages.Where(x => x.name.Contains("Large"));

                    foreach (var cage in smCages)
                    {
                        string enemyInside = ColosseumCageSmallPrefabConfig.GetEnemyPrefabNameInsideCage(cage);

                        var c = SpawnerExtensions.SpawnEntityAt("Arena Cage Small", cage.transform.position, true, false);
                        var cc = c.GetComponent<ArenaCageControl>();
                        if (cc != null)
                        {
                            cc.owner = this;
                            cc.Spawn(null, enemyInside, waveDelay);
                        }
                    }

                    foreach (var cage in lgCages)
                    {
                        string enemyInside = ColosseumCageLargePrefabConfig.GetEnemyPrefabNameInsideCage(cage);

                        var c = SpawnerExtensions.SpawnEntityAt("Arena Cage Large", cage.transform.position, true, false);
                        var cc = c.GetComponent<ArenaCageControl>();
                        if (cc != null)
                        {
                            cc.owner = this;
                            cc.Spawn(null, enemyInside, waveDelay);
                        }
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"{e.Message} {e.StackTrace}"); }
        }

        public bool waitingOnZote = false;
        public bool waitingOnLancer = false;

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
            Spawner.RemoveDeadChildren();
            FSM.SetState(currentState.Name);
        }

        IEnumerator DoBattle()
        {
            SetFSMToCurrentState();
            while (stateIndex < states.Count)
            {
                //if (stateIndex == 2)
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

        protected override int typeSeedOffset => 0;

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

        protected override int typeSeedOffset => 10000;

        protected override void BuildWaves(PlayMakerFSM fsm)
        {
            HutongGames.PlayMaker.FsmState current = fsm.GetState("Init");
            while (current.Name != "End")
            {
                HutongGames.PlayMaker.FsmState prev = current;
                current = fsm.GetState(current.Transitions[0].ToState);

                if (prev.Name.Contains("Wave ") ||
                    prev.Name.Contains("Init") ||
                    prev.Name.Contains("Cheer") ||
                    prev.Name.Contains("Arena ") ||
                    prev.Name.Contains("Hopper Arena") ||
                    prev.Name.Contains("Reset") ||
                    prev.Name.Contains("Respawn") ||
                    prev.Name.Contains("Pause") ||
                    prev.Name.Contains("Spikes"))
                {
                    prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "WAKE").ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SPAWN").ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<SetIntValue>().ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<IntCompare>().ToList().ForEach(x => x.Enabled = false);
                    states.Add(prev);
                    prev.Transitions = new HutongGames.PlayMaker.FsmTransition[0];
                }
            }
        }
    }

    public class ColoGold : CustomArena
    {
        public override string name => "Gold Arena";

        protected override int typeSeedOffset => 100000;

        protected override void BuildWaves(PlayMakerFSM fsm)
        {
            HutongGames.PlayMaker.FsmState current = fsm.GetState("Init");
            while (current.Name != "End")
            {
                HutongGames.PlayMaker.FsmState prev = current;
                current = fsm.GetState(current.Transitions[0].ToState);

                if (prev.Name.Contains("Wave ") ||
                    prev.Name.Contains("Init") ||
                    prev.Name.Contains("Cheer") ||
                    prev.Name.Contains("Arena ") ||
                    prev.Name.Contains("Reset") ||
                    prev.Name.Contains("Respawn") ||
                    prev.Name.Contains("Pause") ||

                    prev.Name.Contains("Garpedes") ||
                    prev.Name.Contains("Spike Pit") ||
                    prev.Name.Contains("Ceiling") ||
                    prev.Name.Contains("GC") ||
                    prev.Name.Contains("Walls") ||
                    prev.Name.Contains("Shake") ||

                    prev.Name.Contains("Spikes"))
                {
                    prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "WAKE").ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<SendEventByName>().Where(x => x.sendEvent.Value == "SPAWN").ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<SetIntValue>().ToList().ForEach(x => x.Enabled = false);
                    prev.GetActions<IntCompare>().ToList().ForEach(x => x.Enabled = false);
                    states.Add(prev);
                    prev.Transitions = new HutongGames.PlayMaker.FsmTransition[0];

                    if (prev.Name.Contains("Lancer Shake 1"))
                    {
                        prev.AddCustomAction(() => {
                            var originLancer = new Vector2(113f, 7.5f);
                            var originLobster = new Vector2(116f, 7.5f);

                            var boss1 = SpawnerExtensions.GetRandomPrefabNameForArenaBoss(rng);
                            var boss2 = SpawnerExtensions.GetRandomPrefabNameForArenaBoss(rng);

                            SpawnerExtensions.SpawnEntityAt("Death Explode Boss", originLancer, true, false);
                            SpawnerExtensions.SpawnEntityAt("Death Explode Boss", originLobster, true, false);

                            var lancer = Spawn(originLancer, boss1, "Lancer");
                            var lobster = Spawn(originLobster, boss2, "Lobster");

                            TuneBoss(lancer);
                            TuneBoss(lobster);

                            lancer.SafeSetActive(true);
                            lobster.SafeSetActive(true);

                            waitingOnLancer = false;
                        });
                    }
                }
            }
        }

        protected virtual void TuneBoss(GameObject boss)
        {
            //make sure they have boss hp
            var soc = boss.GetComponent<DefaultSpawnedEnemyControl>();
            if(soc != null)
            {
                int minBossHP = 500;
                if (soc.defaultScaledMaxHP < minBossHP)
                {
                    soc.defaultScaledMaxHP = minBossHP;
                    soc.CurrentHP = minBossHP;
                }
            }

            {
                var bossType = boss.GetComponent<ZoteBossControl>();
                if (bossType != null)
                {
                    boss.ScaleObject(2.0f);
                    boss.ScaleAudio(0.7f);
                    soc.PhysicsBody.gravityScale = 0.5f;
                }
            }

            {
                var bossType = boss.GetComponent<SuperSpitterControl>();
                if (bossType != null)
                {
                    bossType.MakeSuperBoss();
                    boss.ScaleObject(2.2f);
                }
            }

            {
                var bossType = boss.GetComponent<GiantHopperControl>();
                if (bossType != null)
                {
                    //TODO
                    boss.ScaleObject(1.25f);
                }
            }

            {
                var bossType = boss.GetComponent<MantisHeavyControl>();
                if (bossType != null)
                {
                    //TODO
                    boss.ScaleObject(1.5f);
                }
            }

            {
                var bossType = boss.GetComponent<LesserMawlekControl>();
                if (bossType != null)
                {
                    //TODO
                    boss.ScaleObject(1.5f);
                }
            }

            {
                var bossType = boss.GetComponent<MageKnightControl>();
                if (bossType != null)
                {
                    bossType.canFakeout = true;
                    boss.ScaleObject(1.5f);
                }
            }

            {
                var bossType = boss.GetComponent<GorgeousHuskControl>();
                if (bossType != null)
                {
                    bossType.MakeSuperHusk();
                }
            }

            {
                var bossType = boss.GetComponent<GreatShieldZombieControl>();
                if (bossType != null)
                {
                    boss.ScaleObject(2.0f);
                    boss.ScaleAudio(0.7f);
                    //TODO:
                }
            }

            {
                var bossType = boss.GetComponent<MossKnightControl>();
                if (bossType != null)
                {
                    boss.ScaleObject(2.0f);
                }
            }

            {
                var bossType = boss.GetComponent<MossKnightFatControl>();
                if (bossType != null)
                {
                    boss.ScaleObject(2.5f);
                }
            }

            {
                var bossType = boss.GetComponent<RoyalGaurdControl>();
                if (bossType != null)
                {
                    bossType.MakeElite();
                }
            }
        }
    }
}

