using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace EnemyRandomizerMod
{
    public class Geo : IEquatable<Geo>
    {
        public HealthManager EnemyHealthManager { get; protected set; }

        public Geo(GameObject source)
        {
            Dev.Log("creating geo manager");
            if (source != null)
            {
                Dev.Log("getting health manager");
                EnemyHealthManager = source.GetComponent<HealthManager>();
            }
        }

        public int SmallGeo
        {
            get
            {
                Dev.Where();
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
                Dev.Where();
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
                Dev.Where();
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
                    Dev.Log("getting geo value ");
                    return SmallGeo + MedGeo + LargeGeo;
                }
                catch(Exception e)
                {
                    Dev.Log("Caught error getting geo value");
                    return 0;
                }
            }
            set
            {
                Dev.Where();
                int lg = 0;
                int med = 0;
                int sm = 0;

                int rem = 0;
                if(value > 25)
                {
                    lg = value / 25;
                    rem = value % 25;
                }
                if(rem > 0)
                {
                    med = rem / 5;
                    rem = rem % 5;
                }
                if(rem  > 0)
                {
                    sm = rem;
                }

                Dev.Log("setting geo values");
                LargeGeo = lg;
                MedGeo = med;
                SmallGeo = sm;
                Dev.Log("done setting geo values");
            }
        }

        public static Geo operator *(Geo a, float b)
        {
            Dev.Log("scaling geo values");
            a.Value = (int)((float)a.Value * b);
            return a;
        }

        public static Geo operator /(Geo a, float b)
        {
            Dev.Where();
            if (Mathnv.FastApproximately(b, 0f, 0.001f))
                return a;

            a.Value = (int)((float)a.Value / b);
            return a;
        }

        public static bool operator <(Geo a, Geo b)
        {
            Dev.Where();
            if (ReferenceEquals(a, null))
                return true;

            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Geo a, Geo b)
        {
            Dev.Where();
            if (ReferenceEquals(a, null))
                return false;

            return a.CompareTo(b) > 0; ;
        }

        public static bool operator ==(Geo a, Geo b)
        {
            Dev.Where();
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
            Dev.Where();
            return !(a == b);
        }

        public static bool operator <=(Geo a, Geo b)
        {
            Dev.Where();
            if (ReferenceEquals(a, null))
                return true;

            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Geo a, Geo b)
        {
            Dev.Where();
            if (ReferenceEquals(a, null))
                return false;

            return a.CompareTo(b) >= 0;
        }

        public override string ToString()
        {
            Dev.Where();
            return $"[Geo(L:{LargeGeo} M:{MedGeo} S:{SmallGeo})]";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Geo);
        }

        public override int GetHashCode()
        {
            Dev.Where();
            return Value.GetHashCode();
        }

        bool IEquatable<Geo>.Equals(Geo other)
        {
            Dev.Where();
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

        public static implicit operator int(Geo other)
        {
            Dev.Where();
            return other.SmallGeo + other.MedGeo + other.LargeGeo;
        }
    }
}
