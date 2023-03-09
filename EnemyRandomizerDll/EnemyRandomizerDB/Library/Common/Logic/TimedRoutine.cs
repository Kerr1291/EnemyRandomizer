using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{

    public class TimedRoutine : SmartRoutine
    {
        protected virtual void SetDefaults()
        {
            UpdateBehavior = TimedUpdateBhavior;
            UpdateRate = () => { return UnityEngine.Time.deltaTime; };
            YieldBehavior = () => { return new WaitForEndOfFrame(); };
            Looping = false;
        }

        public virtual Func<float> UpdateRate { get; set; }
        public virtual Func<YieldInstruction> YieldBehavior { get; set; }

        public virtual Action<float> OnLerp { get; set; }
        public virtual Action OnLoop { get; set; }

        public virtual bool Looping { get; set; }
        public virtual float Length { get; set; }
        public virtual float Time { get; private set; }

        public virtual float TimeNormalized
        {
            get
            {
                return Time / Length;
            }
        }

        public virtual float TimeRemaining
        {
            get
            {
                return Length - Time;
            }
        }

        public virtual float TimeRemainingNormalized
        {
            get
            {
                return TimeRemaining / Length;
            }
        }

        public virtual IEnumerator TimedUpdateBhavior(params object[] args)
        {
            Dev.Where();
            do
            {
                Time = 0f;

                while(Time < Length)
                {
                    SafeInvoke(OnLerp, TimeNormalized);
                    Time += UpdateRate();
                    yield return YieldBehavior();
                }

                Time = Length;
                SafeInvoke(OnLerp, TimeNormalized);

                if(Looping)
                    SafeInvoke(OnLoop);
            }
            while(Looping);
        }

        protected override void ClearUpdateBehaviorInstance()
        {
            Time = 0f;
            base.ClearUpdateBehaviorInstance();
        }

        public TimedRoutine(float length = 1f)
        {
            Length = length;
            SetDefaults();
        }

        public TimedRoutine(float length, Action onComplete)
        {
            SetDefaults();
            OnComplete -= onComplete;
            OnComplete += onComplete;
            Length = length;
        }

        public TimedRoutine(float length, Action<bool> onStop)
        {
            SetDefaults();
            OnStop -= onStop;
            OnStop += onStop;
            Length = length;
        }


        public TimedRoutine(float length, Action<bool> onStop, Action onStart)
        {
            SetDefaults();
            OnStart -= onStart;
            OnStart += onStart;
            OnStop -= onStop;
            OnStop += onStop;
            Length = length;
        }

        public TimedRoutine(float length, Action onComplete, Action onStart)
        {
            SetDefaults();
            OnStart -= onStart;
            OnStart += onStart;
            OnComplete -= onComplete;
            OnComplete += onComplete;
            Length = length;
        }

        /// <summary>
        /// Primary Start() method. All overrides call into this
        /// </summary>
        /// <param name="tryResume"></param>
        /// <param name="args"></param>
        public override IEnumerator Start(bool tryResume, params object[] args)
        {
            if(args.Length <= 0)
                return base.Start(tryResume, args);

            Length = (float)args[0];
            List<object> argsList = new List<object>(args);
            argsList.RemoveAt(0);
            return base.Start(tryResume, argsList.ToArray());
        }

        public override bool MoveNext()
        {
            if(UpdateBehaviorInstance == null)
            {
                if(UpdateBehavior != null)
                {
                    Start(false, Length);
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
                Start(true, Length);

            return true;
        }
    }
}