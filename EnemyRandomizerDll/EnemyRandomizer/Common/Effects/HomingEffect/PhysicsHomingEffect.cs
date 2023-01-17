using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class PhysicsHomingEffect : HomingEffect
    {
        protected Rigidbody body;
        public Rigidbody Body
        {
            get
            {
                return body ?? (body = GetComponent<Rigidbody>());
            }
        }

        public ForceMode homingForceType = ForceMode.VelocityChange;

        protected override void Reset()
        {
            base.Reset();
            if(Body != null)
                body.useGravity = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(Body == null)
            {
                homingForceType = ForceMode.VelocityChange;
            }
        }

        public override Vector3 Velocity
        {
            get
            {
                if(Body != null)
                    return Body.velocity;
                return base.Velocity;
            }

            set
            {
                if(Body != null)
                    Body.velocity = value;
                else
                    base.Velocity = value;
            }
        }

        protected override void Step(Vector3 step)
        {
            if(Body != null)
            {
                ApplyForce(step);
            }
            else
            {
                base.Step(step);
            }
        }

        protected void ApplyForce(Vector3 force)
        {
            Body.AddForce(force, homingForceType);
        }
    }
}
