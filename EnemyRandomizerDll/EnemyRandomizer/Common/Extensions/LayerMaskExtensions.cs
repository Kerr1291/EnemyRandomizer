using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Checks if the given layer is inside the mask
        /// </summary>
        public static bool Contains( this LayerMask mask, int layerID )
        {
            return ( mask.value & ( 1 << layerID ) ) > 0;
        }

        /// <summary>
        /// Checks if the given gameobject's layer is inside the mask
        /// </summary>
        public static bool Contains(this LayerMask mask, GameObject go)
        {
            return (mask.value & (1 << go.layer)) > 0;
        }

        public static bool Any(this LayerMask mask, GameObject go)
        {
            return mask.Any(go.layer);
        }

        public static bool Any(this LayerMask mask, Component c)
        {
            return mask.Any(c.gameObject.layer);
        }

        /// <summary>
        /// Checks if the any layer (reading the input as a mask) is inside the mask
        /// </summary>
        public static bool Any(this LayerMask mask, LayerMask otherMask)
        {
            return (mask.value & otherMask.value) > 0;
        }

        /// <summary>
        /// Checks if the any layer (reading the input as a mask) is inside the mask
        /// </summary>
        public static bool Any(this LayerMask mask, int otherMask)
        {
            return (mask.value & otherMask) > 0;
        }

    }
}
