using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;
using System.Linq;
using UniRx;
using System;
using System.Reflection;
using UnityEngine.UI;
using Satchel.BetterMenus;

namespace EnemyRandomizerMod
{
    public static class EnemyRandomizerAPI
    {
        /// <summary>
        /// Provides a way for other mods to load their modules
        /// </summary>
        public static void LoadExternalLogics(string rootDirectoryWithLogics)
        {
            var externalLogics = LogicLoader.LoadLogics(rootDirectoryWithLogics);

            LoadExternalLogics(externalLogics.Select(x => x.Value));
        }

        /// <summary>
        /// Provides a way for other mods to load their modules
        /// </summary>
        public static void LoadExternalLogics(IEnumerable<IRandomizerLogic> logicsToLoad)
        {
            Dev.Where();
            var externalLogics = logicsToLoad.ToDictionary(x => x.Name);

            var previouslyLoadedLogics = EnemyRandomizer.GlobalSettings.loadedLogics;
 
            //load the logics and return the ones that were enabled
            var loadedLogics = EnemyRandomizer.Instance.enemyReplacer.ConstructLogics(previouslyLoadedLogics, externalLogics);
                        
            //create the logic menu options
            var menuOptions = EnemyRandomizer.GetLogicMenuOptions(externalLogics);
            menuOptions.ForEach(x => EnemyRandomizer.LogicsOptionsMenu.AddElement(x));

            // create the sub pages for each logic
            externalLogics.Values.ToList().ForEach(logic =>
            {
                var menu = logic.GetSubpage();
                EnemyRandomizer.SubPages[logic.Name] = menu;

                // add button to go the logic options sub pages
                EnemyRandomizer.RootMenuObject.AddElement(
                        Blueprints.NavigateToMenu(
                            logic.Name,
                            logic.Info,
                            () => menu.GetMenuScreen(EnemyRandomizer.RootMenuObject.menuScreen)));
            });

            loadedLogics.ForEach(x => EnemyRandomizer.RootMenuObject.Find(x.Name).isVisible = true);
        }
    }

    /// <summary>
    /// General/global mod settings
    /// </summary>
    public class EnemyRandomizerSettings
    {
        public int seed = -1;
        public List<string> loadedLogics = new List<string>();
        public bool UseCustomSeed = false;
        public List<LogicSettings> logicSettings = new List<LogicSettings>();
    }

    /// <summary>
    /// Save file specific settings
    /// </summary>
    public class EnemyRandomizerPlayerSettings
    {
        public int enemyRandomizerSeed = -1;
    }

    /// <summary>
    /// Settings container used by modules to contain the options used by a logic module
    /// </summary>
    public class LogicSettings
    {
        public string name;
        public List<LogicOption> options = new List<LogicOption>();
    }

    /// <summary>
    /// Option container used by modules to the value of thier toggle settings
    /// </summary>
    public class LogicOption
    {
        public string name;
        public bool value;
    }

    /// <summary>
    /// Methods to interact with the various logic options
    /// </summary>
    public static class LogicSettingsMethods
    {
        /// <summary>
        /// Checks if the logic settings container exists and if not then it creates one.
        /// Will always return a valid settings container even if an invalid name is passed.
        /// </summary>
        public static LogicSettings GetLogicSettings(this EnemyRandomizerSettings self, string logicName)
        {
            if (!self.logicSettings.Any(x => x.name == logicName))
            {
                self.logicSettings.Add(new LogicSettings() { name = logicName });
            }
            return self.logicSettings.FirstOrDefault(x => x.name == logicName);
        }

        /// <summary>
        /// Checks if the option reference container exists and if not then it creates one.
        /// Will always return an option container even if an invalid name is passed.
        /// </summary>
        public static LogicOption GetOption(this LogicSettings settings, string optionName, bool? defaultValue = null)
        {
            if (!settings.options.Any(x => x.name == optionName))
            {
                var newOption = new LogicOption() { name = optionName };
                if (defaultValue != null)
                {
                    newOption.value = defaultValue.Value;
                }
                settings.options.Add(newOption);
            }

            return settings.options.FirstOrDefault(x => x.name == optionName);
        }

        /// <summary>
        /// Checks it the option exists
        /// </summary>
        public static bool HasOption(this LogicSettings settings, string optionName)
        {
            return settings.options.Any(x => x.name == optionName);
        }

        /// <summary>
        /// Returns null if the option name is invalid or uninitialized
        /// </summary>
        public static bool? GetOptionValue(this LogicSettings settings, string optionName)
        {
            if (settings.HasOption(optionName))
            {
                return settings.GetOption(optionName).value;
            }

            return null;
        }
    }
}
