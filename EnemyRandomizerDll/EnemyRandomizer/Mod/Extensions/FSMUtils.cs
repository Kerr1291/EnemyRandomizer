using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using nv;

namespace EnemyRandomizerMod
{    
    public static class FSMUtils
    {
        public static bool RemoveState(this HutongGames.PlayMaker.Fsm fsm, string stateName)
        {
            if (fsm.States.Any(x => x.Name == stateName))
            {
                fsm.States = fsm.States.Where(x => x.Name != stateName).ToArray();
                return true;
            }

            return false;
        }


        public static PlayMakerFSM GetMatchingFSMComponent(this GameObject gameObject, string fsmName, string stateName, string actionName)
        {
            PlayMakerFSM foundFSM = null;
            for (int i = 0; i < gameObject.GetComponentsInChildren<PlayMakerFSM>().Length; ++i)
            {
                PlayMakerFSM fsm = gameObject.GetComponentsInChildren<PlayMakerFSM>()[i];
                if (fsm.FsmName == fsmName)
                {
                    foreach (var s in fsm.FsmStates)
                    {
                        if (s.Name == stateName)
                        {
                            foreach (var a in s.Actions)
                            {
                                if (a.GetType().Name == actionName)
                                {
                                    foundFSM = fsm;
                                    break;
                                }
                            }

                            if (foundFSM != null)
                                break;
                        }
                    }
                }

                if (foundFSM != null)
                    break;
            }
            return foundFSM;
        }

        //if fsmName is empty, try every state on all fsms
        public static HutongGames.PlayMaker.FsmState GetFSMState(this GameObject gameObject, string stateName, string fsmName = "")
        {
            foreach (PlayMakerFSM fsm in gameObject.GetComponents<PlayMakerFSM>())
            {
                if (string.IsNullOrEmpty(fsmName) || fsmName == fsm.FsmName)
                {
                    foreach (var s in fsm.FsmStates)
                    {
                        if (s.Name == stateName)
                            return s;
                    }
                }
            }
            return null;
        }

        public static (HutongGames.PlayMaker.FsmState state, HutongGames.PlayMaker.FsmStateAction action) FindFSMAction(this GameObject gameObject, string actionName)
        {
            foreach (PlayMakerFSM fsm in gameObject.GetComponentsInChildren<PlayMakerFSM>(true))
            {
                if (fsm == null)
                    continue;

                if (fsm.FsmStates == null)
                    continue;

                foreach (var s in fsm.FsmStates)
                {
                    if (s.Actions == null || s.Actions.Length <= 0)
                        continue;

                    var action = s.Actions.Where(x => x != null).FirstOrDefault(x => x.GetType().Name.Contains(actionName));
                    if (action != null)
                        return (s, action);
                }
            }
            return (null, null);
        }

        public static T GetFSMActionOnState<T>(this GameObject gameObject, string actionName, string stateName, string fsmName = "") where T : HutongGames.PlayMaker.FsmStateAction
        {
            var state = gameObject.GetFSMState(stateName, fsmName);
            if (state != null)
            {
                foreach (var action in state.Actions)
                {
                    if (action.Name == actionName)
                    {
                        return action as T;
                    }
                }
            }

            return null;
        }

        public static T GetFSMActionOnState<T>(this GameObject gameObject, string stateName, string fsmName = "") where T : HutongGames.PlayMaker.FsmStateAction
        {
            var state = gameObject.GetFSMState(stateName, fsmName);
            if (state != null)
            {
                foreach (var action in state.Actions)
                {
                    if (action.GetType() == typeof(T))
                    {
                        return action as T;
                    }
                }
            }

            return null;
        }

        public static List<T> GetFSMActionsOnState<T>(this GameObject gameObject, string actionName, string stateName, string fsmName = "") where T : HutongGames.PlayMaker.FsmStateAction
        {
            List<T> actions = new List<T>();
            var state = gameObject.GetFSMState(stateName, fsmName);
            if (state != null)
            {
                foreach (var action in state.Actions)
                {
                    if (action.Name == actionName)
                    {
                        actions.Add(action as T);
                    }
                }
            }

            return actions;
        }

        public static List<T> GetFSMActionsOnState<T>(this GameObject gameObject, string stateName, string fsmName = "") where T : HutongGames.PlayMaker.FsmStateAction
        {
            List<T> actions = new List<T>();
            var state = gameObject.GetFSMState(stateName, fsmName);
            if (state != null)
            {
                foreach (var action in state.Actions)
                {
                    if (action.GetType() == typeof(T))
                    {
                        actions.Add(action as T);
                    }
                }
            }
            else
            {
                Dev.Log("Warning: No state named " + stateName + "found in fsm " + fsmName);
            }

            return actions;
        }

        public static List<T> GetFSMActionsOnStates<T>(this GameObject gameObject, List<string> stateNames, string fsmName = "") where T : HutongGames.PlayMaker.FsmStateAction
        {
            List<T> actions = new List<T>();
            foreach (string stateName in stateNames)
            {
                var state = gameObject.GetFSMState(stateName, fsmName);
                if (state != null)
                {
                    foreach (var action in state.Actions)
                    {
                        if (action.GetType() == typeof(T))
                        {
                            actions.Add(action as T);
                        }
                    }
                }
            }

            return actions;
        }
    }
}
