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
    public class GrimmkinEnemy : DefaultEnemy
    {
        public override void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            EnemyObject = prefabObject;

            if (enemy.gameObjectPath.Contains("Colosseum"))
                return;

            if (enemy.gameObjectPath == "Flamebearer Spawn")
            {
                string gkName = "";
                if (enemy.sceneName == "Mines_10")
                    gkName = "Flamebearer Small";
                else if (enemy.sceneName == "RestingGrounds_06")
                    gkName = "Flamebearer Med";
                else
                    gkName = "Flamebearer Large";
                
                enemy = knownEnemyTypes.FirstOrDefault(x => x.name == gkName);
                EnemyObject = prefabObject.LocateMyFSM("Spawn Control").Fsm.GetFsmGameObject("Grimmkin Obj").Value;
                Dev.Log("Loaded Grimmkin: " + EnemyObject.name);
            }

            base.Setup(enemy, knownEnemyTypes, EnemyObject);
        }
    }
}