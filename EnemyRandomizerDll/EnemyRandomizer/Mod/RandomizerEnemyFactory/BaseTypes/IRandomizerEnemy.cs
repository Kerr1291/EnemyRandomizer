using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;

namespace EnemyRandomizerMod
{
    public interface IRandomizerEnemy
    {
        GameObject EnemyObject { get; }

        // Methods used to configure the source prefab and data of our enemy type

        /// <summary>
        /// Logic to run on the source prefab of the enemy after its given during initialization
        /// </summary>
        void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject);

        /// <summary>
        /// The core method called when creating an instance of an enemy.
        /// </summary>
        GameObject Instantiate(EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);

        // Methods used to configure a newly created enemy
        
        /// <summary>
        /// Configure the parent of the new enemy
        /// </summary>
        void SetNewEnemyParent(GameObject newEnemy, GameObject enemyToReplace = null);

        /// <summary>
        /// Configure the size of the new enemy
        /// </summary>
        void ScaleNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);

        /// <summary>
        /// Configure the orientation of the new enemy
        /// </summary>
        void RotateNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);

        /// <summary>
        /// Configure the position of the new enemy
        /// </summary>
        void PositionNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);

        /// <summary>
        /// Configure the geo of the new enemy
        /// </summary>
        void ModifyNewEnemyGeo(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);

        /// <summary>
        /// Do any remaining coniguration logic specific to this enemy type (add custom controller components etc)
        /// </summary>
        void FinalizeNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);
        
        /// <summary>
        /// Finally, name our new enemy
        /// </summary>
        void NameNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null);
    }

    public static class IRandomizerEnemyGameObjectExtensions
    {
        public static Vector3 GetVectorToGround(this GameObject entitiy, float maxProjectionDistance)
        {
            Vector2 origin = entitiy.transform.position;
            Vector2 direction = Vector2.down;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin, direction, maxProjectionDistance, Physics2D.AllLayers);

            if (toGround != null)
            {
                foreach (var v in toGround)
                {
                    Dev.Log("GetVectorToGround:: RaycastHit2D hit object: " + v.collider.gameObject.name);
                    if (v.collider.gameObject.name.Contains("Chunk"))
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Dev.Log("GetVectorToGround:: RaycastHit2D is null! ");
            }

            return Vector3.zero;
        }

        public static Vector3 GetVectorTo(this GameObject entitiy, Vector2 dir, float max)
        {
            return GetVectorTo(entitiy.transform.position, dir, max);
        }

        public static Vector3 GetVectorTo(Vector2 origin, Vector2 dir, float max)
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin, direction, max, Physics2D.AllLayers);

            if (toGround != null)
            {
                foreach (var v in toGround)
                {
                    Dev.Log("GetVectorTo:: RaycastHit2D hit object: " + v.collider.gameObject.name);
                    if (v.collider.gameObject.name.Contains("Chunk") || v.collider.gameObject.name.Contains("Platform") || v.collider.gameObject.name.Contains("Roof"))
                    {
                        Vector2 vectorToGround = v.point - origin;
                        return vectorToGround;
                    }
                }
            }
            else
            {
                Dev.Log("GetVectorTo:: RaycastHit2D is null! ");
            }

            return Vector3.one * max;
        }

        public static Vector3 GetNearestVectorToChunk(this GameObject entitiy, float max)
        {
            return GetNearestVectorToChunk(entitiy.transform.position, max);
        }

        public static Vector2 GetNearestVectorToChunk(Vector2 origin, float max)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                Physics2D.RaycastAll(origin, Vector2.up, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.left, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.right, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(x => x.collider.gameObject.name.Contains("Chunk") || x.collider.gameObject.name.Contains("Platform") || x.collider.gameObject.name.Contains("Roof"));
            var sorted = hitsWithChunks.Select(x => x.point - origin).OrderBy(y => y.sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetNearestVectorToGround(this GameObject entitiy, float max)
        {
            return GetNearestVectorToGround(entitiy.transform.position, max);
        }

        public static Vector2 GetNearestVectorToGround(Vector2 origin, float max)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                //Physics2D.RaycastAll(origin, Vector2.up, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers)
                //Physics2D.RaycastAll(origin, Vector2.left, max, Physics2D.AllLayers),
                //Physics2D.RaycastAll(origin, Vector2.right, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(x => x.collider.gameObject.name.Contains("Chunk") || x.collider.gameObject.name.Contains("Platform") || x.collider.gameObject.name.Contains("Roof"));
            var sorted = hitsWithChunks.Select(x => x.point - origin).OrderBy(y => y.sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetNearestPointOnGround(this GameObject entitiy, float max)
        {
            return GetNearestPointOnGround(entitiy.transform.position, max);
        }

        public static Vector3 GetNearestPointOnGround(Vector2 origin, float max)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                //Physics2D.RaycastAll(origin, Vector2.up, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers)
                //Physics2D.RaycastAll(origin, Vector2.left, max, Physics2D.AllLayers),
                //Physics2D.RaycastAll(origin, Vector2.right, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(x => x.collider.gameObject.name.Contains("Chunk") || x.collider.gameObject.name.Contains("Platform") || x.collider.gameObject.name.Contains("Roof"));
            var sorted = hitsWithChunks.Select(x => x.point).OrderBy(y => (y - origin).sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetNearestPointOnSurface(this GameObject entitiy, float max)
        {
            return GetNearestPointOnSurface(entitiy.transform.position, max);
        }

        public static Vector3 GetNearestPointOnSurface(Vector2 origin, float max)
        {
            List<RaycastHit2D[]> allHits = new List<RaycastHit2D[]>()
            {
                Physics2D.RaycastAll(origin, Vector2.up, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.down, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.left, max, Physics2D.AllLayers),
                Physics2D.RaycastAll(origin, Vector2.right, max, Physics2D.AllLayers)
            };

            var allGoodHits = allHits.Where(x => x != null).SelectMany(y => y);
            var hitsWithChunks = allGoodHits.Where(x => x.collider.gameObject.name.Contains("Chunk") || x.collider.gameObject.name.Contains("Platform") || x.collider.gameObject.name.Contains("Roof"));
            var sorted = hitsWithChunks.Select(x => x.point).OrderBy(y => (y - origin).sqrMagnitude);
            return sorted.FirstOrDefault();
        }

        public static Vector3 GetPointOn(this GameObject entitiy, Vector2 dir, float max)
        {
            return GetPointOn(entitiy.transform.position, dir, max);
        }

        public static Vector3 GetPointOn(Vector2 origin, Vector2 dir, float max)
        {
            Vector2 direction = dir;

            RaycastHit2D[] toGround = Physics2D.RaycastAll(origin, direction, max, Physics2D.AllLayers);

            Vector2 lastGoodPoint = Vector2.zero;

            if (toGround != null)
            {
                foreach (var v in toGround)
                {
                    Dev.Log("GetPointOn:: RaycastHit2D hit object: " + v.collider.gameObject.name);
                    if (v.collider.gameObject.name.Contains("Chunk") || v.collider.gameObject.name.Contains("Platform") || v.collider.gameObject.name.Contains("Roof"))
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
            else
            {
                Dev.Log("GetPointOn:: RaycastHit2D is null! ");
            }

            return lastGoodPoint;
        }
    }
}
