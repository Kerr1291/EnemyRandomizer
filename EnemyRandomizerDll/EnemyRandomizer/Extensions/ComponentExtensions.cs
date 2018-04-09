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
                    file.WriteLine( componentHeader + @" \--GameObject activeSelf: " + ( c as Transform ).gameObject.activeSelf );
                    file.WriteLine( componentHeader + @" \--GameObject layer: " + ( c as Transform ).gameObject.layer );
                    file.WriteLine( componentHeader + @" \--GameObject tag: " + ( c as Transform ).gameObject.tag );
                    file.WriteLine( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                    file.WriteLine( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                    file.WriteLine( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--GameObject activeSelf: " + ( c as Transform ).gameObject.activeSelf );
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

        public static void PrintCircleCollider2D( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as CircleCollider2D != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--CircleCollider2D radius: " + ( c as CircleCollider2D ).radius );
                    file.WriteLine( componentHeader + @" \--CircleCollider2D Offset: " + ( c as CircleCollider2D ).offset );
                    file.WriteLine( componentHeader + @" \--CircleCollider2D Bounds-Min: " + ( c as CircleCollider2D ).bounds.min );
                    file.WriteLine( componentHeader + @" \--CircleCollider2D Bounds-Max: " + ( c as CircleCollider2D ).bounds.max );
                    file.WriteLine( componentHeader + @" \--CircleCollider2D isTrigger: " + ( c as CircleCollider2D ).isTrigger );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--CircleCollider2D radius: " + ( c as CircleCollider2D ).radius );
                    Dev.Log( componentHeader + @" \--CircleCollider2D Offset: " + ( c as CircleCollider2D ).offset );
                    Dev.Log( componentHeader + @" \--CircleCollider2D Bounds-Min: " + ( c as CircleCollider2D ).bounds.min );
                    Dev.Log( componentHeader + @" \--CircleCollider2D Bounds-Max: " + ( c as CircleCollider2D ).bounds.max );
                    Dev.Log( componentHeader + @" \--CircleCollider2D isTrigger: " + ( c as CircleCollider2D ).isTrigger );
                }
            }
        }

        public static void PrintPolygonCollider2D( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as PolygonCollider2D != null )
            {
                if( file != null )
                {
                    foreach(var p in ( c as PolygonCollider2D ).points )
                        file.WriteLine( componentHeader + @" \--PolygonCollider2D points: " + p );
                    file.WriteLine( componentHeader + @" \--PolygonCollider2D Offset: " + ( c as PolygonCollider2D ).offset );
                    file.WriteLine( componentHeader + @" \--PolygonCollider2D Bounds-Min: " + ( c as PolygonCollider2D ).bounds.min );
                    file.WriteLine( componentHeader + @" \--PolygonCollider2D Bounds-Max: " + ( c as PolygonCollider2D ).bounds.max );
                    file.WriteLine( componentHeader + @" \--PolygonCollider2D isTrigger: " + ( c as PolygonCollider2D ).isTrigger );
                }
                else
                {
                    foreach( var p in ( c as PolygonCollider2D ).points )
                        Dev.Log( componentHeader + @" \--PolygonCollider2D points: " + p );
                    Dev.Log( componentHeader + @" \--PolygonCollider2D Offset: " + ( c as PolygonCollider2D ).offset );
                    Dev.Log( componentHeader + @" \--PolygonCollider2D Bounds-Min: " + ( c as PolygonCollider2D ).bounds.min );
                    Dev.Log( componentHeader + @" \--PolygonCollider2D Bounds-Max: " + ( c as PolygonCollider2D ).bounds.max );
                    Dev.Log( componentHeader + @" \--PolygonCollider2D isTrigger: " + ( c as PolygonCollider2D ).isTrigger );
                }
            }
        }

        public static void PrintRigidbody2D( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as Rigidbody2D != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D mass: " + ( c as Rigidbody2D ).mass );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D velocity: " + ( c as Rigidbody2D ).velocity );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D drag: " + ( c as Rigidbody2D ).drag );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D angularVelocity: " + ( c as Rigidbody2D ).angularVelocity );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D angularDrag: " + ( c as Rigidbody2D ).angularDrag );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D gravityScale: " + ( c as Rigidbody2D ).gravityScale );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D isKinematic: " + ( c as Rigidbody2D ).isKinematic );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D interpolation: " + ( c as Rigidbody2D ).interpolation );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D freezeRotation: " + ( c as Rigidbody2D ).freezeRotation );
                    file.WriteLine( componentHeader + @" \--PrintRigidbody2D collisionDetectionMode: " + ( c as Rigidbody2D ).collisionDetectionMode );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D mass: " + ( c as Rigidbody2D ).mass );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D velocity: " + ( c as Rigidbody2D ).velocity );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D drag: " + ( c as Rigidbody2D ).drag );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D angularVelocity: " + ( c as Rigidbody2D ).angularVelocity );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D angularDrag: " + ( c as Rigidbody2D ).angularDrag );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D gravityScale: " + ( c as Rigidbody2D ).gravityScale );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D isKinematic: " + ( c as Rigidbody2D ).isKinematic );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D interpolation: " + ( c as Rigidbody2D ).interpolation );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D freezeRotation: " + ( c as Rigidbody2D ).freezeRotation );
                    Dev.Log( componentHeader + @" \--PrintRigidbody2D collisionDetectionMode: " + ( c as Rigidbody2D ).collisionDetectionMode );
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

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.BuildString );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (addToEnd) = " + a?.addToEnd );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (addToEnd?.Name) = " + a?.addToEnd?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (separator) = " + a?.separator );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (separator?.Name) = " + a?.separator?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BuildString (storeResult) = " + a?.storeResult );
                                        if( a?.stringParts != null )
                                            foreach( var e in a?.stringParts )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "stringParts", e?.Value ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "stringParts Name", e?.Name ) );
                                            }
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetLanguageString );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (convName) = " + a?.convName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (convName?.Name) = " + a?.convName?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (sheetName) = " + a?.sheetName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (sheetName?.Name) = " + a?.sheetName?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (storeValue) = " + a?.storeValue );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetLanguageString (storeValue?.Name) = " + a?.storeValue?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetStringValue );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetStringValue (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetStringValue (stringValue) = " + a?.stringValue );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetStringValue (stringValue?.Name) = " + a?.stringValue?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetStringValue (stringVariable) = " + a?.stringVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetStringValue (stringVariable?.Name) = " + a?.stringVariable?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetIntValue );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIntValue (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIntValue (intValue) = " + a?.intValue );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIntValue (intValue?.Name) = " + a?.intValue?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIntValue (intVariable) = " + a?.intVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIntValue (intVariable?.Name) = " + a?.intVariable?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetFloatValue );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFloatValue (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFloatValue (floatValue) = " + a?.floatValue );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFloatValue (floatValue?.Name) = " + a?.floatValue?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFloatValue (floatVariable) = " + a?.floatVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFloatValue (floatVariable?.Name) = " + a?.floatVariable?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetBoolValue );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetBoolValue (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetBoolValue (boolValue) = " + a?.boolValue );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetBoolValue (boolValue?.Name) = " + a?.boolValue?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetBoolValue (boolVariable) = " + a?.boolVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetBoolValue (boolVariable?.Name) = " + a?.boolVariable?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.ActivateGameObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- ActivateGameObject (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- ActivateGameObject (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- ActivateGameObject (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- ActivateGameObject (activate) = " + a?.activate );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetPosition );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (GameObject Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (vector) = " + a?.vector );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (vector Name) = " + a?.vector?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (space) = " + a?.space );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.x ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.y ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.z ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.vector ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetPosition );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (GameObject Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (vector) = " + a?.vector );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (vector Name) = " + a?.vector?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (space) = " + a?.space );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (lateUpdate) = " + a?.lateUpdate );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.x ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.y ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.z ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.vector ) );

                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetRotation );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (GameObject Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (vector) = " + a?.vector );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (vector name) = " + a?.vector?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (lateUpdate) = " + a?.lateUpdate );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (space) = " + a?.space );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (xAngle) = " + a?.xAngle );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (yAngle) = " + a?.yAngle );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (zAngle) = " + a?.zAngle );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (xAngle name) = " + a?.xAngle?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (yAngle name) = " + a?.yAngle?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetRotation (zAngle name) = " + a?.zAngle?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetScale );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (GameObject Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (GameObject V) = " + a?.gameObject?.GameObject?.Value );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (GameObject VName) = " + a?.gameObject?.GameObject?.Value?.name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (vector) = " + a?.vector );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (vector Name) = " + a?.vector?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (lateUpdate) = " + a?.lateUpdate );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (x) = " + a?.x );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (x Name) = " + a?.x?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (y) = " + a?.y );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (y Name) = " + a?.y?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (z) = " + a?.z );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetScale (z Name) = " + a?.z?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatCompare );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (float1) = " + a?.float1 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (float2) = " + a?.float2 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (float1 Name) = " + a?.float1?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (float2 Name) = " + a?.float2?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (greaterThan) = " + a?.greaterThan?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (lessThan) = " + a?.lessThan?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (equal) = " + a?.equal?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (everyFrame) = " + a?.everyFrame );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.IntAdd );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntAdd (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntAdd (add) = " + a?.add );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntAdd (add?.Name) = " + a?.add?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntAdd (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntAdd (intVariable) = " + a?.intVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntAdd (intVariable?.Name) = " + a?.intVariable?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.IntCompare );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (integer1) = " + a?.integer1 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (integer2) = " + a?.integer2 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (greaterThan) = " + a?.greaterThan?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (lessThan) = " + a?.lessThan?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (equal) = " + a?.equal?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- IntCompare (everyFrame) = " + a?.everyFrame );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetFsmInt );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (fsmName) = " + a?.fsmName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (variableName) = " + a?.variableName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetFsmInt (storeValue) = " + a?.storeValue );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetFsmBool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmBool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmBool (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmBool (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmBool (fsmName) = " + a?.fsmName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmBool (variableName) = " + a?.variableName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmBool (setValue) = " + a?.setValue );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetFsmString );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmString (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmString (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmString (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmString (fsmName) = " + a?.fsmName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmString (variableName) = " + a?.variableName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmString (setValue) = " + a?.setValue );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SendEventByName );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (delay) = " + a?.delay );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.excludeSelf) = " + a?.eventTarget?.excludeSelf );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.fsmComponent?.FsmName) = " + a?.eventTarget?.fsmComponent?.FsmName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.fsmName) = " + a?.eventTarget?.fsmName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.gameObject?.GameObject?.Name) = " + a?.eventTarget?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.sendToChildren) = " + a?.eventTarget?.sendToChildren );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.target) = " + a?.eventTarget?.target );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (eventTarget?.fsmComponent?.gameObject?.name ) = " + a?.eventTarget?.fsmComponent?.gameObject?.name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (sendEvent) = " + a?.sendEvent );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.position ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.rotation ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spawnPoint ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.storeObject ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.position ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.FSM ) );
                                        //file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (FSM) = " + a?.FSM );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (FSMEvent) = " + a?.FSMEvent );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spawnPoint ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.CreateObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.position ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.rotation ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spawnPoint ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.storeObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (networkGroup) = " + a?.networkGroup );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (networkInstantiate) = " + a?.networkInstantiate );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.RayCast2d );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (GameObject) = " + a?.fromGameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (OwnerOption) = " + a?.fromGameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (fromPosition) = " + a?.fromPosition );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (direction) = " + a?.direction );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (distance) = " + a?.distance );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (hitEvent) = " + a?.hitEvent );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (layerMask) = " + a?.layerMask );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (maxDepth) = " + a?.maxDepth );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (minDepth) = " + a?.minDepth );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (space) = " + a?.space );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (storeDidHit) = " + a?.storeDidHit );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RayCast2d (storeHitDistance) = " + a?.storeHitDistance );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.BoolTest );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (boolVariable) = " + a?.boolVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (boolVariable?.Name) = " + a?.boolVariable?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (isFalse) = " + a?.isFalse?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (isTrue) = " + a?.isTrue?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.BoolTestMulti );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTestMulti (Name) = " + a?.Name );
                                        if(a?.boolStates != null)
                                        foreach( var e in a?.boolStates )
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( e) );
                                        if( a?.boolVariables != null )
                                            foreach( var e in a?.boolVariables )
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( e ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.storeResult ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTestMulti (falseEvent) = " + a?.falseEvent?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTestMulti (trueEvent) = " + a?.trueEvent?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatTestToBool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (everyFrame) = " + a?.everyFrame );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (float1) = " + a?.float1 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (float1 Name) = " + a?.float1?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (float2) = " + a?.float2 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (float2 Name) = " + a?.float2?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (greaterThanBool) = " + a?.greaterThanBool );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (lessThanBool) = " + a?.lessThanBool );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (equalBool) = " + a?.equalBool );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatTestToBool (tolerance) = " + a?.tolerance );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.RandomFloat );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RandomFloat (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RandomFloat (min) = " + a?.min );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RandomFloat (max) = " + a?.max );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- RandomFloat (storeResult) = " + a?.storeResult );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatOperator );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.float1 ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.float2 ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "operation",a?.operation ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.storeResult ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatOperator (everyFrame) = " + a?.everyFrame );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatMultiply );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatMultiply (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatMultiply (floatVariable) = " + a?.floatVariable );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatMultiply (multiplyBy) = " + a?.multiplyBy );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.Tk2dPlayAnimation );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- Tk2dPlayAnimation (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- Tk2dPlayAnimation (animLibName) = " + a?.animLibName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- Tk2dPlayAnimation (clipName) = " + a?.clipName );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetVelocity2d );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (gameObject?.GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (vector?.Value) = " + a?.vector?.Value );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (x) = " + a?.x );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (y) = " + a?.y );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (vector?.Value name) = " + a?.vector?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (x name) = " + a?.x?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (y name) = " + a?.y?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (everyFrame) = " + a?.everyFrame );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetIsKinematic2d );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIsKinematic2d (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIsKinematic2d (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetIsKinematic2d (isKinematic) = " + a?.isKinematic );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetCollider );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetCollider (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetCollider (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetCollider (active) = " + a?.active );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetMeshRenderer );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetMeshRenderer (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetMeshRenderer (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetMeshRenderer (active) = " + a?.active );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetGravity2dScale );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetGravity2dScale (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetGravity2dScale (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetGravity2dScale (gravityScale?.Value) = " + a?.gravityScale?.Value );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetVelocity2d );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (vector?.Value) = " + a?.vector?.Value );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (x) = " + a?.x );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (y) = " + a?.y );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (space) = " + a?.space );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetVelocity2d (everyFrame) = " + a?.everyFrame );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetPlayerDataBool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPlayerDataBool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPlayerDataBool (boolName) = " + a?.boolName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPlayerDataBool (storeValue) = " + a?.storeValue );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetPlayerDataInt );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPlayerDataInt (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPlayerDataInt (boolName) = " + a?.intName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPlayerDataInt (storeValue) = " + a?.storeValue );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetFsmGameObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmGameObject (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmGameObject (fsmName) = " + a?.fsmName );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmGameObject (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmGameObject (setValue?.Name) = " + a?.setValue?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetFsmGameObject (variableName) = " + a?.variableName );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FlingObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObject (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObject (flungObject?.GameObject?.Name) = " + a?.flungObject?.GameObject?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GameObjectIsNull );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GameObjectIsNull (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GameObjectIsNull (gameObject?.Name) = " + a?.gameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GameObjectIsNull (isNotNull?.Name) = " + a?.isNotNull?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GameObjectIsNull (isNull?.Name) = " + a?.isNull?.Name );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetBoxCollider2DSizeVector );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject1?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.offset ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.size ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SendRandomEvent );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delay ) );
                                        if( a?.events != null )
                                            foreach(var e in a?.events)
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Event", e?.Name ) );
                                        if( a?.weights != null )
                                            foreach( var e in a?.weights )
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Weight", e?.Value ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SendRandomEventV2 );
                                    if( a != null )
                                    {
                                        foreach( var e in a?.trackingInts )
                                        {
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "TrackingIntsName", e?.Name ) );
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "TrackingIntsValue", e?.Value ) );
                                        }
                                        if( a?.events != null )
                                            foreach( var e in a?.events )
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Event", e?.Name ) );
                                        if( a?.weights != null )
                                            foreach( var e in a?.weights )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Weight", e?.Value ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Weight Name", e?.Name ) );
                                            }
                                        if( a?.eventMax != null )
                                            foreach( var e in a?.eventMax )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "eventMax", e?.Value ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "eventMax Name", e?.Name ) );
                                            }
                                    }
                                }
                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SendRandomEventV3 );
                                    if( a != null )
                                    {
                                        if( a?.trackingInts != null )
                                            foreach( var e in a?.trackingInts )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "TrackingIntsName", e?.Name ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "TrackingIntsValue", e?.Value ) );
                                            }
                                        if( a?.trackingIntsMissed != null )
                                            foreach( var e in a?.trackingIntsMissed )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "trackingIntsMissed", e?.Name ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "trackingIntsMissed", e?.Value ) );
                                            }
                                        if( a?.events != null )
                                            foreach( var e in a?.events )
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Event", e?.Name ) );
                                        if( a?.weights != null )
                                            foreach( var e in a?.weights )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Weight", e?.Value ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Weight Name", e?.Name ) );
                                            }
                                        if( a?.eventMax != null )
                                            foreach( var e in a?.eventMax )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "eventMax", e?.Value ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "eventMax Name", e?.Name ) );
                                            }
                                        if( a?.missedMax != null )
                                            foreach( var e in a?.missedMax )
                                            {
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "missedMax", e?.Value ) );
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "missedMax Name", e?.Name ) );
                                            }
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.WaitRandom );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "finishEvent", a?.finishEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "realTime", a?.realTime ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.timeMin ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.timeMax ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.Wait );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "finishEvent", a?.finishEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "realTime", a?.realTime ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.time ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "animationCompleteEvent", a?.animationCompleteEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "animationTriggerEvent", a?.animationTriggerEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.clipName ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.AudioPlayerOneShot );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.audioPlayer ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delay ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.pitchMax ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.pitchMin ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spawnPoint ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.storePlayer ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.volume ) );
                                        if( a?.weights != null )
                                            foreach( var e in a?.weights )
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "Weight", e?.Value ) );
                                        if( a?.audioClips != null )
                                            foreach( var e in a?.audioClips )
                                            file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "audioClips", e?.name ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.AudioPlaySimple );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.volume ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.oneShotClip ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetScale );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame",a?.everyFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "space", a?.space ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.vector ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.xScale ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.yScale ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.zScale ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.CheckCollisionSideEnter );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.ignoreTriggers ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.bottomHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "bottomHitEvent", a?.bottomHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.leftHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "leftHitEvent", a?.leftHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.rightHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "rightHitEvent", a?.rightHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.topHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "topHitEvent", a?.topHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "otherLayer", a?.otherLayer ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "otherLayerNumber", a?.otherLayerNumber ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.CheckCollisionSide );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.ignoreTriggers ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.bottomHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "bottomHitEvent", a?.bottomHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.leftHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "leftHitEvent", a?.leftHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.rightHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "rightHitEvent", a?.rightHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.topHit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "topHitEvent", a?.topHitEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "otherLayer", a?.otherLayer ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "otherLayerNumber", a?.otherLayerNumber ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.DecelerateXY );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.decelerationX ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.decelerationY ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.DecelerateV2 );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.deceleration ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FaceObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.objectA ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.objectB ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.newAnimationClip ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "playNewAnimation", a?.playNewAnimation ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "resetFrame", a?.resetFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spriteFacesRight ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatSubtract );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.floatVariable ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "perSecond", a?.perSecond ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.subtract ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatInRange );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.floatVariable ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.boolVariable ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "falseEvent", a?.falseEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "trueEvent", a?.trueEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.lowerValue ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.upperValue ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.NextFrameEvent );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "sendEvent", a?.sendEvent?.Name ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetBoxColliderTrigger );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name",a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.trigger ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FireAtTarget );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.target ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "target value", a?.target?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "target value name", a?.target?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "target name", a?.target?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.position ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "position name", a?.position?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.speed ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spread ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.PlayParticleEmitter );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.emit ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetVelocityAsAngle );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.angle ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.speed ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SendEvent );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "sendEvent", a?.sendEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delay ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.AudioPlayInState );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString(a?.volume) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString(a?.gameObject?.GameObject ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.volume ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.audioClip", a?.audioClip ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.audioClip?.Name",a?.audioClip?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.audioClip?.Value", a?.audioClip?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.audioPlayer ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.audioPlayer?.Name",a?.audioPlayer?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.audioPlayer?.Value", a?.audioPlayer?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.spawnPoint ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.spawnPoint?.Name", a?.spawnPoint?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "a?.spawnPoint?.Value", a?.spawnPoint?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.pitchMax ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.pitchMin ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delay ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.TransitionToAudioSnapshot );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "snapshot? ", a?.snapshot ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "snapshot?. type", a?.snapshot?.GetType()?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "snapshot?.Name",a?.snapshot?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.transitionTime ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.ApplyMusicCue );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delayTime ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "musicCue?.Name", a?.musicCue?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "musicCue", a?.musicCue ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "musicCue type", a?.musicCue?.GetType()?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.transitionTime ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetGameObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.variable ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.CheckTargetDirection );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.target ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.aboveBool ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.rightBool ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.leftBool ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.belowBool ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "aboveEvent",a?.aboveEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "rightBool", a?.rightBool?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "leftBool", a?.leftBool?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "belowBool", a?.belowBool?.Name ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetAngleToTarget2D );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.target ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.storeAngle ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.offsetX ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "offsetX Name", a?.offsetX?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.offsetY ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "offsetY Name", a?.offsetY?.Name ) );
                                    }
                                }


                                
                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FaceAngle );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.angleOffset ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FaceAngleV2 );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.angleOffset ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.worldSpace ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetParent );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.parent ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.resetLocalPosition ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.resetLocalRotation ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.Tk2dPlayFrame );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.frame ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.Tk2dWatchAnimationEvents );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerEventToString( a?.animationCompleteEvent ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerEventToString( a?.animationTriggerEvent ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.iTweenMoveTo );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delay ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "easeType ", a?.easeType ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "finishEvent ", a?.finishEvent ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "finishEvent ", a?.finishEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.id ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.speed ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "startEvent ", a?.startEvent ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "startEvent name", a?.startEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.stopOnExit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "startEvent axis", a?.axis ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.lookAhead ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.lookAtObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.lookAtVector ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.lookTime ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.loopDontFinish ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "loopType ", a?.loopType ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.moveToPath ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.orientToPath ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.realTime ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.reverse ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.transformPosition ) );
                                        if( a?.transforms != null )
                                            foreach( var v in a?.transforms )
                                                file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "transforms ", v ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.vectorPosition ) );
                                        if(a?.vectors != null)
                                        foreach(var v in a?.vectors)
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "vectors ", v ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.iTweenScaleTo );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.delay ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "easeType ", a?.easeType ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "finishEvent ", a?.finishEvent ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "finishEvent ", a?.finishEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.id ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.loopDontFinish ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "loopType ", a?.loopType ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.speed ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "startEvent ", a?.startEvent ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "startEvent ", a?.startEvent?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.stopOnExit ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.transformScale ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.vectorScale ) );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.GetRotation );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.gameObject?.GameObject ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value", a?.gameObject?.GameObject?.Value ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject value name", a?.gameObject?.GameObject?.Value?.name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "GameObject name", a?.gameObject?.GameObject?.Name ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.quaternion ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "space", a?.space ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.vector ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.xAngle ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.yAngle ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( a?.zAngle ) );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- " + a.GetType().Name + PlaymakerTypeToString( "everyFrame", a?.everyFrame ) );
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
        }//end print playmaker fsm

        static string PlaymakerEventToString<T>( T var ) where T : HutongGames.PlayMaker.FsmEvent
        {
            return "(" + var?.GetType()?.Name + ", " + var?.Name + ") = " + var + "      :: IsGlobal? = " + var?.IsGlobal;
        }

        static string PlaymakerTypeToString<T>(T var) where T : HutongGames.PlayMaker.NamedVariable
        {
            return "("+var?.GetType()?.Name +", "+var?.Name+") = "+var + "      :: IsNone? = "+var?.IsNone;
        }
        static string PlaymakerTypeToString<T>( string label, T var )
        {
            return "(" + var?.GetType()?.Name + ", " + label + ") = " + var;
        }
    }
}
