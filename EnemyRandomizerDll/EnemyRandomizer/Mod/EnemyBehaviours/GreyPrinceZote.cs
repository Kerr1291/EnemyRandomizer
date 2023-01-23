using System.Collections;
using System.Linq;
using UnityEngine;
using Vasi;
using nv;

namespace EnemyRandomizerMod.Behaviours
{
    public class GreyPrinceZote : EnemyBehaviour
    {
        private PlayMakerFSM _constrainX;
        private PlayMakerFSM _control;
        int inAirTime = -1;
        int stompTime = -1;

        private void Awake()
        {
            _constrainX = gameObject.LocateMyFSM("Constrain X");
            _control = gameObject.LocateMyFSM("Control");
            
        }

        private IEnumerator Start()
        {
            _constrainX.Fsm.GetFsmFloat("Edge L").Value = bounds.xMin;
            _constrainX.Fsm.GetFsmFloat("Edge R").Value = bounds.xMax;
            
            _control.Fsm.GetFsmFloat("Left X").Value = bounds.xMin + 2;
            _control.Fsm.GetFsmFloat("Right X").Value = bounds.xMax - 2;

            _control.GetAction<GGCheckIfBossScene>("Level Check").regularSceneEvent = _control.Fsm.Events.First(@event => @event.Name == "3");

            _control.GetAction<SetDamageHeroAmount>("Set Damage", 0).damageDealt = 1;
            _control.GetAction<SetDamageHeroAmount>("Set Damage", 1).damageDealt = 1;
            _control.GetAction<SetDamageHeroAmount>("Set Damage", 2).damageDealt = 1;

            _control.SetState("Pause");

            yield return new WaitUntil(() => _control.ActiveStateName == "Dormant");

            GetComponent<HealthManager>().IsInvincible = false;
            _control.SetState("Activate");
        }

        private void Update()
        {
            if (_control.ActiveStateName == "In Air")
            {
                inAirTime++;
                if (inAirTime > 180)
                {
                    _control.Fsm.GetFsmBool("bottomHit").Value = true;
                    inAirTime = -1;
                }
            }
            else
                inAirTime = -1; 
            if (_control.ActiveStateName == "Stomp")
            {
                stompTime++;
                if (stompTime > 180)
                {
                    _control.Fsm.GetFsmBool("bottomHit").Value = true;
                    stompTime = -1;
                }
            }
            else
                stompTime = -1;
            Dev.Log("GPZ State: " + _control.ActiveStateName);
            Dev.Log("Y Pos: " + _control.Fsm.GetFsmFloat("Left X").Value);
            //_control.SetState("Idle Start");
        }
    }
}