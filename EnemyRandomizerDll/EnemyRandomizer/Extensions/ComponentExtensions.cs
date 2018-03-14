using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class ComponentExtensions
    {
        public static void PrintComponentType( this Component c, string componentHeader = "" )
        {
            if( c == null )
                return;
            Dev.Log( componentHeader + @" \--Component: " + c.GetType().Name );
        }

        public static void PrintTransform( this Component c, string componentHeader = "" )
        {
            if( c as Transform != null )
            {
                Dev.Log( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                Dev.Log( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                Dev.Log( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
            }
        }

        public static void PrintBoxCollider2D( this Component c, string componentHeader = "" )
        {
            if( c as BoxCollider2D != null )
            {
                Dev.Log( componentHeader + @" \--BoxCollider2D Size: " + ( c as BoxCollider2D ).size );
                Dev.Log( componentHeader + @" \--BoxCollider2D Offset: " + ( c as BoxCollider2D ).offset );
                Dev.Log( componentHeader + @" \--BoxCollider2D Bounds-Min: " + ( c as BoxCollider2D ).bounds.min );
                Dev.Log( componentHeader + @" \--BoxCollider2D Bounds-Max: " + ( c as BoxCollider2D ).bounds.max );
            }
        }

        public static void PrintPlayMakerFSM( this Component c, string componentHeader = "" )
        {
            if( c as PlayMakerFSM != null )
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
                }
                Dev.Log( componentHeader + @" \--PFSM Active: " + ( c as PlayMakerFSM ).Active );
                Dev.Log( componentHeader + @" \--PFSM ActiveStateName: " + ( c as PlayMakerFSM ).ActiveStateName );
            }
        }

    }
}
