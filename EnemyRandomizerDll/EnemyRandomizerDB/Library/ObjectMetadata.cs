using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using System.Linq;
using System.Reflection;
using static EnemyRandomizerMod.PrefabObject;
using UniRx;
using UniRx.Triggers;
using UniRx.Operators;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;

namespace EnemyRandomizerMod
{
    public class ObjectMetadata : System.IDisposable
    {
        public const int VERBOSE_HEADER_OFFSET = -1;
        public static bool VERBOSE_DEBUG = false;

        public ObjectMetadata() { GenerateReactives(); }
        public ObjectMetadata(GameObject obj) { GenerateReactives(); Setup(obj, EnemyRandomizerDatabase.GetDatabase()); }
        public ObjectMetadata(GameObject obj, EnemyRandomizerDatabase db) { GenerateReactives(); Setup(obj, db); }

        public ReactiveProperty<EnemyRandomizerDatabase> db { get; protected set; }
        public EnemyRandomizerDatabase DB
        {
            get => db.Value;
            set => db.Value = value;
        }

        IObservable<GameObject> nonNullSource;
        IObservable<string> validDBName;
        IObservable<HealthManager> nonNullHealthManager;
        IObservable<EnemyRandomizerDatabase> validDB;
        IObservable<string> validObjectName;
        IObservable<ObjectMetadata> validReplacement;
        IObservable<string> validScenePath;
        IObservable<string> validSceneName;
        IObservable<List<GameObject>> nonNullBB;

        protected virtual void GenerateBaseReactives()
        {
            source = new ReactiveProperty<GameObject>(null);
            objectThisReplaced = new ReactiveProperty<ObjectMetadata>(null);
            db = new ReactiveProperty<EnemyRandomizerDatabase>(null);
            objectName = new ReactiveProperty<string>(null);
            scenePath = new ReactiveProperty<string>(string.Empty);
            sceneName = new ReactiveProperty<string>(string.Empty);
            sizeScale = new ReactiveProperty<float>(1f);

            validDB = db.Where(x => x != null);
            validObjectName = objectName.Where(x => !string.IsNullOrEmpty(x));
            nonNullSource = source.Where(x => x != null);
            validReplacement = objectThisReplaced.Where(x => x != null);
            validScenePath = scenePath.Where(x => !string.IsNullOrEmpty(x));
            validSceneName = sceneName.Where(x => !string.IsNullOrEmpty(x));

            nonNullSource.Subscribe(x =>
            {
                objectName.Value = x.name;
                //if (VERBOSE_DEBUG)
                //    Dev.Log($"{ObjectName} - in:nonNullSource - {Dev.FunctionHeader(VERBSE_HEADER_OFFSET)}");
                scenePath.Value = x.GetSceneHierarchyPath();
                sceneName.Value = (x.scene.IsValid() ? x.scene.name : null);
            }).AddTo(disposables);

            hasData = validObjectName.CombineLatest(validDB, (x0, x1) =>
            {
                //if (VERBOSE_DEBUG)
                //    Dev.Log($"{ObjectName} - in:hasData - {Dev.FunctionHeader(VERBSE_HEADER_OFFSET)}");
                return EnemyRandomizerDatabase.IsDatabaseObject(x0, x1);
            }).ToReadOnlyReactiveProperty();

            databaseName = validObjectName.Where(_ => HasData).CombineLatest(hasData, (oname, hdata) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:databaseName - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                return hdata ? EnemyRandomizerDatabase.ToDatabaseKey(oname) : string.Empty;
            }).ToReadOnlyReactiveProperty();

            validDBName = databaseName.Where(x => !string.IsNullOrEmpty(x));
            objectType = validDBName.CombineLatest(validDB, (x0, x1) => MetaDataTypes.GetObjectType(x0, x1)).ToReadOnlyReactiveProperty();
            objectPrefab = validDBName.CombineLatest(validDB, (x0, x1) => MetaDataTypes.GetObjectPrefab(x0, x1)).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateObjectValidityReactives()
        {
            isInvalidObject = validScenePath.Select(x => MetaDataTypes.CheckIfIsBadObject(x)).ToReadOnlyReactiveProperty();

            blackBorders = EnemyRandomizerDatabase.GetBlackBorders().ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateTypeReactives()
        {
            isBoss = validDBName.Select(x => MetaDataTypes.Bosses.Contains(x)).ToReadOnlyReactiveProperty();
            tinker = nonNullSource.Where(_ => HasData).Select(x => x.GetComponentInChildren<TinkEffect>()).ToReadOnlyReactiveProperty();
            isTinker = tinker.Select(x => x != null).ToReadOnlyReactiveProperty();
            isFlyingFromComponents = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<Walker>() == null && x.GetComponent<Climber>() == null && x.GetComponent<Rigidbody2D>() != null && x.GetComponent<Rigidbody2D>().gravityScale == 0).ToReadOnlyReactiveProperty();
            isFlying = validDBName.Select(x => MetaDataTypes.Flying.Contains(x)).CombineLatest(isFlyingFromComponents, (x0, x1) => x0 || x1).ToReadOnlyReactiveProperty();

            isCrawling = validDBName.Select(x => MetaDataTypes.Crawling.Contains(x)).CombineLatest(nonNullSource, (x0, x1) => x0 || x1.GetComponent<Crawler>() != null).ToReadOnlyReactiveProperty();
            isClimbing = validDBName.Select(x => MetaDataTypes.Climbing.Contains(x)).CombineLatest(nonNullSource, (x0, x1) => x0 || x1.GetComponent<Climber>() != null).ToReadOnlyReactiveProperty();
            isMobile = validDBName.Select(x => !MetaDataTypes.Static.Contains(x)).ToReadOnlyReactiveProperty();

            isSmasher = nonNullSource.Where(_ => HasData).Select(x => x.EnumerateChildren().Any(y => y.name == "Smasher")).ToReadOnlyReactiveProperty();

            isSummonedByEvent = validDBName.Select(x => MetaDataTypes.IsSummonedByEvent.Contains(x)).ToReadOnlyReactiveProperty();
            isEnemySpawner = validDBName.Select(x => MetaDataTypes.SpawnerEnemies.Contains(x)).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateLogicReactives()
        {
            isPogoLogic = validObjectName.Where(_ => HasData).CombineLatest(objectThisReplaced, (x0, x1) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:isPogoLogic - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                return MetaDataTypes.IsPogoLogicType(x0) || (x1 != null && MetaDataTypes.IsPogoLogicType(x1.ObjectName));
            }).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateGameComponentReactives()
        {
            nonNullHealthManager = nonNullSource.Where(_ => HasData).Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:nonNullHealthManager - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                return x.GetComponent<HealthManager>();
            });

            enemyHealthManager = nonNullHealthManager.ToReadOnlyReactiveProperty();

