using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nv
{
    public static class LayerMaskExtensions
    {
        public static bool Contains( this LayerMask mask, GameObject go )
        {
            return mask.Contains( go.layer );
        }

        public static bool Contains( this LayerMask mask, int layerID )
        {
            return ( mask.value & ( 1 << layerID ) ) > 0;
        }
    }
}
