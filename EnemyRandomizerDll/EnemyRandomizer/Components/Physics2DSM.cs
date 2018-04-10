using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nv
{
    public struct CollisionDirection
    {
        public bool above;
        public bool below;
        public bool right;
        public bool left;
    }

    public class Physics2DSM : GameStateMachine
    {
        protected Rigidbody2D body;
        protected BoxCollider2D bodyCollider;
        protected LayerMask collisionLayer = 8;

        //do we respond to world collisions in these directions?
        public bool checkUp = false;
        public bool checkDown = false;
        public bool checkLeft = true;
        public bool checkRight = true;

        //current status of collisions
        protected bool topHit = false;
        protected bool rightHit = false;
        protected bool bottomHit = false;
        protected bool leftHit = false;

        //distance of collision raycasts
        protected float raycastLength = .08f;

        //used by collision raycasts
        protected List<Vector2> topRays = new List<Vector2>();
        protected List<Vector2> rightRays = new List<Vector2>();
        protected List<Vector2> bottomRays = new List<Vector2>();
        protected List<Vector2> leftRays = new List<Vector2>();

        public override bool Running
        {
            get
            {
                return gameObject.activeInHierarchy;
            }

            set
            {
                gameObject.SetActive(value);
            }
        }

        protected override void SetupRequiredReferences()
        {
            base.SetupRequiredReferences();
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<BoxCollider2D>();
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.layer == collisionLayer)
            {
                CheckTouching(collisionLayer);
            }
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if(collision.gameObject.layer == collisionLayer)
            {
                CheckTouching(collisionLayer);
            }
        }

        //void OnCollisionStay2D(Collision2D collision)
        //{
        //    if(collision.gameObject.layer == 8)
        //    {
        //        CheckTouching(8);
        //    }
        //}

        protected virtual void EnableCollisionsInDirection(bool up, bool down, bool left, bool right)
        {
            checkUp = up;
            checkDown = down;
            checkLeft = left;
            checkRight = right;

            //clear any hit flags we're no longer colliding with
            if(!up)
                topHit = false;
            if(!down)
                bottomHit = false;
            if(!left)
                leftHit = false;
            if(!right)
                rightHit = false;
        }

        protected virtual void ClearPreviousCollisions()
        {
            topHit = false;
            bottomHit = false;
            leftHit = false;
            rightHit = false;
        }

        protected virtual CollisionDirection GetCollisionAlongCurrentVelocity(LayerMask layer, float timeStep)
        {
            CollisionDirection directionSet = new CollisionDirection();

            Vector2 origin = gameObject.transform.position;
            Vector2 direction = body.velocity.normalized;
            float distanceNextTimeStep = body.velocity.magnitude * timeStep;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, direction, distanceNextTimeStep, 1 << layer);

            //we're not going to hit anything
            if(raycastHit2D.collider == null)
                return directionSet;

            Dev.Log("We're about to hit a wall!");
            float x = gameObject.transform.position.x;
            float y = gameObject.transform.position.y;
            float px = raycastHit2D.point.x;
            float py = raycastHit2D.point.y;

            float dx = Mathf.Abs(x - px);
            float dy = Mathf.Abs(y - py);

            //is it an x collision or a y collision?
            if(dx > dy)
            {
                //x collision, hitting left or right from us?
                if(px < x)
                    directionSet.left = true;
                else
                    directionSet.right = true;
            }
            else
            {
                if(py < y)
                    directionSet.below = true;
                else
                    directionSet.above = true;
            }

            return directionSet;
        }

        protected virtual void CheckTouchingNextFrame(Vector2 velocity, LayerMask layer)
        {
            float oneFrame = 0.016f;
            Vector2 distanceNextFrame = oneFrame * velocity;

            if(this.checkUp)
            {
                this.topRays.Clear();
                this.topRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y) + distanceNextFrame);
                this.topRays.Add(new Vector2(this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.max.y) + distanceNextFrame);
                this.topRays.Add(distanceNextFrame + (Vector2)this.bodyCollider.bounds.max);
                this.topHit = false;
                for(int i = 0; i < 3; i++)
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(this.topRays[i], Vector2.up, raycastLength, 1 << layer);
                    if(raycastHit2D.collider != null)
                    {
                        this.topHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkRight)
            {
                this.rightRays.Clear();
                this.rightRays.Add(distanceNextFrame + (Vector2)this.bodyCollider.bounds.max);
                this.rightRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.center.y) + distanceNextFrame);
                this.rightRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y) + distanceNextFrame);
                this.rightHit = false;
                for(int j = 0; j < 3; j++)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this.rightRays[j], Vector2.right, raycastLength, 1 << layer);
                    if(raycastHit2D2.collider != null)
                    {
                        this.rightHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkDown)
            {
                this.bottomRays.Clear();
                this.bottomRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y) + distanceNextFrame);
                this.bottomRays.Add(new Vector2(this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.min.y) + distanceNextFrame);
                this.bottomRays.Add(distanceNextFrame + (Vector2)this.bodyCollider.bounds.min);
                this.bottomHit = false;

                for(int k = 0; k < 3; k++)
                {
                    RaycastHit2D raycastHit2D3 = Physics2D.Raycast(this.bottomRays[k], -Vector2.up, raycastLength, 1 << layer);
                    if(raycastHit2D3.collider != null)
                    {
                        this.bottomHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkLeft)
            {
                this.leftRays.Clear();
                this.leftRays.Add(distanceNextFrame + (Vector2)this.bodyCollider.bounds.min);
                this.leftRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.center.y) + distanceNextFrame);
                this.leftRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y) + distanceNextFrame);
                this.leftHit = false;
                for(int l = 0; l < 3; l++)
                {
                    RaycastHit2D raycastHit2D4 = Physics2D.Raycast(this.leftRays[l], -Vector2.right, raycastLength, 1 << layer);
                    if(raycastHit2D4.collider != null)
                    {
                        this.leftHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
        }//end CheckTouchingNextFrame

        protected virtual void CheckTouching(LayerMask layer)
        {
            if(this.checkUp)
            {
                this.topRays.Clear();
                this.topRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y));
                this.topRays.Add(new Vector2(this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.max.y));
                this.topRays.Add(this.bodyCollider.bounds.max);
                this.topHit = false;
                for(int i = 0; i < 3; i++)
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(this.topRays[i], Vector2.up, raycastLength, 1 << layer);
                    if(raycastHit2D.collider != null)
                    {
                        this.topHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkRight)
            {
                this.rightRays.Clear();
                this.rightRays.Add(this.bodyCollider.bounds.max);
                this.rightRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.center.y));
                this.rightRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y));
                this.rightHit = false;
                for(int j = 0; j < 3; j++)
                {
                    RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this.rightRays[j], Vector2.right, raycastLength, 1 << layer);
                    if(raycastHit2D2.collider != null)
                    {
                        this.rightHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkDown)
            {
                this.bottomRays.Clear();
                this.bottomRays.Add(new Vector2(this.bodyCollider.bounds.max.x, this.bodyCollider.bounds.min.y));
                this.bottomRays.Add(new Vector2(this.bodyCollider.bounds.center.x, this.bodyCollider.bounds.min.y));
                this.bottomRays.Add(this.bodyCollider.bounds.min);
                this.bottomHit = false;

                for(int k = 0; k < 3; k++)
                {
                    RaycastHit2D raycastHit2D3 = Physics2D.Raycast(this.bottomRays[k], -Vector2.up, raycastLength, 1 << layer);
                    if(raycastHit2D3.collider != null)
                    {
                        this.bottomHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
            if(this.checkLeft)
            {
                this.leftRays.Clear();
                this.leftRays.Add(this.bodyCollider.bounds.min);
                this.leftRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.center.y));
                this.leftRays.Add(new Vector2(this.bodyCollider.bounds.min.x, this.bodyCollider.bounds.max.y));
                this.leftHit = false;
                for(int l = 0; l < 3; l++)
                {
                    RaycastHit2D raycastHit2D4 = Physics2D.Raycast(this.leftRays[l], -Vector2.right, raycastLength, 1 << layer);
                    if(raycastHit2D4.collider != null)
                    {
                        this.leftHit = true;
                        //TODO: call a callback here
                        break;
                    }
                }
            }
        }//end CheckTouching

        //if the values are within the tolerance, the object is not enough in that direction to be considered offset from us
        static protected CollisionDirection GetDirectionToTarget(GameObject self, GameObject target, float toleranceX = 0.1f, float toleranceY = 0.5f)
        {
            CollisionDirection direction = new CollisionDirection();
            float num = self.transform.position.x;
            float num2 = self.transform.position.y;
            float num3 = target.transform.position.x;
            float num4 = target.transform.position.y;

            direction.right = (num < num3) && Mathf.Abs(num - num3) > toleranceX;
            direction.left = (num > num3) && Mathf.Abs(num - num3) > toleranceX;
            direction.above = (num2 < num4) && Mathf.Abs(num2 - num4) > toleranceY;
            direction.below = (num2 > num4) && Mathf.Abs(num2 - num4) > toleranceY;

            return direction;
        }

        //TODO: change to a static function that takes 3 vectors, origin, target, and offsets
        static protected float GetAngleToTarget(GameObject self, GameObject target, float offsetX, float offsetY)
        {
            float num = target.transform.position.y + offsetY - self.transform.position.y;
            float num2 = target.transform.position.x + offsetX - self.transform.position.x;
            float num3;
            for(num3 = Mathf.Atan2(num, num2) * 57.2957764f; num3 < 0f; num3 += 360f)
            {
            }
            return num3;
        }

        static protected Vector2 GetVelocityToTarget(Vector2 self, Vector2 projectile, Vector2 target, float speed, float spread = 0f)
        {
            float num = target.y + projectile.y - self.y;
            float num2 = target.x + projectile.x - self.x;
            float num3 = Mathf.Atan2(num, num2) * 57.2957764f;
            if(Mathf.Abs(spread) > Mathf.Epsilon)
            {
                num3 += GameRNG.Rand(-spread, spread);
            }
            float x = speed * Mathf.Cos(num3 * 0.0174532924f);
            float y = speed * Mathf.Sin(num3 * 0.0174532924f);
            Vector2 velocity;
            velocity.x = x;
            velocity.y = y;
            return velocity;
        }

        static protected Vector2 GetVelocityFromSpeedAndAngle(float speed, float angle)
        {
            float x = speed * Mathf.Cos(angle * 0.0174532924f);
            float y = speed * Mathf.Sin(angle * 0.0174532924f);
            Vector2 velocity;
            velocity.x = x;
            velocity.y = y;
            return velocity;
        }
    }
}