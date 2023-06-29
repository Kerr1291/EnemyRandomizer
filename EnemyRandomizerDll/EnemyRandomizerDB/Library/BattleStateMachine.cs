using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

using UniRx;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using On;
using Satchel;

namespace EnemyRandomizerMod
{
    public class BattleStateMachine : IDisposable
    {
        public virtual string name => SceneName + " Battle Controller";

        public Scene BattleScene { get; protected set; }
        public string SceneName { get { return BattleScene.name; } }
        public PlayMakerFSM FSM { get; protected set; }
        public BoxCollider2D FSMAREA { get; protected set; }

        protected CompositeDisposable disposables = new CompositeDisposable();

        public FsmState waitingState = null;
        public bool waitingOnBossKill = false;
        public string waitingEvent = null;
        public bool battleStarted = false;
        public bool battleEnded = false;
        public int preKilledEnemies = 0;

        public bool isCustomArena = false;
        public bool isMiniArena = false;
        public bool useBoxToForceStart = false;

        public virtual Vector2 topLeft => Vector2.zero; //override these
        public virtual Vector2 botRight => Vector2.zero;//override these


        public bool HeroInBox(Vector2 topleft, Vector2 bottomRight)
        {
            var point = HeroController.instance.transform.position.ToVec2();

            // Check if the point's X coordinate is within the box's X range
            bool withinXRange = point.x >= topleft.x && point.x <= bottomRight.x;

            // Check if the point's Y coordinate is within the box's Y range
            bool withinYRange = point.y <= topleft.y && point.y >= bottomRight.y;

            // Return true if the point is within both the X and Y ranges, indicating it is inside the box
            return withinXRange && withinYRange;
        }

        public virtual bool IsHeroInBattleArea()
        {
            return HeroInBox(topLeft, botRight);
        }

        public void ForceBattleStart()
        {
            if(SceneName == "Crossroads_04")
            {
                FSM.SetState("Detect");
                FSM.SendEvent("START");
            }

            OnBattleStarted();
        }

        public void ForceBattleEnd()
        {
            OnBattleEnded();
        }

        public virtual void Dispose()
        {
            preKilledEnemies = 0;
            battleStarted = false;
            battleEnded = false;
            ((IDisposable)disposables).Dispose();

            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEvent -= Fsm_Event_FsmEvent;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEventTarget_string -= Fsm_Event_FsmEventTarget_string;
        }

        public virtual void Setup(Scene scene, PlayMakerFSM fsm)
        {
            preKilledEnemies = 0;
            battleStarted = false;
            battleEnded = false;
            BattleScene = scene;
            FSM = fsm;
            FSMAREA = fsm.GetComponent<BoxCollider2D>();

            On.HutongGames.PlayMaker.Fsm.Event_FsmEvent -= Fsm_Event_FsmEvent;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEventTarget_string -= Fsm_Event_FsmEventTarget_string;

            On.HutongGames.PlayMaker.Fsm.Event_FsmEvent += Fsm_Event_FsmEvent;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEventTarget_string += Fsm_Event_FsmEventTarget_string;

            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
        }

        //IEnumerator FixFlyCorpse()
        //{
        //    GameObject burster;
        //    for (; ; )
        //    {
        //        yield return new WaitForEndOfFrame();
        //        if (SceneName != "Crossroads_04")
        //            yield break;

        //        burster = GameObject.Find("Corpse Big Fly Burster(Clone)");
        //        if (burster != null && burster.LocateMyFSM("burster").enabled)
        //            break;
        //    }

        //    if (SpawnedObjectControl.VERBOSE_DEBUG)
        //        Dev.Log("Found burster corpse");
        //    if (SceneName != "Crossroads_04")
        //        yield break;

