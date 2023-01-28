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
    public class MawlekTurretControl : DefaultEnemyController
    {
        Range upL = new Range(95f, 110f);
        Range upR = new Range(70f, 85f);

        Range dnL = new Range(270f, 350f);
        Range dnR = new Range(190f, 270f);

        Range leL = new Range(340f, 10f);
        Range leR = new Range(20f, 45f);

        Range riL = new Range(160f, 170f);
        Range riR = new Range(180f, 200f);

        public float shotSpeed = 10f;

        public Vector2 spawnPos;
        public float spawnDist = 1.4f;

        void Start()
        {
            var fsm = Instance.LocateMyFSM("Mawlek Turret");

            var spawnPos = fsm.FsmVariables.FindFsmVector3("Spawn Pos");

            var left = fsm.GetState("Fire Left");
            var right = fsm.GetState("Fire Right");

            var leftMin = fsm.FsmVariables.FindFsmFloat("Angle Min L");
            var leftMax = fsm.FsmVariables.FindFsmFloat("Angle Max L");
            var rightMin = fsm.FsmVariables.FindFsmFloat("Angle Min R");
            var rightMax = fsm.FsmVariables.FindFsmFloat("Angle Max R");


            Vector2 up = Vector2.zero;

            float angle = transform.localEulerAngles.z;
            if(      angle > upL.Min && angle < upL.Max)
            {
                up = Vector2.up;
                leftMin.Value = upL.Min;
                leftMax.Value = upL.Max;

                rightMin.Value = upR.Min;
                rightMax.Value = upR.Max;
            }
            else if (angle > dnR.Min && angle < dnL.Max)
            {
                up = Vector2.down;
                leftMin.Value = dnL.Min;
                leftMax.Value = dnL.Max;

                rightMin.Value = upR.Min;
                rightMax.Value = upR.Max;
            }
            else if (angle > -20f && angle < leR.Max)
            {
                up = Vector2.left;
                leftMin.Value = leL.Min;
                leftMax.Value = leL.Max;

                rightMin.Value = leR.Min;
                rightMax.Value = leR.Max;
            }
            else if (angle > riL.Min && angle < riR.Max)
            {
                leftMin.Value = riL.Min;
                leftMax.Value = riL.Max;

                rightMin.Value = upR.Min;
                rightMax.Value = upR.Max;
            }

            spawnPos.Value = up * spawnDist;
        }
    }

    public class MawlekTurret : DefaultEnemy
    {
        public override void SetupPrefab()
        {
            Dev.Where();
            Prefab.AddComponent<MawlekTurretControl>();
        }
    }
}