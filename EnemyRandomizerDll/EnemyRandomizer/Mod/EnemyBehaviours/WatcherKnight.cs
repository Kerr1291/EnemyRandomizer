using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class WatcherKnight : EnemyBehaviour
    {
        private PlayMakerFSM _knight;

        private void Awake()
        {
            _knight = gameObject.LocateMyFSM("Black Knight");
        }

        public override void AdjustPosition()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
        }

        private IEnumerator Start()
        {
            _knight.SetState("Init");

            GetComponent<Rigidbody2D>().isKinematic = false;
            
            yield return new WaitUntil(() => _knight.ActiveStateName == "Rest");

            _knight.SetState("Roar End");
        }
    }
}