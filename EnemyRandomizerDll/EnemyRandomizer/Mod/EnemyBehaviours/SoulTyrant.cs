using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class SoulTyrant : EnemyBehaviour
    {
        private PlayMakerFSM _lord;

        private void Awake()
        {
            _lord = gameObject.LocateMyFSM("Mage Lord");
        }

        private IEnumerator Start()
        {
            _lord.Fsm.GetFsmFloat("Bot Y").Value = bounds.yMin + 3;
            _lord.Fsm.GetFsmFloat("Ground Y").Value = bounds.yMin + 3;
            _lord.Fsm.GetFsmFloat("Knight Quake Y Max").Value = bounds.yMin + 7.5f;
            _lord.Fsm.GetFsmFloat("Left X").Value = bounds.xMin;
            _lord.Fsm.GetFsmFloat("Right X").Value = bounds.xMax;
            _lord.Fsm.GetFsmFloat("Quake Y").Value = bounds.yMin + 10;
            _lord.Fsm.GetFsmFloat("Shockwave Y").Value = bounds.yMin;
            _lord.Fsm.GetFsmFloat("Top Y").Value = bounds.yMin + 8;
            
            _lord.SetState("Init");

            yield return new WaitUntil(() => _lord.ActiveStateName == "Sleep");

            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<HealthManager>().IsInvincible = false;
            _lord.SetState("Set Idle Timer");
        }
    }
}