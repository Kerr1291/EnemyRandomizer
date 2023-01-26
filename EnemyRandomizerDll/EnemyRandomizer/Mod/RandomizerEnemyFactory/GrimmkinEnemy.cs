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
        public override void LinkDataObjects(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            Dev.Where();
            base.LinkDataObjects(enemy, knownEnemyTypes, prefabObject);
            Prefab = prefabObject.LocateMyFSM("Spawn Control").Fsm.GetFsmGameObject("Grimmkin Obj").Value;
        }
    }
}