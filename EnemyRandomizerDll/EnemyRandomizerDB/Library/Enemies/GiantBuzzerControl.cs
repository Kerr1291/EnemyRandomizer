using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyRandomizerMod
{
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Big Buzzer";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            DisableSendEvents(control,
                ("Roar Left", 0),
                ("Roar Right", 0)
                );

            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.AddCustomAction(() =>
            {
                var left = db.Spawn("Buzzer", null);
                var right = db.Spawn("Buzzer", null);

                var leftMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 50f).point;
                var rightMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 50f).point;

                var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

                var leftSpawn = pos2d + Vector2.left * 20f;
                var rightSpawn = pos2d + Vector2.right * 20f;

                var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                left.transform.position = leftShorter;
                right.transform.position = rightShorter;

                right.SetActive(true);
                left.SetActive(true);

                control.SendEvent("FINISHED");
            });

            control.GetState("Idle").InsertCustomAction(() =>
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }, 0);
        }

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return Mathf.FloorToInt( defaultHP / 2f );
        }
    }


    public class GiantBuzzerSpawner : DefaultSpawner<GiantBuzzerControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            int buzzersInScene = GameObject.FindObjectsOfType<GiantBuzzerControl>().Length;

            //change the spawn to be the col buzzer if zote has been rescued
            if (GameManager.instance.GetPlayerDataBool("zoteRescuedBuzzer") ||
                GameManager.instance.GetPlayerDataInt("zoteDeathPos") == 0 ||
                buzzersInScene > 0)
            {
                var db = EnemyRandomizerDatabase.GetDatabase();
                return base.Spawn(db.Enemies["Giant Buzzer Col"], source);
            }
            else
            {
                return base.Spawn(p, source);
            }
        }
    }

    public class GiantBuzzerPrefabConfig : DefaultPrefabConfig<GiantBuzzerControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
    ///






    /////////////////////////////////////////////////////////////////////////////
    /////
    public class GiantBuzzerColControl : FSMAreaControlEnemy
    {
        public override string FSMName => "Big Buzzer";

        protected override Dictionary<string, Func<FSMAreaControlEnemy, float>> FloatRefs => EMPTY_FLOAT_REFS;

        protected override int ScaleHPFromBossToNormal(int defaultHP, int previousHP)
        {
            return Mathf.FloorToInt(previousHP * 2f);
        }

        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            var db = EnemyRandomizerDatabase.GetDatabase();

            DisableSendEvents(control,
                ("Roar Left", 0),
                ("Roar Right", 0)
                );

            var summon = control.GetState("Summon");
            summon.DisableAction(0);
            summon.DisableAction(1);
            summon.DisableAction(2);
            summon.DisableAction(3);
            summon.AddCustomAction(() =>
            {
                var left = db.Spawn("Buzzer", null);
                var right = db.Spawn("Buzzer", null);

                var leftMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.left, 50f).point;
                var rightMax = gameObject.transform.position.Fire2DRayGlobal(Vector2.right, 50f).point;

                var pos2d = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

                var leftSpawn = pos2d + Vector2.left * 20f;
                var rightSpawn = pos2d + Vector2.right * 20f;

                var leftShorter = (leftMax - pos2d).magnitude < (leftSpawn - pos2d).magnitude ? leftMax : leftSpawn;
                var rightShorter = (rightMax - pos2d).magnitude < (rightSpawn - pos2d).magnitude ? rightMax : rightSpawn;

                left.transform.position = leftShorter;
                right.transform.position = rightShorter;

                right.SetActive(true);
                left.SetActive(true);

                control.SendEvent("FINISHED");
            });

            summon.RemoveTransition("GG BOSS");

            control.ChangeTransition("Init", "FINISHED", "Idle");
            control.ChangeTransition("Init", "GG BOSS", "Idle");

            this.InsertHiddenState(control, "Init", "FINISHED", "Idle");
            this.AddResetToStateOnHide(control, "Init");

            control.GetState("Idle").InsertCustomAction(() =>
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }, 0);
        }
    }

    public class GiantBuzzerColSpawner : DefaultSpawner<GiantBuzzerColControl>
    {
        public override GameObject Spawn(PrefabObject p, ObjectMetadata source)
        {
            return base.Spawn(p, source);
        }
    }

    public class GiantBuzzerColPrefabConfig : DefaultPrefabConfig<GiantBuzzerColControl> { }
    /////
    //////////////////////////////////////////////////////////////////////////////
}
