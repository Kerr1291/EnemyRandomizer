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
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- GetPosition (vector) = " + a?.vector );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SetPosition );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (GameObject) = " + a?.gameObject?.GameObject );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (OwnerOption) = " + a?.gameObject?.OwnerOption );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetPosition (vector) = " + a?.vector );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FloatCompare );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (float1) = " + a?.float1 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (float2) = " + a?.float2 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (greaterThan) = " + a?.greaterThan?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (lessThan) = " + a?.lessThan?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatCompare (equal) = " + a?.equal?.Name );
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
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SendEventByName (sendEvent) = " + a?.sendEvent );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.SpawnObjectFromGlobalPool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (gameObject?.Name) = " + a?.gameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (position) = " + a?.position );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (rotation) = " + a?.rotation );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (spawnPoint) = " + a?.spawnPoint );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SpawnObjectFromGlobalPool (storeObject) = " + a?.storeObject );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.FlingObjectsFromGlobalPool );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (gameObject?.Name) = " + a?.gameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (position) = " + a?.position );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (FSM) = " + a?.FSM );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (FSMEvent) = " + a?.FSMEvent );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FlingObjectsFromGlobalPool (spawnPoint) = " + a?.spawnPoint );
                                    }
                                }

                                {
                                    var a = ( x as HutongGames.PlayMaker.Actions.CreateObject );
                                    if( a != null )
                                    {
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (gameObject?.Name) = " + a?.gameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (position) = " + a?.position );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (position) = " + a?.rotation );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (networkGroup) = " + a?.networkGroup );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (networkInstantiate) = " + a?.networkInstantiate );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (spawnPoint) = " + a?.spawnPoint );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- CreateObject (storeObject) = " + a?.storeObject );
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
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatOperator (Name) = " + a?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatOperator (min) = " + a?.float1 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- FloatOperator (max) = " + a?.float2 );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (operation) = " + a?.operation );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- BoolTest (storeResult) = " + a?.storeResult );
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
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (gameObject?.GameObject?.Name) = " + a?.gameObject?.GameObject?.Name );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (vector?.Value) = " + a?.vector?.Value );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (x) = " + a?.x );
                                        file.WriteLine( componentHeader + @" \----PFSM ---- SetVelocity2d (y) = " + a?.y );
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
