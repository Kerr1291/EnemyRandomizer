using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace EnemyRandomizerMod
{
    public static class HealthManagerExtensions
    {
        public static int GetGeoSmall(this HealthManager healthManager)
        {
            FieldInfo fi = healthManager.GetType().GetField("smallGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            object temp = fi.GetValue(healthManager);
            int value = (temp == null ? 0 : (int)temp);
            return value;
        }

        public static int GetGeoMedium(this HealthManager healthManager)
        {
            FieldInfo fi = healthManager.GetType().GetField("mediumGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            object temp = fi.GetValue(healthManager);
            int value = (temp == null ? 0 : (int)temp);
            return value;
        }

        public static int GetGeoLarge(this HealthManager healthManager)
        {
            FieldInfo fi = healthManager.GetType().GetField("largeGeoDrops", BindingFlags.NonPublic | BindingFlags.Instance);
            object temp = fi.GetValue(healthManager);
            int value = (temp == null ? 0 : (int)temp);
            return value;
        }

        public static GameObject GetBattleScene(this HealthManager healthManager)
        {
            FieldInfo fi = typeof(HealthManager).GetField("battleScene", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return fi.GetValue(healthManager) as GameObject;
            }

            return null;
        }
    }
}
