using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace nv
{
    public partial class SmartRoutine
    {
        /// <summary>
        /// Primary Start() method. All overrides call into this
        /// </summary>
        /// <param name="tryResume"></param>
        /// <param name="args"></param>
        public virtual IEnumerator Start(bool tryResume, params object[] args)
        {
            if(UpdateBehavior == null && UpdateBehaviorInstance == null)
            {
                Debug.Log("Warning: UpdateBehavior is null and UpdateBehaviorInstance is null. Start(bool tryResume, params object[] args) will do nothing.");
                return null;
            }
            
            if(UpdateBehavior != null)
            {
                //try to resume, but if it's complete start a new one
                if(!tryResume || IsComplete)
                {
                    CreateUptadeBehaviorInstance(args);
                }
            }

            return StartUpdateBehaviorInstance();
        }

        public virtual IEnumerator Start(IEnumerator updateBehaviorInstance)
        {
            if(updateBehaviorInstance == null)
            {
                Debug.Log("Warning: updateBehaviorInstance is null. Start(IEnumerator updateBehaviorInstance) will do nothing.");
                return null;
            }

            CreateUptadeBehaviorInstance(updateBehaviorInstance);

            return Start(true);
        }


        /// <summary>
        /// Allows Start() with no arguments.
        /// </summary>
        public virtual IEnumerator Start(params object[] args)
        {
            return Start(false, args);
        }

        /// <summary>
        /// Allows Start() with specified update behavior.
        /// </summary>
        public virtual IEnumerator Start(UpdateBehaviorFunc updateBehavior, params object[] args)
        {
            UpdateBehavior = updateBehavior;

            return Start(args);
        }

        public virtual IEnumerator Start(UpdateBehaviorFunc updateBehavior, Action onComplete, params object[] args)
        {
            OnComplete -= onComplete;
            OnComplete += onComplete;

            return Start(updateBehavior, args);
        }

        public virtual IEnumerator Start(UpdateBehaviorFunc updateBehavior, Action<bool> onStop, params object[] args)
        {
            OnStop -= onStop;
            OnStop += onStop;

            return Start(updateBehavior, args);
        }


        public virtual IEnumerator Start(UpdateBehaviorFunc updateBehavior, Action<bool> onStop, Action onStart, params object[] args)
        {
            OnStart -= onStart;
            OnStart += onStart;

            return Start(updateBehavior, onStop, args);
        }

        public virtual IEnumerator Start(UpdateBehaviorFunc updateBehavior, Action onComplete, Action onStart, params object[] args)
        {
            OnStart -= onStart;
            OnStart += onStart;

            return Start(updateBehavior, onComplete, args);
        }



        public virtual IEnumerator Start(Action onComplete, params object[] args)
        {
            return Start(UpdateBehavior, onComplete, false, args);
        }

        public virtual IEnumerator Start(Action onComplete, bool tryResume, params object[] args)
        {
            return Start(UpdateBehavior, onComplete, tryResume, args);
        }

        public virtual IEnumerator Start(Action<bool> onStop, params object[] args)
        {
            return Start(UpdateBehavior, onStop, false, args);
        }

        public virtual IEnumerator Start(Action<bool> onStop, bool tryResume, params object[] args)
        {
            return Start(UpdateBehavior, onStop, tryResume, args);
        }

        public virtual IEnumerator Start(Action<bool> onStop, Action onStart, params object[] args)
        {
            return Start(UpdateBehavior, onStop, onStart, false, args);
        }

        public virtual IEnumerator Start(Action onComplete, Action onStart, params object[] args)
        {
            return Start(UpdateBehavior, onComplete, onStart, false, args);
        }

        public virtual IEnumerator Start(Action onComplete, Action onStart, bool tryResume, params object[] args)
        {
            return Start(UpdateBehavior, onComplete, onStart, tryResume, args);
        }

        public virtual IEnumerator Start(Action<bool> onStop, Action onStart, bool tryResume, params object[] args)
        {
            return Start(UpdateBehavior, onStop, onStart, tryResume, args);
        }




        public virtual IEnumerator Start(IEnumerator updateBehaviorInstance, Action onComplete)
        {
            OnComplete -= onComplete;
            OnComplete += onComplete;

            return Start(updateBehaviorInstance);
        }

        public virtual IEnumerator Start(IEnumerator updateBehaviorInstance, Action<bool> onStop)
        {
            OnStop -= onStop;
            OnStop += onStop;

            return Start(updateBehaviorInstance);
        }

        public virtual IEnumerator Start(IEnumerator updateBehaviorInstance, Action<bool> onStop, Action onStart)
        {
            OnStart -= onStart;
            OnStart += onStart;

            return Start(updateBehaviorInstance, onStop);
        }

        public virtual IEnumerator Start(IEnumerator updateBehaviorInstance, Action onComplete, Action onStart)
        {
            OnStart -= onStart;
            OnStart += onStart;

            return Start(updateBehaviorInstance, onComplete);
        }
    }
}