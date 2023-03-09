using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(TriangleExplosion))]
    public class ExplodeOnContact : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        Collider colliderToDisable;

        [SerializeField]
        [HideInInspector]
        Rigidbody bodyToDisable;

        [SerializeField]
        [HideInInspector]
        TriangleExplosion explosionToTrigger;

        [ContextMenu("ForceInit")]
        void Init()
        {
            colliderToDisable = GetComponent<Collider>();
            bodyToDisable = GetComponent<Rigidbody>();
            explosionToTrigger = gameObject.GetOrAddComponent<TriangleExplosion>();
        }

        void Reset()
        {
            Init();
        }

        [Tooltip("The object must hit at least this hard to explode")]
        [Header("The object must hit at least this hard to explode")]
        public float minimumForceToTriggerExplosion;

        [Tooltip("Spawn this prefab when the explosion is triggered")]
        [Header("Spawn this prefab when the explosion is triggered")]
        public GameObject explodeEffect;
        GameObject _effect;

        [Tooltip("Disable these objects when the explosion is triggered")]
        [Header("Disable these objects when the explosion is triggered")]
        public GameObject[] disableOnContact;

        [Tooltip("Do not trigger from these objects")]
        [Header("Do not trigger from these objects")]
        public List<GameObject> ignoreOnContact;

        [Tooltip("Do not trigger from objects with these tags")]
        [Header("Do not trigger from objects with these tags")]
        public List<string> ignoreTags;

        bool exploded = false;

        float CalculateHitForce(Collision collision)
        {
            float averageA = 0.0f;
            for(int i = 0; i < collision.contacts.Length; ++i)
            {
                Vector3 normal = collision.contacts[i].normal;
                Vector3 velocity = collision.relativeVelocity;

                float A = Vector3.Dot(normal, collision.relativeVelocity);

                float forceA = A * bodyToDisable.mass;
                averageA += forceA;
            }
            averageA /= collision.contacts.Length;
            return Mathf.Abs(averageA);
        }

        void OnCollisionEnter(Collision collision)
        {
            if(ignoreOnContact.Contains(collision.gameObject))
                return;

            if(ignoreTags.Contains(collision.gameObject.tag))
                return;

            float collisionForce = CalculateHitForce(collision);
            //DLog.Log( "collisionForce "+collisionForce);
            if(collisionForce < minimumForceToTriggerExplosion)
                return;

            TryExplode(collision.contacts[0].point);
        }

        void TryExplode(Vector3 explosionPoint)
        {
            if(explosionToTrigger != null && !exploded)
            {
                exploded = true;

                //disable related components
                colliderToDisable.enabled = false;
                bodyToDisable.velocity = Vector3.zero;
                bodyToDisable.isKinematic = true;

                //finally trigger explosion
                explosionToTrigger.explosionPoint = explosionPoint;
                explosionToTrigger.ExplodeMesh();

                //create any optional things
                if(explodeEffect != null)
                {
                    _effect = (GameObject)Instantiate(explodeEffect, Vector3.zero, Quaternion.identity);
                    _effect.transform.SetParent(transform);
                    _effect.transform.localPosition = Vector3.zero;
                }

                //disable anything we need to
                if(disableOnContact != null)
                {
                    for(int i = 0; i < disableOnContact.Length; ++i)
                    {
                        disableOnContact[i].SetActive(false);
                    }
                }
            }
        }
    }
}