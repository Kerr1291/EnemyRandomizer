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
    public class WhitePalaceFlyControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.EnemyHealthManager.hp = 1;
        }

        /// <summary>
        /// This needs to be set each frame to make the palace fly killable
        /// </summary>
        protected virtual void Update()
        {
            if (thisMetadata == null)
                return;

            if (thisMetadata.EnemyHealthManager.hp > 1)
                thisMetadata.EnemyHealthManager.hp = 1;
        }
    }

    public class WhitePalaceFlySpawner : DefaultSpawner<WhitePalaceFlyControl> { }

    public class WhitePalaceFlyPrefabConfig : DefaultPrefabConfig<WhitePalaceFlyControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////




    /////////////////////////////////////////////////////////////////////////////
    /////
    public class HatcherControl : DefaultSpawnedEnemyControl
    {
        public int maxBabies = 3;
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;
        public bool dieChildrenOnDeath = true;

        public PlayMakerFSM FSM { get; set; }

        public List<GameObject> children = new List<GameObject>();

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (other == null)
            {
                isVanilla = true;
            }
            else
            {
                isVanilla = thisMetadata.Source == other.Source;
            }

            if (isVanilla)
                maxBabies = vanillaMaxBabies;

            FSM = gameObject.LocateMyFSM("Hatcher");

            var init = FSM.GetState("Initiate");
            init.DisableAction(2);

            var hatchedMaxCheck = FSM.GetState("Hatched Max Check");
            hatchedMaxCheck.DisableAction(1);
            hatchedMaxCheck.InsertCustomAction(() => { FSM.FsmVariables.GetFsmInt("Cage Children").Value = maxBabies - children.Count; }, 0);

            var fire = FSM.GetState("Fire");
            fire.DisableAction(1);
            fire.DisableAction(2);
            fire.DisableAction(6);
            fire.DisableAction(11);
            fire.InsertCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Shot").Value.SafeSetActive(true);
            }, 6);
            fire.InsertCustomAction(() => {
                var child = EnemyRandomizerDatabase.GetDatabase().Spawn("Fly", null);
                children.Add(child);
                FSM.FsmVariables.GetFsmGameObject("Shot").Value = child;
            }, 0);
            fire.AddCustomAction(() => { FSM.SendEvent("WAIT"); });
        }

        protected override void OnDestroy()
        {
            if (dieChildrenOnDeath)
            {
                children.ForEach(x =>
                {
                    if (x == null)
                        return;

                    var hm = x.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        hm.Die(null, AttackTypes.Generic, true);
                    }
                });
            }
        }

        protected override void Update()
        {
            base.Update();

            if (children == null)
                return;

            for (int i = 0; i < children.Count;)
            {
                if (i >= children.Count)
                    break;

                if (children[i] == null)
                {
                    children.RemoveAt(i);
                    continue;
                }
                else
                {
                    var hm = children[i].GetComponent<HealthManager>();
                    if (hm.hp <= 0 || hm.isDead)
                    {
                        children.RemoveAt(i);
                        continue;
                    }
                }

                ++i;
            }
        }
    }



    public class HatcherSpawner : DefaultSpawner<HatcherControl> { }

    public class HatcherPrefabConfig : DefaultPrefabConfig<HatcherControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class CentipedeHatcherControl : DefaultSpawnedEnemyControl
    {
        public int maxBabies = 3;
        public int vanillaMaxBabies = 5;

        //if true, will set the max babies to 5
        public bool isVanilla;
        public bool dieChildrenOnDeath = true;

        public PlayMakerFSM FSM { get; set; }

        public List<GameObject> children = new List<GameObject>();

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            if (other == null)
            {
                isVanilla = true;
            }
            else
            {
                isVanilla = thisMetadata.Source == other.Source;
            }

            if (isVanilla)
                maxBabies = vanillaMaxBabies;

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
                if(children.Count >= maxBabies)
                    FSM.SendEvent("EXHAUSTED"); 
            });
            getCentipede.AddCustomAction(() => {
                var child = EnemyRandomizerDatabase.GetDatabase().Spawn("Baby Centipede", null);
                children.Add(child);
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
            birth.InsertCustomAction(() => {
                FSM.FsmVariables.GetFsmGameObject("Hatchling").Value.SafeSetActive(true);
            }, 6);

            var checkBirths = FSM.GetState("Check Births");
            birth.DisableAction(0);
            checkBirths.InsertCustomAction(() => {
                if (children.Count >= maxBabies)
                    FSM.SendEvent("HATCHED MAX");
            }, 0);
        }


        protected override void OnDestroy()
        {
            if (dieChildrenOnDeath)
            {
                children.ForEach(x =>
                {
                    if (x == null)
                        return;

                    var hm = x.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        hm.Die(null, AttackTypes.Generic, true);
                    }
                });
            }
        }

        protected virtual void Update()
        {
            if (children == null)
                return;

            for (int i = 0; i < children.Count;)
            {
                if (i >= children.Count)
                    break;

                if (children[i] == null)
                {
                    children.RemoveAt(i);
                    continue;
                }
                else
                {
                    var hm = children[i].GetComponent<HealthManager>();
                    if (hm.hp <= 0 || hm.isDead)
                    {
                        children.RemoveAt(i);
                        continue;
                    }
                }

                ++i;
            }
        }
    }

    public class CentipedeHatcherSpawner : DefaultSpawner<CentipedeHatcherControl> { }

    public class CentipedeHatcherPrefabConfig : DefaultPrefabConfig<CentipedeHatcherControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class MageControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Mage";

        protected override bool ControlCameraLocks => false;

        protected override Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> FloatRefs =>
            new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>()
            {
                    { "X Max", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.right, 500f).point.x; } },
                    { "X Min", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.left, 500f).point.x; } },
                    { "Y Max", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.up, 500f).point.y; } },
                    { "Y Min", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.down, 500f).point.y; } },
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

        public float hpRatioTriggerNerf = 2f;
        public float aggroRadius = 30f;

        protected override bool ControlCameraLocks => false;

        protected override Dictionary<string, Func<DefaultSpawnedEnemyControl, float>> FloatRefs =>
            new Dictionary<string, Func<DefaultSpawnedEnemyControl, float>>()
            {
                    { "X Max", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.right, 500f).point.x; } },
                    { "X Min", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.left, 500f).point.x; } },
                    { "Y Max", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.up, 500f).point.y; } },
                    { "Y Min", x => { return x.heroPosWithOffset.FireRayGlobal(Vector2.down, 500f).point.y; } },
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

        protected override bool HeroInAggroRange()
        {
            float dist = (transform.position - HeroController.instance.transform.position).magnitude;
            return (dist <= aggroRadius);
        }

        protected override void ScaleHP()
        {
            if (originialMetadata == null)
                return;

            float mageHP = thisMetadata.MaxHP;
            float originalHP = originialMetadata.MaxHP;

            float ratio = mageHP / originalHP;

            if (ratio > hpRatioTriggerNerf)
            {
                thisMetadata.EnemyHealthManager.hp = Mathf.FloorToInt(mageHP / 2f);
            }
        }
    }

    public class ElectricMageSpawner : DefaultSpawner<ElectricMageControl> { }

    public class ElectricMagePrefabConfig : DefaultPrefabConfig<ElectricMageControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    ///// (Oblobbles)
    public class MegaFatBeeControl : DefaultSpawnedEnemyControl
    {
        public PlayMakerFSM FSM { get; set; }
        public PlayMakerFSM FSMattack { get; set; }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            FSM = gameObject.LocateMyFSM("fat fly bounce");
            FSMattack = gameObject.LocateMyFSM("Fatty Fly Attack");

            var init = FSM.GetState("Initialise");
            init.DisableAction(2);

            //skip the swoop in animation
            FSM.ChangeTransition("Initialise", "FINISHED", "Activate");
        }
    }

    public class MegaFatBeeSpawner : DefaultSpawner<MegaFatBeeControl> { }

    public class MegaFatBeePrefabConfig : DefaultPrefabConfig<MegaFatBeeControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////

}
