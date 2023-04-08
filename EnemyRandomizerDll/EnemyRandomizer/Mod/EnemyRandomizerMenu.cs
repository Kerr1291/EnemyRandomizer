﻿using System.Collections;
using Satchel.BetterMenus;
using System.Collections.Generic;
using Modding;
using System.Linq;

namespace EnemyRandomizerMod
{

    public partial class EnemyRandomizer : ICustomMenuMod
    {
        /// <summary>
        /// Will the toggle button (for an ITogglableMod) be inside the returned menu screen.
        /// If this is set, an `ITogglableMod` will not create the toggle entry in the main
        /// menu.
        /// </summary>
        public bool ToggleButtonInsideMenu => false;

        protected string[] playtesters = new string[]
        {
            "ColetteMSLP",
            "oatmille1",
            "Dwarfwoof",
            "...and you; thanks for playing!"
        };

        public static Menu RootMenuObject;

        public static Menu GeneralOptionsMenu;

        public static Dictionary<string, Menu> SubPages = new Dictionary<string, Menu>();

        public static Menu logicsOptionsMenu;
        public static Menu LogicsOptionsMenu
        {
            get
            {
                return logicsOptionsMenu;
            }
        }

        protected virtual List<Element> GetGeneralEntries()
        {
            var elements = new List<Element> 
        {
            Blueprints.IntInputField(
                name: "Randomizer Seed",
                _storeValue: i => GlobalSettings.seed = i,
                _loadValue: () => GlobalSettings.seed,
                _placeholder: "Click to type a custom seed",
                _characterLimit: 11,
                Id: "SeedInput"
            ),

            // i didnt feel it was necessary to add space but if you want it do
            // new StaticPanel("empty space", _ => { }),

            Blueprints.HorizontalBoolOption(
                name: "Use custom seed for new game",
                description: "Applies to new files or files first loaded without a saved seed",
                applySetting: b =>
                {
                    GlobalSettings.UseCustomSeed = b;
                    GeneralOptionsMenu.Find("SeedInput").isVisible = b;
                    GeneralOptionsMenu.Reflow();
                    UpdateModVersionLabel();
                },
                loadSetting: () => GlobalSettings.UseCustomSeed
            ),
            Blueprints.HorizontalBoolOption(
                name: "Allow Boss Replacement",
                description: "If disabled, bosses will never be touched by the mod",
                applySetting: b => GlobalSettings.RandomizeBosses = b,
                loadSetting: () => GlobalSettings.RandomizeBosses
            ),
            new TextPanel("------------------------"),
            new TextPanel("Credits"),
            new TextPanel("Satchel & Tons of UI help: Mulhima"),
            new TextPanel("Inspiring my return to modding: TheDanielMat"),
            new TextPanel("Enemy Randomizer Author: Kerr1291"),
            new TextPanel("Playtesters: "+string.Join(", ",playtesters)),
            //Blueprints.GenericHorizontalOption<System.FormattableString>(
            //    name: "Credits",
            //    description: "(Click to see more)",
            //    values: new System.FormattableString[] 
            //    { 
            //        $"Satchel & Tons of UI help: Mulhima",
            //        $"Enemy Randomizer Author: Kerr",
            //    },
            //    applySetting: x => { },
            //    loadSetting: () => { return $""; }
            //)
        };
            return elements;
        }

        public static List<Element> GetLogicMenuOptions(Dictionary<string, IRandomizerLogic> logicTypes)
        {
            var elements = new List<Element>();

            foreach (var (dllName, logic) in logicTypes)
            {
                Dev.Log("creating entry for " + dllName);
                elements.Add(Blueprints.HorizontalBoolOption(
                    name: logic.Name,
                    description: logic.Info,
                    applySetting: enabled =>
                    {
                        //try this
                        //if (logic.Enabled && enabled)
                        //{
                        //    return;
                        //}

                        var subMenuButton = RootMenuObject.Find(logic.Name);

                        if (enabled)
                        {
                            subMenuButton.Show();
                            EnemyRandomizer.instance.enemyReplacer.EnableLogic(logic);
                        }
                        else
                        {
                            subMenuButton.Hide();
                            EnemyRandomizer.instance.enemyReplacer.DisableLogic(logic);
                        }
                    },
                    loadSetting: () => isLogicLoaded(logic)));
            }

            return elements;
        }

