using System.Collections;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class Lobster : EnemyBehaviour
    {
        private PlayMakerFSM _control;

        private void Awake()
        {
            _control = gameObject.LocateMyFSM("Control");
        }

        public override void AdjustPosition()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 3, transform.position.z);
        }

        private IEnumerator Start()
        {
            _control.SetState("Init");
            
            yield return new WaitForEndOfFrame();
            //yield return new WaitUntil(() => _control.ActiveStateName == "Init");

            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<PlayMakerFixedUpdate>().enabled = true;
            _control.SetState("Idle");
        }
    }
}