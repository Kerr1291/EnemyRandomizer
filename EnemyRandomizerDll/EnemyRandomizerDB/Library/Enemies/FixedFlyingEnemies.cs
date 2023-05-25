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
                Sprite.color = Color.cyan * .4f;
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

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if (other == null)
            {
                isVanilla = true;
            }
            else
            {
                isVanilla = gameObject == other;
            }

            ChildController.maxChildren = isVanilla ? vanillaMaxBabies : 3;

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
                ChildController.ActivateAndTrackSpawnedObject(control.FsmVariables.GetFsmGameObject("Shot").Value);
            }, 6);
            fire.InsertCustomAction(() => {
                var child = ChildController.SpawnAndTrackChild("Fly", transform.position, false);
                control.FsmVariables.GetFsmGameObject("Shot").Value = child;
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

        public override string FSMName => "Hatcher";

        public PlayMakerFSM FSM { get; set; }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            if (other == null)
            {
                isVanilla = true;
            }
            else
            {
                isVanilla = gameObject == other;
            }

            ChildController.maxChildren = isVanilla ? vanillaMaxBabies : 3;

            FSM = gameObject.LocateMyFSM("Centipede Hatcher");

            var init = FSM.GetState("Init");
            init.DisableAction(1);
            //init.DisableAction(3);

            var getCentipede = FSM.GetState("Get Centipede");
            getCentipede.DisableAction(0);
            getCentipede.DisableAction(1);
            getCentipede.DisableAction(2);
            getCentipede.DisableAction(3);
            getCentipede.AddCustomAction(() => { 
                if(ChildController.AtMaxChildren)
                    FSM.SendEvent("EXHAUSTED"); 
            });
            getCentipede.AddCustomAction(() => {
                var child = ChildController.SpawnAndTrackChild("Baby Centipede",transform.position,false);
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value = child;
            });
            getCentipede.AddCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.transform.SetParent(transform);
            });
            getCentipede.AddCustomAction(() => {
                    FSM.SendEvent("FINISHED");
            });

            var birth = FSM.GetState("Birth");
            birth.DisableAction(1);
            birth.DisableAction(7);
            birth.DisableAction(15);//disable the reversing the scale
            birth.InsertCustomAction(() => {
                ChildController.ActivateAndTrackSpawnedObject(FSM.FsmVariables.GetFsmGameObject("Hatchling").Value);
                //FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.SafeSetActive(true);
            }, 6);

            var checkBirths = FSM.GetState("Check Births");
            birth.DisableAction(0);
            checkBirths.InsertCustomAction(() => {
                if (ChildController.AtMaxChildren)
                    FSM.SendEvent("HATCHED MAX");
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

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var wake = control.GetState("Wake");
            wake.DisableActions(1, 2, 3);
            wake.InsertCustomAction(() => {
                var pos = gameObject.GetRandomPositionInLOSofSelf(5f, 30f, 1f, 1f);
                transform.position = pos;
            }, 0);

            var st = control.GetState("Select Target");
            //disable the teleplane actions
            control.OverrideState("Select Target", () =>
            {
                var pos = gameObject.GetRandomPositionInLOSofSelf(5f, 20f, 1f, 4f);
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = new Vector3(pos.x, pos.y, 0.006f);
                control.SendEvent("TELEPORT");
            });

            control.ChangeTransition("Tele Away", "FINISHED", "Init");

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            //this.AddResetToStateOnHide(control, "Init");
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

        public float hpRatioTriggerNerf = 2f;
        public override float aggroRange => 20f;

        protected override bool DisableCameraLocks => false;

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var wake = control.GetState("Wake");
            wake.DisableActions(0, 1, 2);
            wake.InsertCustomAction(() => {
                var pos = gameObject.GetRandomPositionInLOSofSelf(5f, 30f, 1f, 1f);
                transform.position = pos;
            }, 0);

            var st = control.GetState("Select Target");
            //disable the teleplane actions
            control.OverrideState("Select Target", () =>
            {
                var pos = gameObject.GetRandomPositionInLOSofSelf(5f, 20f, 1f, 4f);
                control.FsmVariables.GetFsmVector3("Teleport Point").Value = new Vector3(pos.x, pos.y, 0.006f);
                control.SendEvent("TELEPORT");
            });


            var gen = control.GetState("Gen");
            gen.InsertCustomAction(() => {
                if (!HeroInAggroRange())
                    control.SendEvent("END");
            }, 0);

            this.InsertHiddenState(control, "Init", "FINISHED", "Wake");
            //this.AddResetToStateOnHide(control, "Init");
        }

        protected override int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            var result = base.GetStartingMaxHP(objectThatWillBeReplaced);

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
