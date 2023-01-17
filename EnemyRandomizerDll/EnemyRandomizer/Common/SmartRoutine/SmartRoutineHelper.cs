using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace nv
{
    public class SmartRoutineHelper : GameSingleton<SmartRoutineHelper>
    {
        List<SmartRoutine> routines = null;
        public List<SmartRoutine> Routines
        {
            get
            {
                if(routines == null)
                {
                    routines = typeof(SmartRoutine).GetField("routines", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as List<SmartRoutine>;
                }
                return routines;
            }
        }

        List<UnityRoutine> unityRoutines = null;
        public List<UnityRoutine> UnityRoutines
        {
            get
            {
                if(unityRoutines == null)
                {
                    unityRoutines = typeof(UnityRoutine).GetField("routines", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as List<UnityRoutine>;
                }
                return unityRoutines;
            }
        }

        protected void Update()
        {
            foreach(UnityRoutine sr in UnityRoutines)
            {
                if(sr.OnUpdate != null)
                {
                    sr.OnUpdate.Invoke();
                }
            }
        }

        protected void FixedUpdate()
        {
            foreach(UnityRoutine sr in UnityRoutines)
            {
                if(sr.OnFixedUpdate != null)
                {
                    sr.OnFixedUpdate.Invoke();
                }
            }
        }

        List<SmartRoutine> completedRoutines = new List<SmartRoutine>();
        protected void LateUpdate()
        {
            foreach(SmartRoutine sr in Routines)
            {
                UnityRoutine ur = sr as UnityRoutine;

                if(ur != null && ur.OnLateUpdate != null)
                {
                    ur.OnLateUpdate.Invoke();
                }

                if(sr.IsComplete)
                {
                    completedRoutines.Add(sr);
                }
            }

            foreach(SmartRoutine sr in completedRoutines)
            {
                sr.Stop();
            }
        }
    }
}