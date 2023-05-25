using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Satchel.BetterMenus;


namespace EnemyRandomizerMod
{
    public class RNGModule : BaseRandomizerLogic
    {
        public override string Name => "Randomization Modes";

        public override string Info => "Configure how objects should be randomized";

        public override bool EnableByDefault => true;

        protected RNG onStartGameRNG;

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }


        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Transition", "(Old Chaos Mode) Each time you change rooms everything is will be different", false),
            ("Object", "Each object will change to a different type", false),
            ("Room", "Each room will change one object type", true),
            ("Zone", "Each zone will change one object type", false),
            ("Type", "This will result in one type of object being changed in the same way throughout the game", false),
        };

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => CustomOptions;
        }

        public override List<Element> GetEntries()
        {
            var setting = CustomOptions.FirstOrDefault(x => Settings.GetOption(x.Name).value).Name;


            var elems = new List<Element>();
            var types = new string[] { "Transition", "Object", "Room", "Zone", "Type" };
            var desc = new string[] {
                "(Old Chaos Mode) Each time you change rooms everything is will be different",
                "Each object will change to a different type",
                "Each room will change one object type",
                "Each zone will change one object type",
                "This will result in one type of object being changed in the same way throughout the game" };
            elems.Add(new TextPanel("Choose 1"));
            elems.Add(new HorizontalOption("Randomization Type", "",
                    types, i =>
                    {
                        setting = types[i];
                        (EnemyRandomizer.SubPages[Name].Find("Nice") as HorizontalOption).Description = $"{desc[i]}";
                        EnemyRandomizer.SubPages[Name].Update();

                        CustomOptions.ForEach(x => Settings.GetOption(x.Name).value = false);
                        Settings.GetOption(setting).value = true;
                    }, 
                    () =>
                    {
                        CustomOptions.ForEach(x => Settings.GetOption(x.Name).value = false);
                        Settings.GetOption(setting).value = true;

                        return types.ToList().IndexOf(setting);
                    }, 
                    Id: "Nice"
                    )
                );

            return elems;
        }

        //public override void SetOptionStateFromMenu(string name, bool state)
        //{
        //    if (state == true)
        //    {
        //        var spd = EnemyRandomizer.Instance.Subpages.FirstOrDefault(x => x.title == this.Name);
        //        if (spd != null && spd.subpageMenu.Value != null)
        //        {
        //            CustomOptions.ForEach(x =>
        //            {
        //                if (x.Name != name)
        //                    LogicSettingsMethods.SetSubpageMenuValue(x.Name, false, spd.subpageMenu.Value);
        //                //else
        //                //    LogicSettingsMethods.SetSubpageMenuValue(x.Name, false, spd.subpageMenu.Value);
        //            });
        //        }
        //    }
        //    base.SetOptionStateFromMenu(name, state);
        //}

        public override void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            base.OnStartGame(settings);
            onStartGameRNG = new RNG();
            if (settings.enemyRandomizerSeed >= 0)
                onStartGameRNG.Seed = settings.enemyRandomizerSeed;
            else
                onStartGameRNG.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public override RNG GetRNG(GameObject sourceData, RNG rng, int seed)
        {
            if (Settings.GetOption(CustomOptions[0].Name).value)
            {
                return onStartGameRNG;
            }
            else if (Settings.GetOption(CustomOptions[1].Name).value)
            {
                return RandomizeEachObject(sourceData.ObjectName(), sourceData.SceneName(), seed);
            }
            else if (Settings.GetOption(CustomOptions[2].Name).value)
            {
                return RandomizeEachRoom(sourceData.GetDatabaseKey(), sourceData.SceneName(), seed);
            }
            else if (Settings.GetOption(CustomOptions[3].Name).value)
            {
                return RandomizeEachZone(sourceData.GetDatabaseKey(), seed);
            }
            else if (Settings.GetOption(CustomOptions[4].Name).value)
            {
                return RandomizeEachGame(sourceData.GetDatabaseKey(), seed);
            }
            else
            {
                Dev.LogError("No value in the RNGModule was set! This should never happen!");
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

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        public override bool WillReplaceType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, modify a given type?
        /// </summary>
        public override bool WillModifyType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }

        public override bool WillModifyRNG()
        {
            return true;
        }
    }
}
