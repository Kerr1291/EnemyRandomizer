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

namespace EnemyRandomizerMod
{

    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BuzzerControl : DefaultSpawnedEnemyControl { }

    public class BuzzerSpawner : DefaultSpawner<BuzzerControl> { }

    public class BuzzerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MosquitoControl : DefaultSpawnedEnemyControl { }

    public class MosquitoSpawner : DefaultSpawner<MosquitoControl> { }

    public class MosquitoPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class BurstingBouncerControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                corpse.GetOrAddComponent<ExplodeOnCorpseRemoved>();
            }
        }
    }

    public class BurstingBouncerSpawner : DefaultSpawner<BurstingBouncerControl> { }

    public class BurstingBouncerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    ///// TODO: add explosion to corpse
    public class AngryBuzzerControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var corpse = thisMetadata.Corpse;
            if (corpse != null)
            {
                corpse.GetOrAddComponent<ExplodeOnCorpseRemoved>();
            }
        }
    }

    public class AngryBuzzerSpawner : DefaultSpawner<AngryBuzzerControl> { }

    public class AngryBuzzerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavyFlyerControl : DefaultSpawnedEnemyControl { }

    public class MantisHeavyFlyerSpawner : DefaultSpawner<MantisHeavyFlyerControl> { }

    public class MantisHeavyFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlyControl : DefaultSpawnedEnemyControl { }

    public class FlySpawner : DefaultSpawner<FlyControl> { }

    public class FlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlobbleControl : DefaultSpawnedEnemyControl { }

    public class BlobbleSpawner : DefaultSpawner<BlobbleControl> { }

    public class BlobblePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ShadeSiblingControl : DefaultSpawnedEnemyControl { }

    public class ShadeSiblingSpawner : DefaultSpawner<ShadeSiblingControl> { }

    public class ShadeSiblingPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanTopControl : DefaultSpawnedEnemyControl { }

    public class FlukemanTopSpawner : DefaultSpawner<FlukemanTopControl> { }

    public class FlukemanTopPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpitterControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if(this.Geo <= 0)
            {
                SetGeoRandomBetween(1, 10);
            }
        }
    }

    public class SpitterSpawner : DefaultSpawner<SpitterControl> { }

    public class SpitterPrefabConfig : DefaultPrefabConfig { }


    public class SpitterRControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (this.Geo <= 0)
            {
                SetGeoRandomBetween(1, 10);
            }
        }
    }

    public class SpitterRSpawner : DefaultSpawner<SpitterRControl> { }

    public class SpitterRPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungoonBabyControl : DefaultSpawnedEnemyControl
    {
    }

    public class FungoonBabySpawner : DefaultSpawner<FungoonBabyControl> { }

    public class FungoonBabyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AcidFlyerControl : DefaultSpawnedEnemyControl { }

    public class AcidFlyerSpawner : DefaultSpawner<AcidFlyerControl> { }

    public class AcidFlyerPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            try
            {
                var db = EnemyRandomizerDatabase.GetDatabase();
                if (db != null && !db.otherNames.Contains(EnemyRandomizerDatabase.BlockHitEffectName))
                {
                    var tink = p.prefab.GetComponentInChildren<TinkEffect>(true);

                    var beClone = GameObject.Instantiate(tink.blockEffect);
                    beClone.SetActive(false);
                    GameObject.DontDestroyOnLoad(beClone);
                    PrefabObject p2 = new PrefabObject();
                    SceneObject sp2 = new SceneObject();
                    sp2.components = new List<string>();
                    sp2.Scene = p.source.Scene;
                    p2.prefabName = EnemyRandomizerDatabase.BlockHitEffectName;
                    beClone.name = p2.prefabName;
                    sp2.path = beClone.name;
                    p2.prefab = beClone;
                    p2.source = sp2;
                    sp2.LoadedObject = p2;
                    sp2.Scene.sceneObjects.Add(sp2);
                    db.otherPrefabs.Add(p2);
                    db.Others[p2.prefabName] = p2;
                    db.Objects[p2.prefabName] = p2;
                    sp2.Loaded = true;
                }
            }
            catch(Exception e)
            {
                Dev.LogError($"Error creating block hit effect:{e.Message} Stacktrace:{e.StackTrace}");
            }
        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBalloonControl : DefaultSpawnedEnemyControl { }

    public class MageBalloonSpawner : DefaultSpawner<MageBalloonControl> { }

    public class MageBalloonPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsFlyingSentryControl : DefaultSpawnedEnemyControl { }

    public class RuinsFlyingSentrySpawner : DefaultSpawner<RuinsFlyingSentryControl> { }

    public class RuinsFlyingSentryPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsFlyingSentryJavelinControl : DefaultSpawnedEnemyControl { }

    public class RuinsFlyingSentryJavelinSpawner : DefaultSpawner<RuinsFlyingSentryJavelinControl> { }

    public class RuinsFlyingSentryJavelinPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FatFlyControl : DefaultSpawnedEnemyControl { }

    public class FatFlySpawner : DefaultSpawner<FatFlyControl> { }

    public class FatFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LazyFlyerEnemyControl : DefaultSpawnedEnemyControl { }

    public class LazyFlyerEnemySpawner : DefaultSpawner<LazyFlyerEnemyControl> { }

    public class LazyFlyerEnemyPrefabConfig : DefaultPrefabConfig { }

    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SuperSpitterControl : DefaultSpawnedEnemyControl { }

    public class SuperSpitterSpawner : DefaultSpawner<SuperSpitterControl> { }

    public class SuperSpitterPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Armoured_MosquitoControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Armoured_MosquitoSpawner : DefaultSpawner<Colosseum_Armoured_MosquitoControl> { }

    public class Colosseum_Armoured_MosquitoPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungusFlyerControl : DefaultSpawnedEnemyControl { }

    public class FungusFlyerSpawner : DefaultSpawner<FungusFlyerControl> { }

    public class FungusFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Flying_SentryControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Flying_SentrySpawner : DefaultSpawner<Colosseum_Flying_SentryControl> { }

    public class Colosseum_Flying_SentryPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishBabyControl : DefaultSpawnedEnemyControl { }

    public class JellyfishBabySpawner : DefaultSpawner<JellyfishBabyControl> { }

    public class JellyfishBabyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossFlyerControl : DefaultSpawnedEnemyControl { }

    public class MossFlyerSpawner : DefaultSpawner<MossFlyerControl> { }

    public class MossFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalFlyerControl : DefaultSpawnedEnemyControl { }

    public class CrystalFlyerSpawner : DefaultSpawner<CrystalFlyerControl> { }

    public class CrystalFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpiderFlyerControl : DefaultSpawnedEnemyControl { }

    public class SpiderFlyerSpawner : DefaultSpawner<SpiderFlyerControl> { }

    public class SpiderFlyerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlowFlyControl : DefaultSpawnedEnemyControl { }

    public class BlowFlySpawner : DefaultSpawner<BlowFlyControl> { }

    public class BlowFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BeeHatchlingAmbientControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            Geo = 2;
        }
    }

    public class BeeHatchlingAmbientSpawner : DefaultSpawner<BeeHatchlingAmbientControl> { }

    public class BeeHatchlingAmbientPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ParasiteBalloonControl : DefaultSpawnedEnemyControl { }

    public class ParasiteBalloonSpawner : DefaultSpawner<ParasiteBalloonControl> { }

    public class ParasiteBalloonPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class InflaterControl : DefaultSpawnedEnemyControl { }

    public class InflaterSpawner : DefaultSpawner<InflaterControl> { }

    public class InflaterPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukeFlyControl : DefaultSpawnedEnemyControl { }

    public class FlukeFlySpawner : DefaultSpawner<FlukeFlyControl> { }

    public class FlukeFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BeeStingerControl : DefaultSpawnedEnemyControl { }

    public class BeeStingerSpawner : DefaultSpawner<BeeStingerControl> { }

    public class BeeStingerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BigBeeControl : DefaultSpawnedEnemyControl { }

    public class BigBeeSpawner : DefaultSpawner<BigBeeControl> { }

    public class BigBeePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////











    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Jellyfish";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var spawnerFunc = GetRandomAttackSpawnerFunc();

            var detach = control.GetState("Detach");
            detach.AddCustomAction(() => {
                var go = spawnerFunc?.Invoke();
                go.transform.parent = null;
                go.transform.position = transform.position;
                go.SafeSetActive(true);
            });
        }
    }

    public class JellyfishSpawner : DefaultSpawner<JellyfishControl> { }

    public class JellyfishPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LilJellyfishControl : DefaultSpawnedEnemyControl { }

    public class LilJellyfishSpawner : DefaultSpawner<LilJellyfishControl> { }

    public class LilJellyfishPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////



}
