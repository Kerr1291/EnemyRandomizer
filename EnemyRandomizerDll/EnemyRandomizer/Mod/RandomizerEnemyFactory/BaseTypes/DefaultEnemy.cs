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
using HutongGames.PlayMaker;
//using EnemyRandomizerMod.Extensions;
using HutongGames.PlayMaker.Actions;
//using EnemyRandomizerMod.Behaviours;
using System.Reflection;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class DefaultEnemyController : MonoBehaviour
    {
        public GameObject replacedEnemy;

        //destroys the game object after this time if it wasn't killed properly
        protected float forceDeleteOOBTimer = .5f;

        //protected virtual IEnumerator Start()
        //{
        //    if (!IsEnemyRandomizerEnemy())
        //    {
        //        Destroy(this);
        //        yield break;
        //    }

        //    while(IsInBounds() && !gameObject.IsEnemyDead())
        //        yield return new WaitForEndOfFrame();

        //    if (ShouldForceKill())
        //    {
        //        Dev.Log("Sending force kill for out of scene to " + gameObject.name);
        //        DoForceKill();
        //        yield return new WaitForSeconds(forceDeleteOOBTimer);
        //        Destroy(gameObject);
        //    }

        //    yield break;
        //}

        protected virtual bool IsEnemyRandomizerEnemy()
        {
            return name.StartsWith(EnemyRandomizer.ENEMY_RANDO_PREFIX);
        }

        protected virtual bool IsInBounds()
        {
            return EnemyRandomizer.Instance.IsInBounds(gameObject);
        }

        protected virtual bool ShouldForceKill()
        {
            return !gameObject.IsEnemyDead() && name.Contains(EnemyRandomizer.ENEMY_RANDO_PREFIX);
        }

        protected virtual void DoForceKill()
        {
            var hm = GetComponent<HealthManager>();
            if (hm == null)
                Destroy(this);
            else
                hm.Die(null, AttackTypes.Splatter, true);
        }
    }

    public class DefaultEnemy : IRandomizerEnemy
    {
        public GameObject EnemyObject { get; protected set; }

        public virtual void Setup(EnemyData enemy, List<EnemyData> knownEnemyTypes, GameObject prefabObject)
        {
            EnemyObject = prefabObject;

            //TODO: look into if this is still a good idea. imported from my old rando logic
            try
            {
                //delete persistant bool items
                {
                    PersistentBoolItem pbi = EnemyObject.GetComponent<PersistentBoolItem>();
                    if (pbi != null)
                    {
                        GameObject.Destroy(pbi);
                    }
                }

                //remove this, because it can deactivate some enemies....
                {
                    DeactivateIfPlayerdataTrue toRemove = EnemyObject.GetComponent<DeactivateIfPlayerdataTrue>();
                    if (toRemove != null)
                    {
                        GameObject.Destroy(toRemove);
                    }
                }
            }
            catch (Exception e)
            {
                Dev.LogError("Failed to customize prefab for enemy: " + enemy.name);
                Dev.LogError("Error: " + e.Message + " Stacktrace: " + e.StackTrace);
            }
        }

        public virtual GameObject Instantiate(EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            var newEnemy = GameObject.Instantiate(EnemyObject);
            Dev.Log("Trying to make " + newEnemy.name);

            ScaleNewEnemy(newEnemy, sourceData, enemyToReplace, matchingData);
            RotateNewEnemy(newEnemy, sourceData, enemyToReplace, matchingData);
            PositionNewEnemy(newEnemy, sourceData, enemyToReplace, matchingData);
            ModifyNewEnemyGeo(newEnemy, sourceData, enemyToReplace, matchingData);
            FinalizeNewEnemy(newEnemy, sourceData, enemyToReplace, matchingData);
            NameNewEnemy(newEnemy, sourceData, enemyToReplace, matchingData);

            return newEnemy;
        }


        public virtual void SetNewEnemyParent(GameObject newEnemy, GameObject enemyToReplace = null)
        {
            if (enemyToReplace == null)
                return;

            newEnemy.transform.SetParent(enemyToReplace.transform.parent);
        }

        public virtual void ScaleNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            if (enemyToReplace == null)
                return;

            Vector3 originalNewEnemyScale = newEnemy.transform.localScale;

            Collider2D newC = newEnemy.GetComponent<Collider2D>();
            Collider2D oldC = enemyToReplace == null ? null : enemyToReplace.GetComponent<Collider2D>();
            tk2dSprite newS = newEnemy.GetComponent<tk2dSprite>();
            tk2dSprite oldS = enemyToReplace == null ? null : enemyToReplace.GetComponent<tk2dSprite>();

            //TODO: this logic still seems broken so disable it for now
            //TODO: create a settings bool for silly scaling
            //new scale logic, compare the size of the colliders and adjust the scale by the ratios
            if (newC != null && oldC != null && newS != null && oldS != null)
            {
                //bounds returns null on colliders of uninstantiated and unactivated game objects
                //so we need to determine the type of collider manually by downcasting and then query its bounds by hand
                Vector2 newSize = Vector2.zero;
                Vector2 oldSize = Vector2.zero;

                {
                    PolygonCollider2D newCPoly = newC as PolygonCollider2D;
                    CircleCollider2D newCCircle = newC as CircleCollider2D;
                    BoxCollider2D newCBox = newC as BoxCollider2D;
                    if (newCPoly != null)
                    {
                        newSize = new Vector2(newCPoly.points.Select(x => x.x).Max() - newCPoly.points.Select(x => x.x).Min(), newCPoly.points.Select(x => x.y).Max() - newCPoly.points.Select(x => x.y).Min());
                    }
                    else if(newCCircle != null)
                    {
                        newSize = Vector2.one * newCCircle.radius;
                    }
                    else if(newCBox != null)
                    {
                        newSize = newCBox.size;
                    }
                    else if(newS != null)
                    {
                        newSize = newS.GetBounds().size;
                    }
                }
                {
                    PolygonCollider2D oldCPoly = oldC as PolygonCollider2D;
                    CircleCollider2D oldCCircle = oldC as CircleCollider2D;
                    BoxCollider2D oldCBox = oldC as BoxCollider2D;
                    if (oldCPoly != null)
                    {
                        oldSize = new Vector2(oldCPoly.points.Select(x => x.x).Max() - oldCPoly.points.Select(x => x.x).Min(), oldCPoly.points.Select(x => x.y).Max() - oldCPoly.points.Select(x => x.y).Min());
                    }
                    else if(oldCCircle != null)
                    {
                        oldSize = Vector2.one * oldCCircle.radius;
                    }
                    else if(oldCBox != null)
                    {
                        oldSize = oldCBox.size;
                    }
                    else if (newS != null)
                    {
                        newSize = newS.GetBounds().size;
                    }
                }

                Dev.Log(oldSize.x + " / " + newSize.x);
                Dev.Log(oldSize.y + " / " + newSize.y);

                float scaleX = oldSize.x / newSize.x;
                float scaleY = oldSize.y / newSize.y;
                float scale = scaleX > scaleY ? scaleY : scaleX;

                if (scale < .1f)
                    scale = .1f;

                if (scale > 2.5f)
                    scale = 2.5f;

                bool tryAlternate = false;
                try
                {
                    if (newS.boxCollider2D != null)
                    {
                        newS.scale = new Vector3(newS.scale.x * scale, newS.scale.y * scale, 1.0f);
                        Vector2 b = newS.boxCollider2D.size;
                        b.x *= scale;
                        b.y *= scale;
                        newS.boxCollider2D.size = b;
                    }
                    else
                    {
                        tryAlternate = true;
                    }
                }
                catch (Exception e)
                {
                    tryAlternate = true;
                    Dev.LogError("Error applying scale: " + e.Message);
                }

                if (tryAlternate)
                {
                    newEnemy.transform.localScale = new Vector3(newEnemy.transform.localScale.x * scale,
                        newEnemy.transform.localScale.y * scale, 1f);
                }
            }
            //no two colliders to compare? then do old logic (until it's removed....)
            else
            {
                if (matchingData != null)
                {
                    if (sourceData.isLargeEnemy && !matchingData.isLargeEnemy)
                    {
                        newEnemy.transform.localScale = newEnemy.transform.localScale * .5f;
                    }
                    else if (!sourceData.isLargeEnemy && matchingData.isLargeEnemy)
                    {
                        newEnemy.transform.localScale = newEnemy.transform.localScale * 1.5f;
                    }

                    if (sourceData.name.Contains("Mawlek Turret") && !matchingData.isLargeEnemy)
                    {
                        newEnemy.transform.localScale = originalNewEnemyScale * .5f;
                    }

                    //shrink him!
                    if (sourceData.name.Contains("Mantis Traitor Lord") && !matchingData.isLargeEnemy)
                    {
                        newEnemy.transform.localScale = newEnemy.transform.localScale * .4f;
                    }
                }
                else
                {
                    //just always shrink these things a bit...
                    if (sourceData.name.Contains("Mawlek Turret"))
                    {
                        newEnemy.transform.localScale = originalNewEnemyScale * .6f;
                    }
                }
            }
        }

        public virtual void RotateNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            if (enemyToReplace != null)
            {
                if (!newEnemy.name.Contains("Ceiling Dropper"))
                    newEnemy.transform.rotation = enemyToReplace.transform.rotation;

                //if they were a wall flying mantis, don't rotate the replacement
                if (enemyToReplace.name.Contains("Mantis Flyer Child"))
                {
                    newEnemy.transform.rotation = Quaternion.identity;
                }

                if(newEnemy.name.Contains("Mawlek Turret"))
                {
                    Quaternion rotate = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                    newEnemy.transform.rotation = rotate * enemyToReplace.transform.rotation;
                }

                //mosquitos rotate, so spawn replacements with default rotation
                if (enemyToReplace.name.Contains("Mosquito"))
                {
                    newEnemy.transform.rotation = Quaternion.identity;
                }

                //TODO: look up what's going on with this
                if (newEnemy.name.Contains("Crystallised Lazer Bug") && enemyToReplace != null)
                {
                    //Dev.Log("Old rotation = " + newEnemy.transform.rotation.eulerAngles);
                    Quaternion rotate = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                    newEnemy.transform.rotation = rotate * enemyToReplace.transform.rotation;
                    //Dev.Log("New rotation = " + newEnemy.transform.rotation.eulerAngles);
                    //newEnemy.PrintSceneHierarchyTree(true);
                }

                if (newEnemy.name.Contains("Mines Crawler"))
                {
                    Quaternion rot180degrees = Quaternion.Euler(-enemyToReplace.transform.rotation.eulerAngles);
                    newEnemy.transform.rotation = rot180degrees * enemyToReplace.transform.rotation;
                    newEnemy.transform.rotation = Quaternion.identity;
                }

                //TODO: test me
                if (newEnemy.name.Contains("Abyss Crawler"))
                {
                    Quaternion rot180degrees = Quaternion.Euler(-enemyToReplace.transform.rotation.eulerAngles);
                    newEnemy.transform.rotation = rot180degrees * enemyToReplace.transform.rotation;
                    newEnemy.transform.rotation = Quaternion.identity;
                }
                //if( oldEnemy.name.Contains( "Moss Walker" ) )
                //{
                //    //Quaternion rot180degrees = Quaternion.Euler(-oldEnemy.transform.rotation.eulerAngles);
                //    //newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
                //    newEnemy.transform.rotation = Quaternion.identity;
                //}
            }
        }

        public virtual void PositionNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            //adjust the position to take into account the new monster type and/or size
            if (enemyToReplace != null)
            {
                newEnemy.transform.position = enemyToReplace.transform.position;
            }

            BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
            Vector3 positionOffset = Vector3.zero;
            Vector3 onGround = Vector3.zero;
            Vector2 scale = newEnemy.transform.localScale;

            if (sourceData.IsGroundEnemy())
            {
                Vector3 toGround = newEnemy.GetNearestVectorToGround(50f);
                onGround = newEnemy.GetNearestPointOnGround(50f);

                newEnemy.transform.position = onGround;

                positionOffset = new Vector3(0f, collider.size.y * scale.y, 0f);

                if (newEnemy.name.Contains("Lobster"))
                {
                    positionOffset = positionOffset + (Vector3)(Vector2.up * 2f) * scale.y;
                }
                if (newEnemy.name.Contains("Blocker"))
                {
                    positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                }
                if (newEnemy.name == ("Moss Knight"))
                {
                    positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                }
                if (newEnemy.name == ("Enemy"))
                {
                    positionOffset = positionOffset + (Vector3)(Vector2.up * -0.5f) * scale.y;
                }
            }
            else
            {
                onGround = Vector3.zero;
            }

            Vector2 originalUp = (enemyToReplace == null) ? Vector2.up : new Vector2(enemyToReplace.transform.up.normalized.x, enemyToReplace.transform.up.normalized.y);

            Vector2 ePos = newEnemy.transform.position;

            Vector2 upOffset = ePos + originalUp * 5f;

            Vector2 originalDown = -originalUp;

            float projectionDistance = 500f;

            Vector3 toSurface = IRandomizerEnemyGameObjectExtensions.GetNearestVectorToChunk(ePos, projectionDistance);

            Vector2 finalDir = toSurface.normalized;

            bool newEnemyIsMantisFlyerChild = newEnemy.name.Contains("Mantis Flyer Child");

            bool wasCrawlerEnemy = matchingData != null && matchingData.IsCrawlerEnemy();

            if (enemyToReplace != null && enemyToReplace.name.Contains("Moss Walker"))
            {
                positionOffset = originalUp * collider.size.y * scale.y;
            }

            if (newEnemyIsMantisFlyerChild || sourceData.IsWallEnemy() || sourceData.IsCrawlerEnemy() || wasCrawlerEnemy)
            {

                //project the ceiling droppers onto the ceiling
                if (newEnemy.name.Contains("Ceiling Dropper"))
                {
                    projectionDistance = 500f;
                    toSurface = IRandomizerEnemyGameObjectExtensions.GetVectorTo(ePos, Vector2.up, projectionDistance);
                    onGround = IRandomizerEnemyGameObjectExtensions.GetPointOn(ePos, finalDir, projectionDistance);
                }
                else
                {
                    finalDir = toSurface.normalized;
                    onGround = IRandomizerEnemyGameObjectExtensions.GetNearestPointOnGround(ePos, projectionDistance);
                }

                newEnemy.transform.position = onGround;

                if (newEnemy.name.Contains("Plant Trap"))
                {
                    positionOffset = originalUp * 2f * scale.y;
                }
                if (collider != null && newEnemy.name.Contains("Mawlek Turret"))
                {
                    positionOffset = originalUp * collider.size.y / 3f * scale.y;
                }
                if (collider != null && newEnemy.name.Contains("Mushroom Turret"))
                {
                    positionOffset = (originalUp * .5f) * scale.y;
                }
                if (newEnemy.name.Contains("Plant Turret"))
                {
                    positionOffset = originalUp * .7f * scale.y;
                }
                if (collider != null && newEnemy.name.Contains("Laser Turret"))
                {
                    positionOffset = originalUp * collider.size.y / 10f * scale.y;
                }
                if (collider != null && newEnemy.name.Contains("Worm"))
                {
                    positionOffset = originalUp * collider.size.y / 3f * scale.y;
                }

                if (newEnemy.name.Contains("Ceiling Dropper"))
                {
                    //move it down a bit, keeps spawning in roof
                    positionOffset = Vector3.down * 2f * scale.y;
                }

                if (sourceData.IsCrawlerEnemy() || wasCrawlerEnemy)
                {
                    //positionOffset =  * 1f;
                    //BoxCollider2D collider = newEnemy.GetComponent<BoxCollider2D>();
                    if (collider != null)
                        positionOffset = new Vector3(collider.size.x * originalUp.x * scale.x, collider.size.y * originalUp.y * scale.y, 0f);

                    //TODO: Testing new values
                    if (newEnemy.name.Contains("Crystallised Lazer Bug"))
                    {
                        //suppposedly 1/2 their Y collider space offset should be 1.25
                        //but whatever we set it at, they spawn pretty broken, so spawn them out of the ground a bit so they're still a threat
                        positionOffset = -finalDir * collider.size.y * 1.5f * scale.y;
                    }

                    //TODO: Testing new values
                    if (newEnemy.name.Contains("Mines Crawler"))
                    {
                        positionOffset = -finalDir * 1.5f * scale.y;
                    }

                    //TODO: Testing new values
                    if (newEnemy.name.Contains("Spider Mini"))
                    {
                        positionOffset = -finalDir * collider.size.y * 1.5f * scale.y; ;
                    }

                    //TODO: Testing new values
                    if (newEnemy.name.Contains("Abyss Crawler"))
                    {
                        positionOffset = -finalDir * collider.size.y * 1.5f * scale.y; ;
                    }

                    //TODO: Testing new values
                    if (newEnemy.name.Contains("Climber") || wasCrawlerEnemy)
                    {
                        positionOffset = -finalDir * collider.size.y * 1.5f * scale.y;
                    }
                }

                //show adjustment
                //Dev.CreateLineRenderer( onGround, newEnemy.transform.position + positionOffset, Color.red, -1f );
            }

            newEnemy.transform.position = newEnemy.transform.position + positionOffset;
        }

        public virtual void ModifyNewEnemyGeo(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            if (EnemyRandomizer.GlobalSettings.RandomizeGeo)
            {
                int smallGeo = EnemyRandomizer.pRNG.Rand(0, !sourceData.isLargeEnemy ? 20 : 10);
                int medGeo = EnemyRandomizer.pRNG.Rand(0, 2);
                int bigGeo = EnemyRandomizer.pRNG.Rand(0, sourceData.isLargeEnemy ? 20 : 10);

                if (sourceData.isHard)
                {
                    smallGeo += EnemyRandomizer.pRNG.Rand(0, 40);
                    medGeo += EnemyRandomizer.pRNG.Rand(0, 20);
                    bigGeo += EnemyRandomizer.pRNG.Rand(0, 20);
                }

                if (sourceData.isBoss)
                {
                    smallGeo += EnemyRandomizer.pRNG.Rand(0, 10);
                    medGeo += EnemyRandomizer.pRNG.Rand(0, 40);
                    bigGeo += EnemyRandomizer.pRNG.Rand(0, 60);
                }

                newEnemy.SetEnemyGeoRates(smallGeo, medGeo, bigGeo);
            }
            else
            {
                //don't mess with geo rates then.. but add some to these guys? test...
                if (newEnemy.name == "Bursting Zombie")
                {
                    int smallGeo = EnemyRandomizer.pRNG.Rand(0, 5);
                    int medGeo = EnemyRandomizer.pRNG.Rand(1, 2);
                    int bigGeo = EnemyRandomizer.pRNG.Rand(0, 1);

                    newEnemy.SetEnemyGeoRates(smallGeo, medGeo, bigGeo);
                }

                if (newEnemy.name == "Lazy Flyer Enemy")
                {
                    int smallGeo = 0;
                    int medGeo = 0;
                    int bigGeo = EnemyRandomizer.pRNG.Rand(0, 10);

                    newEnemy.SetEnemyGeoRates(smallGeo, medGeo, bigGeo);
                }
            }
        }

        public virtual void FinalizeNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            if (enemyToReplace != null)
            {
                //hook up the old enemy's battle scene to the new enemy replacement
                HealthManager oldHM = enemyToReplace.GetComponent<HealthManager>();
                HealthManager newHM = newEnemy.GetComponent<HealthManager>();
                if (oldHM != null && newHM != null)
                {
                    FieldInfo fi = oldHM.GetType().GetField("battleScene", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fi != null)
                    {
                        GameObject battleScene = fi.GetValue(oldHM) as GameObject;
                        if (battleScene != null)
                            fi.SetValue(newHM, battleScene);
                    }
                }
            }

            newEnemy.AddComponent<DefaultEnemyController>();

            // Checks if enemy has a roar, then disable the roar by skipping over the roar state
            // Fixes the roar push out of bounds on room entry issue. Workaround until I work out how to disable the stun/push
            PlayMakerFSM control = FSMUtility.GetFSM(newEnemy);
            if (control != null)
            {
                FsmState roar = control.FsmStates.Where(state => state.Name == "Roar").FirstOrDefault();

                if (roar != null)
                {
                    foreach (FsmState fsmS in control.FsmStates)
                    {
                        foreach (FsmTransition trans in fsmS.Transitions)
                        {
                            if (trans.ToState == "Roar")
                            {
                                trans.ToState = "Roar End";
                            }
                        }
                    }
                }
            }
        }

        public virtual void NameNewEnemy(GameObject newEnemy, EnemyData sourceData, GameObject enemyToReplace = null, EnemyData matchingData = null)
        {
            newEnemy.name = EnemyRandomizer.ENEMY_RANDO_PREFIX + newEnemy.name; //gameObject.name; //if we put the game object's name here it'll re-randomize itself (whoops)
        }
    }
}
