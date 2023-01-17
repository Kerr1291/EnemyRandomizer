using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

using nv;

public class MapScene : MonoBehaviour {

    public SpriteRenderer output;

    public float cellSize = 1f;

    public int setupThrottle = 100;

    public LayerMask debugLayerMask;

    Rect bounds;
    Vector2Int mapPos
    {
        get
        {
            return bounds.position.ToInt();
        }
    }

    ArrayGrid<int> map;
    ArrayGrid<int> debugMap;

    Texture2D mapTex;

    Rect mapRect;
    void UpdateSprite()
    {
        output.sprite = Sprite.Create(mapTex, new Rect(Vector2.zero, mapRect.size), Vector2.zero);
        //output.transform.localScale = new Vector3(mapRect.size.x * 10f, mapRect.size.y * 10f, 1f);
        output.transform.localScale = new Vector3(100f, 100f, 1f);
        output.transform.position = bounds.position;
    }

    Color WriteColor(int type)
    {
        if(type == 1)
            return Color.red;
        if(type == 2)
            return Color.white;
        if(type == 3)
            return Color.green;
        return Color.clear;
    }

    int ReadColor(Color type)
    {
        if(type == Color.red)
            return 1;
        if(type == Color.white)
            return 2;
        if(type == Color.green)
            return 3;
        return 0;
    }

    Vector2 MapToWorldPosition(Vector2Int mapPos)
    {
        return bounds.position + (new Vector2(mapPos.x, mapPos.y) * cellSize);
    }

    Vector2Int WorldToMapPosition(Vector2 worldPos)
    {
        Vector2 local = worldPos / cellSize - bounds.position;
        return local.ToInt();
    }

    IEnumerator Start()
    {
        //yield return ReadFromFile(Application.dataPath + "/TestMap.png");

        if(mapTex == null)
        {
            float size = 100f;
            mapRect = new Rect(-size / 2f, -size / 2f, size, size);

            List<Vector2Int> startPositions = new List<Vector2Int>() { new Vector2Int(10, 0), new Vector2Int(50, 98) };

            yield return Setup(mapRect, startPositions);
            yield return Map();

            //path finding code
            //if(false)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                //FindAPath<int> pathFinder = new FindAPath<int>();

                //Node start = new Node(startPositions[0], (int)startPositions[0].x, (int)startPositions[0].y, 1f);
                //Node end = new Node(startPositions[1], (int)startPositions[1].x, (int)startPositions[1].y, 1f);

                debugMap = new ArrayGrid<int>(map);

                //path = pathFinder.GetPath(map, start, end).Select(x=> x.localPosition).ToList();
                AStar pathFinder = new AStar();
                pathFinder.throttle = 100;

                //pathFinder.throttleIterations = 100;
                IEnumerator runningPathFinder = pathFinder.FindPath(map, startPositions[0], startPositions[1], null, true, true);
                runningPathFinder.MoveNext();
                while(pathFinder.result == null)
                {
                    //clear previous path
                    //map.SetElements(path, 0);

                    //get next path
                    runningPathFinder.MoveNext();

                    if(pathFinder.debugPath != null)
                    {
                        mapTex = debugMap.ToTexture(WriteColor);
                        debugMap.SetElements(pathFinder.debugPath, 2);
                        UpdateSprite();
                    }

                    yield return new WaitForEndOfFrame();
                }

                path = pathFinder.result;

                map.SetElements(path, 3);
            }

            mapTex = map.ToTexture(WriteColor);

            //yield return WriteToFile(Application.dataPath + "/TestMap.png");
        }

        UpdateSprite();

