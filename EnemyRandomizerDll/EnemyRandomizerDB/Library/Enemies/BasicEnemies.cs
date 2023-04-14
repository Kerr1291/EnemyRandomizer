﻿using HutongGames.PlayMaker.Actions;
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
    public class HopperControl : DefaultSpawnedEnemyControl
    {
        //protected virtual void OnEnable()
        //{
        //    gameObject.StickToGround();
        //}
    }

    public class HopperSpawner : DefaultSpawner<HopperControl> { }

    public class HopperPrefabConfig : DefaultPrefabConfig<HopperControl> { }
                                                                             /////
    //////////////////////////////////////////////////////////////////////////////
    
    


    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantHopperControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

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
    
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
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
    
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MageKnightSpawner : DefaultSpawner<MageKnightControl> { }

    public class MageKnightPrefabConfig : DefaultPrefabConfig<MageKnightControl> { }
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
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class LancerSpawner : DefaultSpawner<LancerControl> { }

    public class LancerPrefabConfig : DefaultPrefabConfig<LancerControl> { }
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
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MenderBugSpawner : DefaultSpawner<MenderBugControl> { }

    public class MenderBugPrefabConfig : DefaultPrefabConfig<MenderBugControl> { }
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
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class BlockerSpawner : DefaultSpawner<BlockerControl> { }

    public class BlockerPrefabConfig : DefaultPrefabConfig<BlockerControl> { }
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
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
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
                gameObject.StickToClosestSurface(200f, false);
            }
            else
            {
                gameObject.StickToRoof();
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

        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class BlackKnightSpawner : DefaultSpawner<BlackKnightControl> { }

    public class BlackKnightPrefabConfig : DefaultPrefabConfig<BlackKnightControl> { }

    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MossWalkerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
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
            gameObject.StickToClosestSurface(200f, -.5f, true, false);
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
    public class FungoonBabyControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class FungoonBabySpawner : DefaultSpawner<FungoonBabyControl> { }

    public class FungoonBabyPrefabConfig : DefaultPrefabConfig<FungoonBabyControl> { }
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
    public class MushroomBrawlerControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class MushroomBrawlerSpawner : DefaultSpawner<MushroomBrawlerControl> { }

    public class MushroomBrawlerPrefabConfig : DefaultPrefabConfig<MushroomBrawlerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MushroomBabyControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
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
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
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
            gameObject.StickToGround();
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
    /////  TODO: fix like hatcher
    public class ZombieHiveControl : DefaultSpawnedEnemyControl
    {
        protected virtual void OnEnable()
        {
            gameObject.StickToGround();
        }
    }

    public class ZombieHiveSpawner : DefaultSpawner<ZombieHiveControl> { }

    public class ZombieHivePrefabConfig : DefaultPrefabConfig<ZombieHiveControl> { }
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
    public class OrdealZotelingControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public float startYPos;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body != null)
                body.isKinematic = false;

            RNG geoRNG = new RNG();
            geoRNG.Reset();

            thisMetadata.EnemyHealthManager.hp = other.MaxHP;
            thisMetadata.EnemyHealthManager.SetGeoMedium(geoRNG.Rand(0, 5));
            thisMetadata.EnemyHealthManager.SetGeoSmall(geoRNG.Rand(1, 10));

            var init = control.GetState("Init");
            init.DisableAction(0);
            init.DisableAction(1);

            var ball = control.GetState("Ball");
            ball.DisableAction(1);
            control.FsmVariables.GetFsmFloat("X Pos").Value = pos2d.x;
            ball.GetAction<SetPosition>(2).y.Value = pos2d.y;
            ball.DisableAction(15);

            var dr = control.GetState("Dir");
            dr.DisableAction(0);
            dr.DisableAction(1);
            dr.AddCustomAction(() => {

                RNG rng = new RNG();
                rng.Reset();

                bool left = rng.Randf() > .5f;
                if (left)
                    control.SendEvent("L");
                else
                    control.SendEvent("R");
            });

            var endState = control.AddState("DestroyGO");
            endState.AddCustomAction(() => { Destroy(gameObject); });

            //var reset = control.GetState("Reset");
            //reset.RemoveTransition("FINISHED");

            var death = control.GetState("Die");
            death.DisableAction(0);
            death.DisableAction(1);
            death.DisableAction(3);
            death.AddCustomAction(() => { GameObject.Destroy(gameObject); });

            this.OverrideState(control, "Reset", () => { GameObject.Destroy(gameObject); });
            this.OverrideState(control, "Respawn Pause", () => { GameObject.Destroy(gameObject); });

            this.InsertHiddenState(control, "Init", "FINISHED", "Ball");
            this.AddResetToStateOnHide(control, "Init");

            CustomFloatRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                //{"Shockwave Y" , x => floorY},
            };
        }

        protected override bool HeroInAggroRange()
        {
            return (heroPos2d - pos2d).magnitude < 50f;
        }
    }

    public class OrdealZotelingSpawner : DefaultSpawner<OrdealZotelingControl> { }

    public class OrdealZotelingPrefabConfig : DefaultPrefabConfig<OrdealZotelingControl> { }
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

    public class ZombieSpider1Control : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }
    }

    public class ZombieSpider1Spawner : DefaultSpawner<ZombieSpider1Control>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZombieSpider1Control>();
            fsm.control = go.LocateMyFSM("Chase");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                //var hm = go.GetComponent<HealthManager>();
                //hm.hp = source.MaxHP;
            }

            return go;
        }
    }

    public class ZombieSpider1PrefabConfig : DefaultPrefabConfig<ZombieSpider1Control>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Chase");

                //change the start transition to just begin the spawn antics
                fsm.ChangeTransition("Check Battle", "BATTLE", "Wait 2");

                fsm.RemoveTransition("Battle Inert", "BATTLE START");
                //fsm.Fsm.RemoveState("Battle Inert");
            }
        }
    }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///









    /////////////////////////////////////////////////////////////////////////////
    /////   




    public class ZombieSpider2Control : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM control;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
        }
    }

    public class ZombieSpider2Spawner : DefaultSpawner<ZombieSpider2Control>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            var go = base.Spawn(p, source);
            var fsm = go.GetComponent<ZombieSpider2Control>();
            fsm.control = go.LocateMyFSM("Chase");

            if (source.IsBoss)
            {
                //TODO:
            }
            else
            {
                //var hm = go.GetComponent<HealthManager>();
                //hm.hp = source.MaxHP;
            }

            return go;
        }
    }
    public class ZombieSpider2PrefabConfig : DefaultPrefabConfig<ZombieSpider2Control>
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            {
                var fsm = p.prefab.LocateMyFSM("Chase");

                //change the start transition to just begin the spawn antics
                fsm.ChangeTransition("Check Battle", "BATTLE", "Wait 2");

                fsm.RemoveTransition("Battle Inert", "BATTLE START");
                //fsm.Fsm.RemoveState("Battle Inert");
            }
        }
    }


    /////
    //////////////////////////////////////////////////////////////////////////////






    /////////////////////////////////////////////////////////////////////////////
    /////


    public class MossChargerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mossy Control";

        public override string FSMHiddenStateName => "MOSS_HIDDEN";

        public bool didInit = false;
        public bool isVisible = false;
        public bool isRunning = false;

        protected Dictionary<string, Func<FSMAreaControlEnemy, float>> MossyFloatsRefs;

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => MossyFloatsRefs;

        public float edgeL;
        public float edgeR;
        public float aggroGroundYSize;
        public float aggroGroundXSize;
        public Vector2 sizeOfAggroArea;
        public Vector2 centerOfAggroArea;
        public UnityEngine.Bounds aggroBounds;

        protected override bool HeroInAggroRange()
        {
            if (aggroBounds == null)
                return false;

            return aggroBounds.Contains(HeroController.instance.transform.position);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var down = gameObject.transform.position.Fire2DRayGlobal(Vector2.down, 500f);
            var floor = down.collider;
            var floorsize = floor.bounds.size;

            var left = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 500f);
            var right = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 500f);

            var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

            var wallL = (left.point - pos2d).magnitude;
            var wallR = (right.point - pos2d).magnitude;

            edgeL = wallL < floorsize.x / 2f ? wallL : floorsize.x / 2f;
            edgeR = wallR < floorsize.x / 2f ? wallR : floorsize.x / 2f;

            //is it too big for the platform? then shrink it
            if (edgeR - edgeL < thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale)
            {
                float ratio = (edgeR - edgeL) / (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale);
                thisMetadata.ApplySizeScale(ratio * .5f);
            }

            //for the aggro range, allow the full y size without scaling
            aggroGroundYSize = thisMetadata.ObjectSize.y;
            aggroGroundXSize = edgeR - edgeL;
            centerOfAggroArea = new Vector2(edgeL + edgeR / 2f, down.point.y + aggroGroundYSize / 2f);
            sizeOfAggroArea = new Vector2(aggroGroundXSize, aggroGroundYSize);

            aggroBounds = new UnityEngine.Bounds(centerOfAggroArea, sizeOfAggroArea);

            MossyFloatsRefs = new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                {"X Max" , x => edgeR},
                {"X Min" , x => edgeL},
                {"Start Y" , x => (HeroController.instance.transform.position + new Vector3(0f, 1f, 0f)).Fire2DRayGlobal(Vector2.down, 50f).point.y},
                {"Start X" , x => centerOfAggroArea.x },
            };

            //just skip this state
            var hs = control.GetState("Hidden");
            hs.DisableAction(1);
            hs.DisableAction(2);
            hs.AddCustomAction(() =>
            {
                control.SendEvent("IN RANGE");
            });

            //use our custom hidden aggro range logic
            this.InsertHiddenState(control, "Init", "FINISHED", "Hidden");
            control.GetState(FSMHiddenStateName).InsertCustomAction(() =>
            {
                //reset our flags at the start of being outside aggro range
                isVisible = false;
                isRunning = false;
            }, 0);

            //change the manual reset states to point to our custom hidden state
            control.ChangeTransition("Play Range", "FINISHED", FSMHiddenStateName);
            control.ChangeTransition("Hero Beyond?", "CANCEL", FSMHiddenStateName);

            DisableSendEvents(control,
                ("Emerge", 1)
                );

            var eRight = control.GetState("Emerge Right");
            var eLeft = control.GetState("Emerge Left");

            //configure emerge right
            {
                eRight.DisableAction(0);
                eRight.DisableAction(1);
                eRight.DisableAction(3);
                eRight.DisableAction(4);
                eRight.DisableAction(5);
                eRight.DisableAction(9);

                eRight.AddCustomAction(() =>
                {
                    //too close to edge? do emerge left
                    float dist = edgeR - HeroX;
                    if (dist < thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale)
                    {
                        control.SendEvent("LEFT");
                        return;
                    }

                    //pick a random spot between hero and edge
                    RNG rng = new RNG();
                    rng.Reset();
                    float spawnX = rng.Rand(HeroX, edgeR - (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale));

                    control.FsmVariables.GetFsmFloat("Appear X").Value = spawnX;

                    control.FsmVariables.GetFsmVector2("RayForward Direction").Value = new Vector2(-1f, 0f);
                    control.FsmVariables.GetFsmVector2("RayDown X").Value = new Vector2(-6.5f, -.5f);

                    control.SendEvent("FINISHED");
                });
            }

            //configure emerge left
            {
                eLeft.DisableAction(1);
                eLeft.DisableAction(2);
                eLeft.DisableAction(3);
                eLeft.DisableAction(4);
                eLeft.DisableAction(5);
                eLeft.DisableAction(8);

                eLeft.AddCustomAction(() =>
                {
                    //too close to edge? do emerge left
                    float dist = HeroX - edgeL;
                    if (dist < thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale)
                    {
                        control.SendEvent("RIGHT");
                        return;
                    }

                    //pick a random spot between hero and edge
                    RNG rng = new RNG();
                    rng.Reset();
                    float spawnX = rng.Rand(edgeL + (thisMetadata.ObjectSize.x * this.thisMetadata.SizeScale), HeroX);

                    control.FsmVariables.GetFsmFloat("Appear X").Value = spawnX;

                    control.FsmVariables.GetFsmVector2("RayForward Direction").Value = new Vector2(1f, 0f);
                    control.FsmVariables.GetFsmVector2("RayDown X").Value = new Vector2(6.5f, -.5f);

                    control.SendEvent("FINISHED");
                });
            }

            //we calculated the emerge position using world space, so make sure the action uses world space
            control.GetState("Emerge").GetAction<SetPosition>(5).space = Space.World;
            control.AddState("Emerge").InsertCustomAction(() => isVisible = true, 0);

            //mark this flag for our custom hide state
            control.GetState("Init").AddCustomAction(() => didInit = true);

            //add this state to choose the correct "hiding" animation
            var customHide = control.AddState("CUSTOM_HIDE");

            customHide.AddTransition("NEEDS_INIT", "Init");
            customHide.AddTransition("DO_INVIS_HIDE", FSMHiddenStateName);
            customHide.AddTransition("DO_SUBMERGE", "Submerge");
            customHide.AddTransition("DO_DIG", "Dig Start");

            customHide.AddCustomAction(() =>
            {
                if (!didInit)
                {
                    control.SendEvent("NEEDS_INIT");
                    return;
                }
                else if (isVisible && !isRunning)
                {
                    control.SendEvent("DO_SUBMERGE");
                    return;
                }
                else if (isVisible && isRunning)
                {
                    control.SendEvent("DO_DIG");
                    return;
                }
                else
                {
                    control.SendEvent("DO_INVIS_HIDE");
                }
            });

            this.AddResetToStateOnHide(control, "CUSTOM_HIDE");

            control.GetState("Submerge CD").GetAction<SetPosition>(6).space = Space.World;
        }

        protected override void UpdateCustomRefs()
        {
            base.UpdateCustomRefs();
            UpdateRefs(control, MossyFloatsRefs);
        }
    }


    public class MossChargerSpawner : DefaultSpawner<MossChargerControl> { }

    public class MossChargerPrefabConfig : DefaultPrefabConfig<MossChargerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////










    /////////////////////////////////////////////////////////////////////////////
    /////   



    public class MawlekTurretControl : DefaultSpawnedEnemyControl
    {
        Range upL = new Range(95f, 110f);
        Range upR = new Range(70f, 85f);

        Range leL = new Range(185f, 200f);
        Range leR = new Range(160f, 175f);

        Range dnL = new Range(250f, 265f);
        Range dnR = new Range(275f, 290f);

        Range riL = new Range(5f, 20f);
        Range riR = new Range(340f, 355f);

        //TODO: implement modified shot speed
        //public float shotSpeed = 10f;
        public float spawnDist = 1.4f;

        public bool isFloorTurret = false;

        void Start()
        {
            gameObject.StickToClosestSurface();

            var fsm = gameObject.LocateMyFSM("Mawlek Turret");

            var spawnPos = fsm.FsmVariables.FindFsmVector3("Spawn Pos");

            var left = fsm.GetState("Fire Left");
            var right = fsm.GetState("Fire Right");

            var leftMin = fsm.FsmVariables.FindFsmFloat("Angle Min L");
            var leftMax = fsm.FsmVariables.FindFsmFloat("Angle Max L");
            var rightMin = fsm.FsmVariables.FindFsmFloat("Angle Min R");
            var rightMax = fsm.FsmVariables.FindFsmFloat("Angle Max R");


            Vector2 up = Vector2.zero;

            float angle = transform.localEulerAngles.z % 360f;
            if (!isFloorTurret)
            {
                angle = (angle + 180f) % 360f;
            }

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
                leftMin.Value = upL.Min;
                leftMax.Value = upL.Max;

                rightMin.Value = upR.Min;
                rightMax.Value = upR.Max;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
                leftMin.Value = leL.Min;
                leftMax.Value = leL.Max;

                rightMin.Value = leR.Min;
                rightMax.Value = leR.Max;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
                leftMin.Value = dnL.Min;
                leftMax.Value = dnL.Max;

                rightMin.Value = dnL.Min;
                rightMax.Value = dnL.Max;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
                leftMin.Value = riL.Min;
                leftMax.Value = riL.Max;

                rightMin.Value = riR.Min;
                rightMax.Value = riR.Max;
            }

            spawnPos.Value = up * spawnDist * transform.localScale.y;
        }
    }

    public class MawlekTurretPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<MawlekTurretControl>();
            control.isFloorTurret = true;
        }
    }

    //TODO: remove ceiling version entirely
    public class MawlekTurretCeilingPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            //overwrite the upside-down one since the code now works just fine for the upright one in all cases
            var uprightTurret = p.source.Scene.sceneObjects.FirstOrDefault(x => x.LoadedObject.prefabName == "Mawlek Turret");
            p.prefab = uprightTurret.LoadedObject.prefab;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<MawlekTurretControl>();
            control.isFloorTurret = true;
        }
    }



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
