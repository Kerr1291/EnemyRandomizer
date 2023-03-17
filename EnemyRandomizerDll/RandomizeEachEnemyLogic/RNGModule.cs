using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace EnemyRandomizerMod
{
    public class RNGModule : BaseRandomizerLogic
    {
        public override string Name => "Randomization Modes";

        public override string Info => "Configure how objects should be randomized";

        protected RNG onStartGameRNG;

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }


        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Randomize Each Entry", "(Old Chaos Mode) Each time you change rooms everything is will be different", false),
            ("Randomize Once Per Object", "Each object will change to a different type", false),
            ("Randomize Once Per Room", "Each room will change one object type", true),
            ("Randomize Once Per Zone", "Each zone will change one object type", false),
            ("Randomize Once Per Type", "This will result in one type of object being changed in the same way throughout the game", false),
        };

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override void SetOptionStateFromMenu(string name, bool state)
        {
            if (state == true)
            {
                var spd = EnemyRandomizer.Instance.Subpages.FirstOrDefault(x => x.title == this.Name);
                if (spd != null && spd.subpageMenu.Value != null)
                {
                    CustomOptions.ForEach(x =>
                    {
                        if (x.Name != name)
                            LogicSettingsMethods.SetSubpageMenuValue(x.Name, false, spd.subpageMenu.Value);
                        //else
                        //    LogicSettingsMethods.SetSubpageMenuValue(x.Name, false, spd.subpageMenu.Value);
                    });
                }
            }
            base.SetOptionStateFromMenu(name, state);
        }

        public override void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            base.OnStartGame(settings);
            onStartGameRNG = new RNG();
            if (settings.seed >= 0)
                onStartGameRNG.Seed = settings.seed;
            else
                onStartGameRNG.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public override RNG GetRNG(ObjectMetadata sourceData, RNG rng, int seed)
        {
            if (Settings.GetOption(CustomOptions[0].Name).value)
            {
                return onStartGameRNG;
            }
            else if (Settings.GetOption(CustomOptions[1].Name).value)
            {
                return RandomizeEachObject(sourceData.ObjectName, sourceData.SceneName, seed);
            }
            else if (Settings.GetOption(CustomOptions[2].Name).value)
            {
                return RandomizeEachRoom(sourceData.DatabaseName, sourceData.SceneName, seed);
            }
            else if (Settings.GetOption(CustomOptions[3].Name).value)
            {
                return RandomizeEachZone(sourceData.DatabaseName, seed);
            }
            else if (Settings.GetOption(CustomOptions[4].Name).value)
            {
                return RandomizeEachGame(sourceData.DatabaseName, seed);
            }

            return rng;
        }

        public virtual RNG RandomizeEachObject(string enemyName, string sceneName, int seed)
        {
            RNG rng = new RNG();
            string rawEnemyName = enemyName;
            int stringHashValue = rawEnemyName.GetHashCode();
            int sceneHashValue = sceneName.GetHashCode();

            if (seed >= 0)
            {
                rng.Seed = stringHashValue + seed + sceneHashValue;
            }
            else
            {
                rng.Seed = stringHashValue + sceneHashValue;
            }

            return rng;
        }

        public virtual RNG RandomizeEachRoom(string databaseName, string sceneName, int seed)
        {
            RNG rng = new RNG();

            int stringHashValue = databaseName.GetHashCode();
            int sceneHashValue = sceneName.GetHashCode();

            if (seed >= 0)
            {
                rng.Seed = stringHashValue + seed + sceneHashValue;
            }
            else
            {
                rng.Seed = stringHashValue + sceneHashValue;
            }

            return rng;
        }

        public virtual RNG RandomizeEachZone(string databaseName, int seed)
        {
            RNG rng = new RNG();

            int stringHashValue = databaseName.GetHashCode();
            //int zoneHashValue = EnemyRandomizer.GetCurrentMapZone().GetHashCode();
            int zoneHashValue = GameManager.instance.GetCurrentMapZone().GetHashCode();

            if (seed >= 0)
            {
                rng.Seed = stringHashValue + seed + zoneHashValue;
            }
            else
            {
                rng.Seed = stringHashValue + zoneHashValue;
            }

            return rng;
        }

        public virtual RNG RandomizeEachGame(string databaseName, int seed)
        {
            RNG rng = new RNG();
            int stringHashValue = databaseName.GetHashCode();

            if (seed >= 0)
            {
                rng.Seed = stringHashValue + seed;
            }
            else
            {
                rng.Seed = stringHashValue;
            }

            return rng;
        }

        //public override GameObject ReplaceEnemy(GameObject other)
        //{
        //    if (!Settings.GetOption(DefaultRandomizeEnemiesOption).value)
        //        return base.ReplaceEnemy(other);

        //    SetupRNGForReplacement(other.name, other.scene.name);
        //    var prefab = Database.enemyPrefabs.GetRandomElementFromList(rng);
        //    ObjectMetadata otherData = new ObjectMetadata();
        //    otherData.Setup(other, Database);
        //    GameObject newEnemy = Database.Spawn(prefab, otherData);
        //    newEnemy.SetParentToOthersParent(otherData);
        //    newEnemy.PositionNewEnemy(otherData);
        //    return newEnemy;
        //}

        //List<PrefabObject> allowedEffectReplacements;

        //public override GameObject ReplaceHazardObject(GameObject other)
        //{
        //    if (!Settings.GetOption(DefaultRandomizeHazardsOption).value)
        //        return base.ReplaceHazardObject(other);

        //    if (allowedEffectReplacements == null)
        //        allowedEffectReplacements = Database.hazardPrefabs.Where(x => !EnemyReplacer.ReplacementEffectsToSkip.Contains(x.prefabName)).ToList();

        //    SetupRNGForReplacement(other.name, other.scene.name);
        //    var prefab = allowedEffectReplacements.GetRandomElementFromList(rng);
        //    ObjectMetadata otherData = new ObjectMetadata();
        //    otherData.Setup(other, Database);
        //    GameObject newHazard = Database.Spawn(prefab, otherData);
        //    newHazard.SetParentToOthersParent(otherData);
        //    newHazard.PositionNewEnemy(otherData);
        //    return newHazard;
        //}

        //public override GameObject ReplacePooledObject(GameObject other)
        //{
        //    if (!Settings.GetOption(DefaultRandomizeEffectsOption).value)
        //        return base.ReplacePooledObject(other);

        //    SetupRNGForReplacement(other.name, other.scene.name);
        //    var prefab = Database.effectPrefabs.GetRandomElementFromList(rng);
        //    ObjectMetadata otherData = new ObjectMetadata();
        //    otherData.Setup(other, Database);
        //    GameObject newEffect = Database.Spawn(prefab, otherData);
        //    newEffect.SetParentToOthersParent(otherData);
        //    newEffect.PositionNewEnemy(otherData);
        //    return newEffect;
        //}
    }
}
