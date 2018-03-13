using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class GameObjectExtensions
    {
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

        public static string PrintSceneHierarchyPath( this GameObject gameObject )
        {
            if( gameObject == null )
                return "WARNING: NULL GAMEOBJECT";

            string objStr = gameObject.name;

            if( gameObject.transform.parent != null )
                objStr = gameObject.transform.parent.gameObject.PrintSceneHierarchyPath() + "\\" + gameObject.name;

            return objStr;
        }

        public static void PrintSceneHierarchyTree( this GameObject gameObject, bool printComponents = false )
        {
            if( gameObject == null )
                return;

            Dev.Log( "DebugPrintObjectTree START =====================================================" );
            Dev.Log( "Printing scene hierarchy for game object: " + gameObject.name );
            foreach( Transform t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                string objectNameAndPath = t.gameObject.PrintSceneHierarchyPath();
                Dev.Log( objectNameAndPath );

                if( printComponents )
                {
                    string componentHeader = "";
                    for( int i = 0; i < ( objectNameAndPath.Length - t.gameObject.name.Length ); ++i )
                        componentHeader += " ";

                    foreach( Component c in t.GetComponents<Component>() )
                    {
                        c.PrintComponentType( componentHeader );
                        c.PrintTransform( componentHeader );
                        c.PrintBoxCollider2D( componentHeader );
                        c.PrintPlayMakerFSM( componentHeader );
                    }
                }
            }
            Dev.Log( "DebugPrintObjectTree END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
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


        public static bool IsGameEnemy( this GameObject gameObject )
        {
            if( gameObject == null )
                return false;

            //TODO: wrap in #if/#else for case where playmaker is gone
            return ( FSMUtility.ContainsFSM( gameObject, "health_manager_enemy" ) ) || ( FSMUtility.ContainsFSM( gameObject, "health_manager" ) );
        }

        //TODO: wrap in #if/#else for case where playmaker is gone
        public static PlayMakerFSM GetEnemyFSM( this GameObject gameObject )
        {
            PlayMakerFSM fsm = null;
            if( gameObject == null )
                return fsm;

            fsm = ( FSMUtility.LocateFSM( gameObject, "health_manager_enemy" ) );
            if( fsm != null )
                return fsm;
            fsm = ( FSMUtility.LocateFSM( gameObject, "health_manager" ) );
            return fsm;

        }

        public static void ChangeEnemyGeoRates( this GameObject gameObject, float multiplier )
        {
            if( gameObject == null )
                return;

            if( !gameObject.IsGameEnemy() )
                return;

            //TODO: wrap in #if/#else for case where playmaker is gone
            PlayMakerFSM fsm = gameObject.GetEnemyFSM();
            if( fsm != null )
            {
                fsm.FsmVariables.GetFsmInt( "Geo Small" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Small" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Large" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Large" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value * multiplier );
            }
        }

        public static void SetEnemyGeoRates( this GameObject gameObject, int smallValue, int medValue, int largeValue )
        {
            if( gameObject == null )
                return;

            if( !gameObject.IsGameEnemy() )
                return;

            //TODO: wrap in #if/#else for case where playmaker is gone
            PlayMakerFSM fsm = gameObject.GetEnemyFSM();
            if( fsm != null )
            {
                fsm.FsmVariables.GetFsmInt( "Geo Small" ).Value = smallValue;
                fsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value = smallValue;
                fsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value = medValue;
                fsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value = medValue;
                fsm.FsmVariables.GetFsmInt( "Geo Large" ).Value = largeValue;
                fsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value = largeValue;
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

            //TODO: wrap in #if/#else for case where playmaker is gone
            PlayMakerFSM fsm = gameObject.GetEnemyFSM();
            PlayMakerFSM otherFsm = copyFrom.GetEnemyFSM();
            if( fsm != null )
            {
                fsm.FsmVariables.GetFsmInt( "Geo Small" ).Value = otherFsm.FsmVariables.GetFsmInt( "Geo Small" ).Value;
                fsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value = otherFsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value;
                fsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value = otherFsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value;
                fsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value = otherFsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value;
                fsm.FsmVariables.GetFsmInt( "Geo Large" ).Value = otherFsm.FsmVariables.GetFsmInt( "Geo Large" ).Value;
                fsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value = otherFsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value;
            }
        }
    }
}
