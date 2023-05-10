﻿using UnityEngine;
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

        public static bool StickToClosestSurface(this GameObject gameObject, float maxRange = 100f, bool alsoStickCorpse = true)
        {
            if (MetaDataTypes.IsPogoLogicType(gameObject.name))
                return false;

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
            if (MetaDataTypes.IsPogoLogicType(gameObject.name))
                return false;

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

        public static bool StickToClosestSurfaceWithoutRotation(this GameObject gameObject, float maxRange, float extraOffsetScale = 0.33f)
        {
            if (MetaDataTypes.IsPogoLogicType(gameObject.name))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetNearestSurface(gameObject, maxRange);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);

            return true;
        }

        public static RaycastHit2D GetGroundRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999,-9999) };

            RaycastHit2D closest = GetGround(gameObject);
            return closest;
        }

        public static RaycastHit2D GetRoofRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999, -9999) };

            RaycastHit2D closest = GetRoof(gameObject);
            return closest;
        }

        public static RaycastHit2D GetLeftRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999, -9999) };

            RaycastHit2D closest = GetLeft(gameObject);
            return closest;
        }

        public static RaycastHit2D GetRightRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999, -9999) };

            RaycastHit2D closest = GetRight(gameObject);
            return closest;
        }


        public static bool StickToGround(this GameObject gameObject, float extraOffsetScale = 0.53f)
        {
            if (MetaDataTypes.IsPogoLogicType(gameObject.name))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetGround(gameObject);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, false);

            return true;
        }

        public static Vector2 ToVec2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static bool StickToGroundX(this GameObject gameObject, float extraOffsetScale = 0.53f)
        {
            if (MetaDataTypes.IsPogoLogicType(gameObject.name))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetRayOn(gameObject.transform.position.ToVec2() + Vector2.up, Vector2.down, float.MaxValue);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, false);

            return true;
        }

        public static bool StickToRoof(this GameObject gameObject, float extraOffsetScale = 0.33f, bool flipped = false)
        {
            if (MetaDataTypes.IsPogoLogicType(gameObject.name))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetRoof(gameObject);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, flipped);

            return true;
        }

        public static CorpseOrientationFixer AddCorpseOrientationFixer(float newAngle, GameObject corpse)
        {
            var fixer = corpse.gameObject.AddComponent<CorpseOrientationFixer>();
            fixer.corpseAngle = newAngle;
            return fixer;
        }

        public static CorpseRemover AddCorpseRemoverWithEffect(GameObject corpse, string effect = null)
        {
            var remover = corpse.gameObject.AddComponent<CorpseRemover>();
            if (!string.IsNullOrEmpty(effect))
            {
                remover.replacementEffect = effect;
            }
            return remover;
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


        /// <summary>
        /// Apply this via transform.equlerAngles
        /// </summary>
        public static float RotateToDirection(this Vector2 input, float angleOffset = 90f)
        {
            input = input.normalized;
            float angle = Mathf.Atan2(input.y, input.x) * 57.2957764f + angleOffset;
            return angle;
        }


        /// <summary>
        /// Apply this via transform.equlerAngles
        /// </summary>
        public static void RotateToDirection(this GameObject input, Vector2 dir, float angleOffset = 90f)
        {
            float angle = RotateToDirection(dir, angleOffset);
            input.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }

        public static Vector3 SetPositionToRayCollisionPoint(GameObject gameObject, RaycastHit2D closest, float offsetScale = 0.33f)
        {
            var collider = gameObject.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                var sprite = gameObject.GetComponent<tk2dSprite>();
                if (sprite != null)
                {
                    collider = sprite.boxCollider2D;
                }
            }

            if (collider != null && closest.collider != null)
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

        public static RaycastHit2D GetLeft(GameObject gameObject)
        {
            RaycastHit2D result = FireRayLocal(gameObject, Vector2.left, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetRight(GameObject gameObject)
        {
            RaycastHit2D result = FireRayLocal(gameObject, Vector2.right, float.MaxValue);
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

        public static Vector3 GetVectorTo(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetVectorTo(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetPointOn(this GameObject entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(entitiy.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetRayOn(this GameObject entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetRayOn(entitiy.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetPointOn(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetRayOn(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetRayOn(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorToSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestVectorTo(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestPointOn(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(Vector2 origin, float max)
        {
            return Mathnv.GetNearestPointOn(origin, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayOnSurface(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestRayOn(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayOnSurface(Vector2 origin, float max)
        {
            return Mathnv.GetNearestRayOn(origin, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestVectorDown(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestPointDown(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(Vector2 origin, float max)
        {
            return Mathnv.GetNearestPointDown(origin, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayDown(this GameObject entitiy, float max)
        {
            return Mathnv.GetNearestRayDown(entitiy.transform.position, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayDown(Vector2 origin, float max)
        {
            return Mathnv.GetNearestRayDown(origin, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestSurfaceX(GameObject gameObject, float maxDistanceToCheck)
        {
            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                GetRayOn(gameObject, Vector2.down, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.up, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.left, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.right, maxDistanceToCheck),
            };

            var closest = raycastHit2D.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
            return closest;
        }

        public static List<RaycastHit2D> GetCardinalRays(GameObject gameObject, float maxDistanceToCheck)
        {
            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                GetRayOn(gameObject, Vector2.down, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.up, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.left, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.right, maxDistanceToCheck),
            };

            return raycastHit2D;
        }

        public static List<RaycastHit2D> GetOctagonalRays(GameObject gameObject, float maxDistanceToCheck)
        {
            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                GetRayOn(gameObject, Vector2.down, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.up, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.left, maxDistanceToCheck),
                GetRayOn(gameObject, Vector2.right, maxDistanceToCheck),

                GetRayOn(gameObject, (Vector2.down + Vector2.left).normalized , maxDistanceToCheck),
                GetRayOn(gameObject, (Vector2.down + Vector2.right).normalized, maxDistanceToCheck),
                GetRayOn(gameObject, (Vector2.up + Vector2.left).normalized, maxDistanceToCheck),
                GetRayOn(gameObject, (Vector2.up + Vector2.right).normalized, maxDistanceToCheck),
            };

            return raycastHit2D;
        }

        public static List<RaycastHit2D> GetDiagonalRays(GameObject gameObject, float maxDistanceToCheck)
        {
            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                GetRayOn(gameObject, (Vector2.down + Vector2.left).normalized , maxDistanceToCheck),
                GetRayOn(gameObject, (Vector2.down + Vector2.right).normalized, maxDistanceToCheck),
                GetRayOn(gameObject, (Vector2.up + Vector2.left).normalized, maxDistanceToCheck),
                GetRayOn(gameObject, (Vector2.up + Vector2.right).normalized, maxDistanceToCheck),
            };

            return raycastHit2D;
        }

        public static RaycastHit2D GetGroundX(GameObject gameObject)
        {
            RaycastHit2D result = GetNearestRayDown(gameObject, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetRoofX(GameObject gameObject)
        {
            RaycastHit2D result = GetRayOn(gameObject, Vector2.up, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetLeftX(GameObject gameObject)
        {
            RaycastHit2D result = GetRayOn(gameObject, Vector2.left, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetRightX(GameObject gameObject)
        {
            RaycastHit2D result = GetRayOn(gameObject, Vector2.right, float.MaxValue);
            return result;
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
