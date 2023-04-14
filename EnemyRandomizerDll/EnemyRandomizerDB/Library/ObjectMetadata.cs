using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using System.Linq;
using System;
using System.Reflection;
using static EnemyRandomizerMod.PrefabObject;
using UniRx;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;

//NOTE: adjust aduio player oneshot pitch values and audio source component pitch values when shrinking/growing things
//NOTE: walker enemies need their "rightScale" float changed when the base transform scale is changed so they match or sprites will squish weirdly
//      ALSO need to scan their FSMs for states with "SetScale" actions with x values that are 1/-1 and change those to match the x scale
//      check the "IsNone" property to see if only X is used on the SetScale floats

namespace EnemyRandomizerMod
{
    public class ObjectMetadata
    {
        public bool HasData { get; protected set; }
        public string DatabaseName { get; protected set; }
        public PrefabObject.PrefabType ObjectType { get; protected set; }
        public PrefabObject ObjectPrefab { get; protected set; }
        public string SceneName { get; protected set; }
        public string ObjectName { get; protected set; }
        public string ScenePath { get; protected set; }
        public string MapZone { get; protected set; }
        public bool IsInvalidObject { get; protected set; }
        public bool IsDisabledBySavedGameState { get; protected set; }
        public bool IsBoss { get; protected set; }
        public bool IsFlying { get; protected set; }
        public bool IsCrawling { get; protected set; }
        public bool IsClimbing { get; protected set; }
        public bool IsTinker { get; protected set; }
        public bool IsMobile { get; protected set; }
        public bool IsWalker { get; protected set; }
        public bool IsBattleEnemy { get; protected set; }
        public bool HasAvailableItem { get; protected set; }
        public bool IsSmasher { get; protected set; }
        public int MaxHP { get; protected set; }
        public bool IsInvincible { get; protected set; }
        public int DamageDealt { get; protected set; }
        public int EnemyDamageDealt { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsSummonedByEvent { get; protected set; }
        public bool IsEnemySpawner { get; protected set; }
        public Vector2 ObjectSize { get; protected set; }
        public Vector3 ObjectPosition { get; protected set; }
        public Vector3 ObjectScale { get; protected set; }
        public float Rotation { get; protected set; }

        public ObjectMetadata ObjectThisReplaced { get; protected set; }
        public float SizeScale { get; protected set; }
        public ReadOnlyReactiveProperty<List<GameObject>> BlackBorders { get; protected set; }

        //These values will become null after a replacement
        public GameObject Source { get; protected set; }
        public GameObject AvailableItem { get; protected set; }
        public PersistentBoolItem SceneSaveData { get; protected set; }
        public HealthManager EnemyHealthManager { get; protected set; }
        public Collider2D Collider { get; protected set; }
        public tk2dSprite Sprite { get; protected set; }
        public DamageHero HeroDamage { get; protected set; }
        public DamageEnemies EnemyDamage { get; protected set; }
        public Walker Walker { get; protected set; }
        public EnemyDeathEffects DeathEffects { get; protected set; }
        public ManagedObject RandoObject { get; protected set; }
        public BattleManagedObject BattleRandoObject { get; protected set; }

        public bool IsAReplacementObject { get { return RandoObject == null ? false : RandoObject.replaced; } }

        protected virtual void SetupObjectType(string databaseName, EnemyRandomizerDatabase database)
        {
            if (database.Enemies.ContainsKey(databaseName))
                ObjectType = PrefabType.Enemy;

            if (database.Hazards.ContainsKey(databaseName))
                ObjectType = PrefabType.Hazard;

            if (database.Effects.ContainsKey(databaseName))
                ObjectType = PrefabType.Effect;

            ObjectPrefab = database.Objects[databaseName];
        }

        protected virtual bool SetupDatabaseRefs(GameObject sceneObject, EnemyRandomizerDatabase database)
        {
            if (!EnemyRandomizerDatabase.IsDatabaseObject(sceneObject))
            {
                HasData = false;
            }
            else
            {
                DatabaseName = EnemyRandomizerDatabase.ToDatabaseKey(sceneObject.name);
                if (!string.IsNullOrEmpty(DatabaseName))
                {
                    HasData = IsObjectInDatabase(database);
                    if(HasData)
                    {
                        SetupObjectType(DatabaseName, database);
                    }
                }
            }

            return HasData;
        }

        protected virtual void SetupComponentRefs(GameObject sceneObject)
        {
            RandoObject = sceneObject.GetComponent<ManagedObject>();
            BattleRandoObject = sceneObject.GetComponent<BattleManagedObject>();

            EnemyHealthManager = sceneObject.GetComponent<HealthManager>();
            if (EnemyHealthManager != null)
            {
                MaxHP = EnemyHealthManager.hp;
                IsInvincible = EnemyHealthManager.IsInvincible;

                var battleScene = EnemyHealthManager.GetBattleScene();
                if (battleScene != null)
                {
                    IsBattleEnemy = true;
                }
            }

            HeroDamage = sceneObject.GetComponent<DamageHero>();
            if (HeroDamage != null)
            {
                DamageDealt = HeroDamage.damageDealt;
            }

            EnemyDamage = sceneObject.GetComponent<DamageEnemies>();
            if (EnemyDamage != null)
            {
                EnemyDamageDealt = EnemyDamage.damageDealt;
            }

            Walker = sceneObject.GetComponent<Walker>();
            IsWalker = Walker != null;

            //check if it's disabled/completed already
            var pbi = sceneObject.GetComponent<PersistentBoolItem>();
            if (pbi != null)
            {
                SceneSaveData = pbi;

                if (pbi.isActiveAndEnabled)
                {
                    IsDisabledBySavedGameState = pbi.persistentBoolData.activated;
                }
                else
                {
                    var data = global::SceneData.instance.FindMyState(pbi.persistentBoolData);
                    if (data != null)
                        IsDisabledBySavedGameState = data.activated;
                    else
                        IsDisabledBySavedGameState = false;
                }
            }

            Collider = sceneObject.GetComponent<Collider2D>();
            Sprite = sceneObject.GetComponent<tk2dSprite>();

            DeathEffects = sceneObject.GetComponent<EnemyDeathEffects>();
        }

        protected virtual void SetupTransformValues(GameObject sceneObject)
        {
            SizeScale = 1f;


            if (sceneObject.GetComponent<tk2dSprite>() && sceneObject.GetComponent<tk2dSprite>().boxCollider2D != null)
            {
                ObjectSize = sceneObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                Dev.Log($"Size of SPRITE {sceneObject} is {ObjectSize}");
            }
            else if (sceneObject.GetComponent<BoxCollider2D>())
            {
                ObjectSize = sceneObject.GetComponent<BoxCollider2D>().size;
                Dev.Log($"Size of BOX {sceneObject} is {ObjectSize}");
            }
            else if (sceneObject.GetComponent<CircleCollider2D>())
            {
                var newCCircle = sceneObject.GetComponent<CircleCollider2D>();
                ObjectSize = Vector2.one * newCCircle.radius;
                Dev.Log($"Size of CIRCLE {sceneObject} is {ObjectSize}");
            }
            else if (sceneObject.GetComponent<PolygonCollider2D>())
            {
                var newCPoly = sceneObject.GetComponent<PolygonCollider2D>();
                ObjectSize = new Vector2(newCPoly.points.Select(x => x.x).Max() - newCPoly.points.Select(x => x.x).Min(), newCPoly.points.Select(x => x.y).Max() - newCPoly.points.Select(x => x.y).Min());

                Dev.Log($"Size of POLYGON {sceneObject} is {ObjectSize}");
            }
            else
            {
                ObjectSize = sceneObject.transform.localScale;
                Dev.Log($"Size of TRANSFORM SCALE {sceneObject} is {ObjectSize}");

                if (ObjectSize.x < 0)
                    ObjectSize = new Vector2(-ObjectSize.x, ObjectSize.y);
            }

            UpdateTransformValues();
        }

        public virtual void UpdateTransformValues()
        {
            if (Source != null)
            {
                ObjectPosition = Source.transform.position;
                ObjectScale = Source.transform.localScale;
                Rotation = Source.transform.localEulerAngles.z;
            }
        }

        protected virtual void SetupItemValues(GameObject sceneObject)
        {
            //check and see if this enemy would have dropped an item
            HasAvailableItem = sceneObject.GetComponent<PreInstantiateGameObject>();
            if (!HasAvailableItem)
            {
                var ede = sceneObject.GetComponent<EnemyDeathEffects>();
                if (ede != null)
                {
                    var corpse = ede.GetCorpseFromDeathEffects();
                    if (corpse != null)
                    {
                        if (corpse.GetComponent<PreInstantiateGameObject>())
                        {
                            var go = corpse.GetComponent<PreInstantiateGameObject>().InstantiatedGameObject;
                            if (go != null && go.name.Contains("Shiny Item"))
                            {
                                var go_pbi = go.GetComponent<PersistentBoolItem>();
                                if (go_pbi != null && !go_pbi.persistentBoolData.activated)
                                {
                                    HasAvailableItem = true;
                                    AvailableItem = go;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (EnemyHealthManager != null)
                            Debug.LogError(ScenePath + " has no corpse!");
                    }
                }
            }
        }

        protected virtual void SetupRandoProperties(GameObject sceneObject)
        {
            IsTinker = sceneObject.GetComponentInChildren<TinkEffect>() != null;

            if (EnemyHealthManager == null)
            {
                if (IsTinker)
                    IsInvincible = true;
                return;
            }

            bool isFlyingFromComponents =
                (sceneObject.GetComponent<Rigidbody2D>() != null && sceneObject.GetComponent<Rigidbody2D>().gravityScale == 0) &&
                (sceneObject.GetComponent<Climber>() == null);

            IsBoss = DefaultMetadata.Bossses.Contains(DatabaseName);
            IsFlying = isFlyingFromComponents || DefaultMetadata.Flying.Contains(DatabaseName);
            IsCrawling = sceneObject.GetComponent<Crawler>() != null || DefaultMetadata.Crawling.Contains(DatabaseName);
            IsClimbing = sceneObject.GetComponent<Climber>() != null || DefaultMetadata.Climbing.Contains(DatabaseName);
            IsMobile = !DefaultMetadata.Static.Contains(DatabaseName);
            IsSmasher = sceneObject.GetDirectChildren().Any(x => x.name == "Smasher");//can use this child object layer to enable smashing of platforms/walls
            IsSummonedByEvent = DefaultMetadata.IsSummonedByEvent.Contains(DatabaseName);
            IsEnemySpawner = DefaultMetadata.SpawnerEnemies.Contains(DatabaseName);
            CheckIfIsBattleEnemy(sceneObject);
        }

        protected virtual void CheckIfIsBattleEnemy(GameObject sceneObject)
        {
            if (EnemyHealthManager == null)
                return;

            if (!IsBattleEnemy)
            {
                if (sceneObject.GetComponent<BattleManagedObject>())
                    IsBattleEnemy = true;

                if (!IsBattleEnemy)
                {
                    IsBattleEnemy = IsBoss;

                    if (!IsBattleEnemy)
                    {
                        IsBattleEnemy = ScenePath.Split('/').Any(x => BattleManager.battleControllers.Any(y => x.Contains(y)));
                    }

                    if (!IsBattleEnemy)
                    {
                        bool hasScene = DefaultMetadata.BattleEnemies.TryGetValue("ANY", out var enemies);
                        if (hasScene)
                        {
                            IsBattleEnemy = enemies.Contains(DatabaseName);
                        }

                        if (!IsBattleEnemy)
                        {
                            hasScene = DefaultMetadata.BattleEnemies.TryGetValue(SceneName, out var enemies2);
                            if (hasScene)
                            {
                                IsBattleEnemy = enemies2.Contains(ScenePath);
                            }
                        }
                    }
                }
            }
        }

        protected virtual bool IsVisibleNow()
        {
            bool isVisible = false;
            GameObject sceneObject = Source;
            if (Source == null)
            {
                //Dev.Log($"{ScenePath} source has been deleted so this object can never be active!");
                return false;
            }

            if (sceneObject.gameObject.activeSelf)
            {
                isVisible = sceneObject.gameObject.activeSelf;
                //Dev.Log($"{ScenePath} has self active state: {isVisible}");

                //might not actually be...
                if (isVisible)
                {
                    //count "out of bounds" enemies as not visible

                    if (BlackBorders.Value != null && BlackBorders.Value.Count > 0)
                    {
                        var leftRight = BlackBorders.Value.Where(x => x.transform.localScale.x == 20);
                        var topBot = BlackBorders.Value.Where(x => x.transform.localScale.y == 20);

                        var xmin = leftRight.Min(o => (o.transform.position.x - 10f));
                        var xmax = leftRight.Max(o => (o.transform.position.x + 10f));
                        var ymin = topBot.Min(o => (o.transform.position.y - 10f));
                        var ymax = topBot.Max(o => (o.transform.position.y + 10f));

                        //Dev.Log($"{ScenePath} pos:{sceneObject.transform.position}");
                        //Dev.Log($"{ScenePath} BOUNDS[ xmin:{xmin} xmax:{xmax} ymin:{ymin} ymax:{ymax}]");

                        if (sceneObject.transform.position.x < xmin)
                            isVisible = false;
                        else if (sceneObject.transform.position.x > xmax)
                            isVisible = false;
                        else if (sceneObject.transform.position.y < ymin)
                            isVisible = false;
                        else if (sceneObject.transform.position.y > ymax)
                            isVisible = false;
                        
                        //if(isVisible)
                        //    Dev.Log($"{ScenePath} is in bounds!");
                        //else
                        //    Dev.Log($"{ScenePath} is out of bounds!");
                    }
                    else
                    {
                        //Dev.Log($"{ScenePath} BOUNDS NOT READY YET! Cannot check if visible");
                        //not visible yet
                        return false;
                    }
                }

                //still might not actually be...
                if (isVisible)
                {
                    Collider2D collider = sceneObject.GetComponent<Collider2D>();
                    MeshRenderer renderer = sceneObject.GetComponent<MeshRenderer>();
                    if (collider != null || renderer != null)
                    {
                        if (collider != null && renderer == null)
                            isVisible = collider.enabled;
                        else if (collider == null && renderer != null)
                            isVisible = renderer.enabled;
                        else //if (collider != null && renderer != null)
                            isVisible = collider.enabled && renderer.enabled;
                    }

                    //if(!isVisible)
                    //    Dev.Log($"{ScenePath} has disabled colliders or renderers and is considered invisible!");
                }
            }
            return isVisible;
        }

        public virtual bool CheckIfIsActiveAndVisible()
        {
            if (Source == null)
                return false;

            bool IsVisible = IsVisibleNow(); 
            IsActive = IsVisible && !IsDisabledBySavedGameState;
            return IsVisible && IsActive;
        }

        public bool IsTemporarilyInactive()
        { 
            return HasData && !IsAReplacementObject && Source.activeInHierarchy && EnemyHealthManager != null && !IsDisabledBySavedGameState && !CheckIfIsActiveAndVisible();
        } 

        public bool IsBattleInactive()
        {
            return HasData && !IsAReplacementObject && IsBattleEnemy && !Source.activeInHierarchy && EnemyHealthManager != null && !IsDisabledBySavedGameState && !CheckIfIsActiveAndVisible();
        }

        public bool Setup(GameObject sceneObject, EnemyRandomizerDatabase database)
        {
            Source = sceneObject;
            ObjectName = sceneObject.name;
            ScenePath = sceneObject.GetSceneHierarchyPath();
            SceneName = sceneObject.scene.IsValid() ? sceneObject.scene.name : null;
            MapZone = GameManager.instance.GetCurrentMapZone();
            IsInvalidObject = CheckIfIsBadObject(ScenePath);
            BlackBorders = EnemyRandomizerDatabase.GetBlackBorders().ToReadOnlyReactiveProperty();

            if (IsInvalidObject)
                return false;

            if(!SetupDatabaseRefs(sceneObject, database))
                return false;

            SetupComponentRefs(sceneObject);
            SetupTransformValues(sceneObject);
            SetupItemValues(sceneObject);
            SetupRandoProperties(sceneObject);
            CheckIfIsActiveAndVisible();

            return true;
        }

        public void Dump()
        {
            var self = this;
            var props = self.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            props.ToList().ForEach(x => Dev.Log($"{x.Name}: {x.GetValue(self)}"));
        }


        ////use another object's object metadata to configure this metadata
        //public void MarkAsReplacement(ObjectMetadata other)
        //{
        //}


        static bool DEBUG_WARN_IF_NOT_FOUND = true;

        public virtual bool IsObjectAnEnemy(EnemyRandomizerDatabase database)
        {
            if (!string.IsNullOrEmpty(DatabaseName) && database.Enemies.ContainsKey(DatabaseName))
                return true;
            else
            {
                if (DEBUG_WARN_IF_NOT_FOUND)
                    Dev.LogWarning($"WARNING: [{DatabaseName}] was not found in the enemy database when searching for [{ObjectName}]!");
            }
            return false;
        }

        public virtual bool IsObjectInDatabase(EnemyRandomizerDatabase database)
        {
            if (!string.IsNullOrEmpty(DatabaseName) && database.Objects.ContainsKey(DatabaseName))
                return true;
            else
            {
                if (DEBUG_WARN_IF_NOT_FOUND)
                    Dev.LogWarning($"WARNING: [{DatabaseName}] was not found in the object database when searching for [{ObjectName}]!");
            }
            return false;
        }

        public virtual bool IsEffectInDatabase(EnemyRandomizerDatabase database)
        {
            if (!string.IsNullOrEmpty(DatabaseName) && database.Effects.ContainsKey(DatabaseName))
                return true;
            else
            {
                if (DEBUG_WARN_IF_NOT_FOUND)
                    Dev.LogWarning($"WARNING: [{DatabaseName}] was not found in the effect database when searching for [{ObjectName}]!");
            }
            return false;
        }

        public virtual bool IsHazardInDatabase(EnemyRandomizerDatabase database)
        {
            if (!string.IsNullOrEmpty(DatabaseName) && database.Hazards.ContainsKey(DatabaseName))
                return true;
            else
            {
                if (DEBUG_WARN_IF_NOT_FOUND)
                    Dev.LogWarning($"WARNING: [{DatabaseName}] was not found in the effect database when searching for [{ObjectName}]!");
            }
            return false;
        }


        public virtual bool CanProcessObject()
        {
            if (!HasData)
                return false;

            if (IsInvalidObject)
                return false;

            if (!IsActive)
            {
                if(ObjectType != PrefabType.Effect)
                    return false;
            }

            if (IsAReplacementObject)
                return false;

            if (!CheckIfIsActiveAndVisible())
            {
                return false;
            }

            return true;
        }

        protected virtual bool CheckIfIsBadObject(string scenePath)
        {
            return DefaultMetadata.AlwaysDeleteObject.Any(x => scenePath.Contains(x));
        }

        public virtual void DestroySource(bool disableObjectBeforeDestroy = true)
        {
            if (Source != null)
            {
                UpdateTransformValues();

                if (ObjectName.Contains("Fly") && SceneName == "Crossroads_04")
                {
                    //this seems to correctly decrement the count from the battle manager
                    BattleManager.StateMachine.Value.RegisterEnemyDeath(null);
                }

                if (disableObjectBeforeDestroy)
                {
                    //if (oldEnemy.activeSelf)
                    Source.SetActive(false);
                }

                GameObject.Destroy(Source);
                Source = null;
            }
        }

        public virtual float GetRelativeScale(ObjectMetadata other, float min = .1f, float max = 2.5f)
        {
            var oldSize = other.ObjectSize;
            var newSize = ObjectSize;

            float scaleX = oldSize.x / newSize.x;
            float scaleY = oldSize.y / newSize.y;
            float scale = scaleX > scaleY ? scaleY : scaleX;

            if (scale < min)
                scale = min;

            if (scale > max)
                scale = max;

            return scale;
        }

        public virtual void ApplyPosition(Vector3 position)
        {
            ObjectPosition = position;
            Source.transform.position = position;
        }

        public virtual void ApplySizeScale(float scale)
        {
            SizeScale = scale;

            if (Source == null)
                return;

            Source.transform.localScale = new Vector3(Source.transform.localScale.x * scale,
                Source.transform.localScale.y * scale, 1f);
            ObjectScale = Source.transform.localScale;

            if (Walker != null)
            {
                if(Walker.GetRightScale() > 0)
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
                    a.GetType().GetField("xScale", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(a,scale);
                }
            }
        }

        public virtual void SetAudioToMatchScale()
        {
            if (Source == null)
                return;

            if (Mathnv.FastApproximately(SizeScale, 1f, .01f))
                return;

            float max = 2f;
            float min = .5f;
            Range range = new Range(min, max);
            float t = range.NormalizedValue(SizeScale);
            float pitch = max - range.Evaluate(t);

            var go = Source;
            var audioSources = go.GetComponentsInChildren<AudioSource>();
            var audioSourcesPitchRandomizer = go.GetComponentsInChildren<AudioSourcePitchRandomizer>();
            //var audioPlayActions = go.GetActionsOfType<AudioPlay>();
            var audioPlayOneShot = go.GetActionsOfType<AudioPlayerOneShot>();
            var audioPlayRandom = go.GetActionsOfType<AudioPlayRandom>();
            var audioPlayOneShotSingle = go.GetActionsOfType<AudioPlayerOneShotSingle>();
            //var audioPlayInState = go.GetActionsOfType<AudioPlayInState>();
            var audioPlayRandomSingle = go.GetActionsOfType<AudioPlayRandomSingle>();
            //var audioPlaySimple = go.GetActionsOfType<AudioPlaySimple>();
            //var audioPlayV2 = go.GetActionsOfType<AudioPlayV2>();
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

        public virtual void MarkObjectAsReplacement(ObjectMetadata oldObject)
        {
            ObjectThisReplaced = oldObject;

            if (RandoObject == null)
            {
                if (oldObject.IsBattleEnemy)
                {
                    BattleRandoObject = Source.AddComponent<BattleManagedObject>();
                    RandoObject = BattleRandoObject;
                }
                else
                {
                    RandoObject = Source.AddComponent<ManagedObject>();
                }

                RandoObject.Setup(oldObject);
            }

            RandoObject.replaced = true;
        }

        public virtual GameObject ActivateSource()
        {
            //error?
            if (Source == null)
                return null;

            if (ObjectThisReplaced != null && this != ObjectThisReplaced)
            {
                var oedf = ObjectThisReplaced.DeathEffects;
                var nedf = DeathEffects;
                if (oedf != null && nedf != null)
                {
                    string oplayerDataName = oedf.GetPlayerDataNameFromDeathEffects();
                    nedf.SetPlayerDataNameFromDeathEffects(oplayerDataName);
                }
            }

            Source.SafeSetActive(true);

            if(ObjectThisReplaced != null && this != ObjectThisReplaced)
                ObjectThisReplaced.DestroySource();

            return Source;
        }

        public void PlaceOnGround()
        {
            if (Source == null)
                return;

            var ground = Source.GetNearestPointDown(500f);
            var positionOffset = new Vector3(0f, ObjectSize.y * -0.5f * SizeScale, 0f);
            ApplyPosition(ground + positionOffset);
        }


        public void MatchPositionOfOther(ObjectMetadata otherdata = null)
        {
            if (otherdata == null)
                return;


            if (DatabaseName.Contains("Mawlek Turret"))
                return;

            float rotation = otherdata.Rotation;
            Vector2 originalUp = Vector2.zero;

            if (Mathf.Approximately(0f, rotation) || Mathf.Approximately(360f, rotation))
                originalUp = Vector2.up;

            if (Mathf.Approximately(90f, rotation) || Mathf.Approximately(-270f, rotation))
                originalUp = Vector2.right;

            if (Mathf.Approximately(-90f, rotation) || Mathf.Approximately(270f, rotation))
                originalUp = Vector2.left;

            if (Mathf.Approximately(180f, rotation))
                originalUp = Vector2.down;

            Vector3 positionOfObject = otherdata.ObjectPosition;
            Vector3 positionOffset = Vector3.zero;
            Vector2 objectSize = ObjectSize;
            Vector2 scale = ObjectScale;
            Vector2 originalPosition = positionOfObject;
            float projectionDistance = 500f;

            if (DatabaseName.Contains("Mantis Flyer Child"))
            {
                positionOffset = new Vector3(objectSize.x * originalUp.x * ObjectScale.x, objectSize.y * originalUp.y * ObjectScale.y, 0f);
            }
            //project the ceiling droppers onto the ceiling
            if (DatabaseName.Contains("Ceiling Dropper"))
            {
                positionOfObject = Mathnv.GetPointOn(originalPosition, Vector2.up, projectionDistance, IsSurfaceOrPlatform);
                //move it down a bit, keeps spawning in roof
                positionOffset = Vector3.down * 2f * scale.y;
            }


            Vector2 originalDown = -originalUp;
            Vector3 toSurface = Mathnv.GetNearestVectorTo(originalPosition, projectionDistance, IsSurfaceOrPlatform);
            Vector2 toSurfaceDir = toSurface.normalized;
            Vector2 toSurfaceUp = -toSurfaceDir;

            if (!IsFlying)
            {
                positionOfObject = Mathnv.GetNearestPointOn(originalPosition, projectionDistance, IsSurfaceOrPlatform);
            }

            if (!IsFlying)
            {
                if (Mathf.Approximately(0f, rotation))
                {
                    positionOfObject = GetPointOn(otherdata, Vector2.down, projectionDistance);

                    if (DatabaseName.Contains("Lobster"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * 2f) * scale.y;
                    }
                    if (DatabaseName.Contains("Blocker"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                    }
                    if (DatabaseName == ("Moss Knight"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -1f) * scale.y;
                    }
                    if (DatabaseName == ("Enemy"))
                    {
                        positionOffset = positionOffset + (Vector3)(Vector2.up * -0.5f) * scale.y;
                    }
                }
                else
                {
                   positionOfObject = GetPointOn(otherdata, toSurfaceDir, projectionDistance);
                }

                positionOffset = new Vector3(objectSize.x * originalUp.x * scale.x / 3f, objectSize.y * originalUp.y * scale.y / 3f, 0f);

                if (DatabaseName.Contains("Moss Walker"))
                {
                    positionOffset = toSurfaceUp * objectSize.y * scale.y / 3f;
                }
                if (DatabaseName.Contains("Plant Trap"))
                {
                    positionOffset = toSurfaceUp * 2f * scale.y;
                }
                if (DatabaseName.Contains("Mushroom Turret"))
                {
                    positionOffset = (toSurfaceUp * .5f) * scale.y;
                }
                if (DatabaseName.Contains("Plant Turret"))
                {
                    positionOffset = toSurfaceUp * .7f * scale.y;
                }
                if (DatabaseName.Contains("Laser Turret"))
                {
                    positionOffset = toSurfaceUp * objectSize.y / 10f * scale.y;
                }
                if (DatabaseName.Contains("Worm"))
                {
                    positionOffset = toSurfaceUp * objectSize.y / 3f * scale.y;
                }
                if (DatabaseName.Contains("Crystallised Lazer Bug"))
                {
                    //suppposedly 1/2 their Y collider space offset should be 1.25
                    //but whatever we set it at, they spawn pretty broken, so spawn them out of the ground a bit so they're still a threat
                    positionOffset = toSurfaceUp * objectSize.y * 1.5f * scale.y;
                }
                if (DatabaseName.Contains("Mines Crawler"))
                {
                    positionOffset = toSurfaceUp * 1.5f * scale.y;
                }
                if (DatabaseName.Contains("Spider Mini"))
                {
                    positionOffset = toSurfaceUp * objectSize.y * 1.5f * scale.y; ;
                }
                if (DatabaseName.Contains("Abyss Crawler"))
                {
                    positionOffset = toSurfaceUp * objectSize.y * 1.5f * scale.y; ;
                }
                if (DatabaseName.Contains("Climber"))
                {
                    positionOffset = toSurfaceUp * objectSize.y * 1.5f * scale.y;
                }

                if (DatabaseName.ToLower().Contains("zote") || DatabaseName.ToLower().Contains("prince"))
                {
                    positionOffset = toSurfaceUp * objectSize.y * 1.5f * scale.y;
                }
            }
            else
            {
                positionOfObject = originalPosition;
            }

            ApplyPosition(positionOfObject + positionOffset);
        }

        public static bool IsSurfaceOrPlatform(GameObject gameObject)
        {
            if (gameObject == null)
                return false;

            return IsSurfaceOrPlatform(gameObject.name);
        }

        public static bool IsSurfaceOrPlatform(string name)
        {
            //First process skips or exclusions
            List<string> groundOrPlatformName = new List<string>()
            {
                "Chunk",
                "Platform",
                "plat_",
                "Roof"
            };

            return groundOrPlatformName.Any(x => name.Contains(x));
        }

        public static Vector3 GetVectorTo(ObjectMetadata entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetVectorTo(entitiy.ObjectPosition, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetPointOn(ObjectMetadata entitiy, Vector2 dir, float max)
        {
            return Mathnv.GetPointOn(entitiy.ObjectPosition, dir, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorToSurface(ObjectMetadata entitiy, float max)
        {
            return Mathnv.GetNearestVectorTo(entitiy.ObjectPosition, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointOnSurface(ObjectMetadata entitiy, float max)
        {
            return Mathnv.GetNearestPointOn(entitiy.ObjectPosition, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestVectorDown(ObjectMetadata entitiy, float max)
        {
            return Mathnv.GetNearestVectorDown(entitiy.ObjectPosition, max, IsSurfaceOrPlatform);
        }

        public static Vector3 GetNearestPointDown(ObjectMetadata entitiy, float max)
        {
            return Mathnv.GetNearestPointDown(entitiy.ObjectPosition, max, IsSurfaceOrPlatform);
        }
    }

    public static class DefaultMetadata
    {
        public static List<string> Bossses = new List<string>() {
            "Giant Fly",
            "Zote Boss",
            "Buzzer R",
            "Ghost Warrior Slug",
            "White Defender",
            "Jellyfish GG",
            "Giant Buzzer Col",
            "Mega Fat Bee",
            "Lobster",
            "Lancer",
            "Mawlek Body",
            "False Knight New",
            "Mage Lord",
            "Mage Lord Phase2",
            "Mage Lord",
            "Mage Lord Phase2",
            "Black Knight",
            "Jar Collector",
            "Hornet Boss 1",
            "Giant Buzzer",
            "Mega Moss Charger",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Mantis Traitor Lord",
            "Grave Zombie",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Mimic Spider",
            "Hornet Boss 2",
            "Infected Knight",
            "Dung Defender",
            "Fluke Mother",
            "Ghost Warrior Galien",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "False Knight Dream",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Lost Kin",
            "Grey Prince",
            "Radiance",
            "Hollow Knight Boss",
            "HK Prime",
            "Pale Lurker",
            "Oro",
            "Mato",
            "Sheo Boss",
            "Absolute Radiance",
            "Sly Boss",
            "Hornet Nosk",
            };

        public static List<string> IsWallMounted = new List<string>() {
            "Ceiling Dropper",
            "Ceiling Dropper Col",
            "Mantis Flyer Child",
            "Plant Trap",
            "Plant Turret",
            "Plant Turret Right",
            "Mushroom Turret",
            "Laser Turret Frames",
            };

        public static List<string> Flying = new List<string>() {
            "Flamebearer Small",
            "Flamebearer Med",
            "Flamebearer Large",
            "Mosquito",
            "Bursting Bouncer",
            "Super Spitter",
            "Buzzer Col",
            "Giant Fly",
            "Blobble",
            "Colosseum_Armoured_Mosquito",
            "Colosseum_Flying_Sentry",
            "Angry Buzzer",
            "Mantis Heavy Flyer",
            "Fly",
            "Spitter R",
            "Jellyfish",
            "Lil Jellyfish",
            "Mantis Flyer Child",
            "Shade Sibling",
            "Colosseum_Armoured_Mosquito R",
            "Super Spitter R",
            "Buzzer",
            "Mega Fat Bee",
            "Giant Buzzer Col",
            "Mage",
            "Electric Mage",
            "Spitter",
            "Hatcher",
            "Ruins Sentry",
            "Ruins Flying Sentry",
            "Ruins Flying Sentry Javelin",
            "Mage Balloon",
            "Mage Lord",
            "Mage Lord Phase2",
            "Pigeon",
            "Acid Flyer",
            "Fat Fly",
            "Lazy Flyer Enemy",
            "Jellyfish Baby",
            "Moss Flyer",
            "Ghost Warrior Marmu",
            "Mega Jellyfish",
            "Ghost Warrior Xero",
            "Crystal Flyer",
            "Centipede Hatcher",
            "Blow Fly",
            "Bee Hatchling Ambient",
            "Ghost Warrior Markoth",
            "Spider Flyer",
            "Parasite Balloon",
            "Inflater",
            "Fluke Fly",
            "White Palace Fly",
            "Bee Stinger",
            "Big Bee",
            "Zote Balloon",
            "Radiance",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Zote Balloon Ordeal",
            "Zote Salubra",
            "Zote Fluke",
            "Hornet Nosk",
            "Fungus Flyer",
            "Hatcher Baby",
            };

        public static List<string> Crawling = new List<string>() {
            "Crawler",
            "Crystal Crawler",
            "Mines Crawler",
            };

        public static List<string> Climbing = new List<string>() {
            "Tiny Spider",
            "Climber",
            "Crystallised Lazer Bug",
            "Abyss Crawler",
            };

        public static List<string> Static = new List<string>() {
            "Blocker",
            "Egg Sac",
            "Plant Trap",
            "Mawlek Turret",
            "Mawlek Turret Ceiling",
            "fluke_baby_01",
            "fluke_baby_02",
            "fluke_baby_03",
            "Zote Turret",
            };

        public static List<string> IsSummonedByEvent = new List<string>() {
            "Giant Fly Col",
            "Buzzer Col",
            "Colosseum_Armoured_Roller",
            "Colosseum_Miner",
            "Colosseum_Armoured_Mosquito",
            "Colosseum_Flying_Sentry",
            "Ceiling Dropper Col",
            "Colosseum_Worm",
            "Mawlek Col",
            "Colosseum Grass Hopper",
            "Hatcher Baby",
            "Zombie Spider 2",
            "Zombie Spider 1",
            "Flukeman Top",
            "Flukeman Bot",
            "Colosseum_Armoured_Roller R",
            "Colosseum_Armoured_Mosquito R",
            "Giant Buzzer Col",
            "Mega Fat Bee",
            "Lobster",
            "Mage Knight",
            "Mage",
            "Electric Mage",
            "Mender Bug",
            "Mawlek Body",
            "False Knight New",
            "Mage Lord",
            "Mage Lord Phase2",
            "Black Knight",
            "Moss Knight",
            "Jar Collector",
            "Giant Buzzer",
            "Mega Moss Charger",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Mantis Traitor Lord",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Mimic Spider",
            "Hornet Boss 2",
            "Infected Knight",
            "Dung Defender",
            "Fluke Mother",
            "Ghost Warrior Galien",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "False Knight Dream",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Lost Kin",
            "Grey Prince",
            "Radiance",
            "Hollow Knight Boss",
            "HK Prime",
            "Pale Lurker",
            "Oro",
            "Mato",
            "Sheo Boss",
            "Absolute Radiance",
            "Sly Boss",
            "Hornet Nosk",
            };

        public static List<string> AlwaysDeleteObject = new List<string>() {
            "Fly Spawn",
            "Hatcher Spawn",
            "Hatcher Baby Spawner",
            "Parasite Balloon Spawner",
            };

        public static List<string> SpawnerEnemies = new List<string>() {
            "Zombie Hornhead Sp",
            "Zombie Runner Sp",
            "Centipede Hatcher",
            "Flukeman",
            "Zombie Hive",
            "fluke_baby_01",
            "fluke_baby_02",
            "fluke_baby_03",
            "Fluke Mother",
            "Hatcher",
            "Giant Fly",
            };

        public static Dictionary<string, List<string>> BattleEnemies = new Dictionary<string, List<string>>()
        {
            {"ANY",
                new List<string>()
                {
                    "Giant Fly Col",
                    "Buzzer Col",
                    "Colosseum_Armoured_Roller",
                    "Colosseum_Armoured_Mosquito",
                    "Colosseum_Flying_Sentry",
                    "Colosseum_Miner",
                    "Ceiling Dropper Col",
                    "Colosseum_Worm",
                    "Mawlek Col",
                    "Colosseum Grass Hopper",
                    "Colosseum_Armoured_Roller R",
                    "Colosseum_Armoured_Mosquito R",
                    "Giant Buzzer Col",
                    "Mega Fat Bee",
                    "Mage Knight",
                    "Electric Mage",
                    "Giant Buzzer",
                    "Mawlek Body",
                    "False Knight New",
                    "Jar Collector",
                    "Black Knight",
                }
            }
        };
    }
}
