using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace EnemyRandomizerMod
{
    public class Geo : IEquatable<Geo>
    {
        public HealthManager EnemyHealthManager { get; protected set; }

        public Geo() {
            //Dev.Log($"Default ctor invoked");
        }

        public Geo(GameObject source)
        {
            //Dev.Log($"Creating geo manager with source {source}");
            //Dev.Log($"DIY STACKTRACE -- " +
            //    $"{Dev.FunctionHeader(3)}\n{Dev.FunctionHeader(2)}\n{Dev.FunctionHeader(1)}\n{Dev.FunctionHeader(0)}" +
            //    $"{Dev.FunctionHeader(-1)}\n{Dev.FunctionHeader(-2)}\n{Dev.FunctionHeader(-3)}\n{Dev.FunctionHeader(-4)}");
            //Dev.Log($"End stacktrace");

            SetSource(source);
        }

        public void SetSource(GameObject source)
        {
            //Dev.Log($"Setting source for geo manager {source}");
            if (source != null)
            {
                EnemyHealthManager = source.GetComponent<HealthManager>();
                //Dev.Log($"Got health manager? [{EnemyHealthManager}]");
            }
        }

        public int SmallGeo
        {
            get
            {
                return EnemyHealthManager == null ? 0 : EnemyHealthManager.GetSmallGeo();
            }
            set
            {
                if (EnemyHealthManager != null) EnemyHealthManager.SetGeoSmall(value);
            }
        }

        public int MedGeo
        {
            get
            {
                return EnemyHealthManager == null ? 0 : EnemyHealthManager.GetMedGeo();
            }
            set
            {
                if (EnemyHealthManager != null) EnemyHealthManager.SetGeoMedium(value);
            }
        }

        public int LargeGeo
        {
            get
            {
                return EnemyHealthManager == null ? 0 : EnemyHealthManager.GetLargeGeo();
            }
            set
            {
                if (EnemyHealthManager != null) EnemyHealthManager.SetGeoLarge(value);
            }
        }

        public int Value
        {
            get
            {
                try
                {
                    //Dev.Log("getting geo value ");
                    return SmallGeo + MedGeo * 5 + LargeGeo * 25;
                }
                catch(Exception e)
                {
                    //Dev.Log("Caught error getting geo value");
                    return 0;
                }
            }
            set
            {
                int lg = 0;
                int med = 0;
                int sm = 0;

                int rem = 0;
                if(value > 25)
                {
                    lg = value / 25;
                    rem = value % 25;
                }
                else
                {
                    rem = value;
                }

                if(rem > 5)
                {
                    med = rem / 5;
                    rem = rem % 5;
                }

                if(rem  > 0)
                {
                    sm = rem;
                }

                //Dev.Log("setting geo values");
                try
                {
                    LargeGeo = lg;
                    MedGeo = med;
                    SmallGeo = sm;
                }
                catch (Exception e)
                {
                    //Dev.Log("Caught error setting geo values");
                }
                //Dev.Log("done setting geo values");
            }
        }

        public static Geo operator *(Geo a, float b)
        {
            a.Value = (int)((float)a.Value * b);
            return a;
        }

        public static Geo operator /(Geo a, float b)
        {
            if (Mathnv.FastApproximately(b, 0f, 0.001f))
                return a;

            a.Value = (int)((float)a.Value / b);
            return a;
        }

        public static bool operator <(Geo a, Geo b)
        {
            if (ReferenceEquals(a, null))
                return true;

            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Geo a, Geo b)
        {
            if (ReferenceEquals(a, null))
                return false;

            return a.CompareTo(b) > 0; ;
        }

        public static bool operator ==(Geo a, Geo b)
        {
            // Check if both objects are null or reference the same instance
            if (ReferenceEquals(a, b))
                return true;

            // Check if either object is null, as we've already checked for both being null or referencing the same instance
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Value == b.Value;
        }

        public static bool operator !=(Geo a, Geo b)
        {
            return !(a == b);
        }

        public static bool operator <=(Geo a, Geo b)
        {
            if (ReferenceEquals(a, null))
                return true;

            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Geo a, Geo b)
        {
            if (ReferenceEquals(a, null))
                return false;

            return a.CompareTo(b) >= 0;
        }

        public override string ToString()
        {
            return $"[Geo(L:{LargeGeo} M:{MedGeo} S:{SmallGeo})]";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Geo);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        bool IEquatable<Geo>.Equals(Geo other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (ReferenceEquals(other, null) || GetType() != other.GetType())
                return false;

            return this == (Geo)other;
        }

        public int CompareTo(Geo other)
        {
            if (ReferenceEquals(other, null))
                return 1;

            return Value.CompareTo(other.Value);
        }

        //public static implicit operator int(Geo other)
        //{
        //    Dev.Where();
        //    return other.SmallGeo + other.MedGeo + other.LargeGeo;
        //}
    }
}
