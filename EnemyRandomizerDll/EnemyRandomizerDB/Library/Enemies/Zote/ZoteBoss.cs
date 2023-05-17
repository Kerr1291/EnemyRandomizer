using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class GGZoteCorpseFixer : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return new WaitUntil(() => gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid") != null);
            Dev.Log("trying to fix corpse white flash");
            var whiteScreenEffect = gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid").gameObject;

            if (whiteScreenEffect == null)
            {
                Dev.LogError("Failed to find white screen effect");
                yield break;
            }

            var corpseFSM = gameObject.LocateMyFSM("Control");
            var fsm = whiteScreenEffect.LocateMyFSM("FSM");

            while (fsm == null)

            {
                fsm = whiteScreenEffect.LocateMyFSM("FSM");
                yield return null;
            }

            while (fsm.GetState("Init") == null)
                yield return null;

            while (fsm.GetState("Down") == null)
                yield return null;


            if (fsm.ActiveStateName == "Init")
                fsm.SendEvent("UP");

            while (fsm.ActiveStateName != "Upped")
                yield return null;

            HeroController.instance.RegainControl();
            HeroController.instance.StartAnimationControl();

            if (fsm.ActiveStateName == "Upped")
                fsm.SendEvent("DOWN");


            while (corpseFSM.ActiveStateName != "Notify")
                yield return null;


            while (corpseFSM.ActiveStateName == "Notify")
            {
                corpseFSM.SendEvent("CORPSE END");
                yield return null;
            }

            Destroy(this);
            yield break;
        }
    }


    public class ZoteBossControl : FSMBossAreaControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            Geo = 1;

            var whiteScreenEffectfsm = gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid");
            if (whiteScreenEffectfsm != null)
            {
                var whiteScreenEffect = whiteScreenEffectfsm.gameObject;
                if (whiteScreenEffect != null)
                {
                    Destroy(whiteScreenEffect);
                }
            }
            else
            {
                Dev.LogError("Could not find white screen child object on Zote Boss!");
            }    

            var corpse = thisMetadata.Corpse;
            if(corpse != null)
            {
                var white2 = corpse.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid");
                if (white2 != null && white2.gameObject != null)
                    Destroy(white2.gameObject);

                var corpseFSM = corpse.LocateMyFSM("Control");
                if (corpseFSM != null)
                {
                    var init = corpseFSM.GetState("Init");
                    DisableActions(init, 0, 1, 7, 8, 9, 10, 11, 15);

                    var inAir = corpseFSM.GetState("In Air");
                    DisableActions(inAir, 0);
                    AddTimeoutAction(inAir, "LAND", 1f);

                    var burst = corpseFSM.GetState("Burst");
                    burst.DisableAction(5);
                    burst.ChangeTransition("FINISHED", "End");

                    var notify = corpseFSM.GetState("Notify");
                    notify.DisableAction(0);
                    notify.AddCustomAction(() => { control.SendEvent("CORPSE END"); });

                    var end = corpseFSM.GetState("End");
                    DisableActions(end, 0, 1, 2, 3);

                    var land = corpseFSM.GetState("Land");
                    DisableActions(land, 0, 3, 4, 5, 6, 7, 13);
                }
                else
                {
                    Dev.LogError("corpseFSM not found in Zote Boss");
                }
            }
            else
            {
                Debug.LogError("Failed to find corpse on Zote boss!");
            }

            var roara = control.GetState("Roar Antic");
            if (roara != null)
            {
                roara.ChangeTransition("FINISHED", "Roar End");
            }
            else
            {
                Dev.LogError("No roar antic on zote boss?");
            }

            DisableKillFreeze();
        }
    }
        
    public class ZoteBossSpawner : DefaultSpawner<ZoteBossControl> { }

    public class ZoteBossPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);

            var Prefab = p.prefab;

            Dev.Log("getting death effects");
            var deathEffects = Prefab.GetComponentInChildren<EnemyDeathEffectsUninfected>(true);
            //var baseDeathEffects = deathEffects as EnemyDeathEffects;

            deathEffects.doKillFreeze = false;


            var corpsePrefab = (GameObject)deathEffects.GetType().BaseType.GetField("corpsePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);

            if (corpsePrefab == null)
                Dev.LogError("Failed to find corpse prefab zote boss");
            else
            {
                corpsePrefab.AddComponent<GGZoteCorpseFixer>();
            }
        }
    }
}
