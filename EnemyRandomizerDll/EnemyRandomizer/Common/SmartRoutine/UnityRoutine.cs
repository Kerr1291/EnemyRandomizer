using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace nv
{
    public class UnityRoutine : SmartRoutine
    {
        static List<UnityRoutine> routines = new List<UnityRoutine>();
        static List<UnityRoutine> Routines
        {
            get
            {
                if(routines == null)
                    routines = new List<UnityRoutine>();
                return routines;
            }
        }

        public virtual Action OnUpdate { get; set; }
        public virtual Action OnFixedUpdate { get; set; }
        public virtual Action OnLateUpdate { get; set; }    
    }
}