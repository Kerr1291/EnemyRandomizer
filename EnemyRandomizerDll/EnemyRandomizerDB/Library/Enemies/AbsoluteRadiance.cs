﻿using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EnemyRandomizerMod.Futils;
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{
    public class AbsoluteRadianceControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Control";

        public Rect bounds;

        public PlayMakerFSM attackCommands;
        public PlayMakerFSM teleport;

        protected override void SetupBossAsNormalEnemy()
        {
            base.SetupBossAsNormalEnemy();

            if (thisMetadata.SizeScale >= 1f)
            {
                thisMetadata.ApplySizeScale(thisMetadata.SizeScale * 0.25f);
            }

            if (teleport == null)
                teleport = gameObject.LocateMyFSM("Teleport");

            if (control == null)
                control = gameObject.LocateMyFSM("Control");

            if (attackCommands == null)
                attackCommands = gameObject.LocateMyFSM("Attack Commands");

            //disable a variety of camera shake actions

            try
            {
                //remove the big shake action
                control.GetState("First Tele").GetAction<SendEventByName>(3).sendEvent = string.Empty;
                control.GetState("First Tele").RemoveAction(3);
            }
            catch (Exception e) { Dev.Log("error in first tele"); }

            try
            {
                control.GetState("Rage1 Start").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
                control.GetState("Rage1 Start").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
            }
            catch (Exception e) { Dev.Log("error in Rage1"); }

            try
            {
                control.GetState("Stun1 Start").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
                control.GetState("Stun1 Start").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
            }
            catch (Exception e) { Dev.Log("error in Stun1"); }

            try
            {
                control.GetState("Tendrils1").GetFirstActionOfType<SetFsmBool>().variableName = string.Empty;
                control.GetState("Tendrils1").GetFirstActionOfType<SetFsmBool>().setValue.Clear();
            }
            catch (Exception e) { Dev.Log("error in Tendrils1"); }

            try
            {
                //clip off both camera shakes
                control.GetState("Stun1 Roar").Actions = control.GetState("Stun1 Roar").Actions.Take(control.GetState("Stun1 Roar").Actions.Length - 2).ToArray();
            }
            catch (Exception e) { Dev.Log("error in Stun1"); }

            try
            {
                control.GetState("Stun1 Out").GetAction<ActivateGameObject>(8).activate = false;
                control.GetState("Stun1 Out").GetAction<SendEventByName>(9).sendEvent = string.Empty;
            }
            catch (Exception e) { Dev.Log("error in Stun1 out"); }


            //reduce this non-boss radiance to spawn only 1 or 2 shots
            attackCommands.GetState("Orb Antic").GetFirstActionOfType<RandomInt>().min.Value = 1;
            attackCommands.GetState("Orb Antic").GetFirstActionOfType<RandomInt>().max.Value = 2;

            //disable enemy kill shake commands that make the camera shake
            attackCommands.GetState("EB 1").GetAction<SendEventByName>(3).sendEvent = string.Empty;
            attackCommands.GetState("EB 2").GetAction<SendEventByName>(4).sendEvent = string.Empty;
            attackCommands.GetState("EB 3").GetAction<SendEventByName>(4).sendEvent = string.Empty;
            attackCommands.GetState("EB 7").GetAction<SendEventByName>(3).sendEvent = string.Empty;
            attackCommands.GetState("EB 8").GetAction<SendEventByName>(3).sendEvent = string.Empty;
            attackCommands.GetState("EB 9").GetAction<SendEventByName>(3).sendEvent = string.Empty;
            attackCommands.GetState("Spawn Fireball").GetAction<SendEventByName>(0).sendEvent = string.Empty;
            attackCommands.GetState("Aim").GetAction<SendEventByName>(2).sendEvent = string.Empty;

            var orbPrefab = attackCommands.GetState("Spawn Fireball").GetAction<SpawnObjectFromGlobalPool>(1).gameObject.Value;
            orbPrefab.transform.localScale = orbPrefab.transform.localScale * 0.4f;

            if (orbPrefab.GetComponent<DamageHero>() != null)
                orbPrefab.GetComponent<DamageHero>().damageDealt = 1;

            //grab attacks
            List<GameObject> attacks = new List<GameObject>() {
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst1").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst2").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Burst3").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Eye Beam Glow").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Self").Value,
            attackCommands.FsmVariables.GetFsmGameObject("Shot Charge").Value,
            };

            //reduce damage and shrink attacks
            attacks.Select(x => x.GetComponent<DamageHero>()).Where(x => x != null)
                .ToList().ForEach(x => { x.damageDealt = 1; x.transform.localScale = new Vector3(.4f, .4f, 1f); });

            control.ChangeTransition("Intro End", "FINISHED", "Arena 1 Idle");

            //disable shaking from teleport
            teleport.GetState("Arrive").GetAction<SendEventByName>(5).sendEvent = string.Empty;

            //add aggro radius controls to teleport
            try
            {
                var preHideState = teleport.GetState("Music?");
                var hidden = teleport.AddState("Hidden");

                teleport.AddVariable<FsmBool>("IsAggro");
                var isAggro = teleport.FsmVariables.GetFsmBool("IsAggro");

                //change the teleport she does to keep her hidden until a player is nearby
                preHideState.ChangeTransition("FINISHED", "Hidden");
                teleport.AddTransition("Hidden", "SHOW", "Arrive");
                //hidden.AddTransition("SHOW", "Flash");
                isAggro.Value = false;
                hidden.AddAction(new BoolTest() { boolVariable = isAggro, isTrue = new FsmEvent("SHOW"), everyFrame = true });
            }
            catch (Exception e) { Dev.Log("error in teleport modification"); }

            //add aggro radius controls to control
            try
            {
                var firstTele = control.GetState("First Tele");
                var hidden = control.AddState("Hidden");

                control.AddVariable<FsmBool>("IsAggro");
                var isAggro = control.FsmVariables.GetFsmBool("IsAggro");
                isAggro.Value = false;

                control.RemoveAction("First Tele", 3); //remove big shake
                //firstTele.ChangeTransition("FINISHED", "Hidden");

                //introRecover.AddTransition("SHOW", "Arena 1 Start");
                control.RemoveTransition("First Tele", "TELEPORTED");
                control.AddTransition("First Tele", "FINISHED", "Hidden");
                firstTele.AddAction(new Wait() { finishEvent = new FsmEvent("FINISHED") });
                //control.AddTransition("Intro Recover", "SHOW", "Arena 1 Start");
                control.AddTransition("Hidden", "SHOW", "Intro Recover");
                hidden.AddAction(new BoolTest() { boolVariable = isAggro, isTrue = new FsmEvent("SHOW"), everyFrame = true });
            }
            catch (Exception e) { Dev.Log("error in control modification"); }

            //mute the init sfx
            control.GetState("Set Arena 1").GetFirstActionOfType<AudioPlayerOneShotSingle>().volume = 0f;
        }

        protected virtual Dictionary<string, Func<FSMAreaControlEnemy, float>> CommandFloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "Orb Max X", x => x.xR.Max - 1},
                { "Orb Max Y", x => x.yR.Max - 1},
                { "Orb Min X", x => x.xR.Min + 1},
                { "Orb Min Y", x => x.yR.Min + 3},
            };
        }

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs
        {
            get => new Dictionary<string, Func<FSMAreaControlEnemy, float>>()
            {
                { "A1 X Max", x => x.xR.Max - 2},
                { "A1 X Min", x => x.xR.Min + 2},
            };
        }

        protected override void UpdateRefs(PlayMakerFSM fsm, Dictionary<string, Func<FSMAreaControlEnemy, float>> refs)
        {
            base.UpdateRefs(fsm, refs);
            base.UpdateRefs(attackCommands, CommandFloatRefs);
        }

        protected override void BuildArena(Vector3 spawnPoint)
        {
            base.BuildArena(spawnPoint);
            bounds = new Rect(spawnPoint.x, spawnPoint.y, xR.Size, yR.Size);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);
        }

        protected virtual void OnEnable()
        {
            BuildArena(gameObject.transform.position);
        }

        protected override IEnumerator Start()
        {
            GameObject comb = attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject.Value;
            comb.transform.position = new Vector3(bounds.center.x, bounds.center.y, 0.006f);

            PlayMakerFSM combControl = comb.LocateMyFSM("Control");
            combControl.GetFirstActionOfType<SetPosition>("TL").x = bounds.xMin;
            combControl.GetFirstActionOfType<SetPosition>("TR").x = bounds.xMax;
            combControl.GetFirstActionOfType<RandomFloat>("Top").min = bounds.center.x - 1;
            combControl.GetFirstActionOfType<RandomFloat>("Top").max = bounds.center.x + 1;
            combControl.GetFirstActionOfType<SetPosition>("Top").y = bounds.yMax;
            combControl.GetFirstActionOfType<SetPosition>("L").x = bounds.xMin;
            combControl.GetFirstActionOfType<SetPosition>("L").y = bounds.center.y;
            combControl.GetFirstActionOfType<SetPosition>("R").x = bounds.xMax;
            combControl.GetFirstActionOfType<SetPosition>("R").y = bounds.center.y;

            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb Top").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb L").gameObject = comb;
            attackCommands.GetFirstActionOfType<SpawnObjectFromGlobalPool>("Comb R").gameObject = comb;

            control.GetAction<RandomFloat>("Set Dest", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest", 4).max = transform.position.y + 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).min = transform.position.y - 1;
            control.GetAction<RandomFloat>("Set Dest 2", 4).max = transform.position.y + 1;
            control.GetFirstActionOfType<SetFsmVector3>("First Tele").setValue = transform.position;
            control.GetFirstActionOfType<SetFsmVector3>("Rage1 Tele").setValue = transform.position;

            bool replaced = false;
            var actions = control.GetState("Climb Plats").Actions;
            control.GetState("Climb Plats").Actions =
                actions.Select(x =>
                {
                    if (!replaced && x == actions.First())
                    {
                        replaced = true;
                        return new CustomFsmAction(() => Destroy(gameObject));
                    }
                    return x;
                }).ToArray();

            if (!HeroInAggroRange())
                Hide();

            for (; ; )
            {
                UpdateHeroRefs();

                if (HeroInAggroRange())
                    Show();
                else
                    Hide();

                yield return new WaitForSeconds(1f);
            }
        }

        protected override bool HeroInAggroRange()
        {
            var size = new Vector2(30f, 30f);
            var center = new Vector2(xR.Mid, yR.Mid);
            var herop = new Vector2(HeroX, HeroY);
            var dist = herop - center;
            return (dist.sqrMagnitude < size.sqrMagnitude);
        }

        protected override void Show()
        {
            base.Show();
            attackCommands.SetState("Init");

            if (control.ActiveStateName == "Hidden")
                control.SendEvent("SHOW");

            if (teleport.ActiveStateName == "Hidden")
                teleport.SendEvent("SHOW");

        }

        protected override void Hide()
        {
            base.Hide();

            control.SetState("Init");
        }
    }

    public class AbsoluteRadianceSpawner : DefaultSpawner<AbsoluteRadianceControl>
    {
    }

    public class AbsoluteRadiancePrefabConfig : DefaultPrefabConfig<AbsoluteRadianceControl>
    {
    }
}
