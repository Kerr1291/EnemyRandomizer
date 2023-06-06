using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Satchel;
using HutongGames.PlayMaker.Actions;
using UniRx;
using System.Reflection;
using System.Collections;
using HutongGames.PlayMaker;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public static class SpawnerExtensions
    {
        static ReadOnlyReactiveProperty<List<GameObject>> blackBorders { get; set; }
        static ReadOnlyReactiveProperty<List<GameObject>> nonNullBB { get; set; }
        static ReadOnlyReactiveProperty<float> bb_xmin { get; set; }// = float.MaxValue;
        static ReadOnlyReactiveProperty<float> bb_xmax { get; set; }// = float.MinValue;
        static ReadOnlyReactiveProperty<float> bb_ymin { get; set; }// = float.MaxValue;
        static ReadOnlyReactiveProperty<float> bb_ymax { get; set; }// = float.MinValue;

        public static void SetupBoundsReactives()
        {
            SetupBB();
            SetupBounds();
        }

        static void SetupBB()
        {
            if (EnemyRandomizerDatabase.GetBlackBorders == null)
            {
                Dev.LogError("GetBlackBorders hasn't been setup set (This really should never happen)... Fatal error (might crash?)");
            }

            if (blackBorders == null)
            {
                blackBorders = EnemyRandomizerDatabase.GetBlackBorders().ToReadOnlyReactiveProperty();
            }
            if (nonNullBB == null)
            {
                nonNullBB = blackBorders.Select(bbs => (bbs != null && bbs.Count > 0) ? bbs : null).Where(x => x != null).ToReadOnlyReactiveProperty();
            }
        }

        static void SetupBounds()
        {
            if (bb_xmin == null)
            {
                bb_xmin = nonNullBB.Select(x => x.Where(z => Mathf.FloorToInt(z.transform.localScale.x) == 20).Min(o => (o.transform.position.x - 10f))).ToReadOnlyReactiveProperty();
                bb_xmax = nonNullBB.Select(x => x.Where(z => Mathf.FloorToInt(z.transform.localScale.x) == 20).Max(o => (o.transform.position.x + 10f))).ToReadOnlyReactiveProperty();
                bb_ymin = nonNullBB.Select(x => x.Where(z => Mathf.FloorToInt(z.transform.localScale.y) == 20).Min(o => (o.transform.position.y - 10f))).ToReadOnlyReactiveProperty();
                bb_ymax = nonNullBB.Select(x => x.Where(z => Mathf.FloorToInt(z.transform.localScale.y) == 20).Max(o => (o.transform.position.y + 10f))).ToReadOnlyReactiveProperty();
            }
        }

        public static string ObjectName(this GameObject gameObject)
        {
            if (ObjectMetadata.Get(gameObject) != null)
                return ObjectMetadata.Get(gameObject).ObjectName;

            return gameObject == null ? null : gameObject.name;
        }

        public static string SceneName(this GameObject gameObject)
        {
            if (ObjectMetadata.Get(gameObject) != null)
                return ObjectMetadata.Get(gameObject).SceneName;

            return gameObject == null || !gameObject.scene.IsValid() ? null : gameObject.scene.name;
        }

        public static HealthManager GetEnemyHealthManager(this GameObject source)
        {
            return source == null ? null : source.GetComponent<HealthManager>();
        }

        public static tk2dSprite GetSprite(this GameObject source)
        {
            return source == null ? null : source.GetComponent<tk2dSprite>();
        }

        public static tk2dSpriteAnimator GetAnimator(this GameObject source)
        {
            return source == null ? null : source.GetComponent<tk2dSpriteAnimator>();
        }

        public static DamageHero GetHeroDamage(this GameObject source)
        {
            return source == null ? null : source.GetComponent<DamageHero>();
        }

        public static DamageEnemies GetEnemyDamage(this GameObject source)
        {
            return source == null ? null : source.GetComponent<DamageEnemies>();
        }

        public static Walker GetWalker(this GameObject source)
        {
            return source == null ? null : source.GetComponent<Walker>();
        }

        public static TinkEffect GetTinker(this GameObject source)
        {
            return source == null ? null : source.GetComponent<TinkEffect>();
        }

        public static EnemyDeathEffects GetDeathEffects(this GameObject source)
        {
            return source == null ? null : source.GetComponent<EnemyDeathEffects>();
        }

        public static Rigidbody2D GetPhysicsBody(this GameObject source)
        {
            return source == null ? null : source.GetComponent<Rigidbody2D>();
        }

        public static Collider2D GetCollider(this GameObject source)
        {
            return source == null ? null : source.GetComponent<Collider2D>();
        }

        public static MeshRenderer GetMRenderer(this GameObject source)
        {
            return source == null ? null : source.GetComponent<MeshRenderer>();
        }

        public static PreInstantiateGameObject GetPreInstantiatedGameObject(this GameObject source)
        {
            return source == null ? null : source.GetComponent<PreInstantiateGameObject>();
        }

        public static bool IsWalker(this GameObject source)
        {
            return source == null ? false : (source.GetComponent<Walker>() != null);
        }

        public static bool IsFlyingFromComponents(this GameObject source)
        {
            return source == null ? false : (source.GetComponent<Walker>() == null && source.GetComponent<Climber>() == null && source.GetComponent<Rigidbody2D>() != null && source.GetComponent<Rigidbody2D>().gravityScale == 0);
        }

        public static bool IsCrawlingFromComponents(this GameObject source)
        {
            return source == null ? false : (source.GetComponent<Crawler>() != null);
        }

        public static bool IsClimbingFromComponents(this GameObject source)
        {
            return source == null ? false : (source.GetComponent<Climber>() != null);
        }

        public static bool IsSurfaceOrPlatform(this GameObject gameObject)
        {
            //First process skips or exclusions
            List<string> groundOrPlatformName = new List<string>()
            {
                "Chunk",
                "Platform",
                "plat_",
                "Roof",
                "Colosseum Wall"
            };

            return groundOrPlatformName.Any(x => gameObject.name.Contains(x));
        }

        static GameObject GetCorpse<T>(this GameObject prefab)
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

        static GameObject GetCorpsePrefab<T>(this GameObject prefab)
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

        public static void SetParentToOthersParent(this GameObject gameObject, GameObject other)
        {
            if (other == null)
                return;

            gameObject.transform.SetParent(other.transform.parent);
        }

        public static bool IsValid(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            if (!gameObject.IsInAValidScene() && !gameObject.IsInALoadingScene())
                return false;

            if (gameObject.IsInvalidSceneObject())
                return false;

            if (!gameObject.IsDatabaseObject())
                return false;

            if (gameObject.HasReplacedAnObject())
                return false;

            if (gameObject.IsDisabledBySavedGameState())
                return false;

            return true;
        }

        public static GameObject GetAvailableItem(this GameObject gameObject)
        {
            var c = GetCorpseObject(gameObject);
            if (c != null)
            {
                var cpigo = c.GetComponent<PreInstantiateGameObject>();
                if (cpigo != null)
                {
                    if (cpigo.InstantiatedGameObject != null)
                    {
                        if (cpigo.name.Contains("Shiny Item") && cpigo.GetComponent<PersistentBoolItem>() != null)
                        {
                            var pbi = cpigo.GetComponent<PersistentBoolItem>();
                            if (!pbi.persistentBoolData.activated)
                            {
                                return pbi.gameObject;
                            }
                        }
                    }
                }
            }
            return null;
        }

        //use this when the other object has a smasher
        public static GameObject CopyAndAddSmasher(this GameObject gameObject, GameObject smasherObject)
        {
            if (smasherObject == null)
                return null;

            if (!smasherObject.IsSmasher())
                return null;

            var smasher = smasherObject.FindGameObjectInDirectChildren("Smasher");
            var copy = GameObject.Instantiate(smasher, gameObject.transform);
            copy.SetActive(false);
            return copy;
        }

        public static bool StickToClosestSurface(this GameObject gameObject, float maxRange = 100f, bool alsoStickCorpse = true)
        {
            if (CheckIfIsPogoLogicType(gameObject))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetNearestSurfaceX(gameObject, maxRange);
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
            if (CheckIfIsPogoLogicType(gameObject))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetNearestSurfaceX(gameObject, maxRange);
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
            if (CheckIfIsPogoLogicType(gameObject))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetNearestSurfaceX(gameObject, maxRange);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);

            return true;
        }

        public static RaycastHit2D GetGroundRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999,-9999) };

            RaycastHit2D closest = GetGroundX(gameObject);
            return closest;
        }

        public static RaycastHit2D GetRoofRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999, -9999) };

            RaycastHit2D closest = GetRoofX(gameObject);
            return closest;
        }

        public static RaycastHit2D GetLeftRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999, -9999) };

            RaycastHit2D closest = GetLeftX(gameObject);
            return closest;
        }

        public static RaycastHit2D GetRightRay(this GameObject gameObject)
        {
            if (gameObject.GetComponent<Collider2D>() == null)
                return new RaycastHit2D() { point = new Vector2(-9999, -9999) };

            RaycastHit2D closest = GetRightX(gameObject);
            return closest;
        }


        public static bool StickToGround(this GameObject gameObject, float extraOffsetScale = 0.53f)
        {
            if (CheckIfIsPogoLogicType(gameObject))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            if (MetaDataTypes.InGroundEnemy.Contains(EnemyRandomizerDatabase.ToDatabaseKey(gameObject.name)))
            {
                gameObject.transform.position = gameObject.transform.position.ToVec2() + GetUpFromSelfAngle(gameObject.transform.localEulerAngles.z, false) * 2f;
            }

            RaycastHit2D closest = GetGroundX(gameObject);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, false);

            return true;
        }

        public static Vector2 GetUpFromSelfAngle(float selfAngle, bool isFlipped)
        {
            Vector2 up = Vector2.zero;

            float angle = selfAngle % 360f;
            if (!isFlipped)
            {
                angle = (angle + 180f) % 360f;
            }

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
            }

            return up;
        }

        public static Vector2 ToVec2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static bool StickToGroundX(this GameObject gameObject, float extraOffsetScale = 0.53f)
        {
            if (CheckIfIsPogoLogicType(gameObject))
                return false;

            bool isPlantTrap = false;
            if (gameObject.GetComponent<Collider2D>() == null)
            {
                isPlantTrap = gameObject.GetDatabaseKey() == "Plant Trap";

                if(!isPlantTrap)
                    return false;
            }

            if(isPlantTrap)
            {
                var plantPos = GetRayOn(gameObject.transform.position.ToVec2() + Vector2.up, Vector2.down, float.MaxValue);
                gameObject.transform.position = plantPos.point;
                SetRotationToRayCollisionNormal(gameObject, plantPos, false);
                return true;
            }

            RaycastHit2D closest = GetRayOn(gameObject.transform.position.ToVec2() + Vector2.up, Vector2.down, float.MaxValue);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
            float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, false);

            return true;
        }

        //public static bool StickToRoof(this GameObject gameObject, float extraOffsetScale = 0.33f, bool flipped = false)
        //{
        //    if (CheckIfIsPogoLogicType(gameObject))
        //        return false;

        //    if (gameObject.GetComponent<Collider2D>() == null)
        //        return false;

        //    RaycastHit2D closest = GetRoofX(gameObject);
        //    SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);
        //    float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, flipped);

        //    return true;
        //}


        public static bool StickToRoof(this GameObject gameObject, float extraOffsetScale = 0.33f, bool flipped = false, bool rotateToPlaceOnRoof = true)
        {
            if (CheckIfIsPogoLogicType(gameObject))
                return false;

            if (gameObject.GetComponent<Collider2D>() == null)
                return false;

            RaycastHit2D closest = GetRoofX(gameObject);
            SetPositionToRayCollisionPoint(gameObject, closest, extraOffsetScale);

            if (rotateToPlaceOnRoof)
            {
                float newAngle = SetRotationToRayCollisionNormal(gameObject, closest, flipped);
            }

            return true;
        }

        public static CorpseOrientationFixer AddCorpseOrientationFixer(float newAngle, GameObject corpse)
        {
            var fixer = corpse.gameObject.GetOrAddComponent<CorpseOrientationFixer>();
            fixer.corpseAngle = newAngle;
            return fixer;
        }

        public static CorpseRemover AddCorpseRemoverWithEffect(this GameObject corpse, GameObject owner, string effect = null)
        {
            var remover = corpse.gameObject.GetOrAddComponent<CorpseRemover>();
            if (!string.IsNullOrEmpty(effect))
            {
                remover.effectToSpawn = effect;
            }
            //remover.owner = ObjectMetadata.Get(owner);
            return remover;
        }

        public static void AddDieOnHPZeroToState(this FsmState state, HealthManager healthManager, string effectToSpawn = null)
        {
            if (state == null)
                return;

            if (healthManager == null)
                return;

            state.InsertCustomAction(() => {
                if (healthManager.hp <= 0)
                {
                    Dev.Log($"Destroying {healthManager.gameObject} because HP hit zero in state {state.Name}");
                    if(!string.IsNullOrEmpty(effectToSpawn))
                        EnemyRandomizerDatabase.CustomSpawnWithLogic(healthManager.transform.position, effectToSpawn, null, true);
                    GameObject.Destroy(healthManager.gameObject);
                }
            }, 0);
        }

        public static void PlayIdleAnimation(this GameObject source)
        {
            var anim = source.GetComponent<tk2dSpriteAnimator>();
            if (anim == null)
                return;

            bool playedIdle = false;
            if (source.IsFlying())
            {
                try
                {
                    //test this
                    anim.Play("Fly");
                    playedIdle = true;
                }
                catch
                {

                }
            }

            if (!playedIdle)
            {
                anim.Play(anim.DefaultClip);
                playedIdle = true;
            }
        }

        public static void LockIntoPosition(this GameObject source, Vector3 pos)
        {
            var posLock = source.GetOrAddComponent<PositionLocker>();
            posLock.positionLock = pos;
        }

        public static void StripMovements(this GameObject source, bool makeStatic)
        {
            if (source == null)
                return;

            var fsms = source.GetComponents<PlayMakerFSM>();
            fsms.ToList().ForEach(x => GameObject.Destroy(x));

            var actives = source.GetComponents<FSMActivator>();
            actives.ToList().ForEach(x => GameObject.Destroy(x));

            {
                var p = source.GetComponents<PlayMakerCollisionEnter>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }
            {
                var p = source.GetComponents<PlayMakerFixedUpdate>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }
            {
                var p = source.GetComponents<PlayMakerCollisionExit>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }
            {
                var p = source.GetComponents<PlayMakerCollectionProxy>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }

            {
                var p = source.GetComponents<PreventOutOfBounds>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }

            {
                var p = source.GetComponents<PreventInsideWalls>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }

            if (makeStatic)
            {
                var body = source.GetOrAddComponent<Rigidbody2D>();
                body.isKinematic = true;
                body.gravityScale = 0f;
            }
        }

        public static Vector2 GetOriginalObjectSize(string objectName, bool checkSpriteColliderLast = false)
        {
            string DatabaseName = EnemyRandomizerDatabase.ToDatabaseKey(objectName);
            if (string.IsNullOrEmpty(DatabaseName))
                return Vector2.one;

            if (MetaDataTypes.HasUniqueSizeEnemies.Contains(DatabaseName))
            {
                return GetSizeFromUniqueObject(objectName);
            }
            else
            {
                return GetSizeFromComponents(objectName, checkSpriteColliderLast);
            }
        }

        public static Vector2 GetOriginalObjectSize(this GameObject source, bool checkSpriteColliderLast = false)
        {
            string DatabaseName = EnemyRandomizerDatabase.GetDatabaseKey(source);
            if (string.IsNullOrEmpty(DatabaseName))
                return Vector2.one;

            if (MetaDataTypes.HasUniqueSizeEnemies.Contains(DatabaseName))
            {
                return GetSizeFromUniqueObject(source);
            }
            else
            {
                return GetSizeFromComponents(source, checkSpriteColliderLast);
            }
        }

        public static void MakeSmasher(this GameObject source, GameObject smasherObject)
        {
            var smasher = source.CopyAndAddSmasher(smasherObject);
            var col = smasher.GetComponent<BoxCollider2D>();
            var size = GetOriginalObjectSize(smasherObject);
            col.size = size * 1.2f;

            smasher.transform.localPosition = Vector3.zero;
            smasher.SetActive(true);
        }

        public static void MakeTinker(this GameObject source, bool makeInvincible, bool makeSpellVulnerable)
        {
            var tinkEffect = source.GetOrAddComponent<TinkEffect>();
            var blockHitEffect = EnemyRandomizerDatabase.GetDatabase().Spawn(EnemyRandomizerDatabase.BlockHitEffectName);

            if (blockHitEffect == null)
            {
                Dev.LogError($"Failed to make {source} into a tinker using effect name {EnemyRandomizerDatabase.BlockHitEffectName}");
                return;
            }

            tinkEffect.blockEffect = blockHitEffect;
            if (makeInvincible)
            {
                var enemyHealthManager = source.GetEnemyHealthManager();
                if (enemyHealthManager != null)
                {
                    enemyHealthManager.IsInvincible = true;
                }
            }

            if (makeSpellVulnerable)
            {
                var extraDamage = source.GetOrAddComponent<ExtraDamageable>();
                var isSpellVulnerableField = extraDamage.GetType().GetField("isSpellVulnerable", BindingFlags.NonPublic | BindingFlags.Instance);
                isSpellVulnerableField?.SetValue(extraDamage, true);
            }
        }

        public static bool RenderersVisible(this GameObject gameObject)
        {
            var col = gameObject.GetComponent<Collider2D>();
            var mr = gameObject.GetComponent<MeshRenderer>();
            if (col != null || mr != null)
            {
                if (col != null && mr == null)
                    return col.enabled;
                else if (col == null && mr != null)
                    return mr.enabled;
                else //if (collider != null && renderer != null)
                    return col.enabled && mr.enabled;
            }

            return false;
        }

        public static bool InBounds(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            return InBounds(gameObject.transform.position);
        }

        public static bool InBounds(Vector2 pos)
        {
            if (bb_xmin == null)
                return false;

            if (bb_xmax == null)
                return false;

            if (bb_ymin == null)
                return false;

            if (bb_ymax == null)
                return false;

            if (pos.x < bb_xmin.Value)
                return false;
            else if (pos.x > bb_xmax.Value)
                return false;
            else if (pos.y < bb_ymin.Value)
                return false;
            else if (pos.y > bb_ymax.Value)
                return false;

            return true;
        }

        public static bool IsVisible(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            if (gameObject.ObjectType() == PrefabObject.PrefabType.Effect)
                return true;

            return gameObject != null && gameObject.activeInHierarchy && gameObject.RenderersVisible() && gameObject.InBounds();
        }

        public static bool IsBattleEnemy(this GameObject gameObject)
        {
            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
            {
                return false;
            }

            var originalObject = ObjectMetadata.GetOriginal(gameObject);
            if (originalObject != null)
            {
                return IsBattleEnemy(originalObject.ScenePath, originalObject.SceneName);
            }

            string scenePath = gameObject.GetSceneHierarchyPath();
            string sceneName = gameObject.scene.IsValid() ? null : gameObject.scene.name;
            
            return IsBattleEnemy(scenePath, sceneName);
        }

        public static bool IsBattleEnemy(string scenePath, string sceneName = null)
        {
            var splitPath = scenePath.Split('/');

            string databaseName = EnemyRandomizerDatabase.ToDatabaseKey(splitPath.LastOrDefault());

            if (splitPath.Any(x => BattleManager.battleControllers.Any(y => x.Contains(y))))
            {
                return true;
            }

            if (string.IsNullOrEmpty(sceneName))
                return false;

            if (sceneName.Contains("Room_Colosseum"))
                return true;

            if (MetaDataTypes.BattleEnemies.ContainsKey(sceneName))
            {
                bool sceneHasBattleEnemies = MetaDataTypes.BattleEnemies.TryGetValue(sceneName, out var currentSceneBattleEnemies);

                if (sceneHasBattleEnemies)
                {
                    if (currentSceneBattleEnemies.Contains(databaseName))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (MetaDataTypes.BattleEnemies.TryGetValue("ANY", out var anyBattleEnemies))
                {
                    if (anyBattleEnemies.Contains(databaseName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsInvalidSceneObject(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            var originalObject = ObjectMetadata.GetOriginal(gameObject);
            if (originalObject != null)
            {
                return IsInvalidSceneObject(originalObject.ScenePath);
            }

            string scenePath = gameObject.GetSceneHierarchyPath();
            return IsInvalidSceneObject(scenePath);
        }

        public static bool IsInvalidSceneObject(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                return false;

            return string.IsNullOrEmpty(scenePath) ? true : MetaDataTypes.AlwaysDeleteObject.Any(x => scenePath.Contains(x));
        }

        public static bool IsDatabaseObject(this GameObject gameObject)
        {
            var key = EnemyRandomizerDatabase.GetDatabaseKey(gameObject);
            if (key == null)
                return false;

            var db = EnemyRandomizerDatabase.GetDatabase();
            return db.Objects.ContainsKey(key);
        }

        public static string GetDatabaseKey(this ObjectMetadata metaObject)
        {
            return EnemyRandomizerDatabase.ToDatabaseKey(metaObject.ObjectName);
        }

        public static string GetDatabaseKey(this GameObject gameObject)
        {
            return EnemyRandomizerDatabase.GetDatabaseKey(gameObject);
        }

        public static GameObject SpawnExplosionAt(this Vector3 pos)
        {
            return SpawnEntityAt("Gas Explosion Recycle M", pos, true);
        }

        public static GameObject SpawnEntityAt(string entityName, Vector3 pos, bool setActive = false, bool allowRandomization = false)
        {
            if (allowRandomization)
                return EnemyRandomizerDatabase.CustomSpawn(pos, entityName, setActive);

            return EnemyRandomizerDatabase.CustomSpawnWithLogic(pos, entityName, null, true);
        }

        public static GameObject SpawnEntity(this GameObject gameObject, string entityName, bool setActive = false, bool allowRandomization = false)
        {
            if (allowRandomization)
                return EnemyRandomizerDatabase.CustomSpawn(gameObject.transform.position, entityName, setActive);

            return EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, entityName, null, true);
        }

        public static bool IsPossibleReplacementObject(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            if (!gameObject.IsDatabaseObject())
                return false;

            if (!gameObject.IsInAValidScene())
                return false;

            if (gameObject.HasReplacedAnObject())
                return false;

            if (gameObject.IsDisabledBySavedGameState())
                return false;

            return true;
        }

        public static bool CanProcessObject(this GameObject gameObject)
        {
            if (gameObject.IsInvalidSceneObject())
                return false;

            if (!gameObject.IsPossibleReplacementObject())
                return false;

            if (gameObject.IsInALoadingScene())
                return false;

            if (!gameObject.IsVisible())
                return false;

            return true;
        }

        public static bool IsTemporarilyInactive(this GameObject gameObject)
        {
            if (!gameObject.IsPossibleReplacementObject())
                return false;

            //only enemies can be temporarily inactive
            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return false;

            //we expect our object to be either in a loading scene or not visible to qualify as temporarily inactive
            if (gameObject.IsInALoadingScene() || !gameObject.IsVisible())
                return true;

            return false;
        }

        public static PrefabObject.PrefabType ObjectType(this GameObject gameObject)
        {
            if (gameObject == null)
                return PrefabObject.PrefabType.None;

            var metaObject = ObjectMetadata.Get(gameObject);
            if (metaObject != null)
            {
                return ObjectType(metaObject.ObjectName);
            }

            return ObjectType(gameObject.name);
        }

        public static PrefabObject.PrefabType ObjectType(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return PrefabObject.PrefabType.None;

            var key = EnemyRandomizerDatabase.ToDatabaseKey(objectName);
            if (key == null)
                return PrefabObject.PrefabType.None;

            if (EnemyRandomizerDatabase.GetDatabase().Objects.TryGetValue(key, out var po))
            {
                return po.prefabType;
            }

            return PrefabObject.PrefabType.None;
        }

        public static PrefabObject GetObjectPrefab(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            string key = EnemyRandomizerDatabase.GetDatabaseKey(gameObject);
            return GetObjectPrefab(key);
        }

        public static PrefabObject GetObjectPrefab(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return null;

            string key = EnemyRandomizerDatabase.ToDatabaseKey(objectName);

            if (EnemyRandomizerDatabase.GetDatabase().Objects.TryGetValue(key, out var po))
            {
                return po;
            }

            return null;
        }

        public static bool IsAValidScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return false;

            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid())
                return true;

            return false;
        }


        public static bool IsInAValidScene(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            var metaObject = ObjectMetadata.Get(gameObject);
            if (metaObject != null)
            {
                return IsAValidScene(metaObject.SceneName);
            }

            if (!gameObject.scene.IsValid())
                return false;

            string sceneName = gameObject.scene.name;
            return IsAValidScene(sceneName);
        }

        public static bool IsALoadingScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return false;

            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && !scene.isLoaded)
                return true;

            return false;
        }

        public static bool IsInALoadingScene(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            var metaObject = ObjectMetadata.Get(gameObject);
            if (metaObject != null)
            {
                return IsALoadingScene(metaObject.SceneName);
            }

            if (!gameObject.scene.IsValid())
                return false;

            string sceneName = gameObject.scene.name;

            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && !scene.isLoaded)
                return true;

            return false;
        }


        public static int OriginalPrefabHP(this GameObject gameObject)
        {
            if (gameObject == null)
                return 0;

            var po = GetObjectPrefab(gameObject);

            return (po == null || po.prefabType != PrefabObject.PrefabType.Enemy || po.prefab == null || po.prefab.GetComponent<HealthManager>() == null) ? 0 : po.prefab.GetComponent<HealthManager>().hp;
        }

        public static int OriginalPrefabHP(string objectName)
        {
            var po = GetObjectPrefab(objectName);

            return (po == null || po.prefabType != PrefabObject.PrefabType.Enemy || po.prefab == null || po.prefab.GetComponent<HealthManager>() == null) ? 0 : po.prefab.GetComponent<HealthManager>().hp;
        }

        public static bool HasReplacedAnObject(this GameObject gameObject)
        {
            var metaObject = ObjectMetadata.Get(gameObject);
            var originalObject = ObjectMetadata.GetOriginal(gameObject);
            return metaObject != null && originalObject != null;
        }

        public static bool IsAFreshObject(this GameObject gameObject)
        {
            var metaObject = ObjectMetadata.Get(gameObject);
            var originalObject = ObjectMetadata.GetOriginal(gameObject);
            return metaObject == null && originalObject == null;
        }

        public static bool IsASpawnedObject(this GameObject gameObject)
        {
            var metaObject = ObjectMetadata.Get(gameObject);
            var originalObject = ObjectMetadata.GetOriginal(gameObject);
            return metaObject != null && originalObject == null;
        }

        public static bool IsDisabledBySavedGameState(this GameObject gameObject)
        {
            var pdb = GetPersistentBoolData(gameObject);
            return pdb != null ? pdb.activated : false;
        }

        public static void SavePersistentBoolData(this GameObject gameObject, bool resetActivatedFlagOnBench = true, bool setActivatedFlag = false)
        {
            if (gameObject == null)
                return;

            var metaObject = ObjectMetadata.Get(gameObject);
            var originalObject = ObjectMetadata.GetOriginal(gameObject);

            if (originalObject != null)
            {
                var replacedName = originalObject.ObjectName;
                var replacedScene = originalObject.SceneName;

                //return global::SceneData.instance.FindMyState(new PersistentBoolData() { id = replacedName, sceneName = replacedScene });
                global::SceneData.instance.SaveMyState(new PersistentBoolData() { id = replacedName, sceneName = replacedScene, semiPersistent = resetActivatedFlagOnBench, activated = setActivatedFlag });
            }
            else if (metaObject != null)
            {
                var thisName = metaObject.ObjectName;
                var thisScene = metaObject.SceneName;

                //return global::SceneData.instance.FindMyState(new PersistentBoolData() { id = thisName, sceneName = thisScene });
                global::SceneData.instance.SaveMyState(new PersistentBoolData() { id = thisName, sceneName = thisScene, semiPersistent = resetActivatedFlagOnBench, activated = setActivatedFlag });
            }
            else
            {
                global::SceneData.instance.SaveMyState(new PersistentBoolData() { id = gameObject.name, sceneName = gameObject.scene.name, semiPersistent = resetActivatedFlagOnBench, activated = setActivatedFlag });
            }

        }

        public static PersistentBoolData GetPersistentBoolData(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var metaObject = ObjectMetadata.Get(gameObject);
            var originalObject = ObjectMetadata.GetOriginal(gameObject);

            if (originalObject != null)
            {
                var replacedName = originalObject.ObjectName;
                var replacedScene = originalObject.SceneName;

                return global::SceneData.instance.FindMyState(new PersistentBoolData() { id = replacedName, sceneName = replacedScene });

            }
            else if (metaObject != null)
            {
                var thisName = metaObject.ObjectName;
                var thisScene = metaObject.SceneName;

                return global::SceneData.instance.FindMyState(new PersistentBoolData() { id = thisName, sceneName = thisScene });

            }
            else
            {
                return global::SceneData.instance.FindMyState(new PersistentBoolData() { id = gameObject.name, sceneName = gameObject.scene.name });
            }
        }

        public static SpawnEffectOnDestroy AddEffectSpawnerOnCorpseRemoved(this GameObject corpse, GameObject owner, string effect = null)
        {
            var remover = corpse.gameObject.GetOrAddComponent<SpawnEffectOnDestroy>();
            if (!string.IsNullOrEmpty(effect))
            {
                remover.effectToSpawn = effect;
            }
            //remover.owner = ObjectMetadata.Get(owner);
            return remover;
        }

        public static bool RollProbability(out int result, int needValueOrLess = 5, int maxPossibleValue = 20)
        {
            RNG rng = new RNG();
            rng.Reset();
            result = rng.Rand(0, maxPossibleValue);
            return result < needValueOrLess;
        }

        public static GameObject GetCorpseObject(this GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var corpse = gameObject.GetCorpse<EnemyDeathEffects>();
            if (corpse == null)
            {
                corpse = gameObject.GetCorpsePrefab<EnemyDeathEffects>();
            }

            return corpse;
        }

        public static bool IsBoss(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.Bosses.Contains(DatabaseName);
        }

        public static bool IsBoss(this ObjectMetadata metaObject)
        {
            if (metaObject == null)
                return false;

            var dbKey = EnemyRandomizerDatabase.ToDatabaseKey(metaObject.ObjectName);
            return MetaDataTypes.Bosses.Contains(dbKey);
        }

        public static bool IsBoss(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            string DatabaseName = gameObject.GetDatabaseKey();

            var originalObject = ObjectMetadata.GetOriginal(gameObject);
            if (originalObject != null)
            {
                var dbKey = EnemyRandomizerDatabase.ToDatabaseKey(originalObject.ObjectName);
                return IsBoss(dbKey);
            }

            return DatabaseName == null ? false : MetaDataTypes.Bosses.Contains(DatabaseName);
        }

        public static bool IsFlying(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.Flying.Contains(DatabaseName);
        }

        public static bool IsFlying(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsFlying(DatabaseName);
        }

        public static bool IsCrawling(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.Crawling.Contains(DatabaseName);
        }

        public static bool IsCrawling(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsCrawling(DatabaseName);
        }

        public static bool IsClimbing(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.Climbing.Contains(DatabaseName);
        }

        public static bool IsClimbing(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsClimbing(DatabaseName);
        }

        public static bool IsMobile(string DatabaseName)
        {
            return DatabaseName == null ? false : !MetaDataTypes.Static.Contains(DatabaseName);
        }

        public static bool IsMobile(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsMobile(DatabaseName);
        }

        public static bool IsInGroundEnemy(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.InGroundEnemy.Contains(DatabaseName);
        }

        public static bool IsInGroundEnemy(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsInGroundEnemy(DatabaseName);
        }

        public static bool IsEnemySpawner(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.SpawnerEnemies.Contains(DatabaseName);
        }

        public static bool IsEnemySpawner(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsEnemySpawner(DatabaseName);
        }

        public static bool IsTinker(string DatabaseName)
        {
            return DatabaseName == null ? false : MetaDataTypes.TinkerEnemies.Contains(DatabaseName);
        }

        public static bool IsTinker(this GameObject gameObject)
        {
            string DatabaseName = gameObject.GetDatabaseKey();
            return IsTinker(DatabaseName);
        }

        public static Vector2 GetSizeFromUniqueObject(this GameObject sceneObject)
        {
            Dev.Log($"Setting size for object {sceneObject}");
            if (EnemyRandomizerDatabase.ToDatabaseKey(sceneObject.name) == "Acid Flyer")
            {
                Dev.Log($"Looking for shell in {sceneObject}");
                var shell = sceneObject.FindGameObjectInChildrenWithName("Shell");
                if (shell == null)
                {
                    var result = GameObjectExtensions.EnumerateRootObjects(true).Where(x => x.name.Contains("Shell"))
                        .Select(x => x.LocateMyFSM("FSM")).Where(x => x != null).FirstOrDefault(x => x.FsmVariables.GetFsmGameObject("Parent").Value == sceneObject);
                    if (result != null)
                        shell = result.gameObject;
                }
                Dev.Log($"returning box collider for {shell}");
                return shell.GetComponent<BoxCollider2D>().size;
            }

            return Vector2.one;
        }

        public static Vector2 GetSizeFromUniqueObject(string objectName)
        {
            string databaseName = EnemyRandomizerDatabase.ToDatabaseKey(objectName);

            Dev.Log($"Getting size for object {objectName}");
            if (databaseName == "Acid Flyer")
            {
                var dbo = EnemyRandomizerDatabase.GetDatabase().Objects[databaseName];

                Dev.Log($"Looking for shell in {dbo}");
                var shell = dbo.prefab.FindGameObjectInChildrenWithName("Shell");
                if (shell == null)
                {
                    var result = GameObjectExtensions.EnumerateRootObjects(true).Where(x => x.name.Contains("Shell"))
                        .Select(x => x.LocateMyFSM("FSM")).Where(x => x != null).FirstOrDefault(x => x.FsmVariables.GetFsmGameObject("Parent").Value != null && x.FsmVariables.GetFsmGameObject("Parent").Value.name == objectName);
                    if (result != null)
                        shell = result.gameObject;
                }
                Dev.Log($"returning box collider for {shell}");
                return shell.GetComponent<BoxCollider2D>().size;
            }

            return Vector2.one;
        }

        public static Vector2 GetSizeFromComponents(GameObject sceneObject, bool checkSpriteColliderLast = false)
        {
            Vector2 result = Vector2.one;
            if (!checkSpriteColliderLast && sceneObject.GetComponent<tk2dSprite>() && sceneObject.GetComponent<tk2dSprite>().boxCollider2D != null)
            {
                result = sceneObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                //Dev.Log($"Size of SPRITE {sceneObject} is {result}");
            }
            else if (sceneObject.GetComponent<BoxCollider2D>())
            {
                result = sceneObject.GetComponent<BoxCollider2D>().size;
                //Dev.Log($"Size of BOX {sceneObject} is {result}");
            }
            else if (sceneObject.GetComponent<CircleCollider2D>())
            {
                var newCCircle = sceneObject.GetComponent<CircleCollider2D>();
                result = Vector2.one * newCCircle.radius;
                //Dev.Log($"Size of CIRCLE {sceneObject} is {result}");
            }
            else if (sceneObject.GetComponent<PolygonCollider2D>())
            {
                var newCPoly = sceneObject.GetComponent<PolygonCollider2D>();
                result = new Vector2(newCPoly.points.Select(x => x.x).Max() - newCPoly.points.Select(x => x.x).Min(), newCPoly.points.Select(x => x.y).Max() - newCPoly.points.Select(x => x.y).Min());

                //Dev.Log($"Size of POLYGON {sceneObject} is {result}");
            }
            else if (checkSpriteColliderLast && sceneObject.GetComponent<tk2dSprite>() && sceneObject.GetComponent<tk2dSprite>().boxCollider2D != null)
            {
                result = sceneObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                //Dev.Log($"Size of SPRITE {sceneObject} is {result}");
            }
            else
            {
                result = sceneObject.transform.localScale;
                //Dev.Log($"Size of TRANSFORM SCALE {sceneObject} is {result}");

                if (result.x < 0)
                    result = new Vector2(-result.x, result.y);
            }
            return result;
        }

        public static bool IsSmasher(string objectName)
        {
            var po = GetObjectPrefab(objectName);
            if (po == null)
                return false;

            return po.prefabName.Contains("Big Bee");
        }

        public static bool IsSmasher(this GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            return IsSmasher(gameObject.name);
        }

        public static int GetOriginalGeo(string objectName)
        {
            var po = GetObjectPrefab(objectName);
            if (po == null)
                return 0;

            if (po.prefabType != PrefabObject.PrefabType.Enemy)
                return 0;
            
            var hm = po.prefab.GetComponent<HealthManager>();
            if (hm == null)
                return 0;

            int sm = hm.GetSmallGeo();
            int md = hm.GetMedGeo() * 5;
            int lg = hm.GetLargeGeo() * 25;

            return sm + md + lg;
        }


        public static Vector2 GetSizeFromComponents(string objectName, bool checkSpriteColliderLast = false)
        {
            string databaseName = EnemyRandomizerDatabase.ToDatabaseKey(objectName);
            if (string.IsNullOrEmpty(databaseName))
                return Vector2.one;

            if (!EnemyRandomizerDatabase.GetDatabase().Objects.TryGetValue(databaseName, out var dbo))
                return Vector2.one;

            var prefabObject = dbo.prefab;

            Vector2 result = Vector2.one;

            if (!checkSpriteColliderLast && prefabObject.GetComponent<tk2dSprite>() && prefabObject.GetComponent<tk2dSprite>().boxCollider2D != null)
            {
                result = prefabObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                Dev.Log($"Size of SPRITE {prefabObject} is {result}");
            }
            else if (prefabObject.GetComponent<BoxCollider2D>())
            {
                result = prefabObject.GetComponent<BoxCollider2D>().size;
                Dev.Log($"Size of BOX {prefabObject} is {result}");
            }
            else if (prefabObject.GetComponent<CircleCollider2D>())
            {
                var newCCircle = prefabObject.GetComponent<CircleCollider2D>();
                result = Vector2.one * newCCircle.radius;
                Dev.Log($"Size of CIRCLE {prefabObject} is {result}");
            }
            else if (prefabObject.GetComponent<PolygonCollider2D>())
            {
                var newCPoly = prefabObject.GetComponent<PolygonCollider2D>();
                result = new Vector2(newCPoly.points.Select(x => x.x).Max() - newCPoly.points.Select(x => x.x).Min(), newCPoly.points.Select(x => x.y).Max() - newCPoly.points.Select(x => x.y).Min());

                Dev.Log($"Size of POLYGON {prefabObject} is {result}");
            }
            else if (checkSpriteColliderLast && prefabObject.GetComponent<tk2dSprite>() && prefabObject.GetComponent<tk2dSprite>().boxCollider2D != null)
            {
                result = prefabObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                Dev.Log($"Size of SPRITE {prefabObject} is {result}");
            }
            else
            {
                result = prefabObject.transform.localScale;
                Dev.Log($"Size of TRANSFORM SCALE {prefabObject} is {result}");

                if (result.x < 0)
                    result = new Vector2(-result.x, result.y);
            }
            return result;
        }

        public static bool IsPogoLogicType(string gameObjectName)
        {
            var dbName = EnemyRandomizerDatabase.ToDatabaseKey(gameObjectName);
            return MetaDataTypes.PogoLogicEnemies.Contains(dbName);
        }

        public static bool CheckIfIsPogoLogicType(this GameObject sceneObject)
        {
            if (sceneObject == null)
                return false;

            var originalObject = ObjectMetadata.GetOriginal(sceneObject);
            if (originalObject != null)
            {
                if (IsPogoLogicType(originalObject.ObjectName))
                    return true;
            }

            return IsPogoLogicType(sceneObject.name);
        }

        public static bool CheckIfIsCustomArenaCageType(this GameObject sceneObject)
        {
            if (sceneObject == null)
                return false;

            return sceneObject.name.Contains("Arena Cage");
        }

        public static bool CheckIfIsBadObject(string scenePath)
        {
            return MetaDataTypes.AlwaysDeleteObject.Any(x => scenePath.Contains(x));
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

        public static float SetRotationToDirection(this GameObject gameObject, Vector2 direction, bool flipped = false)
        {
            var normal = direction.normalized;
            {
                var angles = gameObject.transform.localEulerAngles;

                if (normal.y > 0)
                {
                    angles.z = 0f;
                }
                else if (normal.y < 0)
                {
                    angles.z = 180f;
                }
                else if (normal.x < 0)
                {
                    angles.z = 90f;
                }
                else if (normal.x > 0)
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


        public static bool SkipForLogic(this GameObject sceneObject)
        {
            if (sceneObject == null)
                return false;

            //for now we only need to do logic if we are an enemy
            if (sceneObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return false;

            var dbKey = EnemyRandomizerDatabase.ToDatabaseKey(sceneObject.ObjectName());
            if (dbKey == null)
                return false;

            return MetaDataTypes.SkipReplacementOfEnemyForLogicReasons.Contains(dbKey);
        }

        public static void FixForLogic(this GameObject sceneObject, GameObject other)
        {
            if (sceneObject == null || other == null)
                return;

            //for now we only need to do logic if we are an enemy
            if (sceneObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

            //did we replace an enemy?
            if (other.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

            //no fix needed
            if (sceneObject == other)
                return;

            //did we replace a pogo logic enemy?
            if (CheckIfIsPogoLogicType(other))
            {
                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.Log($"{sceneObject.GetSceneHierarchyPath()}: Trying to fix pogos....");
                sceneObject.FixForPogos(other);
            }

            if (other.IsSmasher())
            {
                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.Log($"{sceneObject.GetSceneHierarchyPath()}: Trying to fix smasher....");
                sceneObject.MakeSmasher(other);

                //TODO: if this was a static replacement we need to give it movement.... or solve some other way
            }
        }

        public static void FixForPogos(this GameObject gameObject, GameObject other)
        {
            float tweenDirection = 1f;
            float tweenRate = 3f;

            var dbName = EnemyRandomizerDatabase.ToDatabaseKey(other.ObjectName());

            if (dbName.Contains("Acid"))
            {
                if (IsFlying(dbName) && IsMobile(dbName))
                {
                    var po = other;
                        //GetObjectPrefab(other.ObjectName());

                    var tweener = other.GetComponentsInChildren<PlayMakerFSM>(true).FirstOrDefault(x => x.FsmName == "Tween");
                    tweenDirection = tweener.FsmVariables.GetFsmVector3("Move Vector").Value.y;
                    tweenRate = tweener.FsmVariables.GetFsmFloat("Speed").Value;
                }
            }

            gameObject.StripMovements(true);
            gameObject.MakeTinker(true, true);
            gameObject.PlayIdleAnimation();

            var collider = gameObject.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            //this will be the white palace fly
            if (!gameObject.IsMobile())
            {
                gameObject.transform.position = other.transform.position;
                gameObject.LockIntoPosition(other.transform.position);
            }
            //this will be acid flyer or acid walker
            else if (IsTinker(dbName))
            {
                if (IsFlying(dbName))
                {
                    //v = d / t
                    //v t = d
                    //t = d / v
                    //tween travel time
                    var tween = gameObject.GetOrAddComponent<CustomTweener>();
                    tween.lerpFunc = tween.easeInOutSine;
                    tween.from = other.transform.position;
                    tween.to = tween.from + Vector3.up * 1f * tweenDirection;
                    float distance = (tween.to - tween.from).magnitude;
                    tween.travelTime = Mathf.Abs(distance / tweenRate);
                }
                //walker
                else
                {
                    //if (!IsWalker(dbName))
                    {
                        var leftRay = SpawnerExtensions.GetRayOn(other.transform.position, Vector2.left, float.MaxValue);
                        var rightRay = SpawnerExtensions.GetRayOn(other.transform.position, Vector2.right, float.MaxValue);

                        var tween = gameObject.GetOrAddComponent<CustomTweener>();
                        tween.lerpFunc = tween.linear;
                        tween.from = leftRay.point;
                        tween.to = rightRay.point;
                        tween.travelTime = (leftRay.distance + rightRay.distance) / 2f;


                        //var walker = gameObject.GetOrAddComponent<Walker>();
                        //walker.walkSpeedL = 2f;
                        //walker.walkSpeedR = 2f;
                        //walker.startInactive = false;
                        //walker.ignoreHoles = false;
                        //walker.StartMoving();
                    }
                }
            }
        }

        public static void DestroyObject(GameObject source, bool disableObjectBeforeDestroy = true)
        {
            if (source == null)
                return;

            Dev.Log($"Destroying [{source.GetSceneHierarchyPath()}]");
            if (source.name.Contains("Fly") && source.scene.name == "Crossroads_04")
            {
                //this seems to correctly decrement the count from the battle manager
                BattleManager.StateMachine.Value.RegisterEnemyDeath(null);
            }

            if (disableObjectBeforeDestroy)
            {
                source.SetActive(false);
            }

            GameObject.Destroy(source);
        }

        public static float GetRelativeScale(string sourceName, string otherName, float min = .2f, float max = 2.5f)
        {
            var oldSize = GetOriginalObjectSize(otherName);
            var newSize = GetOriginalObjectSize(sourceName);

            float scaleX = oldSize.x / newSize.x;
            float scaleY = oldSize.y / newSize.y;
            float scale = scaleX > scaleY ? scaleY : scaleX;

            if (scale < min)
                scale = min;

            if (scale > max)
                scale = max;

            return scale;
        }

        public static float GetRelativeScale(this GameObject source, GameObject other, float min = .1f, float max = 2.5f)
        {
            var oldSize = other.GetOriginalObjectSize();
            var newSize = source.GetOriginalObjectSize();

            float scaleX = oldSize.x / newSize.x;
            float scaleY = oldSize.y / newSize.y;
            float scale = scaleX > scaleY ? scaleY : scaleX;

            if (scale < min)
                scale = min;

            if (scale > max)
                scale = max;

            return scale;
        }

        public static void ScaleObject(this GameObject Source, float scale)
        {
            float SizeScale = scale;
            var originalScale = Vector3.one;

            var soc = Source.GetComponent<SpawnedObjectControl>();
            if (soc != null)
                soc.SizeScale = scale;

            var po = Source.GetObjectPrefab();
            if(po != null)
            {
                originalScale = po.prefab.transform.localScale;
            }

            Source.transform.localScale = new Vector3(originalScale.x * scale, originalScale.y * scale, originalScale.z);

            var Walker = Source.GetWalker();
            if (Walker != null)
            {
                if (Walker.GetRightScale() > 0)
                    Walker.SetRightScale(scale);
                else
                    Walker.SetRightScale(-scale);
            }

            var fsms = Source.GetComponents<PlayMakerFSM>();
            {
                var sactions = fsms.SelectMany(x => x.Fsm.States.SelectMany(y => y.GetActions<SetScale>())).Where(x => x.y.IsNone && x.z.IsNone);
                foreach (var a in sactions)
                {
                    if (a.x.Value < 0)
                        a.x.Value = -scale;
                    else
                        a.x.Value = scale;
                }
            }
            {
                var sactions = fsms.SelectMany(x => x.Fsm.States.SelectMany(y => y.GetActions<SetScale>())).Where(x => x.y.Value == 0f && x.z.Value == 0f);
                foreach (var a in sactions)
                {
                    if (a.x.Value < 0)
                        a.x.Value = -scale;
                    else
                        a.x.Value = scale;
                }
            }
            {
                var sactions = fsms.SelectMany(x => x.Fsm.States.SelectMany(y => y.GetActions<FaceObject>()));
                foreach (var a in sactions)
                {
                    a.GetType().GetField("xScale", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a, scale);
                }
            }
        }

        public static void SetAudioToMatchScale(this GameObject Source, float sizeScale, bool scaleCorpse = true)
        {
            Source.ScaleAudio(sizeScale);

            if (scaleCorpse)
            {
                GameObject corpse = Source.GetCorpseObject();
                if (corpse != null)
                {
                    corpse.ScaleAudio(sizeScale);
                }
            }
        }

        public static void ScaleAudio(this GameObject go, float sizeScale)
        {
            if (go == null || Mathnv.FastApproximately(sizeScale, 1f, .01f))
                return;

            float size_min = 0.1f;
            float size_max = 2.5f;

            Range size_rangeTop = new Range(1f, size_max);
            Range size_rangeBot = new Range(size_min, 1f);

            float pitch_max = 2f; //higher pitch for smaller enemies
            float pitch_min = .7f; //lower pitch for bigger enemies

            Range pitch_rangeTop = new Range(1f, pitch_max);
            Range pitch_rangeBot = new Range(pitch_min, 1f);

            float t = 0f;
            float pitch = 0f;

            if (sizeScale < 1f)
            {
                // .7   .1 - 1
                //      (.9) * .7
                // .7 / .9
                // = .78
                // 1 - .78
                // = .22
                // eval(.22)
                // =

                float range = 1f - size_min;
                float ratio = sizeScale / range;
                float inver = 1f - ratio;
                float fpitch = 1f + (pitch_max - 1f) * inver;

                t = inver;
                pitch = fpitch;

                //t = 1f - size_rangeBot.NormalizedValue(SizeScale);
                //pitch = pitch_rangeTop.Evaluate(t);
            }
            else//if(SizeScale > 1f)
            {
                float range = size_max - 1f;
                float ratio = (sizeScale - 1f) / range;

                if (ratio > 1f)
                    ratio = 1f;

                float fpitch = 1f - (1f - pitch_min) * ratio;

                t = ratio;
                pitch = fpitch;


                //t = 1f - size_rangeTop.NormalizedValue(SizeScale);
                //pitch = pitch_rangeBot.Evaluate(t);
            }

            Dev.Log($"{go.GetSceneHierarchyPath()} audio pitch from size {sizeScale} to t {t} to pitch {pitch}");

            var audioSources = go.GetComponentsInChildren<AudioSource>(true);
            var audioSourcesPitchRandomizer = go.GetComponentsInChildren<AudioSourcePitchRandomizer>(true);
            var audioPlayOneShot = go.GetActionsOfType<AudioPlayerOneShot>();
            var audioPlayRandom = go.GetActionsOfType<AudioPlayRandom>();
            var audioPlayOneShotSingle = go.GetActionsOfType<AudioPlayerOneShotSingle>();
            var audioPlayRandomSingle = go.GetActionsOfType<AudioPlayRandomSingle>();
            var audioPlayAudioEvent = go.GetActionsOfType<PlayAudioEvent>();

            audioSources.ToList().ForEach(x => x.pitch = pitch);
            audioSourcesPitchRandomizer.ToList().ForEach(x => x.pitchLower = pitch);
            audioSourcesPitchRandomizer.ToList().ForEach(x => x.pitchUpper = pitch);
            audioPlayOneShot.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayOneShot.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayRandom.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayRandom.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayOneShotSingle.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayOneShotSingle.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayRandomSingle.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayRandomSingle.ToList().ForEach(x => x.pitchMax = pitch);
            audioPlayAudioEvent.ToList().ForEach(x => x.pitchMin = pitch);
            audioPlayAudioEvent.ToList().ForEach(x => x.pitchMax = pitch);
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

            float colliderSize = 0f;

            if (collider == null)
            {
                var collider2 = gameObject.GetComponent<CircleCollider2D>();
                if (collider2 != null)
                {
                    colliderSize = collider2.radius * 2f;
                }
                else
                {
                    var collider3 = gameObject.GetComponent<PolygonCollider2D>();
                    if(collider3 != null)
                    {
                        colliderSize = Mathf.Abs(Vector2.Dot(collider3.bounds.size, closest.normal));
                    }
                    else
                    {
                        colliderSize = 1f;
                    }
                }
            }
            else
            {
                colliderSize = Mathf.Abs(Vector2.Dot(collider.size, closest.normal));
            }

            Vector2 offset = closest.normal * colliderSize * offsetScale;
            offset.x = offset.x * gameObject.transform.localScale.x;
            offset.y = offset.y * gameObject.transform.localScale.y;

            var newPos = closest.point + offset;
            gameObject.transform.position = newPos;

            return newPos;
        }

        //public static RaycastHit2D GetNearestSurface(GameObject gameObject, float maxDistanceToCheck)
        //{
        //    List<RaycastHit2D> raycastHit2D = new List<RaycastHit2D>()
        //    {
        //        FireRayLocal(gameObject, Vector2.down, maxDistanceToCheck),
        //        FireRayLocal(gameObject, Vector2.up, maxDistanceToCheck),
        //        FireRayLocal(gameObject, Vector2.left, maxDistanceToCheck),
        //        FireRayLocal(gameObject, Vector2.right, maxDistanceToCheck),
        //    };

        //    var closest = raycastHit2D.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
        //    return closest;
        //}

        //public static RaycastHit2D GetGround(GameObject gameObject)
        //{
        //    RaycastHit2D result = FireRayLocal(gameObject, Vector2.down, float.MaxValue);
        //    return result;
        //}

        //public static RaycastHit2D GetRoof(GameObject gameObject)
        //{
        //    RaycastHit2D result = FireRayLocal(gameObject, Vector2.up, float.MaxValue);
        //    return result;
        //}

        //public static RaycastHit2D GetLeft(GameObject gameObject)
        //{
        //    RaycastHit2D result = FireRayLocal(gameObject, Vector2.left, float.MaxValue);
        //    return result;
        //}

        //public static RaycastHit2D GetRight(GameObject gameObject)
        //{
        //    RaycastHit2D result = FireRayLocal(gameObject, Vector2.right, float.MaxValue);
        //    return result;
        //}

        //public static Dictionary<Vector2,RaycastHit2D> GetNearestSurfaces(this GameObject gameObject, float maxDistanceToCheck)
        //{
        //    Dictionary < Vector2, RaycastHit2D> raycastHits2D = new Dictionary<Vector2,RaycastHit2D>()
        //    {
        //        {Vector2.down, FireRayLocal(gameObject, Vector2.down, maxDistanceToCheck) },
        //        {Vector2.up,   FireRayLocal(gameObject, Vector2.up,   maxDistanceToCheck) },
        //        {Vector2.left, FireRayLocal(gameObject, Vector2.left, maxDistanceToCheck) },
        //        {Vector2.right,FireRayLocal(gameObject, Vector2.right, maxDistanceToCheck) },
        //    };

        //    return raycastHits2D;
        //}

        //public static RaycastHit2D FireRayLocal(GameObject gameObject, Vector2 direction, float length)
        //{
        //    var collider = gameObject.GetComponent<Collider2D>();
        //    Vector2 vector = collider.transform.TransformPoint(collider.offset);
        //    Vector2 vector2 = collider.transform.TransformDirection(direction);
        //    RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
        //    //Debug.DrawRay(vector, vector2);
        //    return raycastHit2D;
        //}

        //public static RaycastHit2D FireRayGlobal(this Vector2 origin, Vector2 direction, float length)
        //{
        //    Vector2 vector = origin;
        //    Vector2 vector2 = direction;
        //    RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
        //    //Debug.DrawRay(vector, vector2);
        //    return raycastHit2D;
        //}

        //public static RaycastHit2D Fire2DRayGlobal(this Vector3 origin, Vector2 direction, float length)
        //{
        //    Vector2 vector = origin;
        //    Vector2 vector2 = direction;
        //    RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, vector2, length, 256);
        //    //Debug.DrawRay(vector, vector2);
        //    return raycastHit2D;
        //}

        public static Vector3 GetVectorTo(this GameObject entity, Vector2 dir, float max)
        {
            return Mathnv.GetVectorTo(entity.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetVectorTo(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetVectorTo(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetPointOn(this GameObject entity, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(entity.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetRayOn(this GameObject entity, Vector2 dir, float max)
        {
            return Mathnv.GetRayOn(entity.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestChunkIntersection(this GameObject entity, Vector2 dir, float max)
        {
            return Mathnv.GetNearestChunkIntersection(entity.transform.position, dir, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestChunkIntersection(this Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetNearestChunkIntersection(origin, dir, max, IsSurfaceOrPlatform);
        }

        //public static int CountChunkIntersections(this GameObject entity, Vector2 dir, float max)
        //{
        //    return Mathnv.CountChunkIntersections(entity.transform.position, dir, max, IsSurfaceOrPlatform);
        //}

        //public static int CountChunkIntersections(this Vector2 origin, Vector2 dir, float max)
        //{
        //    return Mathnv.CountChunkIntersections(origin, dir, max, IsSurfaceOrPlatform);
        //}

        public static Vector3 GetPointOn(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetRayOn(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetRayOn(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static List<RaycastHit2D> GetRaysOn(Vector2 origin, Vector2 dir, float max)
        {
            return Mathnv.GetRaysOn(origin, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorToSurface(this GameObject entity, float max)
        {
            return Mathnv.GetNearestVectorTo(entity.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(this GameObject entity, float max)
        {
            return Mathnv.GetNearestPointOn(entity.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(Vector2 origin, float max)
        {
            return Mathnv.GetNearestPointOn(origin, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayOnSurface(this GameObject entity, float max)
        {
            return Mathnv.GetNearestRayOn(entity.transform.position, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayOnSurface(Vector2 origin, float max)
        {
            return Mathnv.GetNearestRayOn(origin, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorDown(this GameObject entity, float max)
        {
            return Mathnv.GetNearestVectorDown(entity.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(this GameObject entity, float max)
        {
            return Mathnv.GetNearestPointDown(entity.transform.position, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(Vector2 origin, float max)
        {
            return Mathnv.GetNearestPointDown(origin, max, IsSurfaceOrPlatform);
        }

        public static RaycastHit2D GetNearestRayDown(this GameObject entity, float max)
        {
            return Mathnv.GetNearestRayDown(entity.transform.position, max, IsSurfaceOrPlatform);
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

        public static List<RaycastHit2D> GetCardinalRays(this GameObject gameObject, float maxDistanceToCheck)
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

        //TODO: experimental, needs testing
        //public static bool IsInsideWalls(this GameObject gameObject)
        //{
        //    List<int> intersections = new List<int>()
        //    {
        //        CountChunkIntersections(gameObject, Vector2.down, float.MaxValue),
        //        CountChunkIntersections(gameObject, Vector2.up, float.MaxValue),
        //        CountChunkIntersections(gameObject, Vector2.left, float.MaxValue),
        //        CountChunkIntersections(gameObject, Vector2.right, float.MaxValue),
        //    };

        //    if (intersections.All(x => x == 0))
        //        return true;

        //    return intersections.Where(x => x > 0).Any(x => x % 2 == 1);
        //}

        //public static bool IsInsideWalls(this Vector2 point)
        //{
        //    List<int> intersections = new List<int>()
        //    {
        //        CountChunkIntersections(point, Vector2.down, float.MaxValue),
        //        CountChunkIntersections(point, Vector2.up, float.MaxValue),
        //        CountChunkIntersections(point, Vector2.left, float.MaxValue),
        //        CountChunkIntersections(point, Vector2.right, float.MaxValue),
        //    };

        //    if (intersections.All(x => x == 0))
        //        return true;

        //    return intersections.Where(x => x > 0).Any(x => x % 2 == 1);
        //}

        public static List<RaycastHit2D> GetWallRays(this GameObject gameObject)
        {
            List<RaycastHit2D> intersections = new List<RaycastHit2D>()
            {
                GetNearestChunkIntersection(gameObject, Vector2.down, float.MaxValue),
                GetNearestChunkIntersection(gameObject, Vector2.up, float.MaxValue),
                GetNearestChunkIntersection(gameObject, Vector2.left, float.MaxValue),
                GetNearestChunkIntersection(gameObject, Vector2.right, float.MaxValue),
            };

            return intersections;
        }

        public static RaycastHit2D GetNearstRayOutOfWalls(this GameObject gameObject)
        {
            List<RaycastHit2D> intersections = new List<RaycastHit2D>()
            {
                GetNearestChunkIntersection(gameObject, Vector2.down, float.MaxValue),
                GetNearestChunkIntersection(gameObject, Vector2.up, float.MaxValue),
                GetNearestChunkIntersection(gameObject, Vector2.left, float.MaxValue),
                GetNearestChunkIntersection(gameObject, Vector2.right, float.MaxValue),
            };

            var closest = intersections.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
            return closest;
        }

        public static RaycastHit2D GetNearstRayOutOfWalls(this Vector2 point)
        {
            List<RaycastHit2D> intersections = new List<RaycastHit2D>()
            {
                GetNearestChunkIntersection(point, Vector2.down, float.MaxValue),
                GetNearestChunkIntersection(point, Vector2.up, float.MaxValue),
                GetNearestChunkIntersection(point, Vector2.left, float.MaxValue),
                GetNearestChunkIntersection(point, Vector2.right, float.MaxValue),
            };

            var closest = intersections.Where(x => x.collider != null).OrderBy(x => x.distance).FirstOrDefault();
            return closest;
        }

        public static List<RaycastHit2D> GetOctagonalRays(this GameObject gameObject, float maxDistanceToCheck)
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

        public static List<RaycastHit2D> GetDiagonalRays(this GameObject gameObject, float maxDistanceToCheck)
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

        public static RaycastHit2D GetGroundX(this GameObject gameObject)
        {
            RaycastHit2D result = GetNearestRayDown(gameObject, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetRoofX(this GameObject gameObject)
        {
            RaycastHit2D result = GetRayOn(gameObject, Vector2.up, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetLeftX(this GameObject gameObject)
        {
            RaycastHit2D result = GetRayOn(gameObject, Vector2.left, float.MaxValue);
            return result;
        }

        public static RaycastHit2D GetRightX(this GameObject gameObject)
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

        public static IEnumerator DistanceFlyChase(this GameObject self, GameObject target, float distance, float acceleration, float speedMax, float? followHeightOffset = null, IEnumerator innerAttack = null, float? innerAttackDelay = null)
        {
            float attackTimeout = 0f;
            while (self.activeInHierarchy)
            {
                var rb2d = self.GetComponent<Rigidbody2D>();
                if (rb2d == null)
                {
                    yield break;
                }

                if(innerAttackDelay != null && innerAttack != null)
                {
                    attackTimeout += Time.deltaTime;
                    if(attackTimeout >= innerAttackDelay.Value)
                    {
                        yield return innerAttack;
                        attackTimeout = 0f;
                    }
                }

                float distanceAway = (self.transform.position.ToVec2() - target.transform.position.ToVec2()).magnitude;
                //var distanceAway = Mathf.Sqrt(Mathf.Pow(self.transform.position.x - target.transform.position.x, 2f) + Mathf.Pow(self.transform.position.y - target.transform.position.y, 2f));
                Vector2 velocity = rb2d.velocity;
                if (distanceAway > distance)
                {
                    if (self.transform.position.x < target.transform.position.x)
                    {
                        velocity.x += acceleration;
                    }
                    else
                    {
                        velocity.x -= acceleration;
                    }
                    if (followHeightOffset == null)
                    {
                        if (self.transform.position.y < target.transform.position.y)
                        {
                            velocity.y += acceleration;
                        }
                        else
                        {
                            velocity.y -= acceleration;
                        }
                    }
                }
                else
                {
                    if (self.transform.position.x < target.transform.position.x)
                    {
                        velocity.x -= acceleration;
                    }
                    else
                    {
                        velocity.x += acceleration;
                    }
                    if (followHeightOffset == null)
                    {
                        if (self.transform.position.y < target.transform.position.y)
                        {
                            velocity.y -= acceleration;
                        }
                        else
                        {
                            velocity.y += acceleration;
                        }
                    }
                }
                if (followHeightOffset != null)
                {
                    if (self.transform.position.y < target.transform.position.y + followHeightOffset.Value)
                    {
                        velocity.y += acceleration;
                    }
                    if (self.transform.position.y > target.transform.position.y + followHeightOffset.Value)
                    {
                        velocity.y -= acceleration;
                    }
                }
                if (velocity.x > speedMax)
                {
                    velocity.x = speedMax;
                }
                if (velocity.x < -speedMax)
                {
                    velocity.x = -speedMax;
                }
                if (velocity.y > speedMax)
                {
                    velocity.y = speedMax;
                }
                if (velocity.y < -speedMax)
                {
                    velocity.y = -speedMax;
                }
                rb2d.velocity = velocity;
                yield return new WaitForFixedUpdate();
            }
        }

        public static IEnumerator GetBigBeeChargeAttack(this GameObject owner, GameObject target, Action<GameObject, RaycastHit2D> onHit, Action<GameObject> onComplete)
        {
            return GetBigBeeChargeAttack(owner, target.transform.position, onHit, onComplete);
        }

        public static IEnumerator GetBigBeeChargeAttack(this GameObject owner, Vector2 target, Action<GameObject, RaycastHit2D> onHit, Action<GameObject> onComplete)
        {
            var bigBeePrefab = SpawnerExtensions.GetObjectPrefab("Big Bee");
            var po = bigBeePrefab.prefab;
            var fsm = po.LocateMyFSM("Big Bee");

            var audio_bigBeePrepare = fsm.GetState("Charge Antic").GetAction<AudioPlaySimple>(2).oneShotClip.Value as AudioClip;
            var anim_bigBeePrepare = fsm.GetState("Charge Antic").GetAction<Tk2dPlayAnimationV2>(7).clipName.Value;

            var audio_bigBeeChargeStart = fsm.GetState("Charge Start").GetAction<SetAudioClip>(0).audioClip.Value as AudioClip;
            var anim_bigBeeChargeStart = fsm.GetState("Charge Start").GetAction<Tk2dPlayAnimationV2>(3).clipName.Value;

            var audio_bigBeeBounce = fsm.GetState("Check Dir").GetAction<AudioPlaySimple>(0).oneShotClip.Value as AudioClip;

            var slamEffect = fsm.GetState("Hit Up").GetAction<SpawnObjectFromGlobalPool>(0).gameObject.Value;
            var slamEffectSplat = fsm.GetState("Spatter Honey").GetAction<FlingObjectsFromGlobalPool>(1).gameObject.Value;

            Func<GameObject, bool> isSurfaceOrPlatform = SpawnerExtensions.IsSurfaceOrPlatform;

            Action<GameObject> onStart = (x) =>
            {
                var audio = owner.GetComponent<AudioSource>();
                var anim = owner.GetComponent<tk2dSpriteAnimator>();

                audio.PlayOneShot(audio_bigBeePrepare);
                anim.Play(anim_bigBeePrepare);
            };

            Action<GameObject> onCharge = (x) =>
            {
                var audio = owner.GetComponent<AudioSource>();
                var anim = owner.GetComponent<tk2dSpriteAnimator>();

                audio.PlayOneShot(audio_bigBeeChargeStart);
                anim.Play(anim_bigBeeChargeStart);
            };

            Action<GameObject, RaycastHit2D> onBounce = (x,y) =>
            {
                var audio = owner.GetComponent<AudioSource>();
                var anim = owner.GetComponent<tk2dSpriteAnimator>();

                audio.PlayOneShot(audio_bigBeeBounce);
                //anim.Play(anim_bigBeeChargeStart);
            };

            var chargeRoutine = DoBouncingCharge(owner, (target - owner.transform.position.ToVec2()), 
                onStart: onStart, 
                onCharge: onCharge, 
                onHit: onHit, 
                onBounce: onBounce, 
                onComplete: onComplete, 
                isSurfaceOrPlatform: isSurfaceOrPlatform);

            return chargeRoutine;
        }

        public static IEnumerator DoBouncingCharge(this GameObject owner, Vector2 startingDirection, float chargeVelocity = 30f,
        int maxBounces = 3, float maxChargeTime = 10f, Action<GameObject> onStart = null, Action<GameObject> onCharge = null, Action <GameObject, RaycastHit2D> onHit = null, Action<GameObject, RaycastHit2D> onBounce = null, Action<GameObject> onComplete = null, Func<GameObject, bool> isSurfaceOrPlatform = null)
        {
            if (isSurfaceOrPlatform == null)
                isSurfaceOrPlatform = SpawnerExtensions.IsSurfaceOrPlatform;

            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
            Collider2D collider = owner.GetComponent<Collider2D>();

            float timeElapsed = 0f;
            int bounces = 0;
            float accelerationTime = 0.2f;
            float decelerationTime = 0.2f;
            float acceleration = chargeVelocity / accelerationTime;
            float totalDecelerationTime = 0f;
            float currentVelocity = 0f;

            var startingVelocity = rb.velocity;
            var finalDirection = startingDirection;
            Vector2 movingDirection = startingDirection;

            onStart?.Invoke(owner);
            bool doneAccelerating = false;

            while (owner != null && owner.activeInHierarchy)
            {
                if(timeElapsed > 0f && currentVelocity <= 0f)
                    break;

                float fdt = Time.fixedDeltaTime;
                if (timeElapsed < accelerationTime)
                {
                    currentVelocity = Mathf.Lerp(0f, chargeVelocity, timeElapsed / accelerationTime);
                }
                else if (timeElapsed >= accelerationTime && (bounces >= maxBounces || timeElapsed >= maxChargeTime))
                {
                    totalDecelerationTime += fdt;
                    currentVelocity = Mathf.Lerp(chargeVelocity, 0f, totalDecelerationTime / decelerationTime);
                }

                if(!doneAccelerating && timeElapsed >= accelerationTime)
                {
                    doneAccelerating = true;
                    onCharge?.Invoke(owner);
                }

                if (currentVelocity <= 0f)
                {
                    currentVelocity = 0f;
                    break;
                }

                float distance = currentVelocity * fdt;
                Vector2 movement = movingDirection.normalized * distance;

                var hits = Physics2D.BoxCastAll(collider.bounds.center, collider.bounds.size, 0f, movement.normalized,
                    movement.magnitude, Physics2D.AllLayers);

                var nearstWallHit = hits.Where(x => x.collider != null && isSurfaceOrPlatform(x.collider.gameObject)).OrderBy(x => x.distance).FirstOrDefault();
                var thingsHit = hits.Where(x => x.collider != null && !isSurfaceOrPlatform(x.collider.gameObject)).OrderBy(x => x.distance);

                if (nearstWallHit.collider != null)
                {
                    movingDirection = Vector2.Reflect(movement.normalized, nearstWallHit.normal);
                    Vector2 remainingMovement = movement - nearstWallHit.distance * movingDirection;
                    
                    rb.position = nearstWallHit.point + nearstWallHit.normal * collider.bounds.extents.x;

                    rb.MovePosition(rb.position + remainingMovement);
                    rb.velocity = Vector2.zero; //TODO: derive velocity? apply it?

                    bounces++;

                    onBounce?.Invoke(owner, nearstWallHit);
                }
                else
                {
                    rb.MovePosition(rb.position + movement);
                }

                if (thingsHit.Any())
                {
                    foreach (var hit in thingsHit)
                    {
                        var ed = owner.GetEnemyDamage();
                        var hd = owner.GetHeroDamage();

                        if(ed != null && hit.collider != null && hit.collider.GetComponent<HealthManager>() != null)
                        {
                            hit.collider.GetComponent<HealthManager>().Hit(new HitInstance() { AttackType = AttackTypes.Generic, DamageDealt = ed.damageDealt, Direction = GetActualDirection(owner, hit.collider.gameObject) });
                        }

                        if(hd != null && hit.collider != null && hit.collider.GetComponent<HealthManager>() != null)
                        {
                            hit.collider.GetComponent<HealthManager>().Hit(new HitInstance() { AttackType = AttackTypes.Generic, DamageDealt = hd.damageDealt, Direction = GetActualDirection(owner, hit.collider.gameObject) });
                        }

                        onHit?.Invoke(owner, hit);
                    }
                }

                rb.velocity = Vector2.zero;
                timeElapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            rb.velocity = Vector2.zero;
            rb.velocity = movingDirection * startingVelocity.magnitude;

            onComplete?.Invoke(owner);
        }


        public static Func<GameObject> GetRandomAttackSpawnerFunc(this GameObject gameObject)
        {
            RNG rng = new RNG();
            rng.Reset();

            List<Func<GameObject>> trailEffects = new List<Func<GameObject>>()
                {
                    //WARNING: using "CustomSpawnWithLogic" will override any replacement randomization modules
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Mega Jelly Zap", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Falling Barrel", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Electro Zap", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Shot PickAxe", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Dung Ball Small", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Paint Shot P Down", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Paint Shot B_fix", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Paint Shot R", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Gas Explosion Recycle L", null, false),
                    () => EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, "Lil Jellyfish", null, false),
                };

            var selection = trailEffects.GetRandomElementFromList(rng);
            return selection;
        }

        public static void UpdateRefs(this SpawnedObjectControl self, PlayMakerFSM fsm, Dictionary<string, Func<SpawnedObjectControl, float>> refs)
        {
            if (fsm == null)
                return;

            try
            {
                foreach (var fref in refs)
                {
                    var fvar = fsm.FsmVariables.GetFsmFloat(fref.Key);
                    if (fvar != null)
                    {
                        fvar.Value = fref.Value.Invoke(self);
                    }
                }
            }
            catch (Exception e)
            {
                Dev.Log($"{self}:{self.thisMetadata}: Caught exception in UpdateRefs");
            }
        }


        public static void SetXScaleSign(this GameObject gameObject, bool makeNegative)
        {
            float scale = gameObject.transform.localScale.x;
            if (scale > 0 && makeNegative)
                gameObject.transform.localScale = new Vector3(-scale, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            else if (scale < 0 && !makeNegative)
                gameObject.transform.localScale = new Vector3(-scale, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }

        public static void RemoveFSM(this GameObject gameObject, string name)
        {
            var fsm = gameObject.LocateMyFSM(name);
            if (fsm != null)
                GameObject.Destroy(fsm);
        }

        public static void KillObjectNow(this GameObject gameObject)
        {
            var hm = gameObject.GetComponent<HealthManager>();
            if (hm)
                hm.Die(null, AttackTypes.Generic, true);

            if(hm == null)
            {
                var scut = gameObject.GetComponent<ScuttlerControl>();
                if (scut != null)
                {
                    //generic hits on scuttlers just 
                    scut.Hit(new HitInstance() { AttackType = AttackTypes.Generic });
                }
            }
        }

        public static void SpawnBlood(this Vector2 pos, short minCount, short maxCount, float minSpeed, float maxSpeed, float angleMin = 0f, float angleMax = 360f, Color? bloodColor = null)
        {
            GlobalPrefabDefaults.Instance.SpawnBlood(pos, minCount, maxCount, minSpeed, maxSpeed, angleMin, angleMax, bloodColor);
        }

        public static void SpawnBlood(this GameObject gameObject, short minCount, short maxCount, float minSpeed, float maxSpeed, float angleMin = 0f, float angleMax = 360f, Color? bloodColor = null)
        {
            GlobalPrefabDefaults.Instance.SpawnBlood(gameObject.transform.position, minCount, maxCount, minSpeed, maxSpeed, angleMin, angleMax, bloodColor);
        }

        public static float GetActualDirection(this GameObject gameObject, GameObject target)
        {
            if (gameObject != null && target != null)
            {
                Vector2 vector = target.transform.position - gameObject.transform.position;
                return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
            }
            return 0f;
        }

        public static float GetActualDirection(this Vector2 from, Vector2 to)
        {
            Vector2 vector = to - from;
            return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
        }

        public static void RecordCustomJournalOnDeath(this GameObject gameObject)
        {
            var metaObject = ObjectMetadata.Get(gameObject);

            if (metaObject != null && !metaObject.IsCustomPlayerDataName)
                return;

            PlayerData playerData = GameManager.instance.playerData;
            string text = "killed" + metaObject.PlayerDataName;
            string text2 = "kills" + metaObject.PlayerDataName;
            string text3 = "newData" + metaObject.PlayerDataName;
            bool flag = false;
            if (!playerData.GetBool(text))
            {
                flag = true;
                playerData.SetBool(text, true);
                playerData.SetBool(text3, true);
            }
            bool flag2 = false;
            int num = playerData.GetInt(text2);
            if (num > 0)
            {
                num--;
                playerData.SetInt(text2, num);
                if (num <= 0)
                {
                    flag2 = true;
                }
            }
            if (playerData.hasJournal)
            {
                bool flag3 = false;
                if (flag2)
                {
                    flag3 = true;
                    playerData.journalEntriesCompleted++;
                }
                else if (flag)
                {
                    flag3 = true;
                    playerData.journalNotesCompleted++;
                }
                if (flag3)
                {
                    //in lieu of the proper journal unlock effect, just blow up in a very noticable way
                    SpawnEntityAt("Item Get Effect R", gameObject.transform.position, true);
                }
            }
        }

        public static string GetCustomPlayerDataName(this GameObject gameObject)
        {
            return GetCustomPlayerDataName(gameObject.name);
        }

        public static string GetCustomPlayerDataName(string objectName)
        {
            var dbKey = EnemyRandomizerDatabase.ToDatabaseKey(objectName);

            if (MetaDataTypes.CustomReplacementName.TryGetValue(dbKey, out string customName))
            {
                return customName;
            }

            return string.Empty;
        }

        public static bool CreateNewDatabasePrefabObject(string databaseID, GameObject source, SceneData sourceScene, PrefabObject.PrefabType prefabType = PrefabObject.PrefabType.Other)
        {
            bool result = false;
            try
            {
                var db = EnemyRandomizerDatabase.GetDatabase();
                if (db != null && !db.otherNames.Contains(databaseID))
                {
                    var beClone = GameObject.Instantiate(source);
                    beClone.SetActive(false);
                    GameObject.DontDestroyOnLoad(beClone);
                    PrefabObject p2 = new PrefabObject();
                    SceneObject sp2 = new SceneObject();
                    sp2.components = new List<string>();
                    sp2.Scene = sourceScene;
                    p2.prefabName = databaseID;
                    p2.prefabType = prefabType;
                    beClone.name = p2.prefabName;
                    sp2.path = beClone.name;
                    p2.prefab = beClone;
                    p2.source = sp2;
                    sp2.LoadedObject = p2;
                    sp2.Scene.sceneObjects.Add(sp2);
                    db.otherPrefabs.Add(p2);
                    db.Others[p2.prefabName] = p2;
                    db.Objects[p2.prefabName] = p2;
                    sp2.Loaded = true;
                    result = true;
                }
            }
            catch (Exception e)
            {
                Dev.Log($"Error creating new database prefab object with ID {databaseID} from source {source}. ERROR:{e.Message} STACKTRACE:{e.StackTrace}");
            }

            return result;
        }

        public static Vector3 GetRandomPositionInLOSofPlayer(this GameObject gameObject, float minTeleportDistance = 5f, float maxTeleportDistance = 35f, float minDistanceFromPlayer = 2f, int maxTries = 10)
        {
            RNG rng = new RNG();
            rng.Reset();

            int tries = maxTries;
            int i = 0;

            Vector2 startPosition = gameObject.transform.position.ToVec2() + Vector2.up * 0.1f; 
            Vector2 telePoint = HeroController.instance.transform.position;
            Vector2 origin = HeroController.instance.transform.position.ToVec2();

            //calculate object's size to resolve wall penetration and stuff
            Vector2 objectSize = Vector2.one;
            var soc = gameObject.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                float scale = soc.SizeScale;
                objectSize = gameObject.GetOriginalObjectSize(true) * scale;

                minTeleportDistance = minTeleportDistance * scale;
                maxTeleportDistance = maxTeleportDistance * scale;
                minDistanceFromPlayer = minDistanceFromPlayer * scale;
            }

            if (minDistanceFromPlayer < 2f)
                minDistanceFromPlayer = 2f;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);
                var teleDir = UnityEngine.Random.insideUnitCircle;

                //check to see if we hit a wall
                var spawnRay = SpawnerExtensions.GetRayOn(HeroController.instance.transform.position, teleDir, teleDist);

                if (spawnRay.collider != null)
                {
                    float offsetThroughWall = Mathf.Abs(Vector2.Dot(spawnRay.normal, objectSize));
                    teleDist = spawnRay.distance - offsetThroughWall;

                    //if we hit a wall, need to check above and below for space

                    Vector2 normalDir1 = new Vector2(-spawnRay.normal.y, spawnRay.normal.x).normalized;
                    Vector2 normalDir2 = -normalDir1;

                    Vector2 possiblePoint = origin + teleDir * teleDist;

                    var nRay1 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir1, offsetThroughWall);
                    var nRay2 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir2, offsetThroughWall);

                    //no good, it probably won't fit here
                    if (nRay1.collider != null || nRay2.collider != null)
                        continue;
                }

                telePoint = origin + teleDir * teleDist;

                if (teleDist <= 0)
                    continue;

                if (IsNearPlayer(telePoint, minDistanceFromPlayer))
                    continue;

                break;
            }

            if (i == tries)
                telePoint = startPosition;

            return telePoint;
        }


        public static Vector2 GetTeleportPositionAbovePlayer(this GameObject gameObject, float minTeleportDistance = 5f, float maxTeleportDistance = 35f, float minDistanceFromPlayer = 2f, int maxTries = 10)
        {
            RNG rng = new RNG();
            rng.Reset();

            int tries = maxTries;
            int i = 0;

            Vector2 startPosition = gameObject.transform.position.ToVec2() + Vector2.up * 0.1f;
            Vector2 telePoint = HeroController.instance.transform.position;
            Vector2 origin = HeroController.instance.transform.position.ToVec2();

            //calculate object's size to resolve wall penetration and stuff
            Vector2 objectSize = Vector2.one;
            var soc = gameObject.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                float scale = soc.SizeScale;
                objectSize = gameObject.GetOriginalObjectSize(true) * scale;

                minTeleportDistance = minTeleportDistance * scale;
                maxTeleportDistance = maxTeleportDistance * scale;
                minDistanceFromPlayer = minDistanceFromPlayer * scale;
            }

            if (minDistanceFromPlayer < 2f)
                minDistanceFromPlayer = 2f;

            var teleDir = Vector2.up;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);

                //check to see if we hit a wall
                var spawnRay = SpawnerExtensions.GetRayOn(origin, teleDir, teleDist);

                if (spawnRay.collider != null)
                {
                    float offsetThroughWall = Mathf.Abs(Vector2.Dot(spawnRay.normal, objectSize));
                    teleDist = spawnRay.distance - offsetThroughWall;

                    //if we hit a wall, need to check above and below for space

                    Vector2 normalDir1 = new Vector2(-spawnRay.normal.y, spawnRay.normal.x).normalized;
                    Vector2 normalDir2 = -normalDir1;

                    Vector2 possiblePoint = origin + teleDir * teleDist;

                    var nRay1 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir1, offsetThroughWall);
                    var nRay2 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir2, offsetThroughWall);

                    //no good, it probably won't fit here
                    if (nRay1.collider != null || nRay2.collider != null)
                        continue;
                }

                telePoint = origin + teleDir * teleDist;

                if (teleDist <= 0)
                    continue;

                if (IsNearPlayer(telePoint, minDistanceFromPlayer))
                    continue;

                break;
            }

            if (i == tries)
                telePoint = startPosition;

            return telePoint;
        }



        public static Vector2 GetTeleportPositionAboveSelf(this GameObject gameObject, float minTeleportDistance = 5f, float maxTeleportDistance = 35f, int maxTries = 10)
        {
            RNG rng = new RNG();
            rng.Reset();

            int tries = maxTries;
            int i = 0;

            Vector2 startPosition = gameObject.transform.position.ToVec2() + Vector2.up * 0.1f;
            Vector2 telePoint = startPosition;
            Vector2 origin = startPosition;

            //calculate object's size to resolve wall penetration and stuff
            Vector2 objectSize = Vector2.one;
            var soc = gameObject.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                float scale = soc.SizeScale;
                objectSize = gameObject.GetOriginalObjectSize(true) * scale;

                minTeleportDistance = minTeleportDistance * scale;
                maxTeleportDistance = maxTeleportDistance * scale;
            }

            var teleDir = Vector2.up;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);

                //check to see if we hit a wall
                var spawnRay = SpawnerExtensions.GetRayOn(origin, teleDir, teleDist);

                if (spawnRay.collider != null)
                {
                    float offsetThroughWall = Mathf.Abs(Vector2.Dot(spawnRay.normal, objectSize));
                    teleDist = spawnRay.distance - offsetThroughWall;

                    //if we hit a wall, need to check above and below for space

                    Vector2 normalDir1 = new Vector2(-spawnRay.normal.y, spawnRay.normal.x).normalized;
                    Vector2 normalDir2 = -normalDir1;

                    Vector2 possiblePoint = origin + teleDir * teleDist;

                    var nRay1 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir1, offsetThroughWall);
                    var nRay2 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir2, offsetThroughWall);

                    //no good, it probably won't fit here
                    if (nRay1.collider != null || nRay2.collider != null)
                        continue;
                }

                telePoint = origin + teleDir * teleDist;

                if (teleDist <= 0)
                    continue;

                break;
            }

            if (i == tries)
                telePoint = startPosition;

            return telePoint;
        }


        public static Vector2 GetHorizontalTeleportPositionFromPlayer(this GameObject gameObject, bool right, float heightOffset = 0f, float minTeleportDistance = 5f, float maxTeleportDistance = 35f, float minDistanceFromPlayer = 2f, int maxTries = 10)
        {
            RNG rng = new RNG();
            rng.Reset();

            int tries = maxTries;
            int i = 0;

            Vector2 startPosition = gameObject.transform.position.ToVec2() + Vector2.up * 0.1f;
            Vector2 telePoint = HeroController.instance.transform.position;
            Vector2 origin = HeroController.instance.transform.position.ToVec2() + Vector2.up * heightOffset;

            //calculate object's size to resolve wall penetration and stuff
            Vector2 objectSize = Vector2.one;
            var soc = gameObject.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                float scale = soc.SizeScale;
                objectSize = gameObject.GetOriginalObjectSize(true) * scale;

                minTeleportDistance = minTeleportDistance * scale;
                maxTeleportDistance = maxTeleportDistance * scale;
                minDistanceFromPlayer = minDistanceFromPlayer * scale;
            }

            if (minDistanceFromPlayer < 2f)
                minDistanceFromPlayer = 2f;

            var teleDir = Vector2.left;
            if (right)
                teleDir = Vector2.right;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);                

                //check to see if we hit a wall
                var spawnRay = SpawnerExtensions.GetRayOn(origin, teleDir, teleDist);

                if (spawnRay.collider != null)
                {
                    float offsetThroughWall = Mathf.Abs(Vector2.Dot(spawnRay.normal, objectSize));
                    teleDist = spawnRay.distance - offsetThroughWall;

                    //if we hit a wall, need to check above and below for space

                    Vector2 normalDir1 = new Vector2(-spawnRay.normal.y, spawnRay.normal.x).normalized;
                    Vector2 normalDir2 = -normalDir1;

                    Vector2 possiblePoint = origin + teleDir * teleDist;

                    var nRay1 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir1, offsetThroughWall);
                    var nRay2 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir2, offsetThroughWall);

                    //no good, it probably won't fit here
                    if (nRay1.collider != null || nRay2.collider != null)
                        continue;
                }

                telePoint = origin + teleDir * teleDist;

                if (teleDist <= 0)
                    continue;

                if (IsNearPlayer(telePoint, minDistanceFromPlayer))
                    continue;

                break;
            }

            if (i == tries)
                telePoint = startPosition;

            return telePoint;
        }

        public static Vector2 GetRandomDirectionFromSelf(this GameObject gameObject, bool upwardOnly = false)
        {
            Vector2 movementDir = UnityEngine.Random.insideUnitCircle;

            if (upwardOnly && movementDir.y < 0)
                movementDir.y = -movementDir.y;

            return movementDir;
        }

        public static Vector3 GetRandomPositionInLOSofSelf(this GameObject gameObject, float minTeleportDistance = 0f, float maxTeleportDistance = 25f, float minDistanceFromPlayer = 2f, bool mustEndInLOSOfPlayer = false, bool moveTowardsPlayer = true, int maxTries = 10)
        {
            var posToUse = gameObject.transform.position.ToVec2() + Vector2.up * 0.1f;

            RNG rng = new RNG();
            rng.Reset();

            int tries = maxTries;
            int i = 0;

            Vector2 startPosition = posToUse;
            Vector2 telePoint = posToUse;
            var toPlayer = gameObject.DirectionToPlayer();

            //calculate object's size to resolve wall penetration and stuff
            Vector2 objectSize = Vector2.one;
            var soc = gameObject.GetComponent<SpawnedObjectControl>();
            if (soc != null)
            {
                float scale = soc.SizeScale;
                objectSize = gameObject.GetOriginalObjectSize(true) * scale;

                minTeleportDistance = minTeleportDistance * scale;
                maxTeleportDistance = maxTeleportDistance * scale;
                minDistanceFromPlayer = minDistanceFromPlayer * scale;
            }

            if (minDistanceFromPlayer < 2f)
                minDistanceFromPlayer = 2f;

            for (i = 0; i < tries; ++i)
            {
                float teleDist = rng.Rand(minTeleportDistance, maxTeleportDistance);
                var teleDir = UnityEngine.Random.insideUnitCircle;

                //if the generated direction is away from our player, flip it
                if (moveTowardsPlayer)
                {
                    float dotProduct = Vector2.Dot(toPlayer, teleDir);
                    if (dotProduct < 0f)
                        teleDir = -teleDir;
                }

                //check to see if we hit a wall
                var spawnRay = SpawnerExtensions.GetRayOn(posToUse, teleDir, teleDist);

                if (spawnRay.collider != null)
                {
                    float offsetThroughWall = Mathf.Abs(Vector2.Dot(spawnRay.normal, objectSize));
                    teleDist = spawnRay.distance - offsetThroughWall;

                    //if we hit a wall, need to check above and below for space

                    Vector2 normalDir1 = new Vector2(-spawnRay.normal.y, spawnRay.normal.x).normalized;
                    Vector2 normalDir2 = -normalDir1;

                    Vector2 possiblePoint = startPosition + teleDir * teleDist;

                    var nRay1 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir1, offsetThroughWall);
                    var nRay2 = SpawnerExtensions.GetRayOn(possiblePoint, normalDir2, offsetThroughWall);

                    //no good, it probably won't fit here
                    if (nRay1.collider != null || nRay2.collider != null)
                        continue;
                }

                telePoint = startPosition + teleDir * teleDist;

                if (teleDist <= 0)
                    continue;

                if (IsNearPlayer(telePoint, minDistanceFromPlayer))
                    continue;

                if (mustEndInLOSOfPlayer && !CanSeePlayer(telePoint))
                    continue;

                break;
            }

            if (i == tries)
                telePoint = startPosition;

            return telePoint;
        }


        public static void DoBlueHealHero(this GameObject gameObject, float maxHealDistance = 40f)
        {
            HeroController.instance.StartCoroutine(BlueHealHero(gameObject, maxHealDistance));
        }

        static IEnumerator BlueHealHero(GameObject gameObject, float maxHealDistance = 40f)
        {
            var flash = HeroController.instance.GetComponent<SpriteFlash>();

            if (flash != null)
                flash.flashHealBlue();

            GameManager.UnloadLevel doHeal = null;
            doHeal = delegate ()
            {
                EventRegister.SendEvent("ADD BLUE HEALTH");
                GameManager.instance.UnloadingLevel -= doHeal;
                doHeal = null;
            };
            GameManager.instance.UnloadingLevel += doHeal;
            if (HeroController.instance && Vector2.Distance(gameObject.transform.position, HeroController.instance.transform.position) > maxHealDistance)
            {
                //too far to heal
                yield break;
            }

            yield return new WaitForSeconds(1.2f);
            if (doHeal != null)
            {
                doHeal();
            }
            yield break;
        }

        public static Vector2 Reflect(this Vector2 velocity, Vector2 reflectionNormal)
        {
            // Calculate the dot product of the velocity and the normal
            float dotProduct = Vector2.Dot(velocity, reflectionNormal);

            // Calculate the reflection vector
            Vector2 reflection = velocity - 2f * dotProduct * reflectionNormal;

            // Return the reflected vector
            return reflection;
        }



        public static Vector2 DirectionToPlayer(this GameObject gameObject)
        {
            Vector2 vector = gameObject.transform.position.ToVec2();
            Vector2 vector2 = HeroController.instance.transform.position.ToVec2();
            return (vector2 - vector).normalized;
        }

        public static float DistanceToPlayer(this GameObject gameObject)
        {
            Vector2 vector = gameObject.transform.position.ToVec2();
            Vector2 vector2 = HeroController.instance.transform.position.ToVec2();
            Vector2 vector3 = vector2 - vector;
            return vector3.magnitude;
        }

        public static float DistanceToPlayer(this Vector2 point)
        {
            Vector2 vector = point;
            Vector2 vector2 = HeroController.instance.transform.position.ToVec2();
            Vector2 vector3 = vector2 - vector;
            return vector3.magnitude;
        }

        public static bool IsNearPlayer(this Vector2 point, float near)
        {
            Vector2 vector = point;
            Vector2 vector2 = HeroController.instance.transform.position.ToVec2();
            Vector2 vector3 = vector2 - vector;
            return vector3.magnitude < near;
        }

        public static bool IsNearPlayer(this GameObject gameObject, float dist)
        {
            return gameObject.DistanceToPlayer() <= dist;
        }

        public static IEnumerable<CameraLockArea> GetCameraLocksFromScene(this GameObject gameObject)
        {
            return gameObject.GetComponentsFromScene<CameraLockArea>();
        }

        public static void UnlockCameras(this GameObject gameObject)
        {
            gameObject.GetCameraLocksFromScene().UnlockCameras();
        }

        public static void UnlockCameras(this IEnumerable<CameraLockArea> cameraLocks)
        {
            foreach (var c in cameraLocks)
            {
                c.gameObject.SetActive(false);
            }
        }

        public static Vector2 GetUpFromSelfAngle(this GameObject gameObject, bool isFlipped)
        {
            Vector2 up = Vector2.zero;

            float angle = gameObject.transform.localEulerAngles.z % 360f;
            if (!isFlipped)
            {
                angle = (angle + 180f) % 360f;
            }

            if (angle < 5f && angle < 355f)
            {
                up = Vector2.up;
            }
            else if (angle > 85f && angle < 95f)
            {
                up = Vector2.left;
            }
            else if (angle > 175f && angle < 185f)
            {
                up = Vector2.down;
            }
            else if (angle > 265f || angle < 275f)
            {
                up = Vector2.right;
            }

            return up;
        }

        public static string GetRandomPrefabNameForSpawnerEnemy(RNG rng)
        {
            if(rng == null)
            {
                rng = new RNG();
                rng.Reset();
            }

            var list = MetaDataTypes.PossibleEnemiesFromSpawner.ToList();
            var weights = list.Select(x => x.Value).ToList();

            int replacementIndex = rng.WeightedRand(weights);
            return list[replacementIndex].Key;
        }

        public static string GetRandomPrefabNameForArenaEnemy(RNG rng, string originalEnemy = null)
        {
            if (rng == null)
            {
                rng = new RNG();
                rng.Reset();
            }

            bool isArenaOK(string name)
            {
                if (MetaDataTypes.SafeForArenas.TryGetValue(name, out var isok))
                {
                    return isok;
                }
                return false;
            }

            if (originalEnemy == null)
            {
                var list = MetaDataTypes.RNGWeights.Where(x => isArenaOK(x.Key)).ToList();
                var weights = list.Select(x => x.Value).ToList();

                int replacementIndex = rng.WeightedRand(weights);
                return list[replacementIndex].Key;
            }
            else
            {
                var list = MetaDataTypes.RNGWeights.Where(x => isArenaOK(x.Key)).ToList();
                bool isFlying = SpawnerExtensions.IsFlying(EnemyRandomizerDatabase.ToDatabaseKey(originalEnemy));
                var typeMatch = list.Where(x => SpawnerExtensions.IsFlying(x.Key) == isFlying);
                var weights = typeMatch.Select(x => x.Value).ToList();

                int replacementIndex = rng.WeightedRand(weights);
                return list[replacementIndex].Key;
            }
        }

        public static void AddTimeoutAction(this PlayMakerFSM control, FsmState state, string eventName, float timeout)
        {
            state.AddCustomAction(() => { control.StartTimeoutState(state.Name, eventName, timeout); });
        }

        public static void DisableKillFreeze(this GameObject gameObject)
        {
            var deathEffects = gameObject.GetComponentInChildren<EnemyDeathEffectsUninfected>(true);
            if (deathEffects != null)
            {
                deathEffects.doKillFreeze = false;
            }
        }

        public static void StartTimeoutState(this PlayMakerFSM control, string currentState, string endEvent, float timeout)
        {
            control.StartCoroutine(TimeoutState(control, currentState, endEvent, timeout));
        }

        public static IEnumerator TimeoutState(PlayMakerFSM control, string currentState, string endEvent, float timeout)
        {
            while (control.ActiveStateName == currentState)
            {
                timeout -= Time.deltaTime;

                if (timeout <= 0f)
                {
                    control.SendEvent(endEvent);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }

        public static void ChangeRandomIntRange(this PlayMakerFSM fsm, string stateName, int min, int max)
        {
            fsm.GetState(stateName).GetFirstActionOfType<RandomInt>().min.Value = min;
            fsm.GetState(stateName).GetFirstActionOfType<RandomInt>().max.Value = max;
        }

        public static void ChangeRandomIntRange(this FsmState state, int min, int max)
        {
            state.GetFirstActionOfType<RandomInt>().min.Value = min;
            state.GetFirstActionOfType<RandomInt>().max.Value = max;
        }

        public static void SetAudioOneShotVolume(this PlayMakerFSM fsm, string stateName, float vol = 0f)
        {
            fsm.GetState(stateName).GetFirstActionOfType<AudioPlayerOneShotSingle>().volume = vol;
        }

        public static void SetAudioOneShotVolume(this FsmState state, float vol = 0f)
        {
            state.GetFirstActionOfType<AudioPlayerOneShotSingle>().volume = vol;
        }

        /// <summary>
        /// WARNING: will remove ALL previous actions on the state
        /// </summary>
        public static void OverrideState(this PlayMakerFSM fsm, string stateName, Action stateAction)
        {
            var overrideState = fsm.GetState(stateName);
            overrideState.Actions = new FsmStateAction[] {
                new CustomFsmAction(stateAction)
            };
        }

        public static void DisableActions(this PlayMakerFSM fsm, params (string, int)[] statesActions)
        {
            foreach (var i in statesActions)
            {
                fsm.GetState(i.Item1).DisableAction(i.Item2);
            }
        }

        public static void DisableActions(this FsmState state, params int[] indices)
        {
            foreach (int i in indices)
            {
                state.DisableAction(i);
            }
        }

        public static void StartTrailEffectSpawns(this MonoBehaviour mb, int count, float spawnRate, string entityToSpawn)
        {
            mb.StartCoroutine(TrailEffectSpawns(mb.gameObject, count, spawnRate, entityToSpawn));
        }

        public static IEnumerator TrailEffectSpawns(GameObject gameObject, int count, float spawnRate, string entityToSpawn)
        {
            for (int i = 0; i < count; ++i)
            {
                EnemyRandomizerDatabase.CustomSpawnWithLogic(gameObject.transform.position, entityToSpawn, null, true);
                yield return new WaitForSeconds(spawnRate);
            }
        }

        public static void StartTrailEffectSpawns(this MonoBehaviour mb, int count, float spawnRate, Func<GameObject> spawner)
        {
            mb.StartCoroutine(TrailEffectSpawns(mb.gameObject, count, spawnRate, spawner));
        }

        public static IEnumerator TrailEffectSpawns(GameObject gameObject, int count, float spawnRate, Func<GameObject> spawner)
        {
            for (int i = 0; i < count; ++i)
            {
                var result = spawner.Invoke();
                result.transform.position = gameObject.transform.position;
                result.SetActive(true);
                yield return new WaitForSeconds(spawnRate);
            }
        }

        public static void StopPhysicsBody(this GameObject gameObject)
        {
            var PhysicsBody = gameObject.GetComponent<Rigidbody2D>();
            if (PhysicsBody == null)
                return;

            var body = PhysicsBody;
            if (body != null)
            {
                body.velocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        public static bool CanSeePlayer(this GameObject gameObject)
        {
            HeroController instance = HeroController.instance;
            if (instance == null)
            {
                return false;
            }
            Vector2 vector = gameObject.transform.position;
            Vector2 vector2 = instance.transform.position;
            Vector2 vector3 = vector2 - vector;
            if (Physics2D.Raycast(vector, vector3.normalized, vector3.magnitude, 256))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CanSeePlayer(this Vector2 losOrigin)
        {
            HeroController instance = HeroController.instance;
            if (instance == null)
            {
                return false;
            }
            Vector2 vector = losOrigin;
            Vector2 vector2 = instance.transform.position;
            Vector2 vector3 = vector2 - vector;
            if (Physics2D.Raycast(vector, vector3.normalized, vector3.magnitude, 256))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int GetRandomValueBetween(int min, int max)
        {
            RNG rng = new RNG();
            rng.Reset();
            return rng.Rand(min, max);
        }


        public static GameObject AddParticleEffect_TorchFire(this GameObject gameObject, int fireSize = 5, Transform customParent = null)
        {
            GameObject effect = SpawnerExtensions.SpawnEntityAt("Fire Particles", gameObject.transform.position, false);
            if (customParent == null)
                effect.transform.parent = gameObject.transform;
            else
                effect.transform.parent = customParent;

            effect.transform.localPosition = Vector3.zero;
            var pe = effect.GetComponent<ParticleSystem>();
            pe.startSize = fireSize;
            pe.simulationSpace = ParticleSystemSimulationSpace.World;
            effect.SafeSetActive(true);
            return effect;
        }

        public static GameObject AddParticleEffect_TorchShadeEmissions(this GameObject gameObject)
        {
            GameObject effect = SpawnerExtensions.SpawnEntityAt("Particle System B", gameObject.transform.position);
            effect.transform.parent = gameObject.transform;
            effect.transform.localPosition = Vector3.zero;
            effect.SafeSetActive(true);
            return effect;
        }

        public static ParticleSystem AddParticleEffect_WhiteSoulEmissions(this GameObject gameObject, Color? customColor = null)
        {
            var glow = SpawnerExtensions.SpawnEntityAt("Summon", gameObject.transform.position);
            var ge = glow.GetComponent<ParticleSystem>();
            glow.transform.parent = gameObject.transform;
            glow.transform.localPosition = Vector3.zero;
            ge.simulationSpace = ParticleSystemSimulationSpace.World;
            ge.startSize = 3;
            if (customColor != null)
                ge.startColor = customColor.Value;
            glow.SetActive(true);
            return ge;
        }


        public static void InsertHiddenState(this PlayMakerFSM fsm, string hiddenStateName,
            string preHideStateName, string preHideTransitionEventName,
            string postHideStateName, bool createNewPreTransitionEvent = false)
        {
            try
            {
                var preHideState = fsm.GetState(preHideStateName);
                var hidden = fsm.AddState(hiddenStateName);

                fsm.AddVariable<FsmBool>("IsAggro");
                var isAggro = fsm.FsmVariables.GetFsmBool("IsAggro");
                isAggro.Value = false;

                if (createNewPreTransitionEvent)
                {
                    preHideState.RemoveTransition(preHideTransitionEventName);
                    preHideState.AddTransition("FINISHED", hiddenStateName);

                    var waitAction = preHideState.Actions.OfType<Wait>().LastOrDefault();
                    if (waitAction == null)
                        preHideState.AddAction(new Wait() { finishEvent = new FsmEvent("FINISHED") });
                    else
                        waitAction.finishEvent = new FsmEvent("FINISHED");
                }
                else
                {
                    //change the state to stall the FSM until a player is nearby
                    preHideState.ChangeTransition(preHideTransitionEventName, hiddenStateName);
                }

                fsm.AddTransition(hiddenStateName, "SHOW", postHideStateName);
                hidden.AddAction(new BoolTest() { boolVariable = isAggro, isTrue = new FsmEvent("SHOW"), everyFrame = true });

                //FSMsUsingHiddenStates.Add(fsm);
            }
            catch (Exception e) { Dev.Log($"Error in adding an aggro range state to FSM:{fsm.name} PRE STATE:{preHideStateName} EVENT:{preHideTransitionEventName} POST-STATE:{postHideStateName}"); }
        }

        public static void SetRotation(this GameObject gameObject, float angle)
        {
            gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, angle);
        }


        public static void SetSpriteDirection(this GameObject gameObject, bool left, bool baseIsFlipped = false)
        {
            Vector3 localScale = gameObject.transform.localScale;
            float xScale = gameObject.transform.localScale.x;

            if(baseIsFlipped)
            {
                if (!left)
                {
                    if (xScale > 0)
                        xScale = -xScale;
                }
                else
                {
                    if (xScale < 0)
                        xScale = -xScale;
                }
            }
            else
            {
                if (left)
                {
                    if (xScale > 0)
                        xScale = -xScale;
                }
                else
                {
                    if (xScale < 0)
                        xScale = -xScale;
                }
            }

            localScale.x = xScale;

            gameObject.transform.localScale = localScale;
        }


        public static void FinalizeReplacement(this GameObject gameObject, GameObject objectToReplace = null)
        {
            //should not be null..
            var thisMetaData = ObjectMetadata.Get(gameObject);

            //must be null!
            var originalMetaData = ObjectMetadata.GetOriginal(gameObject);

            //this case can happen if, for example, some enemies are pre-loaded, then disabled and re-enabled later
            if(thisMetaData != null && originalMetaData != null)
            {
                //since preloading is a valid case, let's just warn that this is happening in case it's expected behaviour
                Dev.LogWarning($"{thisMetaData}: Cannot re-replace an object that's already replaced something!");
                return;
                //throw new InvalidOperationException($"{thisMetaData}: Cannot re-replace an object that's already replaced something!");
            }

            if(thisMetaData == null)
            {
                throw new InvalidOperationException($"{gameObject}: Error! This object hasn't been setup yet! No metadata or enemy randomizer component found!");
            }

            //the meta of the object we want to replace, should not be null?
            var otherMetaData = ObjectMetadata.Get(objectToReplace);
            if(objectToReplace != null && otherMetaData == null)
            {
                otherMetaData = new ObjectMetadata(objectToReplace);
            }

            if (SpawnedObjectControl.VERBOSE_DEBUG && objectToReplace != null)
                Dev.LogWarning($"{gameObject}: Will be activated and will replace [{objectToReplace}]");
            else
                Dev.LogWarning($"{gameObject}: Will be activated and will replace nothing");

            //nothing to do here if these objects are the same
            if (objectToReplace != null && gameObject != objectToReplace)
            {
                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.LogWarning($"{thisMetaData}: The replacement object {otherMetaData} is both not null and unique, so a replacement will be performed");

                var oedf = objectToReplace.GetDeathEffects();
                var nedf = gameObject.GetDeathEffects();
                if (oedf != null && nedf != null)
                {
                    string oplayerDataName = oedf.GetPlayerDataNameFromDeathEffects();

                    thisMetaData.playerDataName = oplayerDataName;

                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.LogWarning($"{thisMetaData}: The replacement object player data name {oplayerDataName} from the death effects will be applied to this object");

                    nedf.SetPlayerDataNameFromDeathEffects(oplayerDataName);
                }

                if (thisMetaData.playerDataName == null && gameObject.ObjectType() == PrefabObject.PrefabType.Enemy)
                {
                    thisMetaData.playerDataName = string.Empty;

                    string result = GetCustomPlayerDataName(objectToReplace);

                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.LogWarning($"{thisMetaData}: The custom player data name {result} will set/updated used this object is destroyed");

                    if (!string.IsNullOrEmpty(result))
                    {
                        //this sets custom to true
                        thisMetaData.PlayerDataName = result;
                    }
                }

                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.LogWarning($"{thisMetaData}: Notifying any global observers of object replacement");

                EnemyRandomizerDatabase.OnObjectReplaced?.Invoke((gameObject, objectToReplace));
            }

            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.LogWarning($"{thisMetaData}: Enabling the new game object");

            //finally link the metadatas
            var soc = gameObject.GetComponent<SpawnedObjectControl>();
            if(soc != null)
            {
                if (objectToReplace == null)
                {
                    //link to itself -- this object was finalized with no replacement, so prevent it from ever being replaced
                    otherMetaData = thisMetaData;
                }
                soc.originialMetadata = otherMetaData;

                //apply the position logic
                soc.SetPositionOnSpawn();
            }

            //turn it on! (this will trigger the randomizer again but should bypass now that the originialMetadata field is set)
            gameObject.SafeSetActive(true);

            //nothing to do here if these objects are the same
            if (objectToReplace != null && gameObject != objectToReplace)
            {
                Dev.Log($"{thisMetaData} is replacing {otherMetaData} and so {objectToReplace} will now be destroyed...");

                //boom
                SpawnerExtensions.DestroyObject(objectToReplace);

                if (SpawnedObjectControl.VERBOSE_DEBUG)
                    Dev.LogWarning($"{thisMetaData}: DestroyObject has completed and the object that generated {otherMetaData} is gone.");
            }

            if (SpawnedObjectControl.VERBOSE_DEBUG)
                Dev.LogWarning($"{thisMetaData}: Completed replacing {otherMetaData} with {thisMetaData}.");

            //finally mark the spawned object as officially loaded
            if(soc != null)
            {
                soc.MarkLoaded();
            }
        }

        public static GameObject SpawnEnemyForEnemySpawner(Vector2 pos, bool setActive = false, string originalEnemy = null, RNG rng = null)
        {
            GameObject enemy = null;
            string enemyToSpawn = null;
            try
            {
                enemyToSpawn = SpawnerExtensions.GetRandomPrefabNameForSpawnerEnemy(rng);
                enemy = SpawnerExtensions.SpawnEntityAt(enemyToSpawn, pos, false, false);
                if (enemy != null)
                {
                    var soc = enemy.GetComponent<SpawnedObjectControl>();
                    if (soc != null)
                    {
                        soc.placeGroundSpawnOnGround = false;
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnEnemyForEnemySpawner when trying to spawn {enemyToSpawn} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            try
            {
                if (enemy != null && !string.IsNullOrEmpty(originalEnemy) && !string.IsNullOrEmpty(enemyToSpawn))
                {
                    float sizeScale = SpawnerExtensions.GetRelativeScale(enemyToSpawn, originalEnemy);
                    if (!Mathnv.FastApproximately(sizeScale, 1f, 0.01f))
                    {
                        enemy.ScaleObject(sizeScale);
                        enemy.ScaleAudio(sizeScale);//might not need this....

                        var soc2 = enemy.GetComponent<DefaultSpawnedEnemyControl>();
                        if (soc2 != null)
                        {
                            var nmax = SpawnerExtensions.OriginalPrefabHP(originalEnemy);
                            if (nmax > 1)
                            {
                                soc2.MaxHP = nmax;
                                soc2.CurrentHP = soc2.MaxHP;
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnEnemyForEnemySpawner when trying to scale {enemyToSpawn} to match {originalEnemy} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            if (setActive)
                enemy.SafeSetActive(true);
            
            return enemy;
        }

        //public static bool ResolveInsideWalls(this GameObject gameObject)
        //{
        //    if (!gameObject.InBounds())
        //    {
        //        var oobPos = gameObject.transform.position;
        //        var origin = HeroController.instance.transform.position;
        //        var emergencyCorrectionDir = (oobPos - origin).normalized;
        //        var ray = Mathnv.GetRayOn(origin, emergencyCorrectionDir, float.MaxValue, SpawnerExtensions.IsSurfaceOrPlatform);

        //        if (SpawnedObjectControl.VERBOSE_DEBUG)
        //            Dev.LogWarning($"{gameObject}: Was out of bounds at {oobPos} and will be moved to {ray.point} - For reference, the hero is at {origin}.");

        //        gameObject.transform.position = ray.point;
        //        //previousLocation = transform.position;
        //        return true;
        //    }

        //    if (gameObject.IsInsideWalls())
        //    {
        //        var oobPos = gameObject.transform.position;
        //        var rayOutOfWalls = gameObject.GetNearstRayOutOfWalls();
        //        var direction = -rayOutOfWalls.normal;
        //        var pointOnWall = rayOutOfWalls.point;
        //        var vectorOutOfWall = Vector2.Dot(direction, gameObject.GetOriginalObjectSize()) * direction * 0.5f;
        //        var outOfWall = pointOnWall + vectorOutOfWall;

        //        if (SpawnedObjectControl.VERBOSE_DEBUG)
        //            Dev.LogWarning($"{gameObject}: Was inside walls at {oobPos} and will be moved to the wall at point {pointOnWall} and then out of the wall by {vectorOutOfWall} to {outOfWall} - For reference, the hero is at {HeroController.instance.transform.position}.");

        //        gameObject.transform.position = outOfWall;
        //        return true;
        //        //previousLocation = transform.position;
        //    }
        //    return false;
        //}
    }
}
