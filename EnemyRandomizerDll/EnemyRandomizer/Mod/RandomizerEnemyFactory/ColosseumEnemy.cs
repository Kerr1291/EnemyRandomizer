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

        public override void LinkDataObjects(EnemyData enemyDataType, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            Dev.Where();
            base.LinkDataObjects(enemyDataType, knownEnemyTypes, prefabObject);

            Dev.Log($"Loading nested enemy {enemyDataType.name}");

            if(string.IsNullOrEmpty(enemyDataType.name))
            {
                Prefab = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy").Value;
            }
            else if(DefaultLoading.Contains(enemyDataType.name))
            {
                //leave it default?
            }
            else if(CorpseFSMLoading.Contains(enemyDataType.name))
            {
                Prefab = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Corpse to Instantiate").Value;
            }
            else
            {
                Prefab = prefabObject.LocateMyFSM("Spawn").Fsm.GetFsmGameObject("Enemy Type").Value;
            }
        }
    }
}