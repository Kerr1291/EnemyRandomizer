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
    public class WhitePalaceFlyControl : DefaultSpawnedEnemyControl
    {
        public override string spawnEntityOnDeath => isZapFly ? null : customOnDeathEffect;
        public string customOnDeathEffect = "Electro Zap";

        public int chanceToBeZapFly = 1;
        public int chanceToBeMaxZapFly = 10;
        public bool isZapFly;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            isZapFly = SpawnerExtensions.RollProbability(out _, chanceToBeZapFly, chanceToBeMaxZapFly);

            if(isZapFly)
            {
                SpawnerExtensions.AddParticleEffect_WhiteSoulEmissions(gameObject, Color.cyan);
            }

            CurrentHP = 1;
        }

        /// <summary>
        /// This needs to be set each frame to make the palace fly killable
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (!loaded)
                return;

            if (CurrentHP > 1)
                CurrentHP = 1;
        }
    }

    public class WhitePalaceFlySpawner : DefaultSpawner<WhitePalaceFlyControl> { }

    public class WhitePalaceFlyPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HatcherControl : DefaultSpawnedEnemyControl
    {
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;

        public override string FSMName => "Hatcher";

        public GameObject lastSpawnedObject;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            isVanilla = gameObject == other;

            ChildController.maxChildren = isVanilla ? vanillaMaxBabies : 2;

            var init = control.GetState("Initiate");
            init.DisableAction(2);

            var hatchedMaxCheck = control.GetState("Hatched Max Check");
            hatchedMaxCheck.DisableAction(1);
            hatchedMaxCheck.InsertCustomAction(() => {
                control.FsmVariables.GetFsmInt("Cage Children").Value = ChildController.RemainingSpawns; }, 0);

            var fire = control.GetState("Fire");
            fire.DisableAction(1);
            fire.DisableAction(2);
            fire.DisableAction(6);
            fire.DisableAction(11);
            fire.InsertCustomAction(() => {
                lastSpawnedObject = ChildController.ActivateAndTrackSpawnedObject(lastSpawnedObject);
            }, 6);
            fire.InsertCustomAction(() => {
                lastSpawnedObject = SpawnChildForEnemySpawner(transform.position, false, "Fly", "Shot");
            }, 0);
            fire.AddCustomAction(() => { control.SendEvent("WAIT"); });
        }
    }

    public class HatcherSpawner : DefaultSpawner<HatcherControl> { }

    public class HatcherPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class CentipedeHatcherControl : DefaultSpawnedEnemyControl
    {
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;

        public override string FSMName => "Centipede Hatcher";

        public PlayMakerFSM FSM { get; set; }

        public GameObject lastSpawnedObject;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            isVanilla = gameObject == other;

            ChildController.maxChildren = isVanilla ? vanillaMaxBabies : 3;

            var init = control.GetState("Init");
            init.DisableAction(1);
            //init.DisableAction(3);

            var checkRange = control.GetState("Check Range");
            control.OverrideState("Check Range", () => { 
            
                if(gameObject.CanSeePlayer())
                {
                    control.FsmVariables.GetFsmBool("In Attack Range").Value = true;
                    control.SendEvent("HATCH");
                }
                else
                {
                    control.FsmVariables.GetFsmBool("In Attack Range").Value = false;
                }            
            });
            checkRange.AddAction(new WaitRandom() { timeMin = 0.2f, timeMax = 1f });

            var getCentipede = control.GetState("Get Centipede");
            getCentipede.DisableAction(0);
            getCentipede.DisableAction(1);
            getCentipede.DisableAction(2);
            getCentipede.DisableAction(3);
            getCentipede.AddCustomAction(() => {

                if (ChildController.AtMaxChildren)
                    control.SendEvent("EXHAUSTED");
                else
                {
                    lastSpawnedObject = SpawnChildForEnemySpawner(transform.position, false, "Fly", "Hatchling");
                    control.SendEvent("FINISHED");
                }
            });

            var birth = control.GetState("Birth");
            birth.DisableAction(1);
            birth.DisableAction(7);
            birth.DisableAction(15);//disable the reversing the scale
            birth.InsertCustomAction(() => {
                if (lastSpawnedObject != null)
                {
                    lastSpawnedObject = ChildController.ActivateAndTrackSpawnedObject(lastSpawnedObject);
                    control.FsmVariables.GetFsmGameObject("Hatchling").Value = lastSpawnedObject;
                }
            }, 6);

            var checkBirths = control.GetState("Check Births");
            checkBirths.DisableAction(0);
            checkBirths.InsertCustomAction(() => {
                if (ChildController.AtMaxChildren)
                    control.SendEvent("HATCHED MAX");
            }, 0);
        }
    }

    public class CentipedeHatcherSpawner : DefaultSpawner<CentipedeHatcherControl> { }

    public class CentipedeHatcherPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Mage";

        protected override bool DisableCameraLocks => false;

        public override bool preventOutOfBoundsAfterPositioning => true;
        public override bool preventInsideWallsAfterPositioning => false;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var wake = control.GetState("Wake");
            wake.DisableActions(1, 2, 3);
            wake.InsertCustomAction(() => {
                var pos = gameObject.GetRandomPositionInLOSofPlayer(5f, 30f, 2f, 5f);
                transform.position = pos;
            }, 0);

            var st = control.GetState("Select Target");
            //disable the teleplane actions
            control.OverrideState("Select Target", () =>
            {
                var pos = gameObject.GetRandomPositionInLOSofPlayer(5f, 30f, 2f, 5f);
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = new Vector3(pos.x, pos.y, 0.006f);
                control.SendEvent("TELEPORT");
            });

            control.ChangeTransition("Tele Away", "FINISHED", "Init");

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
        }
    }

    public class MageSpawner : DefaultSpawner<MageControl> { }

    public class MagePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ElectricMageControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Electric Mage";

        public override float aggroRange => 20f;

        protected override bool DisableCameraLocks => false;

        public override bool preventOutOfBoundsAfterPositioning => true;
        public override bool preventInsideWallsAfterPositioning => false;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var wake = control.GetState("Wake");
            wake.DisableActions(0, 1, 2);
            wake.InsertCustomAction(() => {
                var pos = gameObject.GetRandomPositionInLOSofPlayer(5f, 30f, 2f, 5f);
                transform.position = pos;
            }, 0);

            var st = control.GetState("Select Target");
            //disable the teleplane actions
            control.OverrideState("Select Target", () =>
            {
                var pos = gameObject.GetRandomPositionInLOSofPlayer(5f, 30f, 2f, 5f);
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = new Vector3(pos.x, pos.y, 0.006f);
                control.SendEvent("TELEPORT");
            });


            var gen = control.GetState("Gen");
            gen.InsertCustomAction(() => {
                if (!HeroInAggroRange())
                    control.SendEvent("END");
            }, 0);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
        }

        protected override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            var result = base.GetStartingMaxHP(objectThatWillBeReplaced);

            //nerf their hp more
            result = result / 2;
            if (result <= 0)
                result = 1;

            return result;
        }
    }

    public class ElectricMageSpawner : DefaultSpawner<ElectricMageControl> { }

    public class ElectricMagePrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////




}
