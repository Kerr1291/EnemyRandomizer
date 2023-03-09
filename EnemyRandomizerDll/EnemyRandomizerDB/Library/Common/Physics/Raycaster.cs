using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Raycaster : MonoBehaviour
    {
        [Header("If enabled will update every frame (may cause performance issues)")]
        [SerializeField]
        public bool checkEachFrame = false;
        [Header("If enabled show the current state of the caster)")]
        [SerializeField]
        public bool debug = false;
        Color debugColor = Color.white;

        [SerializeField]
        Color debugHitColor = Color.green;
        [SerializeField]
        Color debugNoHitColor = Color.red;

        public float distance = 1.0f;

        public bool updateOnAwake = true;

        [Header("Use y as forward?")]
        public bool is2DRaycast = false;

        public bool use2DTriggersOnly = false;
        List<Transform> _2dTriggers = new List<Transform>();

        [Header("What to check against when casting")]
        public UnityEngine.LayerMask layerMask = Physics.DefaultRaycastLayers;

        [Header("(For 3D) Hit triggers?")]
        public QueryTriggerInteraction triggerInteraction;

        bool _frame_cached = false;

        List<Transform> _hits = new List<Transform>();

        [Header("Optimize by using a box collider?")]
        [SerializeField]
        bool useBoxColliderCache = false;

        public bool UseBoxColliderCache {
            get {
                return useBoxColliderCache;
            }
            set {
                useBoxColliderCache = value;
                SyncBoxColliderForOptimization();
            }
        }

        

        //bool _box_cached = false;
        bool raycastDirty = false;

        public List<Collider> ignoreList;
        public List<Collider2D> ignoreList2D;

        //TODO: fix this, would be a good optimization
        //[Header("(For 3D) Only get the first hit?")]
        bool useOneHit3DOptimization = false;

        [SerializeField,HideInInspector]
        BoxCollider2D _box2d;
        [SerializeField,HideInInspector]
        BoxCollider _box;

        public float boxColliderExtraFactor = 0.01f;

        //TimedRoutine raycastChecker;

        void Reset()
        {
            //set the raycaster layer to the layer of a collider in the child component
            gameObject.layer = GetComponentInParent<Rigidbody2D>().GetComponentInChildren<Collider2D>().gameObject.layer;
        }

        //Creates a temporary raycaster that will destroy itself after a short while
        public static Raycaster CreateTempRaycaster()
        {
            GameObject r_obj = new GameObject("Temp Raycaster");
            Raycaster ray = r_obj.AddComponent<Raycaster>();
            TimedRoutine time = new TimedRoutine(.5f);
            //destroy the object when the timer runs out
            time.Start(() => { GameObject.Destroy(r_obj); });
            return ray;
        }

        public bool RaycastHasHit()
        {
            DoRaycast();

            return _hits.Count > 0;
        }

        public List<Transform> Raycast()
        {
            DoRaycast();
            return _hits;
        }

        void DoRaycast2D()
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(CastRay2DOrigin, CastRay2DDirection, distance, layerMask.value);

            for(int i = 0; i < hits.Length; ++i)
            {
                if(hits[i].transform.GetComponent<Raycaster>() == null)
                {
                    if( ignoreList2D.Contains( hits[ i ].collider ) )
                        continue;

                    if(debug)
                        Debug.Log("Raycaster " + gameObject.name + " hit " + hits[i].transform.gameObject.name);
                    _hits.Add(hits[i].transform);
                }
            }

            if(use2DTriggersOnly)
            {
                foreach(Transform t in _2dTriggers)
                {
                    if(_hits.Contains(t) == false)
                        _hits.Add(t);
                }
            }
        }

        void DoRaycast3D()
        {
            RaycastHit[] new_hits = new RaycastHit[1];

            if(useOneHit3DOptimization && _box != null)
            {
                RaycastHit hit = new RaycastHit();
                if(_box.Raycast(CastRay, out hit, distance))
                    new_hits[0] = hit;
                else
                    new_hits = new RaycastHit[0];
            }
            else
            {
                new_hits = Physics.RaycastAll(CastRay.origin, CastRay.direction, distance, layerMask, triggerInteraction);
            }

            for(int i = 0; i < new_hits.Length; ++i)
            {
                if(new_hits[i].collider.GetComponent<Raycaster>() == null)
                {
                    if(ignoreList.Contains(new_hits[i].collider))
                        continue;

                    _hits.Add(new_hits[i].transform);
                }
            }
        }

        void DoRaycast()
        {
            //if(debug)
            //    Debug.Log("_frame_cached: " + _frame_cached);

            if(_frame_cached == true)
                return;

            //if(debug)
            //    Debug.Log("_box_cached: " + _box_cached);

            if(useBoxColliderCache && !raycastDirty)
                return;

            _hits.Clear();

            if(is2DRaycast)
            {
                DoRaycast2D();
            }
            else
            {
                DoRaycast3D();
            }

            if( _hits.Count > 0 )
            {
                raycastDirty = false;
            }

            if(debug)
            {
                if(_hits.Count > 0)
                    debugColor = debugHitColor;
                else
                    debugColor = debugNoHitColor;
            }

            _frame_cached = true;

            //if(useBoxColliderCache && _box_cached == false)
            //    _box_cached = true;
        }

        void SyncBoxColliderForOptimization()
        {
            if( !useBoxColliderCache )
            {
                if( _box2d != null )
                {
                    Destroy( _box2d );
                    _box2d = null;
                }
                else if( _box != null )
                {
                    Destroy( _box );
                    _box = null;
                }
            }
            else
            {
                if( is2DRaycast )
                {
                    if( _box2d == null )
                        _box2d = gameObject.AddComponent<BoxCollider2D>();

                    _box2d.size = new Vector2( 0.05f, ( distance + boxColliderExtraFactor ) );
                    _box2d.offset = new Vector2( 0.0f, ( distance + boxColliderExtraFactor ) * 0.5f );
                    _box2d.isTrigger = true;
                }
                else
                {
                    if( _box == null )
                        _box = gameObject.AddComponent<BoxCollider>();

                    _box.size = new Vector3( 0.05f, 0.05f, ( distance + boxColliderExtraFactor ) );
                    _box.center = new Vector3( 0.0f, 0.0f, ( distance + boxColliderExtraFactor ) * 0.5f );
                    _box.isTrigger = true;
                }
            }

        }

        void Awake()
        {
            SyncBoxColliderForOptimization();

            //raycastChecker = new TimedRoutine(.1f, () => { DoRaycast(); });

            if(updateOnAwake)
                DoRaycast();
        }

        void Update()
        {
            _frame_cached = false;

            if( !useBoxColliderCache )
            {
                if( checkEachFrame )
                    DoRaycast();
            }
            else
            {
                if( raycastDirty )
                    DoRaycast();
            }

            if(debug)
                Debug.DrawLine(DebugStartPoint, DebugEndPoint, debugColor);
        }

        Ray _casting_ray = new Ray();
        Ray CastRay
        {
            get
            {
                _casting_ray.origin = transform.position;
                _casting_ray.direction = Forward;
                return _casting_ray;
            }
        }

        Vector2 CastRay2DOrigin
        {
            get
            {
                return new Vector2(CastRay.origin.x, CastRay.origin.y);
            }
        }

        Vector2 CastRay2DDirection
        {
            get
            {
                return new Vector2(CastRay.direction.x, CastRay.direction.y);
            }
        }

        Vector3 Forward
        {
            get
            {
                if(is2DRaycast)
                    return transform.up;
                else
                    return transform.forward;
            }
        }

        Vector3 DebugStartPoint
        {
            get
            {
                if(is2DRaycast)
                    return CastRay2DOrigin;

                return CastRay.origin;
            }
        }

        Vector3 DebugEndPoint
        {
            get
            {
                if(is2DRaycast)
                    return CastRay2DOrigin + CastRay2DDirection * distance;

                return CastRay.origin + CastRay.direction * distance;
            }
        }

        void OnCollisionEnter(Collision other)
        {
            raycastDirty = useBoxColliderCache && true;
            UpdateRaycaster(other.collider);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            raycastDirty = useBoxColliderCache && true;
            UpdateRaycaster(other.collider);
        }

        void OnTriggerEnter(Collider other)
        {
            raycastDirty = useBoxColliderCache && true;
            UpdateRaycaster(other);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            raycastDirty = useBoxColliderCache && true;

            if(!ignoreList2D.Contains(other))
            {
                if(use2DTriggersOnly)
                    _2dTriggers.Add(other.transform);
            }
            if(debug)
                Debug.Log(gameObject.name + " hit " + other.gameObject.name + " entered");
            UpdateRaycaster(other);
        }

        void OnTriggerExit(Collider other)
        {
            raycastDirty = useBoxColliderCache && true;
            UpdateRaycaster(other);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            raycastDirty = useBoxColliderCache && true;
            if(!ignoreList2D.Contains(other))
            {
                if(use2DTriggersOnly)
                    _2dTriggers.Remove(other.transform);
            }
            if(debug)
                Debug.Log(gameObject.name + " hit " + other.gameObject.name + " exited");
            UpdateRaycaster(other);
        }

        void UpdateRaycaster(Collider other)
        {
            if(ignoreList.Contains(other))
                return;

            //if(!is2DRaycast && useBoxColliderCache)
            //    _box_cached = false;

            DoRaycast();
        }

        void UpdateRaycaster(Collider2D other)
        {
            if(ignoreList2D.Contains(other))
                return;

            //if(is2DRaycast && useBoxColliderCache)
            //    _box_cached = false;

            DoRaycast();
        }
    }
}