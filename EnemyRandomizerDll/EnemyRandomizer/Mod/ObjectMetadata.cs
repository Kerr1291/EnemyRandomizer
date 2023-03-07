using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using nv;
using System.Linq;
using UniRx;
using System;
using System.Reflection;
using static EnemyRandomizerMod.PrefabObject;

//NOTE: adjust aduio player oneshot pitch values and audio source component pitch values when shrinking/growing things
//NOTE: walker enemies need their "rightScale" float changed when the base transform scale is changed so they match or sprites will squish weirdly
//      ALSO need to scan their FSMs for states with "SetScale" actions with x values that are 1/-1 and change those to match the x scale
//      check the "IsNone" property to see if only X is used on the SetScale floats

namespace EnemyRandomizerMod
{
    public class ObjectMetadata
    {
        public bool HasData { get; protected set; }
        public PrefabType ObjectType { get; protected set; }
        public string DatabaseName { get; protected set; }
        public string ImportedSourceName { get; protected set; }
        public string SceneName { get; protected set; }
        public string ObjectName { get; protected set; }
        public string ScenePath { get; protected set; }
        public string ImportedSourcePath { get; protected set; }
        public string MapZone { get; protected set; }
        public bool IsDisabled { get; protected set; }
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
        public bool IsVisible { get; protected set; }
        public bool IsActive { get; protected set; }
        public bool IsSummonedByEvent { get; protected set; }
        public Vector2 ObjectSize { get; protected set; }
        public Vector3 ObjectPosition { get; protected set; }

        //These values will become null after a replacement
        public GameObject Source { get; protected set; }
        public GameObject AvailableItem { get; protected set; }
        public PersistentBoolItem SceneSaveData { get; protected set; }
        public HealthManager EnemyHealthManager { get; protected set; }
        public DamageHero HeroDamage { get; protected set; }
        public DamageEnemies EnemyDamage { get; protected set; }
        public ManagedObject RandoObject { get; protected set; }
        public BattleManagedObject BattleRandoObject { get; protected set; }


