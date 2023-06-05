using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System;

namespace EnemyRandomizerMod
{
    public partial class PreventInsideWalls : MonoBehaviour
    {
#if DEBUG
        public static bool VERBOSE_DEBUGGING = true;
#else
        public static bool VERBOSE_DEBUGGING = false;
#endif



        static List<string> groundOrPlatformName = new List<string>()
            {
                "Chunk",
                "Roof",
                "Platform",
                "plat_",
                "Colosseum Wall"
            };

        Vector2 currentLocation => transform.position.ToVec2();
        Vector2 heroLocation => HeroController.instance.transform.position.ToVec2() + Vector2.up * 0.5f;
        Vector2 toHero => (heroLocation - currentLocation);
        Vector2 toHeroDir => (heroLocation - currentLocation).normalized;

        public LayerMask boundsLayer = (1 << 8);

        FieldInfo raycast2Dfield;

        int frameThreshold = 10;//check every 10 frames
        int framesInsideWall = 6;//how many frame checks it should be inside a wall before attempting resolution
        int insideWallCounter = 0;

        public List<RaycastHit2D> edgeIntersections = new List<RaycastHit2D>();

#if DEBUG
        public int maxDebugRays = 3;
        bool isBuggyEdgeCase = false;
#endif

        List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();
        List<Vector2> edgeCollisionPoints = new List<Vector2>();

        List<RaycastHit2D> resolutionHits = new List<RaycastHit2D>();

        List<RaycastHit2D> GetEdgeIntersections(Vector2 from, Vector2 to, RaycastHit2D sourceHit, EdgeCollider2D eCollider)
        {
            List<RaycastHit2D> edgeIntersections = new List<RaycastHit2D>();
            edgeCollisionPoints.Clear();
            LineSegment2D testLine = new LineSegment2D() { a = from, b = to };
            var testDir = testLine.dir.normalized;
            var length = (to - from).magnitude;

            List<LineSegment2D> edges = new List<LineSegment2D>();
            int points = eCollider.GetPoints(edgeCollisionPoints);
            for(int i = 1; i < edgeCollisionPoints.Count; ++i)
            {
                var a = edgeCollisionPoints[i - 1];
                var b = edgeCollisionPoints[i];
                edges.Add(new LineSegment2D() { a = a, b = b });
            }

            if(raycast2Dfield == null)
                raycast2Dfield = typeof(RaycastHit2D).GetField("m_Collider", BindingFlags.NonPublic | BindingFlags.Instance);

            int sourceHitColliderHandle = (int)raycast2Dfield.GetValue(sourceHit);


            for (int i = 0; i < edges.Count; ++i)
            {
                if(Intersects(testLine, edges[i], out var intersectionPoint, out var intersectionNormal))
                {
                    float dist = (intersectionPoint - from).magnitude;
                    var norm = intersectionNormal;

                    //if the generated edge normal is away from our direction, flip it
                    float dotProduct = Vector2.Dot(testDir, intersectionNormal);
                    if (dotProduct < 0f)
                        norm = -norm;

                    var newHit = new RaycastHit2D()
                    {
                        centroid = sourceHit.centroid,
                        distance = dist,
                        fraction = length > 0f ? dist / length : 0f,
                        normal = norm,
                        point = intersectionPoint,
                    };

                    //connect the raycast hit to the source collider
                    raycast2Dfield.SetValue(newHit, sourceHitColliderHandle);

                    edgeIntersections.Add(newHit);
                }
            }

            return edgeIntersections.OrderBy(x => x.distance).ToList();
        }

        bool Intersects(LineSegment2D line0, LineSegment2D line1, out Vector2 intersectionPoint, out Vector2 intersectionNormal)
        {
            intersectionPoint = Vector2.zero;
            intersectionNormal = Vector2.zero;

            Vector2 distBetweenStarts = line1.a - line0.a;
            Vector2 dir0 = line0.dir;
            Vector2 dir1 = line1.dir;
            float denominator = dir0.x * dir1.y - dir0.y * dir1.x;

            if (Mathnv.FastApproximately(denominator, 0f, 0.01f))
            {
                // The line segments are parallel or coincident, no intersection
                return false;
            }

            float t = (distBetweenStarts.x * dir1.y - distBetweenStarts.y * dir1.x) / denominator;
            float u = (distBetweenStarts.x * dir0.y - distBetweenStarts.y * dir0.x) / denominator;

            if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
            {
                // Intersection point lies within both line segments
                intersectionPoint = line0.a + dir0 * t;

                // Calculate intersection normal
                Vector2 normalDir = new Vector2(-dir0.y, dir0.x).normalized;
                intersectionNormal = normalDir;

                return true;
            }

            // No intersection point within the line segments
            return false;
        }


