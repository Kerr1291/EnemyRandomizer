using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{

    /////////////////////////////////////////////////////////////////////////////
    /////
    public class TinySpiderControl : DefaultSpawnedEnemyControl { }

    public class TinySpiderSpawner : DefaultSpawner<TinySpiderControl> { }

    public class TinySpiderPrefabConfig : DefaultPrefabConfig<TinySpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ClimberControl : DefaultSpawnedEnemyControl { }

    public class ClimberSpawner : DefaultSpawner<ClimberControl> { }

    public class ClimberPrefabConfig : DefaultPrefabConfig<ClimberControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystallisedLazerBugControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //TODO: some logic to determine if it's safe to leave the enemy as invincible
            thisMetadata.EnemyHealthManager.IsInvincible = false;
        }
    }

    public class CrystallisedLazerBugSpawner : DefaultSpawner<CrystallisedLazerBugControl>
    {
    }

    public class CrystallisedLazerBugPrefabConfig : DefaultPrefabConfig<CrystallisedLazerBugControl>
    {
    }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
