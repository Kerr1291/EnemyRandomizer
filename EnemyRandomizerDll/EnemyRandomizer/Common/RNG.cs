using System;
using UnityEngine;
using System.Collections.Generic;

using Meisui.Random;

namespace nv
{
    /// <summary>
    /// This class may be used to produce a sequence of pseudorandom numbers using the mersene twister algorithm
    /// </summary>
    [Serializable]
    public class RNG
    {
        [SerializeField]
        MersenneTwister _mtRNG;

        MersenneTwister mtRNG {
            get {
                if( _mtRNG == null )
                    Reset();
                return _mtRNG;
            }
            set {
                _mtRNG = value;
            }
        }

        [SerializeField]
        int _seed;

        public int Seed {
            get {
                return _seed;
            }
            set {
                Reset(value);
            }
        }

        public RNG()
        {
        }

        public RNG( int s )
        {
            Reset(s);
        }

        //Reset the RNG internal engine with a new random seed
        public void Reset()
        {
            int newseed = UnityEngine.Random.Range( Int32.MinValue, Int32.MaxValue );
            Reset( newseed );
        }

        //Reset the RNG internal engine with a given seed
        public void Reset( int s )
        {
            _seed = s;
            UInt32 i32seed = (UInt32)( s );
            mtRNG = new MersenneTwister( i32seed );
        }

        public uint Rand()
        {
            return mtRNG.genrand_Int32();
        }

        public uint Rand( uint r )
        {
            return Rand() % r;
        }

        //Generates a [0-N] int
        public int Rand( int value )
        {
            int random_value;
            if( value == 0 )
                return 0;

            random_value = (int)( ( (float)mtRNG.genrand_Int32() / (float)0xFFFFFFFF ) * ( value ) );
            return random_value;
        }

        public int Randi()
        {
            return (int)Randl();
        }

        public long Randl()
        {
            return mtRNG.genrand_Int31();
        }

        //Generates a [0-1] float
        public float Randf()
        {
            return (float)( mtRNG.genrand_real1() );
        }

        //Generates a [0-N] float
        public float Rand( float value )
        {
            return Randf() * value;
        }

        public double Randd()
        {
            return mtRNG.genrand_real1();
        }

        //Generates a [0-N] double
        public double Rand( double a )
        {
            double n = Randd() * a;
            return n;
        }

        public uint Rand( uint a, uint b )
        {
            if( a == 0 ) return Rand( b );
            if( a > b ) Mathnv.Swap( ref a, ref b );
            uint n = 2 * b;
            uint m = a + b;
            uint c = n % m;
            return a + Rand() % c;
        }

        public int Rand( int a, int b )
        {
            if( a > b ) Mathnv.Swap( ref a, ref b );

            int c = b - a;
            return a + Randi() % c;
        }

        public float Rand( float a, float b )
        {
            if( a > b ) Mathnv.Swap( ref a, ref b );

            float c = b - a;
            return a + Randf() * c;
        }

        public double Rand( double a, double b )
        {
            if( a > b ) Mathnv.Swap( ref a, ref b );

            double c = b - a;
            return a + Randd() * c;
        }

        // rolling min or max will be rare, but rolling exactly between the two will be common
        public double GaussianRandom( double min, double max )
        {
            if( min > max ) Mathnv.Swap<double>( ref min, ref max );
            min /= 3;
            max /= 3;
            return Rand( min, max ) + Rand( min, max ) + Rand( min, max ); ;
        }

        //random point from [ [0f,0f] , [a.x,a.y] ]
        public Vector2 Rand( Vector2 a )
        {
            float x = Randf() * a.x;
            float y = Randf() * a.y;
            return new Vector2( x, y );
        }

        //random point in an area
        public Vector2 Rand( Vector2 a, Vector2 b )
        {
            if( a.x > b.x ) Mathnv.Swap( ref a.x, ref b.x );
            if( a.y > b.y ) Mathnv.Swap( ref a.y, ref b.y );

            float cx = b.x - a.x;
            float cy = b.y - a.y;
            return new Vector2( a.x + Randf() * cx, a.y + Randf() * cy );
        }

        //random point in an area
        public Vector2 Rand( Rect a )
        {
            float cx = a.width;
            float cy = a.height;
            return new Vector2( a.x + Randf() * cx, a.y + Randf() * cy );
        }

        // random vector
        //from [ [min.x,min.y] , [max.x,max.y] ]
        public Vector3 RandVec3( float min, float max )
        {
            Vector3 vec = new Vector3(Rand(min, max), Rand(min, max), Rand(min, max)).normalized;
            return vec;
        }

        // random normalized vector
        //from [ [min.x,min.y] , [max.x,max.y] ]
        public Vector3 RandVec3Normalized( float min, float max )
        {
            if( min > max ) Mathnv.Swap( ref min, ref max );
            Vector3 vec = new Vector3(Rand(min, max), Rand(min, max), Rand(min, max)).normalized;
            vec.Normalize();
            return vec;
        }

        //Returns true if generated value [0-value] is less than limit
        public bool RandomLowerThanLimit( int limit, int value )
        {
            if( value == 0 )
                return false;
            return ( Rand( value ) < limit );
        }

        //50/50 RNG
        public bool CoinToss()
        {
            return Rand( 2 ) != 0;
        }

        //uses a given distribution and a range to weight the outcomes
        //NOTE: Assumes the X range of the curve to be [0,1]
        public int WeightedRand(AnimationCurve distribution, int min, int max)
        {
            float range = max - min;

            //integrate the curve
            float maxWeight = 0f;
            for(int i = 0; i < range; ++i)
            {
                float t = i;
                maxWeight += distribution.Evaluate(t / range);
            }

            float rngValue = Rand(maxWeight);

            //find the outcome
            float sum = 0f;
            for(int i = 0; i < range; ++i)
            {
                float t = i;
                sum += distribution.Evaluate(t / range);
                if(sum > rngValue)
                    return min + i;
            }
            return max;
        }

        //uses a given distribution and a range to weight the outcomes
        //returns the selected random index
        public int WeightedRand( List<float> distribution )
        {
            //integrate the curve
            float maxWeight = 0f;
            for( int i = 0; i < distribution.Count; ++i )
            {
                maxWeight += distribution[i];
            }

            float rngValue = Rand(0f,maxWeight);

            //find the outcome
            float sum = 0f;
            for( int i = 0; i < distribution.Count; ++i )
            {
                sum += distribution[ i ];
                if( sum > rngValue )
                    return i;
            }
            return distribution.Count - 1;
        }
    }
}