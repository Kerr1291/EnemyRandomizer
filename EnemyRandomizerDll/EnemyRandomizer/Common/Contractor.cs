using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace nv
{
    public class ContractorManager : GameSingleton<ContractorManager>
    {
        Dictionary<Contractor,IEnumerator> contractors = new Dictionary<Contractor,IEnumerator>();

        public List<Contractor> ActiveContractors {
            get {
                return new List<Contractor>( contractors.Keys );
            }
        }

        public void Add( Contractor contractor, IEnumerator main )
        {
            if( contractors.ContainsKey( contractor ) )
                return;
            contractors.Add( contractor, main );
        }

        public void Remove( Contractor contractor )
        {
            if( !contractors.ContainsKey( contractor ) )
                return;
            contractors.Remove( contractor );
        }

        public void BeginContract( Contractor contractor )
        {
            if( !contractors.ContainsKey( contractor ) )
                return;
            StartCoroutine( contractors[ contractor ] );
        }

        public void EndContract( Contractor contractor )
        {
            if( !contractors.ContainsKey( contractor ) )
                return;
            StopCoroutine( contractors[ contractor ] );
        }

        public bool IsActive( Contractor action )
        {
            return ( contractors.ContainsKey( action ) );
        }
    }

    public class Contractor
    {
        public enum UpdateRateType
        {
            Fixed
            , Frame
            , Custom
        }

        UpdateRateType updateRateType;
        YieldInstruction updateYieldType;
        float updateRate;

        public virtual System.Action OnStart { get; set; }
        public virtual System.Action OnUpdate { get; set; }
        public virtual System.Action<float> OnLerp { get; set; }
        public virtual System.Action OnComplete { get; set; }
        public virtual System.Action OnLoop { get; set; }
        public virtual bool Looping { get; set; }

        public virtual float Duration { get; set; }

        public virtual float CurrentTime { get; private set; }

        public virtual float NormalizedCurrentTime {
            get {
                return CurrentTime / Duration;
            }
        }

        public virtual float TimeRemaining {
            get {
                return Duration - CurrentTime;
            }
        }

        public virtual float NormalizedTimeRemaining {
            get {
                return TimeRemaining / Duration;
            }
        }

        public virtual bool IsActive {
            get {
                return ContractorManager.Instance != null && ContractorManager.Instance.IsActive( this );
            }
        }

        public void SetUpdateRate( UpdateRateType type )
        {
            updateRateType = type;
            if( updateRateType == UpdateRateType.Fixed )
            {
                updateYieldType = new WaitForFixedUpdate();
                updateRate = UnityEngine.Time.fixedDeltaTime;
            }
            else if( updateRateType == UpdateRateType.Frame )
            {
                updateYieldType = new WaitForEndOfFrame();
                updateRate = UnityEngine.Time.deltaTime;
            }
            else
            {
                //custom makes no assumptions on what the user wants
            }
        }

        public void SetCustomUpdateRate( float customRate )
        {
            updateRate = customRate;
            updateRateType = UpdateRateType.Custom;
            updateYieldType = new WaitForSeconds( customRate );
        }

        public void SetCustomYieldType( YieldInstruction yeildType )
        {
            updateRateType = UpdateRateType.Custom;
            updateYieldType = yeildType;
        }

        public void Start()
        {
            if( IsActive || CurrentTime > 0f )
                return;

            if( ContractorManager.Instance == null )
                return;

            ContractorManager.Instance.Add( this, Main() );
            InvokeAction( OnStart );
            ContractorManager.Instance.BeginContract( this );
        }

        public void Complete()
        {
            if( IsActive )
                InvokeAction( OnComplete );
            Reset();
        }

        public void Reset()
        {
            if( ContractorManager.Instance == null )
                return;

            ContractorManager.Instance.EndContract( this );
            ContractorManager.Instance.Remove( this );
            CurrentTime = 0f;
        }

        IEnumerator Main()
        {
            do
            {
                CurrentTime = 0f;

                while( CurrentTime < Duration )
                {
                    InvokeAction( OnUpdate );
                    InvokeAction( OnLerp, NormalizedCurrentTime );
                    CurrentTime += updateRate;
                    yield return updateYieldType;
                }

                CurrentTime = Duration;
                InvokeAction( OnUpdate );
                InvokeAction( OnLerp, NormalizedCurrentTime );

                if( Looping )
                    InvokeAction( OnLoop );
            }
            while( Looping );
            Complete();
        }

        public Contractor()
        {
            updateRateType = UpdateRateType.Fixed;
            updateYieldType = new WaitForFixedUpdate();
            updateRate = .02f;
            Duration = 1f;
        }

        public Contractor( System.Action onComplete, float duration = 1f )
        {
            updateRateType = UpdateRateType.Fixed;
            updateYieldType = new WaitForFixedUpdate();
            updateRate = .02f;

            OnComplete = onComplete;
            Duration = duration;
        }

        static void InvokeAction( System.Action action )
        {
            if( action == null )
                return;
            action.Invoke();
        }

        static void InvokeAction( System.Action<float> action, float t )
        {
            if( action == null )
                return;
            action.Invoke( t );
        }
    }
}