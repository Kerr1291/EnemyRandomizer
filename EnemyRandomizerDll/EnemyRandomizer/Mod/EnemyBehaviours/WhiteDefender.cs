using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class WhiteDefender : EnemyBehaviour
    {
        private PlayMakerFSM _constrainX;
        private PlayMakerFSM _dd;

        private void Awake()
        {
            _constrainX = gameObject.LocateMyFSM("Constrain X");
            _dd = gameObject.LocateMyFSM("Dung Defender");
        }

        private IEnumerator Start()
        {
            _dd.SetState("Init");

            foreach (Transform pillarTransform in gameObject.transform.Find("Slam Pillars"))
            {
                GameObject dungPillar = pillarTransform.gameObject;
                PlayMakerFSM pillarCtrl = dungPillar.LocateMyFSM("Control");
                pillarCtrl.GetAction<SetDamageHeroAmount>("Init").damageDealt = 1;
            }

            _constrainX.Fsm.GetFsmFloat("Edge L").Value = bounds.xMin;
            _constrainX.Fsm.GetFsmFloat("Edge R").Value = bounds.xMax;
            
            _dd.Fsm.GetFsmFloat("Centre X").Value = bounds.center.x;
            _dd.Fsm.GetFsmFloat("Dolphin Max X").Value = bounds.xMax - 4 ;
            _dd.Fsm.GetFsmFloat("Dolphin Min X").Value = bounds.xMin + 4;
            _dd.Fsm.GetFsmFloat("Erupt Y").Value = bounds.yMin;
            _dd.Fsm.GetFsmFloat("Max X").Value = bounds.xMax;
            _dd.Fsm.GetFsmFloat("Min X").Value = bounds.xMin;

            _dd.GetState("Roar?").RemoveAction<SendEventByName>();
            _dd.GetState("Roar?").RemoveAction<SetFsmGameObject>();
            _dd.GetState("Rage Roar").RemoveAction<SendEventByName>();
            _dd.GetState("Rage Roar").RemoveAction<SetFsmGameObject>();
            _dd.GetState("Init").RemoveAction<GetPlayerDataInt>();

            for (int i = 16; i <= 43; i += 3)
            {
                _dd.GetAction<SetDamageHeroAmount>("Erupt Out", i).damageDealt = 1;
            }
            _dd.GetAction<SetDamageHeroAmount>("Init 2", 1).damageDealt = 1;
            _dd.GetAction<SetDamageHeroAmount>("Init 2", 2).damageDealt = 1;
            _dd.GetAction<SetDamageHeroAmount>("Pillar", 2).damageDealt = 1;
            _dd.GetAction<SetDamageHeroAmount>("Pillar", 6).damageDealt = 1;
            _dd.GetAction<SetDamageHeroAmount>("Throw 1").damageDealt = 1;

            GetComponent<DamageHero>().damageDealt = 1;
            GameObject corpse =
                ReflectionHelper.GetField<EnemyDeathEffects, GameObject>(GetComponent<EnemyDeathEffectsUninfected>(),
                    "corpse");
            corpse.LocateMyFSM("Control").GetState("Blow 2").AddMethod(() => Destroy(corpse));

            yield return new WaitUntil(() => _dd.ActiveStateName == "Sleep");

            GetComponent<HealthManager>().IsInvincible = false;

            _dd.SetState("After Evade");
            _dd.Fsm.GetFsmInt("Damage").Value = 1;
        }
    }
}