using UnityEngine;
using System;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Satchel;
using Satchel.Futils;
using System.Collections.Generic;
using UniRx;
using System.Collections;

namespace EnemyRandomizerMod
{
    public class SpawnedObjectControl : MonoBehaviour, IExtraDamageable, IHitResponder
    {
        public static bool VERBOSE_DEBUG = true;

        public ObjectMetadata thisMetadata;
        public ObjectMetadata originialMetadata;

        public bool loaded { get; protected set; }

        /// <summary>
        /// override to autmatically set the control fsm used by this enemy in setup
        /// </summary>
        public virtual string FSMName => null;
        protected virtual string FSMHiddenStateName => "_Hidden_";

        PlayMakerFSM _internal_control;
        public virtual PlayMakerFSM control
        {
            get
            {
                if (_internal_control != null)
                    return _internal_control;

                if (_internal_control == null && !string.IsNullOrEmpty(FSMName))
                    _internal_control = gameObject.LocateMyFSM(FSMName);

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
        public virtual Vector2 pos2dWithOffset => gameObject.transform.position.ToVec2() + new Vector2(0, 1f);
        public virtual Vector2 heroPos2d => HeroController.instance.transform.position.ToVec2();
        public virtual Vector2 heroPosWithOffset => heroPos2d + new Vector2(0, 1f);
        public virtual float floorY => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.down, float.MaxValue).point.y;
        public virtual float roofY => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.up, float.MaxValue).point.y;
        public virtual float edgeL => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.left, float.MaxValue).point.x;
        public virtual float edgeR => SpawnerExtensions.GetRayOn(pos2dWithOffset, Vector2.right, float.MaxValue).point.x;

        protected virtual void OnHit(int dmg) { }
        protected virtual void OnHeroInAggroRangeTheFirstTime() { }

        public virtual bool isInvincible => false;
        public virtual bool spawnOrientationIsFlipped => false;
        public virtual float spawnPositionOffset => 0.53f;
        public virtual bool spawnShouldStickCorpse => false;
        public virtual bool useCustomPositonOnSpawn => false;
        public virtual bool preventOutOfBoundsAfterPositioning => true;
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
                    //originialMetadata = originalObjectMeta;

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

        protected virtual void CorrectInsideWallPosition()
        {
            if(gameObject.IsInsideWalls())
            {
                var cardinal = gameObject.GetCardinalRays(float.MaxValue);
                var shortest = cardinal.OrderBy(x => x.distance).FirstOrDefault();
                var direction = -shortest.normal;
                gameObject.transform.position = shortest.point + Vector2.Dot(direction, gameObject.GetOriginalObjectSize()) * direction;
            }
        }

        protected virtual void StickToRoof()
        {
            gameObject.StickToRoof(spawnPositionOffset, spawnOrientationIsFlipped);
        }

        protected virtual void StickToSurface()
        {
            gameObject.StickToClosestSurface(float.MaxValue, spawnPositionOffset, spawnShouldStickCorpse, spawnOrientationIsFlipped);
        }

        protected virtual void DoWallCling()
        {
            gameObject.StickToClosestSurfaceWithoutRotation(float.MaxValue, spawnPositionOffset);
        }

        //protected virtual void PlaceInsideGround()
        //{
        //    var nearest = gameObject.GetNearestRayOnSurface(float.MaxValue);
        //    gameObject.transform.position = gameObject.transform.position - nearest.normal

        //    gameObject.StickToGroundX(-spawnPositionOffset);
        //}

        protected virtual void PlaceOnGround()
        {
            gameObject.StickToGroundX(spawnPositionOffset);
        }

        public virtual void SetPositionOnSpawn()
        {
            //first, if the enemy is or was inside a wall, fix that
            CorrectInsideWallPosition();

            //then, place it "on the ground" or whereever it should be
            if (useCustomPositonOnSpawn)
                SetCustomPositionOnSpawn();
            else
                SetDefaultPosition();

            if (preventOutOfBoundsAfterPositioning)
                gameObject.AddComponent<PreventOutOfBounds>();

            //if (gameObject.IsInGroundEnemy())
            //{
            //    //PlaceInsideGround();
            //}
            //else
            //{
            //}
        }

        /// <summary>
        /// a custom override for how to position this enemy
        /// </summary>
        protected virtual void SetCustomPositionOnSpawn()
        {
            if (!gameObject.IsFlying())
                gameObject.StickToGroundX(spawnPositionOffset);
        }

        /// <summary>
        /// the default way to determine how to place this enemy
        /// </summary>
        protected virtual void SetDefaultPosition()
        {
            if(name.Contains("Ceiling Dropper"))
            {
                StickToRoof();
            }
            else if(name.Contains("Mantis Flyer Child"))
            {
                StickToSurface();
            }
            else
            {
                if (gameObject.IsClimbing() || gameObject.IsClimbingFromComponents())
                {
                    gameObject.StickToClosestSurface(float.MaxValue, extraOffsetScale: spawnPositionOffset, alsoStickCorpse: spawnShouldStickCorpse, flipped: spawnOrientationIsFlipped);
                }
                else if(!gameObject.IsMobile())//static enemies
                {
                    gameObject.StickToClosestSurface(float.MaxValue, extraOffsetScale: spawnPositionOffset, alsoStickCorpse: spawnShouldStickCorpse, flipped: spawnOrientationIsFlipped);
                }
                else if(!gameObject.IsFlying() && !gameObject.IsFlyingFromComponents())
                {
                    gameObject.StickToGroundX();
                }
            }
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
            catch (Exception e) { Dev.LogError($"Exception caught on disable callback for {thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.Message}"); }
        }

