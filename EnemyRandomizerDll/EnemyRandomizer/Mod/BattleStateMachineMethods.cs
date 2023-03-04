using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using nv;
using UniRx;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    //public static partial class BattleGates
    //{
    //    //public static void OpenGates(string scene)
    //    //{
    //    //    if (SceneGates.TryGetValue(scene, out var gates))
    //    //    {
    //    //        gates.Select(x => GameObjectExtensions.FindGameObject(x))
    //    //            .Where(x => x != null)
    //    //            .Select(x => x.GetComponent<PlayMakerFSM>())
    //    //            .ToList()
    //    //            .ForEach(x =>
    //    //            {
    //    //                x.SendEvent("BG OPEN");
    //    //            });
    //    //    }
    //    //}

    //    //public static void CloseGates(string scene)
    //    //{
    //    //    if (SceneGates.TryGetValue(scene, out var gates))
    //    //    {
    //    //        gates.Select(x => GameObjectExtensions.FindGameObject(x)).Where(x => x != null).Select(x => x.GetComponent<PlayMakerFSM>()).ToList().ForEach(x =>
    //    //        {
    //    //            x.SendEvent("BG CLOSE");
    //    //        });
    //    //    }
    //    //}

    //    //public static void CloseGates()
    //    //{
    //    //    var battleGate = GameObject.Find("Battle Gate");
    //    //    if (battleGate != null)
    //    //    {
    //    //        BattleGates.CloseGates(battleGate.scene.name);
    //    //    }
    //    //}

    //    //public static void OpenGates()
    //    //{
    //    //    var battleGate = GameObject.Find("Battle Gate");
    //    //    if (battleGate != null)
    //    //    {
    //    //        BattleGates.OpenGates(battleGate.scene.name);
    //    //    }
    //    //}
    //}

    public partial class BattleState
    {
        public void Setup(FsmState state)
        {
            var keys = state.Transitions.Select(x => x.EventName).ToList();
            var values = state.Transitions.Select(x => x.ToState).ToList();

            Transitions = keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            var badKeys = Transitions.Where(x => StateMachines.BadStates.Contains(x.Value));
            badKeys.ToList().ForEach(x => Transitions.Remove(x.Key));

            if (Transitions.Count <= 0)
            {
                Dev.Log($"Warning: state:{name} has no transitions!");
            }

            if(state.Name == "End")
            {
                IsEndState = true;
            }
            if (state.Name == "Completed")
            {
                IsEndState = true;
            }
        }

        public static BattleState Build(Scene owner, FsmState state)
        {
            BattleState bs = new BattleState()
            {
                name = state.Name,
                scene = owner,
                actions = new List<BattleAction>()
            };

            bs.Setup(state);

            var fsmActions = StateMachines.GetBattleActions(state);

            bs.actions = fsmActions.Select(x =>
            {
                var atype = StateMachines.PtoA[x.GetType()];

                if(atype == typeof(BattleWave))
                {
                    string stateName = state.Name;
                    int waveIndex = StateMachines.StateNameToBattleWave[stateName];
                    if (BattleManager.StateMachine.Value.BattleWaves.ContainsKey(waveIndex))
                    {
                        var battleWave = BattleManager.StateMachine.Value.BattleWaves[waveIndex];
                        battleWave.Setup(bs, owner, x);
                        return battleWave;
                    }
                    else
                    {
                        var battleWave = (BattleWave)Activator.CreateInstance(atype);
                        battleWave.Setup(bs, owner, x);
                        BattleManager.StateMachine.Value.BattleWaves.Add(waveIndex, battleWave);
                        return battleWave;
                    }
                }

                var inst = (BattleAction)Activator.CreateInstance(atype);
                inst.Setup(bs, owner, x);
                return inst;
            }).ToList();

            return bs;
        }
    }

    public partial class BattleWave
    {
        public void AddEnemy(BattleManagedObject bmo)
        {
            if (bmo.ThisIsSourceObject)
            {
                defaultEnemies.Add(bmo);
                Total.Value++;
            }
            else
            {
                bool isUnique = !defaultEnemies.Any(x => x.originalGameObjectPath == bmo.originalGameObjectPath);

                waveEnemies.Add(bmo);
                if (isUnique)
                    Total.Value++;

                RegisterWaveEnemy(bmo);
            }

            bmo.myWave = this;
        }


        public void RemoveEnemy(BattleManagedObject bmo)
        {
            if (bmo.ThisIsSourceObject)
            {
                if (defaultEnemies.Contains(bmo))
                    defaultEnemies.Remove(bmo);
            }
            else
            {
                if (waveEnemies.Contains(bmo))
                    waveEnemies.Remove(bmo);
            }

            bool isInDefaults = defaultEnemies.Any(x => x.originalGameObjectPath == bmo.originalGameObjectPath);
            bool isInWave = waveEnemies.Any(x => x.originalGameObjectPath == bmo.originalGameObjectPath);

            if (!isInDefaults && !isInWave)
            {
                UnregisterWaveEnemy(bmo);
                Total.Value--;
            }

            bmo.myWave = null;
        }

        public virtual void RegisterWaveEnemy(BattleManagedObject aliveEnemy)
        {
            ActiveWaveEnemies.Add(aliveEnemy);
        }

        public virtual void UnregisterWaveEnemy(BattleManagedObject aliveEnemy)
        {
            ActiveWaveEnemies.Remove(aliveEnemy);
        }

        public virtual void RegisterEnemyDeath(BattleManagedObject deadEnemy)
        {
            ActiveWaveEnemies.Remove(deadEnemy);
            EnemiesDead.Value++;
        }
    }

    public partial class BattleStateMachine
    {
        public void NotifyPlayerEnteredTrigger()
        {
            if (InProgress)
                return;

            PlayerEnteredTrigger.Value = true;
        }

        public virtual void StopMusic()
        {
            if (battleMusicRegion != null)
                battleMusicRegion.gameObject.SetActive(false);
        }

        public virtual void MarkPersistantBoolComplete()
        {
            IsBattleComplete.persistentBoolData.activated = true;
            global::SceneData.instance.SaveMyState(IsBattleComplete.persistentBoolData);
        }

        public virtual void UnlockCameras(bool destroy = true)
        {
            cameraLocks.ForEach(x => x.gameObject.SetActive(false));
            if (destroy)
                cameraLocks.ForEach(x => GameObject.Destroy(x.gameObject));
        }


        public void RegisterEnemy(BattleManagedObject bmo)
        {
            int wave = bmo.GetMyWave();

            if (!BattleWaves.TryGetValue(wave, out var bwave))
            {
                if (wave >= 0)
                {
                    bwave = new BattleWave(wave, bmo.gameObject.scene);
                    BattleWaves.Add(wave, bwave);
                }
                else
                {
                    if (!BattleWaves.ContainsKey(0))
                    {
                        BattleWaves.Add(wave, new BattleWave(0, bmo.gameObject.scene));
                    }

                    var w0 = BattleWaves[0];
                    //add all subwaves to wave 0 for now until we find a case where this doesn't work out..
                    int subwave = -wave;
                    if (!w0.SubWaves.TryGetValue(subwave, out var sw))
                    {
                        sw = new BattleWave(subwave, bmo.gameObject.scene);
                        w0.SubWaves.Add(subwave, sw);
                        sw.ParentWave = w0;
                    }

                    sw.AddEnemy(bmo);
                    return;
                }
            }

            bwave.AddEnemy(bmo);
        }

        public void UnregisterEnemy(BattleManagedObject bmo)
        {
            UnregisterEnemy(bmo, bmo.GetMyWave());
        }

        public void UnregisterEnemy(BattleManagedObject bmo, int oldWave)
        {
            if(oldWave < 0)
            {
                if (!BattleWaves.ContainsKey(0))
                    return;

                int subwave = -oldWave;
                var swave = BattleWaves[0].SubWaves[subwave];
                swave.RemoveEnemy(bmo);

                //if the wave is empty, remove it
                if (swave.WaveSizeOnSetup <= 0)
                {
                    swave.Dispose();
                    BattleWaves[0].SubWaves.Remove(subwave);
                }
                return;
            }

            if (BattleWaves.TryGetValue(oldWave, out var bwave))
            {
                bwave.RemoveEnemy(bmo);

                //if the wave is empty, remove it
                if (bwave.WaveSizeOnSetup <= 0 && oldWave != 0)
                {
                    bwave.Dispose();
                    BattleWaves.Remove(oldWave);
                }
            }
        }
    }



    public static partial class StateMachines
    {
        public static BoxCollider2D GetStartTriggerArea(PlayMakerFSM fsm)
        {
            var result = fsm.GetComponent<BoxCollider2D>();

            if (result == null)
            {
                //some battle scenes have no trigger areas -- this is fine
            }

            return result;
        }

        public static HutongGames.PlayMaker.FsmState GetInitState(PlayMakerFSM fsm)
        {
            var init = fsm.FsmStates.FirstOrDefault(x => x.Name == "Init");
            if (init == null)
            {
                //assume idle is the starting state
                init = fsm.FsmStates.FirstOrDefault(x => x.Name == "Idle");
            }

            return init;
        }

        //public static HutongGames.PlayMaker.FsmState GetStartingState(PlayMakerFSM fsm)
        //{
        //    var start = fsm.FsmStates.FirstOrDefault(x => x.Name == "Start");
        //    if (start == null)
        //    {
        //        //assume idle is the starting state
        //        //init = fsm.FsmStates.FirstOrDefault(x => x.Name == "Idle");
        //    }

        //    return start;
        //}

        public static IEnumerable<HutongGames.PlayMaker.FsmState> GetBattleStates(PlayMakerFSM fsm, FsmState initState)
        {
            return fsm.FsmStates.Where(x => !StateMachines.BadStates.Contains(x.Name) && initState.Name != x.Name);
        }

        public static IEnumerable<HutongGames.PlayMaker.FsmStateAction> GetBattleActions(FsmState battleState)
        {

            return battleState.Actions.Where(x => !StateMachines.BadActions.Contains(x.GetType()));
        }

        public static IEnumerable<HutongGames.PlayMaker.FsmStateAction> GetBattleActions(IEnumerable<HutongGames.PlayMaker.FsmState> battleStates)
        {
            return battleStates.SelectMany(x => x.Actions).Where(x => !StateMachines.BadActions.Contains(x.GetType()));
        }

        public static FsmState GetState(PlayMakerFSM fsm, string name)
        {
            return fsm.GetState(name);
        }

        public static UnityEngine.Audio.AudioMixerSnapshot GetAudioMixerSnapshotFromState(HutongGames.PlayMaker.FsmState state)
        {
            return state.Actions.OfType<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>()
                .Cast<HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot>()
                .Where(x => x != null)
                .Where(x => x.snapshot.Value != null)
                .Select(x => x.snapshot.Value)
                .FirstOrDefault() as UnityEngine.Audio.AudioMixerSnapshot;
        }

        public static int GetBattleEnemiesFromState(HutongGames.PlayMaker.FsmState state)
        {
            return state.Actions.OfType<HutongGames.PlayMaker.Actions.IntCompare>()
                .Cast<HutongGames.PlayMaker.Actions.IntCompare>()
                .Where(x => x != null)
                .Where(x => x.integer1.Name == "Battle Enemies")
                .Select(x => x.integer2.Value)
                .FirstOrDefault();
        }

        public static IEnumerable<HutongGames.PlayMaker.FsmState> GetBattleWaveStates(PlayMakerFSM fsm, HutongGames.PlayMaker.FsmState state)
        {
            return fsm.FsmStates
                        .Where(x => x.Actions
                                    .OfType<IntCompare>()
                                    .Cast<IntCompare>()
                                    .Where(y => y != null)
                                    .Any(y => y.integer1.Name == "Battle Enemies")
                               );
        }

        public static int GetBattleEnemiesFromFsm(PlayMakerFSM fsm)
        {
            return fsm.FsmStates.Sum(x => GetBattleEnemiesFromState(x));
        }

        public static IEnumerable<CameraLockArea> GetCameraLocksFromScene(GameObject battleManagerOwner)
        {
            return battleManagerOwner.GetComponentsFromScene<CameraLockArea>();
        }

        public static IEnumerable<MusicRegion> GetMusicRegionsFromScene(GameObject battleManagerOwner)
        {
            return battleManagerOwner.GetComponentsFromScene<MusicRegion>();
        }

        public static void TryDestroyGameObjectAtPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var go = GameObjectExtensions.FindGameObject(path);
                if (go != null)
                    GameObject.Destroy(go);
            }
        }

        public static void TryDisableGameObjectAtPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var go = GameObjectExtensions.FindGameObject(path);
                if (go != null)
                    go.SetActive(false);
            }
        }

        public static void TryEnableGameObjectAtPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var go = GameObjectExtensions.FindGameObject(path);
                if (go != null)
                    go.SetActive(true);
            }
        }

    }
}
