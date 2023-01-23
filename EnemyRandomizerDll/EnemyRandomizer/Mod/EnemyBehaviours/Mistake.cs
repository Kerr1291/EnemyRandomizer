using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class Mistake : EnemyBehaviour
    {
        private PlayMakerFSM _blob;

        private void Awake()
        {
            _blob = gameObject.LocateMyFSM("Blob");
        }

        private IEnumerator Start()
        {
            _blob.SetState("Init");

            yield return new WaitWhile(() => _blob.ActiveStateName != "Hide");

            _blob.SetState("Activate");
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}