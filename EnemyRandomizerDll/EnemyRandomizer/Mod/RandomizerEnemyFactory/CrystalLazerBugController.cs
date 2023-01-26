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
        public override void SetupPrefab()
        {
            Dev.Where();
            base.SetupPrefab();
            var hm = Prefab.GetComponent<HealthManager>();
            hm.IsInvincible = false;
        }
    }
}