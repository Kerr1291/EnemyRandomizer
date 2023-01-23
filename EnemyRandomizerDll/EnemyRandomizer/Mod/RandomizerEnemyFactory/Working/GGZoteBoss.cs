using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class GGZoteCorpseFixer : MonoBehaviour
    {
        IEnumerator Start()
        {
            Dev.Log("trying to fix corpse white flash");
            var whiteScreenEffect = gameObject.FindGameObjectInChildrenWithName("white_solid");

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


            //fsm.ChangeTransition("Init", "UP", "Down");
            //fsm.RemoveTransition("Down", "UP");

            Destroy(this);
            yield break;
        }
    }

    public class GGZoteBoss : DefaultEnemy
    {
        public override void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            EnemyObject = prefabObject;

            base.Setup(enemy, knownEnemyTypes, EnemyObject);
        }

        public override GameObject Instantiate(EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            var newZote = base.Instantiate(sourceData, enemyToReplace, matchingData);
            newZote.SetActive(true);

            var deathEffects = newZote.GetComponentInChildren<EnemyDeathEffects>(true);
            deathEffects.doKillFreeze = false;

            if (deathEffects == null)
                Dev.LogError("Failed to find death effects on zote boss");
            else
                Dev.Log("found corpse holder");

            //load the corpse
            deathEffects.PreInstantiate();

            var corpse = deathEffects.GetFieldValue<GameObject>("corpse");

            if (corpse == null)
                Dev.LogError("Failed to find corpse zote boss");
            else
                corpse.AddComponent<GGZoteCorpseFixer>();

            return newZote;
        }
    }
}