        public static bool isLogicLoaded(IRandomizerLogic logic) => GlobalSettings.loadedLogics.Contains(logic.Name);


        public static void CreateLogicOptionsMenu(Dictionary<string, IRandomizerLogic> logicTypes)
        {
            if (logicsOptionsMenu == null)
                logicsOptionsMenu = new Menu("Module List", GetLogicMenuOptions(logicTypes).ToArray());
        }

        MenuScreen ICustomMenuMod.GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            EnemyRandomizer.Instance.logicTypes ??= LogicLoader.LoadLogics();

            var previouslyLoadedLogics = EnemyRandomizer.GlobalSettings.loadedLogics;
            var loadedLogics = EnemyRandomizer.Instance.enemyReplacer.ConstructLogics(previouslyLoadedLogics, logicTypes);
            
            if (RootMenuObject == null)
            {
                // create instance of BetterMenus.Menu. It will be used to create the elements
                RootMenuObject = new Menu("Enemy Randomizer Settings");

                // create sub menus
                GeneralOptionsMenu = new Menu("General Settings", GetGeneralEntries().ToArray());
                CreateLogicOptionsMenu(logicTypes);

                // add buttons to go to the sub menus
                RootMenuObject.AddElement(Blueprints.NavigateToMenu(
                    name: "General Settings",
                    description: "Seed settings, credits, and other general mod settings",
                    getScreen: () => GeneralOptionsMenu.GetMenuScreen(RootMenuObject.menuScreen)));

                RootMenuObject.AddElement(Blueprints.NavigateToMenu(
                    name: "Module List",
                    description: "The list of modules that may be enabled",
                    getScreen: () => LogicsOptionsMenu.GetMenuScreen(RootMenuObject.menuScreen)));

                RootMenuObject.AddElement(new TextPanel("------- Enabled Modules --------"));
                RootMenuObject.AddElement(new TextPanel("Select a module to configure it"));
                RootMenuObject.AddElement(new TextPanel(""));

                // create the sub pages for each logic
                logicTypes.Values.ToList().ForEach(logic =>
                {
                    var menu = logic.GetSubpage();
                    SubPages[logic.Name] = menu;

                // add button to go the logic options sub pages
                RootMenuObject.AddElement(
                        Blueprints.NavigateToMenu(
                            logic.Name,
                            logic.Info,
                            () => menu.GetMenuScreen(RootMenuObject.menuScreen)));
                });

                // Because of an oversight on how is visible is set, we we need to do this
                // work around to set default visibility of the buttons
                GeneralOptionsMenu.OnBuilt += (_, _) =>
                    GeneralOptionsMenu.Find("SeedInput").isVisible = GlobalSettings.UseCustomSeed;

                //TODO: call this when its available. if Menu is already built, dont do on built instead just set
                // is visible and call MenuRef.Update
                RootMenuObject.OnBuilt += (_, _) =>
                {
                    logicTypes.Values.ToList().ForEach(logic =>
                        RootMenuObject.Find(logic.Name).isVisible = isLogicLoaded(logic));
                };

                // menu is jank it is what it is
                On.UIManager.ShowMenu -= FixButtonPositions;
                On.UIManager.ShowMenu += FixButtonPositions;
            }
            // return a MenuScreen MAPI can use.
            return RootMenuObject.GetMenuScreen(modListMenu);
        }

        private IEnumerator FixButtonPositions(On.UIManager.orig_ShowMenu orig, UIManager self, MenuScreen menu)
        {
            yield return orig(self, menu);

            if (RootMenuObject?.menuScreen != null && RootMenuObject.menuScreen == menu)
            {
                RootMenuObject.Reflow();
            }
        }
    }

}