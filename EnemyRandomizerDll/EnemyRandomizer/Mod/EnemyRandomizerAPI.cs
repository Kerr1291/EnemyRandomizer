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
}
