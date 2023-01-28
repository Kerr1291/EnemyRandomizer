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
using HutongGames.PlayMaker.Actions;
using System.Reflection;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class DefaultEnemyController : MonoBehaviour, IRandomizerEnemyController
    {
        public string InstanceDefinitionName { get; protected set; }

        public IRandomizerEnemy EnemyDefinition { get; protected set; }

        public GameObject Instance { get; protected set; }

        public virtual bool IsDead
        {
            get
            {
                var hm = this.GetHealthManager();
                return hm != null ? hm.GetIsDead() : false;
            }
        }

        public virtual void LinkDataObjects(GameObject newEnemy, IRandomizerEnemy enemyTypeData)
        {
            EnemyDefinition = enemyTypeData;
            Instance = newEnemy;
        }

        public virtual void SetupInstance(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            try
            {
                ScaleNewEnemy(enemyToReplace, dataOfEnemyToReplace);
                RotateNewEnemy(enemyToReplace, dataOfEnemyToReplace);
                PositionNewEnemy(enemyToReplace, dataOfEnemyToReplace);
                ModifyNewEnemyGeo(enemyToReplace, dataOfEnemyToReplace);
                NameNewEnemy(enemyToReplace, dataOfEnemyToReplace);
                FinalizeNewEnemy(enemyToReplace, dataOfEnemyToReplace);
            }
            catch (Exception e)
            {
                Dev.LogError($"Error configuring instance of {EnemyDefinition.Prefab.name}: {e.Message} >> {e.StackTrace}");
            }
        }

        public virtual void SetNewEnemyParent(Transform newParentObject = null)
        {
            if (newParentObject == null)
                return;

            gameObject.transform.SetParent(newParentObject);
        }

        //TODO:::: ADD OPTION TO ALLOW FOR THE AUDIO PITCH TO SCALE WITH SIZE
        public virtual void ScaleNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            if (enemyToReplace == null)
                return;

            Vector3 originalNewEnemyScale = gameObject.transform.localScale;

            Collider2D newC = gameObject.GetComponent<Collider2D>();
            Collider2D oldC = enemyToReplace == null ? null : enemyToReplace.GetComponent<Collider2D>();
            tk2dSprite newS = gameObject.GetComponent<tk2dSprite>();
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
                    else if (newCCircle != null)
                    {
                        newSize = Vector2.one * newCCircle.radius;
                    }
                    else if (newCBox != null)
                    {
                        newSize = newCBox.size;
                    }
                    else if (newS != null)
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
                    else if (oldCCircle != null)
                    {
                        oldSize = Vector2.one * oldCCircle.radius;
                    }
                    else if (oldCBox != null)
                    {
                        oldSize = oldCBox.size;
                    }
                    else if (newS != null)
                    {
                        oldSize = newS.GetBounds().size;
                    }
                }

                //Dev.Log(oldSize.x + " / " + newSize.x);
                //Dev.Log(oldSize.y + " / " + newSize.y);

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
                    //tryAlternate = true;
                    if (newS.boxCollider2D != null)
                    {
                        var allSprites = newS.GetComponentsInChildren<tk2dSprite>(true);
                        {
                            allSprites.ToList().ForEach(x =>
                            {
                                x.scale = new Vector3(x.scale.x * scale, x.scale.y * scale, 1.0f);
                                if (x.boxCollider2D != null)
                                {
                                    Vector2 b = x.boxCollider2D.size;
                                    b.x *= scale;
                                    b.y *= scale;
                                    x.boxCollider2D.size = b;
                                }
                            });
                        }

                        var allAnims = newS.GetComponentsInChildren<tk2dAnimatedSprite>(true);
                        if(allAnims != null)
                        {
                            allAnims.ToList().ForEach(x =>
                            {
                                x.scale = new Vector3(x.scale.x * scale, x.scale.y * scale, 1.0f);

                                if (x.boxCollider2D != null)
                                {
                                    Vector2 b = x.boxCollider2D.size;
                                    b.x *= scale;
                                    b.y *= scale;
                                    x.boxCollider2D.size = b;
                                }
                            });
                        }

                        //newS.scale = new Vector3(newS.scale.x * scale, newS.scale.y * scale, 1.0f);
                        //Vector2 b = newS.boxCollider2D.size;
                        //b.x *= scale;
                        //b.y *= scale;
                        //newS.boxCollider2D.size = b;
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
                    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * scale,
                        gameObject.transform.localScale.y * scale, 1f);
                }
            }
            //no two colliders to compare? then do old logic (until it's removed....)
            else
            {
                if (dataOfEnemyToReplace != null)
                {
                    if (EnemyDefinition.Data.isLargeEnemy && !dataOfEnemyToReplace.isLargeEnemy)
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale * .5f;
                    }
                    else if (!EnemyDefinition.Data.isLargeEnemy && dataOfEnemyToReplace.isLargeEnemy)
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale * 1.5f;
                    }

                    if (EnemyDefinition.Data.name.Contains("Mawlek Turret") && !dataOfEnemyToReplace.isLargeEnemy)
                    {
                        gameObject.transform.localScale = originalNewEnemyScale * .5f;
                    }

                    //shrink him!
                    if (EnemyDefinition.Data.name.Contains("Mantis Traitor Lord") && !dataOfEnemyToReplace.isLargeEnemy)
                    {
                        gameObject.transform.localScale = gameObject.transform.localScale * .4f;
                    }
                }
                else
                {
                    //just always shrink these things a bit...
                    if (EnemyDefinition.Data.name.Contains("Mawlek Turret"))
                    {
                        gameObject.transform.localScale = originalNewEnemyScale * .6f;
                    }
                }
            }
        }

        public virtual void RotateNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            if (enemyToReplace != null)
            {
                if (!EnemyDefinition.Data.name.Contains("Ceiling Dropper"))
                    gameObject.transform.rotation = enemyToReplace.transform.rotation;

                //if they were a wall flying mantis, don't rotate the replacement
                if (enemyToReplace.name.Contains("Mantis Flyer Child"))
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }

                if (EnemyDefinition.Data.name.Contains("Mawlek Turret"))
                {
                    //Quaternion rotate = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                    //newEnemy.transform.rotation = rotate * enemyToReplace.transform.rotation;
                }

                if (EnemyDefinition.Data.name.Contains("Mawlek Turret Ceiling"))
                {
                    //Quaternion rotate = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                    //gameObject.transform.rotation = rotate * enemyToReplace.transform.rotation;
                }

                if (enemyToReplace.name.Contains("Plant Turret"))
                {
                    //Quaternion rotate = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                    //gameObject.transform.rotation = rotate * enemyToReplace.transform.rotation;
                }

                //mosquitos rotate, so spawn replacements with default rotation
                if (enemyToReplace.name.Contains("Mosquito"))
                {
                    gameObject.transform.rotation = Quaternion.identity;
                }

                //TODO: look up what's going on with this
                if (EnemyDefinition.Data.name.Contains("Crystallised Lazer Bug") && enemyToReplace != null)
                {
                    //Dev.Log("Old rotation = " + newEnemy.transform.rotation.eulerAngles);
                    //Quaternion rotate = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                    //gameObject.transform.rotation = rotate * enemyToReplace.transform.rotation;
                    //Dev.Log("New rotation = " + newEnemy.transform.rotation.eulerAngles);
                    //newEnemy.PrintSceneHierarchyTree(true);
                }

                if (EnemyDefinition.Data.name.Contains("Mines Crawler"))
                {
                    //Quaternion rot180degrees = Quaternion.Euler(-enemyToReplace.transform.rotation.eulerAngles);
                    //gameObject.transform.rotation = rot180degrees * enemyToReplace.transform.rotation;
                    //gameObject.transform.rotation = Quaternion.identity;
                }

                //TODO: test me
                if (EnemyDefinition.Data.name.Contains("Abyss Crawler"))
                {
                    //Quaternion rot180degrees = Quaternion.Euler(-enemyToReplace.transform.rotation.eulerAngles);
                    //gameObject.transform.rotation = rot180degrees * enemyToReplace.transform.rotation;
                    //gameObject.transform.rotation = Quaternion.identity;
                }
                //if( oldEnemy.name.Contains( "Moss Walker" ) )
                //{
                //    //Quaternion rot180degrees = Quaternion.Euler(-oldEnemy.transform.rotation.eulerAngles);
                //    //newEnemy.transform.rotation = rot180degrees * oldEnemy.transform.rotation;
                //    newEnemy.transform.rotation = Quaternion.identity;
                //}
            }
        }

        public virtual void PositionNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            if (enemyToReplace == null)
                return;

            float rotation = enemyToReplace.transform.localEulerAngles.z;
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
            gameObject.transform.position = enemyToReplace.transform.position;

            BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
            Vector2 colliderSize = collider == null ? Vector2.one : collider.size;
            Vector2 scale = gameObject.transform.localScale;
            Vector2 originalPosition = gameObject.transform.position;


            Vector2 originalDown = -originalUp;
            float projectionDistance = 500f;
            Vector3 toSurface = Mathnv.GetNearestVectorTo(originalPosition, projectionDistance, EnemyRandomizer.IsSurfaceOrPlatform);
            Vector2 toSurfaceDir = toSurface.normalized;
            Vector2 toSurfaceUp = -toSurfaceDir;

            Vector3 positionOnSurface = Mathnv.GetNearestPointOn(originalPosition, projectionDistance, EnemyRandomizer.IsSurfaceOrPlatform);
            Vector3 positionOffset = Vector3.zero;

            if (EnemyDefinition.Data.name.Contains("Mantis Flyer Child"))
            {
                positionOffset = new Vector3(colliderSize.x * originalUp.x * scale.x, colliderSize.y * originalUp.y * scale.y, 0f);
            }
            //project the ceiling droppers onto the ceiling
            if (EnemyDefinition.Data.name.Contains("Ceiling Dropper"))
            {
                positionOnSurface = Mathnv.GetPointOn(originalPosition, Vector2.up, projectionDistance, EnemyRandomizer.IsSurfaceOrPlatform);
                //move it down a bit, keeps spawning in roof
                positionOffset = Vector3.down * 2f * scale.y;
            }
            else if (!EnemyDefinition.IsFlyer)
            {
                if(Mathf.Approximately(0f, rotation))
                {
                    positionOnSurface = gameObject.GetPointOn(Vector2.down, projectionDistance);

                    if (EnemyDefinition.Data.name.Contains("Lobster"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * 2f) * scale.y;
                    }
                    if (EnemyDefinition.Data.name.Contains("Blocker"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                    }
                    if (EnemyDefinition.Data.name == ("Moss Knight"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                    }
                    if (EnemyDefinition.Data.name == ("Enemy"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -0.5f) * scale.y;
                    }
                }
                else
                {
                    positionOnSurface = gameObject.GetPointOn(toSurfaceDir, projectionDistance);
                }

                positionOffset = new Vector3(colliderSize.x * originalUp.x * scale.x, colliderSize.y * originalUp.y * scale.y, 0f);

                if (enemyToReplace != null && enemyToReplace.name.Contains("Moss Walker"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * scale.y;
                }
                if (EnemyDefinition.Data.name.Contains("Plant Trap"))
                {
                    positionOffset = toSurfaceUp * 2f * scale.y;
                }
                if (collider != null && EnemyDefinition.Data.name.Contains("Mawlek Turret"))
                {
                    positionOffset = toSurfaceUp * collider.size.y / 3f * scale.y;
                }
                if (collider != null && EnemyDefinition.Data.name.Contains("Mushroom Turret"))
                {
                    positionOffset = (toSurfaceUp * .5f) * scale.y;
                }
                if (EnemyDefinition.Data.name.Contains("Plant Turret"))
                {
                    positionOffset = toSurfaceUp * .7f * scale.y;
                }
                if (collider != null && EnemyDefinition.Data.name.Contains("Laser Turret"))
                {
                    positionOffset = toSurfaceUp * collider.size.y / 10f * scale.y;
                }
                if (collider != null && EnemyDefinition.Data.name.Contains("Worm"))
                {
                    positionOffset = toSurfaceUp * collider.size.y / 3f * scale.y;
                }
                if (EnemyDefinition.Data.name.Contains("Crystallised Lazer Bug"))
                {
                    //suppposedly 1/2 their Y collider space offset should be 1.25
                    //but whatever we set it at, they spawn pretty broken, so spawn them out of the ground a bit so they're still a threat
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y;
                }
                if (EnemyDefinition.Data.name.Contains("Mines Crawler"))
                {
                    positionOffset = toSurfaceUp * 1.5f * scale.y;
                }
                if (EnemyDefinition.Data.name.Contains("Spider Mini"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y; ;
                }
                if (EnemyDefinition.Data.name.Contains("Abyss Crawler"))
                {
                    positionOffset = toSurfaceUp * collider.size.y * 1.5f * scale.y; ;
                }
                if (EnemyDefinition.Data.name.Contains("Climber"))
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

        public virtual void ModifyNewEnemyGeo(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            if (EnemyRandomizer.GlobalSettings.RandomizeGeo)
            {
                int smallGeo = EnemyRandomizer.pRNG.Rand(0, Mathf.FloorToInt(EnemyDefinition.Difficulty * .5f));
                int medGeo = EnemyRandomizer.pRNG.Rand(0, Mathf.FloorToInt(EnemyDefinition.Difficulty * .25f));
                int bigGeo = EnemyRandomizer.pRNG.Rand(0, Mathf.FloorToInt(EnemyDefinition.Difficulty * .1f));

                this.SetEnemyGeoRates(smallGeo, medGeo, bigGeo);
            }
            else
            {
                //don't mess with geo rates then.. but add some to these guys
                if (EnemyDefinition.Data.name == "Bursting Zombie")
                {
                    int smallGeo = EnemyRandomizer.pRNG.Rand(0, 5);
                    int medGeo = EnemyRandomizer.pRNG.Rand(1, 2);
                    int bigGeo = EnemyRandomizer.pRNG.Rand(0, 1);

                    this.SetEnemyGeoRates(smallGeo, medGeo, bigGeo);
                }

                if (EnemyDefinition.Data.name == "Lazy Flyer Enemy")
                {
                    int smallGeo = 0;
                    int medGeo = 0;
                    int bigGeo = EnemyRandomizer.pRNG.Rand(0, 10);

                    this.SetEnemyGeoRates(smallGeo, medGeo, bigGeo);
                }
            }
        }

        public virtual void NameNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            //save this here to allow for easier debugging
            InstanceDefinitionName = EnemyDefinition.Data.name;

            //each enemy in a scene that wants to reigster with the PersistentBoolItem's state in the game manager needs to have
            //a unique game object name.... we want to be able to access this state when a scene reloads, so we can't use
            //our new enemy name and in-fact need to use the original enemy name to ensure that when re-loading a scene an enemy
            //remains defeated until benching.
            if (enemyToReplace != null)
            {
                Instance.name = enemyToReplace.name;
            }
            else
            {
                Instance.name = EnemyDefinition.Data.name;
            }
        }

        public virtual void FinalizeNewEnemy(GameObject enemyToReplace = null, EnemyData dataOfEnemyToReplace = null)
        {
            if (enemyToReplace != null)
            {
                try
                {
                    //configure persistant bool items -- used to keep enemies dead until bench reset
                    {
                        PersistentBoolItem old_pbi = enemyToReplace.GetComponent<PersistentBoolItem>();
                        PersistentBoolItem new_pbi = gameObject.GetComponent<PersistentBoolItem>();

                        if (old_pbi != null)
                        {
                            if (new_pbi == null)
                            {
                                new_pbi = gameObject.AddComponent<PersistentBoolItem>();
                            }
                        }
                        else if (old_pbi == null)
                        {
                            if (new_pbi != null)
                            {
                                new_pbi = gameObject.GetComponent<PersistentBoolItem>();
                                GameObject.Destroy(new_pbi);
                                new_pbi = null;
                            }
                        }

                        if (old_pbi != null && new_pbi != null)
                        {
                            new_pbi.semiPersistent = old_pbi.semiPersistent;
                        }
                    }

                    //DeactivateIfPlayerdataTrue
                    {
                        DeactivateIfPlayerdataTrue old_DIPDT = enemyToReplace.GetComponent<DeactivateIfPlayerdataTrue>();
                        DeactivateIfPlayerdataTrue new_DIPDT = gameObject.GetComponent<DeactivateIfPlayerdataTrue>();
                        if (old_DIPDT != null)
                        {
                            if (new_DIPDT == null)
                            {
                                new_DIPDT = gameObject.AddComponent<DeactivateIfPlayerdataTrue>();
                            }
                        }
                        else if (old_DIPDT == null)
                        {
                            if (new_DIPDT != null)
                            {
                                new_DIPDT = gameObject.GetComponent<DeactivateIfPlayerdataTrue>();
                                GameObject.Destroy(new_DIPDT);
                                new_DIPDT = null;
                            }
                        }

                        if (old_DIPDT != null && new_DIPDT != null)
                        {
                            new_DIPDT.boolName = old_DIPDT.boolName;
                        }
                    }

                    //ActivateIfPlayerdataTrue
                    {
                        ActivateIfPlayerdataTrue old_PDComponent = enemyToReplace.GetComponent<ActivateIfPlayerdataTrue>();
                        ActivateIfPlayerdataTrue new_PDComponent = gameObject.GetComponent<ActivateIfPlayerdataTrue>();
                        if (old_PDComponent != null)
                        {
                            if (new_PDComponent == null)
                            {
                                new_PDComponent = gameObject.AddComponent<ActivateIfPlayerdataTrue>();
                            }
                        }
                        else if (old_PDComponent == null)
                        {
                            if (new_PDComponent != null)
                            {
                                new_PDComponent = gameObject.GetComponent<ActivateIfPlayerdataTrue>();
                                GameObject.Destroy(new_PDComponent);
                                new_PDComponent = null;
                            }
                        }

                        if (old_PDComponent != null && new_PDComponent != null)
                        {
                            new_PDComponent.boolName = old_PDComponent.boolName;
                        }
                    }

                    //DeactivateIfPlayerdataFalseDelayed
                    {
                        DeactivateIfPlayerdataFalseDelayed old_PDComponent = enemyToReplace.GetComponent<DeactivateIfPlayerdataFalseDelayed>();
                        DeactivateIfPlayerdataFalseDelayed new_PDComponent = gameObject.GetComponent<DeactivateIfPlayerdataFalseDelayed>();
                        if (old_PDComponent != null)
                        {
                            if (new_PDComponent == null)
                            {
                                new_PDComponent = gameObject.AddComponent<DeactivateIfPlayerdataFalseDelayed>();
                            }
                        }
                        else if (old_PDComponent == null)
                        {
                            if (new_PDComponent != null)
                            {
                                new_PDComponent = gameObject.GetComponent<DeactivateIfPlayerdataFalseDelayed>();
                                GameObject.Destroy(new_PDComponent);
                                new_PDComponent = null;
                            }
                        }

                        if (old_PDComponent != null && new_PDComponent != null)
                        {
                            new_PDComponent.boolName = old_PDComponent.boolName;
                            new_PDComponent.delay = old_PDComponent.delay;
                        }
                    }

                    //DeactivateIfPlayerdataFalse
                    {
                        DeactivateIfPlayerdataFalse old_PDComponent = enemyToReplace.GetComponent<DeactivateIfPlayerdataFalse>();
                        DeactivateIfPlayerdataFalse new_PDComponent = gameObject.GetComponent<DeactivateIfPlayerdataFalse>();
                        if (old_PDComponent != null)
                        {
                            if (new_PDComponent == null)
                            {
                                new_PDComponent = gameObject.AddComponent<DeactivateIfPlayerdataFalse>();
                            }
                        }
                        else if (old_PDComponent == null)
                        {
                            if (new_PDComponent != null)
                            {
                                new_PDComponent = gameObject.GetComponent<DeactivateIfPlayerdataFalse>();
                                GameObject.Destroy(new_PDComponent);
                                new_PDComponent = null;
                            }
                        }

                        if (old_PDComponent != null && new_PDComponent != null)
                        {
                            new_PDComponent.boolName = old_PDComponent.boolName;
                        }
                    }

                    //SetPosIfPlayerdataBool
                    {
                        SetPosIfPlayerdataBool old_PDComponent = enemyToReplace.GetComponent<SetPosIfPlayerdataBool>();
                        SetPosIfPlayerdataBool new_PDComponent = gameObject.GetComponent<SetPosIfPlayerdataBool>();
                        if (old_PDComponent != null)
                        {
                            if (new_PDComponent == null)
                            {
                                new_PDComponent = gameObject.AddComponent<SetPosIfPlayerdataBool>();
                            }
                        }
                        else if (old_PDComponent == null)
                        {
                            if (new_PDComponent != null)
                            {
                                new_PDComponent = gameObject.GetComponent<SetPosIfPlayerdataBool>();
                                GameObject.Destroy(new_PDComponent);
                                new_PDComponent = null;
                            }
                        }

                        if (old_PDComponent != null && new_PDComponent != null)
                        {
                            new_PDComponent.playerDataBool = old_PDComponent.playerDataBool;
                            new_PDComponent.onceOnly = old_PDComponent.onceOnly;
                            new_PDComponent.setX = old_PDComponent.setX;
                            new_PDComponent.setY = old_PDComponent.setY;
                        }
                    }
                }
                catch (Exception e)
                {
                    Dev.LogError("Failed finalize new instance of enemy: " + EnemyDefinition.Data.name);
                    Dev.LogError("Error: " + e.Message + " Stacktrace: " + e.StackTrace);
                }


                //hook up the old enemy's battle scene to the new enemy replacement
                HealthManager oldHM = enemyToReplace.GetComponent<HealthManager>();
                if (oldHM != null)
                {
                    this.SetBattleScene(oldHM.GetBattleScene());
                }
            }
            else
            {
                //not replacing, so just delete this
                PersistentBoolItem new_pbi = gameObject.GetComponent<PersistentBoolItem>();
                if (new_pbi != null)
                    GameObject.Destroy(new_pbi);

                //remove this, because it can deactivate some enemies....
                DeactivateIfPlayerdataTrue toRemove = gameObject.GetComponent<DeactivateIfPlayerdataTrue>();
                if (toRemove != null)
                    GameObject.Destroy(toRemove);
            }

            // Checks if enemy has a roar, then disable the roar by skipping over the roar state
            // Fixes the roar push out of bounds on room entry issue. Workaround until I work out how to disable the stun/push
            PlayMakerFSM control = FSMUtility.GetFSM(gameObject);
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
    }
}
