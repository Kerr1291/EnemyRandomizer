using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;

namespace EnemyRandomizerMod.Behaviours
{
    public class Nosk : EnemyBehaviour
    {
        private HealthManager _hm;
        private PlayMakerFSM _constrainX;
        private PlayMakerFSM _spider;

        private void Awake()
        {
            _hm = GetComponent<HealthManager>();
            _constrainX = gameObject.LocateMyFSM("constrain_x");
            _spider = gameObject.LocateMyFSM("Mimic Spider");
        }

        private IEnumerator Start()
        {
            _constrainX.Fsm.GetFsmFloat("Edge L").Value = bounds.xMin;
            _constrainX.Fsm.GetFsmFloat("Edge R").Value = bounds.xMax;

            _spider.Fsm.GetFsmFloat("Jump Max X").Value = bounds.xMax - 6;
            _spider.Fsm.GetFsmFloat("Jump Min X").Value = bounds.xMin + 6;
            _spider.Fsm.GetFsmFloat("Roof Y").Value = bounds.yMax - 2;

            _spider.GetState("Trans 1").RemoveAction<ApplyMusicCue>();
            _spider.GetState("Trans 1").RemoveAction<CreateObject>();
            _spider.GetState("Trans 1").RemoveAction<SetFsmGameObject>();
            _spider.GetState("Trans 1").RemoveAction(6);

            _spider.GetState("Roof Drop").InsertMethod(0, () => _hm.IsInvincible = true);
            _spider.GetState("Falling").InsertMethod(0, () => _hm.IsInvincible = false);

            _spider.SetState("Init");

            yield return new WaitUntil(() => _spider.ActiveStateName == "Hollow Idle");

            _hm.IsInvincible = false;
            // For some reason Nosk dies twice
            HeroController.instance.gameObject.LocateMyFSM("Roar Lock").SendEvent("ROAR EXIT");
            _spider.SetState("Idle");
        }
    }
}