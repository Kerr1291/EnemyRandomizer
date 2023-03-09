using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EnemyRandomizerMod
{
    [ExecuteInEditMode]
    public class RaySensor : MonoBehaviour
    {
        [Header( "Required" )]
        public List<Collider2D> bodyColliders;
        //rays are positioned around the shell
        public Collider2D shell;

        //rays are parented to this root
        public GameObject rayRoot;

        [Header("Settings")]
        public int numRaysBottom = 3;
        public int numRaysLeft = 3;
        public int numRaysRight = 3;
        public int numRaysTop = 3;

        public bool debug = false;

        public UnityEngine.LayerMask layerMask = Physics.DefaultRaycastLayers;

        public float sensorRange = 1f;
        public float extraRangeFactor = .05f;

        [Range(0f,2f)]
        public float raySpacingScalar = 1f;

        [SerializeField]
        List<Raycaster> raysBottom;
        [SerializeField]
        List<Raycaster> raysLeft;
        [SerializeField]
        List<Raycaster> raysRight;
        [SerializeField]
        List<Raycaster> raysTop;

        List<Raycaster> AllRays {
            get {
                return raysBottom.Concat( raysTop ).Concat( raysLeft ).Concat( raysRight ).ToList();
            }
        }

        public bool HasHit {
            get {
                return AllRays.Where( x => x.RaycastHasHit() ).Any();
            }
        }

        void OnValidate()
        {
            if( shell == null )
                shell = GetComponent<Collider2D>();

            if( shell == null )
                return;

            if( rayRoot == null )
                return;

            Setup();
        }

        void Start()
        {
            Setup();
        }

        void Update()
        {
            if( Application.isEditor )
            {
                CleanOrphanRays();
                ConfigureRays();
            }
        }

        void Setup()
        {
            //setup rays
            GenerateRays();

            ConfigureRays();
        }

        void CleanChildRays()
        {
            foreach( Raycaster r in rayRoot.GetComponentsInChildren<Raycaster>() )
            {
                if( Application.isEditor && !Application.isPlaying )
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () => { if( r != null ) DestroyImmediate( r.gameObject ); };
#endif
                }
                else
                {
                    Destroy( r.gameObject );
                }
            }
        }

        void CleanOrphanRays()
        {
            var allRays = AllRays;

            foreach( Raycaster r in rayRoot.GetComponentsInChildren<Raycaster>() )
            {
                if(!allRays.Contains(r))
                {
                    if( Application.isEditor && !Application.isPlaying )
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.delayCall += () => { if( r != null ) DestroyImmediate( r.gameObject ); };
#endif
                    }
                    else
                    {
                        Destroy( r.gameObject );
                    }
                }
            }
        }

        void GenerateRays()
        {
            CleanChildRays();

            GenerateRays( ref raysBottom, numRaysBottom, Vector2.down, nameof(raysBottom));
            GenerateRays( ref raysLeft, numRaysLeft, Vector2.left, nameof( raysLeft ) );
            GenerateRays( ref raysRight, numRaysRight, Vector2.right, nameof( raysRight ) );
            GenerateRays( ref raysTop, numRaysTop, Vector2.up, nameof( raysTop ) );
        }

        void ConfigureRays()
        {
            List<Collider2D> optimizationColliders = rayRoot.GetComponentsInChildren<Collider2D>().ToList();

            foreach( Raycaster r in AllRays )
            {
                r.is2DRaycast = true;
                r.debug = debug;
                r.UseBoxColliderCache = true;
                r.layerMask = layerMask;

                r.distance = sensorRange;
                r.boxColliderExtraFactor = extraRangeFactor;

                if( r.ignoreList2D == null )
                    r.ignoreList2D = new List<Collider2D>();

                r.ignoreList2D.Clear();
                r.ignoreList2D.Add( shell );
                r.ignoreList2D = r.ignoreList2D.Concat( bodyColliders ).Concat( optimizationColliders ).ToList();
            } 
        }

        IEnumerator EditorDestroy( GameObject go )
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate( go );
        }

        void GenerateRays( ref List<Raycaster> rays, int count, Vector2 direction, string name )
        {
            if(rays != null)
            {
                foreach(Raycaster r in rays)
                {
                    if( r == null )
                        continue;

                    if( Application.isEditor && !Application.isPlaying )
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.delayCall += () => { if( r != null ) DestroyImmediate( r.gameObject ); };
#endif
                    }
                    else
                    {
                        Destroy( r.gameObject );
                    }
                }
            }

            rays = new List<Raycaster>();

            Vector2 shellSize = Vector2.zero;
            shellSize.x = shell.bounds.extents.x;
            shellSize.y = shell.bounds.extents.y;

            for(int i = 0; i < count; ++i )
            {
                GameObject rayObject = new GameObject($"Raycaster - {name}: {i}");

                Raycaster ray = rayObject.AddComponent<Raycaster>();

                float positionAlongEdge = (float)i/((float)(count-1));

                PositionRay( direction, shellSize, ray, rayRoot, positionAlongEdge );

                rays.Add( ray );
            }
        }

        void PositionRay( Vector2 direction, Vector2 shellSize, Raycaster ray, GameObject root, float position )
        {
            ray.transform.SetParent( root.transform );
            ray.transform.localPosition = Vector3.zero;

            Vector2 dir = direction.normalized;
            Vector2 extent = Mathf.Abs(Vector2.Dot(shellSize, dir)) * dir;

            Vector2 dirPerp = Vector2.Perpendicular(dir);
            Vector2 extentPerp = Mathf.Abs(Vector2.Dot(shellSize, dirPerp)) * dirPerp;

            Vector2 origin = extent + extentPerp * (position * 2f - 1f) * raySpacingScalar;

            ray.transform.localPosition = origin;
            ray.transform.up = dir;
        }
    }
}