using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EnemyRandomizerMod
{
    public interface IPrefabConfig
    {
        void SetupPrefab(PrefabObject p);
    }

    public interface ISpawner
    {
        GameObject Spawn(PrefabObject p, ObjectMetadata source);
    }

    public static class SpawnerExtensions
    {
        public static bool IsSurfaceOrPlatform(this GameObject gameObject)
        {
            //First process skips or exclusions
            List<string> groundOrPlatformName = new List<string>()
            {
                "Chunk",
                "Platform",
                "plat_",
                "Roof"
            };

            return groundOrPlatformName.Any(x => gameObject.name.Contains(x));
        }
        public static GameObject GetCorpse<T>(this GameObject prefab)
            where T : EnemyDeathEffects
        {
            var deathEffects = prefab.GetComponentInChildren<T>(true);

            if (deathEffects == null)
                return null;

            var rootType = deathEffects.GetType();

            System.Reflection.FieldInfo GetCorpseField(Type t)
            {
                return t.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            while (rootType != typeof(EnemyDeathEffects) && rootType != null)
            {
                if (GetCorpseField(rootType) != null)
                    break;
                rootType = rootType.BaseType;
            }

            if (rootType == null)
                return null;

            var corpsePrefab = (GameObject)GetCorpseField(rootType).GetValue(deathEffects);

            if (corpsePrefab == null)
            {
                return null;
            }
            else
            {
                return corpsePrefab;
            }
        }

        public static GameObject GetCorpsePrefab<T>(this GameObject prefab)
            where T : EnemyDeathEffects
        {
            var deathEffects = prefab.GetComponentInChildren<T>(true);

            if (deathEffects == null)
                return null;

            var rootType = deathEffects.GetType();

            while(rootType != typeof(EnemyDeathEffects) && rootType != null)
            {
                rootType = rootType.BaseType;
            }

            if (rootType == null)
                return null;

            var corpsePrefab = (GameObject)rootType.GetField("corpsePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);

            if (corpsePrefab == null)
            {
                return null;
            }
            else
            {
                return corpsePrefab;
            }
        }

        public static void SetParentToOthersParent(this GameObject gameObject, ObjectMetadata other)
        {
            if (other == null)
                return;

            if (!other.HasData)
                return;

            if(other.Source != null)
                gameObject.transform.SetParent(other.Source.transform.parent);
        }

        //use this when the other object has a smasher
        public static GameObject CopyAndAddSmasher(this GameObject gameObject, ObjectMetadata other)
        {
            if (other == null || !other.HasData || other.Source == null)
                return null;

            if (!other.IsSmasher)
                return null;

            var smasher = other.Source.FindGameObjectInDirectChildren("Smasher");
            var copy = GameObject.Instantiate(smasher, gameObject.transform);
            copy.SetActive(false);
            return copy;
        }

        //TODO:::: ADD OPTION TO ALLOW FOR THE AUDIO PITCH TO SCALE WITH SIZE
        public static ObjectMetadata SetScaleToOthersScale(this GameObject gameObject, ObjectMetadata other)
        {
            if (other == null)
                return null;

            if (!other.HasData)
                return null;

            ObjectMetadata thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());

            var scale = thisMetadata.GetRelativeScale(other);
            thisMetadata.ApplySizeScale(scale);

            return thisMetadata;
        }

        public static void SetAudioToMatchScale(this GameObject gameObject, ObjectMetadata other)
        {
            if (other == null)
                return;

            if (!other.HasData)
                return;

            if (!Mathnv.FastApproximately(other.SizeScale, 1f, .01f))
                return;

            ObjectMetadata thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());

            //TODO:....
        }

        public static void RotateNewEnemy(this GameObject gameObject, ObjectMetadata other = null)
        {
            if (gameObject != null)
            {
                if (!other.DatabaseName.Contains("Ceiling Dropper"))
                    gameObject.transform.rotation = other.Source.transform.rotation;

                //if they were a wall flying mantis, don't rotate the replacement
                if (gameObject.name.Contains("Mantis Flyer Child"))
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }

                //mosquitos rotate, so spawn replacements with default rotation
                if (gameObject.name.Contains("Mosquito"))
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }
            }
        }

        public static void StickToClosestSurface(this GameObject gameObject, ObjectMetadata thisObject)
        {
            var corpse = gameObject.GetCorpse<EnemyDeathEffects>();
            if (corpse == null)
            {
                corpse = gameObject.GetCorpsePrefab<EnemyDeathEffects>();
            }

            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                FireRayLocal(thisObject, Vector2.down, 100f),
                FireRayLocal(thisObject, Vector2.up, 100f),
                FireRayLocal(thisObject, Vector2.left, 100f),
                FireRayLocal(thisObject, Vector2.right, 100f),
            };

            var closest = raycastHit2D.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();

            var col = thisObject.Collider as BoxCollider2D;

            if (closest.collider != null && col != null)
            {
                gameObject.transform.position = closest.point + closest.normal * col.size.y / 3f * gameObject.transform.localScale.y;
                var angles = gameObject.transform.localEulerAngles;
                if (closest.normal.y > 0)
                {
                    angles.z = 0f;
                }
                else if (closest.normal.y < 0)
                {
                    angles.z = 180f;
                }
                else if (closest.normal.x < 0)
                {
                    angles.z = 90f;
                }
                else if (closest.normal.x > 0)
                {
                    angles.z = 270f;
                }
                gameObject.transform.localEulerAngles = angles;
                if (corpse != null)
                {
                    var fixer = corpse.gameObject.AddComponent<CorpseOrientationFixer>();
                    fixer.corpseAngle = angles.z;
                }
            }
        }

        public static RaycastHit2D FireRayLocal(ObjectMetadata otherdata, Vector2 direction, float length)
        {
            Vector2 vector = otherdata.Collider.transform.TransformPoint(otherdata.Collider.offset);
            Vector2 vector2 = otherdata.Collider.transform.TransformDirection(direction);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
            //Debug.DrawRay(vector, vector2);
            return raycastHit2D;
        }
        public static Vector3 GetVectorTo(this GameObject entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetVectorTo(entitiy.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetPointOn(this GameObject entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(entitiy.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorToSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestVectorTo(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestPointOn(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestVectorDown(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestPointDown(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static void PositionNewEnemy(this GameObject gameObject, ObjectMetadata otherdata = null)
        {
            GameObject other = otherdata.Source;
            if (other == null)
                return;

            ObjectMetadata thisMetadata = new ObjectMetadata();
            thisMetadata.Setup(gameObject, EnemyRandomizerDatabase.GetDatabase());

            float rotation = other.transform.localEulerAngles.z;
            Vector2 originalUp = Vector2.zero;

            if (Mathf.Approximately(0f, rotation) || Mathf.Approximately(360f, rotation))
                originalUp = Vector2.up;

            if (Mathf.Approximately(90f, rotation) || Mathf.Approximately(-270f, rotation))
                originalUp = Vector2.right;

            if (Mathf.Approximately(-90f, rotation) || Mathf.Approximately(270f, rotation))
                originalUp = Vector2.left;

            if (Mathf.Approximately(180f, rotation))
                originalUp = Vector2.down;

            //adjust the position to take into account the new monster type and/or size
            gameObject.transform.position = other.transform.position;

            BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
            Vector2 colliderSize = collider == null ? Vector2.one : collider.size;
            Vector2 scale = gameObject.transform.localScale;
            Vector2 originalPosition = gameObject.transform.position;


            Vector2 originalDown = -originalUp;
            float projectionDistance = 500f;
            Vector3 toSurface = Mathnv.GetNearestVectorTo(originalPosition, projectionDistance, IsSurfaceOrPlatform);
            Vector2 toSurfaceDir = toSurface.normalized;
            Vector2 toSurfaceUp = -toSurfaceDir;

            Vector3 positionOnSurface = Mathnv.GetNearestPointOn(originalPosition, projectionDistance, IsSurfaceOrPlatform);
            Vector3 positionOffset = Vector3.zero;

            if (thisMetadata.DatabaseName.Contains("Mantis Flyer Child"))
            {
                positionOffset = new Vector3(colliderSize.x * originalUp.x * scale.x, colliderSize.y * originalUp.y * scale.y, 0f);
            }
            //project the ceiling droppers onto the ceiling
            if (thisMetadata.DatabaseName.Contains("Ceiling Dropper"))
            {
                positionOnSurface = Mathnv.GetPointOn(originalPosition, Vector2.up, projectionDistance, IsSurfaceOrPlatform);
                //move it down a bit, keeps spawning in roof
                positionOffset = Vector3.down * 2f * scale.y;
            }
            else if (!thisMetadata.IsFlying)
            {
                if (Mathf.Approximately(0f, rotation))
                {
                    positionOnSurface = gameObject.GetPointOn(Vector2.down, projectionDistance);

                    if (thisMetadata.DatabaseName.Contains("Lobster"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * 2f) * scale.y;
                    }
                    if (thisMetadata.DatabaseName.Contains("Blocker"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                    }
                    if (thisMetadata.DatabaseName == ("Moss Knight"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                    }
                    if (thisMetadata.DatabaseName == ("Enemy"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -0.5f) * scale.y;
                    }
                }
                else
                {
                    positionOnSurface = gameObject.GetPointOn(toSurfaceDir, projectionDistance);
                }

                positionOffset = new Vector3(colliderSize.x * originalUp.x * scale.x, colliderSize.y * originalUp.y * scale.y, 0f);

                if (other != null && other.name.Contains("Moss Walker"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * scale.y;
                }
                if (thisMetadata.DatabaseName.Contains("Plant Trap"))
                {
                    positionOffset = toSurfaceUp * 2f * scale.y;
                }
                if (collider != null && thisMetadata.DatabaseName.Contains("Mawlek Turret"))
                {
                    positionOffset = toSurfaceUp * collider.size.y / 3f * scale.y;
                }
                if (collider != null && thisMetadata.DatabaseName.Contains("Mushroom Turret"))
                {
                    positionOffset = (toSurfaceUp * .5f) * scale.y;
                }
                if (thisMetadata.DatabaseName.Contains("Plant Turret"))
                {
                    positionOffset = toSurfaceUp * .7f * scale.y;
                }
                if (collider != null && thisMetadata.DatabaseName.Contains("Laser Turret"))
                {
                    positionOffset = toSurfaceUp * collider.size.y / 10f * scale.y;
                }
                if (collider != null && thisMetadata.DatabaseName.Contains("Worm"))
                {
                    positionOffset = toSurfaceUp * collider.size.y / 3f * scale.y;
                }
                if (thisMetadata.DatabaseName.Contains("Crystallised Lazer Bug"))
                {
                    //suppposedly 1/2 their Y collider space offset should be 1.25
                    //but whatever we set it at, they spawn pretty broken, so spawn them out of the ground a bit so they're still a threat
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y;
                }
                if (thisMetadata.DatabaseName.Contains("Mines Crawler"))
                {
                    positionOffset = toSurfaceUp * 1.5f * scale.y;
                }
                if (thisMetadata.DatabaseName.Contains("Spider Mini"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y; ;
                }
                if (thisMetadata.DatabaseName.Contains("Abyss Crawler"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y; ;
                }
                if (thisMetadata.DatabaseName.Contains("Climber"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y;
                }
            }
            else
            {
                positionOnSurface = gameObject.transform.position;
            }

            gameObject.transform.position = positionOnSurface + positionOffset;
        }
    }
}
