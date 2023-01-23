using System.Collections.Generic;
using UnityEngine;
using nv;
 

namespace EnemyRandomizerMod
{
    public static class GameObjectExtensions
    { 
        public static bool IsRandomizerEnemy( this GameObject gameObject, List<string> enemyTypes )
        {
            if( gameObject == null )
                return false;

            if( !gameObject.IsGameEnemy() )
                return false;

            if( gameObject.name.Contains( "Corpse" ) )
                return false;

            if( gameObject.name.Contains( "Lil Jellyfish" ) )
                return false;

            //TEST: should randomize spawn rollers
            if( gameObject.name.Contains( "Spawn Roller v2" ) )
                return true;

            string enemyName = gameObject.name;
            string trimmedName = enemyName.TrimGameObjectName();

            bool isRandoEnemyType = enemyTypes.Contains( trimmedName );

            return isRandoEnemyType;
        }

        public static bool IsEnemyDead(this GameObject enemy)
        {
            return enemy == null
                || enemy.activeInHierarchy == false
                || (enemy.IsGameEnemy() && enemy.GetEnemyHP() <= 0)
                || (enemy.IsGameEnemy() && enemy.GetEnemyHealthManager().isDead);
        }
        public static bool IsGameEnemy(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            return gameObject.GetComponent<HealthManager>() != null;
        }

        public static HealthManager GetEnemyHealthManager(this GameObject gameObject)
        {
            HealthManager hm = null;
            if (gameObject == null)
                return hm;

            hm = gameObject.GetComponent<HealthManager>();

            return hm;
        }
        public static void ChangeEnemyGeoRates(this GameObject gameObject, float multiplier)
        {
            if (gameObject == null)
                return;

            if (!gameObject.IsGameEnemy())
                return;

            HealthManager hm = gameObject.GetEnemyHealthManager();
            if (hm != null)
            {
                hm.SetGeoSmall(hm.GetGeoSmall() * (int)multiplier);
                hm.SetGeoMedium(hm.GetGeoMedium() * (int)multiplier);
                hm.SetGeoLarge(hm.GetGeoLarge() * (int)multiplier);
            }
        }

        public static void SetEnemyGeoRates(this GameObject gameObject, int smallValue, int medValue, int largeValue)
        {
            if (gameObject == null)
                return;

            if (!gameObject.IsGameEnemy())
                return;

            HealthManager hm = gameObject.GetEnemyHealthManager();
            if (hm != null)
            {
                hm.SetGeoSmall(smallValue);
                hm.SetGeoMedium(medValue);
                hm.SetGeoLarge(largeValue);
            }
        }

        public static void SetEnemyGeoRates(this GameObject gameObject, GameObject copyFrom)
        {
            if (gameObject == null)
                return;

            if (!gameObject.IsGameEnemy())
                return;

            if (copyFrom == null)
                return;

            if (!copyFrom.IsGameEnemy())
                return;

            HealthManager hm = gameObject.GetEnemyHealthManager();
            HealthManager otherHM = gameObject.GetEnemyHealthManager();
            if (hm != null && otherHM != null)
            {
                hm.SetGeoSmall(otherHM.GetGeoSmall());
                hm.SetGeoMedium(otherHM.GetGeoMedium());
                hm.SetGeoLarge(otherHM.GetGeoLarge());
            }
        }
        public static int GetEnemyHP(this GameObject gameObject)
        {
            int hp = 0;
            HealthManager hm = gameObject.GetEnemyHealthManager();
            if (hm != null)
            {
                hp = hm.hp;
            }
            return hp;
        }

        public static void SetEnemyHP(this GameObject gameObject, int newHP)
        {
            HealthManager hm = gameObject.GetEnemyHealthManager();
            if (hm != null)
            {
                hm.hp = newHP;
            }
        }


        //public static int GetEnemyHP(this GameObject gameObject)
        //{
        //    int hp = 0;
        //    HealthManager hm = gameObject.GetEnemyHealthManager();
        //    if (hm != null)
        //    {
        //        hp = hm.hp;
        //    }
        //    return hp;
        //}
        //public static void SetEnemyHP(this GameObject gameObject, int newHP)
        //{
        //    HealthManager hm = gameObject.GetEnemyHealthManager();
        //    if (hm != null)
        //    {
        //        hm.hp = newHP;
        //    }
        //}

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