            isInvincible = nonNullSource.Where(_ => HasData).CombineLatest(isTinker, (x0, x1) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:isInvincible - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                bool isHMInvincible = false;
                if (!x1)
                {
                    var hm = x0.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        isHMInvincible = hm.IsInvincible;
                    }
                }
                return isHMInvincible || x1;
            }).ToReadOnlyReactiveProperty();

            defaultHP = objectPrefab.Where(x => x.prefab != null).Select(x => x.prefab.GetComponent<HealthManager>())
                                    .Where(x => x != null).Select(x => x.hp).ToReadOnlyReactiveProperty();

            currentHP = nonNullHealthManager.Select(x => x.hp).ToReadOnlyReactiveProperty();

            geoManager = nonNullHealthManager.Select(x => new Geo(this)).ToReadOnlyReactiveProperty();

            heroDamage = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<DamageHero>()).ToReadOnlyReactiveProperty();
            damageDealt = heroDamage.Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:damageDealt - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (x == null)
                    return 0;
                return x.damageDealt;
            }).ToReadOnlyReactiveProperty();

            enemyDamage = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<DamageEnemies>()).ToReadOnlyReactiveProperty();
            enemyDamageDealt = enemyDamage.Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:enemyDamageDealt - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (x == null)
                    return 0;
                return x.damageDealt;
            }).ToReadOnlyReactiveProperty();

            walker = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<Walker>()).ToReadOnlyReactiveProperty();
            isWalker = walker.Select(x => x != null).ToReadOnlyReactiveProperty();

            physicsBody = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<Rigidbody2D>()).ToReadOnlyReactiveProperty();
            sceneSaveData = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<PersistentBoolItem>()).ToReadOnlyReactiveProperty();
            isDisabledBySavedGameState = sceneSaveData.Where(_ => HasData).Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:isDisabledBySavedGameState - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (x == null)
                    return false;

                if (x.isActiveAndEnabled)
                {
                    return x.persistentBoolData.activated;
                }
                else
                {
                    var data = global::SceneData.instance.FindMyState(x.persistentBoolData);
                    if (data != null)
                        return data.activated;
                    else
                        return false;
                }
            }).ToReadOnlyReactiveProperty();

            collider = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<Collider2D>()).ToReadOnlyReactiveProperty();
            mRenderer = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<MeshRenderer>()).ToReadOnlyReactiveProperty();
            sprite = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<tk2dSprite>()).ToReadOnlyReactiveProperty();
            deathEffects = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<EnemyDeathEffects>()).ToReadOnlyReactiveProperty();
            animator = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<tk2dSpriteAnimator>()).ToReadOnlyReactiveProperty();
            preInstantiateGO = nonNullSource.Where(_ => HasData).Select(x => x.GetComponent<PreInstantiateGameObject>()).ToReadOnlyReactiveProperty();

            corpse = deathEffects.Where(_ => HasData).Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:corpse - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (x == null)
                    return null;

                var c = x.GetCorpseFromDeathEffects();
                if (c != null)
                    return c;

                return null;
            }).ToReadOnlyReactiveProperty();

            customAvailableItem = new ReactiveProperty<GameObject>(null);
            availableItem = corpse.Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:availableItem - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (x == null)
                    return null;
                return x.GetComponent<PreInstantiateGameObject>();
            }).Select(x =>
            {
                if (x == null)
                    return null;
                return x.InstantiatedGameObject;
            }).Select(x =>
            {
                if (x == null)
                    return null;
                if (x.name.Contains("Shiny Item"))
                    return x.GetComponent<PersistentBoolItem>();
                return null;
            }).Select(x => {
                if (x == null)
                    return null;
                if (!x.persistentBoolData.activated)
                    return x.gameObject;
                return null;
            }).ToReadOnlyReactiveProperty();

            //hasAvailableItem = availableItem.Select(x => x != null).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateRandoReactives()
        {
            battleRandoObject = validReplacement.CombineLatest(nonNullSource, (x0, x1) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:battleRandoObject - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (!x0.IsBattleEnemy)
                    return null;

                BattleManagedObject bmo = x1.GetComponent<BattleManagedObject>();
                if (bmo == null)
                {
                    bmo = x1.AddComponent<BattleManagedObject>();
                    bmo.Setup(x0);
                    bmo.replaced = true;
                }
                return bmo;
            }).ToReadOnlyReactiveProperty();

            randoObject = validReplacement.CombineLatest(nonNullSource, (x0, x1) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:randoObject - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                ManagedObject mo = x1.GetComponent<ManagedObject>();
                if (!x0.IsBattleEnemy)
                {
                    if (mo == null)
                    {
                        mo = x1.AddComponent<ManagedObject>();
                        mo.Setup(x0);
                        mo.replaced = true;
                    }
                }
                return mo;
            }).ToReadOnlyReactiveProperty();


            var checkIsBoss = isBoss;
            var checkIsBattleRandoObject = battleRandoObject.Select(x => x != null);
            var checkIsScenePathBattleObject = validScenePath.Select(x => x.Split('/').Any(y => BattleManager.battleControllers.Any(z => y.Contains(z))));
            var checkIsANYInBattleEnemiesMap = validDBName.Select(x => MetaDataTypes.BattleEnemies["ANY"].Contains(x));
            var checkIsSCENEInBattleEnemiesMap = validDBName.CombineLatest(validSceneName, (x0, x1) => MetaDataTypes.BattleEnemies.ContainsKey(x1) && MetaDataTypes.BattleEnemies[x1].Contains(x0));
            var checkIsNonNullLocalBattleScene = nonNullHealthManager.Select(x => x.GetBattleScene()).Select(x => x != null);

            isBattleEnemy = isBoss.CombineLatest(
                checkIsBattleRandoObject,
                checkIsScenePathBattleObject,
                checkIsANYInBattleEnemiesMap,
                checkIsSCENEInBattleEnemiesMap,
                checkIsNonNullLocalBattleScene,
                (x0, x1, x2, x3, x4, x5) => x0 || x1 || x2 || x3 || x4 || x5).ToReadOnlyReactiveProperty();

            isAReplacementObject = objectThisReplaced.Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:isAReplacementObject - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                return x != null;
            }).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateTransformReactives()
        {
            nonNullSource.Where(_ => HasData && objectSize == null).Where(x => MetaDataTypes.HasUniqueSizeEnemies.Contains(EnemyRandomizerDatabase.ToDatabaseKey(x.name))).Subscribe(x => objectSize = new ReactiveProperty<Vector2>(MetaDataTypes.SetSizeFromUniqueObject(x))).AddTo(disposables);
            nonNullSource.Where(_ => HasData && objectSize == null).Where(x => !MetaDataTypes.HasUniqueSizeEnemies.Contains(EnemyRandomizerDatabase.ToDatabaseKey(x.name))).Subscribe(x => objectSize = new ReactiveProperty<Vector2>(MetaDataTypes.SetSizeFromComponents(x))).AddTo(disposables);

            objectPosition = nonNullSource.Where(_ => HasData).Select(x => x.transform).SelectMany(x => x.ObserveEveryValueChanged(y => y.position.ToVec2())).ToReadOnlyReactiveProperty();
            objectScale = nonNullSource.Where(_ => HasData && ObjectType != PrefabType.None).Select(x => x.transform).SelectMany(x => x.ObserveEveryValueChanged(y => y.localScale.ToVec2())).ToReadOnlyReactiveProperty();
            rotation = nonNullSource.Where(_ => HasData && ObjectType != PrefabType.None).Select(x => x.transform).SelectMany(x => x.ObserveEveryValueChanged(y => y.localEulerAngles.z)).ToReadOnlyReactiveProperty();
        }

        IEnumerable<GameObject> bb_leftRight;
        IEnumerable<GameObject> bb_topBot;
        float bb_xmin = float.MaxValue;
        float bb_xmax = float.MinValue;
        float bb_ymin = float.MaxValue;
        float bb_ymax = float.MinValue;

        protected virtual void GenerateIsVisibleReactives()
        {
            nonNullBB = blackBorders.Where(_ => HasData).Where(x => x != null && x.Count > 0);

            nonNullBB.Where(_ => HasData).Subscribe(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:nonNullBB - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                bb_leftRight = x.Where(z => z.transform.localScale.x == 20);
                bb_topBot = x.Where(z => z.transform.localScale.y == 20);

                bb_xmin = bb_leftRight.Min(o => (o.transform.position.x - 10f));
                bb_xmax = bb_leftRight.Max(o => (o.transform.position.x + 10f));
                bb_ymin = bb_topBot.Min(o => (o.transform.position.y - 10f));
                bb_ymax = bb_topBot.Max(o => (o.transform.position.y + 10f));

                //Dev.Log($"{ScenePath} pos:{pos}");
                //Dev.Log($"{ScenePath} BOUNDS[ xmin:{xmin} xmax:{xmax} ymin:{ymin} ymax:{ymax}]");
            }).AddTo(disposables);

            activeSelf = nonNullSource.Select(x => x.activeSelf).ToReadOnlyReactiveProperty();
            inBounds = objectPosition.CombineLatest(blackBorders, (pos, _) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:inBounds - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (pos.x < bb_xmin)
                    return false;
                else if (pos.x > bb_xmax)
                    return false;
                else if (pos.y < bb_ymin)
                    return false;
                else if (pos.y > bb_ymax)
                    return false;

                return true;
            }).ToReadOnlyReactiveProperty();

            renderersVisible = collider.CombineLatest(mRenderer, (col, mr) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:renderersVisible - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
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
            }).ToReadOnlyReactiveProperty();

            isVisible = renderersVisible.CombineLatest(inBounds, activeSelf,
                (isrender, inbounds, active) =>
                {
                    if (VERBOSE_DEBUG)
                        Dev.Log($"{ObjectName} - in:isVisible - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                    return (active && inbounds && isrender);
                }).ToReadOnlyReactiveProperty();

            isActive = isVisible.CombineLatest(isDisabledBySavedGameState, (isVisible, isDisabled) =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:isActive - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                return !isDisabled && isVisible;
            }).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateReplacementReactives()
        {
            activeInHeirarchy = nonNullSource.Select(x =>
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:activeInHeirarchy - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                return x.activeInHierarchy;
            }).ToReadOnlyReactiveProperty();

            isTemporarilyInactive = hasData.CombineLatest(isAReplacementObject, activeInHeirarchy, nonNullHealthManager, isDisabledBySavedGameState, isActive,
                (has, isRepl, goActive, nnHM, isDisabled, active) =>
                {
                    if (VERBOSE_DEBUG)
                        Dev.Log($"{ObjectName} - in:isTemporarilyInactive - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                    return has && !isRepl && goActive && nnHM && !isDisabled && !active;
                }).ToReadOnlyReactiveProperty();

            isBattleInactive = hasData.CombineLatest(isAReplacementObject, isBattleEnemy, activeInHeirarchy, nonNullHealthManager, isDisabledBySavedGameState, isActive,
                (has, isRepl, isBE, goActive, nnHM, isDisabled, active) =>
                {
                    if (VERBOSE_DEBUG)
                        Dev.Log($"{ObjectName} - in:isBattleInactive - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                    return has && !isRepl && isBE && !goActive && nnHM && !isDisabled && !active;
                }).ToReadOnlyReactiveProperty();

            canProcessObject = hasData.CombineLatest(isInvalidObject, isActive, objectType, isAReplacementObject,
                (has, isInvalidObject, isActive, oT, isAReplacementO) =>
                {
                    if (VERBOSE_DEBUG)
                        Dev.Log($"{ObjectName} - in:canProcessObject - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                    return has && !isInvalidObject && (isActive || oT == PrefabType.Effect) && !isAReplacementO;
                }).ToReadOnlyReactiveProperty();
        }

        protected virtual void GenerateReactives()
        {
            GenerateBaseReactives();
            GenerateObjectValidityReactives();
            GenerateTypeReactives();
            GenerateLogicReactives();
            GenerateGameComponentReactives();
            GenerateRandoReactives();
            GenerateTransformReactives();
            GenerateIsVisibleReactives();
            GenerateReplacementReactives();
        }

        public virtual void MarkObjectAsReplacement(ObjectMetadata oldObject)
        {
            ObjectThisReplaced = oldObject;
        }

        public bool Setup(GameObject sceneObject, EnemyRandomizerDatabase database)
        {
            DB = database;
            Source = sceneObject;

            if (IsInvalidObject || !HasData)
                return false;

            return true;
        }

        public ReactiveProperty<GameObject> source { get; protected set; }
        public GameObject Source
        {
            get => source.Value;
            set => source.Value = value;
        }

        public ReactiveProperty<string> sceneName { get; protected set; }
        public string SceneName => sceneName == null ? null : sceneName.Value;

        public ReactiveProperty<string> objectName { get; protected set; }
        public string ObjectName => objectName == null ? null : objectName.Value;

        public ReactiveProperty<string> scenePath { get; protected set; }
        public string ScenePath => scenePath == null ? null : scenePath.Value;

        public ReactiveProperty<Vector2> objectSize { get; protected set; }
        public Vector2 ObjectSize
        {
            get => objectSize == null ? Vector2.zero : objectSize.Value;
            set { if (objectSize != null) objectSize.Value = value; }
        }

        public ReadOnlyReactiveProperty<string> databaseName { get; protected set; }
        public string DatabaseName
        {
            get => databaseName == null ? null : databaseName.Value;
        }

        public ReadOnlyReactiveProperty<Vector2> objectPosition { get; protected set; }
        public Vector2 ObjectPosition
        {
            get => objectPosition == null ? (Source == null ? Vector2.zero : Source.transform.position.ToVec2()) : objectPosition.Value;
            set { if (Source != null) Source.transform.position = new Vector3(value.x, value.y, Source.transform.position.z); }
        }

        public ReadOnlyReactiveProperty<Vector2> objectScale { get; protected set; }
        public Vector2 ObjectScale
        {
            get => objectScale == null ? (Source == null ? Vector2.one : Source.transform.localScale.ToVec2()) : objectScale.Value;
            set { if (Source != null) Source.transform.localScale = new Vector3(value.x, value.y, Source.transform.localScale.z); }
        }

        public ReadOnlyReactiveProperty<float> rotation { get; protected set; }
        public float Rotation
        {
            get => rotation == null ? (Source == null ? 0f : Source.transform.localEulerAngles.z) : rotation.Value;
            set { if (Source != null) Source.transform.localEulerAngles = new Vector3(Source.transform.localEulerAngles.x, Source.transform.localEulerAngles.y, value); }
        }

        public ReactiveProperty<ObjectMetadata> objectThisReplaced { get; protected set; }
        public ObjectMetadata ObjectThisReplaced
        {
            get => objectThisReplaced.Value;
            set => objectThisReplaced.Value = value;
        }

        public ReactiveProperty<float> sizeScale { get; protected set; }
        public float SizeScale
        {
            get => sizeScale == null ? 0 : sizeScale.Value;
            set { if (sizeScale != null) sizeScale.Value = value; }
        }

        public ReadOnlyReactiveProperty<bool> hasData { get; protected set; }
        public bool HasData
        {
            get => hasData == null ? false : hasData.Value;
        }

        public ReadOnlyReactiveProperty<PrefabObject.PrefabType> objectType { get; protected set; }
        public PrefabObject.PrefabType ObjectType
        {
            get => objectType == null ? PrefabType.None : objectType.Value;
        }

        public ReadOnlyReactiveProperty<PrefabObject> objectPrefab { get; protected set; }
        public PrefabObject ObjectPrefab
        {
            get => objectPrefab == null ? null : objectPrefab.Value;
        }

        public ReadOnlyReactiveProperty<bool> isInvalidObject { get; protected set; }
        public bool IsInvalidObject
        {
            get => isInvalidObject == null ? true : isInvalidObject.Value;
        }

        public string MapZone => GameManager.instance.GetCurrentMapZone();

        public ReadOnlyReactiveProperty<bool> isDisabledBySavedGameState { get; protected set; }
        public bool IsDisabledBySavedGameState
        {
            get => isDisabledBySavedGameState == null ? false : isDisabledBySavedGameState.Value;
        }

        public ReadOnlyReactiveProperty<bool> isBoss { get; protected set; }
        public bool IsBoss
        {
            get => isBoss == null ? false : isBoss.Value;
        }

        public ReadOnlyReactiveProperty<bool> isFlyingFromComponents { get; protected set; }

        public ReadOnlyReactiveProperty<bool> isFlying { get; protected set; }
        public bool IsFlying
        {
            get => isFlying == null ? false : isFlying.Value;
        }

        public ReadOnlyReactiveProperty<bool> isCrawling { get; protected set; }
        public bool IsCrawling
        {
            get => isCrawling == null ? false : isCrawling.Value;
        }

        public ReadOnlyReactiveProperty<bool> isClimbing { get; protected set; }
        public bool IsClimbing
        {
            get => isClimbing == null ? false : isClimbing.Value;
        }

        public ReadOnlyReactiveProperty<TinkEffect> tinker { get; protected set; }
        public TinkEffect Tinker
        {
            get => tinker == null ? (Source == null ? null : Source.GetComponent<TinkEffect>()) : tinker.Value;
        }

        public ReadOnlyReactiveProperty<bool> isTinker { get; protected set; }
        public bool IsTinker
        {
            get => isTinker == null ? (Source == null ? false : Source.GetComponent<TinkEffect>() != null) : isTinker.Value;
        }

        public ReadOnlyReactiveProperty<bool> isMobile { get; protected set; }
        public bool IsMobile
        {
            get => isMobile == null ? false : isMobile.Value;
        }

        public ReadOnlyReactiveProperty<bool> isWalker { get; protected set; }
        public bool IsWalker
        {
            get => isWalker == null ? false : isWalker.Value;
        }

        public ReadOnlyReactiveProperty<bool> isBattleEnemy { get; protected set; }
        public bool IsBattleEnemy
        {
            get => isBattleEnemy == null ? false : isBattleEnemy.Value;
        }

        //public ReadOnlyReactiveProperty<bool> hasAvailableItem { get; protected set; }
        public bool HasAvailableItem
        {
            get
            {
                if (customAvailableItem != null && customAvailableItem.Value != null)
                    return true;

                if (availableItem != null && customAvailableItem.Value != null)
                    return true;

                return false;

                //hasAvailableItem == null ? false : hasAvailableItem.Value;
            }
        }

        public ReadOnlyReactiveProperty<bool> isSmasher { get; protected set; }
        public bool IsSmasher => isSmasher == null ? false : isSmasher.Value;

        public ReadOnlyReactiveProperty<bool> isPogoLogic { get; protected set; }
        public bool IsPogoLogic
        {
            get => isPogoLogic == null ? false : isPogoLogic.Value;
        }

        public ReadOnlyReactiveProperty<int> currentHP { get; protected set; }
        public int CurrentHP
        {
            get => currentHP == null ? -1 : currentHP.Value;
            set { if (EnemyHealthManager != null) EnemyHealthManager.hp = value; }
        }
        public float CurrentHPf
        {
            get => CurrentHP;
            set => CurrentHP = Mathf.FloorToInt(value);
        }

        public ReadOnlyReactiveProperty<int> defaultHP { get; protected set; }
        public int DefaultHP
        {
            get => defaultHP == null ? -1 : defaultHP.Value;
        }

        public ReadOnlyReactiveProperty<bool> isInvincible { get; protected set; }
        public bool IsInvincible
        {
            get => isInvincible == null ? false : isInvincible.Value;
            set { if (EnemyHealthManager != null) { EnemyHealthManager.IsInvincible = value; } }
        }

        public ReadOnlyReactiveProperty<int> damageDealt { get; protected set; }
        public int DamageDealt
        {
            get => damageDealt == null ? 0 : damageDealt.Value;
            set { if (damageDealt != null && Source != null && Source.GetComponent<DamageHero>() != null) Source.GetComponent<DamageHero>().damageDealt = value; }
        }

        public ReadOnlyReactiveProperty<int> enemyDamageDealt { get; protected set; }
        public int EnemyDamageDealt
        {
            get => enemyDamageDealt == null ? 0 : enemyDamageDealt.Value;
            set { if (enemyDamageDealt != null && Source != null && Source.GetComponent<DamageEnemies>() != null) Source.GetComponent<DamageEnemies>().damageDealt = value; }
        }

        public ReadOnlyReactiveProperty<bool> isActive { get; protected set; }
        public bool IsActive
        {
            get => isActive == null ? false : isActive.Value;
        }

        public ReadOnlyReactiveProperty<bool> isSummonedByEvent { get; protected set; }
        public bool IsSummonedByEvent
        {
            get => isSummonedByEvent == null ? false : isSummonedByEvent.Value;
        }

        public ReadOnlyReactiveProperty<bool> isEnemySpawner { get; protected set; }
        public bool IsEnemySpawner
        {
            get => isEnemySpawner == null ? false : isEnemySpawner.Value;
        }

        public ReadOnlyReactiveProperty<List<GameObject>> blackBorders { get; protected set; }
        public List<GameObject> BlackBorders
        {
            get => blackBorders == null ? null : blackBorders.Value;
        }

        //These values will become null after a replacement
        public ReadOnlyReactiveProperty<GameObject> availableItem { get; protected set; }
        public ReactiveProperty<GameObject> customAvailableItem { get; protected set; }
        public GameObject AvailableItem
        {
            get
            {
                if (customAvailableItem != null && customAvailableItem.Value != null)
                    return customAvailableItem.Value;

                if (availableItem != null)
                    return availableItem.Value;

                return null;
            }
            set
            {
                if (customAvailableItem == null)
                    customAvailableItem = new ReactiveProperty<GameObject>(value);
                else
                    customAvailableItem.Value = value;
            }
        }

        public ReadOnlyReactiveProperty<PersistentBoolItem> sceneSaveData { get; protected set; }
        public PersistentBoolItem SceneSaveData => sceneSaveData == null ? null : sceneSaveData.Value;

        public ReadOnlyReactiveProperty<HealthManager> enemyHealthManager { get; protected set; }
        public HealthManager EnemyHealthManager => enemyHealthManager == null ? (Source == null ? null : Source.GetComponent<HealthManager>()) : enemyHealthManager.Value;

        public ReadOnlyReactiveProperty<Geo> geoManager { get; protected set; }
        public int Geo
        {
            get => geoManager == null ? 0 : geoManager.Value;
            set { if (geoManager != null) {
                    var geo = geoManager.Value;
                    geo.Value = value; } }
        }

        public ReadOnlyReactiveProperty<Collider2D> collider { get; protected set; }
        public Collider2D Collider => collider == null ? null : collider.Value;

        public ReadOnlyReactiveProperty<MeshRenderer> mRenderer { get; protected set; }
        public MeshRenderer MRenderer => mRenderer == null ? null : mRenderer.Value;

        public ReadOnlyReactiveProperty<bool> renderersVisible { get; protected set; }
        public bool RenderersVisible => renderersVisible == null ? false : renderersVisible.Value;

        public ReadOnlyReactiveProperty<bool> inBounds { get; protected set; }
        public bool InBounds
        {
            get
            {
                if (VERBOSE_DEBUG)
                    Dev.Log($"{ObjectName} - in:inBounds - {Dev.FunctionHeader(VERBOSE_HEADER_OFFSET)}");
                if (ObjectPosition.x < bb_xmin)
                    return false;
                else if (ObjectPosition.x > bb_xmax)
                    return false;
                else if (ObjectPosition.y < bb_ymin)
                    return false;
                else if (ObjectPosition.y > bb_ymax)
                    return false;

                return true;
            }
        }

        public ReadOnlyReactiveProperty<bool> activeSelf { get; protected set; }
        public bool ActiveSelf => activeSelf == null ? ( Source == null ? false : Source.activeSelf) : activeSelf.Value;

        public ReadOnlyReactiveProperty<bool> activeInHeirarchy { get; protected set; }
        public bool ActiveInHeirarchy => activeInHeirarchy == null ? (Source == null ? false : Source.activeInHierarchy) : activeInHeirarchy.Value;

        public ReadOnlyReactiveProperty<tk2dSprite> sprite { get; protected set; }
        public tk2dSprite Sprite => sprite == null ? null : sprite.Value;

        public ReadOnlyReactiveProperty<tk2dSpriteAnimator> animator { get; protected set; }
        public tk2dSpriteAnimator Animator => animator == null ? null : animator.Value;

        public ReadOnlyReactiveProperty<DamageHero> heroDamage { get; protected set; }
        public DamageHero HeroDamage => heroDamage == null ? null : heroDamage.Value;

        public ReadOnlyReactiveProperty<DamageEnemies> enemyDamage { get; protected set; }
        public DamageEnemies EnemyDamage => enemyDamage == null ? null : enemyDamage.Value;

        public ReadOnlyReactiveProperty<Walker> walker { get; protected set; }
        public Walker Walker => walker == null ? null : walker.Value;

        public ReadOnlyReactiveProperty<GameObject> corpse { get; protected set; }
        public GameObject Corpse => corpse == null ? null : corpse.Value;

        public ReadOnlyReactiveProperty<EnemyDeathEffects> deathEffects { get; protected set; }
        public EnemyDeathEffects DeathEffects => deathEffects == null ? null : deathEffects.Value;

        public ReadOnlyReactiveProperty<ManagedObject> randoObject { get; protected set; }
        public ManagedObject RandoObject => randoObject == null ? null : randoObject.Value;

        public ReadOnlyReactiveProperty<BattleManagedObject> battleRandoObject { get; protected set; }
        public BattleManagedObject BattleRandoObject => battleRandoObject == null ? null : battleRandoObject.Value;

        public ReadOnlyReactiveProperty<Rigidbody2D> physicsBody { get; protected set; }
        public Rigidbody2D PhysicsBody => physicsBody == null ? null : physicsBody.Value;

        public ReadOnlyReactiveProperty<PreInstantiateGameObject> preInstantiateGO { get; protected set; }
        public PreInstantiateGameObject PreInstantiateGO => preInstantiateGO == null ? null : preInstantiateGO.Value;

        public ReadOnlyReactiveProperty<bool> isAReplacementObject { get; protected set; }
        public bool IsAReplacementObject => isAReplacementObject == null ? false : isAReplacementObject.Value;

        public ReadOnlyReactiveProperty<bool> isVisible { get; protected set; }
        public bool IsVisible => isVisible == null ? false : isVisible.Value;

        public ReadOnlyReactiveProperty<bool> isTemporarilyInactive { get; protected set; }
        public bool IsTemporarilyInactive
        {
            get => isTemporarilyInactive == null ? false : isTemporarilyInactive.Value;
        }
        public ReadOnlyReactiveProperty<bool> isBattleInactive { get; protected set; }
        public bool IsBattleInactive
        {
            get => isBattleInactive == null ? false : isBattleInactive.Value;
        }
        public ReadOnlyReactiveProperty<bool> canProcessObject { get; protected set; }
        public bool CanProcessObject
        {
            get => canProcessObject == null ? false : canProcessObject.Value;
        }

        public bool IsInGroundEnemy
        {
            get
            {
                return MetaDataTypes.InGroundEnemy.Contains(DatabaseName);
            }
        }

        public ReactiveProperty<string> playerDataName { get; protected set; }
        public string PlayerDataName
        {
            get
            {
                return playerDataName == null ? null : playerDataName.Value;
            }
            set
            {
                IsCustomPlayerDataName = true;
                if (playerDataName != null)
                {
                    playerDataName.Value = value;
                }
                else
                {
                    playerDataName = new ReactiveProperty<string>(value);
                }
            }
        }
        public bool IsCustomPlayerDataName { get; protected set; }

        protected CompositeDisposable disposables = new CompositeDisposable();

        public virtual void DestroySource(bool disableObjectBeforeDestroy = true)
        {
            Dev.Log($"{ScenePath} Destroying This");
            if (Source != null)
            {
                if (ObjectName.Contains("Fly") && SceneName == "Crossroads_04")
                {
                    //this seems to correctly decrement the count from the battle manager
                    BattleManager.StateMachine.Value.RegisterEnemyDeath(null);
                }

                if (disableObjectBeforeDestroy)
                {
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

        public virtual void SetPosition(Vector2 pos)
        {
            ObjectPosition = pos;
        }

        public virtual void ApplySizeScale(float scale)
        {
            SizeScale = scale;
            ObjectScale = new Vector2(ObjectScale.x * scale, ObjectScale.y * scale);
            
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

        public virtual void SetAudioToMatchScale()
        {
            SetAudioToMatchScale(Source);

            //TODO: add an option to include the corpse
            GameObject corpse = SpawnerExtensions.GetCorpseObject(Source);
            if (corpse != null)
            {
                SetAudioToMatchScale(corpse);
            }
        }

        public virtual void SetAudioToMatchScale(GameObject go)
        {
            if (Source == null || Mathnv.FastApproximately(SizeScale, 1f, .01f))
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

            if (SizeScale < 1f)
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
                float ratio = SizeScale / range;
                float inver = 1f - ratio;
                float fpitch = 1f + (pitch_max - 1f) * inver;

                t = inver;
                pitch = fpitch;

                //t = 1f - size_rangeBot.NormalizedValue(SizeScale);
                //pitch = pitch_rangeTop.Evaluate(t);
            }
            else//if(SizeScale > 1f)
            {

                float range = 2.5f - 1f;
                float ratio = SizeScale / range;

                if (ratio > 1f)
                    ratio = 1f;

                float inver = 1f - ratio;
                float fpitch = 1f - (1f - pitch_min) * inver;

                t = inver;
                pitch = fpitch;


                //t = 1f - size_rangeTop.NormalizedValue(SizeScale);
                //pitch = pitch_rangeBot.Evaluate(t);
            }

            Dev.Log($"{ScenePath} audio pitch from size {SizeScale} to t {t} to pitch {pitch}");

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

                    playerDataName = new ReactiveProperty<string>(oplayerDataName);

                    nedf.SetPlayerDataNameFromDeathEffects(oplayerDataName);
                }

                if (playerDataName == null)
                {
                    playerDataName = new ReactiveProperty<string>(string.Empty);

                    string result = GetCustomPlayerDataNameFromReplacement(ObjectThisReplaced.DatabaseName);
                    if(!string.IsNullOrEmpty(result))
                    {
                        //this sets custom to true
                        PlayerDataName = result;
                    }
                    else
                    {
                        IsCustomPlayerDataName = false;
                    }
    
                    if (IsCustomPlayerDataName && EnemyHealthManager != null)
                    {
                        EnemyHealthManager.OnDeath -= RecordCustomJournalOnDeath;
                        EnemyHealthManager.OnDeath += RecordCustomJournalOnDeath;
                    }
                }
            }

            Source.SafeSetActive(true);

            if (ObjectThisReplaced != null && this != ObjectThisReplaced)
                ObjectThisReplaced.DestroySource();

            return Source;
        }

        protected virtual string GetCustomPlayerDataNameFromReplacement(string replacementName)
        {
            if(MetaDataTypes.CustomReplacementName.TryGetValue(replacementName, out string customName))
            {
                return customName;
            }

            return string.Empty;
        }

        protected virtual void RecordCustomJournalOnDeath()
        {
            if (!IsCustomPlayerDataName)
                return;


            PlayerData playerData = GameManager.instance.playerData;
            string text = "killed" +   PlayerDataName;
            string text2 = "kills" +   PlayerDataName;
            string text3 = "newData" + PlayerDataName;
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
                    EnemyRandomizerDatabase.CustomSpawnWithLogic(Source.transform.position, "Death Explode Boss", null, true);
                }
            }
        }

        public virtual void FixForLogic()
        {
            //for now we only need to do logic if we are an enemy
            if (ObjectType != PrefabType.Enemy)
                return;

            if (Source == null)
                return;

            //didn't replace anything
            if (ObjectThisReplaced == null)
                return;

            //did we replace an enemy?
            if (ObjectThisReplaced.ObjectType != PrefabType.Enemy)
                return;

            //no fix needed
            if (ObjectThisReplaced != null && this == ObjectThisReplaced)
                return;

            //did we replace a pogo logic enemy?
            if (ObjectThisReplaced.IsPogoLogic)
            {
                float tweenDirection = 1f;
                float tweenRate = 3f;

                if (ObjectThisReplaced.IsFlying && ObjectThisReplaced.IsMobile)
                {
                    var tweener = ObjectThisReplaced.Source.GetComponentsInChildren<PlayMakerFSM>().FirstOrDefault(x => x.FsmName == "Tween");
                    tweenDirection = tweener.FsmVariables.GetFsmVector3("Move Vector").Value.y;
                    tweenRate = tweener.FsmVariables.GetFsmFloat("Speed").Value;
                }

                StripMovements(true);
                MakeTinker(true);
                PlayIdleAnimation();

                var collider = Source.GetComponent<BoxCollider2D>();
                if(collider != null)
                {
                    collider.enabled = true;
                }

                //this will be the white palace fly
                if (!ObjectThisReplaced.IsMobile)
                {
                    SetPosition(ObjectThisReplaced.Source.transform.position);
                    LockIntoPosition(ObjectThisReplaced.Source.transform.position);
                } 
                //this will be acid flyer or acid walker
                else if (ObjectThisReplaced.IsTinker)
                {
                    if (ObjectThisReplaced.IsFlying)
                    {
                        //v = d / t
                        //v t = d
                        //t = d / v
                        //tween travel time
                        var tween = Source.GetOrAddComponent<CustomTweener>();
                        tween.from = ObjectThisReplaced.Source.transform.position;
                        tween.to = tween.from + Vector3.up * 1f * tweenDirection;
                        tween.travelTime = Mathf.Abs( (tweenRate / tweenDirection) * 4f );
                    }
                    //walker
                    else
                    {
                        if(!IsWalker)
                        {
                            var walker = Source.GetOrAddComponent<Walker>();
                            walker.walkSpeedL = 2f;
                            walker.walkSpeedR = 2f;
                            walker.startInactive = false;
                            walker.ignoreHoles = false;
                            walker.StartMoving();
                        }
                    }
                }
            }

            if (ObjectThisReplaced.IsSmasher)
            {
                MakeSmasher();

                //TODO: if this was a static replacement we need to give it movement.... or solve some other way
            }
        }

        public virtual void StripMovements(bool makeStatic)
        {
            if (Source == null)
                return;

            var fsms = Source.GetComponents<PlayMakerFSM>();
            fsms.ToList().ForEach(x => GameObject.Destroy(x));

            var actives = Source.GetComponents<FSMActivator>();
            actives.ToList().ForEach(x => GameObject.Destroy(x));

            {
                var p = Source.GetComponents<PlayMakerCollisionEnter>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }
            {
                var p = Source.GetComponents<PlayMakerFixedUpdate>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }
            {
                var p = Source.GetComponents<PlayMakerCollisionExit>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }
            {
                var p = Source.GetComponents<PlayMakerCollectionProxy>();
                p.ToList().ForEach(x => GameObject.Destroy(x));
            }

            if (makeStatic)
            {
                var body = Source.GetOrAddComponent<Rigidbody2D>();
                PhysicsBody.isKinematic = true;
                PhysicsBody.gravityScale = 0f;
            }
        }

        public virtual void LockIntoPosition(Vector3 pos)
        {
            var posLock = Source.GetOrAddComponent<PositionFixer>();
            posLock.positionLock = pos;
        }

        public virtual void PlayIdleAnimation()
        {
            var anim = Source.GetComponent<tk2dSpriteAnimator>();
            if (anim == null)
                return;

            bool playedIdle = false;
            if(IsFlying)
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

            if(!playedIdle)
            {
                anim.Play(anim.DefaultClip);
                playedIdle = true;
            }
        }

        public virtual void MakeSmasher()
        {
            var smasher = Source.CopyAndAddSmasher(ObjectThisReplaced);
            var col = smasher.GetComponent<BoxCollider2D>();
            var size = ObjectSize;
            col.size = size * 1.2f;

            smasher.transform.localPosition = Vector3.zero;
            smasher.SetActive(true);
        }

        public virtual void MakeTinker(bool makeInvincible)
        {
            var tinkEffect = Source.GetOrAddComponent<TinkEffect>();
            tinkEffect.blockEffect = DB.Spawn(EnemyRandomizerDatabase.BlockHitEffectName, null);            
            if (makeInvincible)
            {
                IsInvincible = true;
            }
        }

        public virtual void ImportItem(ObjectMetadata other)
        {
            AvailableItem = other.AvailableItem;
        }

        static bool DEBUG_WARN_IF_NOT_FOUND = false;

        public void Dump()
        {
            var self = this;
            var props = self.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            props.ToList().ForEach(x => Dev.Log($"{x.Name}: {x.GetValue(self)}"));
        }

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

        public void Dispose()
        {
            ((System.IDisposable)disposables).Dispose();
        }
    }

    public static class MetaDataTypes
    {
        public static PrefabType GetObjectType(string databaseName, EnemyRandomizerDatabase db)
        {
            if (string.IsNullOrEmpty(databaseName) || db == null)
                return PrefabType.None;

            if (db.Enemies.ContainsKey(databaseName))
                return PrefabType.Enemy;

            if (db.Hazards.ContainsKey(databaseName))
                return PrefabType.Hazard;

            if (db.Effects.ContainsKey(databaseName))
                return PrefabType.Effect;

            if (db.Others.ContainsKey(databaseName))
                return PrefabType.Other;

            return PrefabType.None;
            //ObjectPrefab = database.Objects[databaseName];
        }

        public static PrefabObject GetObjectPrefab(string databaseName, EnemyRandomizerDatabase db)
        {
            if (string.IsNullOrEmpty(databaseName) || db == null)
                return null;

            if (db.Objects.TryGetValue(databaseName, out PrefabObject val))
                return val;
            return null;
        }

        public static Vector2 SetSizeFromUniqueObject(GameObject sceneObject)
        {
            Dev.Log($"Setting size for object {sceneObject}");
            if (EnemyRandomizerDatabase.ToDatabaseKey(sceneObject.name) == "Acid Flyer")
            {
                Dev.Log($"Looking for shell in {sceneObject}");
                var shell = sceneObject.FindGameObjectInChildrenWithName("Shell");
                if(shell == null)
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

        public static Vector2 SetSizeFromComponents(GameObject sceneObject)
        {
            Vector2 result = Vector2.one;
            if (sceneObject.GetComponent<tk2dSprite>() && sceneObject.GetComponent<tk2dSprite>().boxCollider2D != null)
            {
                result = sceneObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                Dev.Log($"Size of SPRITE {sceneObject} is {result}");
            }
            else if (sceneObject.GetComponent<BoxCollider2D>())
            {
                result = sceneObject.GetComponent<BoxCollider2D>().size;
                Dev.Log($"Size of BOX {sceneObject} is {result}");
            }
            else if (sceneObject.GetComponent<CircleCollider2D>())
            {
                var newCCircle = sceneObject.GetComponent<CircleCollider2D>();
                result = Vector2.one * newCCircle.radius;
                Dev.Log($"Size of CIRCLE {sceneObject} is {result}");
            }
            else if (sceneObject.GetComponent<PolygonCollider2D>())
            {
                var newCPoly = sceneObject.GetComponent<PolygonCollider2D>();
                result = new Vector2(newCPoly.points.Select(x => x.x).Max() - newCPoly.points.Select(x => x.x).Min(), newCPoly.points.Select(x => x.y).Max() - newCPoly.points.Select(x => x.y).Min());

                Dev.Log($"Size of POLYGON {sceneObject} is {result}");
            }
            else
            {
                result = sceneObject.transform.localScale;
                Dev.Log($"Size of TRANSFORM SCALE {sceneObject} is {result}");

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

        public static bool CheckIfIsPogoLogicType(string objectName, ObjectMetadata objectThisReplaced)
        {
            bool result = IsPogoLogicType(objectName);
            return result || (objectThisReplaced != null && IsPogoLogicType(objectThisReplaced.ObjectName));
        }


        public static bool CheckIfIsBadObject(string scenePath)
        {
            return MetaDataTypes.AlwaysDeleteObject.Any(x => scenePath.Contains(x));
        }

        public static List<string> Bosses = new List<string>() {
            "Giant Fly",
            "Mega Moss Charger",
            "Mawlek Body",

            "Black Knight",
            "Fluke Mother",
            "Mantis Traitor Lord",

            "Hornet Boss 1",
            "Hornet Boss 2",

            "Giant Buzzer",
            "Giant Buzzer Col",
            "Mega Fat Bee", //obblobble boss
            "Lobster",
            "Lancer",

            "Dung Defender",
            "White Defender",

            "Mega Zombie Beam Miner",
            "Zombie Beam Miner Rematch",

            "Grimm Boss",
            "Nightmare Grimm Boss",

            "Infected Knight",
            "Lost Kin",

            "False Knight New",
            "False Knight Dream",

            "Mage Lord",
            "Mage Lord Phase2",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",

            "Jar Collector",
            "Hornet Nosk",
            "Mimic Spider",
            "Mega Jellyfish",
            "Jellyfish GG",

            "Hive Knight",

            "Pale Lurker",
            "Oro",
            "Mato",
            "Sheo Boss",
            "Sly Boss",

            "Grey Prince",
            "Zote Boss",

            "Hollow Knight Boss",
            "HK Prime",

            "Radiance",
            "Absolute Radiance",

            "Ghost Warrior Xero",
            "Ghost Warrior Marmu",
            "Ghost Warrior Galien",

            "Ghost Warrior Slug",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Ghost Warrior Markoth",
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
            "Mawlek Turret",
            "Mawlek Turret Ceiling",
            };

        public static List<string> Flying = new List<string>() {
            "Acid Flyer",
            "Blobble",
            "Flamebearer Small",
            "Flamebearer Med",
            "Flamebearer Large",
            "Mosquito",
            "Bursting Bouncer",
            "Buzzer Col",
            "Giant Fly",
            "Colosseum_Armoured_Mosquito",
            "Colosseum_Flying_Sentry",
            "Angry Buzzer",
            "Mantis Heavy Flyer",
            "Fly",
            "Shade Sibling",
            "Colosseum_Armoured_Mosquito R",
            "Buzzer",
            "Mega Fat Bee",
            "Giant Buzzer Col",
            "Hatcher",
            "Ruins Flying Sentry",
            "Ruins Flying Sentry Javelin",
            "Acid Flyer",
            "Fat Fly",
            "Lazy Flyer Enemy",
            "Moss Flyer",
            "Crystal Flyer",
            "Centipede Hatcher",
            "Blow Fly",
            "Bee Hatchling Ambient",
            "Spider Flyer",
            "Parasite Balloon",
            "Inflater",
            "Fluke Fly",
            "White Palace Fly",
            "Bee Stinger",
            "Big Bee",
            "Zote Balloon",
            "Zote Balloon Ordeal",
            "Zote Salubra",
            "Zote Fluke",
            "Radiance",
            "Mage",
            "Electric Mage",
            "Mage Balloon",
            "Mage Lord",
            "Mage Lord Phase2",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Hornet Nosk",
            "Fungus Flyer",
            "Hatcher Baby",
            "Jellyfish",
            "Lil Jellyfish",
            "Jellyfish Baby",
            "Super Spitter",
            "Spitter R",
            "Spitter",
            "Super Spitter R",
            "Mega Jellyfish",
            "Jellyfish GG",
            "Ghost Warrior Xero",
            "Ghost Warrior Marmu",
            "Ghost Warrior Galien",
            "Ghost Warrior Slug",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Ghost Warrior Markoth",
            "Electro Zap",
            };

        public static List<string> Crawling = new List<string>() {
            "Crawler",
            "Crystal Crawler",
            "Tiny Spider",
            };

        public static List<string> Climbing = new List<string>() {
            "Spider Mini",
            "Climber",
            "Mines Crawler",
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
            "White Palace Fly",
            "Electro Zap",
            "Zote Balloon",
            "Zote Balloon Ordeal",
            "Plant Turret",
            "Plant Turret Right",
            "Mushroom Turret",
            "Laser Turret Frames",
            "Fluke Mother",
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
            "Mantis Traitor Lord",
            "Mega Zombie Beam Miner",
            "Zombie Beam Miner",
            "Zombie Beam Miner Rematch",
            "Mimic Spider",
            "Hornet Boss 2",
            "Infected Knight",
            "Dung Defender",
            "Fluke Mother",
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
            "Mega Jellyfish",
            "Jellyfish GG",
            "Ghost Warrior Xero",
            "Ghost Warrior Marmu",
            "Ghost Warrior Galien",
            "Ghost Warrior Slug",
            "Ghost Warrior No Eyes",
            "Ghost Warrior Hu",
            "Ghost Warrior Markoth",
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
            "Blocker",
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

        public static List<string> PogoLogicEnemies = new List<string>() 
        {
            "Acid Flyer",
            "Acid Walker",
            "White Palace Fly",
        };

        public static List<string> HasUniqueSizeEnemies = new List<string>()
        {
            "Acid Flyer",
        };

        public static List<string> BadPogoReplacement = new List<string>()
        {
            "Mantis Traitor Lord",
            "Oro",
            "Mato",
            "Sheo Boss",
            "Sly Boss",
            "Dream Mage Lord",
            "Dream Mage Lord Phase2",
            "Hive Knight",
            "Grimm Boss",
            "Nightmare Grimm Boss",
            "Fluke Mother",
            "Mawlek Body",
            "Mage Lord",
            "Mage Lord Phase2",
            "Dung Defender",
            "White Defender",
            "False Knight New",
            "False Knight Dream",
            "Ceiling Dropper",
            "Ceiling Dropper Col",
            "Plant Trap",
            "Plant Turret",
            "Plant Turret Right",
            "Mage",
            "Electric Mage",
            "Ruins Flying Sentry Javelin",
            "Ghost Warrior Xero",
            "Ghost Warrior Markoth",
            "Moss Charger",
            "Mega Moss Charger"
        };

        public static List<string> InGroundEnemy = new List<string>()
        {
            "Pigeon",
            "Dung Defender",
            "White Defender",
            "Plant Trap",
            "Moss Charger",
            "Mega Moss Charger"
        };

        public static List<string> RandoControlledPooling = new List<string>()
        {
            "Radiant Nail",
            "Dust Trail",
        };

        public static List<string> ReplacementEnemiesToSkip = new List<string>()
        {
            "Zote Balloon Ordeal",
            "Dream Mage Lord Phase2",
            "Mage Lord Phase2",
            "Mega Jellyfish",
            "Mega Jellyfish GG",
            "Radiance",
            "Corpse Garden Zombie", //don't spawn this, it's just a corpse
        };

        public static List<string> ReplacementHazardsToSkip = new List<string>()
        {
            "Cave Spikes tile", //small bunch of upward pointing cave spikes
            "Cave Spikes tile(Clone)"
        };

        //TODO: temp solution for "bad effects"
        public static List<string> ReplacementEffectsToSkip = new List<string>()
        {
            "tank_fill",
            "tank_full",
            "Bugs Idle",
            "bee_fg_swarm",
            "spider sil left",
            "Butterflies FG",
            "Butterflies BG",
            "wind system",
            "water component",
            "BG_swarm_01",
            "Ruins_Rain",
            "Snore",
            "BG_swarm_02",
            "FG_swarm_02",
            "FG_swarm_01",
            "Particle System FG",
            "Particle System BG",
            "bg_dream",
            "fung_immediate_BG",  //
            "tank_full", //large rect of acid, LOOPING
            "bg_dream",  //big dream circles that float in the background, LOOPING
            "Bugs Idle", //floaty puffs of glowy moth things, LOOPING
            "Shade Particles", //sprays out shade stuff in an wide upward spray, LOOPING
            "Fire Particles", //emits burning fire particles, LOOPING
            "spawn particles b", //has a serialization error? -- may need fixing/don't destroy on load or something
            "Acid Steam", //has a serialization error?
            "Spre Fizzle", //emits a little upward spray of green particles, LOOPING
            "Dust Land", //emits a crescent of steamy puffs that spray upward a bit originating a bit under the given origin, LOOPING
            "Slash Ball", //emits the huge pale lurker slash AoE, LOOPING
            "Bone", //serialzation error?
            "Particle System", //serialization error?
            "Dust Land Small", //??? seems to be making lots of issues
            "Infected Grass A", //??? seems to be making lots of issues
            //"Dust Trail", // --- likely needs mod to control pooling
        };


        public static List<string> BurstEffects = new List<string>()
        {
            "Roar Feathers",
        };



        public static Dictionary<string, string> CustomReplacementName = new Dictionary<string, string>()
        {
            {"Health Cocoon", "HealthScuttler"},
            {"Health Scuttler", "HealthScuttler"},
            {"Orange Scuttler", "OrangeScuttler"},
            {"Abyss Tendril", "AbyssTendril"},
            {"Flamebearer Small", "FlameBearerSmall"},
            {"Flamebearer Med", "FlameBearerMed"},
            {"Flamebearer Large", "FlameBearerLarge"},
            {"Mosquito", "Mosquito"},
            {"Colosseum_Armoured_Roller", "ColRoller"},
            {"Colosseum_Miner", "ColMiner"},
            {"Zote Boss", "ZoteBoss"},
            {"Bursting Bouncer", "BurstingBouncer"},
            {"Super Spitter", "SuperSpitter"},
            {"Super Spitter Col", "SuperSpitterCol"},
            {"Giant Fly Col", "GiantFlyCol"},
            {"Colosseum_Shield_Zombie", "ColShield"},
            {"Buzzer Col", "BuzzerCol"},
            {"Blobble", "Blobble"},
            {"Colosseum_Armoured_Mosquito", "ColMosquito"},
            {"Colosseum_Flying_Sentry", "ColFlyingSentry"},
            {"Hopper", "Hopper"},
            {"Giant Hopper", "GiantHopper"},
            {"Ceiling Dropper Col", "CeilingDropperCol"},
            {"Colosseum_Worm", "ColWorm"},
            {"Spitting Zombie", "SpittingZombie"},
            {"Bursting Zombie", "BurstingZombie"},
            {"Angry Buzzer", "AngryBuzzer"},
            {"Mawlek Col", "MawlekCol"},
            {"Mantis Heavy", "MantisHeavy"},
            {"Lesser Mawlek", "LesserMawlek"},
            {"Mantis Heavy Flyer", "MantisHeavyFlyer"},
            {"Colosseum Grass Hopper", "ColHopper"},
            {"Fly", "Fly"},
            {"Roller", "Roller"},
            {"Hatcher Baby", "HatcherBaby"},
            {"Roller R", "RollerR"},
            {"Spitter R", "SpitterR"},
            {"Buzzer R", "Buzzer"},
            {"Mossman_Runner", "MossmanRunner"},
            {"Jellyfish", "Jellyfish"},
            {"Lil Jellyfish", "LilJellyfish"},
            {"Mantis Flyer Child", "MantisFlyerChild"},
            {"Ghost Warrior Slug", "GhostAladar"},
            {"Corpse Garden Zombie", "CorpseGardenZombie"},
            {"Baby Centipede", "BabyCentipede"},
            {"Zombie Spider 2", "ZombieSpider2"},
            {"Zombie Spider 1", "ZombieSpider1"},
            {"Tiny Spider", "ShootSpider"},
            {"Shade Sibling", "ShadeSibling"},
            {"Flukeman Top", "FlukemanTop"},
            {"Flukeman Bot", "FlukemanBot"},
            {"White Defender", "WhiteDefender"},
           

            {"Jellyfish GG", "JellyfishGG"},
            {"Colosseum_Armoured_Roller R", "ColosseumArmouredRollerR"},
            {"Colosseum_Armoured_Mosquito R", "ColosseumArmouredMosquitoR"},
            {"Super Spitter R", "SuperSpitterR"},
            {"Crawler", "Crawler"},
            {"Buzzer", "Buzzer"},
            {"Giant Buzzer Col", "GiantBuzzerCol"},
            {"Mega Fat Bee", "Oblobble"},
            {"Lobster", "LobsterLancer"},
            {"Mage Knight", "MageKnight"},
            {"Mage", "Mage"},
            {"Electric Mage", "ElectricMage"},
            {"Mage Blob", "MageBlob"},
            {"Lancer", "LobsterLancer"},
            {"Climber", "Climber"},
            {"Zombie Runner", "ZombieRunner"},
            {"Mender Bug", "MenderBug"},
            {"Spitter", "Spitter"},
            {"Zombie Hornhead", "ZombieHornhead"},
            {"Giant Fly", "GiantFly"},
            {"Zombie Barger", "ZombieBarger"},
            {"Mawlek Body", "Mawlek"},
            {"False Knight New", "FalseKnightNew"},
            {"Prayer Slug", "PrayerSlug"},
            {"Blocker", "Blocker"},
            {"Zombie Shield", "ZombieShield"},
            {"Hatcher", "Hatcher"},
            {"Zombie Leaper", "ZombieLeaper"},
            {"Zombie Guard", "ZombieGuard"},
            {"Zombie Myla", "ZombieMyla"},
            {"Egg Sac", "ZapBug"},
            {"Royal Zombie Fat", "RoyalPlumper"},
            {"Royal Zombie", "RoyalDandy"},
            {"Royal Zombie Coward", "RoyalCoward"},
            {"Gorgeous Husk", "GorgeousHusk"},
            {"Ceiling Dropper", "CeilingDropper"},
            {"Ruins Sentry", "Sentry"},
            {"Ruins Flying Sentry", "FlyingSentrySword"},
            {"Ruins Flying Sentry Javelin", "FlyingSentryJavelin"},
            {"Ruins Sentry Fat", "SentryFat"},
            {"Mage Balloon", "MageBalloon"},
            {"Mage Lord", "MageLord"},
            {"Mage Lord Phase2", "MageLordPhase2"},
            {"Great Shield Zombie", "GreatShieldZombie"},
            {"Great Shield Zombie bottom", "GreatShieldZombie"},
            {"Black Knight", "BlackKnight"},
            {"Jar Collector", "JarCollector"},
            {"Moss Walker", "MossWalker"},
            {"Plant Trap", "SnapperTrap"},
            {"Mossman_Shaker", "MossmanShaker"},
            {"Pigeon", "Pigeon"},
            {"Hornet Boss 1", "Hornet"},
            {"Acid Flyer", "AcidFlyer"},
            {"Moss Charger", "MossCharger"},
            {"Acid Walker", "AcidWalker"},
            {"Plant Turret", "PlantShooter"},
            {"Plant Turret Right", "PlantShooter"},
            {"Fat Fly", "FatFly"},
            {"Giant Buzzer", "BigBuzzer"},
            {"Moss Knight", "MossKnight"},
            {"Grass Hopper", "GrassHopper"},
            {"Lazy Flyer Enemy", "LazyFlyerEnemy"},
            {"Mega Moss Charger", "MegaMossCharger"},



            {"Ghost Warrior No Eyes", "GhostNoEyes"},
            {"Fungoon Baby", "FungoonBaby"},
            {"Mushroom Turret", "MushroomTurret"},
            {"Fungus Flyer", "FungusFlyer"},
            {"Zombie Fungus B", "FungifiedZombie"},
            {"Fung Crawler", "FungCrawler"},
            {"Mushroom Brawler", "MushroomBrawler"},
            {"Mushroom Baby", "MushroomBaby"},
            {"Mushroom Roller", "MushroomRoller"},
            {"Zombie Fungus A", "FungifiedZombie"},
            {"Mantis", "Mantis"},
            {"Ghost Warrior Hu", "GhostHu"},
            {"Jellyfish Baby", "JellyfishBaby"},
            {"Moss Flyer", "MossFlyer"},
            {"Garden Zombie", "GardenZombie"},
            {"Mantis Traitor Lord", "MantisTraitorLord"},
            {"Moss Knight Fat", "MossKnightFat"},
            {"Mantis Heavy Spawn", "HeavyMantis"},
            {"Ghost Warrior Marmu", "GhostMarmu"},
            {"Mega Jellyfish", "MegaJellyfish"},
            {"Ghost Warrior Xero", "GhostXero"},
            {"Grave Zombie", "GraveZombie"},
            {"Crystal Crawler", "CrystalCrawler"},
            {"Zombie Miner", "ZombieMiner"},
            {"Crystal Flyer", "CrystalFlyer"},
            {"Crystallised Lazer Bug", "LaserBug"},
            {"Mines Crawler", "MinesCrawler"},
            {"Mega Zombie Beam Miner", "MegaBeamMiner"},
            {"Zombie Beam Miner", "BeamMiner"},
            {"Zombie Beam Miner Rematch", "MegaBeamMiner"},
            {"Spider Mini", "SpiderMini"},
            {"Zombie Hornhead Sp", "SpiderCorpse"},
            {"Zombie Runner Sp", "SpiderCorpse"},
            {"Centipede Hatcher", "CentipedeHatcher"},
            {"Mimic Spider", "MimicSpider"},
            {"Slash Spider", "SlashSpider"},
            {"Spider Flyer", "SpiderFlyer"},
            {"Ghost Warrior Galien", "GhostGalien"},
            {"Blow Fly", "BlowFly"},
            {"Bee Hatchling Ambient", "BeeHatchling"},
            {"Ghost Warrior Markoth", "GhostMarkoth"},
            {"Hornet Boss 2", "DreamGuard"},
            {"Abyss Crawler", "AbyssCrawler"},
            {"Infected Knight", "InfectedKnight"},
            {"Parasite Balloon", "ParasiteBalloon"},
            {"Mawlek Turret", "MawlekTurret"},
            {"Mawlek Turret Ceiling", "MawlekTurret"},
            {"Flip Hopper", "FlipHopper"},
            {"Inflater", "Inflater"},
            {"Fluke Fly", "FlukeFly"},
            {"Flukeman", "Flukeman"},
            {"Dung Defender", "DungDefender"},
            {"fluke_baby_02", "JellyCrawler"},
            {"fluke_baby_01", "BlobFlyer"},
            {"fluke_baby_03", "MenderBug"},
            {"Fluke Mother", "FlukeMother"},
            {"White Palace Fly", "PalaceFly"},
            {"Enemy", "WhiteRoyal"},
            {"Royal Gaurd", "RoyalGaurd"},
            {"Zombie Hive", "ZombieHive"},
            {"Bee Stinger", "BeeStinger"},
            {"Big Bee", "BigBee"},
            {"Hive Knight", "HiveKnight"},
            {"Grimm Boss", "Grimm"},
            {"Nightmare Grimm Boss", "NightmareGrimm"},
            {"False Knight Dream", "FalseKnightDream"},
            {"Dream Mage Lord", "DreamMageLord"},
            {"Dream Mage Lord Phase2", "DreamMageLordPhase2"},
            {"Lost Kin", "LostKin"},
            {"Zoteling", "Zoteling"},
            {"Zoteling Buzzer", "ZotelingBuzzer"},
            {"Zoteling Hopper", "ZotelingHopper"},
            {"Zote Balloon", "ZoteBalloon"},
            {"Grey Prince", "GreyPrince"},
            {"Radiance", "FinalBoss"},
            {"Hollow Knight Boss", "HollowKnight"},
            {"HK Prime", "HollowKnightPrime"},
            {"Pale Lurker", "PaleLurker"},
            {"Oro", "NailBros"},
            {"Mato", "NailBros"},
            {"Sheo Boss", "Paintmaster"},
            {"Fat Fluke", "FatFluke"},
            {"Absolute Radiance", "AbsoluteRadiance"},
            {"Sly Boss", "Nailsage"},
            {"Zote Turret", "ZoteTurret"},
            {"Zote Balloon Ordeal", "ZotelingBalloon"},
            {"Ordeal Zoteling", "OrdealZoteling"},
            {"Zote Salubra", "EggSac"},
            {"Zote Thwomp", "ZoteThwomp"},
            {"Zote Fluke", "Mummy"},
            {"Zote Crew Normal", "ZoteCrewNormal"},
            {"Zote Crew Fat", "ZoteCrewFat"},
            {"Zote Crew Tall", "ZoteCrewTall"},
            {"Hornet Nosk", "HornetNosk"},
            {"Mace Head Bug", "MaceHeadBug"},
            {"Big Centipede Col", "BigCentipede"},
            {"Laser Turret Frames", "LaserBug"},
            {"Jelly Egg Bomb", "JellyEggBomb"},
            {"Worm", "ColWorm"},
            {"Bee Dropper", "Pigeon"}
        };



        //make markoth shield start rotating
        public static Dictionary<string, bool> SafeForArenas = new Dictionary<string, bool>()
        {
            {"Health Cocoon", false},
            {"Health Scuttler", false},
            {"Orange Scuttler", false},
            {"Abyss Tendril", false},
            {"Flamebearer Small", false},
            {"Flamebearer Med", false},
            {"Flamebearer Large", false},
            {"Mosquito", true},
            {"Colosseum_Armoured_Roller", true},
            {"Colosseum_Miner", true},
            {"Zote Boss", true},//? -- inactive?
            {"Bursting Bouncer", true},//corpse didnt explode
            {"Super Spitter", true},
            {"Super Spitter Col", false},//didnt spawn
            {"Giant Fly Col", false},//didn't spawn
            {"Colosseum_Shield_Zombie", true},
            {"Buzzer Col", false},//didn't spawn
            {"Blobble", true},
            {"Colosseum_Armoured_Mosquito", true},
            {"Colosseum_Flying_Sentry", true},
            {"Hopper", true},
            {"Giant Hopper", true},
            {"Ceiling Dropper Col", false},//didn't spawn
            {"Colosseum_Worm", false},//?? didn't spawn
            {"Spitting Zombie", true},
            {"Bursting Zombie", true},
            {"Angry Buzzer", true},
            {"Mawlek Col", false},//?? didn't spawn
            {"Mantis Heavy", true},
            {"Lesser Mawlek", true},
            {"Mantis Heavy Flyer", true},
            {"Colosseum Grass Hopper", false},//??? didn't spawn
            {"Fly", true},
            {"Roller", true},
            {"Hatcher Baby", false},
            {"Roller R", false},
            {"Spitter R", true},
            {"Buzzer R", false},
            {"Mossman_Runner", true},
            {"Jellyfish", true},
            {"Lil Jellyfish", false},//bomb, worked
            {"Mantis Flyer Child", false},//spawned in ground
            {"Ghost Warrior Slug", false},//isn't attacking and drifts left -- movement broken like markoth
            {"Corpse Garden Zombie", false},
            {"Baby Centipede", true},
            {"Zombie Spider 2", false},//nullref
            {"Zombie Spider 1", false},//didn't spawn
            {"Tiny Spider", true},
            {"Shade Sibling", true},
            {"Flukeman Top", false},//dont spawn
            {"Flukeman Bot", false},//don't spawn
            {"White Defender", false},//error on spawn


            {"Jellyfish GG", false},
            {"Colosseum_Armoured_Roller R", false},  //dont spawn
            {"Colosseum_Armoured_Mosquito R", false},//dont spawn
            {"Super Spitter R", false}, //dont spawn
            {"Crawler", true},
            {"Buzzer", true},
            {"Giant Buzzer Col", true},//coun't hurt
            {"Mega Fat Bee", true}, //???? didn't see it spawn
            {"Lobster", true},//was placed inside floor
            {"Mage Knight", true},
            {"Mage", false},//spawned outside arena
            {"Electric Mage", false},//teleported away
            {"Mage Blob", true},
            {"Lancer", false},//error, spawned stuck and couldn't die as well
            {"Climber", true},
            {"Zombie Runner", true},
            {"Mender Bug", false},
            {"Spitter", true},
            {"Zombie Hornhead", true},
            {"Giant Fly", false}, //spawn error, needs fix
            {"Zombie Barger", true},
            {"Mawlek Body", false},//spawn errror
            {"False Knight New", true}, ///???? some errors but killable
            {"Prayer Slug", true},
            {"Blocker", true},
            {"Zombie Shield", true},
            {"Hatcher", true},
            {"Zombie Leaper", true},
            {"Zombie Guard", true},
            {"Zombie Myla", true},
            {"Egg Sac", false},//didnt transfer item
            {"Royal Zombie Fat", true},
            {"Royal Zombie", true},
            {"Royal Zombie Coward", true},
            {"Gorgeous Husk", true},
            {"Ceiling Dropper", true},
            {"Ruins Sentry", true},
            {"Ruins Flying Sentry", true},
            {"Ruins Flying Sentry Javelin", true},
            {"Ruins Sentry Fat", true},
            {"Mage Balloon", true},
            {"Mage Lord", false},//error? fix?
            {"Mage Lord Phase2", true},//works -- cant find his orb idle spot
            {"Great Shield Zombie", true},
            {"Great Shield Zombie bottom", false},//skip, doesn't spawn
            {"Black Knight", false},//??? still yeets into space
            {"Jar Collector", false},//jars are spawning inactive enemies -- spawned in floor
            {"Moss Walker", true},
            {"Plant Trap", true},
            {"Mossman_Shaker", true},
            {"Pigeon", false},
            {"Hornet Boss 1", true},
            {"Acid Flyer", false},
            {"Moss Charger", false},
            {"Acid Walker", false},
            {"Plant Turret", false},
            {"Plant Turret Right", false},
            {"Fat Fly", true},
            {"Giant Buzzer", true},
            {"Moss Knight", true},
            {"Grass Hopper", true},
            {"Lazy Flyer Enemy", false},
            {"Mega Moss Charger", false},



            {"Ghost Warrior No Eyes", false},
            {"Fungoon Baby", true},
            {"Mushroom Turret", false},//spawned in floor
            {"Fungus Flyer", true},
            {"Zombie Fungus B", true},
            {"Fung Crawler", true},
            {"Mushroom Brawler", true},
            {"Mushroom Baby", true},
            {"Mushroom Roller", true},
            {"Zombie Fungus A", true},
            {"Mantis", true},
            {"Ghost Warrior Hu", false},
            {"Jellyfish Baby", false},
            {"Moss Flyer", true},
            {"Garden Zombie", true},
            {"Mantis Traitor Lord", true},
            {"Moss Knight Fat", true},
            {"Mantis Heavy Spawn", true},
            {"Ghost Warrior Marmu", true},
            {"Mega Jellyfish", false},
            {"Ghost Warrior Xero", false},//killing caused nullrefs
            {"Grave Zombie", true},
            {"Crystal Crawler", true},
            {"Zombie Miner", true},
            {"Crystal Flyer", true},
            {"Crystallised Lazer Bug", false},
            {"Mines Crawler", true},
            {"Mega Zombie Beam Miner", true},//nullrefs on spawn
            {"Zombie Beam Miner", true},
            {"Zombie Beam Miner Rematch", true},//nullrefs on spawn
            {"Spider Mini", true},
            {"Zombie Hornhead Sp", true},
            {"Zombie Runner Sp", true},
            {"Centipede Hatcher", true},
            {"Mimic Spider", true},//nullref
            {"Slash Spider", true},
            {"Spider Flyer", true},
            {"Ghost Warrior Galien", false},
            {"Blow Fly", true},
            {"Bee Hatchling Ambient", true},
            {"Ghost Warrior Markoth", false},
            {"Hornet Boss 2", true},
            {"Abyss Crawler", true},
            {"Infected Knight", true},//didn't spawn
            {"Parasite Balloon", true},
            {"Mawlek Turret", true},
            {"Mawlek Turret Ceiling", false},//didn't spawn
            {"Flip Hopper", true},
            {"Inflater", true},
            {"Fluke Fly", true},
            {"Flukeman", true},
            {"Dung Defender", false},
            {"fluke_baby_02", false},
            {"fluke_baby_01", false},//no blue mask
            {"fluke_baby_03", false},//no explode
            {"Fluke Mother", false}, //nullref
            {"White Palace Fly", true},
            {"Enemy", true},
            {"Royal Gaurd", true},//fix boomerang -- elite not spawning
            {"Zombie Hive", true},
            {"Bee Stinger", true},
            {"Big Bee", true},
            {"Hive Knight", false},//not spawning
            {"Grimm Boss", false},//teleports into walls
            {"Nightmare Grimm Boss", false},//stuck on spawn -- still steals HUD -- death doesn't delete
            {"False Knight Dream", true},
            {"Dream Mage Lord", false},
            {"Dream Mage Lord Phase2", false},
            {"Lost Kin", true},
            {"Zoteling", true},
            {"Zoteling Buzzer", true},
            {"Zoteling Hopper", true},
            {"Zote Balloon", false},
            {"Grey Prince", true},
            {"Radiance", false},
            {"Hollow Knight Boss", false},//fell through world
            {"HK Prime", false},//didn't spawn
            {"Pale Lurker", true},
            {"Oro", false},//didn't delete
            {"Mato", false},//didn't activate or die
            {"Sheo Boss", false},//didn't die
            {"Fat Fluke", true},
            {"Absolute Radiance", false},//nullref
            {"Sly Boss", false},//didn't die
            {"Zote Turret", true},
            {"Zote Balloon Ordeal", false},//skip
            {"Ordeal Zoteling", false},
            {"Zote Salubra", false},
            {"Zote Thwomp", true},
            {"Zote Fluke", false},//add poob
            {"Zote Crew Normal", false},//spawns in floor
            {"Zote Crew Fat", true},
            {"Zote Crew Tall", false},//spawns in floor
            {"Hornet Nosk", true},
            {"Mace Head Bug", false},
            {"Big Centipede Col", false},
            {"Laser Turret Frames", false},//nullref
            {"Jelly Egg Bomb", false},//not loaded?
            {"Worm", false}, //need flipped
            {"Bee Dropper", false} //needs some starting velocity, a collider so it can be hit?, and POOB component
        };
    }
}







//protected virtual void CheckIfIsBattleEnemy(GameObject sceneObject)
//{
//    if (EnemyHealthManager == null)
//        return;

//    if (!IsBattleEnemy)
//    {
//        if (sceneObject.GetComponent<BattleManagedObject>())
//            IsBattleEnemy = true;

//        if (!IsBattleEnemy)
//        {
//            IsBattleEnemy = IsBoss;

//            if (!IsBattleEnemy)
//            {
//                IsBattleEnemy = ScenePath.Split('/').Any(x => BattleManager.battleControllers.Any(y => x.Contains(y)));
//            }

//            if (!IsBattleEnemy)
//            {
//                bool hasScene = MetaDataTypes.BattleEnemies.TryGetValue("ANY", out var enemies);
//                if (hasScene)
//                {
//                    IsBattleEnemy = enemies.Contains(DatabaseName);
//                }

//                if (!IsBattleEnemy)
//                {
//                    hasScene = MetaDataTypes.BattleEnemies.TryGetValue(SceneName, out var enemies2);
//                    if (hasScene)
//                    {
//                        IsBattleEnemy = enemies2.Contains(ScenePath);
//                    }
//                }
//            }
//        }
//    }
//}




//protected virtual bool SetupDatabaseRefs(GameObject sceneObject, EnemyRandomizerDatabase database)
//{
//    if (!EnemyRandomizerDatabase.IsDatabaseObject(sceneObject))
//    {
//        HasData = false;
//    }
//    else
//    {
//        DatabaseName = EnemyRandomizerDatabase.ToDatabaseKey(sceneObject.name);
//        if (!string.IsNullOrEmpty(DatabaseName))
//        {
//            HasData = IsObjectInDatabase(database);
//            if (HasData)
//            {
//                SetupObjectType(DatabaseName, database);
//            }
//        }
//    }

//    return HasData;
//}




//protected virtual void SetupRandoProperties(GameObject sceneObject)
//{
//    IsTinker = sceneObject.GetComponentInChildren<TinkEffect>() != null;

//    if (EnemyHealthManager == null)
//    {
//        if (IsTinker)
//            IsInvincible = true;
//        return;
//    }

//    bool isFlyingFromComponents =
//        (sceneObject.GetComponent<Rigidbody2D>() != null && sceneObject.GetComponent<Rigidbody2D>().gravityScale == 0) &&
//        (sceneObject.GetComponent<Climber>() == null);

//    IsBoss = MetaDataTypes.Bosses.Contains(DatabaseName);
//    IsFlying = isFlyingFromComponents || MetaDataTypes.Flying.Contains(DatabaseName);
//    IsCrawling = sceneObject.GetComponent<Crawler>() != null || MetaDataTypes.Crawling.Contains(DatabaseName);
//    IsClimbing = sceneObject.GetComponent<Climber>() != null || MetaDataTypes.Climbing.Contains(DatabaseName);
//    IsMobile = !MetaDataTypes.Static.Contains(DatabaseName);
//    IsSmasher = sceneObject.GetDirectChildren().Any(x => x.name == "Smasher");//can use this child object layer to enable smashing of platforms/walls
//    IsSummonedByEvent = MetaDataTypes.IsSummonedByEvent.Contains(DatabaseName);
//    IsEnemySpawner = MetaDataTypes.SpawnerEnemies.Contains(DatabaseName);
//    CheckIfIsBattleEnemy(sceneObject);
//}



//protected virtual void SetupComponentRefs(GameObject sceneObject)
//{

//    //RandoObject = sceneObject.GetComponent<ManagedObject>();
//    //BattleRandoObject = sceneObject.GetComponent<BattleManagedObject>();

//    EnemyHealthManager = sceneObject.GetComponent<HealthManager>();
//    if (EnemyHealthManager != null)
//    {
//        DefaultHP = EnemyHealthManager.hp;
//        IsInvincible = EnemyHealthManager.IsInvincible;

//        var battleScene = EnemyHealthManager.GetBattleScene();
//        if (battleScene != null)
//        {
//            IsBattleEnemy = true;
//        }

//        GeoManager = new Geo(this);
//    }

//    HeroDamage = sceneObject.GetComponent<DamageHero>();
//    if (HeroDamage != null)
//    {
//        DamageDealt = HeroDamage.damageDealt;
//    }

//    EnemyDamage = sceneObject.GetComponent<DamageEnemies>();
//    if (EnemyDamage != null)
//    {
//        EnemyDamageDealt = EnemyDamage.damageDealt;
//    }

//    Walker = sceneObject.GetComponent<Walker>();
//    IsWalker = Walker != null;

//    PhysicsBody = sceneObject.GetComponent<Rigidbody2D>();

//    //check if it's disabled/completed already
//    var pbi = sceneObject.GetComponent<PersistentBoolItem>();
//    if (pbi != null)
//    {
//        SceneSaveData = pbi;

//        if (pbi.isActiveAndEnabled)
//        {
//            IsDisabledBySavedGameState = pbi.persistentBoolData.activated;
//        }
//        else
//        {
//            var data = global::SceneData.instance.FindMyState(pbi.persistentBoolData);
//            if (data != null)
//                IsDisabledBySavedGameState = data.activated;
//            else
//                IsDisabledBySavedGameState = false;
//        }
//    }

//    Collider = sceneObject.GetComponent<Collider2D>();
//    Sprite = sceneObject.GetComponent<tk2dSprite>();

//    DeathEffects = sceneObject.GetComponent<EnemyDeathEffects>();
//}






//protected virtual void SetupTransformValues(GameObject sceneObject)
//{
//    SizeScale = 1f;

//    if (MetaDataTypes.HasUniqueSizeEnemies.Contains(DatabaseName))
//    {
//        SetSizeFromUniqueObject(sceneObject);
//    }
//    else
//    {
//        SetSizeFromComponents(sceneObject);
//    }

//    UpdateTransformValues();
//}



//public virtual void UpdateTransformValues()
//{
//    if (Source != null)
//    {
//        ObjectPosition = Source.transform.position;
//        ObjectScale = Source.transform.localScale;
//        Rotation = Source.transform.localEulerAngles.z;
//    }
//}


//protected virtual void SetupItemValues(GameObject sceneObject)
//{
//    //check and see if this enemy would have dropped an item
//    HasAvailableItem = sceneObject.GetComponent<PreInstantiateGameObject>();
//    if (!HasAvailableItem)
//    {
//        var ede = sceneObject.GetComponent<EnemyDeathEffects>();
//        if (ede != null)
//        {
//            var corpse = ede.GetCorpseFromDeathEffects();
//            if (corpse != null)
//            {
//                if (corpse.GetComponent<PreInstantiateGameObject>())
//                {
//                    var go = corpse.GetComponent<PreInstantiateGameObject>().InstantiatedGameObject;
//                    if (go != null && go.name.Contains("Shiny Item"))
//                    {
//                        var go_pbi = go.GetComponent<PersistentBoolItem>();
//                        if (go_pbi != null && !go_pbi.persistentBoolData.activated)
//                        {
//                            HasAvailableItem = true;
//                            AvailableItem = go;
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (EnemyHealthManager != null)
//                    Debug.LogError(ScenePath + " has no corpse!");
//            }
//        }
//    }
//}



//protected virtual bool IsVisibleNow()
//{
//    bool isVisible = false;
//    GameObject sceneObject = Source;
//    if (Source == null)
//    {
//        //Dev.Log($"{ScenePath} source has been deleted so this object can never be active!");
//        return false;
//    }

//    if (sceneObject.gameObject.activeSelf)
//    {
//        isVisible = sceneObject.gameObject.activeSelf;
//        //Dev.Log($"{ScenePath} has self active state: {isVisible}");

//        //might not actually be...
//        if (isVisible)
//        {
//            //count "out of bounds" enemies as not visible

//            if (BlackBorders.Value != null && BlackBorders.Value.Count > 0)
//            {
//                var leftRight = BlackBorders.Value.Where(x => x.transform.localScale.x == 20);
//                var topBot = BlackBorders.Value.Where(x => x.transform.localScale.y == 20);

//                var xmin = leftRight.Min(o => (o.transform.position.x - 10f));
//                var xmax = leftRight.Max(o => (o.transform.position.x + 10f));
//                var ymin = topBot.Min(o => (o.transform.position.y - 10f));
//                var ymax = topBot.Max(o => (o.transform.position.y + 10f));

//                //Dev.Log($"{ScenePath} pos:{sceneObject.transform.position}");
//                //Dev.Log($"{ScenePath} BOUNDS[ xmin:{xmin} xmax:{xmax} ymin:{ymin} ymax:{ymax}]");

//                if (sceneObject.transform.position.x < xmin)
//                    isVisible = false;
//                else if (sceneObject.transform.position.x > xmax)
//                    isVisible = false;
//                else if (sceneObject.transform.position.y < ymin)
//                    isVisible = false;
//                else if (sceneObject.transform.position.y > ymax)
//                    isVisible = false;

//                //if(isVisible)
//                //    Dev.Log($"{ScenePath} is in bounds!");
//                //else
//                //    Dev.Log($"{ScenePath} is out of bounds!");
//            }
//            else
//            {
//                //Dev.Log($"{ScenePath} BOUNDS NOT READY YET! Cannot check if visible");
//                //not visible yet
//                return false;
//            }
//        }

//        //still might not actually be...
//        if (isVisible)
//        {
//            Collider2D collider = sceneObject.GetComponent<Collider2D>();
//            MeshRenderer renderer = sceneObject.GetComponent<MeshRenderer>();
//            if (collider != null || renderer != null)
//            {
//                if (collider != null && renderer == null)
//                    isVisible = collider.enabled;
//                else if (collider == null && renderer != null)
//                    isVisible = renderer.enabled;
//                else //if (collider != null && renderer != null)
//                    isVisible = collider.enabled && renderer.enabled;
//            }

//            //if(!isVisible)
//            //    Dev.Log($"{ScenePath} has disabled colliders or renderers and is considered invisible!");
//        }
//    }
//    return isVisible;
//}




//public virtual bool CheckIfIsActiveAndVisible()
//{
//    if (Source == null)
//        return false;

//    bool IsVisible = IsVisibleNow();
//    IsActive = IsVisible && !IsDisabledBySavedGameState;
//    return IsVisible && IsActive;
//}





//public bool IsTemporarilyInactive()
//{
//    return HasData && !IsAReplacementObject && Source.activeInHierarchy && EnemyHealthManager != null && !IsDisabledBySavedGameState && !CheckIfIsActiveAndVisible();
//}





//public bool IsBattleInactive()
//{
//    return HasData && !IsAReplacementObject && IsBattleEnemy && !Source.activeInHierarchy && EnemyHealthManager != null && !IsDisabledBySavedGameState && !CheckIfIsActiveAndVisible();
//}





//public virtual bool CanProcessObject()
//{
//    if (!HasData)
//        return false;

//    if (IsInvalidObject)
//        return false;

//    if (!IsActive)
//    {
//        if (ObjectType != PrefabType.Effect)
//            return false;
//    }

//    if (IsAReplacementObject)
//        return false;

//    if (!CheckIfIsActiveAndVisible())
//    {
//        return false;
//    }

//    return true;
//}




//public virtual void MarkObjectAsReplacement(ObjectMetadata oldObject)
//{
//    ObjectThisReplaced = oldObject;

//    if (RandoObject == null)
//    {
//        if (oldObject.IsBattleEnemy)
//        {
//            BattleRandoObject = Source.AddComponent<BattleManagedObject>();
//            RandoObject = BattleRandoObject;
//        }
//        else
//        {
//            RandoObject = Source.AddComponent<ManagedObject>();
//        }

//        RandoObject.Setup(oldObject);
//    }

//    RandoObject.replaced = true;
//}