        //    var fsm = burster.LocateMyFSM("burster");
        //    var spawn = fsm.GetState("Spawn Flies 2");
        //    spawn.AddCustomAction(() => {
        //        SpawnerExtensions.SpawnEnemyForEnemySpawner(burster.transform.position, true, "Fly");
        //        SpawnerExtensions.SpawnEnemyForEnemySpawner(burster.transform.position, true, "Fly");
        //        SpawnerExtensions.SpawnEnemyForEnemySpawner(burster.transform.position, true, "Fly");
        //        SpawnerExtensions.SpawnEnemyForEnemySpawner(burster.transform.position, true, "Fly");
        //        SpawnerExtensions.SpawnEnemyForEnemySpawner(burster.transform.position, true, "Fly");
        //        SpawnerExtensions.SpawnEnemyForEnemySpawner(burster.transform.position, true, "Fly");
        //        GameManager.instance.BroadcastFSMEventAfterTime("END", 4f);
        //    });
        //    if (SpawnedObjectControl.VERBOSE_DEBUG)
        //        Dev.Log("Burster corpse setup complete");
        //}

        protected virtual void OnBattleStarted()
        {
            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log("BATTLE STARTED");

            //if (SceneName == "Crossroads_04")
            //{
            //    if (GameManager.instance.playerData.giantFlyDefeated == false)
            //    {
            //        GameManager.instance.StartCoroutine(FixFlyCorpse());
            //    }
            //}

            battleStarted = true;
            battleEnded = false;
            BattleManager.Instance.Value.StartCoroutine(ForceProgressWatchdog(12f));
            if (FSMAREA != null)
            {
                var battleArea = FSMAREA.bounds;
                var bmos = GameObject.FindObjectsOfType<SpawnedObjectControl>().ToList();
                var outsideArea = bmos.Where(x =>
                {
                    var pos = x.transform.position;
                    var localPos = FSMAREA.transform.InverseTransformPoint(pos);
                    bool isInside = battleArea.Contains(localPos);

                    return !isInside;

                });

                outsideArea.ToList().ForEach(x =>
                {
                    var dsec = x.GetComponent<DefaultSpawnedEnemyControl>();
                    if (dsec != null)
                    {
                        var losPos = dsec.gameObject.GetRandomPositionInLOSofPlayer(3f, 20f, 2f);
                        x.transform.position = losPos;
                        var poob = dsec.GetComponent<PreventOutOfBounds>();
                        if (poob != null)
                        {
                            poob.ForcePosition(losPos);
                        }
                    }
                    else
                    {
                        RNG rng = new RNG();
                        rng.Reset();
                        var point = rng.Rand(battleArea.min, battleArea.max);
                        var worldPoint = FSMAREA.transform.TransformPoint(point);

                        x.transform.position = worldPoint;
                    }
                });
            }
            else
            {
                if (GameObject.FindObjectsOfType<HealthManager>().Select(x => x.gameObject).Any(x => x.IsBoss()))
                {
                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.Log("SKIP KILLING EVERYTHING BEFORE ARENA -- THIS IS A BOSS ARENA");
                    return;
                }

                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.LogWarning("Battle has started and we have no collider to check if all previous battle enemies are in the arena!");

                var bmos = SpawnedObjectControl.GetAllBattle.ToList();
                bmos.ForEach(x =>
                {
                    if (x.gameObject.activeInHierarchy
                    && !x.gameObject.IsDisabledBySavedGameState()
                    && x.gameObject.HasReplacedAnObject()
                    && x.gameObject.IsVisible())
                    {
                        if (SpawnedObjectControl.VERBOSE_DEBUG)
                            Dev.Log("Force killing pre-battle enemies for now until I implement a solution to check if they start inside the arena");
                        x.gameObject.KillObjectNow();
                    }
                });
            }
        }

