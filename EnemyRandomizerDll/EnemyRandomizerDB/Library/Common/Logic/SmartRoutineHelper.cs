using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EnemyRandomizerMod
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

        List<SmartRoutine> completedRoutines = new List<SmartRoutine>();
        protected void LateUpdate()
        {
            foreach(SmartRoutine sr in Routines)
            {
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