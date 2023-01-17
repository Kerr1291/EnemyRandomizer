using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class Physics2DHomingEffect : HomingEffect
    {
        protected Rigidbody2D body;
        public Rigidbody2D Body
        {
            get
            {
                return body ?? (body = GetComponent<Rigidbody2D>());
            }
        }

        public ForceMode2D homingForceType = ForceMode2D.Impulse;

        protected override void Reset()
        {
            base.Reset();
            if(Body != null)
                body.gravityScale = 0f;
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
            CapVelocity();
        }
    }
}

