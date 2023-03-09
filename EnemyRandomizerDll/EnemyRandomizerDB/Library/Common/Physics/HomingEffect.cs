using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    public class HomingEffect : MonoBehaviour
    {
        public enum ArriveBehaviour
        {
            DisableAndReset,
            Disable,
            Loop,
            Destroy,
            Custom
        }

        [Tooltip("Will be used instead of by targetPoint if this is not null")]
        public Transform target;
        [Tooltip("Will be overridden by targetPosition if targetPosition is not null")]
        public Vector3 targetPoint;

        [Tooltip("Time it takes to reach full acceleration")]
        public float startupTime = 1f;

        [Tooltip("The rate at which it reaches full acceleration")]
        public AnimationCurve startupRate;

        [Tooltip("The magnitude to cap the velocity at")]
        public float maxVelocity = 100f;

        [Tooltip("[X-Axis: The (normalized) distance from the START to the TARGET] [Y-Axis: The acceleration magnitude]")]
        public AnimationCurve accelerationOverDistance;

        [Tooltip("The velocity to start with on enable")]
        public Vector3 initialVelocity = Vector3.zero;

        [Tooltip("How close the effect needs to be before it is considered to have arrived at the target")]
        public float arrivedDistance = .1f;

        [Tooltip("How close the effect needs to be before no longer applies acceleration and instead just moves to the target. NOTE: This should == approximately sqrt(maxVelocity) if your conditions are such that the homing object may pass the target. If this value is too small (and the arrived distance is also small) it may never reach the target, causing it to orbit forever.")]
        public float forceMoveToDistance = 0f;

        [Range(0f,1f), Tooltip("The strength of the course correction when finally moving to the target. Values closer to 1f will create a sharper, more angular, turn")]
        public float forceMoveToStrength = .25f;

        [Tooltip("The behavior upon reaching the target")]
        public ArriveBehaviour arriveBehaviour = ArriveBehaviour.DisableAndReset;

        [Tooltip("If the calculations should use local or world space positions")]
        public bool isLocalSpace = false;

        [Serializable]
        public class Events
        {
            public UnityEvent onEnable;
            public UnityEvent onArrive;
            public UnityEvent onArriveCustom;
            public UnityEvent onDisable;
            public UnityEvent onLoop;
            public Action onUpdate;
            public Action<HomingEffect> onUpdateWithThis;
        }

        public Events events;

        protected Vector3 start;
        protected float time;
        protected bool arrived;

        Vector3 velocity;

        public virtual float TravelTime
        {
            get
            {
                return time;
            }
        }

        public virtual Vector3 Target
        {
            get
            {
                if(target != null)
                {
                    if(isLocalSpace)
                        targetPoint = target.transform.localPosition;
                    else
                        targetPoint = target.transform.position;
                }
                return targetPoint;
            }
        }

        public virtual Vector3 Position
        {
            get
            {
                if(isLocalSpace)
                    return transform.localPosition;
                else
                    return transform.position;
            }
            set
            {
                if(isLocalSpace)
                    transform.localPosition = value;
                else
                    transform.position = value;
            }
        }

        public virtual Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        public virtual Vector3 Start
        {
            get
            {
                return start;
            }
        }

        public virtual Vector3 VectorToTarget
        {
            get
            {
                return (Target - Position);
            }
        }

        public virtual Vector3 OriginVectorToTarget
        {
            get
            {
                return (Target - Start);
            }
        }

        public virtual Vector3 DirectionToTarget
        {
            get
            {
                return VectorToTarget.normalized;
            }
        }

        public virtual float DistanceToTarget
        {
            get
            {
                return VectorToTarget.magnitude;
            }
        }

        public virtual float DistanceToTargetSq
        {
            get
            {
                return VectorToTarget.sqrMagnitude;
            }
        }

        public virtual float ArrivedDistanceSq
        {
            get
            {
                return arrivedDistance * arrivedDistance;
            }
        }

        public virtual float ForceMoveToDistanceSq
        {
            get
            {
                return forceMoveToDistance * forceMoveToDistance;
            }
        }

        public virtual float OriginDistanceToTargetSq
        {
            get
            {
                return OriginVectorToTarget.sqrMagnitude;
            }
        }

        public virtual float NormalizedDistanceToTarget
        {
            get
            {
                return DistanceToTargetSq / OriginDistanceToTargetSq;
            }
        }

        public virtual float AccelerationRate
        {
            get
            {
                float dist = NormalizedDistanceToTarget;
                if(dist <= 0f || float.IsNaN(dist))
                    return 0f;

                float startupFactor = 1f;
                if(startupTime >= 0f)
                {
                    float startupProgress = Mathf.Clamp01(time / startupTime);
                    startupFactor = startupRate.Evaluate(startupProgress);
                }

                float acceleration = accelerationOverDistance.Evaluate(dist);
                if(float.IsNaN(acceleration) || float.IsNaN(startupFactor))
                    return 0f;
                return acceleration * startupFactor;
            }
        }

        protected virtual void Reset()
        {
            if(startupRate == null)
                startupRate = new AnimationCurve();

            if(accelerationOverDistance == null)
                accelerationOverDistance = new AnimationCurve();

            startupRate.AddKey(0f, 0f);
            startupRate.AddKey(0.5f, 0.5f);
            startupRate.AddKey(1f, 1f);

            accelerationOverDistance.AddKey(0f, 200f);
            accelerationOverDistance.AddKey(.4f, 50f);
            accelerationOverDistance.AddKey(1f, 100f);

            forceMoveToDistance = Mathf.Sqrt(maxVelocity);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            startupRate.FixEndpoints(new Vector2(0f, 0f), new Vector2(1f, 1f));
            startupRate.Normalize();
            accelerationOverDistance.Normalize(true, false);
        }
#endif

        protected virtual void OnEnable()
        {
            Setup();
            events.onEnable.Invoke();
        }

        protected virtual void OnDisable()
        {
            Position = start;
            events.onDisable.Invoke();
        }

        protected virtual void Arrive()
        {
            if(arriveBehaviour == ArriveBehaviour.DisableAndReset)
            {
                Position = start;
                Setup();
                gameObject.SetActive(false);
            }
            else if(arriveBehaviour == ArriveBehaviour.Disable)
            {
                gameObject.SetActive(false);
            }
            else if(arriveBehaviour == ArriveBehaviour.Loop)
            {
                Loop();
            }
            else if(arriveBehaviour == ArriveBehaviour.Destroy)
            {
                Destroy(gameObject);
            }
            else if(arriveBehaviour == ArriveBehaviour.Custom)
            {
                events.onArriveCustom.Invoke();
            }
            arrived = false;
        }

        protected virtual void Setup()
        {
            arrived = false;
            start = Position;
            time = 0f;
            Velocity = initialVelocity;
            CapVelocity();
        }

        protected virtual void Step(Vector3 step)
        {
            Velocity += step;
            Position += Velocity;
        }

        public virtual Vector3 GetStep(float dt)
        {
            return DirectionToTarget * AccelerationRate * dt;
        }

        protected virtual void CapVelocity()
        {
            if(Velocity.magnitude > maxVelocity)
            {
                var vdir = Velocity.normalized;
                Velocity = vdir * maxVelocity;
            }
        }

        protected virtual void Loop()
        {
            Position = start;
            Setup();
            events.onLoop.Invoke();
        }

        protected virtual void ForceMoveToTarget()
        {
            Velocity = Velocity.normalized * Velocity.magnitude * (1f - forceMoveToStrength) + DirectionToTarget * Velocity.magnitude * forceMoveToStrength;
        }

        protected virtual void Update()
        {
            if(arrived)
            {
                Arrive();
            }
            else
            {
                float distSq = DistanceToTargetSq;
                if(distSq < ArrivedDistanceSq)
                {
                    arrived = true;
                    events.onArrive.Invoke();
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if(arrived)
                return;
            
            float dt = Time.fixedDeltaTime * Time.timeScale;
            time += dt;

            float distSq = DistanceToTargetSq;
            if(distSq < ArrivedDistanceSq || float.IsNaN(distSq) || distSq <= 0f)
            {
                return;
            }
            else if(distSq < ForceMoveToDistanceSq)
            {
                ForceMoveToTarget();
            }

            Step(GetStep(dt));
            CapVelocity();
        }

        protected virtual void LateUpdate()
        {
            if(arrived)
                return;

            if(events.onUpdate != null)
                events.onUpdate.Invoke();
            if(events.onUpdateWithThis != null)
                events.onUpdateWithThis.Invoke(this);
        }

        [SerializeField]
        protected Color debugVelocityColor = Color.green;

        [SerializeField]
        protected Color debugAccelerationColor = Color.red;

        protected virtual void OnDrawGizmosSelected()
        {
            Color c = Gizmos.color;

            if(Application.isPlaying)
            {
                Gizmos.color = debugVelocityColor;
                Gizmos.DrawLine(transform.position, transform.position + Velocity);
            }

            if(Application.isPlaying)
            {
                Gizmos.color = debugAccelerationColor;
                if(DistanceToTargetSq < ForceMoveToDistanceSq)
                    Gizmos.color = Color.magenta;

                Vector3 acceleration = GetStep(Time.fixedDeltaTime * Time.timeScale);

                Gizmos.DrawLine(transform.position, transform.position + acceleration);
            }

            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, forceMoveToDistance);
            }

            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, arrivedDistance);
            }

            Gizmos.color = c;
        }
    }
}
