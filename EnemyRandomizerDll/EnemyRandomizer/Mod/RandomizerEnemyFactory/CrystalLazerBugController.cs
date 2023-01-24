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
    public class CrystalLazerBugController : DefaultEnemy
    {
        public override void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            base.Setup(enemy, knownEnemyTypes, prefabObject);

            var hm = EnemyObject.GetComponent<HealthManager>();
            hm.IsInvincible = false;
        }
    }
}