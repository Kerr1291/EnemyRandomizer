using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    public partial class SmartRoutine
    {
        public SmartRoutine() { }

        
        public SmartRoutine(UpdateBehaviorFunc updateBehavior)
        {
            UpdateBehavior = updateBehavior;
        }

        public SmartRoutine(UpdateBehaviorFunc updateBehavior, Action onComplete)
        {
            OnComplete -= onComplete;
            OnComplete += onComplete;
            UpdateBehavior = updateBehavior;
        }

        public SmartRoutine(UpdateBehaviorFunc updateBehavior, Action<bool> onStop)
        {
            OnStop -= onStop;
            OnStop += onStop;
            UpdateBehavior = updateBehavior;
        }


        public SmartRoutine(UpdateBehaviorFunc updateBehavior, Action<bool> onStop, Action onStart)
        {
            OnStart -= onStart;
            OnStart += onStart;
            OnStop -= onStop;
            OnStop += onStop;
            UpdateBehavior = updateBehavior;
        }

        public SmartRoutine(UpdateBehaviorFunc updateBehavior, Action onComplete, Action onStart)
        {
            OnStart -= onStart;
            OnStart += onStart;
            OnComplete -= onComplete;
            OnComplete += onComplete;
            UpdateBehavior = updateBehavior;
        }



        public SmartRoutine(IEnumerator updateBehaviorInstance)
        {
            Start(updateBehaviorInstance);
        }

        public SmartRoutine(IEnumerator updateBehaviorInstance, Action onComplete)
        {
            Start(updateBehaviorInstance, onComplete);
        }

        public SmartRoutine(IEnumerator updateBehaviorInstance, Action<bool> onStop)
        {
            Start(updateBehaviorInstance, onStop);
        }

        public SmartRoutine(IEnumerator updateBehaviorInstance, Action<bool> onStop, Action onStart)
        {
            Start(updateBehaviorInstance, onStop, onStart);
        }

        public SmartRoutine(IEnumerator updateBehaviorInstance, Action onComplete, Action onStart)
        {
            Start(updateBehaviorInstance, onComplete, onStart);
        }
    }
}