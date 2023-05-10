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
    public class ZoteBossControl : FSMBossAreaControl
    {
        public override string FSMName => "Control";

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.Geo = 1;

            var whiteScreenEffect = gameObject.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid").gameObject;
            if (whiteScreenEffect != null)
            {
                Destroy(whiteScreenEffect);
            }

            var corpse = thisMetadata.Corpse;
            if(corpse != null)
            {
                var white2 = corpse.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.gameObject.name == "white_solid");
                if (white2 != null)
                    Destroy(white2.gameObject);

                var corpseFSM = corpse.LocateMyFSM("Control");

                var init = corpseFSM.GetState("Init");
                DisableActions(init, 0,1,8,9,10,11,15);

                var end = corpseFSM.GetState("End");
                DisableActions(end, 0, 1, 2, 3);

                var inAir = corpseFSM.GetState("In Air");
                DisableActions(inAir, 0);
                inAir.AddCustomAction(() => { StartCoroutine(TimeoutState("In Air", "LAND", 2f)); });

                var burst = corpseFSM.GetState("Burst");
                burst.DisableAction(5);
                burst.ChangeTransition("FINISHED", "End");

                var land = corpseFSM.GetState("Land");
                DisableActions(land, 0, 3, 4, 5, 6, 7, 13);
            }

            var roara = control.GetState("Roar Antic");
            roara.ChangeTransition("FINISHED", "Roar End");

            Dev.Log("getting death effects");
            var deathEffects = gameObject.GetComponentInChildren<EnemyDeathEffectsUninfected>(true);
            deathEffects.doKillFreeze = false;
        }

        protected virtual IEnumerator TimeoutState(string currentState, string endEvent, float timeout)
        {
            while (control.ActiveStateName == currentState)
            {
                timeout -= Time.deltaTime;

                if (timeout <= 0f)
                {
                    control.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }
        
    public class ZoteBossSpawner : DefaultSpawner<ZoteBossControl> { }

    public class ZoteBossPrefabConfig : DefaultPrefabConfig<ZoteBossControl> { }
}
