using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace nv
{
    public static class SceneExtensions
    {
        public static void PrintHierarchy( this Scene scene, int localIndex = -1, Bounds? sceneBounds = null, List<string> randomizerEnemyTypes = null )
        {
            if( !scene.IsValid() )
                return;

            Dev.Log( "START =====================================================" );
            Dev.Log( "Printing scene hierarchy for scene: " + scene.name + " [Build index: " + scene.buildIndex + "]" );
            if( localIndex >= 0 )
                Dev.Log( "Local scene index: " + localIndex );

            GameObject[] rootGameObjects = scene.GetRootGameObjects();

            foreach( GameObject go in rootGameObjects )
            {
                if( go == null )
                {
                    Dev.Log( "Scene " + scene.name + " has a null root game object! Skipping debug print scene..." );
                    break;
                }

                foreach( Transform t in go.GetComponentsInChildren<Transform>( true ) )
                {
                    string objectNameAndPath = t.gameObject.PrintSceneHierarchyPath();
                    string logContent = objectNameAndPath;

                    if( sceneBounds.HasValue && !sceneBounds.Value.Contains( t.position ) )
                        logContent += " ::: IsOutsideSceneBounds = true";
                    if( t.gameObject.name.IsSkipLoadingString() )
                        logContent += " ::: IsSkipLoadingString = true";
                    if( t.gameObject.name.IsSkipRandomizingString() )
                        logContent += " ::: IsSkipRandomizingString = true";                                  
                    if( randomizerEnemyTypes != null && t.gameObject.IsRandomizerEnemy( randomizerEnemyTypes ) )
                        logContent += " ::: IsRandomizerEnemy = true";

                    Dev.Log( logContent );
                    t.gameObject.PrintSceneHierarchyTree( true );
                }
            }
            Dev.Log( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
        }
    }
}
