using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nv
{
    //For treating a 1D array as a 2D grid       
    [System.Serializable]
    public class ArrayGrid<T> : IEnumerable, IEnumerable<T>
    {
        List<T> data;

        int sizeX;
        int sizeY;

        //Direct access, no bounds checking
        public T this[float i]
        {
            get
            {
                return data[(int)i];
            }
            set
            {
                data[(int)i] = value;
            }
        }

        //Direct access, no bounds checking
        public T this[int i]
        {
            get
            {
                return data[i];
            }
            set
            {
                data[i] = value;
            }
        }

        public Vector2Int Size
        {
            get { return new Vector2Int(sizeX, sizeY); }
            set { sizeX = value.x; sizeY = value.y; }
        }

        public int w
        {
            get
            {
                return (int)(sizeX);
            }
            set
            {
                sizeX = value;
            }
        }

        //height of map
        public int h
        {
            get
            {
                return (int)(sizeY);
            }
            set
            {
                sizeY = h;
            }
        }

        public Rect Area
        {
            get { return Rect.MinMaxRect(0f,0f,w,h); }
        }

        public Rect ValidArea
        {
            get { return Rect.MinMaxRect(0f, 0f, w-1, h-1); }
        }

        //raw element count
        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        public Vector2Int CenterPosition
        {
            get
            {
                return Vector2Int.FloorToInt(Area.center);
            }
        }

        public bool IsCenterPosition(Vector2Int pos)
        {
            return pos == CenterPosition;
        }

        public bool IsMidpointX(int x)
        {
            return x == CenterPosition.x;
        }

        public bool IsMidpointY(int y)
        {
            return y == CenterPosition.y;
        }

        public void Clear()
        {
            if(data == null)
                return;

            data.Clear();
            data.TrimExcess();
            Size = Vector2Int.zero;
        }

        public bool DataIsValid()
        {
            if(w * h != Count)
            {
                Dev.LogWarning("Error: size " + (w * h) + " != data size " + Count + "; Possible data corruption.");
                return false;
            }
            return true;
        }

        public ArrayGrid()
        {
            Resize(0, 0);
        }

        public ArrayGrid(int x, int y)
        {
            Resize(x, y);
        }

        public ArrayGrid( int x, int y, IEnumerable<T> initialValue )
        {
            Resize( x, y );
            var temp = initialValue.ToList();            
            for( int i = 0; i < data.Count && i < temp.Count; ++i )
                data[ i ] = temp[i];
        }

        public ArrayGrid(int x, int y, T initialValue)
        {
            Resize(x, y);
            for(int i = 0; i < data.Count; ++i)
                data[i] = initialValue;
        }

        public ArrayGrid(Vector2Int size)
        {
            Resize((int)size.x, (int)size.y);
        }

        public ArrayGrid(Vector2Int size, T initialValue)
        {
            Resize((int)size.x, (int)size.y);
            for(int i = 0; i < data.Count; ++i)
                data[i] = initialValue;
        }        

        //create a new ArrayGrid from an old one
        public ArrayGrid(ArrayGrid<T> other)
        {
            Resize(other.Size);
            CopyArea(other, other.Area, Area);
        }

        //create a ArrayGrid from a Texture2D 
        public ArrayGrid(Texture2D data, System.Func<Color,T> fromColorToData)
        {
            Resize(data.width, data.height);

            for(int j = 0; j < h; ++j)
            {
                for(int i = 0; i < w; ++i)
                {
                    SetElement(i, j, fromColorToData(data.GetPixel(i, j)));
                }
            }
        }

        public List<T> Elements
        {
            get
            {
                return data;
            }
        }

        //calls GetElement at each given position, will place null in the output list if a position is invalid
        public List<T> GetElements(List<Vector2Int> positions)
        {
            List<T> elements = new List<T>();
            for(int i = 0; i < positions.Count; ++i)
            {
                elements.Add(GetElement(positions[i]));
            }
            return elements;
        }

        //Direct access, no bounds checking
        public T this[Vector2Int p]
        {
            get
            {
                return this[(int)p.x, (int)p.y];
            }
            set
            {
                this[(int)(p.x), (int)(p.y)] = value;
            }
        }

        //Direct access, no bounds checking
        public T this[int x, int y]
        {
            get
            {
                return data[(y * w + x)];
            }
            set
            {
                data[(y * w + x)] = value;
            }
        }

        public Vector2Int GetPositionFromIndex(int i)
        {
            return new Vector2Int((int)(i % w), (int)(i / w));
        }

        //Safe access, with bounds checking
        public T GetElement(Vector2Int pos)
        {
            return GetElement((int)(pos.x), (int)(pos.y));
        }

        //Safe access, with bounds checking
        public T GetElement(int x, int y)
        {
            if(!IsValidPosition(x, y))
                return default(T);
            return this[x, y];
        }

        //Safe access, with bounds checking; Returns reference to the element (if valid)
        public T SetElement(int x, int y, T element)
        {
            if(IsValidPosition(x, y) == false)
                return default(T);
            this[x, y] = element;
            return this[x, y];
        }

        //Safe access, with bounds checking; Returns reference to the element (if valid)
        public T SetElement(Vector2Int pos, T element)
        {
            return SetElement((int)pos.x, (int)pos.y, element);
        }

        public void CopyArea(ArrayGrid<T> source, Rect source_area, Rect dest_area)
        {
            CopyArea(source, source_area, this, dest_area);
        }

        public static void CopyArea(ArrayGrid<T> source, Rect source_area, ArrayGrid<T> dest, Rect dest_area)
        {
            source_area = Mathnv.Clamp(source_area, source.Area);
            dest_area = Mathnv.Clamp(dest_area, dest.Area);

            int minX = (int)Mathf.Min(dest_area.width, source_area.width);
            int minY = (int)Mathf.Min(dest_area.height, source_area.height);

            for(int j = 0; j < minY; ++j)
            {
                for(int i = 0; i < minX; ++i)
                {
                    dest.SetElement(i + (int)dest_area.xMin, j + (int)dest_area.yMin, source[i + (int)source_area.xMin, j + (int)source_area.yMin]);
                }
            }
        }

        //This will make the destination area reference the source area
        //Warning: only use this if you know what you're doing!
        public static void ReferenceArea(ArrayGrid<T> source, Rect source_area, ArrayGrid<T> dest, Rect dest_area)
        {
            source_area = Mathnv.Clamp(source_area, source.Area);
            dest_area = Mathnv.Clamp(dest_area, dest.Area);

            int minX = (int)Mathf.Min(dest_area.width, source_area.width);
            int minY = (int)Mathf.Min(dest_area.height, source_area.height);

            for(int j = 0; j < minY; ++j)
            {
                for(int i = 0; i < minX; ++i)
                {
                    dest[i + (int)dest_area.xMin, j + (int)dest_area.yMin] = source[i + (int)source_area.xMin, j + (int)source_area.yMin];
                }
            }
        }

        //this will move an element to another position in the grid and null out the source
        public bool MoveElement(Vector2Int from, Vector2Int to)
        {
            if(!IsValidPosition(from))
                return false;
            if(!IsValidPosition(to))
                return false;

            this[to] = this[from];
            this[from] = default(T);
            return true;
        }

        //this will move an element to another position by delta dx in the grid and null out the source
        public bool MoveElementdx(Vector2Int from, Vector2Int dx)
        {
            Vector2Int ddx = from + dx;
            return MoveElement(from, ddx);
        }

        //this will copy an element from one position to another position
        //note that if the type is a reference type both positions will reference the same object
        public bool CopyElement(Vector2Int from, Vector2Int to)
        {
            if(!IsValidPosition(from))
                return false;
            if(!IsValidPosition(to))
                return false;

            this[to] = this[from];
            return true;
        }

        //this will move an element to another position by delta dx in the grid and null out the source
        //note that if the type is a reference type both positions will reference the same object
        public bool CopyElementdx(Vector2Int from, Vector2Int dx)
        {
            Vector2Int ddx = from + dx;
            return CopyElement(from, ddx);
        }

        //max valid (x,y)
        public Vector2Int MaxValidPosition
        {
            get
            {
                Vector2Int max = new Vector2Int(w - 1, h - 1);
                max.x = Mathf.Max(max.x, 0);
                max.y = Mathf.Max(max.y, 0);
                return max;
            }
        }

        public bool IsValidPosition(Vector2Int pos)
        {
            return IsValidPosition((int)(pos.x), (int)(pos.y));
        }

        public bool IsValidPosition(int x, int y)
        {
            return Mathnv.Contains(x, y, Vector2Int.zero, Size);
        }

        //resize the layer. if possible, old data will be preserved
        public void Resize(int x, int y)
        {
            if(x == w && y == h)
                return;

            //growing the array
            if(x > w || y > h)
            {
                List<T> newArray;
                newArray = new List<T>(x * y);
                for(int j = 0; j < y; ++j)
                {
                    for(int i = 0; i < x; ++i)
                    {
                        newArray.Add(default(T));
                    }
                }
                if(data != null)
                {
                    int minX = Mathf.Min(w, x);
                    int minY = Mathf.Min(h, y);

                    for(int j = 0; j < minY; ++j)
                    {
                        for(int i = 0; i < minX; ++i)
                        {
                            newArray[(j * x + i)] = data[(j * w + i)];
                        }
                    }
                }
                Clear();

                data = newArray;
            }
            //shrinking the array
            else
            {
                if(data != null)
                {
                    int minX = Mathf.Min(w, x);
                    int minY = Mathf.Min(h, y);

                    for(int j = 0; j < minY; ++j)
                    {
                        for(int i = 0; i < minX; ++i)
                        {
                            data[(j * x + i)] = data[(j * w + i)];
                        }
                    }
                    int previous_size = data.Count;
                    data.RemoveRange(x * y, previous_size - x * y);
                    data.Capacity = x * y;
                }
                else
                {
                    data = new List<T>(x * y);
                    for(int j = 0; j < y; ++j)
                    {
                        for(int i = 0; i < x; ++i)
                        {
                            data.Add(default(T));
                        }
                    }
                }
            }

            Size = new Vector2Int(x, y);
        }

        public void Resize(Vector2Int new_size)
        {
            Resize((int)new_size.x, (int)new_size.y);
        }

        public void Resize(Rect new_size)
        {
            Resize(new_size.size.ToInt());
        }

        static bool ValidRect(Rect r)
        {
            if(r.width <= 0)
                return false;
            if(r.height <= 0)
                return false;
            return true;
        }

        ///Helpers for iterating over blocks of data in map areas
        static int IterJStart(Rect r)
        {
            return (int)r.TopLeft().y;
        }
        static int IterIStart(Rect r)
        {
            return (int)r.TopLeft().x;
        }
        static int IterJEnd(Rect r)
        {
            return (int)r.BottomRight().y;
        }
        static int IterIEnd(Rect r)
        {
            return (int)r.BottomRight().x;
        }

        class FloodFillData
        {
            public List<Vector2Int> visitedCells = new List<Vector2Int>();
            public List<Vector2Int> cellsToFill = new List<Vector2Int>();
        }

        ///Helper for floodfill
        static void FloodFillCheckAndAdd(ArrayGrid<T> layer, Vector2Int pos, FloodFillData fill_data, List<T> boundry_mask)
        {
            if(layer.IsValidPosition(pos)
                && fill_data.visitedCells.Contains(pos) == false
                && fill_data.cellsToFill.Contains(pos) == false)
            {
                bool boundry = boundry_mask.Contains(layer.GetElement(pos));
                if(!boundry)
                    fill_data.cellsToFill.Add(pos);
            }
        }

        static void FloodFillAdd(ArrayGrid<T> layer, Vector2Int pos, FloodFillData fill_data)
        {
            if(layer.IsValidPosition(pos)
                && fill_data.visitedCells.Contains(pos) == false
                && fill_data.cellsToFill.Contains(pos) == false)
            {
                fill_data.cellsToFill.Add(pos);
            }
        }

        static void FloodFillAddIfType(ArrayGrid<T> layer, Vector2Int pos, FloodFillData fill_data, T type)
        {
            if(layer.IsValidPosition(pos)
                && fill_data.visitedCells.Contains(pos) == false
                && fill_data.cellsToFill.Contains(pos) == false)
            {
                if(EqualityComparer<T>.Default.Equals(layer.GetElement(pos), type))
                    fill_data.cellsToFill.Add(pos);
            }
        }

        static void FloodFillAddIfNotType(ArrayGrid<T> layer, Vector2Int pos, FloodFillData fill_data, T type)
        {
            if(layer.IsValidPosition(pos)
                && fill_data.visitedCells.Contains(pos) == false
                && fill_data.cellsToFill.Contains(pos) == false)
            {
                if(!EqualityComparer<T>.Default.Equals(layer.GetElement(pos), type))
                    fill_data.cellsToFill.Add(pos);
            }
        }

        static bool IsOnBoundry(Rect r, Vector2Int p)
        {
            if(r.xMin == p.x)
                return true;
            if(r.yMin == p.y)
                return true;
            if(r.xMax == p.x)
                return true;
            if(r.yMax == p.y)
                return true;
            return false;
        }

        public bool IsPositionEmpty(int x, int y)
        {
            return IsPositionEmpty(new Vector2Int(x, y));
        }

        public bool IsPositionEmpty(Vector2Int p)
        {
            if(!IsValidPosition(p))
                return true;

            return EqualityComparer<T>.Default.Equals(GetElement(p), default(T));            
        }

        public bool IsPositionOnEdge(int x, int y)
        {
            return IsPositionOnEdge(new Vector2Int(x, y));
        }

        public bool IsPositionOnEdge(Vector2Int p)
        {
            if(!IsValidPosition(p))
                return false;

            if(p.x == 0)
                return true;
            if(p.y == 0)
                return true;
            if(p.x == MaxValidPosition.x)
                return true;
            if(p.y == MaxValidPosition.y)
                return true;
            return false;
        }

        public List<T> GetEmptyElements()
        {
            return data.Where(x => (EqualityComparer<T>.Default.Equals(x, default(T)))).ToList();
        }

        public List<T> GetNonEmptyElements()
        {
            return data.Where(x => (!EqualityComparer<T>.Default.Equals(x, default(T)))).ToList();
        }

        public List<T> GetElementsOfType(T type)
        {
            return data.Where(x => (EqualityComparer<T>.Default.Equals(x, type))).ToList();
        }

        public List<T> GetElementsInMask(List<T> mask)
        {
            return data.Where(x => (mask.Contains(x))).ToList();
        }

        public List<T> GetElementsNotInMask(List<T> mask)
        {
            return data.Where(x => (!mask.Contains(x))).ToList();
        }


        public List<Vector2Int> GetEmptyPositions()
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for(int i = 0; i < data.Count; ++i)
            {
                if(EqualityComparer<T>.Default.Equals(data[i], default(T)))
                {
                    positions.Add(GetPositionFromIndex(i));
                }
            }

            return positions;
        }

        public List<Vector2Int> GetNonEmptyPositions()
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for(int i = 0; i < data.Count; ++i)
            {
                if(!EqualityComparer<T>.Default.Equals(data[i], default(T)))
                {
                    positions.Add(GetPositionFromIndex(i));
                }
            }

            return positions;
        }

        public List<Vector2Int> GetPositionsOfType(T type)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for(int i = 0; i < data.Count; ++i)
            {
                if(EqualityComparer<T>.Default.Equals(data[i], type))
                {
                    positions.Add(GetPositionFromIndex(i));
                }
            }

            return positions;
        }

        public List<Vector2Int> GetPositionsInMask(List<T> mask)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for(int i = 0; i < data.Count; ++i)
            {
                if(mask.Contains(data[i]))
                {
                    positions.Add(GetPositionFromIndex(i));
                }
            }

            return positions;
        }

        public List<Vector2Int> GetPositionsNotInMask(List<T> mask)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            for(int i = 0; i < data.Count; ++i)
            {
                if(!mask.Contains(data[i]))
                {
                    positions.Add(GetPositionFromIndex(i));
                }
            }

            return positions;
        }


        public List<T> GetElementsInArea(Rect area)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<T> elements = new List<T>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    elements.Add(this[i, j]);
                }
            }

            return elements;
        }

        public List<Vector2Int> GetPositionsInArea(Rect area)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<Vector2Int> positions = new List<Vector2Int>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    positions.Add(new Vector2Int(i, j));
                }
            }

            return positions;
        }

        public List<T> GetElementsInAreaOfType(Rect area, T type)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<T> elements = new List<T>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;


                    if(EqualityComparer<T>.Default.Equals(this[i, j], type))
                        elements.Add(this[i, j]);
                }
            }

            return elements;
        }

        public List<Vector2Int> GetPositionsInAreaOfType(Rect area, T type)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<Vector2Int> positions = new List<Vector2Int>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;


                    if(EqualityComparer<T>.Default.Equals(this[i, j], type))
                        positions.Add(new Vector2Int(i, j));
                }
            }

            return positions;
        }

        public List<T> GetElementsInAreaInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<T> elements = new List<T>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    if(mask.Contains(this[i, j]))
                        elements.Add(this[i, j]);
                }
            }

            return elements;
        }

        public List<Vector2Int> GetPositionsInAreaInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<Vector2Int> elements = new List<Vector2Int>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    if(mask.Contains(this[i, j]))
                        elements.Add(new Vector2Int(i, j));
                }
            }

            return elements;
        }

        public List<T> GetElementsInAreaNotInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<T> elements = new List<T>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    if(!mask.Contains(this[i, j]))
                        elements.Add(this[i, j]);
                }
            }

            return elements;
        }

        public List<Vector2Int> GetPositionsInAreaNotInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return null;

            List<Vector2Int> elements = new List<Vector2Int>();

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    if(!mask.Contains(this[i, j]))
                        elements.Add(new Vector2Int(i, j));
                }
            }

            return elements;
        }

        public List<T> GetElementsInFloodFill(Vector2Int start_point, List<T> boundry_mask, bool search_diagonal)
        {
            if(!IsValidPosition(start_point))
                return null;

            //if we try to select a flood fill that starts on a boundry, abort
            if(boundry_mask.Contains(this[start_point]))
                return null;

            FloodFillData ffdata = new FloodFillData();

            ffdata.cellsToFill.Add(start_point);

            List<T> elements = new List<T>();

            //fill until we run out
            while(ffdata.cellsToFill.Count > 0)
            {
                Vector2Int p = ffdata.cellsToFill[0];

                Vector2Int left = new Vector2Int(p.x - 1, p.y);
                Vector2Int right = new Vector2Int(p.x + 1, p.y);
                Vector2Int up = new Vector2Int(p.x, p.y - 1);
                Vector2Int down = new Vector2Int(p.x, p.y + 1);

                FloodFillCheckAndAdd(this, left, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, right, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, up, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, down, ffdata, boundry_mask);

                if(search_diagonal)
                {
                    Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                    Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                    Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                    Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                    FloodFillCheckAndAdd(this, tl, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, tr, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, bl, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, br, ffdata, boundry_mask);
                }

                ffdata.cellsToFill.Remove(p);
                ffdata.visitedCells.Add(p);

                elements.Add(this[p]);
            }

            return elements;
        }


        public List<Vector2Int> GetPositionsInFloodFill(Vector2Int start_point, List<T> boundry_mask, bool search_diagonal)
        {
            if(!IsValidPosition(start_point))
                return null;

            //if we try to select a flood fill that starts on a boundry, abort
            if(boundry_mask.Contains(this[start_point]))
                return null;

            FloodFillData ffdata = new FloodFillData();

            ffdata.cellsToFill.Add(start_point);

            List<Vector2Int> elements = new List<Vector2Int>();

            //fill until we run out
            while(ffdata.cellsToFill.Count > 0)
            {
                Vector2Int p = ffdata.cellsToFill[0];

                Vector2Int left = new Vector2Int(p.x - 1, p.y);
                Vector2Int right = new Vector2Int(p.x + 1, p.y);
                Vector2Int up = new Vector2Int(p.x, p.y - 1);
                Vector2Int down = new Vector2Int(p.x, p.y + 1);

                FloodFillCheckAndAdd(this, left, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, right, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, up, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, down, ffdata, boundry_mask);

                if(search_diagonal)
                {
                    Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                    Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                    Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                    Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                    FloodFillCheckAndAdd(this, tl, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, tr, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, bl, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, br, ffdata, boundry_mask);
                }

                ffdata.cellsToFill.Remove(p);
                ffdata.visitedCells.Add(p);

                elements.Add(p);
            }

            return elements;
        }

        public void FloodFill(Vector2Int start_point, List<T> boundry_mask, bool search_diagonal, T fill_value)
        {
            if(!IsValidPosition(start_point))
                return;

            //if we try to select a flood fill that starts on a boundry, abort
            if(boundry_mask.Contains(this[start_point]))
                return;

            FloodFillData ffdata = new FloodFillData();

            ffdata.cellsToFill.Add(start_point);

            //fill until we run out
            while(ffdata.cellsToFill.Count > 0)
            {
                Vector2Int p = ffdata.cellsToFill[0];

                Vector2Int left = new Vector2Int(p.x - 1, p.y);
                Vector2Int right = new Vector2Int(p.x + 1, p.y);
                Vector2Int up = new Vector2Int(p.x, p.y - 1);
                Vector2Int down = new Vector2Int(p.x, p.y + 1);

                FloodFillCheckAndAdd(this, left, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, right, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, up, ffdata, boundry_mask);
                FloodFillCheckAndAdd(this, down, ffdata, boundry_mask);

                if(search_diagonal)
                {
                    Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                    Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                    Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                    Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                    FloodFillCheckAndAdd(this, tl, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, tr, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, bl, ffdata, boundry_mask);
                    FloodFillCheckAndAdd(this, br, ffdata, boundry_mask);
                }

                ffdata.cellsToFill.Remove(p);
                ffdata.visitedCells.Add(p);

                SetElement(p, fill_value);
            }
        }

        public void FillArea(Rect area, T fill_value)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return;

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    Vector2Int p = new Vector2Int(i, j);

                    if(!IsValidPosition(p))
                        continue;

                    SetElement(p, fill_value);
                }
            }
        }

        public void FillEdge(Rect area, T fill_value)
        {
            Mathnv.Clamp(ref area, ValidArea);

            if(!ValidRect(area))
                return;

            for(int j = IterJStart(area); j <= IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i <= IterIEnd(area); ++i)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if(j == IterJStart(area))
                    {
                        SetElement(p, fill_value);
                        continue;
                    }
                    if(i == IterIStart(area))
                    {
                        SetElement(p, fill_value);
                        continue;
                    }
                    if(j == IterJEnd(area))
                    {
                        SetElement(p, fill_value);
                        continue;
                    }
                    if(i == IterIEnd(area))
                    {
                        SetElement(p, fill_value);
                        continue;
                    }



                    //if( !ValidPosition( p ) )
                    //    continue;

                    //if( IsOnBoundry(area, p) == false )
                    //    continue;

                    //SetElement( p, fill_value );
                }
            }
        }

        public void FillAreaInMask(Rect area, List<T> mask, T fill_value)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return;

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if(!IsValidPosition(p))
                        continue;

                    if(mask.Contains(this[p]))
                        SetElement(p, fill_value);
                }
            }
        }

        public void FillAreaNotInMask(Rect area, List<T> mask, T fill_value)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return;

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if(!IsValidPosition(p))
                        continue;

                    if(!mask.Contains(this[p]))
                        SetElement(p, fill_value);
                }
            }
        }

        public void FillEdgeInMask(Rect area, List<T> mask, T fill_value)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return;

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if(!IsValidPosition(p))
                        continue;

                    if(IsOnBoundry(area, p) == false)
                        continue;

                    if(mask.Contains(this[p]))
                        SetElement(p, fill_value);
                }
            }
        }

        public void FillEdgeNotInMask(Rect area, List<T> mask, T fill_value)
        {
            Mathnv.Clamp(ref area, Area);

            if(!ValidRect(area))
                return;

            for(int j = IterJStart(area); j < IterJEnd(area); ++j)
            {
                for(int i = IterIStart(area); i < IterIEnd(area); ++i)
                {
                    Vector2Int p = new Vector2Int(i, j);
                    if(!IsValidPosition(p))
                        continue;

                    if(IsOnBoundry(area, p) == false)
                        continue;

                    if(!mask.Contains(this[p]))
                        SetElement(p, fill_value);
                }
            }
        }

        public T GetRandomElement()
        {
            Vector2Int p = Vector2Int.FloorToInt(GameRNG.Rand(ValidArea));
            return this[p];
        }

        public Vector2Int GetRandomPosition()
        {
            Vector2Int p = Vector2Int.FloorToInt(GameRNG.Rand(ValidArea));
            return p;
        }

        public T GetRandomNonEmptyElement()
        {
            List<T> elements = GetNonEmptyElements();

            if(elements.Count <= 0)
                return default(T);

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public Vector2Int? GetRandomNonEmptyPosition()
        {
            List<Vector2Int> elements = GetNonEmptyPositions();

            if(elements.Count <= 0)
                return null;

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        //Warning: Expensive call
        public T GetRandomEmptyElement()
        {
            List<T> elements = GetEmptyElements();

            if(elements.Count <= 0)
                return default(T);

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        //Warning: Expensive call
        public Vector2Int? GetRandomEmptyPosition()
        {
            List<Vector2Int> elements = GetEmptyPositions();

            if(elements.Count <= 0)
                return null;

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public T GetRandomElementOfType(T type)
        {
            List<T> elements = GetElementsOfType(type);

            if(elements.Count <= 0)
                return default(T);

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public Vector2Int? GetRandomPositionOfType(T type)
        {
            List<Vector2Int> elements = GetPositionsOfType(type);

            if(elements.Count <= 0)
                return null;

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public T GetRandomElementInArea(Rect area)
        {
            Mathnv.Clamp(ref area, ValidArea);

            Vector2Int p = Vector2Int.FloorToInt(GameRNG.Rand(area));

            return this[p];
        }

        public Vector2Int? GetRandomPositionInArea(Rect area)
        {
            Mathnv.Clamp(ref area, ValidArea);
            
            Vector2Int p = Vector2Int.FloorToInt(GameRNG.Rand(area));

            return p;
        }

        public T GetRandomElementInAreaOfType(Rect area, T type)
        {
            Mathnv.Clamp(ref area, Area);

            List<T> elements = GetElementsInAreaOfType(area, type);

            if(elements.Count <= 0)
                return default(T);

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public Vector2Int? GetRandomPositionInAreaOfType(Rect area, T type)
        {
            Mathnv.Clamp(ref area, Area);

            List<Vector2Int> elements = GetPositionsInAreaOfType(area, type);

            if(elements.Count <= 0)
                return null;

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public T GetRandomElementInAreaInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            List<T> elements = GetElementsInAreaInMask(area, mask);

            if(elements.Count <= 0)
                return default(T);

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public Vector2Int? GetRandomPositionInAreaInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            List<Vector2Int> elements = GetPositionsInAreaInMask(area, mask);

            if(elements.Count <= 0)
                return null;

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public T GetRandomElementInAreaNotInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            List<T> elements = GetElementsInAreaNotInMask(area, mask);

            if(elements.Count <= 0)
                return default(T);

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public Vector2Int? GetRandomPositionInAreaNotInMask(Rect area, List<T> mask)
        {
            Mathnv.Clamp(ref area, Area);

            List<Vector2Int> elements = GetPositionsInAreaNotInMask(area, mask);

            if(elements.Count <= 0)
                return null;

            int i = GameRNG.Rand(elements.Count);

            return elements[i];
        }

        public Rect GetRandomArea(Vector2Int size)
        {
            Rect area = new Rect(Vector2Int.zero, Size);

            if(size.x > w)
                return area;
            if(size.y > h)
                return area;

            area = new Rect(Vector2Int.zero, size);

            Vector2Int p = Vector2Int.FloorToInt(GameRNG.Rand(ValidArea));

            if(p.x + size.x > w)
            {
                p.x += w - (p.x + size.x);
            }

            if(p.y + size.y > h)
            {
                p.y += h - (p.y + size.y);
            }

            area.position = p;

            return area;
        }

        public List<T> GetAdjacentElements(Vector2Int pos, bool search_diagonal)
        {
            if(!IsValidPosition(pos))
                return null;

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAdd(this, left, ffdata);
            FloodFillAdd(this, right, ffdata);
            FloodFillAdd(this, up, ffdata);
            FloodFillAdd(this, down, ffdata);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAdd(this, tl, ffdata);
                FloodFillAdd(this, tr, ffdata);
                FloodFillAdd(this, bl, ffdata);
                FloodFillAdd(this, br, ffdata);
            }

            return GetElements(ffdata.cellsToFill);
        }


        public List<Vector2Int> GetAdjacentPositions(Vector2Int pos, bool search_diagonal)
        {
            if(!IsValidPosition(pos))
                return null;

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAdd(this, left, ffdata);
            FloodFillAdd(this, right, ffdata);
            FloodFillAdd(this, up, ffdata);
            FloodFillAdd(this, down, ffdata);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAdd(this, tl, ffdata);
                FloodFillAdd(this, tr, ffdata);
                FloodFillAdd(this, bl, ffdata);
                FloodFillAdd(this, br, ffdata);
            }

            return ffdata.cellsToFill;
        }

        public List<T> GetAdjacentElementsOfType(Vector2Int pos, bool search_diagonal, T type)
        {
            if(!IsValidPosition(pos))
                return null;

            FloodFillData ffdata = new FloodFillData();
                        
            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAddIfType(this, left, ffdata, type);
            FloodFillAddIfType(this, right, ffdata, type);
            FloodFillAddIfType(this, up, ffdata, type);
            FloodFillAddIfType(this, down, ffdata, type);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAddIfType(this, tl, ffdata, type);
                FloodFillAddIfType(this, tr, ffdata, type);
                FloodFillAddIfType(this, bl, ffdata, type);
                FloodFillAddIfType(this, br, ffdata, type);
            }

            return GetElements(ffdata.cellsToFill);
        }


        public List<Vector2Int> GetAdjacentPositionsOfType(Vector2Int pos, bool search_diagonal, T type)
        {
            if(!IsValidPosition(pos))
                return null;

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAddIfType(this, left, ffdata, type);
            FloodFillAddIfType(this, right, ffdata, type);
            FloodFillAddIfType(this, up, ffdata, type);
            FloodFillAddIfType(this, down, ffdata, type);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAddIfType(this, tl, ffdata, type);
                FloodFillAddIfType(this, tr, ffdata, type);
                FloodFillAddIfType(this, bl, ffdata, type);
                FloodFillAddIfType(this, br, ffdata, type);
            }

            return ffdata.cellsToFill;
        }

        public List<T> GetAdjacentElementsNotOfType(Vector2Int pos, bool search_diagonal, T type)
        {
            if(!IsValidPosition(pos))
                return null;

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAddIfNotType(this, left, ffdata, type);
            FloodFillAddIfNotType(this, right, ffdata, type);
            FloodFillAddIfNotType(this, up, ffdata, type);
            FloodFillAddIfNotType(this, down, ffdata, type);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAddIfNotType(this, tl, ffdata, type);
                FloodFillAddIfNotType(this, tr, ffdata, type);
                FloodFillAddIfNotType(this, bl, ffdata, type);
                FloodFillAddIfNotType(this, br, ffdata, type);
            }

            return GetElements(ffdata.cellsToFill);
        }


        public List<Vector2Int> GetAdjacentPositionsNotOfType(Vector2Int pos, bool search_diagonal, T type)
        {
            if(!IsValidPosition(pos))
                return null;

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAddIfNotType(this, left, ffdata, type);
            FloodFillAddIfNotType(this, right, ffdata, type);
            FloodFillAddIfNotType(this, up, ffdata, type);
            FloodFillAddIfNotType(this, down, ffdata, type);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAddIfNotType(this, tl, ffdata, type);
                FloodFillAddIfNotType(this, tr, ffdata, type);
                FloodFillAddIfNotType(this, bl, ffdata, type);
                FloodFillAddIfNotType(this, br, ffdata, type);
            }

            return ffdata.cellsToFill;
        }

        public List<T> GetAdjacentEmptyElements(Vector2Int pos, bool search_diagonal)
        {
            return GetAdjacentElementsOfType(pos, search_diagonal, default(T));
        }

        public List<Vector2Int> GetAdjacentEmptyPositions(Vector2Int pos, bool search_diagonal)
        {
            return GetAdjacentPositionsOfType(pos, search_diagonal, default(T));
        }

        public List<T> GetAdjacentNonEmptyElements(Vector2Int pos, bool search_diagonal)
        {
            if(!IsValidPosition(pos))
                return null;

            T type = default(T);

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAddIfNotType(this, left, ffdata, type);
            FloodFillAddIfNotType(this, right, ffdata, type);
            FloodFillAddIfNotType(this, up, ffdata, type);
            FloodFillAddIfNotType(this, down, ffdata, type);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAddIfNotType(this, tl, ffdata, type);
                FloodFillAddIfNotType(this, tr, ffdata, type);
                FloodFillAddIfNotType(this, bl, ffdata, type);
                FloodFillAddIfNotType(this, br, ffdata, type);
            }

            return GetElements(ffdata.cellsToFill);
        }

        public List<Vector2Int> GetAdjacentNonEmptyPositions(Vector2Int pos, bool search_diagonal)
        {
            if(!IsValidPosition(pos))
                return null;

            T type = default(T);

            FloodFillData ffdata = new FloodFillData();

            Vector2Int p = pos;

            Vector2Int left = new Vector2Int(p.x - 1, p.y);
            Vector2Int right = new Vector2Int(p.x + 1, p.y);
            Vector2Int up = new Vector2Int(p.x, p.y - 1);
            Vector2Int down = new Vector2Int(p.x, p.y + 1);

            FloodFillAddIfNotType(this, left, ffdata, type);
            FloodFillAddIfNotType(this, right, ffdata, type);
            FloodFillAddIfNotType(this, up, ffdata, type);
            FloodFillAddIfNotType(this, down, ffdata, type);

            if(search_diagonal)
            {
                Vector2Int tl = new Vector2Int(p.x - 1, p.y - 1);
                Vector2Int tr = new Vector2Int(p.x + 1, p.y - 1);
                Vector2Int bl = new Vector2Int(p.x - 1, p.y + 1);
                Vector2Int br = new Vector2Int(p.x + 1, p.y + 1);

                FloodFillAddIfNotType(this, tl, ffdata, type);
                FloodFillAddIfNotType(this, tr, ffdata, type);
                FloodFillAddIfNotType(this, bl, ffdata, type);
                FloodFillAddIfNotType(this, br, ffdata, type);
            }

            return ffdata.cellsToFill;
        }

        public void SetElements(List<Vector2Int> positions, List<T> elements)
        {
            if(elements == null)
                return;
            if(positions == null)
                return;
            if(positions.Count != elements.Count)
                return;

            for(int i = 0; i < elements.Count; ++i)
            {
                if(elements[i] != null)
                    SetElement(positions[i], elements[i]);
            }
        }

        public void SetElements(List<Vector2Int> positions, T element)
        {
            if(element == null)
                return;
            if(positions == null)
                return;

            for(int i = 0; i < positions.Count; ++i)
            {
                SetElement(positions[i], element);
            }
        }

        public static void FillElements(ref List<T> elements, T type)
        {
            if(elements == null)
                return;

            for(int i = 0; i < elements.Count; ++i)
            {
                if(elements[i] != null)
                    elements[i] = type;
            }
        }
                
        public List<Vector2Int> GetPositionsOfAreasOfType(T type, Vector2Int desiredSize)
        {
            List<Vector2Int> areas = new List<Vector2Int>();
            ArrayGrid <int> goodPoints = new ArrayGrid<int>(Size, 0);

            for(int j = IterJStart(Area); j < IterJEnd(Area); ++j)
            {
                int horizontalCount = 0;
                for(int i = IterIStart(Area); i < IterIEnd(Area); ++i)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    if(EqualityComparer<T>.Default.Equals(this[i, j], type))
                    {
                        horizontalCount++;
                    }
                    else
                    {
                        horizontalCount = 0;
                    }

                    if(horizontalCount == (int)desiredSize.x)
                    {
                        goodPoints[1 + i - (int)desiredSize.x, j] = 1;
                        horizontalCount--;
                    }
                }
            }

            for(int i = IterIStart(Area); i < IterIEnd(Area); ++i)
            {
                int verticalCount = 0;
                for(int j = IterJStart(Area); j < IterJEnd(Area); ++j)
                {
                    if(!IsValidPosition(i, j))
                        continue;

                    if(goodPoints[i, j] == 1)
                    {
                        verticalCount++;
                    }
                    else
                    {
                        verticalCount = 0;
                    }

                    if(verticalCount == (int)desiredSize.y)
                    {
                        areas.Add(new Vector2Int(i, 1 + j - (int)desiredSize.y));
                        verticalCount--;
                    }
                }
            }

            return areas;
        }

        public bool GetPositionOfRandomAreaOfType(T type, Vector2Int desiredSize, ref Vector2Int position)
        {
            List<Vector2Int> areas = GetPositionsOfAreasOfType(type, desiredSize);
            if(areas.Count <= 0)
                return false;

            position = areas.GetRandomElementFromList();

            return true;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Elements).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)Elements).GetEnumerator();
        }
    }

}