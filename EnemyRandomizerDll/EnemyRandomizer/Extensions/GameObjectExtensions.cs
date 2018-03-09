using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public static class GameObjectExtensions
    {
        public static bool IsRandomizerEnemy( this GameObject gameObject )
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

            bool isRandoEnemyType = false;

            //TODO: Move this logic into getter in the data manager
            if( EnemyRandomizer.simulateReplacement )
                isRandoEnemyType = EnemyRandoData.enemyTypeNames.Contains( trimmedName );
            else
                isRandoEnemyType = EnemyRandomizer.Instance.loadedEnemyPrefabNames.Contains( trimmedName );

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

            EnemyRandomizer.Instance.Log( "DebugPrintObjectTree START =====================================================" );
            EnemyRandomizer.Instance.Log( "Printing scene hierarchy for game object: " + gameObject.name );
            foreach( Transform t in gameObject.GetComponentsInChildren<Transform>( true ) )
            {
                string objectNameAndPath = t.gameObject.PrintSceneHierarchyPath();
                EnemyRandomizer.Instance.Log( objectNameAndPath );

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
            EnemyRandomizer.Instance.Log( "DebugPrintObjectTree END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
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
    }
}
