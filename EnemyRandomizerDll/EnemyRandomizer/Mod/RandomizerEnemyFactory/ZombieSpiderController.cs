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
    public class ZombieSpiderController : DefaultEnemy
    {
        public override void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            EnemyObject = prefabObject;

            var fsm = prefabObject.LocateMyFSM("Chase");

            //change the start transition to just begin the spawn antics
            fsm.ChangeTransition("Check Battle", "BATTLE", "Wait 2");

            fsm.RemoveTransition("Battle Inert", "BATTLE START");
            fsm.Fsm.RemoveState("Battle Inert");

            //base.Setup(enemy, knownEnemyTypes, EnemyObject);
        }
    }
}