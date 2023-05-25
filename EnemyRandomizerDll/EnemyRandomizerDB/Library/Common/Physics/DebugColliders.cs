using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class DebugColliders : MonoBehaviour
    {
        public List<BoxCollider2D> boxColliders;
        public List<CircleCollider2D> circleColliders;
        public List<PolygonCollider2D> polyColliders;
        public List<EdgeCollider2D> edgeColliders;
        public Dictionary<Color, List<Vector3>> customLineCollections = new Dictionary<Color, List<Vector3>>();
        public List<List<Vector2>> rayPoints = new List<List<Vector2>>();

        public GameObject lineRoot;
        public static GameObject globalLineRoot;

        public float lineWidth = .05f;
        public float zDepth = -2.1f;
        public Color colliderColor = Color.green;
        public Color triggerColor = Color.blue;
        public Color disabledColor = Color.red;
        public Color customColor = Color.yellow;
        public Color gameObjectDisabledColor = Color.gray;
        public Color rayColor = Color.red + Color.yellow;
        public bool renderDisabledColliders = true;
        public static bool runDebugInput = true;

        Dictionary<Collider2D, LineRenderer> lines;
        Dictionary<List<Vector3>, LineRenderer> customLines;
        public Dictionary<List<Vector2>, LineRenderer> rayLines;

        Material colliderLineMat;
        Material triggerLineMat;
        Material disabledLineMat;
        Material gameObjectDisabledLineMat;
        Material customColliderLineMat;
        Material rayLineMat;


        Dictionary<Collider2D, TextMesh> labels;
        Dictionary<List<Vector3>, TextMesh> customLabels;
        Dictionary<List<Vector2>, TextMesh> rayLabels;
        Dictionary<List<Vector2>, List<TextMesh>> rayHitLabels;

        List<Vector2> GetPointsFromCollider(BoxCollider2D col)
        {
            List<Vector2> points = new List<Vector2>();

            Vector2 cSize = col.size * .5f;
            Vector2 bottomLeft = -cSize;
            Vector2 topLeft = new Vector2(-cSize.x, cSize.y);
            Vector2 bottomRight = new Vector2(cSize.x, -cSize.y);
            Vector2 topRight = cSize;

            points.Add(bottomLeft + col.offset);
            points.Add(topLeft + col.offset);
            points.Add(topRight + col.offset);
            points.Add(bottomRight + col.offset);
            points.Add(bottomLeft + col.offset);

            for (int i = 0; i < points.Count; ++i)
            {
                Vector3 result = col.transform.TransformPoint((Vector3)points[i]);
                points[i] = (Vector2)result;
            }

            return points;
        }

        //List<Vector2> GetPointsFromRaycastHits(IEnumerable<RaycastHit2D> col)
        //{
        //    List<Vector2> points = col.Select(x => x.point).ToList();
        //    return points;
        //}

        public LineRenderer CreateRayFromRaycastHits(GameObject owner, Vector2 start, Vector2 end, List<RaycastHit2D> hits)
        {
            Vector2 dir = (end - start).normalized;
            Vector2 pDir = new Vector2(-dir.y, dir.x);
            List<Vector2> points = new List<Vector2>();
            points.Add(start);

            if (hits.Count > 0)
            {
                float intersectionMarkSize = 1f;
                for (int i = 0; i < hits.Count; ++i)
                {
                    bool isEdgeCollider = hits[i].collider is EdgeCollider2D;
                    bool isPolyCollider = hits[i].collider is PolygonCollider2D;
                    bool isBoxCollider = hits[i].collider is BoxCollider2D;
                    bool isCircleCollider = hits[i].collider is CircleCollider2D;

                    var hit = hits[i];
                    points.Add(hit.point);

                    var markSize = intersectionMarkSize;
                    if (isEdgeCollider)
                        markSize = 1.5f;
                    if (isPolyCollider)
                        markSize = 1f;
                    if (isBoxCollider)
                        markSize = 0.5f;
                    if (isCircleCollider)
                        markSize = 0.25f;

                    points.Add(hit.point - pDir * markSize);
                    points.Add(hit.point + pDir * markSize);

                    points.Add(hit.point);

                }
            }

            points.Add(end);
            rayPoints.Add(points);

            for(int i = 0; i < hits.Count; ++i)
            {
                var hit = hits[i];
                if(!rayHitLabels.TryGetValue(points, out var tmlist))
                {
                    tmlist = new List<TextMesh>();
                }

                bool isEdgeCollider = hits[i].collider is EdgeCollider2D;
                bool isPolyCollider = hits[i].collider is PolygonCollider2D;
                bool isBoxCollider = hits[i].collider is BoxCollider2D;
                bool isCircleCollider = hits[i].collider is CircleCollider2D;

                var markString = "none";
                if (isEdgeCollider)
                    markString = "edge";
                if (isPolyCollider)
                    markString = "poly";
                if (isBoxCollider)
                    markString = "box";
                if (isCircleCollider)
                    markString = "circle";

                string point_name = $"<- r[{rayLines.Count}] hit {i+1}/{hits.Count} ";

                GameObject hitLabel = new GameObject(owner.name + " " + point_name + markString);
                hitLabel.transform.SetParent(globalLineRoot.transform);
                hitLabel.transform.localScale = Vector3.one * .25f;
                hitLabel.transform.position = hit.point.ToVector3(-.4f);
                TextMesh hittm = hitLabel.gameObject.AddComponent<TextMesh>();
                hittm.text = point_name + markString;
                hittm.fontSize = 14;
                hittm.color = Color.white;
                hittm.alignment = TextAlignment.Center;
                tmlist.Add(hittm);
            }

            string collidersHit = hits.Count > 0 ? "with hits on: "+string.Join(", ", hits.Select(x => x.collider == null ? "null" : x.collider.name)) : string.Empty;
            string name = "[ " + owner.name + ",  " + "RaycastHit2D " + collidersHit + " ]";

            GameObject newLine = new GameObject(name);
            newLine.transform.SetParent(globalLineRoot.transform);
            newLine.transform.position = Vector3.zero;

            Color lineColor = hits.Count > 0 ? rayColor : Color.gray;

            LineRenderer line = newLine.AddComponent<LineRenderer>();
            line.positionCount = points.Count;
            line.SetPositions(points.Select(x => x.ToVector3(-.3f)).ToArray());
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.sharedMaterial = GetLineMaterial(points.FirstOrDefault());
            line.startColor = lineColor;
            line.endColor = lineColor;
            rayLines.Add(points, line);

            GameObject lineLabel = new GameObject(name + " LABEL");
            lineLabel.transform.SetParent(newLine.transform);
            lineLabel.transform.localScale = Vector3.one * .25f;
            TextMesh tm = newLine.gameObject.AddComponent<TextMesh>();
            tm.text = name;
            tm.fontSize = 14;
            tm.color = Color.white;
            tm.alignment = TextAlignment.Center;
            rayLabels.Add(points, tm);

            return line;
        }

        public List<Vector2> GetPointsFromCollider(Bounds bounds, bool relative = true)
        {
            List<Vector2> points = new List<Vector2>();

            if (relative)
            {
                Vector2 cSize = bounds.size * .5f;
                Vector2 bottomLeft = -cSize;
                Vector2 topLeft = new Vector2(-cSize.x, cSize.y);
                Vector2 bottomRight = new Vector2(cSize.x, -cSize.y);
                Vector2 topRight = cSize;

                points.Add(bottomLeft);
                points.Add(topLeft);
                points.Add(topRight);
                points.Add(bottomRight);
                points.Add(bottomLeft);

                for (int i = 0; i < points.Count; ++i)
                {
                    Vector3 result = gameObject.transform.TransformPoint((Vector3)points[i]);
                    points[i] = (Vector2)result;
                }
            }
            else
            {
                Vector2 bottomLeft = new Rect(bounds.center, bounds.size).BottomLeft();
                Vector2 topLeft = new Rect(bounds.center, bounds.size).TopLeft();
                Vector2 bottomRight = new Rect(bounds.center, bounds.size).BottomRight();
                Vector2 topRight = new Rect(bounds.center, bounds.size).TopRight();

                points.Add(bottomLeft);
                points.Add(topLeft);
                points.Add(topRight);
                points.Add(bottomRight);
                points.Add(bottomLeft);
            }

            return points;
        }

        public List<Vector2> GetPointsFromCollider(Rect bounds, bool relative = true)
        {
            List<Vector2> points = new List<Vector2>();

            if (relative)
            {
                Vector2 cSize = bounds.size * .5f;
                Vector2 bottomLeft = -cSize;
                Vector2 topLeft = new Vector2(-cSize.x, cSize.y);
                Vector2 bottomRight = new Vector2(cSize.x, -cSize.y);
                Vector2 topRight = cSize;

                points.Add(bottomLeft);
                points.Add(topLeft);
                points.Add(topRight);
                points.Add(bottomRight);
                points.Add(bottomLeft);

                for (int i = 0; i < points.Count; ++i)
                {
                    Vector3 result = gameObject.transform.TransformPoint((Vector3)points[i]);
                    points[i] = (Vector2)result;
                }
            }
            else
            {
                Vector2 bottomLeft = bounds.BottomLeft();
                Vector2 topLeft = bounds.TopLeft();
                Vector2 bottomRight = bounds.BottomRight();
                Vector2 topRight = bounds.TopRight();

                points.Add(bottomLeft);
                points.Add(topLeft);
                points.Add(topRight);
                points.Add(bottomRight);
                points.Add(bottomLeft);
            }

            return points;
        }

        public List<Vector2> GetPointsFromCollider(Range x, Range y, bool relative = true)
        {
            List<Vector2> points = new List<Vector2>();

            if (relative)
            {
                Vector2 cSize = new Vector2(x.Size, y.Size) * .5f;
                Vector2 bottomLeft = -cSize;
                Vector2 topLeft = new Vector2(-cSize.x, cSize.y);
                Vector2 bottomRight = new Vector2(cSize.x, -cSize.y);
                Vector2 topRight = cSize;

                points.Add(bottomLeft);
                points.Add(topLeft);
                points.Add(topRight);
                points.Add(bottomRight);
                points.Add(bottomLeft);

                for (int i = 0; i < points.Count; ++i)
                {
                    Vector3 result = gameObject.transform.TransformPoint((Vector3)points[i]);
                    points[i] = (Vector2)result;
                }
            }
            else
            {
                Vector2 mid = new Vector2(x.Mid, y.Mid);
                Vector2 size = new Vector2(x.Size, y.Size);
                Bounds bounds = new Bounds(mid, size);
                Vector2 bottomLeft = new Rect(bounds.center, bounds.size).BottomLeft();
                Vector2 topLeft = new Rect(bounds.center, bounds.size).TopLeft();
                Vector2 bottomRight = new Rect(bounds.center, bounds.size).BottomRight();
                Vector2 topRight = new Rect(bounds.center, bounds.size).TopRight();

                points.Add(bottomLeft);
                points.Add(topLeft);
                points.Add(topRight);
                points.Add(bottomRight);
                points.Add(bottomLeft);
            }

            return points;
        }

        public static List<Vector2> GetPointsFromCollider(Vector2 scale, Vector2 center, float radius)
        {
            List<Vector2> points = new List<Vector2>();

            int slices = 16;

            float sliceSize = (2f * Mathf.PI) / (float)slices;

            for (int i = 0; i < slices; ++i)
            {
                float theta = sliceSize * (float)i;

                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);

                float x = cos - sin;
                float y = sin + cos;

                Vector2 point = new Vector2(x, y);

                point.x = point.x * radius * scale.x;
                point.y = point.y * radius * scale.y;

                points.Add(point + center);
            }

            //add the start point to complete the shape
            {
                float theta = 0f;

                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);

                float x = cos - sin;
                float y = sin + cos;

                Vector2 point = new Vector2(x, y);

                point.x = point.x * radius * scale.x;
                point.y = point.y * radius * scale.y;

                points.Add(point + center);
            }

            return points;
        }

        List<Vector2> GetPointsFromCollider(CircleCollider2D col)
        {
            List<Vector2> points = new List<Vector2>();

            Vector2 center = col.offset + (Vector2)col.transform.position;

            float radius = col.radius;
            int slices = 16;

            float sliceSize = (2f * Mathf.PI) / (float)slices;

            for (int i = 0; i < slices; ++i)
            {
                float theta = sliceSize * (float)i;

                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);

                float x = cos - sin;
                float y = sin + cos;

                Vector2 point = new Vector2(x, y);

                point.x = point.x * radius * col.transform.localScale.x;
                point.y = point.y * radius * col.transform.localScale.y;

                points.Add(point + center);
            }

            //add the start point to complete the shape
            {
                float theta = 0f;

                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);

                float x = cos - sin;
                float y = sin + cos;

                Vector2 point = new Vector2(x, y);

                point.x = point.x * radius * col.transform.localScale.x;
                point.y = point.y * radius * col.transform.localScale.y;

                points.Add(point + center);
            }

            return points;
        }

        List<Vector2> GetPointsFromCollider(PolygonCollider2D col)
        {
            List<Vector2> points = col.points.ToList();
            points.Add(col.points[0]);

            for (int i = 0; i < points.Count; ++i)
            {
                Vector3 result = col.transform.TransformPoint((Vector3)points[i] + (Vector3)col.offset);
                points[i] = (Vector2)result;
            }

            return points;
        }

        List<Vector2> GetPointsFromCollider(EdgeCollider2D col)
        {
            List<Vector2> points = col.points.ToList();

            for (int i = 0; i < points.Count; ++i)
            {
                Vector3 result = col.transform.TransformPoint((Vector3)points[i] + (Vector3)col.offset);
                points[i] = (Vector2)result;
            }

            return points;
            //return col.points.Select( x => { return x + (Vector2)col.transform.position; } ).ToList();
        }

        List<Vector2> GetPointsHelper(Collider2D collider)
        {
            if (collider as PolygonCollider2D)
            {
                return GetPointsFromCollider(collider as PolygonCollider2D);
            }
            if (collider as EdgeCollider2D)
            {
                return GetPointsFromCollider(collider as EdgeCollider2D);
            }
            if (collider as CircleCollider2D)
            {
                return GetPointsFromCollider(collider as CircleCollider2D);
            }
            if (collider as BoxCollider2D)
            {
                return GetPointsFromCollider(collider as BoxCollider2D);
            }

            return new List<Vector2>();
        }

        //use this static bool to keep the input only running on one component 
        static bool forceSingleEntry = false;
        static IEnumerator debugInput = null;
        IEnumerator DebugInput()
        {
            bool suspended = false;
            for (; ; )
            {
                if (suspended)
                    Time.timeScale = 0f;

                //toggle renderers on/off
                if (UnityEngine.Input.GetKeyDown(KeyCode.Slash))
                {
                    Dev.Log("Renderer state toggled with key: /  ");
                    foreach (var v in lines)
                    {
                        v.Value.GetComponent<Renderer>().enabled = !v.Value.GetComponent<Renderer>().enabled;
                    }
                    foreach (var v in customLines)
                    {
                        v.Value.GetComponent<Renderer>().enabled = !v.Value.GetComponent<Renderer>().enabled;
                    }
                    foreach (var v in labels)
                    {
                        v.Value.gameObject.SetActive(!v.Value.gameObject.activeInHierarchy);
                    }
                    foreach (var v in customLabels)
                    {
                        v.Value.gameObject.SetActive(!v.Value.gameObject.activeInHierarchy);
                    }
                    foreach (var v in rayLabels)
                    {
                        v.Value.gameObject.SetActive(!v.Value.gameObject.activeInHierarchy);
                    }
                    foreach (var v in rayHitLabels)
                    {
                        v.Value.ForEach(x => x.gameObject.SetActive(!x.gameObject.activeInHierarchy));
                    }
                }

                //toggle the rendering of 
                if (UnityEngine.Input.GetKeyDown(KeyCode.Backslash))
                {
                    Dev.Log(@"All renderers disabled with key: \\  ");
                    renderDisabledColliders = !renderDisabledColliders;
                }

                //suspend
                if (!forceSingleEntry && UnityEngine.Input.GetKeyDown(KeyCode.Q))
                {
                    Dev.Log("Timescale suspended with key : Q  ");
                    forceSingleEntry = true;
                    Time.timeScale = 0f;
                    suspended = true;
                }
                //advance by about one frame
                if (!forceSingleEntry && UnityEngine.Input.GetKeyDown(KeyCode.W))
                {
                    Dev.Log("Timescale moved forward by 1 frame with key : W  ");
                    forceSingleEntry = true;
                    Time.timeScale = 1f;
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    Time.timeScale = 0f;
                }
                //advance by many frames (hold R)
                if (!forceSingleEntry && UnityEngine.Input.GetKey(KeyCode.R))
                {
                    Dev.Log("Timescale moving with key : R  ");
                    forceSingleEntry = true;
                    Time.timeScale = 1f;
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    Time.timeScale = 0f;
                }
                //resume from suspend
                if (!forceSingleEntry && UnityEngine.Input.GetKeyDown(KeyCode.E))
                {
                    Dev.Log("Timescale resumed with key : E  ");
                    forceSingleEntry = true;
                    Time.timeScale = 1f;
                    suspended = false;
                }
                yield return new WaitForEndOfFrame();
                forceSingleEntry = false;
            }
            //yield break;
        }

        private void OnEnable()
        {
            if (runDebugInput)
            {
                if (debugInput == null)
                {
                    debugInput = DebugInput();
                    GameManager.instance.StartCoroutine(debugInput);
                }
            }
        }

        private void OnDestroy()
        {
            if(globalLineRoot != null)
            {                
                rayLines.Values.ToList().ForEach(x => GameObject.Destroy(x.gameObject));
                rayLines.Clear();
                rayLabels.Values.ToList().ForEach(x => GameObject.Destroy(x.gameObject));
                rayLabels.Clear();
                rayHitLabels.Values.ToList().ForEach(x => x.ForEach(y => GameObject.Destroy(y.gameObject)));
                rayHitLabels.Clear();
            }

            //if (runDebugInput)
            //{
            //    if (debugInput != null)
            //    {
            //        StopCoroutine(debugInput);
            //        debugInput = null;
            //    }
            //}
        }

        private IEnumerator Start()
        {
            boxColliders = GetComponentsInChildren<BoxCollider2D>(true).ToList();
            circleColliders = GetComponentsInChildren<CircleCollider2D>(true).ToList();
            polyColliders = GetComponentsInChildren<PolygonCollider2D>(true).ToList();
            edgeColliders = GetComponentsInChildren<EdgeCollider2D>(true).ToList();

            lineRoot = new GameObject("DebugCollider Lines");
            lineRoot.transform.SetParent(transform);
            lineRoot.transform.position = Vector3.zero;

            if (globalLineRoot == null)
            {
                globalLineRoot = new GameObject("Global DebugCollider Lines");
                globalLineRoot.transform.position = Vector3.zero;
                GameObject.DontDestroyOnLoad(globalLineRoot);
            }

            lines = new Dictionary<Collider2D, LineRenderer>();
            customLines = new Dictionary<List<Vector3>, LineRenderer>();
            rayLines = new Dictionary<List<Vector2>, LineRenderer>();
            labels = new Dictionary<Collider2D, TextMesh>();
            customLabels = new Dictionary<List<Vector3>, TextMesh>();
            rayLabels = new Dictionary<List<Vector2>, TextMesh>();
            rayHitLabels = new Dictionary<List<Vector2>, List<TextMesh>>();

            colliderLineMat = new Material(Shader.Find("Diffuse"));
            triggerLineMat = new Material(Shader.Find("Diffuse"));
            disabledLineMat = new Material(Shader.Find("Diffuse"));
            gameObjectDisabledLineMat = new Material(Shader.Find("Diffuse"));
            customColliderLineMat = new Material(Shader.Find("Diffuse"));
            rayLineMat = new Material(Shader.Find("Diffuse"));

            //yield return new WaitUntil(() => customLineCollections.Count >= 0);

            CreateColliders();

            for (; ; )
            {
                UpdateColliders();
                yield return new WaitForEndOfFrame();
            }
        }

        void CreateColliders()
        {
            CreateLinesFromColliders(boxColliders);
            CreateLinesFromColliders(circleColliders);
            CreateLinesFromColliders(polyColliders);
            CreateLinesFromColliders(edgeColliders);

            //customLineCollections.Select(x => x).ToList().ForEach(x => CreateLinesFromPoints(x.Key, x.Value));
        }

        void CreateLinesFromColliders<T>(List<T> colliders) where T : Collider2D
        {
            foreach (var v in colliders)
            {
                string name = "[" + v.gameObject.name + ",  " + v.GetType().Name + "]";
                GameObject newLine = new GameObject(name);
                newLine.transform.SetParent(lineRoot.transform);
                newLine.transform.position = Vector3.zero;

                Color lineColor = GetLineColor(v);

                //TODO: wrap these line calls in #if checks for unity versions...
                LineRenderer line = newLine.AddComponent<LineRenderer>();
                Vector3[] points = GetPointsHelper(v).Select(x => { return new Vector3(x.x, x.y, zDepth); }).ToArray();
                line.positionCount = points.Length;
                //line.SetVertexCount( points.Length );
                line.SetPositions(points);
                //line.SetWidth( lineWidth, lineWidth );
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                line.sharedMaterial = GetLineMaterial(v);
                //line.SetColors( lineColor, lineColor );
                line.startColor = lineColor;
                line.endColor = lineColor;
                lines.Add(v, line);

                GameObject lineLabel = new GameObject(name + " LABEL");
                lineLabel.transform.SetParent(newLine.transform);
                lineLabel.transform.localScale = Vector3.one * .25f;
                TextMesh tm = newLine.gameObject.AddComponent<TextMesh>();
                tm.text = name;
                tm.fontSize = 14;
                //tm.font = Font.CreateDynamicFontFromOSFont( "Arial", 12 );
                tm.color = Color.white;
                tm.alignment = TextAlignment.Center;
                labels.Add(v, tm);
            }
        }

        void CreateLinesFromPoints(Color customColor, List<Vector3> points)
        {
            string name = "[" + gameObject.name + ",  " + "Custom Points" + "]";
            GameObject newLine = new GameObject(name);
            newLine.transform.SetParent(lineRoot.transform);
            newLine.transform.position = Vector3.zero;

            Color lineColor = customColor;

            LineRenderer line = newLine.AddComponent<LineRenderer>();
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.sharedMaterial = GetLineMaterial(points.FirstOrDefault());
            line.startColor = lineColor;
            line.endColor = lineColor;
            customLines.Add(points, line);

            GameObject lineLabel = new GameObject(name + " LABEL");
            lineLabel.transform.SetParent(newLine.transform);
            lineLabel.transform.localScale = Vector3.one * .25f;
            TextMesh tm = newLine.gameObject.AddComponent<TextMesh>();
            tm.text = name;
            tm.fontSize = 14;
            tm.color = Color.white;
            tm.alignment = TextAlignment.Center;
            customLabels.Add(points, tm);
        }

        Color GetLineColor(Collider2D col)
        {
            if (!col.gameObject.activeInHierarchy)
            {
                return gameObjectDisabledColor;
            }

            if (!col.enabled)
            {
                return disabledColor;
            }

            if (col.isTrigger)
            {
                return triggerColor;
            }

            return colliderColor;
        }

        Material GetLineMaterial(Vector3 col)
        {
            return customColliderLineMat;
        }

        Material GetLineMaterial(Vector2 col)
        {
            return rayLineMat;
        }

        Material GetLineMaterial(Collider2D col)
        {
            if (!col.gameObject.activeInHierarchy)
            {
                return gameObjectDisabledLineMat;
            }

            if (!col.enabled)
            {
                return disabledLineMat;
            }

            if (col.isTrigger)
            {
                return triggerLineMat;
            }

            return colliderLineMat;
        }

        void UpdateColliders()
        {
            float additionalLabelOffset = 0f;
            foreach (var pair in lines)
            {
                if (pair.Key == null)
                    continue;

                if (!renderDisabledColliders)
                {
                    if (!pair.Key.enabled || !pair.Key.gameObject.activeInHierarchy)
                    {
                        pair.Value.enabled = false;
                        continue;
                    }
                }

                pair.Value.sharedMaterial = GetLineMaterial(pair.Key);

                Color lineColor = GetLineColor(pair.Key);
                //pair.Value.SetColors( lineColor, lineColor );
                pair.Value.startColor = lineColor;
                pair.Value.endColor = lineColor;

                if (pair.Value.GetComponent<Renderer>())
                    pair.Value.GetComponent<Renderer>().sharedMaterial.color = lineColor;

                {
                    labels[pair.Key].color = lineColor;
                    labels[pair.Key].transform.position = pair.Key.bounds.max;
                    if (pair.Key as CircleCollider2D)
                    {
                        labels[pair.Key].transform.position = Vector3.one * (pair.Key as CircleCollider2D).radius;
                    }

                    foreach (var v in labels)
                    {
                        if (Mathf.Abs(v.Value.transform.position.y - labels[pair.Key].transform.position.y) < 0.2f)
                        {
                            labels[pair.Key].transform.Translate(new Vector3(0f, additionalLabelOffset, 0f));
                            additionalLabelOffset -= .2f;
                            break;
                        }
                    }

                    Vector3 labelPos = new Vector3(labels[pair.Key].transform.position.x, labels[pair.Key].transform.position.y, -.1f);
                    labels[pair.Key].transform.position = labelPos;
                    labels[pair.Key].transform.localScale = Vector3.one * .2f;
                }

                pair.Value.enabled = true;

                LineRenderer line = pair.Value;
                Vector3[] points = GetPointsHelper(pair.Key).Select(x => { return new Vector3(x.x, x.y, zDepth); }).ToArray();
                line.SetPositions(points);
            }

            foreach(var pair in customLines)
            {
                pair.Value.sharedMaterial = GetLineMaterial(pair.Key.FirstOrDefault());

                Color lineColor = customLineCollections.FirstOrDefault(x => pair.Key == x.Value).Key;
                pair.Value.startColor = lineColor;
                pair.Value.endColor = lineColor;

                if (pair.Value.GetComponent<Renderer>())
                    pair.Value.GetComponent<Renderer>().sharedMaterial.color = lineColor;

                //custom labels
                {
                    customLabels[pair.Key].color = lineColor;
                    customLabels[pair.Key].transform.position = pair.Key.Max();
                    

                    foreach (var v in customLabels)
                    {
                        if (Mathf.Abs(v.Value.transform.position.y - customLabels[pair.Key].transform.position.y) < 0.2f)
                        {
                            customLabels[pair.Key].transform.Translate(new Vector3(0f, additionalLabelOffset, 0f));
                            additionalLabelOffset -= .2f;
                            break;
                        }
                    }

                    Vector3 labelPos = new Vector3(customLabels[pair.Key].transform.position.x, customLabels[pair.Key].transform.position.y, -.1f);
                    customLabels[pair.Key].transform.position = labelPos;
                    customLabels[pair.Key].transform.localScale = Vector3.one * .2f;
                }

                LineRenderer line = pair.Value;
                Vector3[] points = pair.Key.ToArray();
                line.SetPositions(points);
            }


            //foreach (var pair in rayLines)
            //{
            //    pair.Value.sharedMaterial = GetLineMaterial(pair.Key.FirstOrDefault());

            //    Color lineColor = rayColor;// raycastHits.FirstOrDefault(x => pair.Key == x.Value).Key;
            //    pair.Value.startColor = lineColor;
            //    pair.Value.endColor = lineColor;

            //    if (pair.Value.GetComponent<Renderer>())
            //        pair.Value.GetComponent<Renderer>().sharedMaterial.color = lineColor;

            //    //custom labels
            //    //{
            //    //    rayLabels[pair.Key].color = lineColor;
            //    //    rayLabels[pair.Key].transform.position = pair.Key.Max();


            //    //    foreach (var v in rayLabels)
            //    //    {
            //    //        if (Mathf.Abs(v.Value.transform.position.y - rayLabels[pair.Key].transform.position.y) < 0.2f)
            //    //        {
            //    //            rayLabels[pair.Key].transform.Translate(new Vector3(0f, additionalLabelOffset, 0f));
            //    //            additionalLabelOffset -= .2f;
            //    //            break;
            //    //        }
            //    //    }

            //    //    Vector3 labelPos = new Vector3(rayLabels[pair.Key].transform.position.x, rayLabels[pair.Key].transform.position.y, -.1f);
            //    //    rayLabels[pair.Key].transform.position = labelPos;
            //    //    rayLabels[pair.Key].transform.localScale = Vector3.one * .2f;
            //    //}

            //    //LineRenderer line = pair.Value;
            //    //Vector2[] points = pair.Key.ToArray();
            //    //line.SetPositions(points);
            //}
        }
    }
}
