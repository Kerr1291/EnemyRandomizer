using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
//using System.ComponentModel;

namespace nv
{
    public static class HealthManagerExtensions
    {
        public static int GetGeoSmall( this HealthManager healthManager )
        {
            FieldInfo fi = healthManager.GetType().GetField("smallGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance );
            object temp = fi.GetValue(healthManager);
            int value = (temp == null ? 0 : (int)temp);
            return value;
        }

        public static int GetGeoMedium( this HealthManager healthManager )
        {
            FieldInfo fi = healthManager.GetType().GetField("mediumGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance );
            object temp = fi.GetValue(healthManager);
            int value = (temp == null ? 0 : (int)temp);
            return value;
        }

        public static int GetGeoLarge( this HealthManager healthManager )
        {
            FieldInfo fi = healthManager.GetType().GetField("largeGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance );
            object temp = fi.GetValue(healthManager);
            int value = (temp == null ? 0 : (int)temp);
            return value;
        }
    }

    public static class GameObjectExtensions
    {
        public static GameObject FindGameObjectInChildren( this GameObject gameObject, string name )
        {
            if( gameObject == null )
                return null;

            foreach( var t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.name == name )
                    return t.gameObject;
            }
            return null;
        }

        public static GameObject FindGameObjectNameContainsInChildren( this GameObject gameObject, string name )
        {
            if( gameObject == null )
                return null;

            foreach( var t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.name.Contains( name ) )
                    return t.gameObject;
            }
            return null;
        }

        public static string PrintSceneHierarchyPath( this GameObject gameObject )
        {
            if( gameObject == null )
                return "WARNING: NULL GAMEOBJECT";

            string objStr = gameObject.name;

            if( gameObject.transform.parent != null )
                objStr = gameObject.transform.parent.gameObject.PrintSceneHierarchyPath() + "\\" + gameObject.name;

            return objStr;
        }

        public static void PrintSceneHierarchyTree( this GameObject gameObject, bool printComponents = false, System.IO.StreamWriter file = null )
        {
            if( gameObject == null )
                return;

            if( file != null )
            {
                file.WriteLine( "START =====================================================" );
                file.WriteLine( "Printing scene hierarchy for game object: " + gameObject.name );
            }
            else
            {
                Dev.Log( "START =====================================================" );
                Dev.Log( "Printing scene hierarchy for game object: " + gameObject.name );
            }

            foreach( Transform t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.gameObject.name.IsSkipAlwaysString() )
                    continue;

                string objectNameAndPath = t.gameObject.PrintSceneHierarchyPath();

                if( file != null )
                {
                    file.WriteLine( objectNameAndPath );
                }
                else
                {
                    Dev.Log( objectNameAndPath );
                }


                if( printComponents )
                {
                    string componentHeader = "";
                    for( int i = 0; i < ( objectNameAndPath.Length - t.gameObject.name.Length ); ++i )
                        componentHeader += " ";

                    foreach( Component c in t.GetComponents<Component>() )
                    {
                        c.PrintComponentType( componentHeader, file );
                        c.PrintTransform( componentHeader, file );
                        c.PrintBoxCollider2D( componentHeader, file );
                        c.PrintPlayMakerFSM( componentHeader, file );
                        c.PrintPersistentBoolItem( componentHeader, file );
                    }
                }
            }

            if( file != null )
            {
                file.WriteLine( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
            }
            else
            {
                Dev.Log( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
            }
        }

        public static T GetOrAddComponent<T>( this GameObject source ) where T : UnityEngine.Component
        {
            T result = source.GetComponent<T>();
            if( result != null )
                return result;
            result = source.AddComponent<T>();
            return result;
        }

        public static void GetOrAddComponentIfNull<T>( this GameObject source, ref T result ) where T : UnityEngine.Component
        {
            if( result != null )
                return;
            result = source.GetComponent<T>();
            if( result != null )
                return;
            result = source.AddComponent<T>();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Methods with a dependency on playmaker, look into refactoring and/or removing to lessen the dependency


        public static bool IsRandomizerEnemy( this GameObject gameObject, List<string> enemyTypes )
        {
            if( gameObject == null )
                return false;

            if( !IsGameEnemy( gameObject ) )
                return false;

            if( gameObject.name.Contains( "Corpse" ) )
                return false;

            if( gameObject.name.Contains( "Lil Jellyfish" ) )
                return false;

            string enemyName = gameObject.name;
            string trimmedName = enemyName.TrimGameObjectName();

            bool isRandoEnemyType = enemyTypes.Contains( trimmedName );

            return isRandoEnemyType;
        }

        public static bool IsEnemyDead( this GameObject enemy )
        {
            return enemy == null
                || enemy.activeInHierarchy == false
                || ( enemy.IsGameEnemy() && enemy.GetEnemyHP() <= 0 )
                || ( enemy.IsGameEnemy() && enemy.GetEnemyHealthManager().isDead );
            //( enemy.GetEnemyFSM().ActiveStateName.Contains( "Corpse" ) || enemy.GetEnemyFSM().ActiveStateName.Contains( "Death" ) || ( enemy.GetEnemyFSM().Fsm.Variables.FindFsmInt( "HP" ) != null && FSMUtility.GetInt( enemy.GetEnemyFSM(), "HP" ) <= 0 ) ) );
        }

        public static bool IsGameEnemy( this GameObject gameObject )
        {
            if( gameObject == null )
                return false;

            //TODO: wrap in #if/#else for case where playmaker is gone
            return gameObject.GetComponent<HealthManager>() != null;// || ( FSMUtility.ContainsFSM( gameObject, "health_manager_enemy" ) ) || ( FSMUtility.ContainsFSM( gameObject, "health_manager" ) );
        }

        //TODO: wrap in #if/#else for case where playmaker is gone
        //public static PlayMakerFSM GetEnemyFSM( this GameObject gameObject )
        //{
        //    PlayMakerFSM fsm = null;
        //    if( gameObject == null )
        //        return fsm;

        //    fsm = ( FSMUtility.LocateFSM( gameObject, "health_manager_enemy" ) );
        //    if( fsm != null )
        //        return fsm;
        //    fsm = ( FSMUtility.LocateFSM( gameObject, "health_manager" ) );
        //    return fsm;
        //}

        public static HealthManager GetEnemyHealthManager( this GameObject gameObject )
        {
            HealthManager hm = null;
            if( gameObject == null )
                return hm;

            hm = gameObject.GetComponent<HealthManager>();

            return hm;
        }

        public static void ChangeEnemyGeoRates( this GameObject gameObject, float multiplier )
        {
            if( gameObject == null )
                return;

            if( !gameObject.IsGameEnemy() )
                return;

            HealthManager hm = gameObject.GetEnemyHealthManager();
            if(hm != null)
            {
                hm.SetGeoSmall( hm.GetGeoSmall() * (int)multiplier );
                hm.SetGeoMedium( hm.GetGeoMedium() * (int)multiplier );
                hm.SetGeoLarge( hm.GetGeoLarge() * (int)multiplier );
            }
        }

        public static void SetEnemyGeoRates( this GameObject gameObject, int smallValue, int medValue, int largeValue )
        {
            if( gameObject == null )
                return;

            if( !gameObject.IsGameEnemy() )
                return;

            HealthManager hm = gameObject.GetEnemyHealthManager();
            if( hm != null )
            {
                hm.SetGeoSmall( smallValue );
                hm.SetGeoMedium( medValue );
                hm.SetGeoLarge( largeValue );
            }
        }        

        public static void SetEnemyGeoRates( this GameObject gameObject, GameObject copyFrom )
        {
            if( gameObject == null )
                return;

            if( !gameObject.IsGameEnemy() )
                return;

            if( copyFrom == null )
                return;

            if( !copyFrom.IsGameEnemy() )
                return;

            HealthManager hm = gameObject.GetEnemyHealthManager();
            HealthManager otherHM = gameObject.GetEnemyHealthManager();
            if( hm != null && otherHM != null )
            {
                hm.SetGeoSmall( otherHM.GetGeoSmall() );
                hm.SetGeoMedium( otherHM.GetGeoMedium() );
                hm.SetGeoLarge( otherHM.GetGeoLarge() );
            }
        }

        public static int GetEnemyHP( this GameObject gameObject )
        {
            int hp = 0;
            HealthManager hm = gameObject.GetEnemyHealthManager();
            if( hm != null )
            {
                hp = hm.hp;
            }
            return hp;
        }

        public static void SetEnemyHP( this GameObject gameObject, int newHP )
        {
            HealthManager hm = gameObject.GetEnemyHealthManager();
            if( hm != null )
            {
                hm.hp = newHP;
            }
        }

        //if fsmName is empty, try every state on all fsms
        public static HutongGames.PlayMaker.FsmState GetFSMState( this GameObject gameObject, string stateName, string fsmName = "" )
        {
            foreach( PlayMakerFSM fsm in gameObject.GetComponents<PlayMakerFSM>() )
            {
                if( string.IsNullOrEmpty( fsmName ) || fsmName == fsm.FsmName )
                {
                    foreach( var s in fsm.FsmStates )
                    {
                        if( s.Name == stateName )
                            return s;
                    }
                }
            }
            return null;
        }

        public static T GetFSMActionOnState<T>( this GameObject gameObject, string actionName, string stateName, string fsmName = "" ) where T : HutongGames.PlayMaker.FsmStateAction
        {
            var state = gameObject.GetFSMState(stateName,fsmName);
            if( state != null )
            {
                foreach(var action in state.Actions)
                {
                    if(action.Name == actionName)
                    {
                        return action as T;
                    }
                }
            }

            return null;
        }

        public static T GetFSMActionOnState<T>( this GameObject gameObject, string stateName, string fsmName = "" ) where T : HutongGames.PlayMaker.FsmStateAction
        {
            var state = gameObject.GetFSMState(stateName,fsmName);
            if( state != null )
            {
                foreach( var action in state.Actions )
                {
                    if( action.GetType() == typeof(T) )
                    {
                        return action as T;
                    }
                }
            }

            return null;
        }

        public static List<T> GetFSMActionsOnState<T>( this GameObject gameObject, string actionName, string stateName, string fsmName = "" ) where T : HutongGames.PlayMaker.FsmStateAction
        {
            List<T> actions = new List<T>();
            var state = gameObject.GetFSMState(stateName,fsmName);
            if( state != null )
            {
                foreach( var action in state.Actions )
                {
                    if( action.Name == actionName )
                    {
                        actions.Add(action as T);
                    }
                }
            }

            return actions;
        }

        public static List<T> GetFSMActionsOnState<T>( this GameObject gameObject, string stateName, string fsmName = "" ) where T : HutongGames.PlayMaker.FsmStateAction
        {
            List<T> actions = new List<T>();
            var state = gameObject.GetFSMState(stateName,fsmName);
            if( state != null )
            {
                foreach( var action in state.Actions )
                {
                    if( action.GetType() == typeof( T ) )
                    {
                        actions.Add( action as T );
                    }
                }
            }

            return actions;
        }

        public static List<T> GetFSMActionsOnStates<T>( this GameObject gameObject, List<string> stateNames, string fsmName = "" ) where T : HutongGames.PlayMaker.FsmStateAction
        {
            List<T> actions = new List<T>();
            foreach( string stateName in stateNames )
            {
                var state = gameObject.GetFSMState(stateName,fsmName);
                if( state != null )
                {
                    foreach( var action in state.Actions )
                    {
                        if( action.GetType() == typeof( T ) )
                        {
                            actions.Add( action as T );
                        }
                    }
                }
            }

            return actions;
        }
    }
}
