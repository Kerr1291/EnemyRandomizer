using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EnemyRandomizerMod.Futils;

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
    public class BuzzerControl : DefaultSpawnedEnemyControl { }

    public class BuzzerSpawner : DefaultSpawner<BuzzerControl> { }

    public class BuzzerPrefabConfig : DefaultPrefabConfig<BuzzerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrawlerControl : DefaultSpawnedEnemyControl { }

    public class CrawlerSpawner : DefaultSpawner<CrawlerControl> { }

    public class CrawlerPrefabConfig : DefaultPrefabConfig<CrawlerControl> { }
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
    public class HopperControl : DefaultSpawnedEnemyControl { }

    public class HopperSpawner : DefaultSpawner<HopperControl> { }

    public class HopperPrefabConfig : DefaultPrefabConfig<HopperControl> { }
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
    public class BlobbleControl : DefaultSpawnedEnemyControl { }

    public class BlobbleSpawner : DefaultSpawner<BlobbleControl> { }

    public class BlobblePrefabConfig : DefaultPrefabConfig<BlobbleControl> { }
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
    public class SpittingZombieControl : DefaultSpawnedEnemyControl { }

    public class SpittingZombieSpawner : DefaultSpawner<SpittingZombieControl> { }

    public class SpittingZombiePrefabConfig : DefaultPrefabConfig<SpittingZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BurstingZombieControl : DefaultSpawnedEnemyControl { }

    public class BurstingZombieSpawner : DefaultSpawner<BurstingZombieControl> { }

    public class BurstingZombiePrefabConfig : DefaultPrefabConfig<BurstingZombieControl> { }
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
    public class MantisHeavyControl : DefaultSpawnedEnemyControl { }

    public class MantisHeavySpawner : DefaultSpawner<MantisHeavyControl> { }

    public class MantisHeavyPrefabConfig : DefaultPrefabConfig<MantisHeavyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LesserMawlekControl : DefaultSpawnedEnemyControl { }

    public class LesserMawlekSpawner : DefaultSpawner<LesserMawlekControl> { }

    public class LesserMawlekPrefabConfig : DefaultPrefabConfig<LesserMawlekControl> { }
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
    public class RollerControl : DefaultSpawnedEnemyControl { }

    public class RollerSpawner : DefaultSpawner<RollerControl> { }

    public class RollerPrefabConfig : DefaultPrefabConfig<RollerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mossman_RunnerControl : DefaultSpawnedEnemyControl { }

    public class Mossman_RunnerSpawner : DefaultSpawner<Mossman_RunnerControl> { }

    public class Mossman_RunnerPrefabConfig : DefaultPrefabConfig<Mossman_RunnerControl> { }
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
    public class BabyCentipedeControl : DefaultSpawnedEnemyControl { }

    public class BabyCentipedeSpawner : DefaultSpawner<BabyCentipedeControl> { }

    public class BabyCentipedePrefabConfig : DefaultPrefabConfig<BabyCentipedeControl> { }
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
    public class FlukemanBotControl : DefaultSpawnedEnemyControl { }

    public class FlukemanBotSpawner : DefaultSpawner<FlukemanBotControl> { }

    public class FlukemanBotPrefabConfig : DefaultPrefabConfig<FlukemanBotControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////











    /////////////////////////////////////////////////////////////////////////////
    ///// (Oblobbles)
    public class MegaFatBeeControl : DefaultSpawnedEnemyControl { }

    public class MegaFatBeeSpawner : DefaultSpawner<MegaFatBeeControl> { }

    public class MegaFatBeePrefabConfig : DefaultPrefabConfig<MegaFatBeeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LobsterControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected override bool ControlCameraLocks => false;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class LobsterSpawner : DefaultSpawner<LobsterControl> { }

    public class LobsterPrefabConfig : DefaultPrefabConfig<LobsterControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageKnightControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mage Knight";

        protected override bool ControlCameraLocks => false;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs =>
            new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                    { "Floor Y",    x => { return x.HeroY; } },
                    { "Tele X Max", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.right, 500f).point.x; } },
                    { "Tele X Min", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.left, 500f).point.x; } },
            };

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");
        }
    }

    public class MageKnightSpawner : DefaultSpawner<MageKnightControl> { }

    public class MageKnightPrefabConfig : DefaultPrefabConfig<MageKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mage";

        protected override bool ControlCameraLocks => false;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs =>
            new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                    { "X Max", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.right, 500f).point.x; } },
                    { "X Min", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.left, 500f).point.x; } },
                    { "Y Max", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.up, 500f).point.y; } },
                    { "Y Min", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.down, 500f).point.y; } },
            };

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            control.ChangeTransition("Tele Away", "FINISHED", "Init");

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");

            var st = control.GetState("Select Target");
            
            //disable the teleplane actions
            st.DisableAction(1);
            st.DisableAction(2);

            //add an action that updates the refs
            st.InsertCustomAction(() => this.UpdateRefs(control, FloatRefs), 1);
        }
    }

    public class MageSpawner : DefaultSpawner<MageControl> { }

    public class MagePrefabConfig : DefaultPrefabConfig<MageControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ElectricMageControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Electric Mage";

        protected override bool ControlCameraLocks => false;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs =>
            new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                    { "X Max", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.right, 500f).point.x; } },
                    { "X Min", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.left, 500f).point.x; } },
                    { "Y Max", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.up, 500f).point.y; } },
                    { "Y Min", x => { return new Vector2(x.HeroX,x.HeroY).FireRayGlobal(Vector2.down, 500f).point.y; } },
            };

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");

            var st = control.GetState("Select Target");

            //disable the teleplane actions
            st.DisableAction(1);
            st.DisableAction(2);

            //add an action that updates the refs
            st.InsertCustomAction(() => this.UpdateRefs(control, FloatRefs), 1);
        }
    }

    public class ElectricMageSpawner : DefaultSpawner<ElectricMageControl> { }

    public class ElectricMagePrefabConfig : DefaultPrefabConfig<ElectricMageControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageBlobControl : DefaultSpawnedEnemyControl { }

    public class MageBlobSpawner : DefaultSpawner<MageBlobControl> { }

    public class MageBlobPrefabConfig : DefaultPrefabConfig<MageBlobControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class LancerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var init = control.GetState("Init");
            init.DisableAction(6);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");

            this.OverrideState(control, "Defeat", () =>
            {
                this.thisMetadata.EnemyHealthManager.hasSpecialDeath = false;
                this.thisMetadata.EnemyHealthManager.SetSendKilledToObject(null);
                this.thisMetadata.EnemyHealthManager.Die(null, AttackTypes.Generic, true);
            });
        }
    }

    public class LancerSpawner : DefaultSpawner<LancerControl> { }

    public class LancerPrefabConfig : DefaultPrefabConfig<LancerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerControl : DefaultSpawnedEnemyControl { }

    public class ZombieRunnerSpawner : DefaultSpawner<ZombieRunnerControl> { }

    public class ZombieRunnerPrefabConfig : DefaultPrefabConfig<ZombieRunnerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MenderBugControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mender Bug Ctrl";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //disable the mender state killed player data
            var killed = control.GetState("Killed");
            killed.DisableAction(0);

            var destroy = control.GetState("Destroy");
            destroy.DisableAction(0);

            var fly = control.GetState("Fly");
            fly.ChangeTransition("DESTROY", "Init");

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");

            //this.OverrideState(control, "Defeat", () =>
            //{
            //    this.thisMetadata.EnemyHealthManager.hasSpecialDeath = false;
            //    this.thisMetadata.EnemyHealthManager.SetSendKilledToObject(null);
            //    this.thisMetadata.EnemyHealthManager.Die(null, AttackTypes.Generic, true);
            //});
        }
    }

    public class MenderBugSpawner : DefaultSpawner<MenderBugControl> { }

    public class MenderBugPrefabConfig : DefaultPrefabConfig<MenderBugControl> { }
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
    public class ZombieHornheadControl : DefaultSpawnedEnemyControl { }

    public class ZombieHornheadSpawner : DefaultSpawner<ZombieHornheadControl> { }

    public class ZombieHornheadPrefabConfig : DefaultPrefabConfig<ZombieHornheadControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBargerControl : DefaultSpawnedEnemyControl { }

    public class ZombieBargerSpawner : DefaultSpawner<ZombieBargerControl> { }

    public class ZombieBargerPrefabConfig : DefaultPrefabConfig<ZombieBargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PrayerSlugControl : DefaultSpawnedEnemyControl { }

    public class PrayerSlugSpawner : DefaultSpawner<PrayerSlugControl> { }

    public class PrayerSlugPrefabConfig : DefaultPrefabConfig<PrayerSlugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlockerControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var fsm = gameObject.LocateMyFSM("Blocker Control");

            //allow players without spells to kill blockers
            int level = GameManager.instance.GetPlayerDataInt("fireballLevel");
            if (level <= 0)
            {
                thisMetadata.EnemyHealthManager.InvincibleFromDirection = -1;

                //disable the invincible state
                var init = fsm.GetState("Init");
                init.DisableAction(6);

                //disable the invincible state
                var close = fsm.GetState("Close");
                close.DisableAction(0);
            }

            //ignore the checks, allow it to spawn rollers all the time
            this.OverrideState(fsm, "Can Roller?", () =>
            {
                //TODO: actual logic for spawning rollers, for now, some rng
                RNG rng = new RNG();
                rng.Reset();

                bool result = rng.Randf() > .5f;
                if (result)
                    fsm.SendEvent("FINISHED");
                else
                    fsm.SendEvent("GOOP");
            });

            //link the shot to a roller prefab
            var roller = fsm.GetState("Roller");
            var setgoa = roller.GetFirstActionOfType<SetGameObject>();
            setgoa.gameObject = EnemyRandomizerDatabase.GetDatabase().Enemies["Roller"].prefab;

            //have it skip the roller assign state
            fsm.ChangeTransition("Fire", "FINISHED", "Shot Anim End");
        }
    }

    public class BlockerSpawner : DefaultSpawner<BlockerControl> { }

    public class BlockerPrefabConfig : DefaultPrefabConfig<BlockerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieShieldControl : DefaultSpawnedEnemyControl { }

    public class ZombieShieldSpawner : DefaultSpawner<ZombieShieldControl> { }

    public class ZombieShieldPrefabConfig : DefaultPrefabConfig<ZombieShieldControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieLeaperControl : DefaultSpawnedEnemyControl { }

    public class ZombieLeaperSpawner : DefaultSpawner<ZombieLeaperControl> { }

    public class ZombieLeaperPrefabConfig : DefaultPrefabConfig<ZombieLeaperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieGuardControl : DefaultSpawnedEnemyControl { }

    public class ZombieGuardSpawner : DefaultSpawner<ZombieGuardControl> { }

    public class ZombieGuardPrefabConfig : DefaultPrefabConfig<ZombieGuardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMylaControl : DefaultSpawnedEnemyControl { }

    public class ZombieMylaSpawner : DefaultSpawner<ZombieMylaControl> { }

    public class ZombieMylaPrefabConfig : DefaultPrefabConfig<ZombieMylaControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////









    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieFatControl : DefaultSpawnedEnemyControl { }

    public class RoyalZombieFatSpawner : DefaultSpawner<RoyalZombieFatControl> { }

    public class RoyalZombieFatPrefabConfig : DefaultPrefabConfig<RoyalZombieFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieControl : DefaultSpawnedEnemyControl { }

    public class RoyalZombieSpawner : DefaultSpawner<RoyalZombieControl> { }

    public class RoyalZombiePrefabConfig : DefaultPrefabConfig<RoyalZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalZombieCowardControl : DefaultSpawnedEnemyControl { }

    public class RoyalZombieCowardSpawner : DefaultSpawner<RoyalZombieCowardControl> { }

    public class RoyalZombieCowardPrefabConfig : DefaultPrefabConfig<RoyalZombieCowardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GorgeousHuskControl : DefaultSpawnedEnemyControl { }

    public class GorgeousHuskSpawner : DefaultSpawner<GorgeousHuskControl> { }

    public class GorgeousHuskPrefabConfig : DefaultPrefabConfig<GorgeousHuskControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CeilingDropperControl : DefaultSpawnedEnemyControl { }

    public class CeilingDropperSpawner : DefaultSpawner<CeilingDropperControl> { }

    public class CeilingDropperPrefabConfig : DefaultPrefabConfig<CeilingDropperControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RuinsSentryControl : DefaultSpawnedEnemyControl { }

    public class RuinsSentrySpawner : DefaultSpawner<RuinsSentryControl> { }

    public class RuinsSentryPrefabConfig : DefaultPrefabConfig<RuinsSentryControl> { }
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
    public class RuinsSentryFatControl : DefaultSpawnedEnemyControl { }

    public class RuinsSentryFatSpawner : DefaultSpawner<RuinsSentryFatControl> { }

    public class RuinsSentryFatPrefabConfig : DefaultPrefabConfig<RuinsSentryFatControl> { }
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
    public class GreatShieldZombieControl : DefaultSpawnedEnemyControl { }

    public class GreatShieldZombieSpawner : DefaultSpawner<GreatShieldZombieControl> { }

    public class GreatShieldZombiePrefabConfig : DefaultPrefabConfig<GreatShieldZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class BlackKnightControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Black Knight";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            this.InsertHiddenState(control, "Init Facing", "FINISHED", "Bugs In");
            this.AddResetToStateOnHide(control, "Init");

            control.GetState("Cloud Stop").DisableAction(3);
            control.GetState("Cloud Stop").DisableAction(4);

            control.ChangeTransition("Bugs In End", "FINISHED", "Roar End");
        }
    }

    public class BlackKnightSpawner : DefaultSpawner<BlackKnightControl> { }

    public class BlackKnightPrefabConfig : DefaultPrefabConfig<BlackKnightControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossWalkerControl : DefaultSpawnedEnemyControl { }

    public class MossWalkerSpawner : DefaultSpawner<MossWalkerControl> { }

    public class MossWalkerPrefabConfig : DefaultPrefabConfig<MossWalkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTrapControl : DefaultSpawnedEnemyControl { }

    public class PlantTrapSpawner : DefaultSpawner<PlantTrapControl> { }

    public class PlantTrapPrefabConfig : DefaultPrefabConfig<PlantTrapControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mossman_ShakerControl : DefaultSpawnedEnemyControl { }

    public class Mossman_ShakerSpawner : DefaultSpawner<Mossman_ShakerControl> { }

    public class Mossman_ShakerPrefabConfig : DefaultPrefabConfig<Mossman_ShakerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PigeonControl : DefaultSpawnedEnemyControl { }

    public class PigeonSpawner : DefaultSpawner<PigeonControl> { }

    public class PigeonPrefabConfig : DefaultPrefabConfig<PigeonControl> { }
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
    public class AcidWalkerControl : DefaultSpawnedEnemyControl { }

    public class AcidWalkerSpawner : DefaultSpawner<AcidWalkerControl> { }

    public class AcidWalkerPrefabConfig : DefaultPrefabConfig<AcidWalkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTurretControl : DefaultSpawnedEnemyControl { }

    public class PlantTurretSpawner : DefaultSpawner<PlantTurretControl> { }

    public class PlantTurretPrefabConfig : DefaultPrefabConfig<PlantTurretControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PlantTurretRightControl : DefaultSpawnedEnemyControl { }

    public class PlantTurretRightSpawner : DefaultSpawner<PlantTurretRightControl> { }

    public class PlantTurretRightPrefabConfig : DefaultPrefabConfig<PlantTurretRightControl> { }
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
    public class MossKnightControl : DefaultSpawnedEnemyControl { }

    public class MossKnightSpawner : DefaultSpawner<MossKnightControl> { }

    public class MossKnightPrefabConfig : DefaultPrefabConfig<MossKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrassHopperControl : DefaultSpawnedEnemyControl { }

    public class GrassHopperSpawner : DefaultSpawner<GrassHopperControl> { }

    public class GrassHopperPrefabConfig : DefaultPrefabConfig<GrassHopperControl> { }
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
    public class Colosseum_Armoured_RollerControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Armoured_RollerSpawner : DefaultSpawner<Colosseum_Armoured_RollerControl> { }

    public class Colosseum_Armoured_RollerPrefabConfig : DefaultPrefabConfig<Colosseum_Armoured_RollerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_MinerControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_MinerSpawner : DefaultSpawner<Colosseum_MinerControl> { }

    public class Colosseum_MinerPrefabConfig : DefaultPrefabConfig<Colosseum_MinerControl> { }
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
    public class Colosseum_Shield_ZombieControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Shield_ZombieSpawner : DefaultSpawner<Colosseum_Shield_ZombieControl> { }

    public class Colosseum_Shield_ZombiePrefabConfig : DefaultPrefabConfig<Colosseum_Shield_ZombieControl> { }
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
    public class Colosseum_Flying_SentryControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_Flying_SentrySpawner : DefaultSpawner<Colosseum_Flying_SentryControl> { }

    public class Colosseum_Flying_SentryPrefabConfig : DefaultPrefabConfig<Colosseum_Flying_SentryControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Mawlek_ColControl : DefaultSpawnedEnemyControl { }

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
    public class FungoonBabyControl : DefaultSpawnedEnemyControl { }

    public class FungoonBabySpawner : DefaultSpawner<FungoonBabyControl> { }

    public class FungoonBabyPrefabConfig : DefaultPrefabConfig<FungoonBabyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomTurretControl : DefaultSpawnedEnemyControl { }

    public class MushroomTurretSpawner : DefaultSpawner<MushroomTurretControl> { }

    public class MushroomTurretPrefabConfig : DefaultPrefabConfig<MushroomTurretControl> { }
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
    public class ZombieFungusBControl : DefaultSpawnedEnemyControl { }

    public class ZombieFungusBSpawner : DefaultSpawner<ZombieFungusBControl> { }

    public class ZombieFungusBPrefabConfig : DefaultPrefabConfig<ZombieFungusBControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FungCrawlerControl : DefaultSpawnedEnemyControl { }

    public class FungCrawlerSpawner : DefaultSpawner<FungCrawlerControl> { }

    public class FungCrawlerPrefabConfig : DefaultPrefabConfig<FungCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomBrawlerControl : DefaultSpawnedEnemyControl { }

    public class MushroomBrawlerSpawner : DefaultSpawner<MushroomBrawlerControl> { }

    public class MushroomBrawlerPrefabConfig : DefaultPrefabConfig<MushroomBrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomBabyControl : DefaultSpawnedEnemyControl { }

    public class MushroomBabySpawner : DefaultSpawner<MushroomBabyControl> { }

    public class MushroomBabyPrefabConfig : DefaultPrefabConfig<MushroomBabyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomRollerControl : DefaultSpawnedEnemyControl { }

    public class MushroomRollerSpawner : DefaultSpawner<MushroomRollerControl> { }

    public class MushroomRollerPrefabConfig : DefaultPrefabConfig<MushroomRollerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieFungusAControl : DefaultSpawnedEnemyControl { }

    public class ZombieFungusASpawner : DefaultSpawner<ZombieFungusAControl> { }

    public class ZombieFungusAPrefabConfig : DefaultPrefabConfig<ZombieFungusAControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisControl : DefaultSpawnedEnemyControl { }

    public class MantisSpawner : DefaultSpawner<MantisControl> { }

    public class MantisPrefabConfig : DefaultPrefabConfig<MantisControl> { }
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
    public class GardenZombieControl : DefaultSpawnedEnemyControl { }

    public class GardenZombieSpawner : DefaultSpawner<GardenZombieControl> { }

    public class GardenZombiePrefabConfig : DefaultPrefabConfig<GardenZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisTraitorLordControl : DefaultSpawnedEnemyControl { }

    public class MantisTraitorLordSpawner : DefaultSpawner<MantisTraitorLordControl> { }

    public class MantisTraitorLordPrefabConfig : DefaultPrefabConfig<MantisTraitorLordControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossKnightFatControl : DefaultSpawnedEnemyControl { }

    public class MossKnightFatSpawner : DefaultSpawner<MossKnightFatControl> { }

    public class MossKnightFatPrefabConfig : DefaultPrefabConfig<MossKnightFatControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MantisHeavySpawnControl : DefaultSpawnedEnemyControl { }

    public class MantisHeavySpawnSpawner : DefaultSpawner<MantisHeavySpawnControl> { }

    public class MantisHeavySpawnPrefabConfig : DefaultPrefabConfig<MantisHeavySpawnControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GraveZombieControl : DefaultSpawnedEnemyControl { }

    public class GraveZombieSpawner : DefaultSpawner<GraveZombieControl> { }

    public class GraveZombiePrefabConfig : DefaultPrefabConfig<GraveZombieControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystalCrawlerControl : DefaultSpawnedEnemyControl { }

    public class CrystalCrawlerSpawner : DefaultSpawner<CrystalCrawlerControl> { }

    public class CrystalCrawlerPrefabConfig : DefaultPrefabConfig<CrystalCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieMinerControl : DefaultSpawnedEnemyControl { }

    public class ZombieMinerSpawner : DefaultSpawner<ZombieMinerControl> { }

    public class ZombieMinerPrefabConfig : DefaultPrefabConfig<ZombieMinerControl> { }
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
    public class MinesCrawlerControl : DefaultSpawnedEnemyControl { }

    public class MinesCrawlerSpawner : DefaultSpawner<MinesCrawlerControl> { }

    public class MinesCrawlerPrefabConfig : DefaultPrefabConfig<MinesCrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerControl : DefaultSpawnedEnemyControl { }

    public class ZombieBeamMinerSpawner : DefaultSpawner<ZombieBeamMinerControl> { }

    public class ZombieBeamMinerPrefabConfig : DefaultPrefabConfig<ZombieBeamMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpiderMiniControl : DefaultSpawnedEnemyControl { }

    public class SpiderMiniSpawner : DefaultSpawner<SpiderMiniControl> { }

    public class SpiderMiniPrefabConfig : DefaultPrefabConfig<SpiderMiniControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHornheadSpControl : DefaultSpawnedEnemyControl { }

    public class ZombieHornheadSpSpawner : DefaultSpawner<ZombieHornheadSpControl> { }

    public class ZombieHornheadSpPrefabConfig : DefaultPrefabConfig<ZombieHornheadSpControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieRunnerSpControl : DefaultSpawnedEnemyControl { }

    public class ZombieRunnerSpSpawner : DefaultSpawner<ZombieRunnerSpControl> { }

    public class ZombieRunnerSpPrefabConfig : DefaultPrefabConfig<ZombieRunnerSpControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CentipedeHatcherControl : DefaultSpawnedEnemyControl { }

    public class CentipedeHatcherSpawner : DefaultSpawner<CentipedeHatcherControl> { }

    public class CentipedeHatcherPrefabConfig : DefaultPrefabConfig<CentipedeHatcherControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlashSpiderControl : DefaultSpawnedEnemyControl { }

    public class SlashSpiderSpawner : DefaultSpawner<SlashSpiderControl> { }

    public class SlashSpiderPrefabConfig : DefaultPrefabConfig<SlashSpiderControl> { }
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
    public class AbyssCrawlerControl : DefaultSpawnedEnemyControl { }

    public class AbyssCrawlerSpawner : DefaultSpawner<AbyssCrawlerControl> { }

    public class AbyssCrawlerPrefabConfig : DefaultPrefabConfig<AbyssCrawlerControl> { }
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
    public class FlipHopperControl : DefaultSpawnedEnemyControl { }

    public class FlipHopperSpawner : DefaultSpawner<FlipHopperControl> { }

    public class FlipHopperPrefabConfig : DefaultPrefabConfig<FlipHopperControl> { }
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
    public class FlukemanControl : DefaultSpawnedEnemyControl { }

    public class FlukemanSpawner : DefaultSpawner<FlukemanControl> { }

    public class FlukemanPrefabConfig : DefaultPrefabConfig<FlukemanControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_02Control : DefaultSpawnedEnemyControl { }

    public class fluke_baby_02Spawner : DefaultSpawner<fluke_baby_02Control> { }

    public class fluke_baby_02PrefabConfig : DefaultPrefabConfig<fluke_baby_02Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_01Control : DefaultSpawnedEnemyControl { }

    public class fluke_baby_01Spawner : DefaultSpawner<fluke_baby_01Control> { }

    public class fluke_baby_01PrefabConfig : DefaultPrefabConfig<fluke_baby_01Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class fluke_baby_03Control : DefaultSpawnedEnemyControl { }

    public class fluke_baby_03Spawner : DefaultSpawner<fluke_baby_03Control> { }

    public class fluke_baby_03PrefabConfig : DefaultPrefabConfig<fluke_baby_03Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class EnemyControl : DefaultSpawnedEnemyControl { }

    public class EnemySpawner : DefaultSpawner<EnemyControl> { }

    public class EnemyPrefabConfig : DefaultPrefabConfig<EnemyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class RoyalGuardControl : DefaultSpawnedEnemyControl { }

    public class RoyalGuardSpawner : DefaultSpawner<RoyalGuardControl> { }

    public class RoyalGuardPrefabConfig : DefaultPrefabConfig<RoyalGuardControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieHiveControl : DefaultSpawnedEnemyControl { }

    public class ZombieHiveSpawner : DefaultSpawner<ZombieHiveControl> { }

    public class ZombieHivePrefabConfig : DefaultPrefabConfig<ZombieHiveControl> { }
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
    public class MantisFlyerChildControl : DefaultSpawnedEnemyControl { }

    public class MantisFlyerChildSpawner : DefaultSpawner<MantisFlyerChildControl> { }

    public class MantisFlyerChildPrefabConfig : DefaultPrefabConfig<MantisFlyerChildControl> { }
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
    public class FlukeMotherControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Fluke Mother";

        public AudioPlayerOneShotSingle squirtA;
        public AudioPlayerOneShotSingle squirtB;

        public UnityEngine.Bounds spawnArea;

        protected override bool HeroInAggroRange()
        {
            return true;
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var surfaces = gameObject.GetNearestSurfaces(500f);

            var w = surfaces[Vector2.right].point.x - surfaces[Vector2.left].point.x;
            var h = surfaces[Vector2.up].point.y - surfaces[Vector2.down].point.y;

            spawnArea = new UnityEngine.Bounds(gameObject.transform.position, new Vector3(w, h, 0f));

            var db = EnemyRandomizerDatabase.GetDatabase();

            var spawn2 = control.GetState("Spawn 2");

            squirtA = spawn2.GetAction<AudioPlayerOneShotSingle>(9);
            squirtB = spawn2.GetAction<AudioPlayerOneShotSingle>(10);

            var init = control.GetState("Init");
            init.DisableAction(2);
            init.DisableAction(3);
            init.DisableAction(5);
            init.DisableAction(7);
            init.DisableAction(8);

            init.RemoveTransition("GG BOSS");

            var idle = control.GetState("Idle");
            var playIdle = control.GetState("Play Idle");

            idle.DisableAction(4);
            idle.DisableAction(3);

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);
            roarStart.DisableAction(4);
            roarStart.DisableAction(7);
            roarStart.DisableAction(8);
            roarStart.DisableAction(9);
            roarStart.DisableAction(10);
            roarStart.DisableAction(11);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(0);
            roarEnd.DisableAction(1);


            var rage = control.GetState("Rage");
            var customSpawn = control.AddState("Custom Spawn");

            customSpawn.AddCustomAction(() =>
            {
                var fly = db.Spawn("Fluke Fly", null);

                RNG rng = new RNG();
                rng.Reset();

                var spawn = rng.Rand(spawnArea.min, spawnArea.max);
                fly.transform.position = spawn;

                fly.gameObject.SetActive(true);
            });

            customSpawn.AddAction(squirtA);
            customSpawn.AddAction(squirtB);

            rage.ChangeTransition("SPAWN", "Custom Spawn");
        }
    }

    public class FlukeMotherSpawner : DefaultSpawner<FlukeMotherControl> { }

    public class FlukeMotherPrefabConfig : DefaultPrefabConfig<FlukeMotherControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MimicSpiderControl : DefaultSpawnedEnemyControl { }

    public class MimicSpiderSpawner : DefaultSpawner<MimicSpiderControl> { }

    public class MimicSpiderPrefabConfig : DefaultPrefabConfig<MimicSpiderControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss1Control : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> HornetFloatRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => HornetFloatRefs;

        public Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 16);
        public float floorY => heroPos2d.FireRayGlobal(Vector2.down, 50f).point.y;
        public float roofY => heroPos2d.FireRayGlobal(Vector2.up, 200f).point.y;
        public float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, 100f).point.y;
        public float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, 100f).point.y;

        //values taken from hornet's FSM
        public float sphereHeight = 33.8f - 27.55f;
        public float minDstabHeight = 33.31f - 27.55f;
        public float airDashHeight = 31.5f - 27.55f;
        public float throwXLoffset = 22.51f - 15.13f;
        public float throwXRoffset = 37.9f - 30.16f;

        public Vector2 sizeOfAggroArea = new Vector2(50f, 50f);
        public Vector2 centerOfAggroArea => gameObject.transform.position;
        public UnityEngine.Bounds aggroBounds => new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

        protected override bool HeroInAggroRange()
        {
            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            HornetFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Wall X Left" , x => edgeL},
                {"Wall X Right" , x => edgeR},
                {"Floor Y" , x => floorY},
                {"Roof Y" , x => roofY},
                {"Sphere Y" , x => floorY + sphereHeight},
                {"Air Dash Height" , x => floorY + sphereHeight},
                {"Min Dstab Height" , x => floorY + minDstabHeight},
                {"Throw X L" , x => edgeL + throwXLoffset},
                {"Throw X R" , x => edgeR - throwXRoffset},
            };

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("WAKE");
            this.OverrideState(control, "Inert", () => control.SendEvent("REFIGHT"));

            var refightReady = control.GetState("Refight Ready");
            refightReady.DisableAction(4);
            refightReady.DisableAction(5);
            refightReady.DisableAction(6);
            refightReady.AddCustomAction(() => control.SendEvent("WAKE"));

            var refightWake = control.GetState("Refight Wake");
            refightWake.DisableAction(0);
            refightWake.ChangeTransition("FINISHED", "Flourish");

            var flourish = control.GetState("Flourish");
            flourish.DisableAction(2);
            flourish.DisableAction(3);
            flourish.DisableAction(4);
            flourish.DisableAction(5);

            this.InsertHiddenState(control, "Refight Ready", "WAKE", "Refight Wake");
            this.AddResetToStateOnHide(control, "Refight Ready");
        }
    }

    public class HornetBoss1Spawner : DefaultSpawner<HornetBoss1Control> { }

    public class HornetBoss1PrefabConfig : DefaultPrefabConfig<HornetBoss1Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetBoss2Control : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> HornetFloatRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => HornetFloatRefs;

        public Vector2 pos2d => new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        public Vector2 heroPos2d => new Vector2(HeroController.instance.transform.position.x, HeroController.instance.transform.position.y);
        public Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 16);
        public float floorY => heroPos2d.FireRayGlobal(Vector2.down, 50f).point.y;
        public float roofY => heroPos2d.FireRayGlobal(Vector2.up, 200f).point.y;
        public float edgeL => heroPosWithOffset.FireRayGlobal(Vector2.left, 100f).point.y;
        public float edgeR => heroPosWithOffset.FireRayGlobal(Vector2.right, 100f).point.y;

        //values taken from hornet's FSM
        public float sphereHeight = 33.8f - 27.55f;
        public float minDstabHeight = 33.31f - 27.55f;
        public float airDashHeight = 31.5f - 27.55f;
        public float throwXLoffset = 22.51f - 15.13f;
        public float throwXRoffset = 37.9f - 30.16f;

        public Vector2 sizeOfAggroArea = new Vector2(50f, 50f);
        public Vector2 centerOfAggroArea => gameObject.transform.position;
        public UnityEngine.Bounds aggroBounds => new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

        protected override bool HeroInAggroRange()
        {
            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            HornetFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"Wall X Left" , x => edgeL},
                {"Wall X Right" , x => edgeR},
                {"Floor Y" , x => floorY},
                {"Roof Y" , x => roofY},
                {"Sphere Y" , x => floorY + sphereHeight},
                {"Air Dash Height" , x => floorY + sphereHeight},
                {"Min Dstab Height" , x => floorY + minDstabHeight},
                {"Throw X L" , x => edgeL + throwXLoffset},
                {"Throw X R" , x => edgeR - throwXRoffset},
            };

            var inert = control.GetState("Inert");
            inert.RemoveTransition("GG BOSS");
            inert.RemoveTransition("BATTLE START");
            this.OverrideState(control, "Inert", () => control.SendEvent("REFIGHT"));

            var refightReady = control.GetState("Refight Ready");
            refightReady.DisableAction(3);
            refightReady.DisableAction(5);
            refightReady.AddCustomAction(() => control.SendEvent("WAKE"));

            refightReady.ChangeTransition("WAKE", "Refight Wake");

            var refightWake = control.GetState("Refight Wake");
            this.OverrideState(control, "Refight Wake", () => control.SendEvent("FINISHED"));
            refightWake.ChangeTransition("FINISHED", "Wake");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Flourish");

            var flourish = control.GetState("Flourish");
            flourish.DisableAction(3);
            flourish.DisableAction(4);
            flourish.DisableAction(5);
            flourish.DisableAction(6);

            this.InsertHiddenState(control, "Refight Ready", "WAKE", "Refight Wake");
            this.AddResetToStateOnHide(control, "Refight Ready");
        }
    }

    public class HornetBoss2Spawner : DefaultSpawner<HornetBoss2Control> { }

    public class HornetBoss2PrefabConfig : DefaultPrefabConfig<HornetBoss2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaZombieBeamMinerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Beam Miner";
        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;


        protected Tk2dPlayAnimation sleepAnim;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            DisableSendEvents(control,
                ("Land", 2),
                ("Roar", 1)
                );

            var sleep = control.GetState("Sleep");
            sleepAnim = sleep.GetFirstActionOfType<Tk2dPlayAnimation>();

            var deparents = control.GetState("Deparents");
            deparents.AddAction(sleepAnim);

            this.InsertHiddenState(control, "Deparents", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Deparents");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Idle");

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);//disable roar sound
            var roar = control.GetState("Roar");//make the roar emit no waves and be silent
            roar.DisableAction(2);
            roar.DisableAction(3);
            roar.DisableAction(4);
            roar.DisableAction(5);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(1);
        }

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return previousHP * 2;
        }
    }

    public class MegaZombieBeamMinerSpawner : DefaultSpawner<MegaZombieBeamMinerControl> { }

    public class MegaZombieBeamMinerPrefabConfig : DefaultPrefabConfig<MegaZombieBeamMinerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ZombieBeamMinerRematchControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Beam Miner";
        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;


        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            DisableSendEvents(control,
                ("Land", 2),
                ("Roar", 1)
                );


            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            this.AddResetToStateOnHide(control, "Init");

            var wake = control.GetState("Wake");
            wake.ChangeTransition("FINISHED", "Idle");
            wake.DisableAction(1);
            wake.DisableAction(2);
            wake.DisableAction(3);

            var roarStart = control.GetState("Roar Start");
            roarStart.DisableAction(2);//disable roar sound
            var roar = control.GetState("Roar");//make the roar emit no waves and be silent
            roar.DisableAction(2);
            roar.DisableAction(3);
            roar.DisableAction(4);
            roar.DisableAction(5);

            var roarEnd = control.GetState("Roar End");
            roarEnd.DisableAction(1);

            if(!other.IsBoss)
                roarEnd.GetFirstActionOfType<SetDamageHeroAmount>().damageDealt = 1;
        }

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return previousHP * 2;
        }
    }

    public class ZombieBeamMinerRematchSpawner : DefaultSpawner<ZombieBeamMinerRematchControl> { }

    public class ZombieBeamMinerRematchPrefabConfig : DefaultPrefabConfig<ZombieBeamMinerRematchControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HiveKnightControl : DefaultSpawnedEnemyControl { }

    public class HiveKnightSpawner : DefaultSpawner<HiveKnightControl> { }

    public class HiveKnightPrefabConfig : DefaultPrefabConfig<HiveKnightControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GrimmBossControl : DefaultSpawnedEnemyControl { }

    public class GrimmBossSpawner : DefaultSpawner<GrimmBossControl> { }

    public class GrimmBossPrefabConfig : DefaultPrefabConfig<GrimmBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class NightmareGrimmBossControl : DefaultSpawnedEnemyControl { }

    public class NightmareGrimmBossSpawner : DefaultSpawner<NightmareGrimmBossControl> { }

    public class NightmareGrimmBossPrefabConfig : DefaultPrefabConfig<NightmareGrimmBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DreamMageLordControl : DefaultSpawnedEnemyControl { }

    public class DreamMageLordSpawner : DefaultSpawner<DreamMageLordControl> { }

    public class DreamMageLordPrefabConfig : DefaultPrefabConfig<DreamMageLordControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DreamMageLordPhase2Control : DefaultSpawnedEnemyControl { }

    public class DreamMageLordPhase2Spawner : DefaultSpawner<DreamMageLordPhase2Control> { }

    public class DreamMageLordPhase2PrefabConfig : DefaultPrefabConfig<DreamMageLordPhase2Control> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HollowKnightBossControl : DefaultSpawnedEnemyControl { }

    public class HollowKnightBossSpawner : DefaultSpawner<HollowKnightBossControl> { }

    public class HollowKnightBossPrefabConfig : DefaultPrefabConfig<HollowKnightBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HKPrimeControl : DefaultSpawnedEnemyControl { }

    public class HKPrimeSpawner : DefaultSpawner<HKPrimeControl> { }

    public class HKPrimePrefabConfig : DefaultPrefabConfig<HKPrimeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class PaleLurkerControl : DefaultSpawnedEnemyControl { }

    public class PaleLurkerSpawner : DefaultSpawner<PaleLurkerControl> { }

    public class PaleLurkerPrefabConfig : DefaultPrefabConfig<PaleLurkerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class OroControl : DefaultSpawnedEnemyControl { }

    public class OroSpawner : DefaultSpawner<OroControl> { }

    public class OroPrefabConfig : DefaultPrefabConfig<OroControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MatoControl : DefaultSpawnedEnemyControl { }

    public class MatoSpawner : DefaultSpawner<MatoControl> { }

    public class MatoPrefabConfig : DefaultPrefabConfig<MatoControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SheoBossControl : DefaultSpawnedEnemyControl { }

    public class SheoBossSpawner : DefaultSpawner<SheoBossControl> { }

    public class SheoBossPrefabConfig : DefaultPrefabConfig<SheoBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class FatFlukeControl : DefaultSpawnedEnemyControl { }

    public class FatFlukeSpawner : DefaultSpawner<FatFlukeControl> { }

    public class FatFlukePrefabConfig : DefaultPrefabConfig<FatFlukeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SlyBossControl : DefaultSpawnedEnemyControl { }

    public class SlyBossSpawner : DefaultSpawner<SlyBossControl> { }

    public class SlyBossPrefabConfig : DefaultPrefabConfig<SlyBossControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class OrdealZotelingControl : DefaultSpawnedEnemyControl { }

    public class OrdealZotelingSpawner : DefaultSpawner<OrdealZotelingControl> { }

    public class OrdealZotelingPrefabConfig : DefaultPrefabConfig<OrdealZotelingControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HornetNoskControl : DefaultSpawnedEnemyControl { }

    public class HornetNoskSpawner : DefaultSpawner<HornetNoskControl> { }

    public class HornetNoskPrefabConfig : DefaultPrefabConfig<HornetNoskControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class Colosseum_WormControl : DefaultSpawnedEnemyControl { }

    public class Colosseum_WormSpawner : DefaultSpawner<Colosseum_WormControl> { }

    public class Colosseum_WormPrefabConfig : DefaultPrefabConfig<Colosseum_WormControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class DungDefenderControl : DefaultSpawnedEnemyControl { }

    public class DungDefenderSpawner : DefaultSpawner<DungDefenderControl> { }

    public class DungDefenderPrefabConfig : DefaultPrefabConfig<DungDefenderControl> { }
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
    public class GhostWarriorGalienControl : DefaultSpawnedEnemyControl { }

    public class GhostWarriorGalienSpawner : DefaultSpawner<GhostWarriorGalienControl> { }

    public class GhostWarriorGalienPrefabConfig : DefaultPrefabConfig<GhostWarriorGalienControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorXeroControl : DefaultSpawnedEnemyControl { }

    public class GhostWarriorXeroSpawner : DefaultSpawner<GhostWarriorXeroControl> { }

    public class GhostWarriorXeroPrefabConfig : DefaultPrefabConfig<GhostWarriorXeroControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorHuControl : DefaultSpawnedEnemyControl { }

    public class GhostWarriorHuSpawner : DefaultSpawner<GhostWarriorHuControl> { }

    public class GhostWarriorHuPrefabConfig : DefaultPrefabConfig<GhostWarriorHuControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorSlugControl : DefaultSpawnedEnemyControl { }

    public class GhostWarriorSlugSpawner : DefaultSpawner<GhostWarriorSlugControl> { }

    public class GhostWarriorSlugPrefabConfig : DefaultPrefabConfig<GhostWarriorSlugControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorNoEyesControl : DefaultSpawnedEnemyControl { }

    public class GhostWarriorNoEyesSpawner : DefaultSpawner<GhostWarriorNoEyesControl> { }

    public class GhostWarriorNoEyesPrefabConfig : DefaultPrefabConfig<GhostWarriorNoEyesControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GhostWarriorMarkothControl : DefaultSpawnedEnemyControl { }

    public class GhostWarriorMarkothSpawner : DefaultSpawner<GhostWarriorMarkothControl> { }

    public class GhostWarriorMarkothPrefabConfig : DefaultPrefabConfig<GhostWarriorMarkothControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MegaJellyfishControl : DefaultSpawnedEnemyControl { }

    public class MegaJellyfishSpawner : DefaultSpawner<MegaJellyfishControl> { }

    public class MegaJellyfishPrefabConfig : DefaultPrefabConfig<MegaJellyfishControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class JellyfishGGControl : DefaultSpawnedEnemyControl { }

    public class JellyfishGGSpawner : DefaultSpawner<JellyfishGGControl> { }

    public class JellyfishGGPrefabConfig : DefaultPrefabConfig<JellyfishGGControl> { }
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
    public class EggSacControl : DefaultSpawnedEnemyControl { }

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
}
