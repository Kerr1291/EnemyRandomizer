using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public static class Mathnv
    {
        //can't remember the link, but taken from stack overflow or a unity article
        public static bool FastApproximately( float a, float b, float threshold )
        {
            return ( ( a - b ) < 0 ? ( ( a - b ) * -1 ) : ( a - b ) ) <= threshold;
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        public static Vector2 Clamp( Vector2 value, Vector2 min, Vector2 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );

            return value;
        }

        public static Vector2 Clamp01( Vector2 value )
        {
            value.x = Mathf.Clamp( value.x, Vector2.zero.x, Vector2.one.x );
            value.y = Mathf.Clamp( value.y, Vector2.zero.y, Vector2.one.y );

            return value;
        }

        public static Vector3 Clamp( Vector3 value, Vector3 min, Vector3 max )
        {
            value.x = Mathf.Clamp( value.x, min.x, max.x );
            value.y = Mathf.Clamp( value.y, min.y, max.y );
            value.z = Mathf.Clamp( value.z, min.z, max.z );

            return value;
        }

        public static Vector3 Clamp01( Vector3 value )
        {
            value.x = Mathf.Clamp( value.x, Vector3.zero.x, Vector3.one.x );
            value.y = Mathf.Clamp( value.y, Vector3.zero.y, Vector3.one.y );
            value.z = Mathf.Clamp( value.z, Vector3.zero.z, Vector3.one.z );

            return value;
        }

        public static Vector3 RotateToLocalSpace( Vector3 input, Transform localSpace )
        {
            Vector4 p = input;
            Quaternion pq = new Quaternion(p.x, p.y, p.z, 0);
            pq = localSpace.localRotation * pq * Quaternion.Inverse( localSpace.localRotation );
            Vector3 vout = new Vector3(pq.x, pq.y, pq.z);
            return vout;
        }

        public static bool Contains( Vector2 pos, Vector2 min, Vector2 max )
        {
            if( pos.x < min.x )
                return false;
            if( pos.x >= max.x )
                return false;
            if( pos.y < min.y )
                return false;
            if( pos.y >= max.y )
                return false;
            return true;
        }

        public static bool Contains( int x, int y, Vector2 min, Vector2 max )
        {
            if( x < (int)min.x )
                return false;
            if( x >= (int)max.x )
                return false;
            if( y < (int)min.y )
                return false;
            if( y >= (int)max.y )
                return false;
            return true;
        }

        public static Vector2 RectTopLeft( Rect input )
        {
            return input.TopLeft();
        }

        public static Vector2 RectBottomRight( Rect input )
        {
            return input.BottomRight();
        }

        public static void Swap<T>( ref T lhs, ref T rhs )
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void Swap<T>( IList<T> list, int indexA, int indexB)
        {
            T temp;
            temp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temp;
        }

        public static void Sort2<T>( ref T out_val0, ref T out_val1 ) where T : System.IComparable<T>
        {
            if( out_val0.CompareTo( out_val1 ) > 0 )
                Swap( ref out_val0, ref out_val1 );
        }

        public static void Clamp( ref Vector2 pos, Rect area )
        {
            pos.x = Mathf.Clamp(pos.x, area.xMin, area.xMax);
            pos.y = Mathf.Clamp(pos.y, area.yMin, area.yMax);
        }

        public static void Clamp( ref Rect area, Vector2 pos, Vector2 max_dimensions )
        {
            Rect clampRect = new Rect(pos, max_dimensions);
            area = Clamp(area, clampRect);
        }

        public static void Clamp( ref Rect area, Rect min_max )
        {
            area = Clamp(area, min_max);
        }

        public static Rect Clamp( Rect area, Rect min_max )
        {
            float xMin = Mathf.Max( area.xMin, min_max.xMin );
            float yMin = Mathf.Max( area.yMin, min_max.yMin );
            float xMax = Mathf.Min( area.xMax, min_max.xMax );
            float yMax = Mathf.Min( area.yMax, min_max.yMax );
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        /// <summary>
        /// Iterate over a range, [from, to). 
        /// Note that the the 'to' element is exclusive.
        /// </summary>
        public static IEnumerator<int> GetRangeEnumerator(int from, int to)
        {
            for(int i = from; i < to; ++i)
            {
                yield return i;
            }
            yield break;
        }

        public static IEnumerator<int> GetRangeEnumerator(Range range)
        {
            for(int i = (int)range.Min; i < (int)range.Max; ++i)
            {
                yield return i;
            }
            yield break;
        }

        public static IEnumerator<Vector2Int> GetAreaEnumerator(int w, int h)
        {
            var yIter = GetRangeEnumerator(0, h);

            while(yIter.MoveNext())
            {
                var xIter = GetRangeEnumerator(0, w);

                while(xIter.MoveNext())
                {
                    yield return new Vector2Int(xIter.Current, yIter.Current);
                }
            }
        }

        public static IEnumerator<Vector2Int> GetAreaEnumerator( int w, int h, bool transpose )
        {
            if( transpose )
            {
                var yIter = GetRangeEnumerator( 0, h );

                while( yIter.MoveNext() )
                {
                    var xIter = GetRangeEnumerator( 0, w );

                    while( xIter.MoveNext() )
                    {
                        yield return new Vector2Int( yIter.Current, xIter.Current );
                    }
                }
            }
            else
            {
                IEnumerator<Vector2Int> iterator = GetAreaEnumerator( w, h );
                while( iterator.MoveNext() )
                    yield return iterator.Current;
            }
        }

        public static IEnumerator<Vector2Int> GetAreaEnumerator(Vector2Int size, bool transpose = false)
        {
            IEnumerator<Vector2Int> iterator = GetAreaEnumerator(size.x, size.y, transpose );
            while(iterator.MoveNext())
                yield return iterator.Current;
        }

        public static IEnumerator<Vector2Int> GetAreaEnumerator(int fromX, int fromY, int toX, int toY)
        {
            var yIter = GetRangeEnumerator(fromY, toY);

            while(yIter.MoveNext())
            {
                var xIter = GetRangeEnumerator(fromX, toX);

                while(xIter.MoveNext())
                {
                    yield return new Vector2Int(xIter.Current, yIter.Current);
                }
            }
        }

        public static IEnumerator<Vector2Int> GetAreaEnumerator( int fromX, int fromY, int toX, int toY, bool transpose )
        {
            if( transpose )
            {
                var yIter = GetRangeEnumerator( fromY, toY );

                while( yIter.MoveNext() )
                {
                    var xIter = GetRangeEnumerator( fromX, toX );

                    while( xIter.MoveNext() )
                    {
                        yield return new Vector2Int( yIter.Current, xIter.Current );
                    }
                }
            }
            else
            {
                IEnumerator<Vector2Int> iterator = GetAreaEnumerator( fromX, fromY, toX, toY );
                while( iterator.MoveNext() )
                    yield return iterator.Current;
            }
        }

        public static IEnumerator<Vector2Int> GetAreaEnumerator(Vector2Int from, Vector2Int to, bool transpose = false)
        {
            IEnumerator<Vector2Int> iterator = GetAreaEnumerator(from.x, from.y, to.x, to.y, transpose);
            while(iterator.MoveNext())
                yield return iterator.Current;
        }

        /// <summary>
        /// Mathematical modulus, different from the % operation that returns the remainder.
        /// This performs a "wrap around" of the given value assuming the range [0, mod). This does work with negative values.
        /// Define mod 0 to return the value unmodified if it's greater than zero. Else return 0.
        /// </summary>
        public static int Modulus(int value, int mod)
        {
            if(mod == 0)
                return value >= 0 ? value : mod;
            if(value > 0)
                return (value % mod);
            else if(value < 0)
                return (value % mod + mod) % mod;
            else
                return value;
        }


        public static Vector3 GetVectorTo(Vector2 origin, Vector2 dir, float max, Func<GameObject, bool> isRaycastHitObject)
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin, direction, max, Physics2D.AllLayers);

            if (toGround != null)
            {
                foreach (var v in toGround)
                {
                    //v.collider.gameObject.IsSurfaceOrPlatform())
                    if (isRaycastHitObject(v.collider.gameObject))
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }

            return Vector3.one * max;
        }

        public static Vector2 GetNearestVectorTo(Vector2 origin, float max, Func<GameObject, bool> isRaycastHitObject)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                Physics2D.RaycastAll(origin, Vector2.up, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.left, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.right, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(v => isRaycastHitObject(v.collider.gameObject));
            var sorted = hitsWithChunks.Select(x => x.point - origin).OrderBy(y => y.sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector2 GetNearestVectorDown(Vector2 origin, float max, Func<GameObject, bool> isRaycastHitObject)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(v => isRaycastHitObject(v.collider.gameObject));
            var sorted = hitsWithChunks.Select(x => x.point - origin).OrderBy(y => y.sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetNearestPointDown(Vector2 origin, float max, Func<GameObject, bool> isRaycastHitObject)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(v => isRaycastHitObject(v.collider.gameObject));
            var sorted = hitsWithChunks.Select(x => x.point).OrderBy(y => (y - origin).sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetNearestPointOn(Vector2 origin, float max, Func<GameObject, bool> isRaycastHitObject)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                Physics2D.RaycastAll(origin, Vector2.up, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.left, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.right, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(v => isRaycastHitObject(v.collider.gameObject));
            var sorted = hitsWithChunks.Select(x => x.point).OrderBy(y => (y - origin).sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetPointOn(Vector2 origin, Vector2 dir, float max, Func<GameObject, bool> isRaycastHitObject)
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin, direction, max, Physics2D.AllLayers);

            Vector2 lastGoodPoint = direction * max;

            if (toGround != null)
            {
                foreach (var v in toGround)
                {
                    if (isRaycastHitObject(v.collider.gameObject))
                    {
                        return v.point;
                    }
                    else
                    {
                        float newDist = (v.point - origin).magnitude;
                        float oldDist = (lastGoodPoint - origin).magnitude;

                        if (newDist < oldDist)
                        {
                            lastGoodPoint = v.point;
                        }
                    }
                }
            }
            return lastGoodPoint;
        }
    }
}
