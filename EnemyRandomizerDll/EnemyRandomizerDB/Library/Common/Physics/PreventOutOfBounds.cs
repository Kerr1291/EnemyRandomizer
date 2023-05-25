using System;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod
{
    //TODO: 
    //  rename to raycast collision interpolater/resolver or something related since that's where the behavior is going
    //  or more correctly, split the base behavior of other collsion/other raycast into a base class and have PreventOutOfBounds inherit from that
    public class PreventOutOfBounds : MonoBehaviour
    {
        //The raycast, the object colliding, the object that it collided with
        public Action<RaycastHit2D, GameObject, GameObject> onBoundCollision;

        //This collision happens only if both an "otherLayer" layermask is defined and this callback is defined.
        //The raycast, the object colliding, the object that it collided with
        public Action<RaycastHit2D, GameObject, GameObject> onOtherCollision;

        public LayerMask boundsLayer = (1 << 8);
        public LayerMask? otherLayer = null;

        public float minCheckDistance = Mathf.Epsilon;

        Vector3 previousLocation;
        Rigidbody2D body;
        Collider2D bodyCollider;

        int insideWallCounter = 0;
        bool didResolveThisFrame = false;

        public void ForcePosition(Vector3 pos)
        {
            gameObject.transform.position = pos;
            previousLocation = pos;
        }

        private void OnEnable()
        {
            previousLocation = transform.position;
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<Collider2D>();
        }

        private void LateUpdate()
        {
            ResolveMovement();
            didResolveThisFrame = false;
        }

        public void ResolveMovement()
        {
            if (didResolveThisFrame)
                return;

            didResolveThisFrame = true;

            Vector3 currentLocation = transform.position;
            Vector3 movementVector = (currentLocation - previousLocation);
            float distance = movementVector.magnitude;

            if (distance <= minCheckDistance)
                return;

            //check any custom collisions

            RaycastHit2D? otherHit = null;

            if (otherLayer.HasValue && onOtherCollision != null && onOtherCollision.GetInvocationList().Length > 0)
                otherHit = Physics2D.Raycast(previousLocation, movementVector.normalized, distance, otherLayer.Value);

            if (otherHit.HasValue && otherHit.Value.collider != null)
            {
                //we passed through something

                Vector3 collisionPoint = otherHit.Value.point;
                Vector3 collisionNormal = otherHit.Value.normal;

                onOtherCollision.Invoke(otherHit.Value, gameObject, otherHit.Value.collider.gameObject);
            }

            //resolve any collisions

            var result = Physics2D.Raycast(previousLocation, movementVector.normalized, distance, boundsLayer);

            if (result.collider != null)
            {
                //somehow we passed through a wall, fix it
                Vector3 collisionPoint = result.point;
                Vector3 collisionNormal = result.normal;

                var offset = Vector2.Dot(collisionNormal, gameObject.GetOriginalObjectSize(true)) * collisionNormal * 0.5f;

                transform.position = collisionPoint + offset;
                previousLocation = transform.position;

                if (onBoundCollision != null)
                {
                    onBoundCollision.Invoke(result, gameObject, result.collider.gameObject);
                }
            }
            else
            {
                previousLocation = currentLocation;
            }
        }

        //void ResolveInsideWalls()
        //{
        //    bool result = gameObject.ResolveInsideWalls();
        //    if(result)
        //    {
        //        previousLocation = transform.position;
        //    }

        //    //if(!gameObject.InBounds())
        //    //{
        //    //    var oobPos = transform.position;
        //    //    var origin = HeroController.instance.transform.position;
        //    //    var emergencyCorrectionDir = (oobPos - origin).normalized;
        //    //    var ray = Mathnv.GetRayOn(origin, emergencyCorrectionDir, float.MaxValue, SpawnerExtensions.IsSurfaceOrPlatform);
        //    //    gameObject.transform.position = ray.point;
        //    //    previousLocation = transform.position;
        //    //}

        //    //if(gameObject.IsInsideWalls())
        //    //{
        //    //    var rayOutOfWalls = gameObject.GetNearstRayOutOfWalls();
        //    //    var direction = -rayOutOfWalls.normal;
        //    //    gameObject.transform.position = rayOutOfWalls.point + Vector2.Dot(direction, gameObject.GetOriginalObjectSize()) * direction * 0.5f;
        //    //    previousLocation = transform.position;
        //    //}
        //}
    }
}