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
using UniRx;

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
    }

    public class SpittingZombieSpawner : DefaultSpawner<SpittingZombieControl> { }

    public class SpittingZombiePrefabConfig : DefaultPrefabConfig<SpittingZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BurstingZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class BurstingZombieSpawner : DefaultSpawner<BurstingZombieControl> { }

    public class BurstingZombiePrefabConfig : DefaultPrefabConfig<BurstingZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavyControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            gameObject.GetOrAddComponent<PreventOutOfBounds>();
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
    }

    public class Mossman_RunnerSpawner : DefaultSpawner<Mossman_RunnerControl> { }

    public class Mossman_RunnerPrefabConfig : DefaultPrefabConfig<Mossman_RunnerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BabyCentipedeControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Centipede";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Choose Dig Point");
            this.AddResetToStateOnHide(control, "Init");

            var chooseDigPoint = control.GetState("Choose Dig Point");
            DisableActions(chooseDigPoint, 0, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            chooseDigPoint.AddCustomAction(() => {

                var groundUnderHero = SpawnerExtensions.GetGroundRay(HeroController.instance.gameObject);

                var underGroundPoint = groundUnderHero.point + -Vector2.down;

                transform.position = underGroundPoint;

                thisMetadata.MRenderer.enabled = false;
                thisMetadata.PhysicsBody.isKinematic = false;
                thisMetadata.HeroDamage.damageDealt = 0;
                thisMetadata.IsInvincible = true;
                thisMetadata.Collider.enabled = true;

            });

            var dig = control.GetState("Dig");
            DisableActions(dig, 4);
        }

        protected override void SetDefaultPosition()
        {
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
    }

    public class FlukemanBotSpawner : DefaultSpawner<FlukemanBotControl> { }

    public class FlukemanBotPrefabConfig : DefaultPrefabConfig<FlukemanBotControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBlobControl : DefaultSpawnedEnemyControl
    {
    }

    public class MageBlobSpawner : DefaultSpawner<MageBlobControl> { }

    public class MageBlobPrefabConfig : DefaultPrefabConfig<MageBlobControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieRunnerSpawner : DefaultSpawner<ZombieRunnerControl> { }

    public class ZombieRunnerPrefabConfig : DefaultPrefabConfig<ZombieRunnerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieHornheadSpawner : DefaultSpawner<ZombieHornheadControl> { }

    public class ZombieHornheadPrefabConfig : DefaultPrefabConfig<ZombieHornheadControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBargerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieBargerSpawner : DefaultSpawner<ZombieBargerControl> { }

    public class ZombieBargerPrefabConfig : DefaultPrefabConfig<ZombieBargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PrayerSlugControl : DefaultSpawnedEnemyControl
    {
    }

    public class PrayerSlugSpawner : DefaultSpawner<PrayerSlugControl> { }

    public class PrayerSlugPrefabConfig : DefaultPrefabConfig<PrayerSlugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieShieldControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieShieldSpawner : DefaultSpawner<ZombieShieldControl> { }

    public class ZombieShieldPrefabConfig : DefaultPrefabConfig<ZombieShieldControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieLeaperControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieLeaperSpawner : DefaultSpawner<ZombieLeaperControl> { }

    public class ZombieLeaperPrefabConfig : DefaultPrefabConfig<ZombieLeaperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieGuardControl : DefaultSpawnedEnemyControl
    {
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
    }

    public class ZombieMylaSpawner : DefaultSpawner<ZombieMylaControl> { }

    public class ZombieMylaPrefabConfig : DefaultPrefabConfig<ZombieMylaControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieFatControl : DefaultSpawnedEnemyControl
    {
    }

    public class RoyalZombieFatSpawner : DefaultSpawner<RoyalZombieFatControl> { }

    public class RoyalZombieFatPrefabConfig : DefaultPrefabConfig<RoyalZombieFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class RoyalZombieSpawner : DefaultSpawner<RoyalZombieControl> { }

    public class RoyalZombiePrefabConfig : DefaultPrefabConfig<RoyalZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieCowardControl : DefaultSpawnedEnemyControl
    {
    }

    public class RoyalZombieCowardSpawner : DefaultSpawner<RoyalZombieCowardControl> { }

    public class RoyalZombieCowardPrefabConfig : DefaultPrefabConfig<RoyalZombieCowardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GorgeousHuskControl : DefaultSpawnedEnemyControl
    {
        public bool isSuperHusk = false;
        public int isSpecialHuskChanceMax = 20;
        public int isExplodingHuskChance = 5;
        public int isEmissionHuskChance = 10;
        public int isSuperHuskChance = 5;

        public override bool explodeOnDeath => willExplodeOnDeath;
        protected bool willExplodeOnDeath = false;

        public override string spawnEntityOnDeath => entityToSpawnOnDeath;
        protected string entityToSpawnOnDeath;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            willExplodeOnDeath = RollProbability(out int _, isExplodingHuskChance, isSpecialHuskChanceMax);
            bool willEmitOnDeath = RollProbability(out int _, isEmissionHuskChance, isSpecialHuskChanceMax);
            isSuperHusk = RollProbability(out int _, isSuperHuskChance, isSpecialHuskChanceMax);
                                    
            if (willExplodeOnDeath || willEmitOnDeath)
            {
                if (willEmitOnDeath)
                {
                    entityToSpawnOnDeath = "Galien Mini Hammer";
                }

                SetGeoRandomBetween(420, 1420);

                thisMetadata.CurrentHPf = thisMetadata.CurrentHPf * 0.5f;
                thisMetadata.ApplySizeScale(thisMetadata.SizeScale + 0.2f);

                AddParticleEffect_TorchFire();
            }   

            if (isSuperHusk)
            {
                thisMetadata.Geo = thisMetadata.Geo * 2;

                thisMetadata.CurrentHPf = thisMetadata.CurrentHPf * 0.5f;
                thisMetadata.ApplySizeScale(thisMetadata.SizeScale + 0.4f);

                AddParticleEffect_TorchShadeEmissions();

                thisMetadata.currentHP.Subscribe(_ => OnHit()).AddTo(disposables);
            }
        }

        protected virtual void OnHit()
        {
            EnemyRandomizerDatabase.GetDatabase().Spawn("Shot Markoth Nail", null).SafeSetActive(true);
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
        protected override void OnEnable()
        {
            base.OnEnable();
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
    }

    public class RuinsSentrySpawner : DefaultSpawner<RuinsSentryControl> { }

    public class RuinsSentryPrefabConfig : DefaultPrefabConfig<RuinsSentryControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsSentryFatControl : DefaultSpawnedEnemyControl
    {
    }

    public class RuinsSentryFatSpawner : DefaultSpawner<RuinsSentryFatControl> { }

    public class RuinsSentryFatPrefabConfig : DefaultPrefabConfig<RuinsSentryFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GreatShieldZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class GreatShieldZombieSpawner : DefaultSpawner<GreatShieldZombieControl> { }

    public class GreatShieldZombiePrefabConfig : DefaultPrefabConfig<GreatShieldZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossWalkerControl : DefaultSpawnedEnemyControl
    {
    }

    public class MossWalkerSpawner : DefaultSpawner<MossWalkerControl> { }

    public class MossWalkerPrefabConfig : DefaultPrefabConfig<MossWalkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTrapControl : DefaultSpawnedEnemyControl
    {
        protected override void SetDefaultPosition()
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
    }

    public class Mossman_ShakerSpawner : DefaultSpawner<Mossman_ShakerControl> { }

    public class Mossman_ShakerPrefabConfig : DefaultPrefabConfig<Mossman_ShakerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PigeonControl : DefaultSpawnedEnemyControl
    {
    }

    public class PigeonSpawner : DefaultSpawner<PigeonControl> { }

    public class PigeonPrefabConfig : DefaultPrefabConfig<PigeonControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////












    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AcidWalkerControl : DefaultSpawnedEnemyControl
    {
    }

    public class AcidWalkerSpawner : DefaultSpawner<AcidWalkerControl> { }

    public class AcidWalkerPrefabConfig : DefaultPrefabConfig<AcidWalkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTurretControl : DefaultSpawnedEnemyControl
    {
        protected override void SetDefaultPosition()
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
        protected override void SetDefaultPosition()
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
    }

    public class MossKnightSpawner : DefaultSpawner<MossKnightControl> { }

    public class MossKnightPrefabConfig : DefaultPrefabConfig<MossKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrassHopperControl : DefaultSpawnedEnemyControl
    {
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
    }

    public class Colosseum_MinerSpawner : DefaultSpawner<Colosseum_MinerControl> { }

    public class Colosseum_MinerPrefabConfig : DefaultPrefabConfig<Colosseum_MinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Shield_ZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class Colosseum_Shield_ZombieSpawner : DefaultSpawner<Colosseum_Shield_ZombieControl> { }

    public class Colosseum_Shield_ZombiePrefabConfig : DefaultPrefabConfig<Colosseum_Shield_ZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mawlek_ColControl : DefaultSpawnedEnemyControl
    {
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
        protected override void SetDefaultPosition()
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
    }

    public class ZombieFungusBSpawner : DefaultSpawner<ZombieFungusBControl> { }

    public class ZombieFungusBPrefabConfig : DefaultPrefabConfig<ZombieFungusBControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungCrawlerControl : DefaultSpawnedEnemyControl
    {
    }

    public class FungCrawlerSpawner : DefaultSpawner<FungCrawlerControl> { }

    public class FungCrawlerPrefabConfig : DefaultPrefabConfig<FungCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AbyssCrawlerControl : DefaultSpawnedEnemyControl
    {
        protected override void SetDefaultPosition()
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
        protected override void SetDefaultPosition()
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

            SetGeoRandomBetween(1, 50);
        }

        protected override void SetDefaultPosition()
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

        protected Func<GameObject> attackSpawner;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var control = gameObject.LocateMyFSM("Mush Roller");
            attackSpawner = GetRandomAttackSpawnerFunc();

            bool isSuperShroom = RollProbability(out int _, chanceToSpawnSuperShroomOutOf100, 100);

            if (isSuperShroom)
            {
                var airTrail = control.GetState("In Air");
                airTrail.DisableAction(3);
                airTrail.InsertCustomAction(() => {
                    StartTrailEffectSpawns(superEffectsToSpawn, spawnRate, attackSpawner);
                }, 3);

                var groundTrail = control.GetState("Roll");
                groundTrail.DisableAction(3);
                groundTrail.InsertCustomAction(() => {
                    StartTrailEffectSpawns(superEffectsToSpawn, spawnRate, attackSpawner);
                }, 3);

                //add some visual notification that this enemy is different
                AddParticleEffect_WhiteSoulEmissions();
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
    }

    public class ZombieFungusASpawner : DefaultSpawner<ZombieFungusAControl> { }

    public class ZombieFungusAPrefabConfig : DefaultPrefabConfig<ZombieFungusAControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisControl : DefaultSpawnedEnemyControl
    {
        protected override void SetDefaultPosition()
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
    }

    public class GardenZombieSpawner : DefaultSpawner<GardenZombieControl> { }

    public class GardenZombiePrefabConfig : DefaultPrefabConfig<GardenZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossKnightFatControl : DefaultSpawnedEnemyControl
    {
    }

    public class MossKnightFatSpawner : DefaultSpawner<MossKnightFatControl> { }

    public class MossKnightFatPrefabConfig : DefaultPrefabConfig<MossKnightFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavySpawnControl : DefaultSpawnedEnemyControl
    {
    }

    public class MantisHeavySpawnSpawner : DefaultSpawner<MantisHeavySpawnControl> { }

    public class MantisHeavySpawnPrefabConfig : DefaultPrefabConfig<MantisHeavySpawnControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GraveZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class GraveZombieSpawner : DefaultSpawner<GraveZombieControl> { }

    public class GraveZombiePrefabConfig : DefaultPrefabConfig<GraveZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMinerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieMinerSpawner : DefaultSpawner<ZombieMinerControl> { }

    public class ZombieMinerPrefabConfig : DefaultPrefabConfig<ZombieMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieBeamMinerSpawner : DefaultSpawner<ZombieBeamMinerControl> { }

    public class ZombieBeamMinerPrefabConfig : DefaultPrefabConfig<ZombieBeamMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadSpControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieHornheadSpSpawner : DefaultSpawner<ZombieHornheadSpControl> { }

    public class ZombieHornheadSpPrefabConfig : DefaultPrefabConfig<ZombieHornheadSpControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerSpControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieRunnerSpSpawner : DefaultSpawner<ZombieRunnerSpControl> { }

    public class ZombieRunnerSpPrefabConfig : DefaultPrefabConfig<ZombieRunnerSpControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlashSpiderControl : DefaultSpawnedEnemyControl
    {
    }

    public class SlashSpiderSpawner : DefaultSpawner<SlashSpiderControl> { }

    public class SlashSpiderPrefabConfig : DefaultPrefabConfig<SlashSpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlipHopperControl : DefaultSpawnedEnemyControl
    {
    }

    public class FlipHopperSpawner : DefaultSpawner<FlipHopperControl> { }

    public class FlipHopperPrefabConfig : DefaultPrefabConfig<FlipHopperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanControl : DefaultSpawnedEnemyControl
    {
    }

    public class FlukemanSpawner : DefaultSpawner<FlukemanControl> { }

    public class FlukemanPrefabConfig : DefaultPrefabConfig<FlukemanControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_02Control : DefaultSpawnedEnemyControl
    {
        public override bool doBlueHealHeroOnDeath => true;

        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "Inflater";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            thisMetadata.Geo = 2;

            var effect = AddParticleEffect_WhiteSoulEmissions();
            effect.startColor = Color.green;
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
        public override bool doBlueHealHeroOnDeath => true;
        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "Health Scuttler";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
            thisMetadata.Geo = 1;

            var effect = AddParticleEffect_WhiteSoulEmissions();
            effect.startColor = Color.cyan;
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
        public override bool doBlueHealHeroOnDeath => true;
        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "Jelly Egg Bomb";


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.Geo = 3;

            AddParticleEffect_TorchFire();
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
        public override bool doBlueHealHeroOnDeath => true;
        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "HK Plume Prime";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.Geo = 9;

            AddParticleEffect_WhiteSoulEmissions();
        }
    }

    public class EnemySpawner : DefaultSpawner<EnemyControl> { }

    public class EnemyPrefabConfig : DefaultPrefabConfig<EnemyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class RoyalGuardControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Guard";

        public static string ShotKingsGuard = "Shot Kings Guard";
        public static PrefabObject KingsGuardShotObj;

        public GameObject lastBoomerang;

        public int eliteChance = 1;
        public bool isElite;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            isElite = RollProbability(out _, eliteChance, 10);

            if(isElite)
            {
                SetGeoRandomBetween(200, 420);

                thisMetadata.Sprite.color = Color.red;
                AddParticleEffect_TorchFire();
            }

            var throwState = control.GetState("Throw");

            if (KingsGuardShotObj == null)
            {
                var throwObjectAction = throwState.GetAction<CreateObject>(0);
                var throwObjectPrefab = throwObjectAction.gameObject.Value;

                var kgShot = throwObjectPrefab;
                var scene = thisMetadata.ObjectPrefab.source.Scene;
                if(CreateNewDatabasePrefabObject(ShotKingsGuard, kgShot, scene, PrefabObject.PrefabType.Hazard))
                {
                    KingsGuardShotObj = thisMetadata.DB.Objects[ShotKingsGuard];
                }
            }

            //TODO: add a check to see if anything was thrown
            AddTimeoutAction(throwState, "CAUGHT", isElite ? .7f : 3f);

            //replace default boomerang with our custom one
            throwState.DisableAction(0);
            throwState.InsertCustomAction(() => {
                lastBoomerang = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position + new Vector3(0f, -1.5f, 0f),
                    ShotKingsGuard, null, true);
                control.FsmVariables.GetFsmGameObject("Boomerang").Value = lastBoomerang;

                //give them homing boomerangs!
                if(isElite)
                {
                    lastBoomerang.SetActive(false);
                    var homing = lastBoomerang.GetOrAddComponent<Physics2DHomingEffect>();
                    homing.events = new HomingEffect.Events();
                    homing.target = HeroController.instance.transform;
                    homing.startupTime = 3f;
                    homing.startupRate = new AnimationCurve();
                    homing.accelerationOverDistance = new AnimationCurve();
                    homing.maxVelocity = 30f;
                    homing.initialVelocity = DirectionToPlayer() * 5f;
                    homing.startupRate.AddKey(0f, 0f);
                    homing.startupRate.AddKey(0.5f, 0.5f);
                    homing.startupRate.AddKey(1f, 1f);
                    homing.accelerationOverDistance.AddKey(0f, 50f);
                    homing.accelerationOverDistance.AddKey(.4f, 10f);
                    homing.accelerationOverDistance.AddKey(1f, 25f);
                    homing.forceMoveToDistance = 0f;
                    homing.arriveBehaviour = HomingEffect.ArriveBehaviour.Destroy;
                    homing.StartCoroutine(DestroyBoomerangAfterTime(5f));

                    lastBoomerang.SetActive(true);
                    AddParticleEffect_TorchFire(2, lastBoomerang.transform);
                }
            }, 0);

            var throwCatch = control.GetState("Throw Catch");
            throwCatch.InsertCustomAction(() => {

                if (!isElite)
                {
                    if (lastBoomerang != null)
                    {
                        Destroy(lastBoomerang);
                        lastBoomerang = null;
                    }
                }

            }, 0);

            if (isElite)
            {
                var wait = control.GetState("Wait");
                wait.GetAction<WaitRandom>(2).timeMin = 0f;
                wait.GetAction<WaitRandom>(2).timeMax = .1f;

                var idle = control.GetState("Idle");
                wait.GetAction<Wait>(2).time = .1f;

                control.FsmVariables.GetFsmFloat("Run Speed").Value = -15;
                control.FsmVariables.GetFsmFloat("SlashStepSpeed").Value = 30;
            }
        }

        protected override void ScaleHP()
        {
            thisMetadata.CurrentHP = thisMetadata.DefaultHP * 2;
        }

        IEnumerator DestroyBoomerangAfterTime(float time)
        {
            yield return new WaitForSeconds(5f);
            if(lastBoomerang != null)
            {
                SpawnExplosionAt(lastBoomerang.transform.position);
                Destroy(lastBoomerang);
                lastBoomerang = null;
            }
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
        protected override void SetDefaultPosition()
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
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (thisMetadata.SizeScale < 0.7f)
            {
                thisMetadata.DamageDealt = 1;
                thisMetadata.CurrentHPf *= 0.5f;
            }
            else if (thisMetadata.SizeScale < 1.5f)
            {
                thisMetadata.DamageDealt = 3;
                thisMetadata.Geo += 420;
            }
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
    }

    public class Colosseum_WormSpawner : DefaultSpawner<Colosseum_WormControl> { }

    public class Colosseum_WormPrefabConfig : DefaultPrefabConfig<Colosseum_WormControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////  TODO: fix the item transfer
    public class EggSacControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.Geo += 69;
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

    public class HealthScuttlerControl : DefaultSpawnedEnemyControl
    {
    }

    public class HealthScuttlerSpawner : DefaultSpawner<HealthScuttlerControl>
    {
    }

    public class HealthCocoonPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            var flingPrefab = p.prefab.GetComponent<HealthCocoon>().flingPrefabs.First().prefab;
            var prefab = flingPrefab;//"Health Scuttler"

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(prefab.name);

            Dev.Log("HealthCocoon old prefab name = " + p.prefab.name);

            //get actual enemy prefab from the fsm
            p.prefabName = keyName;
            p.prefab = prefab;

            Dev.Log("HealthCocoon New prefab name = " + keyName);
        }
    }

    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////   





    /////
    //////////////////////////////////////////////////////////////////////////////



}
