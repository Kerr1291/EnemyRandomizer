using UnityEngine;
using System.Collections;

namespace nv
{
    /// <summary>
    /// Applies a force to a rigid body when it enters this collider
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ForcePusher : MonoBehaviour
    {
        public float pushAmount = 1.0f;
        public UnityEngine.ForceMode pushForce = ForceMode.VelocityChange;

        [Tooltip("Direction used if transform is null")]
        public Transform pushTransform;

        [Tooltip("Direction used if transform is null")]
        public Vector3 pushDirection = Vector3.up;

        [Tooltip("Sound played when a push is triggered")]
        public AudioSource pushSound;

        public bool pushColliders = true;
        public bool pushTriggers = true;

        public void TryPush(Rigidbody body)
        {
            if(body == null)
                return;

            Vector3 dirToPush = pushDirection.normalized;
            if(pushTransform != null)
                dirToPush = transform.forward;

            body.AddForce(dirToPush * pushAmount, pushForce);
        }

        void OnCollisionEnter(Collision other)
        {
            if(pushColliders)
                TryPush(other.rigidbody);
        }

        void OnTriggerEnter(Collider other)
        {
            if(pushTriggers)
                TryPush(other.attachedRigidbody);
        }
    }
}