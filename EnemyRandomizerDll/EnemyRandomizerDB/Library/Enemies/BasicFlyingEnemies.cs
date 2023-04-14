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

    public class BuzzerPrefabConfig : DefaultPrefabConfig<BuzzerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MosquitoControl : DefaultSpawnedEnemyControl { }

    public class MosquitoSpawner : DefaultSpawner<MosquitoControl> { }

    public class MosquitoPrefabConfig : DefaultPrefabConfig<MosquitoControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BurstingBouncerControl : DefaultSpawnedEnemyControl { }

    public class BurstingBouncerSpawner : DefaultSpawner<BurstingBouncerControl> { }

    public class BurstingBouncerPrefabConfig : DefaultPrefabConfig<BurstingBouncerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AngryBuzzerControl : DefaultSpawnedEnemyControl { }

    public class AngryBuzzerSpawner : DefaultSpawner<AngryBuzzerControl> { }

    public class AngryBuzzerPrefabConfig : DefaultPrefabConfig<AngryBuzzerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavyFlyerControl : DefaultSpawnedEnemyControl { }

    public class MantisHeavyFlyerSpawner : DefaultSpawner<MantisHeavyFlyerControl> { }

    public class MantisHeavyFlyerPrefabConfig : DefaultPrefabConfig<MantisHeavyFlyerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlyControl : DefaultSpawnedEnemyControl { }

    public class FlySpawner : DefaultSpawner<FlyControl> { }

    public class FlyPrefabConfig : DefaultPrefabConfig<FlyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlobbleControl : DefaultSpawnedEnemyControl { }

    public class BlobbleSpawner : DefaultSpawner<BlobbleControl> { }

    public class BlobblePrefabConfig : DefaultPrefabConfig<BlobbleControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ShadeSiblingControl : DefaultSpawnedEnemyControl { }

    public class ShadeSiblingSpawner : DefaultSpawner<ShadeSiblingControl> { }

    public class ShadeSiblingPrefabConfig : DefaultPrefabConfig<ShadeSiblingControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanTopControl : DefaultSpawnedEnemyControl { }

    public class FlukemanTopSpawner : DefaultSpawner<FlukemanTopControl> { }

    public class FlukemanTopPrefabConfig : DefaultPrefabConfig<FlukemanTopControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpitterControl : DefaultSpawnedEnemyControl { }

    public class SpitterSpawner : DefaultSpawner<SpitterControl> { }

    public class SpitterPrefabConfig : DefaultPrefabConfig<SpitterControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AcidFlyerControl : DefaultSpawnedEnemyControl { }

    public class AcidFlyerSpawner : DefaultSpawner<AcidFlyerControl> { }

    public class AcidFlyerPrefabConfig : DefaultPrefabConfig<AcidFlyerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBalloonControl : DefaultSpawnedEnemyControl { }

    public class MageBalloonSpawner : DefaultSpawner<MageBalloonControl> { }

    public class MageBalloonPrefabConfig : DefaultPrefabConfig<MageBalloonControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsFlyingSentryControl : DefaultSpawnedEnemyControl { }

    public class RuinsFlyingSentrySpawner : DefaultSpawner<RuinsFlyingSentryControl> { }

    public class RuinsFlyingSentryPrefabConfig : DefaultPrefabConfig<RuinsFlyingSentryControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsFlyingSentryJavelinControl : DefaultSpawnedEnemyControl { }

    public class RuinsFlyingSentryJavelinSpawner : DefaultSpawner<RuinsFlyingSentryJavelinControl> { }

    public class RuinsFlyingSentryJavelinPrefabConfig : DefaultPrefabConfig<RuinsFlyingSentryJavelinControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FatFlyControl : DefaultSpawnedEnemyControl { }

    public class FatFlySpawner : DefaultSpawner<FatFlyControl> { }

    public class FatFlyPrefabConfig : DefaultPrefabConfig<FatFlyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LazyFlyerEnemyControl : DefaultSpawnedEnemyControl { }

    public class LazyFlyerEnemySpawner : DefaultSpawner<LazyFlyerEnemyControl> { }

    public class LazyFlyerEnemyPrefabConfig : DefaultPrefabConfig<LazyFlyerEnemyControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SuperSpitterControl : DefaultSpawnedEnemyControl { }

    public class SuperSpitterSpawner : DefaultSpawner<SuperSpitterControl> { }

    public class SuperSpitterPrefabConfig : DefaultPrefabConfig<SuperSpitterControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Armoured_MosquitoControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Armoured_MosquitoSpawner : DefaultSpawner<Colosseum_Armoured_MosquitoControl> { }

    public class Colosseum_Armoured_MosquitoPrefabConfig : DefaultPrefabConfig<Colosseum_Armoured_MosquitoControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungusFlyerControl : DefaultSpawnedEnemyControl { }

    public class FungusFlyerSpawner : DefaultSpawner<FungusFlyerControl> { }

    public class FungusFlyerPrefabConfig : DefaultPrefabConfig<FungusFlyerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Flying_SentryControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Flying_SentrySpawner : DefaultSpawner<Colosseum_Flying_SentryControl> { }

    public class Colosseum_Flying_SentryPrefabConfig : DefaultPrefabConfig<Colosseum_Flying_SentryControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishBabyControl : DefaultSpawnedEnemyControl { }

    public class JellyfishBabySpawner : DefaultSpawner<JellyfishBabyControl> { }

    public class JellyfishBabyPrefabConfig : DefaultPrefabConfig<JellyfishBabyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossFlyerControl : DefaultSpawnedEnemyControl { }

    public class MossFlyerSpawner : DefaultSpawner<MossFlyerControl> { }

    public class MossFlyerPrefabConfig : DefaultPrefabConfig<MossFlyerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalFlyerControl : DefaultSpawnedEnemyControl { }

    public class CrystalFlyerSpawner : DefaultSpawner<CrystalFlyerControl> { }

    public class CrystalFlyerPrefabConfig : DefaultPrefabConfig<CrystalFlyerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpiderFlyerControl : DefaultSpawnedEnemyControl { }

    public class SpiderFlyerSpawner : DefaultSpawner<SpiderFlyerControl> { }

    public class SpiderFlyerPrefabConfig : DefaultPrefabConfig<SpiderFlyerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlowFlyControl : DefaultSpawnedEnemyControl { }

    public class BlowFlySpawner : DefaultSpawner<BlowFlyControl> { }

    public class BlowFlyPrefabConfig : DefaultPrefabConfig<BlowFlyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BeeHatchlingAmbientControl : DefaultSpawnedEnemyControl { }

    public class BeeHatchlingAmbientSpawner : DefaultSpawner<BeeHatchlingAmbientControl> { }

    public class BeeHatchlingAmbientPrefabConfig : DefaultPrefabConfig<BeeHatchlingAmbientControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ParasiteBalloonControl : DefaultSpawnedEnemyControl { }

    public class ParasiteBalloonSpawner : DefaultSpawner<ParasiteBalloonControl> { }

    public class ParasiteBalloonPrefabConfig : DefaultPrefabConfig<ParasiteBalloonControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class InflaterControl : DefaultSpawnedEnemyControl { }

    public class InflaterSpawner : DefaultSpawner<InflaterControl> { }

    public class InflaterPrefabConfig : DefaultPrefabConfig<InflaterControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukeFlyControl : DefaultSpawnedEnemyControl { }

    public class FlukeFlySpawner : DefaultSpawner<FlukeFlyControl> { }

    public class FlukeFlyPrefabConfig : DefaultPrefabConfig<FlukeFlyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BeeStingerControl : DefaultSpawnedEnemyControl { }

    public class BeeStingerSpawner : DefaultSpawner<BeeStingerControl> { }

    public class BeeStingerPrefabConfig : DefaultPrefabConfig<BeeStingerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BigBeeControl : DefaultSpawnedEnemyControl { }

    public class BigBeeSpawner : DefaultSpawner<BigBeeControl> { }

    public class BigBeePrefabConfig : DefaultPrefabConfig<BigBeeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////











    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishControl : DefaultSpawnedEnemyControl { }

    public class JellyfishSpawner : DefaultSpawner<JellyfishControl> { }

    public class JellyfishPrefabConfig : DefaultPrefabConfig<JellyfishControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LilJellyfishControl : DefaultSpawnedEnemyControl { }

    public class LilJellyfishSpawner : DefaultSpawner<LilJellyfishControl> { }

    public class LilJellyfishPrefabConfig : DefaultPrefabConfig<LilJellyfishControl> { }
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