        public void Setup(GameObject sceneObject)
        {
            HasData = true;
            Source = sceneObject;
            ObjectName = sceneObject.name;
            DatabaseName = EnemyRandomizerDatabase.ToDatabaseKey(ObjectName);
            SceneName = sceneObject.scene.name;
            ScenePath = sceneObject.GetSceneHierarchyPath();
            MapZone = GameManager.instance.GetCurrentMapZone();
            RandoObject = sceneObject.GetComponent<ManagedObject>();
            BattleRandoObject = sceneObject.GetComponent<BattleManagedObject>();

            EnemyHealthManager = sceneObject.GetComponent<HealthManager>();
            if(EnemyHealthManager != null)
            {
                MaxHP = EnemyHealthManager.hp;
                IsInvincible = EnemyHealthManager.IsInvincible;

                var battleScene = EnemyHealthManager.GetBattleScene();
                if(battleScene != null)
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

            //check if it's disabled/completed already
            var pbi = sceneObject.GetComponent<PersistentBoolItem>();
            if (pbi != null)
            {
                SceneSaveData = pbi;

                if (pbi.isActiveAndEnabled)
                {
                    IsDisabled = pbi.persistentBoolData.activated;
                }
                else
                {
                    var data = global::SceneData.instance.FindMyState(pbi.persistentBoolData);
                    if (data != null)
                        IsDisabled = data.activated;
                    else
                        IsDisabled = false;
                }
            }

            if(sceneObject.GetComponent<BoxCollider2D>())
            {
                ObjectSize = sceneObject.GetComponent<BoxCollider2D>().size;
            }
            else
            {
                if (sceneObject.GetComponent<tk2dSprite>().boxCollider2D != null)
                    ObjectSize = sceneObject.GetComponent<tk2dSprite>().boxCollider2D.size;
                else
                {
                    Debug.LogError("Need a size fallback for " + DatabaseName);
                    ObjectSize = sceneObject.transform.localScale;
                }
            }

            ObjectPosition = sceneObject.transform.position;

            //check and see if this enemy would have dropped an item
            HasAvailableItem = sceneObject.GetComponent<PreInstantiateGameObject>();
            if(!HasAvailableItem)
            {
                var ede = sceneObject.GetComponent<EnemyDeathEffects>();
                if(ede != null)
                {
                    var corpse = ede.GetCorpseFromDeathEffects();
                    if(corpse != null)
                    {
                        if(corpse.GetComponent<PreInstantiateGameObject>())
                        {
                            var go = corpse.GetComponent<PreInstantiateGameObject>().InstantiatedGameObject;
                            if (go != null && go.name.Contains("Shiny Item"))
                            {
                                var go_pbi = go.GetComponent<PersistentBoolItem>();
                                if(go_pbi != null && !go_pbi.persistentBoolData.activated)
                                {
                                    HasAvailableItem = true;
                                    AvailableItem = go;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ObjectType == PrefabType.Enemy)
                            Debug.LogError(ScenePath+" has no corpse!");
                    }
                }
            }

            bool isFlyingFromComponents =
                (sceneObject.GetComponent<Rigidbody2D>() != null && sceneObject.GetComponent<Rigidbody2D>().gravityScale == 0) &&
                (sceneObject.GetComponent<Climber>() == null);

            IsBoss = DefaultMetadata.Bossses.Contains(DatabaseName);
            IsFlying = isFlyingFromComponents || DefaultMetadata.Flying.Contains(DatabaseName);
            IsCrawling = sceneObject.GetComponent<Crawler>() != null || DefaultMetadata.Crawling.Contains(DatabaseName);
            IsClimbing = sceneObject.GetComponent<Climber>() != null || DefaultMetadata.Climbing.Contains(DatabaseName);
            IsTinker = sceneObject.GetComponentInChildren<TinkEffect>() != null;
            IsMobile = !DefaultMetadata.Static.Contains(DatabaseName);
            IsSmasher = sceneObject.GetDirectChildren().Any(x => x.name == "Smasher");//can use this child object layer to enable smashing of platforms/walls

            if (!IsBattleEnemy)
            {
                IsBattleEnemy = IsBoss;
                if(!IsBattleEnemy)
                {
                    IsBattleEnemy = ScenePath.Split('/').Any(x => BattleManager.battleControllers.Any(y => x.Contains(y)));
                }

                if (!IsBattleEnemy)
                {
                    bool hasScene = DefaultMetadata.BattleEnemies.TryGetValue(SceneName, out var enemies);
                    if (hasScene)
                    {
                        IsBattleEnemy = enemies.Contains(ScenePath);
                    }
                }
            }

            if (sceneObject.gameObject.activeSelf)
            {
                IsVisible = sceneObject.gameObject.activeSelf;

                //might not actually be...
                if(IsVisible)
                {
                    //count "out of bounds" enemies as not visible
                    var edges = sceneObject.scene.GetRootGameObjects().Where(x => x.name.Contains("SceneBorder")).Select(x => x.transform);
                    
                    var xmin = edges.Min(o => (o.position.x - 10f));
                    var xmax = edges.Max(o => (o.position.x + 10f));
                    var ymin = edges.Min(o => (o.position.y - 10f));
                    var ymax = edges.Max(o => (o.position.y + 10f));

                    if (sceneObject.transform.position.x < xmin)
                        IsVisible = false;
                    else if (sceneObject.transform.position.x > xmax)
                        IsVisible = false;
                    else if (sceneObject.transform.position.y < ymin)
                        IsVisible = false;
                    else if (sceneObject.transform.position.y > ymax)
                        IsVisible = false;
                }

                //still might not actually be...
                if (IsVisible)
                {
                    Collider2D collider = sceneObject.GetComponent<Collider2D>();
                    MeshRenderer renderer = sceneObject.GetComponent<MeshRenderer>();
                    if (collider != null || renderer != null)
                    {
                        if (collider != null && renderer == null)
                            IsVisible = collider.enabled;
                        else if (collider == null && renderer != null)
                            IsVisible = renderer.enabled;
                        else //if (collider != null && renderer != null)
                            IsVisible = collider.enabled && renderer.enabled;
                    }
                }
            }

            IsActive = IsVisible;


            //TODO: come up with a way to accurately check this
            //IsActive = ???


            //TODO: check the state machine? to see if this is meant to be summoned
            //IsSummonedByEvent = ???
        }


        //use another object's object metadata to configure this metadata
        public void SetupAsReplacement(ObjectMetadata other)
        {
            ImportedSourceName = other.DatabaseName;
            ImportedSourcePath = other.ScenePath;
        }
    }

    public static class DefaultMetadata
    {
        public static List<string> Bossses = new List<string>() {
            "M",
            "M2",
            };

        public static List<string> Flying = new List<string>() {
            "M",
            "M2",
            };

        public static List<string> Crawling = new List<string>() {
            "Crawler",
            "M2",
            };

        public static List<string> Climbing = new List<string>() {
            "M",
            "M2",
            };

        public static List<string> Static = new List<string>() {
            "M",
            "M2",
            };

        public static Dictionary<string, List<string>> BattleEnemies = new Dictionary<string, List<string>>()
        {
            {"SCENE",
                new List<string>()
                { 
                    "PATH1",
                    "PATH2"
                } 
            }
        };
    }
}
