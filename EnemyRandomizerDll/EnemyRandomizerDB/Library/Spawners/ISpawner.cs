using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Satchel;
using Satchel.Futils;

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

        public static bool StickToClosestSurface(this GameObject gameObject, float maxRange = 100f, bool alsoStickCorpse = true)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetNearestSurface(gameObject, maxRange);
            SetPositionToRayCollisionPoint(gameObject, closest);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest);

            if (alsoStickCorpse)
            {
                GameObject corpse = GetCorpseObject(gameObject);
                if (corpse != null)
                {
                    AddCorpseOrientationFixer(newAngle, corpse);
                }
            }

            return true;
        }

        public static bool StickToClosestSurface(this GameObject gameObject, float maxRange, float extraOffsetScale = 0.33f, bool alsoStickCorpse = true, bool flipped = false)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetNearestSurface(gameObject, maxRange);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, flipped);

            if (alsoStickCorpse)
            {
                GameObject corpse = GetCorpseObject(gameObject);
                if (corpse != null)
                {
                    AddCorpseOrientationFixer(newAngle, corpse);
                }
            }

            return true;
        }


        public static bool StickToGround(this GameObject gameObject, float extraOffsetScale = 0.33f)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetGround(gameObject);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, false);

            return true;
        }

        public static bool StickToRoof(this GameObject gameObject, float extraOffsetScale = 0.33f)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetRoof(gameObject);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, false);

            return true;
        }

        public static CorpseOrientationFixer AddCorpseOrientationFixer(float newAngle, GameObject corpse)
        {
            var fixer = corpse.gameObject.AddComponent<CorpseOrientationFixer>();
            fixer.corpseAngle = newAngle;
            return fixer;
        }

        public static GameObject GetCorpseObject(GameObject gameObject)
        {
            var corpse = gameObject.GetCorpse<EnemyDeathEffects>();
            if (corpse == null)
            {
                corpse = gameObject.GetCorpsePrefab<EnemyDeathEffects>();
            }

            return corpse;
        }

        public static float SetRotationToRayCollisionNormal(GameObject gameObject, RaycastHit2D closest, bool flipped = false)
        {
            if (closest.collider != null)
            {
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

                if (flipped)
                {
                    float angle = angles.z % 360f;
                    angle = (angle + 180f) % 360f;
                    angles.z = angle;
                }

                gameObject.transform.localEulerAngles = angles;
                return angles.z;
            }

            return 0f;
        }

        public static Vector3 SetPositionToRayCollisionPoint(GameObject gameObject, RaycastHit2D closest, float offsetScale = 0.33f)
        {
            var collider = gameObject.GetComponent<BoxCollider2D>();
            if (closest.collider != null && collider != null)
            {
                gameObject.transform.position = closest.point + closest.normal * collider.size.y * offsetScale * gameObject.transform.localScale.y;
            }

            return gameObject.transform.position;
        }

        public static RaycastHit2D GetNearestSurface(GameObject gameObject, float maxDistanceToCheck)
        {
            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                FireRayLocal(gameObject, Vector2.down, maxDistanceToCheck),
                FireRayLocal(gameObject, Vector2.up, maxDistanceToCheck),
                FireRayLocal(gameObject, Vector2.left, maxDistanceToCheck),
                FireRayLocal(gameObject, Vector2.right, maxDistanceToCheck),
            };

            var closest = raycastHit2D.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
            return closest;
        }

        public static RaycastHit2D GetGround(GameObject gameObject)
        {
            RaycastHit2D result = FireRayLocal(gameObject, Vector2.down, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetRoof(GameObject gameObject)
        {
            RaycastHit2D result = FireRayLocal(gameObject, Vector2.up, float.MaxValue);
            return result;
        }

        public static Dictionary<Vector2,RaycastHit2D> GetNearestSurfaces(this GameObject gameObject, float maxDistanceToCheck)
        {
            Dictionary < Vector2, RaycastHit2D> raycastHits2D = new Dictionary<Vector2,RaycastHit2D>()
            {
                {Vector2.down, FireRayLocal(gameObject, Vector2.down, maxDistanceToCheck) },
                {Vector2.up,   FireRayLocal(gameObject, Vector2.up,   maxDistanceToCheck) },
                {Vector2.left, FireRayLocal(gameObject, Vector2.left, maxDistanceToCheck) },
                {Vector2.right,FireRayLocal(gameObject, Vector2.right, maxDistanceToCheck) },
            };

            return raycastHits2D;
        }

        public static RaycastHit2D FireRayLocal(GameObject gameObject, Vector2 direction, float length)
        {
            var collider = gameObject.GetComponent<Collider2D>();
            Vector2 vector = collider.transform.TransformPoint(collider.offset);
            Vector2 vector2 = collider.transform.TransformDirection(direction);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
            //Debug.DrawRay(vector, vector2);
            return raycastHit2D;
        }

        public static RaycastHit2D FireRayGlobal(this Vector2 origin, Vector2 direction, float length)
        {
            Vector2 vector = origin;
            Vector2 vector2 = direction;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
            //Debug.DrawRay(vector, vector2);
            return raycastHit2D;
        }

        public static RaycastHit2D Fire2DRayGlobal(this Vector3 origin, Vector2 direction, float length)
        {
            Vector2 vector = origin;
            Vector2 vector2 = direction;
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

        public static IEnumerable<T> GetActionsOfType<T>(this GameObject gameObject)
            where T : HutongGames.PlayMaker.FsmStateAction
        {
            var fsms = gameObject.GetComponents<PlayMakerFSM>();
            var sactions = fsms.SelectMany(x => x.Fsm.States.SelectMany(y => y.GetActions<T>()));
            return sactions;
        }
    }
}
