using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{
    public class FSMAreaControlEnemy : DefaultSpawnedEnemyControl
    {
        //most common fsm name for enemy controls
        public override string FSMName => "Control";
        public override bool showWhenHeroIsInAggroRange => true;
    }

    public class FSMBossAreaControl : FSMAreaControlEnemy
    {
        public override bool useCustomPositonOnShow => true;
    }
}

//fsm.AddTransition(hiddenStateName, "SHOW", "Arrive");
//preHideState.ChangeTransition("FINISHED", hiddenStateName);



//protected virtual void InsertHiddenState(PlayMakerFSM fsm, 
//    string preHideStateName, string preHideTransitionEventName, 
//    string postHideStateName)
//{
//    if (FSMsUsingHiddenStates == null)
//        FSMsUsingHiddenStates = new List<PlayMakerFSM>();

//    try
//    {
//        //var preHideState = fsm.GetState("Music?");
//        var preHideState = fsm.GetState(preHideStateName);
//        var hidden = fsm.AddState(FSMHiddenStateName);

//        fsm.AddVariable<FsmBool>("IsAggro");
//        var isAggro = fsm.FsmVariables.GetFsmBool("IsAggro");

//        //change the teleport she does to keep her hidden until a player is nearby
//        //preHideState.ChangeTransition("FINISHED", hiddenStateName);
//        preHideState.ChangeTransition(preHideTransitionEventName, FSMHiddenStateName);
//        //fsm.AddTransition(hiddenStateName, "SHOW", "Arrive");
//        fsm.AddTransition(FSMHiddenStateName, "SHOW", postHideStateName);
//        isAggro.Value = false;
//        hidden.AddAction(new BoolTest() { boolVariable = isAggro, isTrue = new FsmEvent("SHOW"), everyFrame = true });

//        FSMsUsingHiddenStates.Add(fsm);
//    }
//    catch (Exception e) { Dev.Log($"Error in adding an aggro range state to FSM:{fsm.name} PRE STATE:{preHideStateName} EVENT:{preHideTransitionEventName} POST-STATE:{postHideStateName}"); }
//}






//        protected virtual void SetupCustomDebugArea()
//        {
//#if DEBUG
//            var size = new Vector2(xR.Size, yR.Size);
//            var center = new Vector2(transform.position.x, transform.position.y);
//            var herop = new Vector2(HeroX, HeroY);
//            var dist = herop - center;
//            debugColliders.customLineCollections.Add(Color.yellow,
//                DebugColliders.GetPointsFromCollider(Vector2.one,center,dist.magnitude).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());
//            debugColliders.customLineCollections.Add(Color.magenta, new List<Vector3>() { 
//            herop, center, herop
//            });
//            debugColliders.customLineCollections.Add(Color.cyan, debugColliders.GetPointsFromCollider(bounds, false).Select(x => new Vector3(x.x, x.y, debugColliders.zDepth)).ToList());
//#endif
//        }