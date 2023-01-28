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
    public class ZotePrince : DefaultEnemy
    {
        public override void SetupPrefab()
        {
            Dev.Where();
            base.SetupPrefab();
            var fsm = Prefab.LocateMyFSM("Control");

            //remove the transitions related to chain spawning zotes for the event
            fsm.RemoveTransition("Dormant", "ZOTE APPEAR");
            fsm.RemoveTransition("Dormant", "GG BOSS");
            fsm.RemoveTransition("GG Pause", "FINISHED");

            //change the start transition to just begin the spawn antics
            fsm.ChangeTransition("Level 1", "FINISHED", "Enter 1");
            fsm.ChangeTransition("Level 2", "FINISHED", "Enter 1");
            fsm.ChangeTransition("Level 3", "FINISHED", "Enter 1");
            fsm.ChangeTransition("4+", "FINISHED", "Enter 1");

            //remove the states that were also part of that
            fsm.Fsm.RemoveState("Dormant");
            fsm.Fsm.RemoveState("GG Pause");
        }
    }
}