        protected virtual void OnDestroy()
        {
            try
            {
                if (gameObject.IsInAValidScene())
                    DoValidSceneDestroyEvents();
            }
            catch (Exception e) { Dev.LogError($"Exception caught on destroy callback for {thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.Message}"); }
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

            SpawnerExtensions.SpawnEntityAt(explodeOnDeathEffect, transform.position, true);
        }

        protected virtual void SpawnEntityOnDeath()
        {
            if (string.IsNullOrEmpty(spawnEntityOnDeath))
                return;

            if (!gameObject.activeInHierarchy || !gameObject.IsInAValidScene())
                return;

            SpawnerExtensions.SpawnEntityAt(spawnEntityOnDeath, transform.position, true);
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
        }

        public virtual void Hit(HitInstance damageInstance)
        {
            int dmgAmount = damageInstance.DamageDealt;
            OnHit(dmgAmount);
        }
    }

















    public class DefaultSpawnedEnemyControl : SpawnedObjectControl
    {
        protected Geo geoManager { get; set; }
        public Geo GeoManager
        {
            get
            {
                if (geoManager == null)
                    geoManager = new Geo(gameObject);

                return geoManager;
            }
        }

        public override int Geo
        {
            get => GeoManager.Value;
            set => GeoManager.Value = value;
        }

        public int previousHP = -1;
        public int CurrentHP
        {
            get => EnemyHealthManager == null ? 0 : EnemyHealthManager.hp;
            set
            {
                if (EnemyHealthManager != null)
                    EnemyHealthManager.hp = value;
                else
                { }
            }
        }

        public float CurrentHPf
        {
            get => (float)CurrentHP;
            set => CurrentHP = Mathf.FloorToInt(value);
        }

        /// <summary>
        /// set this to change the max hp of an enemy
        /// </summary>
        public int defaultScaledMaxHP = -1;
        public virtual int MaxHP
        {
            get => defaultScaledMaxHP;
            set
            {
                defaultScaledMaxHP = value;
                if (CurrentHP > defaultScaledMaxHP)
                    CurrentHP = defaultScaledMaxHP;
            }
        }

        public float MaxHPf
        {
            get => (float)MaxHP;
            set => MaxHP = Mathf.FloorToInt(value);
        }


        public virtual bool takesSpecialCharmDamage => gameObject.GetComponent<TinkEffect>() != null ? true : false;
        public virtual bool takesSpecialSpellDamage => gameObject.GetComponent<TinkEffect>() != null ? true : false;
        public virtual bool doBlueHealHeroOnDeath => didOriginalDoBlueHealHeroOnDeath;
        public virtual bool didOriginalDoBlueHealHeroOnDeath => originialMetadata != null && originialMetadata.GetDatabaseKey() != null && originialMetadata.GetDatabaseKey().Contains("Health");
        public virtual bool showWhenHeroIsInAggroRange => false;
        public virtual bool hasSeenPlayer { get; protected set; }
        public virtual bool didShowWhenHeroWasInAggroRange { get; protected set; }
        public virtual float aggroRange => 40f;
        public virtual bool didDisableCameraLocks { get; protected set; }
        protected virtual bool DisableCameraLocks => false;
        protected virtual IEnumerable<CameraLockArea> cameraLocks => DisableCameraLocks ? gameObject.GetCameraLocksFromScene() : null;


        public override void Setup(GameObject objectThatWillBeReplaced = null)
        {
            try
            {
                if (gameObject.ObjectType() == PrefabObject.PrefabType.Enemy)
                {
                    defaultScaledMaxHP = CurrentHP = GetStartingMaxHP(objectThatWillBeReplaced);
                    SetupEnemyGeo();
                }
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in ConfigureRelativeToReplacement ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }
        }

        protected virtual int GetStartingMaxHP(GameObject objectThatWillBeReplaced)
        {
            if (objectThatWillBeReplaced != null && objectThatWillBeReplaced.IsBoss())
            {
                return ScaleHPToBoss(gameObject.OriginalPrefabHP(), objectThatWillBeReplaced.OriginalPrefabHP());
            }
            else
            {
                return ScaleHPToNormal(gameObject.OriginalPrefabHP(), objectThatWillBeReplaced.OriginalPrefabHP());
            }
        }

        protected override void DoValidSceneDestroyEvents()
        {
            base.DoValidSceneDestroyEvents();
            try
            {
                if (doBlueHealHeroOnDeath)
                    gameObject.DoBlueHealHero();

                //if (gameObject.GetAvailableItem() != null)
                //    SpawnAndFlingItem();
            }
            catch (Exception e) { Dev.LogError($"Exception caught on OnDestroy callback for {this}:{thisMetadata} ERROR:{e.Message}  STACKTRACE: {e.Message}"); }
        }

        protected virtual bool HeroInAggroRange()
        {
            if (hasSeenPlayer)
            {
                return gameObject.IsNearPlayer(aggroRange);
            }
            else
            {
                if (gameObject.CanSeePlayer())
                {
                    hasSeenPlayer = true;
                    return true;
                }

                return false;
            }
        }

        protected virtual void Update()
        {
            if (!loaded)
                return;

            if (gameObject.ObjectType() != PrefabObject.PrefabType.Enemy)
                return;

            try
            {
                CheckDisableCameras();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in CheckDisableCameras ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                CheckUpdateHeroInAggroRange();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in CheckUpdateHeroInAggroRange ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }

            try
            {
                CheckControlInCustomHiddenState();
            }
            catch (Exception e)
            {
                Dev.Log($"{this}:{this.thisMetadata}: Caught exception in CheckControlInCustomHiddenState ERROR:{e.Message} STACKTRACE{e.StackTrace}");
            }
        }

        protected virtual void CheckDisableCameras()
        {
            if (!didDisableCameraLocks && DisableCameraLocks)
            {
                var cams = cameraLocks;
                if (cams.Count() > 0)
                {
                    cameraLocks.UnlockCameras();
                    didDisableCameraLocks = true;
                }
            }
        }


        protected virtual void CheckUpdateHeroInAggroRange()
        {
            if (showWhenHeroIsInAggroRange && !didShowWhenHeroWasInAggroRange)
            {
                if (HeroInAggroRange())
                {
                    didShowWhenHeroWasInAggroRange = true;
                    OnHeroInAggroRangeTheFirstTime();
                }
            }
        }

        protected virtual void CheckControlInCustomHiddenState()
        {
            if (control == null)
                return;

            if (control.ActiveStateName == FSMHiddenStateName)
            {
                control.SendEvent("SHOW");
            }
        }

        protected virtual int ScaleHPToNormal(int defaultNewEnemyHP, int originalEnemyHP)
        {
            if (originalEnemyHP * 2 < defaultNewEnemyHP)
                return Mathf.FloorToInt(originalEnemyHP * 2f);
            else if (originalEnemyHP < defaultNewEnemyHP)
                return originalEnemyHP;
            else
                return defaultNewEnemyHP;
        }

        protected virtual int ScaleHPToBoss(int defaultNewEnemyHP, int originalEnemyHP)
        {
            int minBarBossHP = 225;
            if(originalEnemyHP < minBarBossHP)
            {
                int newBossHP = originalEnemyHP * 2;
                minBarBossHP = minBarBossHP + 100;
                for (int i = 0; i < 16; ++i)
                {
                    if (newBossHP > minBarBossHP)
                    {
                        return newBossHP;
                    }
                    newBossHP = originalEnemyHP * 2;
                    minBarBossHP = minBarBossHP + 100;
                }

                return defaultNewEnemyHP;
            }
            else
            {
                if (defaultNewEnemyHP * 2 < defaultNewEnemyHP)
                    return Mathf.FloorToInt(originalEnemyHP * 2f);
                return originalEnemyHP;
            }
        }

        protected virtual void SetupEnemyGeo()
        {
            int originalGeo = SpawnerExtensions.GetOriginalGeo(thisMetadata.ObjectName);
            if (originialMetadata != null && thisMetadata != originialMetadata)
            {
                originalGeo = SpawnerExtensions.GetOriginalGeo(originialMetadata.ObjectName);
            }

            var zone = GameManager.instance.GetCurrentMapZone();
            int geoScale = 1;
            if (MetaDataTypes.GeoZoneScale.TryGetValue(zone, out geoScale))
            {
                if (originalGeo <= 0)
                {
                    Geo = SpawnerExtensions.GetRandomValueBetween(geoScale, geoScale * 5);
                }
                else
                {
                    Geo = SpawnerExtensions.GetRandomValueBetween(originalGeo, originalGeo * geoScale);
                }
            }
            else
            {
                Geo = SpawnerExtensions.GetRandomValueBetween(1 + originalGeo, originalGeo * 2);
            }
        }

        public override void RecieveExtraDamage(ExtraDamageTypes extraDamageType)
        {
            base.RecieveExtraDamage(extraDamageType);
            if (takesSpecialCharmDamage && EnemyHealthManager != null)
            {
                int dmgAmount = ExtraDamageable.GetDamageOfType(extraDamageType);
                EnemyHealthManager.ApplyExtraDamage(dmgAmount);
            }
        }

        public override void Hit(HitInstance damageInstance)
        {
            base.Hit(damageInstance);
            if (takesSpecialSpellDamage && EnemyHealthManager != null && damageInstance.AttackType == AttackTypes.Spell)
            {
                int dmgAmount = damageInstance.DamageDealt;
                EnemyHealthManager.ApplyExtraDamage(dmgAmount);
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