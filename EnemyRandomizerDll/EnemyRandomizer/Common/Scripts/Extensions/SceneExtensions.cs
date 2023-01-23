using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace nv
{
    public static class SceneManagerExtensions
    {
        public static List<Scene> GetLoadedScenes()
        {
            List<Scene> scenes = new List<Scene>();
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                scenes.Add(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
            }
            return scenes;
        }
    }

    public static class SceneExtensions
    {
        public static int GetLoadedIndex(this Scene scene)
        {
            for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                if(scene.buildIndex == UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).buildIndex)
                    return i;
            }
            return -1;
        }

        public static GameObject FindGameObject( this Scene scene, string name )
        {
            if( !scene.IsValid() )
                return null;
            
            GameObject[] rootGameObjects = scene.GetRootGameObjects();

            try
            {
                foreach( GameObject go in rootGameObjects )
                {
                    if( go == null )
                    {
                        break;
                    }

                    GameObject found = go.FindGameObjectInChildrenWithName(name);
                    if( found != null )
                        return found;
                }
            }
            catch( Exception e )
            {
                Dev.Log( "Exception: " + e.Message );
            }

            return null;
        }

        public static void PrintHierarchy( this Scene scene, string outputFilePath = "", bool verbose = true)
        {
            if( !scene.IsValid() )
                return;

            System.IO.StreamWriter file = null;
            if( !string.IsNullOrEmpty( outputFilePath ) )
            {
                try
                {
                    file = new System.IO.StreamWriter( outputFilePath );
                }
                catch(Exception e)
                {
                    Dev.Log( "Exception!: " + e.Message );
                    file = null;
                }
            }

            if( file != null )
            {
                file.WriteLine( "START =====================================================" );
                file.WriteLine( "Printing full hierarchy for scene: " + scene.name + " [Build index: " + scene.buildIndex + "]" );

                if(scene.GetLoadedIndex() >= 0)
                    file.WriteLine("Loaded scene index: " + scene.GetLoadedIndex());
            }
            else
            {
                Debug.Log( "START =====================================================" );
                Debug.Log( "Printing full hierarchy for scene: " + scene.name + " [Build index: " + scene.buildIndex + "]" );

                if(scene.GetLoadedIndex() >= 0)
                    Debug.Log("Loaded scene index: " + scene.GetLoadedIndex());
            }

            GameObject[] rootGameObjects = scene.GetRootGameObjects();

            try
            {
                foreach( GameObject go in rootGameObjects )
                {
                    if( go == null )
                    {
                        if( file != null )
                        {
                            file.WriteLine( "Scene " + scene.name + " has a null root game object! Skipping debug print scene..." );
                        }
                        else
                        {
                            Debug.Log( "Scene " + scene.name + " has a null root game object! Skipping debug print scene..." );
                        }
                        break;
                    }

                    if( string.IsNullOrEmpty( outputFilePath ) )
                    {
                        go.PrintSceneHierarchyTree(verbose);
                    }
                    else
                    {
                        go.PrintSceneHierarchyTree(verbose, file );
                    }
                }
            }
            catch(Exception e)
            {
                Dev.Log( "Exception: " + e.Message );
            }

            if( file != null )
            {
                file.WriteLine( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
                file.Close();
            }
            else
            {
                Debug.Log( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
            }
        }


        public static void PrintHierarchyRoot(this Scene scene, string outputFilePath = "")
        {
            if(!scene.IsValid())
                return;

            System.IO.StreamWriter file = null;
            if(!string.IsNullOrEmpty(outputFilePath))
            {
                try
                {
                    file = new System.IO.StreamWriter(outputFilePath);
                }
                catch(Exception e)
                {
                    Dev.Log("Exception!: " + e.Message);
                    file = null;
                }
            }

            if(file != null)
            {
                file.WriteLine("START =====================================================");
                file.WriteLine("Printing root hierarchy for scene: " + scene.name + " [Build index: " + scene.buildIndex + "]");

                if(scene.GetLoadedIndex() >= 0)
                    file.WriteLine("Loaded scene index: " + scene.GetLoadedIndex());
            }
            else
            {
                Debug.Log("START =====================================================");
                Debug.Log("Printing root hierarchy for scene: " + scene.name + " [Build index: " + scene.buildIndex + "]");

                if(scene.GetLoadedIndex() >= 0)
                    Debug.Log("Loaded scene index: " + scene.GetLoadedIndex());
            }

            GameObject[] rootGameObjects = scene.GetRootGameObjects();

            try
            {
                foreach(GameObject go in rootGameObjects)
                {
                    if(go == null)
                    {
                        if(file != null)
                        {
                            file.WriteLine("Scene " + scene.name + " has a null root game object! Skipping debug print scene...");
                        }
                        else
                        {
                            Debug.Log("Scene " + scene.name + " has a null root game object! Skipping debug print scene...");
                        }
                        break;
                    }

                    if(string.IsNullOrEmpty(outputFilePath))
                    {
                        file.WriteLine(go.GetSceneHierarchyPath());
                    }
                    else
                    {
                        Debug.Log(go.GetSceneHierarchyPath());
                    }
                }
            }
            catch(Exception e)
            {
                Dev.Log("Exception: " + e.Message);
            }

            if(file != null)
            {
                file.WriteLine("END +++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                file.Close();
            }
            else
            {
                Debug.Log("END +++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }
        }
    }
}
