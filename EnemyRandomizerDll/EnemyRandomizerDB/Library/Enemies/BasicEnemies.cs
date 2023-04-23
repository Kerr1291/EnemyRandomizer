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
    public class TEMPLATE_Control : DefaultSpawnedEnemyControl { }

    public class TEMPLATE_Spawner : DefaultSpawner<TEMPLATE_Control> { }

    public class TEMPLATE_PrefabConfig : DefaultPrefabConfig<TEMPLATE_Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HopperControl : DefaultSpawnedEnemyControl { }

    public class HopperSpawner : DefaultSpawner<HopperControl> { }

    public class HopperPrefabConfig : DefaultPrefabConfig<HopperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantHopperControl : DefaultSpawnedEnemyControl { }

    public class GiantHopperSpawner : DefaultSpawner<GiantHopperControl> { }

    public class GiantHopperPrefabConfig : DefaultPrefabConfig<GiantHopperControl> { }
                                                                             /////
    //////////////////////////////////////////////////////////////////////////////
    
    
    


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpittingZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class SpittingZombieSpawner : DefaultSpawner<SpittingZombieControl> { }

    public class SpittingZombiePrefabConfig : DefaultPrefabConfig<SpittingZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BurstingZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class BurstingZombieSpawner : DefaultSpawner<BurstingZombieControl> { }

    public class BurstingZombiePrefabConfig : DefaultPrefabConfig<BurstingZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavyControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MantisHeavySpawner : DefaultSpawner<MantisHeavyControl> { }

    public class MantisHeavyPrefabConfig : DefaultPrefabConfig<MantisHeavyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LesserMawlekControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class LesserMawlekSpawner : DefaultSpawner<LesserMawlekControl> { }

    public class LesserMawlekPrefabConfig : DefaultPrefabConfig<LesserMawlekControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RollerControl : DefaultSpawnedEnemyControl { }

    public class RollerSpawner : DefaultSpawner<RollerControl> { }

    public class RollerPrefabConfig : DefaultPrefabConfig<RollerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mossman_RunnerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class Mossman_RunnerSpawner : DefaultSpawner<Mossman_RunnerControl> { }

    public class Mossman_RunnerPrefabConfig : DefaultPrefabConfig<Mossman_RunnerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BabyCentipedeControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class BabyCentipedeSpawner : DefaultSpawner<BabyCentipedeControl> { }

    public class BabyCentipedePrefabConfig : DefaultPrefabConfig<BabyCentipedeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanBotControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class FlukemanBotSpawner : DefaultSpawner<FlukemanBotControl> { }

    public class FlukemanBotPrefabConfig : DefaultPrefabConfig<FlukemanBotControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBlobControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MageBlobSpawner : DefaultSpawner<MageBlobControl> { }

    public class MageBlobPrefabConfig : DefaultPrefabConfig<MageBlobControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieRunnerSpawner : DefaultSpawner<ZombieRunnerControl> { }

    public class ZombieRunnerPrefabConfig : DefaultPrefabConfig<ZombieRunnerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieHornheadSpawner : DefaultSpawner<ZombieHornheadControl> { }

    public class ZombieHornheadPrefabConfig : DefaultPrefabConfig<ZombieHornheadControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBargerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieBargerSpawner : DefaultSpawner<ZombieBargerControl> { }

    public class ZombieBargerPrefabConfig : DefaultPrefabConfig<ZombieBargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PrayerSlugControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class PrayerSlugSpawner : DefaultSpawner<PrayerSlugControl> { }

    public class PrayerSlugPrefabConfig : DefaultPrefabConfig<PrayerSlugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieShieldControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieShieldSpawner : DefaultSpawner<ZombieShieldControl> { }

    public class ZombieShieldPrefabConfig : DefaultPrefabConfig<ZombieShieldControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieLeaperControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieLeaperSpawner : DefaultSpawner<ZombieLeaperControl> { }

    public class ZombieLeaperPrefabConfig : DefaultPrefabConfig<ZombieLeaperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieGuardControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieGuardSpawner : DefaultSpawner<ZombieGuardControl> { }

    public class ZombieGuardPrefabConfig : DefaultPrefabConfig<ZombieGuardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMylaControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //this checks if the player has superdash and if not disables the game object
            //which is kinda silly.... so destroy it
            var deactivate = gameObject.GetComponent<DeactivateIfPlayerdataFalse>();
            GameObject.Destroy(deactivate);
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieMylaSpawner : DefaultSpawner<ZombieMylaControl> { }

    public class ZombieMylaPrefabConfig : DefaultPrefabConfig<ZombieMylaControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieFatControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class RoyalZombieFatSpawner : DefaultSpawner<RoyalZombieFatControl> { }

    public class RoyalZombieFatPrefabConfig : DefaultPrefabConfig<RoyalZombieFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class RoyalZombieSpawner : DefaultSpawner<RoyalZombieControl> { }

    public class RoyalZombiePrefabConfig : DefaultPrefabConfig<RoyalZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieCowardControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class RoyalZombieCowardSpawner : DefaultSpawner<RoyalZombieCowardControl> { }

    public class RoyalZombieCowardPrefabConfig : DefaultPrefabConfig<RoyalZombieCowardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GorgeousHuskControl : DefaultSpawnedEnemyControl
    {
        public bool doSurprise = false;
        public bool doBounceSurprise = false;
        public bool isSuperHusk = false;

        public GameObject supEffect;
        public GameObject superEffect;

        bool HasSurprise(out int surprise)
        {
            RNG rng = new RNG();
            rng.Reset();
            surprise = rng.Rand(0, 20);
            return surprise < 5;
        }

        bool HasBigSurprise(int previousSurprise)
        {
            return previousSurprise < 10;
        }

        bool IsSuperHusk()
        {
            RNG rng = new RNG();
            rng.Reset();
            int superHusk = rng.Rand(0, 40);
            return superHusk < 10;
        }

        protected virtual void CalculateGeo(bool hasSurprise, bool hasBigSurprise)
        {
            RNG rng = new RNG();
            rng.Reset();
            if (hasSurprise)
            {
                thisMetadata.GeoManager.Value = rng.Rand(500, 2000);
            }
            else if (hasBigSurprise)
            {
                thisMetadata.GeoManager.Value = rng.Rand(500, 2000);
            }
            else
            {
                thisMetadata.GeoManager.Value = rng.Rand(100, 1000);
            }
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            bool hasSurprise = HasSurprise(out int surpriseType);
            bool hasBigSurprise = HasBigSurprise(surpriseType);
            bool isSuperHusk = IsSuperHusk();
            CalculateGeo(hasSurprise, hasBigSurprise);

            doSurprise = hasSurprise;
            doBounceSurprise = hasBigSurprise;

            if (doSurprise || doBounceSurprise)
            {
                thisMetadata.EnemyHealthManager.hp = thisMetadata.EnemyHealthManager.hp * 2;
                thisMetadata.ApplySizeScale(thisMetadata.SizeScale + 0.2f);
                supEffect = EnemyRandomizerDatabase.GetDatabase().Spawn("Fire Particles", null);
                supEffect.transform.parent = transform;
                supEffect.transform.localPosition = Vector3.zero;
                var pe = supEffect.GetComponent<ParticleSystem>();
                pe.startSize = 5;
                pe.simulationSpace = ParticleSystemSimulationSpace.World;
                supEffect.SafeSetActive(true);
            }   

            if (isSuperHusk)
            {
                thisMetadata.EnemyHealthManager.hp = thisMetadata.EnemyHealthManager.hp * 4;
                thisMetadata.ApplySizeScale(thisMetadata.SizeScale + 0.4f);
                isSuperHusk = true;
                thisMetadata.GeoManager.Value = thisMetadata.GeoManager.Value * 4;
                superEffect = EnemyRandomizerDatabase.GetDatabase().Spawn("Particle System B", null);
                superEffect.transform.parent = transform;
                superEffect.transform.localPosition = Vector3.zero;
                superEffect.SafeSetActive(true);
            }

            onHit -= OnHit;
            onHit += OnHit;
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(doSurprise)
            {
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Gas Explosion Recycle L", null, true);
            }
            else if(doBounceSurprise)
            {
                EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Galien Mini Hammer", null, true);
            }
        }

        protected virtual void OnHit()
        {
            if (isSuperHusk)
            {
                EnemyRandomizerDatabase.GetDatabase().Spawn("Shot Markoth Nail", null).SafeSetActive(true);
            }
        }
    }

    public class GorgeousHuskSpawner : DefaultSpawner<GorgeousHuskControl> { }

    public class GorgeousHuskPrefabConfig : DefaultPrefabConfig<GorgeousHuskControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CeilingDropperControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            var roof = SpawnerExtensions.GetRoof(gameObject);
            if (roof.distance > 200f)
            {
                gameObject.StickToClosestSurface(200f, .5f, false, true);
            }
            else
            {
                gameObject.StickToRoof(.5f, true);
            }
        }
    }

    public class CeilingDropperSpawner : DefaultSpawner<CeilingDropperControl> { }

    public class CeilingDropperPrefabConfig : DefaultPrefabConfig<CeilingDropperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsSentryControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class RuinsSentrySpawner : DefaultSpawner<RuinsSentryControl> { }

    public class RuinsSentryPrefabConfig : DefaultPrefabConfig<RuinsSentryControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsSentryFatControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class RuinsSentryFatSpawner : DefaultSpawner<RuinsSentryFatControl> { }

    public class RuinsSentryFatPrefabConfig : DefaultPrefabConfig<RuinsSentryFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GreatShieldZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class GreatShieldZombieSpawner : DefaultSpawner<GreatShieldZombieControl> { }

    public class GreatShieldZombiePrefabConfig : DefaultPrefabConfig<GreatShieldZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossWalkerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            //TODO: test
            gameObject.StickToGround();
        }
    }

    public class MossWalkerSpawner : DefaultSpawner<MossWalkerControl> { }

    public class MossWalkerPrefabConfig : DefaultPrefabConfig<MossWalkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTrapControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(200f, -1.5f, false, false);
        }
    }

    public class PlantTrapSpawner : DefaultSpawner<PlantTrapControl> { }

    public class PlantTrapPrefabConfig : DefaultPrefabConfig<PlantTrapControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mossman_ShakerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class Mossman_ShakerSpawner : DefaultSpawner<Mossman_ShakerControl> { }

    public class Mossman_ShakerPrefabConfig : DefaultPrefabConfig<Mossman_ShakerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PigeonControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class PigeonSpawner : DefaultSpawner<PigeonControl> { }

    public class PigeonPrefabConfig : DefaultPrefabConfig<PigeonControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////












    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AcidWalkerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class AcidWalkerSpawner : DefaultSpawner<AcidWalkerControl> { }

    public class AcidWalkerPrefabConfig : DefaultPrefabConfig<AcidWalkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTurretControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(200f, -.5f, true, false);
        }
    }

    public class PlantTurretSpawner : DefaultSpawner<PlantTurretControl> { }

    public class PlantTurretPrefabConfig : DefaultPrefabConfig<PlantTurretControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTurretRightControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(200f, -.5f, true, true);
        }
    }

    public class PlantTurretRightSpawner : DefaultSpawner<PlantTurretRightControl> { }

    public class PlantTurretRightPrefabConfig : DefaultPrefabConfig<PlantTurretRightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossKnightControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MossKnightSpawner : DefaultSpawner<MossKnightControl> { }

    public class MossKnightPrefabConfig : DefaultPrefabConfig<MossKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrassHopperControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class GrassHopperSpawner : DefaultSpawner<GrassHopperControl> { }

    public class GrassHopperPrefabConfig : DefaultPrefabConfig<GrassHopperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Armoured_RollerControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Armoured_RollerSpawner : DefaultSpawner<Colosseum_Armoured_RollerControl> { }

    public class Colosseum_Armoured_RollerPrefabConfig : DefaultPrefabConfig<Colosseum_Armoured_RollerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_MinerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class Colosseum_MinerSpawner : DefaultSpawner<Colosseum_MinerControl> { }

    public class Colosseum_MinerPrefabConfig : DefaultPrefabConfig<Colosseum_MinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Shield_ZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class Colosseum_Shield_ZombieSpawner : DefaultSpawner<Colosseum_Shield_ZombieControl> { }

    public class Colosseum_Shield_ZombiePrefabConfig : DefaultPrefabConfig<Colosseum_Shield_ZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mawlek_ColControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class Mawlek_ColSpawner : DefaultSpawner<Mawlek_ColControl> { }

    public class Mawlek_ColPrefabConfig : DefaultPrefabConfig<Mawlek_ColControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ColosseumGrassHopperControl : DefaultSpawnedEnemyControl { }

    public class ColosseumGrassHopperSpawner : DefaultSpawner<ColosseumGrassHopperControl> { }

    public class ColosseumGrassHopperPrefabConfig : DefaultPrefabConfig<ColosseumGrassHopperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomTurretControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(200f, -.5f, true, false);
        }
    }

    public class MushroomTurretSpawner : DefaultSpawner<MushroomTurretControl> { }

    public class MushroomTurretPrefabConfig : DefaultPrefabConfig<MushroomTurretControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieFungusBControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieFungusBSpawner : DefaultSpawner<ZombieFungusBControl> { }

    public class ZombieFungusBPrefabConfig : DefaultPrefabConfig<ZombieFungusBControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class FungCrawlerSpawner : DefaultSpawner<FungCrawlerControl> { }

    public class FungCrawlerPrefabConfig : DefaultPrefabConfig<FungCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AbyssCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: -1f, alsoStickCorpse: false, flipped: true);
        }
    }

    public class AbyssCrawlerSpawner : DefaultSpawner<AbyssCrawlerControl> { }

    public class AbyssCrawlerPrefabConfig : DefaultPrefabConfig<AbyssCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MinesCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(100f, extraOffsetScale: 1.3f, alsoStickCorpse: false, flipped: false);
        }
    }

    public class MinesCrawlerSpawner : DefaultSpawner<MinesCrawlerControl> { }

    public class MinesCrawlerPrefabConfig : DefaultPrefabConfig<MinesCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class CrystalCrawlerSpawner : DefaultSpawner<CrystalCrawlerControl> { }

    public class CrystalCrawlerPrefabConfig : DefaultPrefabConfig<CrystalCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class CrawlerSpawner : DefaultSpawner<CrawlerControl> { }

    public class CrawlerPrefabConfig : DefaultPrefabConfig<CrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class TinySpiderControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }


    public class TinySpiderSpawner : DefaultSpawner<TinySpiderControl> { }

    public class TinySpiderPrefabConfig : DefaultPrefabConfig<TinySpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomBabyControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.thisMetadata.GeoManager.Value += 3;
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class MushroomBabySpawner : DefaultSpawner<MushroomBabyControl> { }

    public class MushroomBabyPrefabConfig : DefaultPrefabConfig<MushroomBabyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomRollerControl : DefaultSpawnedEnemyControl
    {
        public int superEffectsToSpawn = 5;
        public float spawnRate = 0.15f;
        public int chanceToSpawnSuperShroomOutOf100 = 20; // -> ( 20 / 100 )

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var control = gameObject.LocateMyFSM("Mush Roller");

            RNG rng = new RNG();
            rng.Reset();

            int superShroom = rng.Rand(0, 100);

            if (superShroom < chanceToSpawnSuperShroomOutOf100)
            {
                List<Func<GameObject>> trailEffects = new List<Func<GameObject>>()
                {
                    //WARNING: using "CustomSpawnWithLogic" will override any replacement randomization modules
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Mega Jelly Zap", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Falling Barrel", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Electro Zap", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Shot PickAxe", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Dung Ball Small", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Paint Shot P Down", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Paint Shot B_fix", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Paint Shot R", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Gas Explosion Recycle L", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Lil Jellyfish", null, false),
                };

                var selection = trailEffects.GetRandomElementFromList(rng);

                var gasState = control.GetState("In Air");
                gasState.DisableAction(3);
                gasState.InsertCustomAction(() => {
                    StartCoroutine(SuperEffectSpawns(selection));
                }, 3);

                var gasState2 = control.GetState("Roll");
                gasState2.DisableAction(3);
                gasState2.InsertCustomAction(() => {
                    StartCoroutine(SuperEffectSpawns(selection));
                }, 3);

                var glow = EnemyRandomizerDatabase.GetDatabase().Spawn("Summon", null);
                var ge = glow.GetComponent<ParticleSystem>();
                glow.transform.parent = transform;
                glow.transform.localPosition = Vector3.zero;
                ge.simulationSpace = ParticleSystemSimulationSpace.World;
                ge.startSize = 3;
                glow.SetActive(true);
            }
        }

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }

        protected virtual IEnumerator SuperEffectSpawns(Func<GameObject> spawner)
        {
            for(int i = 0; i < superEffectsToSpawn; ++i)
            {
                var result = spawner.Invoke();
                result.transform.position = transform.position;
                result.SetActive(true);
                yield return new WaitForSeconds(spawnRate);
            }
        }
    }

    public class MushroomRollerSpawner : DefaultSpawner<MushroomRollerControl> { }

    public class MushroomRollerPrefabConfig : DefaultPrefabConfig<MushroomRollerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieFungusAControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieFungusASpawner : DefaultSpawner<ZombieFungusAControl> { }

    public class ZombieFungusAPrefabConfig : DefaultPrefabConfig<ZombieFungusAControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround(1f);
        }
    }

    public class MantisSpawner : DefaultSpawner<MantisControl> { }

    public class MantisPrefabConfig : DefaultPrefabConfig<MantisControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GardenZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class GardenZombieSpawner : DefaultSpawner<GardenZombieControl> { }

    public class GardenZombiePrefabConfig : DefaultPrefabConfig<GardenZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossKnightFatControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MossKnightFatSpawner : DefaultSpawner<MossKnightFatControl> { }

    public class MossKnightFatPrefabConfig : DefaultPrefabConfig<MossKnightFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavySpawnControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MantisHeavySpawnSpawner : DefaultSpawner<MantisHeavySpawnControl> { }

    public class MantisHeavySpawnPrefabConfig : DefaultPrefabConfig<MantisHeavySpawnControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GraveZombieControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class GraveZombieSpawner : DefaultSpawner<GraveZombieControl> { }

    public class GraveZombiePrefabConfig : DefaultPrefabConfig<GraveZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMinerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieMinerSpawner : DefaultSpawner<ZombieMinerControl> { }

    public class ZombieMinerPrefabConfig : DefaultPrefabConfig<ZombieMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieBeamMinerSpawner : DefaultSpawner<ZombieBeamMinerControl> { }

    public class ZombieBeamMinerPrefabConfig : DefaultPrefabConfig<ZombieBeamMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadSpControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieHornheadSpSpawner : DefaultSpawner<ZombieHornheadSpControl> { }

    public class ZombieHornheadSpPrefabConfig : DefaultPrefabConfig<ZombieHornheadSpControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerSpControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieRunnerSpSpawner : DefaultSpawner<ZombieRunnerSpControl> { }

    public class ZombieRunnerSpPrefabConfig : DefaultPrefabConfig<ZombieRunnerSpControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlashSpiderControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class SlashSpiderSpawner : DefaultSpawner<SlashSpiderControl> { }

    public class SlashSpiderPrefabConfig : DefaultPrefabConfig<SlashSpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlipHopperControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class FlipHopperSpawner : DefaultSpawner<FlipHopperControl> { }

    public class FlipHopperPrefabConfig : DefaultPrefabConfig<FlipHopperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class FlukemanSpawner : DefaultSpawner<FlukemanControl> { }

    public class FlukemanPrefabConfig : DefaultPrefabConfig<FlukemanControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_02Control : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class fluke_baby_02Spawner : DefaultSpawner<fluke_baby_02Control> { }

    public class fluke_baby_02PrefabConfig : DefaultPrefabConfig<fluke_baby_02Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_01Control : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class fluke_baby_01Spawner : DefaultSpawner<fluke_baby_01Control> { }

    public class fluke_baby_01PrefabConfig : DefaultPrefabConfig<fluke_baby_01Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_03Control : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class fluke_baby_03Spawner : DefaultSpawner<fluke_baby_03Control> { }

    public class fluke_baby_03PrefabConfig : DefaultPrefabConfig<fluke_baby_03Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class EnemyControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class EnemySpawner : DefaultSpawner<EnemyControl> { }

    public class EnemyPrefabConfig : DefaultPrefabConfig<EnemyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalGuardControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class RoyalGuardSpawner : DefaultSpawner<RoyalGuardControl> { }

    public class RoyalGuardPrefabConfig : DefaultPrefabConfig<RoyalGuardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisFlyerChildControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToClosestSurface(200f, false);
        }
    }

    public class MantisFlyerChildSpawner : DefaultSpawner<MantisFlyerChildControl> { }

    public class MantisFlyerChildPrefabConfig : DefaultPrefabConfig<MantisFlyerChildControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FatFlukeControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class FatFlukeSpawner : DefaultSpawner<FatFlukeControl> { }

    public class FatFlukePrefabConfig : DefaultPrefabConfig<FatFlukeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_WormControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class Colosseum_WormSpawner : DefaultSpawner<Colosseum_WormControl> { }

    public class Colosseum_WormPrefabConfig : DefaultPrefabConfig<Colosseum_WormControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class EggSacControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class EggSacSpawner : DefaultSpawner<EggSacControl> { }

    public class EggSacPrefabConfig : DefaultPrefabConfig<EggSacControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////   BANNED -- DONT SPAWN THIS -- REMOVE THIS LATER
    public class CorpseGardenZombieControl : DefaultSpawnedEnemyControl { }

    public class CorpseGardenZombieSpawner : DefaultSpawner<CorpseGardenZombieControl> { }

    public class CorpseGardenZombiePrefabConfig : DefaultPrefabConfig<CorpseGardenZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///








    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////



}
