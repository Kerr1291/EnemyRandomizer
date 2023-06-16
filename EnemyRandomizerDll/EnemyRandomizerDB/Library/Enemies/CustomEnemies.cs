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
using UniRx;

namespace EnemyRandomizerMod
{




    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class ZapCloudControl : DefaultSpawnedEnemyControl
    {
    }

    public class ZapCloudSpawner : DefaultSpawner<ZapCloudControl> { }

    public class ZapCloudPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class JellyEggBombControl : DefaultSpawnedEnemyControl
    {
    }

    public class JellyEggBombSpawner : DefaultSpawner<JellyEggBombControl> { }

    public class JellyEggBombPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class LaserTurretFramesControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public override bool takesSpecialCharmDamage => true;
        public override bool takesSpecialSpellDamage => true;

        //GameObject hitCrystals;
        DamageEnemies damageEnemies;

        IEnumerator StopAfter(GameObject hitEffect, float time)
        {
            yield return new WaitForSeconds(time);
            GameObject.Destroy(hitEffect);
            GameObject.Destroy(gameObject);
        }

        public void Splat()
        {
            GameObject hitEffect = SpawnerExtensions.SpawnEntityAt("Hit Crystals", transform.position);
            StartCoroutine(StopAfter(hitEffect, 0.25f));
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var beam = gameObject.FindGameObjectInDirectChildren("Beam");
            var fsm = beam.LocateMyFSM("Beam Extender");

            damageEnemies = beam.GetOrAddComponent<DamageEnemies>();
            damageEnemies.damageDealt = 100;
            damageEnemies.direction = 90f; //TODO: make sure this isn't backwards
            damageEnemies.magnitudeMult = 1f;
            damageEnemies.moveDirection = false;
            damageEnemies.circleDirection = false;
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Generic;
            damageEnemies.specialType = 0;

            var hm = gameObject.GetOrAddComponent<HealthManager>();
            hm.hp = 64;//TODO:
            hm.OnDeath += Splat;
            hm.IsInvincible = false;
            hm.hasSpecialDeath = false;
            Geo = 7;
            MRenderer.enabled = true;
            Collider.enabled = true;

            var explode = SpawnerExtensions.SpawnEntityAt("Death Puff Med", transform.position, null, false, false);
            explode.transform.SetParent(transform);
            explode.transform.localPosition = Vector3.zero;

            var de = gameObject.GetOrAddComponent<EnemyDeathEffectsUninfected>();
            de.doKillFreeze = false;
            de.uninfectedDeathPt = explode;

            var tinker = gameObject.GetComponent<TinkEffect>();
            if (tinker != null)
                GameObject.Destroy(tinker);

            //var extend = fsm.GetState("Extend");
            //extend.InsertCustomAction(() => {

            //    var myUp = gameObject.GetUpFromSelfAngle(false).normalized * 2f;
            //    var toUp = SpawnerExtensions.GetRayOn(transform.position.ToVec2() + myUp, myUp.normalized, 100f).distance;
            //    fsm.FsmVariables.GetFsmFloat("Ray Distance").Value = toUp; }, 1);

            //float angle = SpawnerExtensions.RotateToDirection(dir);
            //gameObject.SetRotation(angle);
        }
    }

    public class LaserTurretFramesSpawner : DefaultSpawner<LaserTurretFramesControl> { }

    public class LaserTurretFramesPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////





    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class LaserTurretControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        public override bool takesSpecialCharmDamage => true;
        public override bool takesSpecialSpellDamage => true;

        //GameObject hitCrystals;
        DamageEnemies damageEnemies;

        IEnumerator StopAfter(GameObject hitEffect, float time)
        {
            yield return new WaitForSeconds(time);
            GameObject.Destroy(hitEffect);
            GameObject.Destroy(gameObject);
        }

        public void Splat()
        {
            GameObject hitEffect = SpawnerExtensions.SpawnEntityAt("Hit Crystals", transform.position);
            StartCoroutine(StopAfter(hitEffect, 0.25f));
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            var beam = gameObject.FindGameObjectInDirectChildren("Beam");
            var fsm = beam.LocateMyFSM("Beam Extender");

            damageEnemies = beam.GetOrAddComponent<DamageEnemies>();
            damageEnemies.damageDealt = 100;
            damageEnemies.direction = 90f; //TODO: make sure this isn't backwards
            damageEnemies.magnitudeMult = 1f;
            damageEnemies.moveDirection = false;
            damageEnemies.circleDirection = false;
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Generic;
            damageEnemies.specialType = 0;

            var hm = gameObject.GetOrAddComponent<HealthManager>();
            hm.hp = 64;//TODO:
            hm.OnDeath += Splat;
            hm.IsInvincible = false;
            hm.hasSpecialDeath = false;
            Geo = 7;
            MRenderer.enabled = true;
            Collider.enabled = true;

            var explode = SpawnerExtensions.SpawnEntityAt("Death Puff Med", transform.position, null, false, false);
            explode.transform.SetParent(transform);
            explode.transform.localPosition = Vector3.zero;

            var de = gameObject.GetOrAddComponent<EnemyDeathEffectsUninfected>();
            de.doKillFreeze = false;
            de.uninfectedDeathPt = explode;

            var tinker = gameObject.GetComponent<TinkEffect>();
            if (tinker != null)
                GameObject.Destroy(tinker);

            //var extend = fsm.GetState("Extend");
            //extend.InsertCustomAction(() => {

            //    var myUp = gameObject.GetUpFromSelfAngle(false).normalized * 2f;
            //    var toUp = SpawnerExtensions.GetRayOn(transform.position.ToVec2() + myUp, myUp.normalized, 100f).distance;
            //    fsm.FsmVariables.GetFsmFloat("Ray Distance").Value = toUp; }, 1);

            //float angle = SpawnerExtensions.RotateToDirection(dir);
            //gameObject.SetRotation(angle);
        }
    }

    public class LaserTurretSpawner : DefaultSpawner<LaserTurretControl> { }

    public class LaserTurretPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////








    /////////////////////////////////////////////////////////////////////////////
    ///// 
    public class BeeDropperControl : DefaultSpawnedEnemyControl
    {
        public override string FSMName => "Control";

        ParticleSystem ptBurst;
        GameObject splat;


        protected virtual void Splat()
        {
            splat.SafeSetActive(true);
            MRenderer.enabled = false;
            Collider.enabled = false;
            ptBurst.Emit(0);
        }

        public override void Setup(GameObject other)
        {
            base.Setup(other);

            gameObject.RemoveFSM("Recoil");
            gameObject.RemoveFSM("Control");

            ptBurst = gameObject.FindGameObjectInDirectChildren("Pt Burst").GetComponent<ParticleSystem>();
            splat = gameObject.FindGameObjectInChildrenWithName("Splat");

            var hm = gameObject.GetOrAddComponent<HealthManager>();
            hm.hp = 10;//TODO:

            hm.OnDeath += Splat;
            Geo = 11;
            PhysicsBody.isKinematic = false;
            PhysicsBody.gravityScale = 0f;
            MRenderer.enabled = true;
            Collider.enabled = true;

            var dir = gameObject.GetRandomDirectionFromSelf();
            PhysicsBody.velocity = dir * 13f;

            float angle = SpawnerExtensions.RotateToDirection(dir);
            gameObject.SetRotation(angle);
        }

        public float nearToWall = .5f;
        public float rayRange = 10f;
        public float fastRayDistance = 5f;
        public bool inFastCheckMode = false;
        public float slowCheckTimer = 1f;
        float timer;
        int bounceCD = 5;
        int bounceiscd = 0;

        protected override void Update()
        {
            base.Update();

            if (PhysicsBody != null && PhysicsBody.velocity.magnitude < 1f)
            {
                PhysicsBody.velocity = new Vector2(1f, 1f);
            }

            if (inFastCheckMode)
            {
                if (bounceiscd > 0)
                {
                    bounceiscd--;
                    return;
                }

                timer = 0f;
                var result = CheckForNearbyWalls();
                if (result != null && result.Value.collider != null)
                {
                    if (result.Value.distance > fastRayDistance)
                        inFastCheckMode = false;
                    else
                    {
                        if (result.Value.distance < nearToWall)
                        {
                            PhysicsBody.velocity = PhysicsBody.velocity.Reflect(result.Value.normal);
                            float angle = SpawnerExtensions.RotateToDirection(PhysicsBody.velocity.normalized);
                            gameObject.SetRotation(angle);
                            bounceiscd = bounceCD;
                        }
                    }
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > slowCheckTimer)
                {
                    timer = 0f;
                    var result = CheckForNearbyWalls();
                    if (result != null && result.Value.collider != null)
                    {
                        if (result.Value.distance < fastRayDistance)
                            inFastCheckMode = true;
                    }
                }
            }
        }

        protected virtual RaycastHit2D? CheckForNearbyWalls()
        {
            RaycastHit2D? closest = SpawnerExtensions.GetOctagonalRays(gameObject, rayRange).Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
            return closest;
        }
    }

    public class BeeDropperSpawner : DefaultSpawner<BeeDropperControl> { }

    public class BeeDropperPrefabConfig : DefaultPrefabConfig { }
    /////
    //////////////////////////////////////////////////////////////////////////////



}
