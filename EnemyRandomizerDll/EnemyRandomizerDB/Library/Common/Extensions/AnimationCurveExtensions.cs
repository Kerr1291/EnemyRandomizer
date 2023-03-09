using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EnemyRandomizerMod
{
    public static class AnimationCurveExtensions
    {
        public static AnimationCurve Normalize(this AnimationCurve curve, bool xAxis = true, bool yAxis = true)
        {
            AnimationCurve output = curve;
            if(output == null || output.keys.Length < 2)
            {
                output = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
            }
            else
            {
                if(!xAxis && !yAxis)
                    return output;

                float xmin = float.MaxValue;
                float ymin = float.MaxValue;

                float xmax = float.MinValue;
                float ymax = float.MinValue;

                var keyframes = output.keys;

                if(xAxis)
                {
                    xmin = keyframes.Min(x => x.time);
                    xmax = keyframes.Max(x => x.time);
                }

                float xRange = xmax - xmin;

                if(yAxis)
                {
                    ymin = keyframes.Min(x => x.value);
                    ymax = keyframes.Max(x => x.value);
                }

                float yRange = ymax - ymin;

                keyframes = keyframes.Select(x =>
                {
                    if(xAxis)
                    {
                        x.time = (x.time - xmin) / xRange;
                    }
                    if(yAxis)
                    {
                        x.value = (x.value - ymin) / yRange;
                    }
                    return x;
                }).ToArray();

                output.keys = keyframes;
            }
            return output;
        }

        public static AnimationCurve NormalizeTo(this AnimationCurve curve, float min, float max, bool xAxis = true, bool yAxis = true)
        {
            AnimationCurve output = curve;
            if(output == null || output.keys.Length < 2)
            {
                output = new AnimationCurve(new Keyframe(min, min), new Keyframe(max, max));
            }
            else
            {
                if(!xAxis && !yAxis)
                    return output;

                var keyframes = output.keys;

                float xRange = max - min;
                float yRange = max - min;

                keyframes = keyframes.Select(x =>
                {
                    if(xAxis)
                    {
                        x.time = (x.time - min) / xRange;
                        x.time = Mathf.LerpUnclamped(min, max, x.time);
                    }
                    if(yAxis)
                    {
                        x.value = (x.value - min) / yRange;
                        x.value = Mathf.LerpUnclamped(min, max, x.value);
                    }
                    return x;
                }).ToArray();

                output.keys = keyframes;
            }
            return output;
        }

        /// <summary>
        /// Force the curve's first and last keyframe to equal the given values
        /// </summary>
        public static AnimationCurve FixEndpoints(this AnimationCurve curve, Vector2 start, Vector2 end)
        {
            return curve.FixEndpoints(new Keyframe(start.x,start.y), new Keyframe(end.x, end.y));
        }

        /// <summary>
        /// Force the curve's first and last keyframe to equal the given values
        /// </summary>
        public static AnimationCurve FixEndpoints(this AnimationCurve curve, Keyframe start, Keyframe end)
        {
            AnimationCurve output = curve;
            if(output == null || output.keys.Length < 2)
            {
                output = new AnimationCurve(start, end);
            }
            else
            {
                var keyframes = output.keys;

                keyframes[0] = start;
                keyframes[keyframes.Length-1] = end;

                output.keys = keyframes;
            }
            return output;
        }
    }
}