        private void LateUpdate()
        {
            insideWallCounter = (insideWallCounter + 1) % 10;
            if (insideWallCounter != 0)
            {
                return;
            }

            //do this before checking anything to make sure the object is really stuck in a wall
            var poob = gameObject.GetComponent<PreventOutOfBounds>();
            if (poob != null)
                poob.ResolveMovement();

#if DEBUG
            isBuggyEdgeCase = false;
#endif
            raycastHits.Clear();
            edgeIntersections.Clear();

            raycastHits = Physics2D.RaycastAll(currentLocation, toHeroDir, toHero.magnitude, Physics2D.AllLayers).Where(x => x.collider != null && groundOrPlatformName.Any(y => x.collider.gameObject.name.Contains(y))).ToList();

            if (raycastHits.Count <= 0)
                return;

            try
            {
                if (IsInsidePolygon() ||
                    IsInsideWorldEdgeWall() ||
                    IsInsideBox() ||
                    IsInsideCircle())
                {
                    framesInsideWall++;
                    if (framesInsideWall >= frameThreshold)
                    {
                        ResolveInsideWall(currentLocation, heroLocation, toHeroDir);
                        framesInsideWall = 0;
                    }
                }
                else
                {
                    framesInsideWall = 0;
                }
            }
            catch(Exception e)
            {
                Dev.Log($"Caught checking for inside walls or resolving inside walls... ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }
        }

        private bool IsInsideCircle()
        {
            bool isInsideWall = false;
            var circleHits = raycastHits.Where(x => x.collider is CircleCollider2D).Select(x => x.collider as CircleCollider2D);
            if (circleHits.Any(x => x.OverlapPoint(currentLocation)))
            {
                isInsideWall = true;
#if DEBUG
                try
                {
                    if (VERBOSE_DEBUGGING)
                        Dev.Log($"{gameObject.GetSceneHierarchyPath()} was inside a circle {circleHits.FirstOrDefault(x => x.OverlapPoint(currentLocation)).gameObject.GetSceneHierarchyPath()}");

                    foreach (var e in circleHits)
                    {
                        if (e.OverlapPoint(currentLocation))
                            Dev.Log($"{e.gameObject.GetSceneHierarchyPath()} is a circle that was hit!");
                    }
                }
                catch (Exception e)
                {
                    Dev.Log($"Caught exception in debug printing... ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
#endif
            }

            if(isInsideWall)
                resolutionHits = raycastHits.Where(x => x.collider is CircleCollider2D && (x.collider as CircleCollider2D).OverlapPoint(currentLocation)).ToList();

            return isInsideWall;
        }

        private bool IsInsideBox()
        {
            bool isInsideWall = false;
            var boxHits = raycastHits.Where(x => x.collider is BoxCollider2D).Select(x => x.collider as BoxCollider2D);
            if (boxHits.Any(x => x.OverlapPoint(currentLocation)))
            {
                isInsideWall = true;
#if DEBUG
                try
                {
                    if (VERBOSE_DEBUGGING)
                        Dev.Log($"{gameObject.GetSceneHierarchyPath()} was inside a box {boxHits.FirstOrDefault(x => x.OverlapPoint(currentLocation)).gameObject.GetSceneHierarchyPath()}");

                    if (VERBOSE_DEBUGGING)
                    {
                        foreach (var e in boxHits)
                        {
                            if (e.OverlapPoint(currentLocation))
                                Dev.Log($"{e.gameObject.GetSceneHierarchyPath()} is a box that was hit!");
                        }
                    }
                }
                catch (Exception e)
                {
                    Dev.Log($"Caught exception in debug printing... ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
#endif

            }

            if (isInsideWall)
                resolutionHits = raycastHits.Where(x => x.collider is BoxCollider2D && (x.collider as BoxCollider2D).OverlapPoint(currentLocation)).ToList();

            return isInsideWall;
        }

        float maxDistanceFromObjectAllowed = 2f;
        private bool IsInsideWorldEdgeWall()
        {
            bool isInsideWall = false;  
            var raycastEdgeHits = raycastHits.Where(x => x.collider is EdgeCollider2D).ToList();
            if (raycastEdgeHits.Count > 0)
            {
                foreach (var hit in raycastEdgeHits)
                {
                    var edgeCollider = hit.collider as EdgeCollider2D;

                    var result = GetEdgeIntersections(currentLocation, heroLocation, hit, edgeCollider);
                    edgeIntersections = edgeIntersections.Concat(result).OrderBy(x => x.distance).ToList();
                }

                edgeIntersections = edgeIntersections.Where(x => x.distance < maxDistanceFromObjectAllowed).ToList();

                var edgeHitsCount = edgeIntersections.Count;
                if (edgeHitsCount > 0 && edgeHitsCount % 2 == 1)
                {
                    isInsideWall = true;
#if DEBUG

                    try
                    {
                        if (VERBOSE_DEBUGGING)
                        {
                            foreach (var e in edgeIntersections)
                            {
                                if (e.transform != null && e.transform.gameObject != null)
                                {
                                    Dev.Log($"{e.transform.gameObject.GetSceneHierarchyPath()} is an edge that was hit at {e.point}!");
                                }
                                else
                                {
                                    Dev.Log($"transform in this edge hit is null!");
                                }
                            }
                            Dev.Log($"{gameObject.GetSceneHierarchyPath()} was inside {edgeHitsCount} edges");
                        }
                    }
                    catch(Exception e)
                    {
                        Dev.Log($"Caught exception in debug printing... ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                    }

                if (edgeHitsCount == 1)
                    isBuggyEdgeCase = true;
#endif

                    resolutionHits = edgeIntersections;
                }
            }

            return isInsideWall;
        }

        private bool IsInsidePolygon()
        {
            bool isInsideWall = false;
            var polyHits = raycastHits.Where(x => x.collider is PolygonCollider2D).Select(x => x.collider as PolygonCollider2D);
            if (polyHits.Any(x => x.OverlapPoint(currentLocation)))
            {
                isInsideWall = true;
#if DEBUG

                try
                {

                    if (VERBOSE_DEBUGGING)
                    {
                        Dev.Log($"{gameObject.GetSceneHierarchyPath()} was inside a polygon {polyHits.FirstOrDefault(x => x.OverlapPoint(currentLocation)).gameObject.GetSceneHierarchyPath()}");

                        foreach (var e in polyHits)
                        {
                            if (e.OverlapPoint(currentLocation))
                                Dev.Log($"{e.gameObject.GetSceneHierarchyPath()} is a poly that was hit!");
                        }
                    }
                }
                catch (Exception e)
                {
                    Dev.Log($"Caught exception in debug printing... ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
                }
#endif
            }

            if (isInsideWall)
                resolutionHits = raycastHits.Where(x => x.collider is PolygonCollider2D && (x.collider as PolygonCollider2D).OverlapPoint(currentLocation)).ToList();

            return isInsideWall;
        }

        private void ResolveInsideWall(Vector2 currentLocation, Vector2 heroLocation, Vector2 toHeroDir)
        {
            //TODO: 
            //var heroRaycast = Physics2D.Raycast(heroLocation, -toHeroDir, toHero.magnitude, Physics2D.AllLayers);
            //if(heroRaycast.collider != null && heroRaycast.collider.OverlapPoint(heroLocation))
            //{
            //    if (VERBOSE_DEBUGGING)
            //        Dev.LogWarning($"{gameObject.GetSceneHierarchyPath()} ignore the inside wall check because the player was inside a wall");
            //    return;
            //}

            RaycastHit2D nearest;// = raycastHits.OrderBy(x => )
            if (edgeIntersections.Count > 0)
            {
#if DEBUG
                raycastHits = resolutionHits.OrderBy(x => x.distance).ToList();
                nearest = resolutionHits.First();
#else
                nearest = resolutionHits.OrderBy(x => x.distance).First();
#endif
            }
            else
            {
                nearest = resolutionHits.OrderBy(x => x.distance).First();
            }

            DrawRayInDebugBuild(currentLocation, heroLocation);
            MoveOutOfWall(toHeroDir, nearest);
        }

        private void MoveOutOfWall(Vector2 toHeroDir, RaycastHit2D nearest)
        {
            Vector2 collisionPoint = nearest.point;

            var offset = Vector2.Dot(toHeroDir, gameObject.GetOriginalObjectSize(true)) * toHeroDir * 0.5f;

            if (VERBOSE_DEBUGGING)
                Dev.LogWarning($"{gameObject.GetSceneHierarchyPath()} was moved to {collisionPoint + offset} out from inside a wall at {transform.position}");

            var pos = collisionPoint + offset;
            var poob = gameObject.GetComponent<PreventOutOfBounds>();
            if (poob != null)
                poob.ForcePosition(pos);
            else
                gameObject.transform.position = pos;
        }

        private void DrawRayInDebugBuild(Vector2 currentLocation, Vector2 heroLocation)
        {

#if DEBUG
            if (isBuggyEdgeCase)
            {
                var debugColliders = gameObject.GetComponent<DebugColliders>();
                if (debugColliders != null)
                {
                    if (debugColliders.rayLines != null)
                    {
                        if (debugColliders.rayLines.Count < maxDebugRays)
                            debugColliders.CreateRayFromRaycastHits(gameObject, currentLocation, heroLocation, raycastHits.ToList());
                    }
                }
            }
#endif
        }
    }
}