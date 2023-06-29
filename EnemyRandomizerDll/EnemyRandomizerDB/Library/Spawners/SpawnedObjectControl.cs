using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using System.Collections;
using Modding;

namespace EnemyRandomizerMod
{
    public class SpawnedObjectControl : MonoBehaviour, IExtraDamageable, IHitResponder
    {
#if DEBUG
        public static bool VERBOSE_DEBUG = true;
#else
        public static bool VERBOSE_DEBUG = false;
#endif

        public ObjectMetadata thisMetadata;
        public ObjectMetadata originialMetadata;

        public bool loaded { get; protected set; }

        public virtual void MarkLoaded() { loaded = true; }

        /// <summary>
        /// override to autmatically set the control fsm used by this enemy in setup
        /// </summary>
        public virtual string FSMName => "Control";
        protected virtual string FSMHiddenStateName => "_Hidden_";
        //protected virtual string FSMHiddenStateFrom => null;
        //protected virtual string FSMHiddenStateFromEvent => null;
        //protected virtual string FSMHiddenStateTo => null;

        PlayMakerFSM _internal_control;
        public virtual PlayMakerFSM control
        {
            get
            {
                if (_internal_control != null)
                    return _internal_control;

                try
                {
                    if (_internal_control == null && !string.IsNullOrEmpty(FSMName))
                        _internal_control = gameObject.LocateMyFSM(FSMName);
                }
                catch(Exception)
                {
                    _internal_control = null;
                }

                return _internal_control;
            }
            set
            {
                _internal_control = value;
            }
        }

        public static IEnumerable<SpawnedObjectControl> GetAll => GameObject.FindObjectsOfType<SpawnedObjectControl>();
        public static IEnumerable<SpawnedObjectControl> GetAllBattle => GetAll.Where(x => SpawnerExtensions.IsBattleEnemy(x.gameObject));
        public static IEnumerable<SpawnedObjectControl> GetAllEnemies => GetAll.Where(x => SpawnerExtensions.ObjectType(x.gameObject) == PrefabObject.PrefabType.Enemy);

        public bool IsInColo()
        {
            return GameManager.instance.GetCurrentMapZone() == "COLOSSEUM";
        }

        public float sizeScale = 1f;
        public float SizeScale
        {
            get => sizeScale;
            set => sizeScale = value;
        }

        public virtual int Geo
        {
            get => 0;
            set { }
        }

        public int DamageDealt
        {
            get => HeroDamage == null ? 0 : HeroDamage.damageDealt;
            set
            {
                if (HeroDamage != null)
                    HeroDamage.damageDealt = value;
            }
        }

        public int EnemyDamageDealt
        {
            get => EnemyDamage == null ? 0 : EnemyDamage.damageDealt;
            set
            {
                if (EnemyDamage != null)
                    EnemyDamage.damageDealt = value;
            }
        }

