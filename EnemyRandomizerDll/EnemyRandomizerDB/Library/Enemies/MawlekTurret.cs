using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using EnemyRandomizerMod;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System;
#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif

namespace EnemyRandomizerMod
{

    public class MawlekTurretControl : MonoBehaviour
    {
        Range upL = new Range(95f, 110f);
        Range upR = new Range(70f, 85f);

        Range leL = new Range(185f, 200f);
        Range leR = new Range(160f, 175f);

        Range dnL = new Range(250f, 265f);
        Range dnR = new Range(275f, 290f);

        Range riL = new Range(5f, 20f);
        Range riR = new Range(340f, 355f);

        public float shotSpeed = 10f;

        public Vector2 spawnPos;
        public float spawnDist = 1.4f;

        public bool isFloorTurret = false;

        BoxCollider2D col;

        private RaycastHit2D FireRayLocal(Vector2 direction, float length)
        {
            Vector2 vector = base.transform.TransformPoint(this.col.offset);
            Vector2 vector2 = base.transform.TransformDirection(direction);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
            //Debug.DrawRay(vector, vector2);
            return raycastHit2D;
        }

        private void StickToSurface()
        {
            var corpse = gameObject.GetCorpse<EnemyDeathEffects>();
            if(corpse == null)
            {
                corpse = gameObject.GetCorpsePrefab<EnemyDeathEffects>();
            }

            List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
            {
                this.FireRayLocal(Vector2.down, 100f),
                this.FireRayLocal(Vector2.up, 100f),
                this.FireRayLocal(Vector2.left, 100f),
                this.FireRayLocal(Vector2.right, 100f),
            };

            var closest = raycastHit2D.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();

            if (closest.collider != null)
            {
                base.transform.position = closest.point + closest.normal * col.size.y / 3f * transform.localScale.y;
                var angles = transform.localEulerAngles;
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
                transform.localEulerAngles = angles;
                if(corpse != null)
                {
                    var fixer = corpse.gameObject.AddComponent<CorpseOrientationFixer>();
                    fixer.corpseAngle = angles.z;
                }
            }
        }

        void Start()
        {
            this.col = base.GetComponent<BoxCollider2D>();
            StickToSurface();

            var fsm = gameObject.LocateMyFSM("Mawlek Turret");

            var spawnPos = fsm.FsmVariables.FindFsmVector3("Spawn Pos");

            var left = fsm.GetState("Fire Left");
            var right = fsm.GetState("Fire Right");

            var leftMin = fsm.FsmVariables.FindFsmFloat("Angle Min L");
            var leftMax = fsm.FsmVariables.FindFsmFloat("Angle Max L");
            var rightMin = fsm.FsmVariables.FindFsmFloat("Angle Min R");
            var rightMax = fsm.FsmVariables.FindFsmFloat("Angle Max R");


            Vector2 up = Vector2.zero;

            float angle = transform.localEulerAngles.z % 360f;
            if(!isFloorTurret)
            {
                angle = (angle + 180f) % 360f;
            }

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
                leftMin.Value = upL.Min;
                leftMax.Value = upL.Max;

                rightMin.Value = upR.Min;
                rightMax.Value = upR.Max;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
                leftMin.Value = leL.Min;
                leftMax.Value = leL.Max;

                rightMin.Value = leR.Min;
                rightMax.Value = leR.Max;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
                leftMin.Value = dnL.Min;
                leftMax.Value = dnL.Max;

                rightMin.Value = dnL.Min;
                rightMax.Value = dnL.Max;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
                leftMin.Value = riL.Min;
                leftMax.Value = riL.Max;

                rightMin.Value = riR.Min;
                rightMax.Value = riR.Max;
            }

            spawnPos.Value = up * spawnDist;
        }
    }

    internal class MawlekTurretPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<MawlekTurretControl>();
            control.isFloorTurret = true;
        }
    }

    //TODO: remove ceiling version entirely
    internal class MawlekTurretCeilingPrefabConfig : IPrefabConfig
    {
        public virtual void SetupPrefab(PrefabObject p)
        {
            //overwrite the upside-down one since the code now works just fine for the upright one in all cases
            var uprightTurret = p.source.Scene.sceneObjects.FirstOrDefault(x => x.LoadedObject.prefabName == "Mawlek Turret");
            p.prefab = uprightTurret.LoadedObject.prefab;

            string keyName = EnemyRandomizerDatabase.ToDatabaseKey(p.prefab.name);
            p.prefabName = keyName;

            var control = p.prefab.AddComponent<MawlekTurretControl>();
            control.isFloorTurret = true;
        }
    }
}
