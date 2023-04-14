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
    public class AbyssCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 0.5f, alsoStickCorpse: false, flipped:true);
        }
    }

    public class AbyssCrawlerSpawner : DefaultSpawner<AbyssCrawlerControl> { }

    public class AbyssCrawlerPrefabConfig : DefaultPrefabConfig<AbyssCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class TinySpiderControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 0.5f, alsoStickCorpse: false);
        }
    }


    public class TinySpiderSpawner : DefaultSpawner<TinySpiderControl> { }

    public class TinySpiderPrefabConfig : DefaultPrefabConfig<TinySpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ClimberControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 0.5f, alsoStickCorpse: false);
        }
    }

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

        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale:0.5f, alsoStickCorpse: false);
        }
    }

    public class CrystallisedLazerBugSpawner : DefaultSpawner<CrystallisedLazerBugControl> { }

    public class CrystallisedLazerBugPrefabConfig : DefaultPrefabConfig<CrystallisedLazerBugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MinesCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 0.5f, alsoStickCorpse: false);
        }
    }

    public class MinesCrawlerSpawner : DefaultSpawner<MinesCrawlerControl> { }

    public class MinesCrawlerPrefabConfig : DefaultPrefabConfig<MinesCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 0.5f, alsoStickCorpse: false);
        }
    }

    public class CrawlerSpawner : DefaultSpawner<CrawlerControl> { }

    public class CrawlerPrefabConfig : DefaultPrefabConfig<CrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 0.5f, alsoStickCorpse: false);
        }
    }

    public class CrystalCrawlerSpawner : DefaultSpawner<CrystalCrawlerControl> { }

    public class CrystalCrawlerPrefabConfig : DefaultPrefabConfig<CrystalCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpiderMiniControl : DefaultSpawnedEnemyControl { }

    public class SpiderMiniSpawner : DefaultSpawner<SpiderMiniControl> { }

    public class SpiderMiniPrefabConfig : DefaultPrefabConfig<SpiderMiniControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
