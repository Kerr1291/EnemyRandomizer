using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    //public class GGZoteCorpseFixer : MonoBehaviour
    //{
    //    IEnumerator Start()
    //    {
    //        yield return new WaitUntil(() => gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid") != null);
    //        Dev.Log("trying to fix corpse white flash");
    //        var whiteScreenEffect = gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid").gameObject;

    //        if (whiteScreenEffect == null)
    //        {
    //            Dev.LogError("Failed to find white screen effect");
    //            yield break;
    //        }

    //        var corpseFSM = gameObject.LocateMyFSM("Control");
    //        var fsm = whiteScreenEffect.LocateMyFSM("FSM");

    //        while (fsm == null)

    //        {
    //            fsm = whiteScreenEffect.LocateMyFSM("FSM");
    //            yield return null;
    //        }

    //        while (fsm.GetState("Init") == null)
    //            yield return null;

    //        while (fsm.GetState("Down") == null)
    //            yield return null;


    //        if (fsm.ActiveStateName == "Init")
    //            fsm.SendEvent("UP");

    //        while (fsm.ActiveStateName != "Upped")
    //            yield return null;

    //        HeroController.instance.RegainControl();
    //        HeroController.instance.StartAnimationControl();

    //        if (fsm.ActiveStateName == "Upped")
    //            fsm.SendEvent("DOWN");


    //        while (corpseFSM.ActiveStateName != "Notify")
    //            yield return null;


    //        while (corpseFSM.ActiveStateName == "Notify")
    //        {
    //            corpseFSM.SendEvent("CORPSE END");
    //            yield return null;
    //        }

    //        Destroy(this);
    //        yield break;
    //    }
    //}

}
