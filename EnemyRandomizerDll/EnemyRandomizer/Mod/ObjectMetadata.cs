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
        public bool IsEnemySpawner { get; protected set; }
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
            IsSummonedByEvent = DefaultMetadata.IsSummonedByEvent.Contains(DatabaseName);
            IsEnemySpawner = DefaultMetadata.SpawnerEnemies.Contains(DatabaseName);

            if (!IsBattleEnemy)
            {
                IsBattleEnemy = IsBoss;
                if(!IsBattleEnemy)
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

                    if (!hasScene)
                    {
                        hasScene = DefaultMetadata.BattleEnemies.TryGetValue(SceneName, out var enemies2);
                        if (hasScene)
                        {
                            IsBattleEnemy = enemies2.Contains(ScenePath);
                        }
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
