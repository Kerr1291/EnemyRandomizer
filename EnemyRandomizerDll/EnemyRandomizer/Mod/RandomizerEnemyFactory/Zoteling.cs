using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;

namespace EnemyRandomizerMod
{
    public class Zoteling : DefaultEnemy
    {
        public override void SetupPrefab()
        {
            Dev.Where();
            base.SetupPrefab();
            var fsm = Prefab.LocateMyFSM("Control");

            //remove the transitions related to chain spawning zotes for the event
            fsm.RemoveTransition("Dormant", "SPAWN");
            //fsm.RemoveTransition("Die", "FINISHED");
            fsm.RemoveTransition("Respawn Pause", "SPAWN");
            fsm.RemoveTransition("Ball", "FINISHED");

            //change the start transition to just begin the spawn antics
            fsm.ChangeTransition("Init", "FINISHED", "Choice");
            fsm.ChangeTransition("Reset", "FINISHED", "Dormant");

            //remove the states that were also part of that
            //fsm.Fsm.RemoveState("Dormant");
            //fsm.Fsm.RemoveState("Reset");
            fsm.Fsm.RemoveState("Respawn Pause");
            fsm.Fsm.RemoveState("Ball");

        }
    }
}