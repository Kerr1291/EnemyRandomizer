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

    public class TEMPLATE_PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HopperControl : DefaultSpawnedEnemyControl { }

    public class HopperSpawner : DefaultSpawner<HopperControl> { }

    public class HopperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantHopperControl : DefaultSpawnedEnemyControl { }

    public class GiantHopperSpawner : DefaultSpawner<GiantHopperControl> { }

    public class GiantHopperPrefabConfig : DefaultPrefabConfig { }
                                                                             /////
    //////////////////////////////////////////////////////////////////////////////
    
    
    


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpittingZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class SpittingZombieSpawner : DefaultSpawner<SpittingZombieControl> { }

    public class SpittingZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BurstingZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class BurstingZombieSpawner : DefaultSpawner<BurstingZombieControl> { }

    public class BurstingZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavyControl : DefaultSpawnedEnemyControl
    {
    }

    public class MantisHeavySpawner : DefaultSpawner<MantisHeavyControl> { }

    public class MantisHeavyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LesserMawlekControl : DefaultSpawnedEnemyControl
    {
    }

    public class LesserMawlekSpawner : DefaultSpawner<LesserMawlekControl> { }

    public class LesserMawlekPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RollerControl : DefaultSpawnedEnemyControl { }

    public class RollerSpawner : DefaultSpawner<RollerControl> { }

    public class RollerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mossman_RunnerControl : DefaultSpawnedEnemyControl
    {
    }

    public class Mossman_RunnerSpawner : DefaultSpawner<Mossman_RunnerControl> { }

    public class Mossman_RunnerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BabyCentipedeControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Centipede";

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Choose Dig Point");
            //this.AddResetToStateOnHide(control, "Init");

            var chooseDigPoint = control.GetState("Choose Dig Point");
            chooseDigPoint.DisableActions(0, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            chooseDigPoint.AddCustomAction(() => {

                var groundUnderHero = SpawnerExtensions.GetGroundRay(HeroController.instance.gameObject);

                var underGroundPoint = groundUnderHero.point + -Vector2.down;

                transform.position = underGroundPoint;

                MRenderer.enabled = false;
                PhysicsBody.isKinematic = false;
                HeroDamage.damageDealt = 0;
                if(EnemyHealthManager != null)
                    EnemyHealthManager.IsInvincible = true;
                Collider.enabled = true;

            });

            var dig = control.GetState("Dig");
            dig.DisableActions(4);
        }
    }

    public class BabyCentipedeSpawner : DefaultSpawner<BabyCentipedeControl> { }

    public class BabyCentipedePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanBotControl : DefaultSpawnedEnemyControl
    {
    }

    public class FlukemanBotSpawner : DefaultSpawner<FlukemanBotControl> { }

    public class FlukemanBotPrefabConfig : DefaultPrefabConfig { }

    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBlobControl : DefaultSpawnedEnemyControl
    {
    }

    public class MageBlobSpawner : DefaultSpawner<MageBlobControl> { }

    public class MageBlobPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieRunnerSpawner : DefaultSpawner<ZombieRunnerControl> { }

    public class ZombieRunnerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieHornheadSpawner : DefaultSpawner<ZombieHornheadControl> { }

    public class ZombieHornheadPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBargerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieBargerSpawner : DefaultSpawner<ZombieBargerControl> { }

    public class ZombieBargerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PrayerSlugControl : DefaultSpawnedEnemyControl
    {
    }

    public class PrayerSlugSpawner : DefaultSpawner<PrayerSlugControl> { }

    public class PrayerSlugPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieShieldControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieShieldSpawner : DefaultSpawner<ZombieShieldControl> { }

    public class ZombieShieldPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieLeaperControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieLeaperSpawner : DefaultSpawner<ZombieLeaperControl> { }

    public class ZombieLeaperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieGuardControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieGuardSpawner : DefaultSpawner<ZombieGuardControl> { }

    public class ZombieGuardPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMylaControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject other)
        {
            base.Setup(other);

            //this checks if the player has superdash and if not disables the game object
            //which is kinda silly.... so destroy it
            var deactivate = gameObject.GetComponent<DeactivateIfPlayerdataFalse>();
            GameObject.Destroy(deactivate);
        }
    }

    public class ZombieMylaSpawner : DefaultSpawner<ZombieMylaControl> { }

    public class ZombieMylaPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieFatControl : DefaultSpawnedEnemyControl
    {
    }

    public class RoyalZombieFatSpawner : DefaultSpawner<RoyalZombieFatControl> { }

    public class RoyalZombieFatPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class RoyalZombieSpawner : DefaultSpawner<RoyalZombieControl> { }

    public class RoyalZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieCowardControl : DefaultSpawnedEnemyControl
    {
    }

    public class RoyalZombieCowardSpawner : DefaultSpawner<RoyalZombieCowardControl> { }

    public class RoyalZombieCowardPrefabConfig : DefaultPrefabConfig { }
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

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            willExplodeOnDeath = SpawnerExtensions.RollProbability(out int _, isExplodingHuskChance, isSpecialHuskChanceMax);
            bool willEmitOnDeath = SpawnerExtensions.RollProbability(out int _, isEmissionHuskChance, isSpecialHuskChanceMax);
            isSuperHusk = SpawnerExtensions.RollProbability(out int _, isSuperHuskChance, isSpecialHuskChanceMax);
                                    
            if (willExplodeOnDeath || willEmitOnDeath)
            {
                if (willEmitOnDeath)
                {
                    entityToSpawnOnDeath = "Galien Mini Hammer";
                }

                Geo = SpawnerExtensions.GetRandomValueBetween(420, 1420);

                CurrentHPf = CurrentHPf * 0.5f;
                gameObject.ScaleObject(SizeScale + 0.2f);

                gameObject.AddParticleEffect_TorchFire();
            }   

            if (isSuperHusk)
            {
                Geo = Geo * 2;

                CurrentHPf = CurrentHPf * 0.5f;
                gameObject.ScaleObject(SizeScale + 0.4f);

                gameObject.AddParticleEffect_TorchShadeEmissions();
            }
        }

        protected override void OnHit(int dmgAmount)
        {
            if(isSuperHusk)
                gameObject.SpawnEntity("Shot Markoth Nail", true);
        }
    }

    public class GorgeousHuskSpawner : DefaultSpawner<GorgeousHuskControl> { }

    public class GorgeousHuskPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CeilingDropperControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => .5f;
        public override bool spawnOrientationIsFlipped => true;
    }

    public class CeilingDropperSpawner : DefaultSpawner<CeilingDropperControl> { }

    public class CeilingDropperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsSentryControl : DefaultSpawnedEnemyControl
    {
    }

    public class RuinsSentrySpawner : DefaultSpawner<RuinsSentryControl> { }

    public class RuinsSentryPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsSentryFatControl : DefaultSpawnedEnemyControl
    {
    }

    public class RuinsSentryFatSpawner : DefaultSpawner<RuinsSentryFatControl> { }

    public class RuinsSentryFatPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GreatShieldZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class GreatShieldZombieSpawner : DefaultSpawner<GreatShieldZombieControl> { }

    public class GreatShieldZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossWalkerControl : DefaultSpawnedEnemyControl
    {
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;
    }

    public class MossWalkerSpawner : DefaultSpawner<MossWalkerControl> { }

    public class MossWalkerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTrapControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => .3f;
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;
    }

    public class PlantTrapSpawner : DefaultSpawner<PlantTrapControl> { }

    public class PlantTrapPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mossman_ShakerControl : DefaultSpawnedEnemyControl
    {
    }

    public class Mossman_ShakerSpawner : DefaultSpawner<Mossman_ShakerControl> { }

    public class Mossman_ShakerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PigeonControl : DefaultSpawnedEnemyControl
    {
    }

    public class PigeonSpawner : DefaultSpawner<PigeonControl> { }

    public class PigeonPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////












    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AcidWalkerControl : DefaultSpawnedEnemyControl
    {
    }

    public class AcidWalkerSpawner : DefaultSpawner<AcidWalkerControl> { }

    public class AcidWalkerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTurretControl : DefaultSpawnedEnemyControl
    {
        public override bool preventInsideWallsAfterPositioning => false;
        public override bool preventOutOfBoundsAfterPositioning => false;

        public override string FSMName => "Plant Turret";

        public override float spawnPositionOffset => 0f;
        public override bool spawnShouldStickCorpse => true;

        public float shotSpeed = 12f; //taken from the FSM
        public float shotOffset = 0.4f;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            //replace the CreateObject action with our own
            var fire = control.GetState("Fire");
            fire.DisableAction(3);
            fire.InsertCustomAction(() =>
            {
                var dirToHero = gameObject.DirectionToPlayer();

                var shot = control.FsmVariables.GetFsmGameObject("Shot Instance").Value;
                if (shot == null)
                {
                    var spitterShot = SpawnerExtensions.SpawnEntityAt("Spike Ball", pos2d + dirToHero * shotOffset, true);
                    if (spitterShot != null)
                    {
                        shot = spitterShot;
                        control.FsmVariables.GetFsmGameObject("Shot Instance").Value = spitterShot;
                    }
                }

                var body = shot.GetComponent<Rigidbody2D>();
                if (body != null)
                    body.velocity = dirToHero * shotSpeed;
            }, 3);
        }
    }

    public class PlantTurretSpawner : DefaultSpawner<PlantTurretControl> { }

    public class PlantTurretPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    //public class PlantTurretRightControl : DefaultSpawnedEnemyControl
    //{
    //    public override string FSMName => "Plant Turret";

    //    public override float spawnPositionOffset => 0f;
    //    public override bool spawnShouldStickCorpse => true;
    //    public override bool spawnOrientationIsFlipped => true;

    //    public float shotSpeed = 12f; //taken from the FSM
    //    public float shotOffset = 0.4f;

    //    public override void Setup(GameObject other)
    //    {
    //        base.Setup(other);

    //        //replace the CreateObject action with our own
    //        var fire = control.GetState("Fire");
    //        fire.DisableAction(3);
    //        fire.InsertCustomAction(() =>
    //        {
    //            var dirToHero = gameObject.DirectionToPlayer();

    //            var shot = control.FsmVariables.GetFsmGameObject("Shot Instance").Value;
    //            if (shot == null)
    //            {
    //                var spitterShot = SpawnerExtensions.SpawnEntityAt("Spike Ball", pos2d + dirToHero * shotOffset, true);
    //                if (spitterShot != null)
    //                {
    //                    shot = spitterShot;
    //                    control.FsmVariables.GetFsmGameObject("Shot Instance").Value = spitterShot;
    //                }
    //            }

    //            var body = shot.GetComponent<Rigidbody2D>();
    //            if (body != null)
    //                body.velocity = dirToHero * shotSpeed;
    //        }, 3);
    //    }
    //}

    //public class PlantTurretRightSpawner : DefaultSpawner<PlantTurretRightControl> { }

    public class PlantTurretRightPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            //overwrite the upside-down one since the code now works just fine for the upright one in all cases
            var uprightTurret = p.source.Scene.sceneObjects.FirstOrDefault(x => x.LoadedObject.prefabName == "Plant Turret");
            p.prefab = uprightTurret.LoadedObject.prefab;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<PlantTurretControl>();
            //control.isFloorTurret = true;
        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossKnightControl : DefaultSpawnedEnemyControl
    {
    }

    public class MossKnightSpawner : DefaultSpawner<MossKnightControl> { }

    public class MossKnightPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrassHopperControl : DefaultSpawnedEnemyControl
    {
    }

    public class GrassHopperSpawner : DefaultSpawner<GrassHopperControl> { }

    public class GrassHopperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Armoured_RollerControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Armoured_RollerSpawner : DefaultSpawner<Colosseum_Armoured_RollerControl> { }

    public class Colosseum_Armoured_RollerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_MinerControl : DefaultSpawnedEnemyControl
    {
    }

    public class Colosseum_MinerSpawner : DefaultSpawner<Colosseum_MinerControl> { }

    public class Colosseum_MinerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_Shield_ZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class Colosseum_Shield_ZombieSpawner : DefaultSpawner<Colosseum_Shield_ZombieControl> { }

    public class Colosseum_Shield_ZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mawlek_ColControl : DefaultSpawnedEnemyControl
    {
    }

    public class Mawlek_ColSpawner : DefaultSpawner<Mawlek_ColControl> { }

    public class Mawlek_ColPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ColosseumGrassHopperControl : DefaultSpawnedEnemyControl { }

    public class ColosseumGrassHopperSpawner : DefaultSpawner<ColosseumGrassHopperControl> { }

    public class ColosseumGrassHopperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomTurretControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => 2f;
        public override bool spawnShouldStickCorpse => true;
    }

    public class MushroomTurretSpawner : DefaultSpawner<MushroomTurretControl> { }

    public class MushroomTurretPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieFungusBControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieFungusBSpawner : DefaultSpawner<ZombieFungusBControl> { }

    public class ZombieFungusBPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungCrawlerControl : DefaultSpawnedEnemyControl
    {
    }

    public class FungCrawlerSpawner : DefaultSpawner<FungCrawlerControl> { }

    public class FungCrawlerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class AbyssCrawlerControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => -1f;
        public override bool spawnShouldStickCorpse => false;
        public override bool spawnOrientationIsFlipped => true;
    }

    public class AbyssCrawlerSpawner : DefaultSpawner<AbyssCrawlerControl> { }

    public class AbyssCrawlerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MinesCrawlerControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => 1.3f;

    }

    public class MinesCrawlerSpawner : DefaultSpawner<MinesCrawlerControl> { }

    public class MinesCrawlerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalCrawlerControl : DefaultSpawnedEnemyControl
    {
    }

    public class CrystalCrawlerSpawner : DefaultSpawner<CrystalCrawlerControl> { }

    public class CrystalCrawlerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrawlerControl : DefaultSpawnedEnemyControl
    {
    }

    public class CrawlerSpawner : DefaultSpawner<CrawlerControl> { }

    public class CrawlerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class TinySpiderControl : DefaultSpawnedEnemyControl
    {
        //not the kind that shoots
    }


    public class TinySpiderSpawner : DefaultSpawner<TinySpiderControl> { }

    public class TinySpiderPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomBabyControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => 1f;
    }

    public class MushroomBabySpawner : DefaultSpawner<MushroomBabyControl> { }

    public class MushroomBabyPrefabConfig : DefaultPrefabConfig { }
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

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var control = gameObject.LocateMyFSM("Mush Roller");
            attackSpawner = gameObject.GetRandomAttackSpawnerFunc();

            bool isSuperShroom = SpawnerExtensions.RollProbability(out int _, chanceToSpawnSuperShroomOutOf100, 100);

            if (isSuperShroom)
            {
                var airTrail = control.GetState("In Air");
                airTrail.DisableAction(3);
                airTrail.InsertCustomAction(() => {
                    this.StartTrailEffectSpawns(superEffectsToSpawn, spawnRate, attackSpawner);
                }, 3);

                var groundTrail = control.GetState("Roll");
                groundTrail.DisableAction(3);
                groundTrail.InsertCustomAction(() => {
                    this.StartTrailEffectSpawns(superEffectsToSpawn, spawnRate, attackSpawner);
                }, 3);

                //add some visual notification that this enemy is different
                gameObject.AddParticleEffect_WhiteSoulEmissions();
            }
        }
    }

    public class MushroomRollerSpawner : DefaultSpawner<MushroomRollerControl> { }

    public class MushroomRollerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieFungusAControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieFungusASpawner : DefaultSpawner<ZombieFungusAControl> { }

    public class ZombieFungusAPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => 1f;
    }

    public class MantisSpawner : DefaultSpawner<MantisControl> { }

    public class MantisPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GardenZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class GardenZombieSpawner : DefaultSpawner<GardenZombieControl> { }

    public class GardenZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossKnightFatControl : DefaultSpawnedEnemyControl
    {
    }

    public class MossKnightFatSpawner : DefaultSpawner<MossKnightFatControl> { }

    public class MossKnightFatPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavySpawnControl : DefaultSpawnedEnemyControl
    {
    }

    public class MantisHeavySpawnSpawner : DefaultSpawner<MantisHeavySpawnControl> { }

    public class MantisHeavySpawnPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GraveZombieControl : DefaultSpawnedEnemyControl
    {
    }

    public class GraveZombieSpawner : DefaultSpawner<GraveZombieControl> { }

    public class GraveZombiePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMinerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieMinerSpawner : DefaultSpawner<ZombieMinerControl> { }

    public class ZombieMinerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieBeamMinerSpawner : DefaultSpawner<ZombieBeamMinerControl> { }

    public class ZombieBeamMinerPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadSpControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieHornheadSpSpawner : DefaultSpawner<ZombieHornheadSpControl> { }

    public class ZombieHornheadSpPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerSpControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZombieRunnerSpSpawner : DefaultSpawner<ZombieRunnerSpControl> { }

    public class ZombieRunnerSpPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlashSpiderControl : DefaultSpawnedEnemyControl
    {
    }

    public class SlashSpiderSpawner : DefaultSpawner<SlashSpiderControl> { }

    public class SlashSpiderPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlipHopperControl : DefaultSpawnedEnemyControl
    {
    }

    public class FlipHopperSpawner : DefaultSpawner<FlipHopperControl> { }

    public class FlipHopperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FlukemanControl : DefaultSpawnedEnemyControl
    {
    }

    public class FlukemanSpawner : DefaultSpawner<FlukemanControl> { }

    public class FlukemanPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_02Control : DefaultSpawnedEnemyControl
    {
        public override bool doBlueHealHeroOnDeath => true;

        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "Inflater";

        public override void Setup(GameObject other)
        {
            base.Setup(other);
            gameObject.AddParticleEffect_WhiteSoulEmissions(Color.green);
        }
    }

    public class fluke_baby_02Spawner : DefaultSpawner<fluke_baby_02Control> { }

    public class fluke_baby_02PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_01Control : DefaultSpawnedEnemyControl
    {
        public override bool doBlueHealHeroOnDeath => true;
        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "Health Scuttler";

        public override void Setup(GameObject other)
        {
            base.Setup(other);
            gameObject.AddParticleEffect_WhiteSoulEmissions(Color.cyan);
        }
    }

    public class fluke_baby_01Spawner : DefaultSpawner<fluke_baby_01Control> { }

    public class fluke_baby_01PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_03Control : DefaultSpawnedEnemyControl
    {
        public override bool doBlueHealHeroOnDeath => true;
        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "Jelly Egg Bomb";


        public override void Setup(GameObject other)
        {
            base.Setup(other);
            gameObject.AddParticleEffect_TorchFire();
        }
    }

    public class fluke_baby_03Spawner : DefaultSpawner<fluke_baby_03Control> { }

    public class fluke_baby_03PrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class EnemyControl : DefaultSpawnedEnemyControl
    {
        public override bool doBlueHealHeroOnDeath => true;
        public override string spawnEntityOnDeath => spawnOnDeath;
        public string spawnOnDeath = "HK Plume Prime";

        public override void Setup(GameObject other)
        {
            base.Setup(other);
            gameObject.AddParticleEffect_WhiteSoulEmissions();
        }
    }

    public class EnemySpawner : DefaultSpawner<EnemyControl> { }

    public class EnemyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class RoyalGuardControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Guard";

        public static string ShotKingsGuard = "Shot Kings Guard";
        public static PrefabObject KingsGuardShotObj;

        public GameObject lastBoomerang;

        public int eliteChance = 2;
        public bool isElite;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            isElite = SpawnerExtensions.RollProbability(out _, eliteChance, 10);

            if(isElite)
            {
                Geo = SpawnerExtensions.GetRandomValueBetween(200, 420);

                Sprite.color = Color.red;
                gameObject.AddParticleEffect_TorchFire();
            }

            var throwState = control.GetState("Throw");

            if (KingsGuardShotObj == null)
            {
                var throwObjectAction = throwState.GetAction<CreateObject>(0);
                var throwObjectPrefab = throwObjectAction.gameObject.Value;

                var kgShot = throwObjectPrefab;
                var scene = gameObject.GetObjectPrefab().source.Scene;
                if(SpawnerExtensions.CreateNewDatabasePrefabObject(ShotKingsGuard, kgShot, scene, PrefabObject.PrefabType.Hazard))
                {
                    KingsGuardShotObj = EnemyRandomizerDatabase.GetDatabase().Objects[ShotKingsGuard];
                }
            }


            //replace default boomerang with our custom one
            throwState.DisableAction(0);
            throwState.InsertCustomAction(() => {
                lastBoomerang = EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position + new Vector3(0f, 1.5f, 0f),
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
                    homing.initialVelocity = gameObject.DirectionToPlayer() * 5f;
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
                    gameObject.AddParticleEffect_TorchFire(2, lastBoomerang.transform);
                }
            }, 0);
            //TODO: add a check to see if anything was thrown
            control.AddTimeoutAction(throwState, "CAUGHT", isElite ? .7f : 2f);

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

        IEnumerator DestroyBoomerangAfterTime(float time)
        {
            yield return new WaitForSeconds(5f);
            if(lastBoomerang != null)
            {
                lastBoomerang.transform.position.SpawnExplosionAt();
                Destroy(lastBoomerang);
                lastBoomerang = null;
            }
        }
    }

    public class RoyalGuardSpawner : DefaultSpawner<RoyalGuardControl> { }

    public class RoyalGuardPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisFlyerChildControl : DefaultSpawnedEnemyControl
    {
    }

    public class MantisFlyerChildSpawner : DefaultSpawner<MantisFlyerChildControl> { }

    public class MantisFlyerChildPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FatFlukeControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if (SizeScale < 0.7f)
            {
                DamageDealt = 1;
                CurrentHPf *= 0.5f;
            }
            else if (SizeScale > 1.5f)
            {
                DamageDealt = 3;
                Geo += 220;
            }
            else if (SizeScale > 2.4f)
            {
                DamageDealt = 4;
                Geo += 420;
                CurrentHPf *= 1.2f;
            }
        }
    }

    public class FatFlukeSpawner : DefaultSpawner<FatFlukeControl> { }

    public class FatFlukePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_WormControl : DefaultSpawnedEnemyControl
    {
    }

    public class Colosseum_WormSpawner : DefaultSpawner<Colosseum_WormControl> { }

    public class Colosseum_WormPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////  TODO: fix the item transfer
    public class EggSacControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(GameObject other)
        {
            base.Setup(other);

            Geo += 69;
        }
    }

    public class EggSacSpawner : DefaultSpawner<EggSacControl> { }

    public class EggSacPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////   BANNED -- DONT SPAWN THIS -- REMOVE THIS LATER
    public class CorpseGardenZombieControl : DefaultSpawnedEnemyControl { }

    public class CorpseGardenZombieSpawner : DefaultSpawner<CorpseGardenZombieControl> { }

    public class CorpseGardenZombiePrefabConfig : DefaultPrefabConfig { }
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