        yield break;
	}

    IEnumerator Setup(Rect mapBounds, List<Vector2Int> startPositions)
    {
        bounds = mapBounds;
        map = new ArrayGrid<int>(mapBounds.size.ToInt());

        LayerMask wallMask = new LayerMask();
        wallMask.value = 8; //TODO: update this with the correct value for walls

        float raycastDistance = cellSize;

        //List<Vector2> visited = new List<Vector2>();

        int throttle = 0;


        for(int j = 0; j < (int)map.h; ++j)
        {
            for(int i = 0; i < (int)map.w; ++i)
            {
                throttle++;
                if(throttle % setupThrottle == 0)
                {
                    mapTex = map.ToTexture(WriteColor);
                    UpdateSprite();
                    yield return new WaitForEndOfFrame();
                }

                Vector2Int localFrom = new Vector2Int(i, j);
                Vector2 from = MapToWorldPosition(localFrom);

                List<Vector2Int> nearby = map.GetAdjacentPositions(localFrom, false);

                //if(nearby == null)
                //    continue;
                bool isInWall = true;

                foreach(Vector2Int v in nearby)
                {
                    Vector2 worldV = MapToWorldPosition(v);

                    Vector2 to = worldV;

                    //TODO: use wallmask....
                    if(Physics2D.Raycast(from, (to - from).normalized, raycastDistance, debugLayerMask))
                    {
                        map[localFrom] = 1;
                    }
                    else
                    {
                        if(map[localFrom] != 1)
                            map[localFrom] = 0;

                        isInWall = false;
                        break;
                    }

                    if(isInWall)
                        map[localFrom] = 1;
                }
            }
        }

        //map[startPositions[0]] = 2;
        //map[startPositions[1]] = 2;

        yield break;
    }

    //start positions are required to be valid points that are outside colliders
    IEnumerator Map()
    {
        //TODO: find paths between all start positions
        for(int i = 0; i < map.Count; ++i)
        {
        }

        yield break;
    }
    
    IEnumerator WriteToFile(string filepath)
    {
        yield return new WaitForEndOfFrame();
        
        // Encode texture into PNG
        byte[] bytes = mapTex.EncodeToPNG();

        //
        File.WriteAllBytes(filepath, bytes);

        yield break;
    }

    IEnumerator ReadFromFile(string filePath)
    {
        yield return new WaitForEndOfFrame();
        byte[] fileData;

        if(File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            mapTex = new Texture2D(2, 2);
            mapTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

            mapRect = new Rect(0f, 0f, mapTex.width, mapTex.height);
            Dev.LogVar(mapRect);

            map = new ArrayGrid<int>(mapTex, ReadColor);
        }

        yield break;
    }
}



//foreach(Vector2 s in startPositions)
//{
//    Vector2 localStart = WorldToMapPosition(s);

//    if(!map.IsValidPosition(localStart))
//        continue;

//    if(visited.Contains(localStart))
//        continue;

//    List<Vector2> open = new List<Vector2>();
//    open.Add(localStart);

//    //iterate over the map and find bounds
//    while(open.Count > 0)
//    {
//        Vector2 current = open[0];
//        open.RemoveAt(0);

//        throttle++;
//        if(throttle % setupThrottle == 0)
//        {
//            mapTex = Dev.ArrayGridToTexture(map, WriteColor);
//            UpdateSprite();
//            yield return new WaitForEndOfFrame();
//        }

//        Vector2 worldCurrent = MapToWorldPosition(current);

//        Vector2 from = worldCurrent;

//        List<Vector2> nearby = map.GetAdjacentPositions(current, false);

//        foreach(Vector2 v in nearby)
//        {
//            Vector2 worldV = MapToWorldPosition(v);

//            Vector2 to = worldV;

//            //TODO: use wallmask....
//            if(Physics2D.Raycast(from, (to - from).normalized, raycastDistance, debugLayerMask))
//            {
//                map[current] = 1;
//            }
//            else
//            {
//                if(map[current] != 1)
//                    map[current] = 0;
//            }

//            if(open.Contains(v))
//                continue;

//            if(visited.Contains(v))
//                continue;

//            open.Add(v);
//        }

//        //Dev.Log("Checked " + current + " and set the type to " + map[current]);
//        visited.Add(current);
//    }
//}