        public EntitySpawner ChildController => gameObject.GetOrAddComponent<EntitySpawner>();
        public EnemyDreamnailReaction DreamnailReaction => gameObject.GetComponent<EnemyDreamnailReaction>();
        public HealthManager EnemyHealthManager => gameObject.GetComponent<HealthManager>();
        public tk2dSprite Sprite => gameObject.GetComponent<tk2dSprite>();
        public tk2dSpriteAnimator Animator => gameObject.GetComponent<tk2dSpriteAnimator>();
        public DamageHero HeroDamage => gameObject.GetComponent<DamageHero>();
        public DamageEnemies EnemyDamage => gameObject.GetComponent<DamageEnemies>();
        public Walker Walker => gameObject.GetComponent<Walker>();
        public TinkEffect Tinker => gameObject.GetComponent<TinkEffect>();
        public EnemyDeathEffects DeathEffects => gameObject.GetComponent<EnemyDeathEffects>();
        public Rigidbody2D PhysicsBody => gameObject.GetComponent<Rigidbody2D>();
        public Collider2D Collider => gameObject.GetComponent<Collider2D>();
        public MeshRenderer MRenderer => gameObject.GetComponent<MeshRenderer>();
        public PreInstantiateGameObject PreInstantiatedGameObject => gameObject.GetComponent<PreInstantiateGameObject>();
        public virtual Vector2 pos2d => gameObject.transform.position.ToVec2();
        public virtual Vector2 pos2dWithOffset => gameObject.transform.position.ToVec2() + new Vector2(0, 0.5f);
        public virtual Vector2 heroPos2d => HeroController.instance.transform.position.ToVec2();
        public virtual Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 0.5f);
        public virtual float floorY => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.down, 200f).point.y;
        public virtual float roofY => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.up, 50f).point.y;
        public virtual float edgeL => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.left, 50f).point.x;
        public virtual float edgeR => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.right, 50f).point.x;

        protected bool didOnHitThisFrame = false;
        protected virtual void OnHit(int dmg) { }
        protected virtual void OnHeroInAggroRangeTheFirstTime() { }

        public bool needsFinalize = true;
        //set this to false when spawning an enemy that you want to "fall" in
        public bool placeGroundSpawnOnGround = true;
        public virtual bool isInvincible => false;
        public virtual bool takeCustomNailDamage => false;
        public virtual bool spawnOrientationIsFlipped => false;
        public virtual float spawnPositionOffset => 0.53f;
        public float? spawnPositionOffsetOverride = null;
        protected float spawnPosOffsetToUse => spawnPositionOffsetOverride == null ?
            spawnPositionOffset : spawnPositionOffsetOverride.Value;
        public virtual bool spawnShouldStickCorpse => false;
        public virtual bool useCustomPositonOnSpawn => false;
        public virtual bool preventOutOfBoundsAfterPositioning => false;
        public virtual bool preventInsideWallsAfterPositioning => false;
        public virtual bool explodeOnDeath => false;
        public virtual string explodeOnDeathEffect => "Gas Explosion M";
        public virtual string spawnEntityOnDeath => null;
        public virtual bool hasCustomDreamnailReaction => gameObject.GetComponent<EnemyDreamnailReaction>() != null;
        public virtual string customDreamnailSourceName { get => originialMetadata == null ? "meme" : originialMetadata.GetDatabaseKey(); }
        public virtual string customDreamnailText { get => $"In another dream, I was a {customDreamnailSourceName}..."; }
        public virtual string _internal_customDreamnailKey { get; protected set; }
        public virtual string customDreamnailKey
        {
            get
            {
                if(_internal_customDreamnailKey == null)
                    _internal_customDreamnailKey = Guid.NewGuid().ToString();
                return _internal_customDreamnailKey;
            }
            set
            {
                _internal_customDreamnailKey = value;
            }
        }


        protected bool isUnloading = false;

        /// <summary>
        /// change this to stop the emission of a corpse completely
        /// </summary>
        protected virtual bool emitCorpse => true;

        protected virtual void Awake()
        {
            GameManager.instance.UnloadingLevel -= SetUnloading;
            GameManager.instance.UnloadingLevel += SetUnloading;

            On.EnemyDeathEffects.EmitCorpse -= EnemyDeathEffects_EmitCorpse;
            On.EnemyDeathEffects.EmitCorpse += EnemyDeathEffects_EmitCorpse;


        }

        protected virtual void BeforeCorpseEmit(GameObject corpseObject, float? attackDirection, bool isWatery, bool spellBurn)
        {
            //TODO: custom logic here
        }

        protected virtual void AfterCorpseEmit(GameObject corpseObject, float? attackDirection, bool isWatery, bool spellBurn)
        {
            //TODO: custom logic here
        }

        private void EnemyDeathEffects_EmitCorpse(On.EnemyDeathEffects.orig_EmitCorpse orig, EnemyDeathEffects self, float? attackDirection, bool isWatery, bool spellBurn)
        {
            //this is a global hook, so only run it for the related game object
            if (gameObject != self.gameObject)
            {
                //Dev.Log(gameObject.GetSceneHierarchyPath() + " != " + self.gameObject.GetSceneHierarchyPath());
                orig(self, attackDirection, isWatery, spellBurn);
                return;
            }

            if (VERBOSE_DEBUG)
                Dev.Log("Emitting corpse from " + self.gameObject.GetSceneHierarchyPath());

            GameObject corpse = null;
            try
            {
                if (emitCorpse)
                {
                    corpse = self.GetCorpseFromEDF();

                    if (VERBOSE_DEBUG)
                    {
                        if (corpse != null)
                            Dev.Log("Corpse of enemy: " + corpse.GetSceneHierarchyPath());
                        else
                            Dev.Log("Corpse is null -- nothing to do here");
                    }

                    if (corpse != null)
                        BeforeCorpseEmit(corpse, attackDirection, isWatery, spellBurn);

                    if (VERBOSE_DEBUG)
                        Dev.Log("Invoking original emission function");

                    orig(self, attackDirection, isWatery, spellBurn);

                    if (corpse != null)
                        AfterCorpseEmit(corpse, attackDirection, isWatery, spellBurn);

                    if (VERBOSE_DEBUG)
                        Dev.Log("Finished emit corpse....");
                }
                else
                {
                    if (VERBOSE_DEBUG)
                        Dev.Log("Corpse emission disabled for: " + self.gameObject.GetSceneHierarchyPath());

                    if (!isUnloading)
                        SpawnerExtensions.SpawnEntityAt("Death Puff Med", self.transform.position, null, true, false);
                }
            }
            catch(Exception e)
            {
                Dev.LogError($"Caught unhandled exception in an EmitCorpse Hook for {gameObject} with corpse {corpse} {e.Message}\n{e.StackTrace}");
            }
        }

        void SetUnloading()
        {
            isUnloading = true;
        }

        public virtual void Setup(GameObject objectThatWillBeReplaced = null)
        {
            if (isUnloading)
                return;

            try
            {
                //if this object doesn't yet have a meta object, make one
                if(thisMetadata == null)
                    thisMetadata = new ObjectMetadata(gameObject);

                if (objectThatWillBeReplaced != null)
                {
                    if (ObjectMetadata.GetOriginal(objectThatWillBeReplaced) != null)
                        throw new InvalidOperationException("Cannot replace a replacement object!");

                    if (ObjectMetadata.GetOriginal(gameObject) != null)
                        throw new InvalidOperationException("Cannot setup this object since it has already replaced something");

                    //get the meta object from the given potential replacement, if it doesn't have one, generate one
                    var originalObjectMeta = ObjectMetadata.Get(objectThatWillBeReplaced);
                    if (originalObjectMeta == null)
                        originalObjectMeta = new ObjectMetadata(objectThatWillBeReplaced);

                    //hold off on doing this assignment until the end of the enemy replacer
                    //unless we're in colo, which does things differently...
                    if (IsInColo())
                    {
                        originialMetadata = originalObjectMeta;
                    }

                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        Dev.Log($"Attempting Setup for {thisMetadata} with {originalObjectMeta}");
                }
                else
                {
                    if (SpawnedObjectControl.VERBOSE_DEBUG)
                        //there is no "replacement"
                        Dev.Log($"Attempting Setup for {thisMetadata} with no replacement given");
                }
            }
            catch (Exception e)
            {
                if (objectThatWillBeReplaced != null)
                {
                    Dev.Log($"{this}:{this.thisMetadata}: Caught exception in metadata creation with {objectThatWillBeReplaced}:{new ObjectMetadata(objectThatWillBeReplaced)} ERROR:{e.Message} STACKTRACE{e.StackTrace}");
                }
                else
                {
                    Dev.Log($"{this}:{this.thisMetadata}: Caught exception in metadata creation without replacement ERROR:{e.Message} STACKTRACE{e.StackTrace}");
                }
            }

            try
            {
                if (control == null && !string.IsNullOrEmpty(FSMName))
                {
                    control = gameObject.LocateMyFSM(FSMName);

                    if (control == null && VERBOSE_DEBUG)
                        Dev.LogError($"Failed to locate my fsm: {FSMName}");
                }
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in if-statement block (control) ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                SetDreamnailInfo();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetDreamnailInfo ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                SetInvincibleState();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetInvincibleState ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                InitPositionOnSetupPreSpawn(objectThatWillBeReplaced);
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in InitPositionOnSetupPreSpawn ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                if (gameObject.IsVisible())
                    SetPositionOnSpawn(objectThatWillBeReplaced);
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetPositionOnSpawn ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            DisableCollidersForBackgroundThings();
        }

        protected virtual void InitPositionOnSetupPreSpawn(GameObject objectThatWillBeReplaced)
        {
            if (objectThatWillBeReplaced != null)
            {
                transform.position = objectThatWillBeReplaced.transform.position;
            }
        }

        protected virtual void SetInvincibleState()
        {
            if (EnemyHealthManager != null)
            {
                EnemyHealthManager.IsInvincible = isInvincible;
            }
        }

        protected virtual void DisableCollidersForBackgroundThings()
        {
            string zone = GameManager.instance.GetCurrentMapZone();
            int zoneScale = MetaDataTypes.ProgressionZoneScale[zone];

            if(zoneScale == MetaDataTypes.ProgressionZoneScale["FOG_CANYON"] 
                || zoneScale == MetaDataTypes.ProgressionZoneScale["MONOMON_ARCHIVE"]
                || zoneScale == MetaDataTypes.ProgressionZoneScale["ROYAL_GARDENS"])
            {
                string currentScene = gameObject.SceneName();

                if(transform.position.z > 10f || currentScene == "Fungus3_50")
                {
                    var setz = gameObject.GetComponent<SetZ>();
                    if(setz != null)
                    {
                        GameObject.Destroy(setz);
                    }

                    if(currentScene != "Fungus3_50" || transform.position.z < 12f)
                    {
                        bool needsAdjust = GetComponents<Collider2D>().Any(x => x.enabled);
                        if (needsAdjust)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, 12f + UnityEngine.Random.Range(0f, 4f));
                            GetComponentsInChildren<Collider2D>(true).ToList().ForEach(x => x.enabled = false);
                            GetComponentsInChildren<DamageHero>(true).ToList().ForEach(x => GameObject.Destroy(x));
                        }
                    }
                    else
                    {
                        GetComponentsInChildren<Collider2D>(true).ToList().ForEach(x => x.enabled = false);
                        GetComponentsInChildren<DamageHero>(true).ToList().ForEach(x => GameObject.Destroy(x));
                    }
                }
            }
        }

        //protected virtual void CorrectInsideWallPosition()
        //{
        //    if(gameObject.IsInsideWalls())
        //    {
        //        var cardinal = gameObject.GetCardinalRays(float.MaxValue);
        //        var shortest = cardinal.OrderBy(x => x.distance).FirstOrDefault();
        //        var direction = -shortest.normal;
        //        gameObject.transform.position = shortest.point + Vector2.Dot(direction, gameObject.GetOriginalObjectSize()) * direction;
        //    }
        //}

        protected virtual void StickToRoof()
        {
            gameObject.StickToRoof(spawnPosOffsetToUse, spawnOrientationIsFlipped);
        }

        protected virtual void StickToSurface()
        {
            gameObject.StickToClosestSurface(float.MaxValue, spawnPosOffsetToUse, spawnShouldStickCorpse, spawnOrientationIsFlipped);
        }

        protected virtual void DoWallCling()
        {
            gameObject.StickToClosestSurfaceWithoutRotation(float.MaxValue, spawnPosOffsetToUse);
        }

        //protected virtual void PlaceInsideGround()
        //{
        //    var nearest = gameObject.GetNearestRayOnSurface(float.MaxValue);
        //    gameObject.transform.position = gameObject.transform.position - nearest.normal

        //    gameObject.StickToGroundX(-spawnPositionOffset);
        //}

        protected virtual void PlaceOnGround()
        {
            gameObject.StickToGroundX(spawnPosOffsetToUse);
        }

        public virtual void SetPositionOnSpawn(GameObject objectThatWillBeReplaced)
        {
            if (isUnloading)
                return;

            PreSetSpawnPosition(objectThatWillBeReplaced);

            DoSetSpawnPosition(objectThatWillBeReplaced);

            AddPositionLogicFixers(objectThatWillBeReplaced);

            OnSetSpawnPosition(objectThatWillBeReplaced);
        }

        protected virtual void PreSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            var poob = gameObject.GetComponent<PreventOutOfBounds>();
            if (poob)
            {
                GameObject.Destroy(poob);
            }

            var piw = gameObject.GetComponent<PreventInsideWalls>();
            if (piw)
            {
                GameObject.Destroy(piw);
            }
        }


        protected virtual void DoSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {
            //then, place it "on the ground" or whereever it should be
            if (useCustomPositonOnSpawn)
                SetCustomPositionOnSpawn();
            else
                SetDefaultPosition();
        }

        protected virtual void AddPositionLogicFixers(GameObject objectThatWillBeReplaced)
        {
            //try this resolution before finalizing their placement with "prevent out of bounds"
            //if (gameObject.IsInsideWalls())
            //    gameObject.ResolveInsideWalls();

            if (!gameObject.IsInGroundEnemy() && !gameObject.CheckIfIsPogoLogicType())
            {
                if (preventOutOfBoundsAfterPositioning)
                    gameObject.GetOrAddComponent<PreventOutOfBounds>();

                if (preventInsideWallsAfterPositioning)
                    gameObject.GetOrAddComponent<PreventInsideWalls>();
            }
        }

        protected virtual void OnSetSpawnPosition(GameObject objectThatWillBeReplaced)
        {

        }

        /// <summary>
        /// a custom override for how to position this enemy
        /// </summary>
        protected virtual void SetCustomPositionOnSpawn()
        {
            if (!gameObject.IsFlying())
                gameObject.StickToGroundX(spawnPosOffsetToUse);
        }

        protected virtual GameObject SpawnChildForEnemySpawner(Vector2 pos, bool setActive = false, string originalEnemy = null, string storageVar = null, RNG rng = null)
        {
            GameObject enemy = null;
            string enemyToSpawn = null;
            try
            {
                enemyToSpawn = SpawnerExtensions.GetRandomPrefabNameForSpawnerEnemy(rng);
                enemy = ChildController.SpawnAndTrackChild(enemyToSpawn, transform.position, originalEnemy, setActive, false);
                if (enemy != null)
                {
                    var soc = enemy.GetComponent<SpawnedObjectControl>();
                    if (soc != null)
                    {
                        soc.placeGroundSpawnOnGround = false;
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnChildEnemy in {control} when trying to spawn {enemyToSpawn} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            try
            {
                if (enemy != null && !string.IsNullOrEmpty(originalEnemy) && !string.IsNullOrEmpty(enemyToSpawn))
                {
                    float sizeScale = SpawnerExtensions.GetRelativeScale(enemyToSpawn, originalEnemy);
                    if (!Mathnv.FastApproximately(sizeScale, 1f, 0.01f))
                    {
                        enemy.ScaleObject(sizeScale);
                        enemy.ScaleAudio(sizeScale);//might not need this....
                    }
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnChildEnemy in {control} when trying to scale {enemyToSpawn} to match {originalEnemy} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }

            try
            {
                if (enemy != null && !string.IsNullOrEmpty(storageVar))
                {
                    control.FsmVariables.GetFsmGameObject(storageVar).Value = enemy;
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught in SpawnChildEnemy in {control} when trying to set fsm var storageVar ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }
            return enemy;
        }

        /// <summary>
        /// the default way to determine how to place this enemy
        /// </summary>
        protected virtual void SetDefaultPosition()
        {
            if(IsRoofEnemy())
            {
                PositionRoofEnemy();
            }
            else if(IsSpecialWallClingEnemy())
            {
                PositionSpecialWallClingEnemy();
            }
            else if (IsClimbingEnemy())
            {
                PositionClimbingEnemy();
            }
            else if (IsStaticEnemy())
            {
                PositionStaticEnemy();
            }
            else if (IsGroundEnemy())
            {
                PositionGroundEnemy();
            }
            else
            {
                PositionFlyingEnemy();
            }
        }

        protected virtual bool IsRoofEnemy()
        {
            return name.Contains("Ceiling Dropper");
        }

        protected virtual bool IsSpecialWallClingEnemy()
        {
            //TODO: add the ruin javlin sentry that clings to walls
            return name.Contains("Mantis Flyer Child");
        }

        protected virtual void PositionRoofEnemy()
        {
            StickToRoof();
        }

        protected virtual void PositionSpecialWallClingEnemy()
        {
            StickToSurface();
        }

        protected virtual bool IsClimbingEnemy()
        {
            return gameObject.IsClimbing() || gameObject.IsClimbingFromComponents();
        }

        protected virtual bool IsStaticEnemy()
        {
            return !gameObject.IsMobile();
        }

        protected virtual bool IsGroundEnemy()
        {
            return !gameObject.IsFlying() && !gameObject.IsFlyingFromComponents();
        }

        protected virtual void PositionClimbingEnemy()
        {
            gameObject.StickToClosestSurface(float.MaxValue, extraOffsetScale: spawnPosOffsetToUse, alsoStickCorpse: spawnShouldStickCorpse, flipped: spawnOrientationIsFlipped);
        }

        protected virtual void PositionStaticEnemy()
        {
            gameObject.StickToClosestSurface(float.MaxValue, extraOffsetScale: spawnPosOffsetToUse, alsoStickCorpse: spawnShouldStickCorpse, flipped: spawnOrientationIsFlipped);
        }

        protected virtual void PositionGroundEnemy()
        {
            if (gameObject.IsInGroundEnemy())
            {
                PositionInGroundEnemy();
            }
            else
            {
                PositionOnGroundEnemy();
            }
        }

        protected virtual void PositionInGroundEnemy()
        {
            var rays = SpawnerExtensions.GetOctagonalRays(gameObject, 100f).Where(x => x.normal.y > 0 && x.collider != null);
            if (rays.Count() <= 0)
            {
                //no valid ground to spawn no, just die
                gameObject.KillObjectNow();
            }
            else
            {
                //try some rays under us
                RNG rng = new RNG();
                rng.Reset();
                var raylist = rays.ToList();
                var random = raylist.GetRandomElementFromList(rng);
                transform.position = random.point + Vector2.up * 2f;
            }

            //now place on ground
            gameObject.StickToGroundX(spawnPosOffsetToUse);
        }

        protected virtual void PositionOnGroundEnemy()
        {
            //should this ground enemy be allowed to fall from where it spawns or attempt to be stuck to some ground under it
            if (placeGroundSpawnOnGround)
            {
                gameObject.StickToGroundX(spawnPosOffsetToUse);
            }
            else
            {
                //nothing, just spawn it and let it fall
            }
        }

        protected virtual void PositionFlyingEnemy()
        {
            //nothing they just end up where they are
        }


        protected virtual void OnDisable()
        {
            if (isUnloading)
                return;

            try
            {
                if (hasCustomDreamnailReaction)
                {
                    On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                    On.Language.Language.Get_string_string -= Language_Get_string_string;
                }
            }
            catch (Exception e) { Dev.LogError($"Exception caught on disable callback for {thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }
        }

        protected virtual void OnDestroy()
        {
            On.EnemyDeathEffects.EmitCorpse -= EnemyDeathEffects_EmitCorpse;
            GameManager.instance.UnloadingLevel -= SetUnloading;

            if (isUnloading)
                return;

            try
            {
                if (gameObject.IsInAValidScene())
                    DoValidSceneDestroyEvents();
            }
            catch (Exception e) { Dev.LogError($"Exception caught on destroy callback for {thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }
        }

        public bool CanSpawnInScene
        {
            get
            {
                if (isUnloading)
                    return false;


                bool canSpawn = false;
                try
                {
                    canSpawn = gameObject.IsInAValidScene() && 
                        ((this.thisMetadata.SceneName == EnemyRandomizerDatabase.GetBlackBorders().Value.FirstOrDefault().scene.name)
                        ||(this.thisMetadata.SceneName.TrimEnd("_boss") == EnemyRandomizerDatabase.GetBlackBorders().Value.FirstOrDefault().scene.name));
                }
                catch (Exception) { }
                
                return canSpawn;
            }
        }

        protected virtual void DoValidSceneDestroyEvents()
        {
            if (!CanSpawnInScene)
                return;

            if (explodeOnDeath && !string.IsNullOrEmpty(explodeOnDeathEffect))
            {
                ExplodeOnDeath();
            }

            if (!string.IsNullOrEmpty(spawnEntityOnDeath))
                SpawnEntityOnDeath();

            if(thisMetadata != null && thisMetadata.IsCustomPlayerDataName)
            {
                gameObject.RecordCustomJournalOnDeath();
            }
        }

        protected virtual void SetDreamnailInfo()
        {
            if (hasCustomDreamnailReaction)
            {
                On.Language.Language.Get_string_string -= Language_Get_string_string;
                On.Language.Language.Get_string_string += Language_Get_string_string;

                On.EnemyDreamnailReaction.SetConvoTitle -= EnemyDreamnailReaction_SetConvoTitle;
                On.EnemyDreamnailReaction.SetConvoTitle += EnemyDreamnailReaction_SetConvoTitle;

                SetDreamnailReactionToCustomText();
            }
        }

        private void EnemyDreamnailReaction_SetConvoTitle(On.EnemyDreamnailReaction.orig_SetConvoTitle orig, EnemyDreamnailReaction self, string title)
        {
            if (self != DreamnailReaction || DreamnailReaction == null)
            {
                orig(self, title);
            }
            else
            {
                orig(self, customDreamnailKey);
            }
        }

        protected virtual void SetDreamnailReactionToCustomText()
        {
            if (DreamnailReaction != null)
            {
                try
                {
                    DreamnailReaction.GetType().GetField("convoTitle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(DreamnailReaction, customDreamnailKey);
                }
                catch (Exception e)
                {
                    Dev.LogError("Error settings custom dreamnail key for object " + gameObject.GetSceneHierarchyPath());
                }
            }
        }

        protected virtual string Language_Get_string_string(On.Language.Language.orig_Get_string_string orig, string key, string sheetTitle)
        {
            if (key.Contains(customDreamnailKey))
            {
                return customDreamnailText;//the actual custom text to return
            }
            else
            {
                return orig(key, sheetTitle);
            }
        }

        protected virtual void ExplodeOnDeath()
        {
            if (!explodeOnDeath || string.IsNullOrEmpty(explodeOnDeathEffect))
                return;

            if (!gameObject.activeInHierarchy || !gameObject.IsInAValidScene())
                return;

            var spawned = SpawnerExtensions.SpawnEntityAt(explodeOnDeathEffect, transform.position, null, false);
            if(spawned != null)
            {
                var metaInfo = new ObjectMetadata(spawned);
                //did this spawn into a different scene?
                if (metaInfo.SceneName != thisMetadata.SceneName)
                    GameObject.Destroy(spawned);
                else
                    spawned.SafeSetActive(true);
            }
        }

        //use to keep spawned enemies from dying instantly
        protected virtual IEnumerator MakeTempInvincible(HealthManager target)
        {
            bool wasinv = target.IsInvincible;
            target.IsInvincible = true;
            yield return new WaitForSeconds(0.2f);
            if(target != null)
                target.IsInvincible = wasinv;
        }

        protected virtual void SpawnEntityOnDeath()
        {
            if (string.IsNullOrEmpty(spawnEntityOnDeath))
                return;

            if (!gameObject.IsInAValidScene())
                return;

            var spawned = SpawnerExtensions.SpawnEntityAt(spawnEntityOnDeath, transform.position, null, false);
            if (spawned != null)
            {
                spawned.ScaleObject(SizeScale);
                spawned.ScaleAudio(SizeScale);

                //if this spawns an enemy, scale the hp to be similar
                if(spawned.ObjectType() == PrefabObject.PrefabType.Enemy)
                {
                    spawned.SetMaxHP(spawned.GetScaledMaxHP(thisMetadata.GetDatabaseKey()));

                    GameManager.instance.StartCoroutine(MakeTempInvincible(spawned.GetEnemyHealthManager()));
                }

                //undo for now
                //var metaInfo = new ObjectMetadata(spawned);
                ////did this spawn into a different scene?
                //if (metaInfo.SceneName != thisMetadata.SceneName)
                //{
                //    Dev.Log($"{gameObject} on death? {metaInfo.SceneName} != {thisMetadata.SceneName} destroying spawned thing");
                //    GameObject.Destroy(spawned);
                //}
                //else
                    spawned.SafeSetActive(true);
            }
        }

        public virtual void InsertHiddenState(string preHideStateName, string preHideTransitionEventName,
            string postHideStateName, bool createNewPreTransitionEvent = false)
        {
            if(control != null)
                InsertHiddenState(control, preHideStateName, preHideTransitionEventName, postHideStateName, createNewPreTransitionEvent);
        }

        public virtual void InsertHiddenState(PlayMakerFSM fsm,
            string preHideStateName, string preHideTransitionEventName,
            string postHideStateName, bool createNewPreTransitionEvent = false)
        {
            fsm.InsertHiddenState(FSMHiddenStateName, preHideStateName, preHideTransitionEventName, postHideStateName, createNewPreTransitionEvent);
        }

        public virtual void RecieveExtraDamage(ExtraDamageTypes extraDamageType)
        {
            if (isUnloading)
                return;

            int dmgAmount = ExtraDamageable.GetDamageOfType(extraDamageType);
            OnHit(dmgAmount);

            if (gameObject.CheckIfIsPogoLogicType())
            {
                if (EnemyHealthManager != null)
                {
                    HitTaker.Hit(gameObject, new HitInstance()
                    {
                        Source = gameObject,
                        AttackType = AttackTypes.Generic,
                        DamageDealt = dmgAmount <= 0 ? 1 : dmgAmount,
                        IgnoreInvulnerable = true,
                        MagnitudeMultiplier = 1,
                        MoveAngle = 0f,
                        MoveDirection = false,
                        Multiplier = 1f,
                        SpecialType = SpecialTypes.None,
                        IsExtraDamage = false,
                    });
                }
                else
                {
                    gameObject.KillObjectNow();
                }
            }
        }

        IEnumerator ResetOnHit(float afterTime)
        {
            yield return new WaitForSeconds(afterTime);
            if (gameObject == null)
                yield break;
            didOnHitThisFrame = false;
        }

        public virtual void Hit(HitInstance damageInstance)
        {
            if (isUnloading)
                return;

            if (didOnHitThisFrame)
                return;

            didOnHitThisFrame = true;
            StartCoroutine(ResetOnHit(0.2f));

            int dmgAmount = damageInstance.DamageDealt;
            OnHit(dmgAmount);

            if (gameObject.CheckIfIsPogoLogicType())
            {
                if (damageInstance.AttackType == AttackTypes.Spell)
                {
                    if (EnemyHealthManager != null)
                    {
                        if (damageInstance.IgnoreInvulnerable == false)
                            damageInstance.MoveAngle = 3;
                        else
                            damageInstance.MoveAngle = damageInstance.MoveAngle - 1;
                        damageInstance.IgnoreInvulnerable = true;
                        HitTaker.Hit(gameObject, damageInstance, (int)damageInstance.MoveAngle);
                    }
                    else
                    {
                        gameObject.KillObjectNow();
                    }
                }
            }
            else
            {
                if (takeCustomNailDamage)
                {
                    if (EnemyHealthManager != null)
                    {
                        EnemyHealthManager.Hit(damageInstance);
                    }
                    else
                    {
                        gameObject.KillObjectNow();
                    }
                }
            }
        }
    }
}


























//protected virtual void AddResetToStateOnHide(PlayMakerFSM fsm, string resetToState)
//{
//    if (FSMsWithResetToStateOnHide == null)
//        FSMsWithResetToStateOnHide = new Dictionary<PlayMakerFSM, string>();

//    FSMsWithResetToStateOnHide.Add(fsm, resetToState);
//}

//protected virtual void SpawnAndFlingItem()
//{
//    Dev.Where();
//    //if (thisMetadata != null && !thisMetadata.IsValidScene)
//    //    return;

//    //if (thisMetadata.AvailableItem != null)
//    //{
//    //    FlingUtils.SelfConfig fling = new FlingUtils.SelfConfig()
//    //    {
//    //        Object = thisMetadata.AvailableItem.Spawn(transform.position),
//    //        SpeedMin = 5f,
//    //        SpeedMax = 10f,
//    //        AngleMin = 0f,
//    //        AngleMax = 180f
//    //    };
//    //    FlingUtils.FlingObject(fling, null, Vector3.zero);
//    //}
//}



//protected virtual void ForceUpdateJournal()
//{
//    var pdName = thisMetadata.PlayerDataName;
//    RecordCustomJournalOnDeath(pdName);
//}

//protected virtual void RecordCustomJournalOnDeath(string pdName)
//{
//    PlayerData playerData = GameManager.instance.playerData;
//    string text = "killed" + pdName;
//    string text2 = "kills" + pdName;
//    string text3 = "newData" + pdName;
//    bool flag = false;
//    if (!playerData.GetBool(text))
//    {
//        flag = true;
//        playerData.SetBool(text, true);
//        playerData.SetBool(text3, true);
//    }
//    bool flag2 = false;
//    int num = playerData.GetInt(text2);
//    if (num > 0)
//    {
//        num--;
//        playerData.SetInt(text2, num);
//        if (num <= 0)
//        {
//            flag2 = true;
//        }
//    }
//    if (playerData.hasJournal)
//    {
//        bool flag3 = false;
//        if (flag2)
//        {
//            flag3 = true;
//            playerData.journalEntriesCompleted++;
//        }
//        else if (flag)
//        {
//            flag3 = true;
//            playerData.journalNotesCompleted++;
//        }
//        if (flag3)
//        {
//            //in lieu of the proper journal unlock effect, just do something
//            EnemyRandomizerDatabase.CustomSpawnWithLogic(transform.position, "Item Get Effect R", null, true);
//        }
//    }
//}