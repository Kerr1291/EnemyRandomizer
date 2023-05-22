using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;

namespace EnemyRandomizerMod
{
    //public static class ClimberVars
    //{
    //    public static float maxClimberSlope = 5f;
    //}


    public class DefaultClimberControl : DefaultSpawnedEnemyControl
    {
        public override float spawnPositionOffset => .5f;

        //protected virtual void OnCollisionEnter2D(Collision2D collision)
        //{
        //    if (collision.gameObject.tag == "Enemy Message" || collision.gameObject.layer == LayerMask.NameToLayer("Enemy Detector"))
        //    {
        //        Flip();
        //    }
        //}

        //protected virtual void Flip()
        //{
        //    var climber = gameObject.GetComponent<Climber>();
        //    var body = gameObject.GetComponent<Rigidbody2D>();
        //    var bflags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        //    //try
        //    //{
        //    //    var old = (bool)climber.GetType().GetField("clockwise", bflags).GetValue(climber);
        //    //    climber.GetType().GetField("clockwise", bflags).SetValue(climber, !old);
        //    //}
        //    //catch
        //    //{
        //    //    Dev.LogError("Error changing the clockwise bool");
        //    //}

        //    string[] names = new string[0];
        //    string name = null;
        //    Enum oldd = null;
        //    try
        //    {
        //        oldd = (System.Enum)climber.GetType().GetField("currentDirection", bflags).GetValue(climber);

        //        names = System.Enum.GetNames(oldd.GetType());
        //        name = System.Enum.GetName(oldd.GetType(), oldd);
        //    }
        //    catch
        //    {
        //        Dev.LogError("Error getting the current direction enum values");
        //    }

        //    try
        //    {
        //        if (name == names[0])
        //            name = names[2];
        //        else if (name == names[2])
        //            name = names[0];
        //        else if (name == names[1])
        //            name = names[3];
        //        else if (name == names[3])
        //            name = names[1];

        //        if (name == null)
        //            Dev.LogError("enum name is null");
        //        if (names.Length <= 0)
        //            Dev.LogError("no enums names found");

        //        climber.GetType().GetField("currentDirection", bflags).SetValue(climber, System.Enum.Parse(oldd.GetType(),name));
        //    }
        //    catch
        //    {
        //        Dev.LogError("Error setting the new current direction value");
        //    }


        //    try
        //    {
        //        if (name == names[0] || name == names[2])
        //        {
        //            if (body.velocity.x == 0f)
        //                body.velocity = new Vector2(climber.speed * (name == names[0] ? 1f : -1f), 0f);

        //            body.velocity = new Vector2(-body.velocity.x, body.velocity.y);
        //        }
        //        else if (name == names[1] || name == names[3])
        //        {
        //            if (body.velocity.y == 0f)
        //                body.velocity = new Vector2(0f, climber.speed * (name == names[3] ? 1f : -1f));

        //            body.velocity = new Vector2(body.velocity.x, -body.velocity.y);
        //        }
        //        StartCoroutine(FlipScale());
        //    }
        //    catch
        //    {
        //        Dev.Log("Error performing flip");
        //    }
        //}

        //IEnumerator FlipScale()
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //}

        //protected virtual void Update()
        //{
        //    //var downLocal = transform.TransformDirection(Vector2.down);
        //    //var ground = SpawnerExtensions.FireRayLocal(gameObject, downLocal, 1f);
        //    //if (ground.collider != null)
        //    //{
        //    //    //var slope = Vector2.Angle(ground.normal, -downLocal);
        //    //    //if (Mathf.Abs(slope) > ClimberVars.maxClimberSlope)
        //    //    //    Flip();
        //    //}

        //    if (thisMetadata != null && thisMetadata.PhysicsBody != null && thisMetadata.PhysicsBody.velocity.magnitude <= 0)
        //    {
        //        var bflags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        //        var climber = gameObject.GetComponent<Climber>();
        //        if (climber == null)
        //            return;

        //        var turning = (Coroutine)climber.GetType().GetField("turnRoutine", bflags).GetValue(climber);
        //        if (turning == null)
        //        {
        //            //stuck, flip us
        //            Flip();
        //        }
        //    }
        //}
    }








    /////////////////////////////////////////////////////////////////////////////
    /////
    public class ClimberControl : DefaultClimberControl
    {
    }

    public class ClimberSpawner : DefaultSpawner<ClimberControl> { }

    public class ClimberPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////







    /////////////////////////////////////////////////////////////////////////////
    /////
    public class CrystallisedLazerBugControl : DefaultClimberControl
    {
    }

    public class CrystallisedLazerBugSpawner : DefaultSpawner<CrystallisedLazerBugControl> { }

    public class CrystallisedLazerBugPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///











    /////////////////////////////////////////////////////////////////////////////
    /////
    public class SpiderMiniControl : DefaultClimberControl
    {
        public float shotSpeed = 15f; //taken from the FSM

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            control = gameObject.LocateMyFSM("Shoot");

            var fire = control.GetState("Fire");
            fire.DisableAction(3);
            fire.InsertCustomAction(() =>
            {
                var dirToHero = gameObject.DirectionToPlayer();

                var shot = control.FsmVariables.GetFsmGameObject("Shot").Value;
                if(shot == null)
                {
                    var spitterShot = SpawnerExtensions.SpawnEntityAt("Spitter Shot R", pos2d + dirToHero, true);
                    if (spitterShot != null)
                    {
                        shot = spitterShot;
                        control.FsmVariables.GetFsmGameObject("Shot").Value = spitterShot;
                    }
                }

                var body = shot.GetComponent<Rigidbody2D>();
                if (body != null)
                    body.velocity = dirToHero * shotSpeed;
            },3);

            var spit = control.GetState("Spit");
            spit.DisableAction(2);
            spit.InsertCustomAction(() =>
            {
                var dir = (heroPos2d - pos2d).normalized;
                if(dir.x < 0)
                {
                    float angle = Vector2.SignedAngle(Vector2.left, dir);
                    control.FsmVariables.GetFsmFloat("AngleToHero").Value = angle;
                }
                else
                {
                    float angle = Vector2.SignedAngle(Vector2.right, dir);
                    control.FsmVariables.GetFsmFloat("AngleToHero").Value = angle;
                }
            },0);
        }
    }

    public class SpiderMiniSpawner : DefaultSpawner<SpiderMiniControl> { }

    public class SpiderMiniPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
