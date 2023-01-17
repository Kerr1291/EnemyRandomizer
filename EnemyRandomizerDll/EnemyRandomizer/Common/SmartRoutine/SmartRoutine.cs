using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace nv
{
    public partial class SmartRoutine : IEnumerator
    {
        protected FieldInfo isDoneField = null;

        static List<SmartRoutine> routines = new List<SmartRoutine>();
        static List<SmartRoutine> Routines
        {
            get
            {
                if(routines == null)
                    routines = new List<SmartRoutine>();
                return routines;
            }
        }
        
        public delegate IEnumerator UpdateBehaviorFunc(params object[] args);
        
        public virtual UpdateBehaviorFunc UpdateBehavior { get; set; }
        protected virtual IEnumerator UpdateBehaviorInstance { get; set; }

        public virtual Action OnStart { get; set; }
        public virtual Action OnComplete { get; set; }
        public virtual Action<bool> OnStop { get; set; }


        public virtual bool IsPaused { get; protected set; }

        public virtual bool IsRunning
        {
            get
            {
                return UpdateBehaviorInstance != null && !IsComplete;
            }
        }

        public virtual bool IsComplete
        {
            get
            {
                bool isDone = false;
                try
                {
                    if(UpdateBehaviorInstance != null)
                    {
                        if (isDoneField == null)
                        {
                            var fields = UpdateBehaviorInstance.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                            foreach (var fi in fields)
                            {
                                if (fi.Name.Contains("$PC"))
                                {
                                    isDoneField = fi;
                                    break;
                                }
                            }

                            //looks like the internal field name changed at some point, but the state field of the enumerable
                            //still seems to be the first one, so we'll just grab that as a fallback for now
                            if (isDoneField == null)
                            {
                                isDoneField = fields[0];
                            }
                        }

                        //use reflection to check if the internal iterator is past the end
                        isDone = ((int)isDoneField.GetValue(UpdateBehaviorInstance) == -1);
                    }
                    else
                    {
                        isDone = true;
                    }
                }
                catch(Exception e)
                {
                    isDone = true;
                    Debug.LogError(e.Message);
                }
                
                return isDone;
            }
        }


        public void Stop(bool resetRoutine = true)
        {
            if(UpdateBehaviorInstance == null)
                return;            

            if(resetRoutine)
            {
                ClearUpdateBehaviorInstance();
            }
            else
            {
                StopUpdateBehaviorInstance();
            }
        }

        protected virtual void TrackRoutine()
        {
            Routines.Add(this);
        }

        protected virtual void UntrackRoutine()
        {
            Routines.Remove(this);
        }

        protected virtual void StopUpdateBehaviorInstance()
        {
            if(UpdateBehaviorInstance != null)
            {
                bool isComplete = IsComplete;
                SmartRoutineHelper.Instance.StopCoroutine(UpdateBehaviorInstance);
                SafeInvoke(OnStop, isComplete);
                if(isComplete)
                {
                    SafeInvoke(OnComplete);
                }
                else
                {
                    IsPaused = true;
                }
                UntrackRoutine();
            }
        }

        protected virtual void ClearUpdateBehaviorInstance()
        {
            IsPaused = false;
            StopUpdateBehaviorInstance();
            UpdateBehaviorInstance = null;
        }

        protected virtual void CreateUptadeBehaviorInstance(params object[] args)
        {
            CreateUptadeBehaviorInstance( UpdateBehavior(args) );
        }

        protected virtual void CreateUptadeBehaviorInstance(IEnumerator updateBehaviorInstance)
        {
            ClearUpdateBehaviorInstance();

            UpdateBehaviorInstance = updateBehaviorInstance;
        }

        protected virtual IEnumerator StartUpdateBehaviorInstance()
        {
            IsPaused = false;
            TrackRoutine();
            SafeInvoke(OnStart);
            SmartRoutineHelper.Instance.StartCoroutine(UpdateBehaviorInstance);
            return UpdateBehaviorInstance;
        }

        protected void SafeInvoke(Action action)
        {
            if(action == null)
                return;

            try
            {
                action.Invoke();
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        protected void SafeInvoke(Action<float> action, float t)
        {
            if(action == null)
                return;
            try
            {
                action.Invoke(t);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        protected void SafeInvoke(Action<bool> action, bool b)
        {
            if(action == null)
                return;
            try
            {
                action.Invoke(b);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public virtual object Current
        {
            get
            {
                if(UpdateBehaviorInstance == null)
                    return null;

                return UpdateBehaviorInstance.Current;
            }
        }

        public virtual bool MoveNext()
        {
            if(UpdateBehaviorInstance == null)
            {
                if(UpdateBehavior != null)
                {
                    Start(false);
                    return true;
                }

                return false;
            }

            if(IsComplete)
            {
                Stop();
                return false;
            }

            if(IsPaused)
                Start(true);

            return true;
        }

        public virtual void Reset()
        {
            Stop();
        }
    }
}