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
    public class ColosseumEnemy : DefaultEnemy
    {
        public List<string> DefaultLoading = new List<string>()
        {
            "Mage",
            "Electric Mage",
            "Lobster",
            "Lancer"
        };

        public List<string> CorpseFSMLoading = new List<string>()
        {
            "Colosseum_Flying_Sentry",
            "Colosseum_Shield_Zombie",
            "Colosseum_Miner",
            "Lesser Mawlek",
            "Mantis Heavy"
        };

        public override void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            EnemyObject = prefabObject;

            //if (!enemy.gameObjectPath.Contains("Colosseum"))
            //    return;

            //not sure why this line is needed? leaving here for now to preserve original algorithm
            //EnemyData data = knownEnemyTypes.FirstOrDefault(ed => ed.gameObjectPath == enemy.gameObjectPath);

            Dev.Log($"Loading nested enemy {enemy.name}");

            if(string.IsNullOrEmpty(enemy.name))
            {
                EnemyObject = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy").Value;
            }
            else if(DefaultLoading.Contains(enemy.name))
            {
                //leave it default?
            }
            else if(CorpseFSMLoading.Contains(enemy.name))
            {
                EnemyObject = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;
            }
            else
            {
                EnemyObject = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;
            }

            base.Setup(enemy, knownEnemyTypes, EnemyObject);
        }
    }
}