using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class ComponentExtensions
    {
        public static void PrintComponentType( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c == null )
                return;

            if( file != null )
            {
                file.WriteLine( componentHeader + @" \--Component: " + c.GetType().Name );
            }
            else
            {
                Dev.Log( componentHeader + @" \--Component: " + c.GetType().Name );
            }
        }

        public static void PrintPersistentBoolItem(this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as PersistentBoolItem != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--PersistentBoolItem semiPersistent: " + ( c as PersistentBoolItem ).semiPersistent );
                    if( ( c as PersistentBoolItem ).persistentBoolData != null )
                    {
                        file.WriteLine( componentHeader + @" \--PersistentBoolItem id: " + ( c as PersistentBoolItem ).persistentBoolData.id );
                        file.WriteLine( componentHeader + @" \--PersistentBoolItem sceneName: " + ( c as PersistentBoolItem ).persistentBoolData.sceneName );
                        file.WriteLine( componentHeader + @" \--PersistentBoolItem activated: " + ( c as PersistentBoolItem ).persistentBoolData.activated );
                    }
                }
                else
                {
                    Dev.Log( componentHeader + @" \--PersistentBoolItem semiPersistent: " + ( c as PersistentBoolItem ).semiPersistent );
                    if( ( c as PersistentBoolItem ).persistentBoolData != null )
                    {
                        Dev.Log( componentHeader + @" \--PersistentBoolItem id: " + ( c as PersistentBoolItem ).persistentBoolData.id );
                        Dev.Log( componentHeader + @" \--PersistentBoolItem sceneName: " + ( c as PersistentBoolItem ).persistentBoolData.sceneName );
                        Dev.Log( componentHeader + @" \--PersistentBoolItem activated: " + ( c as PersistentBoolItem ).persistentBoolData.activated );
                    }
                }
            }
        }

        public static void PrintTransform( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as Transform != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--GameObject layer: " + ( c as Transform ).gameObject.layer );
                    file.WriteLine( componentHeader + @" \--GameObject tag: " + ( c as Transform ).gameObject.tag );
                    file.WriteLine( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                    file.WriteLine( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                    file.WriteLine( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--GameObject layer: " + ( c as Transform ).gameObject.layer );
                    Dev.Log( componentHeader + @" \--GameObject tag: " + ( c as Transform ).gameObject.tag );
                    Dev.Log( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                    Dev.Log( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                    Dev.Log( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
                }
            }
        }

        public static void PrintBoxCollider2D( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as BoxCollider2D != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Size: " + ( c as BoxCollider2D ).size );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Offset: " + ( c as BoxCollider2D ).offset );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Bounds-Min: " + ( c as BoxCollider2D ).bounds.min );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Bounds-Max: " + ( c as BoxCollider2D ).bounds.max );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D isTrigger: " + ( c as BoxCollider2D ).isTrigger );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--BoxCollider2D Size: " + ( c as BoxCollider2D ).size );
                    Dev.Log( componentHeader + @" \--BoxCollider2D Offset: " + ( c as BoxCollider2D ).offset );
                    Dev.Log( componentHeader + @" \--BoxCollider2D Bounds-Min: " + ( c as BoxCollider2D ).bounds.min );
                    Dev.Log( componentHeader + @" \--BoxCollider2D Bounds-Max: " + ( c as BoxCollider2D ).bounds.max );
                    Dev.Log( componentHeader + @" \--BoxCollider2D isTrigger: " + ( c as BoxCollider2D ).isTrigger );
                }
            }
        }

        public static void PrintPlayMakerFSM( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as PlayMakerFSM != null )
            {
                //don't print this one....
                if( ( c as PlayMakerFSM ).FsmName == "recoil" )
                    return;

                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--PFSM Name: " + ( c as PlayMakerFSM ).FsmName );
                    file.WriteLine( componentHeader + @" \--PFSM FsmDescription: " + ( c as PlayMakerFSM ).FsmDescription );

                    string[] stateNames = ( c as PlayMakerFSM ).FsmStates.Select(x=>x.Name).ToArray();

                    file.WriteLine( componentHeader + @" \--PFSM StateNames" );
                    foreach( string s in stateNames )
                    {
                        file.WriteLine( componentHeader + @" \----PFSM StateName: " + s );

                        var selected = ( c as PlayMakerFSM ).FsmStates.Select(x=>x).Where(x=>x.Name == s).ToArray();
                        var transitions = selected[0].Transitions.ToArray();
                        var actions = selected[0].Actions.ToArray();

                        string[] trans = transitions.Select(x=> {return "Transition on "+x.EventName+" to state "+x.ToState; } ).ToArray();

                        //TODO: figure out why x.Name is empty????
                        string[] actionNames = actions.Select(x=> {return "Actions on "+selected[0].Name+" ::: "+x.GetType().Name; } ).ToArray();

                        foreach( string x in trans )
                            file.WriteLine( componentHeader + @" \----PFSM ---- Transitions for state: " + x );

                        foreach( string x in actionNames )
                            file.WriteLine( componentHeader + @" \----PFSM ---- Actions for state: " + x );

                        if( actions != null )
                        {
                            foreach( var x in actions )
                            {
                                var pdb = ( x as HutongGames.PlayMaker.Actions.PlayerDataBoolTest );
                                if( pdb != null && pdb.boolName != null )
                                {
                                    file.WriteLine( componentHeader + @" \----PFSM ---- PlayerDataBoolTest (boolName) = " + pdb.boolName.Value );
                                    try
                                    {
                                        if( HeroController.instance != null && HeroController.instance.playerData != null )
                                        {
                                            file.WriteLine( componentHeader + @" \----PFSM ---- PlayerDataBoolTest (bool.Value) = " + HeroController.instance.playerData.GetBoolInternal( pdb.boolName.Value ) );
                                        }
                                    }
                                    catch( Exception )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- PlayerDataBoolTest (bool.Value) = " + "null? Player data not initialized" );
                                    }
                                }
                            }
                        }
                    }
                    file.WriteLine( componentHeader + @" \--PFSM Active: " + ( c as PlayMakerFSM ).Active );
                    file.WriteLine( componentHeader + @" \--PFSM ActiveStateName: " + ( c as PlayMakerFSM ).ActiveStateName );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--PFSM Name: " + ( c as PlayMakerFSM ).FsmName );
                    Dev.Log( componentHeader + @" \--PFSM FsmDescription: " + ( c as PlayMakerFSM ).FsmDescription );

                    string[] stateNames = ( c as PlayMakerFSM ).FsmStates.Select(x=>x.Name).ToArray();

                    Dev.Log( componentHeader + @" \--PFSM StateNames" );
                    foreach( string s in stateNames )
                    {
                        Dev.Log( componentHeader + @" \----PFSM StateName: " + s );

                        var selected = ( c as PlayMakerFSM ).FsmStates.Select(x=>x).Where(x=>x.Name == s).ToArray();
                        var transitions = selected[0].Transitions.ToArray();
                        var actions = selected[0].Actions.ToArray();

                        string[] trans = transitions.Select(x=> {return "Transition on "+x.EventName+" to state "+x.ToState; } ).ToArray();

                        //TODO: figure out why x.Name is empty????
                        string[] actionNames = actions.Select(x=> {return "Actions on "+selected[0].Name+" ::: "+x.GetType().Name; } ).ToArray();

                        foreach( string x in trans )
                            Dev.Log( componentHeader + @" \----PFSM ---- Transitions for state: " + x );

                        foreach( string x in actionNames )
                            Dev.Log( componentHeader + @" \----PFSM ---- Actions for state: " + x );

                        if( actions != null )
                        {
                            foreach( var x in actions )
                            {
                                var pdb = ( x as HutongGames.PlayMaker.Actions.PlayerDataBoolTest );
                                if( pdb != null && pdb.boolName != null )
                                {
                                    Dev.Log( componentHeader + @" \----PFSM ---- PlayerDataBoolTest (boolName) = " + pdb.boolName.Value );
                                    try
                                    {
                                        if( HeroController.instance != null && HeroController.instance.playerData != null )
                                        {
                                            Dev.Log( componentHeader + @" \----PFSM ---- PlayerDataBoolTest (bool.Value) = " + HeroController.instance.playerData.GetBoolInternal( pdb.boolName.Value ) );
                                        }
                                    }
                                    catch( Exception )
                                    {
                                        Dev.Log( componentHeader + @" \----PFSM ---- PlayerDataBoolTest (bool.Value) = " + "null? Player data not initialized" );
                                    }
                                }
                            }
                        }
                    }
                    Dev.Log( componentHeader + @" \--PFSM Active: " + ( c as PlayMakerFSM ).Active );
                    Dev.Log( componentHeader + @" \--PFSM ActiveStateName: " + ( c as PlayMakerFSM ).ActiveStateName );
                }

            }
        }

    }
}
