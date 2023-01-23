using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class MantisYouth : EnemyBehaviour
    {
        private PlayMakerFSM _flyer;

        private void Awake()
        {
            _flyer = gameObject.LocateMyFSM("Mantis Flyer");
        }

        private IEnumerator Start()
        {
            _flyer.Fsm.GetFsmBool("Start Idle").Value = true;
            
            _flyer.SetState("Init");

            yield return null;
        }
    }
}