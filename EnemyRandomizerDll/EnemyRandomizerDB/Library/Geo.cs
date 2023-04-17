using System.Collections.Generic;
using UnityEngine;
using System;

//NOTE: adjust aduio player oneshot pitch values and audio source component pitch values when shrinking/growing things
//NOTE: walker enemies need their "rightScale" float changed when the base transform scale is changed so they match or sprites will squish weirdly
//      ALSO need to scan their FSMs for states with "SetScale" actions with x values that are 1/-1 and change those to match the x scale
//      check the "IsNone" property to see if only X is used on the SetScale floats

namespace EnemyRandomizerMod
{
    public class Geo : IEquatable<Geo>
    {
        public Geo(ObjectMetadata source)
        {
            hm = source.EnemyHealthManager;
        }

        public void Apply(ObjectMetadata other, float scale = 1f)
        {
            Geo otherGeo = new Geo(other);
            otherGeo.Value = (int)((float)Value * scale);
        }

        public void Apply(HealthManager other, float scale = 1f)
        {
            other.SetGeoSmall(Mathf.FloorToInt(SmallGeo * scale));
            other.SetGeoMedium(Mathf.FloorToInt(MedGeo * scale));
            other.SetGeoLarge(Mathf.FloorToInt(LargeGeo * scale));
        }

        protected HealthManager hm;

        public int SmallGeo
        {
            get
            {
                return hm.GetSmallGeo();
            }
            set
            {
                hm.SetGeoSmall(value);
            }
        }

        public int MedGeo
        {
            get
            {
                return hm.GetMedGeo();
            }
            set
            {
                hm.SetGeoMedium(value);
            }
        }

        public int LargeGeo
        {
            get
            {
                return hm.GetLargeGeo();
            }
            set
            {
                hm.SetGeoLarge(value);
            }
        }

        public int Value
        {
            get
            {
                return SmallGeo + MedGeo + LargeGeo;
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
                if(rem > 0)
                {
                    med = rem / 5;
                    rem = rem % 5;
                }
                if(rem  > 0)
                {
                    sm = rem;
                }

                LargeGeo = lg;
                MedGeo = med;
                SmallGeo = sm;
            }
        }

        public static Geo operator *(Geo a, float b)
        {
            a.Value = (int)((float)a.Value * b);
            return a;
        }

        public static Geo operator /(Geo a, float b)
        {
            a.Value = (int)((float)a.Value / b);
            return a;
        }

        public static bool operator <(Geo a, Geo b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >(Geo a, Geo b)
        {
            return a.Value < b.Value;
        }

        public static bool operator ==(Geo a, Geo b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Geo a, Geo b)
        {
            return a.Value != b.Value;
        }

        public static bool operator <=(Geo a, Geo b)
        {
            return a.Value <= b.Value;
        }

        public static bool operator >=(Geo a, Geo b)
        {
            return a.Value <= b.Value;
        }

        public override string ToString()
        {
            return $"[Geo(L:{LargeGeo} M:{MedGeo} S:{SmallGeo})]";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Geo);
        }

        public bool Equals(Geo other)
        {
            return other != null &&
                   EqualityComparer<HealthManager>.Default.Equals(hm, other.hm) &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            int hashCode = -2140644376;
            hashCode = hashCode * -1521134295 + EqualityComparer<HealthManager>.Default.GetHashCode(hm);
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        public static implicit operator int(Geo other)
        {
            return other.SmallGeo + other.MedGeo + other.LargeGeo;
        }
    }
}
