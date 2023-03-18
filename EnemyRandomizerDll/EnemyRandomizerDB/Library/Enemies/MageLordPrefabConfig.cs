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
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
namespace EnemyRandomizerMod
{
    public class MageLordControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }
    }
    public class MageLordSpawner : DefaultSpawner<MageLordControl> { }
    public class MageLordPrefabConfig : DefaultPrefabConfig<MageLordControl> { }
}

