using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class DungDefender : EnemyBehaviour
    {
        private PlayMakerFSM _dd;

        private void Awake()
        {
            _dd = gameObject.LocateMyFSM("Dung Defender");
        }

        private IEnumerator Start()
        {
            _dd.SetState("Init");

            GetComponent<MeshRenderer>().enabled = true;
            
            yield return new WaitWhile(() => _dd.ActiveStateName != "Sleep");
            
            _dd.SetState("Will Evade?");
        }
    }
}