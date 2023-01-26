using System.Collections.Generic;
using UnityEngine;
using nv;
using System.Linq;

namespace EnemyRandomizerMod
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Check to see if there's at least one custom component on the object
        /// </summary>
        public static bool IsRandomizerEnemy(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            return gameObject.GetComponents<MonoBehaviour>().FirstOrDefault(x => typeof(IRandomizerEnemyController).IsAssignableFrom(x.GetType())) != null;
        }

        /// <summary>
        /// Get all the custom components on the enemy object
        /// </summary>
        public static IEnumerable<IRandomizerEnemyController> GetRandomizerEnemyComponents(this GameObject gameObject)
        {
            return gameObject.GetComponents<MonoBehaviour>().Where(x => typeof(IRandomizerEnemyController).IsAssignableFrom(x.GetType())).Cast<IRandomizerEnemyController>();
        }

        public static void SetupRandomizerComponents(this GameObject gameObject, IRandomizerEnemy enemyData, GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            var randoComponents = gameObject.GetRandomizerEnemyComponents().ToList();
            randoComponents.ForEach(x =>
            {
                x.LinkDataObjects(gameObject, enemyData);
            });
            randoComponents.ForEach(x =>
            {
                x.SetupInstance(enemyToReplace, dataOfEnemyToReplace);
            });
        }

        public static bool IsVisible(this GameObject enemy)
        {
            Collider2D collider = enemy.GetComponent<Collider2D>();
            MeshRenderer renderer = enemy.GetComponent<MeshRenderer>();
            if (collider == null && renderer == null)
                return false;

            if (collider != null && renderer == null)
                return collider.enabled;
            else if (collider == null && renderer != null)
                return renderer.enabled;
            else //if (collider != null && renderer != null)
                return collider.enabled && renderer.enabled;
        }

        public static Vector3 GetVectorTo(this GameObject entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetVectorTo(entitiy.transform.position, dir, max, EnemyRandomizer.IsSurfaceOrPlatform);
        }

        public static Vector3 GetPointOn(this GameObject entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(entitiy.transform.position, dir, max, EnemyRandomizer.IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorToSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestVectorTo(entitiy.transform.position, max, EnemyRandomizer.IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestPointOn(entitiy.transform.position, max, EnemyRandomizer.IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestVectorDown(entitiy.transform.position, max, EnemyRandomizer.IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestPointDown(entitiy.transform.position, max, EnemyRandomizer.IsSurfaceOrPlatform);
        }

    }
}
