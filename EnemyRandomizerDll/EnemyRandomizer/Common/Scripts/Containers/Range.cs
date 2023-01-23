using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


namespace nv
{
    [System.Serializable]
    public class Range : IComparable<Range>
    {
        [SerializeField]
        float from;

        [SerializeField]
        float to;
        
        public float From
        {
            get
            {
                return from;
            }
            set
            {
                from = value;
            }
        }

        public float To
        {
            get
            {
                return to;
            }
            set
            {
                to = value;
            }
        }

        /// <summary>
        /// Will always get/set the smaller of the two values.
        /// </summary>
        public float Min
        {
            get
            {
                return Mathf.Min(From, To);
            }
        }

        /// <summary>
        /// Will always get/set the larger of the two values.
        /// </summary>
        public float Max
        {
            get
            {
                return Mathf.Max(From, To);
            }
        }

        public float Size
        {
            get
            {
                return (Max - Min);
            }
        }

        public float Distance
        {
            get
            {
                return (to - from);
            }
        }

        public Range()
        {
            From = 0f;
            To = 1f;
        }

        public Range(float min, float max)
        {
            From = min;
            To = max;
        }
        
        public Range(Vector2 minMax)
        {
            From = minMax.x;
            To = minMax.y;
        }

        public Range(float size)
        {
            From = 0f;
            To = size;
        }

        public Range(AnimationCurve data, bool getDataFromXAxis = false)
        {
            From = 0f;
            To = 1f;

            if(data.length < 1)
                return;

            if(getDataFromXAxis)
            {
                From = data.keys.Min(x => x.time);
                To = data.keys.Max(x => x.time);
            }
            else
            {
                From = data.keys.Min(x => x.value);
                To = data.keys.Max(x => x.value);
            }
        }

        public float this[int i]
        {
            get
            {
                i = Mathf.Clamp(i, 0, 1);
                if(i == 0)
                    return From;
                return To;
            }
            set
            {
                i = Mathf.Clamp(i, 0, 1);
                if(i == 0)
                    From = value;
                To = value;
            }
        }

        public static implicit operator List<float>(Range r)
        {
            return r.ToList();
        }

        public static implicit operator float[](Range r)
        {
            return r.ToArray();
        }

        public static implicit operator KeyValuePair<float, float>(Range r)
        {
            return new KeyValuePair<float, float>(r.Min, r.Max);
        }

        public static implicit operator Vector2(Range r)
        {
            return new Vector2(r.Min, r.Max);
        }

        public static implicit operator Range(List<float> collection)
        {
            return new Range(collection.Min(), collection.Max());
        }

        public static implicit operator Range(KeyValuePair<float, float> pair)
        {
            return new Range(Mathf.Min(pair.Key, pair.Value), Mathf.Max(pair.Key, pair.Value));
        }

        public static implicit operator Range(Vector2 v)
        {
            return new Range(v.x, v.y);
        }

        public static Range operator *(Range r, float s)
        {
            return new Range(r.Min * s, r.Max * s);
        }

        public static Range operator +(Range r, float s)
        {
            return new Range(r.Min + s, r.Max + s);
        }

        public static Range operator -(Range r, float s)
        {
            return new Range(r.Min - s, r.Max - s);
        }

        public static Range operator /(Range r, float s)
        {
            return new Range(r.Min / s, r.Max / s);
        }

        /// <summary>
        /// Returns the value at normalizedTime where normalizedTime is a [0,1] float. Input outside this range will be clamped.
        /// </summary>
        /// <param name="normalizedTime">A [0,1] float. Input outside this range will be clamped.</param>
        /// <returns>The corrosponding value</returns>
        public float Evaluate(float normalizedTime)
        {
            normalizedTime = Mathf.Clamp01(normalizedTime);
            return Min + Size * normalizedTime;
        }

        /// <summary>
        /// Determins if the value is inside this range.
        /// </summary>
        public bool Contains(float x)
        {
            if(x < Min)
                return false;
            if(x > Max)
                return false;
            return true;
        }

        /// <summary>
        /// Determins if the given range is inside this range.
        /// </summary>
        public bool Contains(Range r)
        {
            if(!Contains(r.Min))
                return false;
            if(!Contains(r.Max))
                return false;
            return true;
        }

        /// <summary>
        /// Returns the value normalized to the range. Input outside this range will be clamped.
        /// </summary>
        /// <param name="x">A float in the range. Input outside this range will be clamped.</param>
        /// <returns>The corrosponding value</returns>
        public float NormalizedValue(float x)
        {
            x = Mathf.Clamp(x, Min, Max);
            return (x - Min) / Size;
        }

        public int RandomValuei(RNG rng)
        {
            return rng.Rand((int)Min, (int)Max);
        }

        public float RandomValuef(RNG rng)
        {
            return rng.Rand(Min, Max);
        }

        public float RandomNormalizedValue(RNG rng)
        {
            return NormalizedValue(rng.Rand(Min, Max));
        }

        public override string ToString()
        {
            return $"[+{Min.ToString()}+,+{Max.ToString()}+]";
        }

        /// <summary>
        /// Convert the range into a set of evenly distributed discrete steps of size 1.
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        public List<int> ToSteps()
        {
            List<int> sets = new List<int>();
            sets.Add((int)Min);
            for(int i = 1; i <= Size; ++i)
            {
                sets.Add((int)Min + i);
            }
            return sets;
        }

        /// <summary>
        /// Convert the range into a set of discrete steps. Example, [0,1] with 10 steps will give a list of [0,.1,.2,.3, .., 1]
        /// </summary>
        /// <param name="steps"></param>
        /// <returns></returns>
        public List<float> ToSteps(int steps)
        {
            float stepSize = Size / steps;
            List<float> sets = new List<float>();
            sets.Add(Min);
            for(int i = 1; i <= steps; ++i)
            {
                sets.Add(Min + stepSize * i);
            }
            return sets;
        }

        public List<float> ToList()
        {
            return new List<float>() { Min, Max };
        }

        public float[] ToArray()
        {
            return new float[] { Min, Max };
        }

        /// <summary>
        /// Ranges are sorted by min value, then max value
        /// </summary>
        public int CompareTo(Range other)
        {
            if(Min < other.Min)
                return -1;
            else if(Min > other.Min)
                return 1;
            else
            {
                if(Max < other.Max)
                    return -1;
                else if(Max > other.Max)
                    return 1;
            }
            return 0;
        }
    }
}