        IEnumerator ForceProgressWatchdog(float timer = 10f)
        {
            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log("WATCHDOG STARTED");

            float updateRate = 0f;
            float t = 0f;
            for (; ; )
            {
                if(GameObject.FindObjectsOfType<HealthManager>().Select(x => x.gameObject).Any(x => x.IsBoss()))
                {
                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.Log("WATCHDOG CANCELED -- THIS IS A BOSS ARENA");
                    yield break;
                }

                if(battleEnded)
                {
                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.Log("WATCHDOG ENDED -- BATTLE COMPLETE");
                    yield break;
                }

                if (!battleStarted)
                {
                    if(SceneName == "Ruins2_09")
                    {
                        if (FSM != null)
                            UnlockCameras(GetCameraLocksFromScene(FSM.gameObject));
                    }


                    if (SceneName == "Ruins3_10")
                    {
                        if (FSM != null)
                            UnlockCameras(GetCameraLocksFromScene(FSM.gameObject));
                    }

                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.Log("WATCHDOG CANCELED");
                    yield break;
                }

                if (battleStarted && preKilledEnemies > 0)
                {
                    DecrementBattleEnemies();
                    preKilledEnemies--;
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                if (updateRate > 1f)
                {
                    int count = SpawnedObjectControl.GetAllBattle.Count();
                    //.Select(x => x.GetComponent<DefaultSpawnedEnemyControl>())
                    //.Count(x =>
                    //{
                    //    if (x.control != null)
                    //        return x.control.enabled == true;
                    //    else
                    //        return x.gameObject.activeInHierarchy == true;
                    //});

                    if (count > 0)
                    {
                        t = 0f;
                    }
                    else
                    {
                        //try force next
                        if (t > (timer * 0.5f) && FSM.Fsm.ActiveState.Transitions.Length > 0)
                        {
                            if (SpawnedObjectControl.VERBOSE_DEBUG)
                                Dev.Log("TIMEOUT -- SENDING NEXT");
                            FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
                        }
                    }

                    if (t > timer)
                    {
                        if (SpawnedObjectControl.VERBOSE_DEBUG)
                            Dev.Log("TIMEOUT -- ENDING BATTLE");
                        bool isWhitePalace = BattleManager.Instance.Value.gameObject.scene.name.Contains("White_Palace_");
                        bool isRuins = BattleManager.Instance.Value.gameObject.scene.name.Contains("Ruins1_23");
                        OpenGates(isWhitePalace, isRuins);
                    }

                    updateRate = 0f;
                }

                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                updateRate += Time.deltaTime;
            }
        }

        protected virtual void OnBattleEnded()
        {
            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.Log("BATTLE OVER");
            preKilledEnemies = 0;
            battleStarted = true;
            battleEnded = true;

            if (FSM != null)
                UnlockCameras(GetCameraLocksFromScene(FSM.gameObject));
        }

        private void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, FsmState self)
        {
            orig(self);

            if (self == null || self.Fsm != FSM.Fsm)
                return;

            if (self.Actions.OfType<SendEventByName>().Any(x => x.sendEvent != null && (
            x.sendEvent.Value == "BG CLOSE" || 
            x.sendEvent.Value == "DREAM GATE CLOSE" ||
            x.sendEvent.Value == "FIGHT START" ||
            x.sendEvent.Value == "GHOST FIGHT START" ||
            x.sendEvent.Value == "BATTLE START"
            || (SceneName == "Crossroads_04" && x.sendEvent.Value == "START")
            
            )))
            {
                OnBattleStarted();
            }

            if (self.Actions.OfType<SendEventByName>().Any(x => x.sendEvent != null && (
            x.sendEvent.Value == "BG OPEN" ||
            x.sendEvent.Value == "GRIMM DEFEATED" ||
            x.sendEvent.Value == "BOSS DEATH" ||
            x.sendEvent.Value == "DISSIPATE" ||
            x.sendEvent.Value == "GHOST DEFEAT" ||
            x.sendEvent.Value == "GHOST DEAD" ||
            x.sendEvent.Value == "DISSIPATE" ||
            x.sendEvent.Value == "KILLED" ||
            x.sendEvent.Value == "IK GATE OPEN" ||
            x.sendEvent.Value == "BATTLE END"
            || (SceneName == "Crossroads_04" && x.sendEvent.Value == "END")
            )))
            {
                bool isColo = BattleManager.Instance.Value.gameObject.scene.name.Contains("Room_Colosseum_");
                if (!isColo)
                {
                    if (battleStarted)
                        OnBattleEnded();
                }
            }

            if (self.Name == "Idle")
            {
                {
                    {
                        if (self.Transitions.Any(x => x.EventName == "KILLED"))
                        {
                            waitingOnBossKill = true;
                            waitingState = self;
                            waitingEvent = "KILLED";
                        }
                    }
                }
            }

            if (self.Name == "Active")
            {
                if (self.Actions.Length == 1)
                {
                    if (self.Transitions.Any(x => x.EventName == "HORNET LEAVE"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "HORNET LEAVE";
                    }
                }
            }

            if (self.Name == "Activate")
            {
                if (self.Actions.Length == 1)
                {
                    if (self.Transitions.Any(x => x.EventName == "KILLED"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "KILLED";
                    }
                }
            }

            if (self.Name == "Dung Start")
            {
                {
                    if (self.Transitions.Any(x => x.EventName == "BATTLE END"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "BATTLE END";
                    }
                }
            }

            if (self.Name == "Start")
            {
                {
                    if (self.Transitions.Any(x => x.EventName == "ROYAL GUARD KILLED"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "ROYAL GUARD KILLED";
                    }
                }
                {
                    if (self.Transitions.Any(x => x.EventName == "BOSS DEATH"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "BOSS DEATH";
                    }
                }
                {
                    if (self.Transitions.Any(x => x.EventName == "BATTLE END"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "BATTLE END";
                    }
                }
            }

            if (self.Name == "Started")
            {
                {
                    if (self.Transitions.Any(x => x.EventName == "KILLED"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "KILLED";
                    }
                }
            }

            if (self.Name == "Hive Knight")
            {
                {
                    if (self.Transitions.Any(x => x.EventName == "BATTLE END"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "BATTLE END";
                    }
                }
            }

            if (self.Name == "Detect")
            {
                {
                    if (self.Transitions.Any(x => x.EventName == "BATTLE END"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "BATTLE END";
                    }

                    if (self.Transitions.Any(x => x.EventName == "KILLED"))
                    {
                        waitingOnBossKill = true;
                        waitingState = self;
                        waitingEvent = "KILLED";
                    }
                }
            }
        }

        private void Fsm_Event_FsmEventTarget_string(On.HutongGames.PlayMaker.Fsm.orig_Event_FsmEventTarget_string orig, Fsm self, FsmEventTarget eventTarget, string fsmEventName)
        {
            orig(self, eventTarget, fsmEventName);

            if (self == null || self != FSM.Fsm)
                return;

            if (SpawnedObjectControl.VERBOSE_DEBUG)
            {
                if (eventTarget != null)
                    Dev.Log($"Sending event {fsmEventName} to TARGET:{eventTarget.target} on GO:[{eventTarget.gameObject}]");
                else
                    Dev.Log($"Sending event {fsmEventName} to {eventTarget}");
            }
        }

        private void Fsm_Event_FsmEvent(On.HutongGames.PlayMaker.Fsm.orig_Event_FsmEvent orig, Fsm self, FsmEvent fsmEvent)
        {
            orig(self, fsmEvent);

            if (self == null || fsmEvent == null || self != FSM.Fsm)
                return;

            if (fsmEvent == null)
            {
                //this can happen in a number of cases....
            }
            else
            {
                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.Log($"Broadcasting event {fsmEvent.Name} from {self.Name}");
            }
        }

        public virtual void DecrementBattleEnemies()
        {
            if (FSM.FsmVariables.Contains("Battle Enemies"))
            {
                var be = FSM.FsmVariables.GetFsmInt("Battle Enemies");
                if (be.Value > 0)
                {
                    be.Value--;

                    if (be.Value <= 0 && FSM.Fsm.ActiveState.Transitions.Length > 0)
                    {
                        FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
                    }
                }
            }
        }

        public virtual void RegisterEnemyDeath(SpawnedObjectControl bmo)
        {
            bool updatedSomething = false;
            bool forceOpenGates = false;
            bool triggerNextWave = false;

            //skip doing this for those
            if (bmo != null && bmo.gameObject.GetSceneHierarchyPath().Contains("Pre Battle Enemies"))
                return;

            if (!battleStarted && !battleEnded)
                preKilledEnemies++;

            if (battleEnded)
            {
                if (FSM != null)
                    UnlockCameras(GetCameraLocksFromScene(FSM.gameObject));
                //return;
            }

            bool isColo = BattleManager.Instance.Value.gameObject.scene.name.Contains("Room_Colosseum_");
            bool isWhitePalace = BattleManager.Instance.Value.gameObject.scene.name.Contains("White_Palace_");
            bool isRuins = BattleManager.Instance.Value.gameObject.scene.name.Contains("Ruins1_23");
            bool isFungus1_32 = BattleManager.Instance.Value.gameObject.scene.name.Contains("Fungus1_32");

            if (FSM.FsmVariables.Contains("Battle Enemies"))
            {
                var be = FSM.FsmVariables.GetFsmInt("Battle Enemies");
                be.Value--;
                updatedSomething = true;

                if (be.Value <= 0)
                {
                    if (isColo)
                        triggerNextWave = true;
                    else
                        forceOpenGates = true;
                }
            }

            if (FSM.Fsm.ActiveState.Name == "Hive Knight")
            {
                triggerNextWave = false;
                forceOpenGates = true;
                FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
                GameManager.instance.SetPlayerDataBool("killedHiveKnight", true);
            }

            if (isColo && FSM.Fsm.ActiveState.Name == "Wave 29 Zote")
            {
                triggerNextWave = false;
                forceOpenGates = true;
            }

            if (isColo && FSM.Fsm.ActiveState.Name == "Lancer Battle")
            {
                triggerNextWave = false;
                forceOpenGates = false;
                FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
            }

            if (isWhitePalace && (bmo != null && bmo.name.Contains("Royal Guard")))
            {
                triggerNextWave = false;
                forceOpenGates = true;
                FSM.Fsm.Event(FSM.Fsm.ActiveState.Transitions[0].EventName);
            }

            if (isFungus1_32 && GameObject.FindObjectsOfType<SpawnedObjectControl>().Where(x => x != bmo).Count() <= 0)
            {
                forceOpenGates = true;
            }

            if (FSM.FsmVariables.Contains("Children"))
            {
                var be = FSM.FsmVariables.GetFsmInt("Children");
                be.Value--;
                updatedSomething = true;
                if (be.Value <= 0)
                    forceOpenGates = true;
            }

            if (isColo && triggerNextWave)
            {
                updatedSomething = true;
            }

            //notify the boss was killed
            if (waitingOnBossKill && waitingState != null && waitingEvent != null)
            {
                waitingState.Fsm.Event(waitingEvent);
                waitingOnBossKill = false;
                waitingState = null;
                waitingEvent = null;
                updatedSomething = true;
            }

            //fallback, if we failed to update the fms appropriately, just open the damn gates
            //when everything is dead
            if (!updatedSomething || (updatedSomething && forceOpenGates && waitingOnBossKill))
            {
                var allBattle = SpawnedObjectControl.GetAllBattle;

                if (!allBattle.Any(x => x.gameObject.IsVisible()))
                {
                    battleStarted = false;
                    OpenGates();
                }
            }

            if (forceOpenGates)
            {
                battleStarted = false;
                OpenGates(isWhitePalace, isRuins);
            }
        }

        public static void OpenGates(bool isWhitePalace = false, bool isRuins = false, bool isMawlek = false)
        {
            PlayMakerFSM.BroadcastEvent("BG OPEN");
            PlayMakerFSM.BroadcastEvent("DREAM GATE OPEN");
            if (isWhitePalace)
            {
                var door2 = GameObject.Find("Palace Gate");
                if (door2 != null)
                {
                    var fsm = door2.LocateMyFSM("control");
                    if (fsm != null)
                    {
                        PlayerData.instance.duskKnightDefeated = true;
                        fsm.SetState("Open Pause");
                    }
                }
            }

            if(isRuins)
                PlayMakerFSM.BroadcastEvent("OPEN");
        }

        public static void CloseGates(bool broadcastBattleStart = false)
        {
            PlayMakerFSM.BroadcastEvent("BG CLOSE");
            PlayMakerFSM.BroadcastEvent("DREAM GATE CLOSE");
            PlayMakerFSM.BroadcastEvent("FIGHT START");
            PlayMakerFSM.BroadcastEvent("GHOST FIGHT START");
            if (broadcastBattleStart)
                PlayMakerFSM.BroadcastEvent("BATTLE START");            
        }

        public static IEnumerable<CameraLockArea> GetCameraLocksFromScene(GameObject gameObject)
        {
            return gameObject.GetComponentsFromScene<CameraLockArea>();
        }

        public static void UnlockCameras(IEnumerable<CameraLockArea> cameraLocks)
        {
            foreach (var c in cameraLocks)
            {
                c.gameObject.SetActive(false);
            }
        }

        public static void UnlockCameras()
        {
            if (BattleManager.StateMachine.Value.FSM != null)
                UnlockCameras(GetCameraLocksFromScene(BattleManager.StateMachine.Value.FSM.gameObject));
        }
    }


}
