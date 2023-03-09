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

namespace EnemyRandomizerMod
{
    public class BattleStateMachine : IDisposable
    {
        public Scene BattleScene { get; protected set; }
        public string SceneName { get { return BattleScene.name; } }
        public PlayMakerFSM FSM { get; protected set; }

        protected CompositeDisposable disposables = new CompositeDisposable();

        public FsmState waitingState = null;
        public bool waitingOnBossKill = false;
        public string waitingEvent = null;

        public virtual void Dispose()
        {
            ((IDisposable)disposables).Dispose();

            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEvent -= Fsm_Event_FsmEvent;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEventTarget_string -= Fsm_Event_FsmEventTarget_string;
        }

        public BattleStateMachine(Scene scene, PlayMakerFSM fsm)
        {
            BattleScene = scene;
            FSM = fsm;

            On.HutongGames.PlayMaker.Fsm.Event_FsmEvent -= Fsm_Event_FsmEvent;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEventTarget_string -= Fsm_Event_FsmEventTarget_string;

            On.HutongGames.PlayMaker.Fsm.Event_FsmEvent += Fsm_Event_FsmEvent;
            On.HutongGames.PlayMaker.Fsm.Event_FsmEventTarget_string += Fsm_Event_FsmEventTarget_string;

            On.HutongGames.PlayMaker.FsmState.OnEnter -= FsmState_OnEnter;
            On.HutongGames.PlayMaker.FsmState.OnEnter += FsmState_OnEnter;
        }

        private void FsmState_OnEnter(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, FsmState self)
        {
            orig(self);

            if (self == null || self.Fsm != FSM.Fsm)
                return;

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
        }

        private void Fsm_Event_FsmEvent(On.HutongGames.PlayMaker.Fsm.orig_Event_FsmEvent orig, Fsm self, FsmEvent fsmEvent)
        {
            orig(self, fsmEvent);
        }

        public void RegisterEnemyDeath(BattleManagedObject bmo)
        {
            bool updatedSomething = false;
            bool forceOpenGates = false;
            bool triggerNextWave = false;

            //skip doing this for those
            if (bmo != null && bmo.gameObject.GetSceneHierarchyPath().Contains("Pre Battle Enemies"))
                return;

            bool isColo = BattleManager.Instance.Value.gameObject.scene.name.Contains("Room_Colosseum_");

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
                var infos = GameObject.FindObjectsOfType<BattleManagedObject>().Select(x =>
                {
                    var info = new ObjectMetadata();
                    info.Setup(x.gameObject, EnemyRandomizerDatabase.GetDatabase());
                    return info;
                }).ToList();

                if (infos.Count < 0 || !infos.Any(x => x.IsVisible))
                    OpenGates();
            }

            if (forceOpenGates)
                OpenGates();
        }

        public void RegisterEnemy(BattleManagedObject bmo)
        {
        }

        public static void OpenGates()
        {
            var fsms = GameObject.FindObjectsOfType<PlayMakerFSM>();
            {
                fsms
                    .Where(x => x != null)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.SendEvent("BG OPEN");
                    });
            }
        }

        public static void CloseGates()
        {
            var fsms = GameObject.FindObjectsOfType<PlayMakerFSM>();
            {
                fsms
                    .Where(x => x != null)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.SendEvent("BG CLOSE");
                    });
            }
        }
    }
}
