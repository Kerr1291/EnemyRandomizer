using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public class RangeCheck : MonoBehaviour
    {
        public virtual bool ObjectIsInRange { get; protected set; }

        public void OnTriggerEnter2D( Collider2D collisionInfo )
        {
            ObjectIsInRange = true;
        }

        public virtual void OnTriggerExit2D( Collider2D collisionInfo )
        {
            ObjectIsInRange = false;
        }
    }
}