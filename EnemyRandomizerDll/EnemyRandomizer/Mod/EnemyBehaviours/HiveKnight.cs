using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class HiveKnight : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        private IEnumerator Start()
        {
            _control.SetState("Init");

            yield return new WaitWhile(() => _control.ActiveStateName != "Sleep");

            GetComponent<MeshRenderer>().enabled = true;

            _control.Fsm.GetFsmFloat("Left X").Value = bounds.xMin;
            _control.Fsm.GetFsmFloat("Right X").Value = bounds.xMax;
            _control.SetState("Activate");
        }
    }
}