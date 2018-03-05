using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        bool IsRandoEnemyType( GameObject enemy )
        {
            if( enemy.name.Contains( "Corpse" ) )
                return false;

            if( enemy.name.Contains( "Lil Jellyfish" ) )
                return false;
            
            string enemyName = enemy.name;
            string trimmedName = TrimEnemyNameToBeLoaded(enemyName);

            bool isRandoEnemyType = false;

            if( simulateReplacement )
                isRandoEnemyType = IsExactlyInList( trimmedName, EnemyRandoData.enemyTypeNames );
            else
                isRandoEnemyType = IsExactlyInList( trimmedName, loadedEnemyPrefabNames );

            return isRandoEnemyType;
        }

        bool IsRandomizerEnemy( GameObject enemy )
        {
            return IsEnemyByFSM( enemy ) && IsRandoEnemyType( enemy );
        }

        static bool IsEnemyByFSM( GameObject enemy )
        {
            return ( FSMUtility.ContainsFSM( enemy, "health_manager_enemy" ) ) || ( FSMUtility.ContainsFSM( enemy, "health_manager" ) );
        }

        static PlayMakerFSM GetEnemyFSM( GameObject enemy )
        {
            PlayMakerFSM fsm = null;

            fsm = ( FSMUtility.LocateFSM( enemy, "health_manager_enemy" ) );
            if( fsm != null )
                return fsm;
            fsm = ( FSMUtility.LocateFSM( enemy, "health_manager" ) );
            return fsm;

        }

        bool IsInList( GameObject enemy, List<string> typeList )
        {
            if( typeList.Count <= 0 )
                return false;

            var words = typeList.Select(w => @"\b" + Regex.Escape(w) + @"\b").ToArray();
            var pattern = new Regex("(" + string.Join(")|(", words) + ")");
            bool isInList = pattern.IsMatch(enemy.name);
            return isInList;
        }

        bool IsInList( string name, List<string> typeList )
        {
            if( typeList.Count <= 0 )
                return false;

            //Log( "name: " + name );
            var words = typeList.Select(w => @"\b" + Regex.Escape(w) + @"\b").ToArray();
            //Log( "Words: "+ words );
            var pattern = new Regex("(" + string.Join(")|(", words) + ")");
            //Log( "pattern: "+ pattern );
            bool isInList = pattern.IsMatch(name);
            //Log( "isInList: " + isInList );
            return isInList;
        }


        bool ListContainsString( string name, List<string> typeList )
        {
            if( typeList.Count <= 0 )
                return false;
            
            foreach(string s in typeList)
            {
                if( s.Contains( name ) )
                    return true;
            }
            return false;
        }

        int GetMatchingIndex( string name, List<string> typeList )
        {
            if( typeList.Count <= 0 )
                return -1;

            return typeList.IndexOf( name );
        }

        bool IsExactlyInList( GameObject go, List<string> typeList )
        {
            return typeList.Contains( go.name );
        }

        bool IsExactlyInList( string name, List<string> typeList )
        {
            return typeList.Contains( name );
        }

        void Debug_PrintObjectOnHit( Collider2D otherCollider, GameObject gameObject )
        {
            if( otherCollider.gameObject.name != recentHit )
            {
                Log( "(" + otherCollider.gameObject.transform.position + ") HIT: " + otherCollider.gameObject.name );
                recentHit = otherCollider.gameObject.name;
            }
        }

        public void DebugCreateLine(Vector2 from, Vector2 to, Color c )
        {
            GameObject lineObj = new GameObject("line: "+from+" -> "+to);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.SetVertexCount( 2 );
            Vector3[] points = new Vector3[]{from,to};
            lr.SetPositions( points );
            lr.SetWidth( .5f, .1f );
            if( lr.GetComponent<Renderer>() )
                lr.GetComponent<Renderer>().material = new Material( Shader.Find( "Diffuse" ) );
            if( lr.GetComponent<Renderer>() )
                lr.GetComponent<Renderer>().material.color = c;
                lr.SetColors( c, c );
        }

        //public void DebugCreateBox( Bounds b, Color c )
        //{
        //    GameObject lineObj = new GameObject("line: "+from+" -> "+to);
        //    LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        //    lr.SetVertexCount( 2 );
        //    Vector3[] points = new Vector3[]{b.min,b.max,};
        //    lr.SetPositions( b. );
        //    lr.SetWidth( .5f, .1f );
        //}

        public void DebugPrintAllObjects( string sceneName, int localIndex = -1 )
        {
            Scene namedScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName( sceneName );

            if( !namedScene.IsValid() )
                return;

            Instance.Log( "START =====================================================" );
            Instance.Log( "Printing scene hierarchy for scene: " + sceneName +" [Build index: "+ namedScene.buildIndex+"]" );
            if( localIndex >= 0 )
                Instance.Log( "Local scene index: " + localIndex );

            GameObject[] rootGameObjects = namedScene.GetRootGameObjects();
            
            foreach( GameObject go in rootGameObjects )
            {
                if( go == null )
                {
                    Log( "Scene " + sceneName + " has a null root game object! Skipping debug print scene..." );
                    break;
                }

                foreach( Transform t in go.GetComponentsInChildren<Transform>( true ) )
                {
                    string objectNameAndPath = GetObjectPath(t);
                    string logContent = objectNameAndPath;
                    if( !sceneBounds.Contains( t.position ) )
                        logContent += " ::: IsOutsideSceneBounds = true";
                    if( SkipLoadingGameObject( t.gameObject.name ) )
                        logContent += " ::: SkipLoadingGameObject = true";

                    bool isRandoEnemy = IsRandomizerEnemy( t.gameObject );
                    if( isRandoEnemy )
                        logContent += " ::: IsRandomizerEnemy = true";

                    Instance.Log( logContent );

                    //also print the all the components on the enemy so we can see what playmaker FSMs are on it
                    //if( isRandoEnemy )
                    {
                        DebugPrintObjectTree( t.gameObject, true );
                    }
                }
            }
            Instance.Log( "END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
        }

        public static void DebugPrintObjectTree( GameObject root, bool printComponents = false )
        {
            if( root == null )
                return;

            Instance.Log( "DebugPrintObjectTree START =====================================================" );
            Instance.Log( "Printing scene hierarchy for game object: " + root.name );
            foreach( Transform t in root.GetComponentsInChildren<Transform>( true ) )
            {
                string objectNameAndPath = GetObjectPath(t);
                Instance.Log( objectNameAndPath );

                if( printComponents )
                {
                    string componentHeader = "";
                    for( int i = 0; i < (objectNameAndPath.Length - t.gameObject.name.Length); ++i )
                        componentHeader += " ";

                    foreach( Component c in t.GetComponents<Component>() )
                    {
                        Instance.Log( componentHeader + @" \--Component: " + c.GetType().Name );

                        if( c as Transform != null )
                        {
                            Instance.Log( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                            Instance.Log( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                            Instance.Log( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
                        }

                        if( c as BoxCollider2D != null )
                        {
                            Instance.Log( componentHeader + @" \--BoxCollider2D Size: " + ( c as BoxCollider2D ).size );
                            Instance.Log( componentHeader + @" \--BoxCollider2D Offset: " + ( c as BoxCollider2D ).offset );
                            Instance.Log( componentHeader + @" \--BoxCollider2D Bounds-Min: " + ( c as BoxCollider2D ).bounds.min );
                            Instance.Log( componentHeader + @" \--BoxCollider2D Bounds-Max: " + ( c as BoxCollider2D ).bounds.max );
                        }

                        if(c as PlayMakerFSM != null)
                        {
                            Instance.Log( componentHeader + @" \--PFSM Name: " + ( c as PlayMakerFSM ).FsmName );
                            Instance.Log( componentHeader + @" \--PFSM FsmDescription: " + ( c as PlayMakerFSM ).FsmDescription );

                            string[] stateNames = ( c as PlayMakerFSM ).FsmStates.Select(x=>x.Name).ToArray();

                            Instance.Log( componentHeader + @" \--PFSM StateNames" );
                            foreach( string s in stateNames )
                            {
                                Instance.Log( componentHeader + @" \----PFSM StateName: " + s );

                                var selected = ( c as PlayMakerFSM ).FsmStates.Select(x=>x).Where(x=>x.Name == s).ToArray();
                                var transitions = selected[0].Transitions.ToArray();
                                var actions = selected[0].Actions.ToArray();

                                string[] trans = transitions.Select(x=> {return "Transition on "+x.EventName+" to state "+x.ToState; } ).ToArray();

                                string[] actionNames = actions.Select(x=> {return "Actions on "+selected[0].Name+" ::: "+x.Name; } ).ToArray();

                                foreach( string x in trans )
                                    Instance.Log( componentHeader + @" \----PFSM ---- Transitions for state: " + x );

                                foreach( string x in actionNames )
                                    Instance.Log( componentHeader + @" \----PFSM ---- Actions for state: " + x );
                            }
                            Instance.Log( componentHeader + @" \--PFSM Active: " + ( c as PlayMakerFSM ).Active );
                            Instance.Log( componentHeader + @" \--PFSM ActiveStateName: " + ( c as PlayMakerFSM ).ActiveStateName );
                            


                        }
                    }
                }
            }
            Instance.Log( "DebugPrintObjectTree END +++++++++++++++++++++++++++++++++++++++++++++++++++++++" );
        }


        public static string GetObjectPath( Transform t )
        {
            string objStr = t.name;

            if( t.parent != null )
                objStr = GetObjectPath( t.parent ) + "\\" + t.name;

            return objStr;
        }

        public static GameObject FindGameObjectInChildren( GameObject obj, string name )
        {
            foreach( var t in obj.GetComponentsInChildren<Transform>( true ) )
            {
                if( t.name == name )
                    return t.gameObject;
            }
            return null;
        }


        public static void ChangeEnemyGetoRates(GameObject enemy, float multiplier)
        {
            if( !IsEnemyByFSM( enemy ) )
                return;

            PlayMakerFSM fsm = GetEnemyFSM(enemy);
            if(fsm != null)
            {
                fsm.FsmVariables.GetFsmInt( "Geo Small" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Small" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Small Extra" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Medium" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Med Extra" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Large" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Large" ).Value * multiplier );
                fsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value = (int)( fsm.FsmVariables.GetFsmInt( "Geo Large Extra" ).Value * multiplier );
            }
        }


        bool SkipLoadingGameObject( string name )
        {
            if( name.Contains( "Corpse" ) )
                return true;

            if( name.Contains( "Rubble" ) )
                return true;

            if( name.Contains( "Chunk" ) )
                return true;

            if( name.Contains( "Rock" ) )
                return true;

            if( name.Contains( "Particle" ) )
                return true;

            if( name.Contains( "Prompt" ) )
                return true;

            if( name.Contains( "death" ) )
                return true;

            if( name.Contains( "Layer" ) )
                return true;

            if( name.Contains( "Terrain" ) )
                return true;

            if( name.Contains( "Rando" ) )
                return true;

            if( name.Contains( "Summon" ) )
                return true;

            return false;
        }


        bool SkipRandomizeEnemy( string name )
        {
            if( name.Contains( "Corpse" ) )
                return true;

            //if( name.Contains( "Rubble" ) )
            //    return true;

            //if( name.Contains( "Chunk" ) )
            //    return true;

            //if( name.Contains( "Rock" ) )
            //    return true;

            //if( name.Contains( "Particle" ) )
            //    return true;

            //if( name.Contains( "Prompt" ) )
            //    return true;

            //if( name.Contains( "death" ) )
            //    return true;

            //if( name.Contains( "Layer" ) )
            //    return true;

            //if( name.Contains( "Terrain" ) )
            //    return true;

            if( name.Contains( "Rando" ) )
                return true;

            //if( name.Contains( "Summon" ) )
            //    return true;

            //if( name.Contains( "Mushroom Brawler" ) )
            //    return true;

            //if( name.Contains( "Lesser Mawlek" ) )
            //    return true;

            //don't randomize mender bug or else players will fall onto an enemy every time
            //they enter crossroads
            if( name.Contains( "Mender" ) )
                return true;

            return false;
        }

        bool ShouldSkipRandomizingScene(string name)
        {
            //if( name.Contains( "Room_Coloss" ) )
            //    return true;
            //if( name.Contains( "Ruins2_09" ) )
            //    return true;
            //if( name.Contains( "Crossroads_09" ) )
            //    return true;
            //if( name.Contains( "Crossroads_08" ) )
            //    return true;
            //if( name.Contains( "Crossroads_04" ) )
            //    return true;
            return false;
        }

        string TrimEnemyNameToBeLoaded( string name )
        {
            //trim off "(Clone)" from the word, if it's there
            int index = name.LastIndexOf("(Clone)");
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (1)" from the word, if it's there
            index = name.LastIndexOf( " (1)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (2)" from the word, if it's there
            index = name.LastIndexOf( " (2)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (3)" from the word, if it's there
            index = name.LastIndexOf( " (3)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (4)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (5)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (6)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (7)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (8)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (9)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (4)" from the word, if it's there
            index = name.LastIndexOf( " (10)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (10)" from the word, if it's there
            index = name.LastIndexOf( " (11)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (10)" from the word, if it's there
            index = name.LastIndexOf( " (12)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            //trim off " (10)" from the word, if it's there
            index = name.LastIndexOf( " (13)" );
            if( index > 0 )
                name = name.Substring( 0, index );

            if( name != "Zombie Spider 1" && name != "Zombie Spider 2" )
            {
                //trim off " 1" from the word, if it's there
                index = name.LastIndexOf( " 1" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 2" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 3" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 4" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 5" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 6" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 7" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 8" );
                if( index > 0 )
                    name = name.Substring( 0, index );

                index = name.LastIndexOf( " 9" );
                if( index > 0 )
                    name = name.Substring( 0, index );
            }

            if( name.Contains( "Zombie Fungus" ) )
            {
                //trim off " B" from the word, if it's there
                index = name.LastIndexOf( " B" );
                if( index > 0 )
                    name = name.Substring( 0, index );
            }

            //trim off " New" from the word, if it's there
            if( name.Contains( "Electric Mage" ) )
            {
                index = name.LastIndexOf( " New" );
                if( index > 0 )
                    name = name.Substring( 0, index );
            }

            return name;
        }
    }
}
