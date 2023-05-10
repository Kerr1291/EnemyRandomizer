using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace EnemyRandomizerMod
{
    public class Geo : IEquatable<Geo>
    {
        public ReadOnlyReactiveProperty<HealthManager> enemyHealthManager { get; protected set; }
        public HealthManager EnemyHealthManager => enemyHealthManager == null ? null : enemyHealthManager.Value;

        public Geo(ObjectMetadata source)
        {
            enemyHealthManager = source.enemyHealthManager;
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
            if (a == null && b != null)
                return false;
            if (a != null && b == null)
                return false;
            if (a == null && b == null)
                return false;
            return a.Value < b.Value;
        }

        public static bool operator >(Geo a, Geo b)
        {
            if (a == null && b != null)
                return false;
            if (a != null && b == null)
                return false;
            if (a == null && b == null)
                return false;
            return a.Value < b.Value;
        }

        public static bool operator ==(Geo a, Geo b)
        {
            if (a == null && b != null)
                return false;
            if (a != null && b == null)
                return false;
            if (a == null && b == null)
                return true;
            return a.Value == b.Value;
        }

        public static bool operator !=(Geo a, Geo b)
        {
            if (a == null && b != null)
                return true;
            if (a != null && b == null)
                return true;
            if (a == null && b == null)
                return false;
            return a.Value != b.Value;
        }

        public static bool operator <=(Geo a, Geo b)
        {
            if (a == null && b != null)
                return false;
            if (a != null && b == null)
                return false;
            if (a == null && b == null)
                return true;
            return a.Value <= b.Value;
        }

        public static bool operator >=(Geo a, Geo b)
        {
            if (a == null && b != null)
                return false;
            if (a != null && b == null)
                return false;
            if (a == null && b == null)
                return true;
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
                   EqualityComparer<HealthManager>.Default.Equals(EnemyHealthManager, other.EnemyHealthManager) &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            if (EnemyHealthManager == null)
                return 0;

            int hashCode = -2140644376;
            hashCode = hashCode * -1521134295 + EqualityComparer<HealthManager>.Default.GetHashCode(EnemyHealthManager);
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        public static implicit operator int(Geo other)
        {
            return other.SmallGeo + other.MedGeo + other.LargeGeo;
        }
    }
}
