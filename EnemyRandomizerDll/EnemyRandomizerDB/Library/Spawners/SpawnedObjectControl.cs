using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;

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

        protected virtual void OnHit(int dmg) { }
        protected virtual void OnHeroInAggroRangeTheFirstTime() { }

        public bool needsFinalize = true;
        //set this to false when spawning an enemy that you want to "fall" in
        public bool placeGroundSpawnOnGround = true;
        public virtual bool isInvincible => false;
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
        public virtual string explodeOnDeathEffect => "Gas Explosion Recycle L";
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

        public virtual void Setup(GameObject objectThatWillBeReplaced = null)
        {
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
                    originialMetadata = originalObjectMeta;

                    Dev.Log($"Attempting Setup for {thisMetadata} with {originalObjectMeta}");
                }
                else
                {
                    //hold off on doing this assignment until the end of the enemy replacer
                    //there is no "replacement"
                    //originialMetadata = thisMetadata;

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

                    if (control == null)
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
                    SetPositionOnSpawn();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in SetPositionOnSpawn ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }
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

        public virtual void SetPositionOnSpawn()
        {
            PreSetSpawnPosition();

            DoSetSpawnPosition();

            AddPositionLogicFixers();

            OnSetSpawnPosition();
        }

        protected virtual void PreSetSpawnPosition()
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

            //first, if the enemy is or was inside a wall, fix that
            //CorrectInsideWallPosition(); 
        }


        protected virtual void DoSetSpawnPosition()
        {
            //then, place it "on the ground" or whereever it should be
            if (useCustomPositonOnSpawn)
                SetCustomPositionOnSpawn();
            else
                SetDefaultPosition();
        }

        protected virtual void AddPositionLogicFixers()
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

        protected virtual void OnSetSpawnPosition()
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
            try
            {
                if (gameObject.IsInAValidScene())
                    DoValidSceneDestroyEvents();
            }
            catch (Exception e) { Dev.LogError($"Exception caught on destroy callback for {thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.StackTrace}"); }
        }

        protected virtual void DoValidSceneDestroyEvents()
        {
            if (explodeOnDeath && !string.IsNullOrEmpty(explodeOnDeathEffect))
                ExplodeOnDeath();

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

        protected virtual void SpawnEntityOnDeath()
        {
            if (string.IsNullOrEmpty(spawnEntityOnDeath))
                return;

            if (!gameObject.IsInAValidScene())
                return;

            var spawned = SpawnerExtensions.SpawnEntityAt(spawnEntityOnDeath, transform.position, null, false);
            if (spawned != null)
            {
                var metaInfo = new ObjectMetadata(spawned);
                //did this spawn into a different scene?
                if (metaInfo.SceneName != thisMetadata.SceneName)
                    GameObject.Destroy(spawned);
                else
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

        public virtual void Hit(HitInstance damageInstance)
        {
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