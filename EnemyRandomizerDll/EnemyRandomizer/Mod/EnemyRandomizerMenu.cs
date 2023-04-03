using System.Collections;
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

        public string[] playtesters = new string[]
        {
            "ColetteMSLP",
            "oatmille1",
            "Dwarfwoof",
            "...and you; thanks for playing!"
        };

        private List<Element> GetGeneralEntries()
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

        private List<Element> GetLogics()
        {
            GlobalSettings.loadedLogics ??= new List<string>();
            var elements = new List<Element>();

            foreach (var (dllName, logic) in logicTypes)
            {
                Dev.Log("creating entry for " + dllName);
                elements.Add(Blueprints.HorizontalBoolOption(
                    name: logic.Name,
                    description: logic.Info,
                    applySetting: enabled =>
                    {
                        if (logic.Enabled && enabled)
                        {
                            return;
                        }

                        var subMenuButton = MenuRef.Find(logic.Name);

                        if (enabled)
                        {
                            subMenuButton.Show();
                            enemyReplacer.EnableLogic(logic);
                        }
                        else
                        {
                            subMenuButton.Hide();
                            enemyReplacer.DisableLogic(logic);
                        }
                    },
                    loadSetting: () => isLogicLoaded(logic)));
            }

            return elements;
        }

        public bool isLogicLoaded(IRandomizerLogic logic) => GlobalSettings.loadedLogics.Contains(logic.Name);

        private static Menu MenuRef;
        private static Menu GeneralOptionsMenu;
        private static Menu LogicsOptionsMenu;
        public static Dictionary<string, Menu> SubPages = new();

        MenuScreen ICustomMenuMod.GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            LoadLogics();

            if (MenuRef == null)
            {
                // create instance of BetterMenus.Menu. It will be used to create the elements
                MenuRef = new Menu("Enemy Randomizer Settings");

                // create sub menus
                GeneralOptionsMenu = new Menu("Mod Settings", GetGeneralEntries().ToArray());
                LogicsOptionsMenu = new Menu("Logic Settings", GetLogics().ToArray());

                // add buttons to go to the sub menus
                MenuRef.AddElement(Blueprints.NavigateToMenu(
                    name: "Mod Settings",
                    description: "Main Settings for mod",
                    getScreen: () => GeneralOptionsMenu.GetMenuScreen(MenuRef.menuScreen)));

                MenuRef.AddElement(Blueprints.NavigateToMenu(
                    name: "Module Settings",
                    description: "Settings for modules",
                    getScreen: () => LogicsOptionsMenu.GetMenuScreen(MenuRef.menuScreen)));

                // create the sub pages for each logic
                logicTypes.Values.ToList().ForEach(logic =>
                {
                    var menu = logic.GetSubpage();
                    SubPages[logic.Name] = menu;

                // add button to go the logic options sub pages
                MenuRef.AddElement(
                        Blueprints.NavigateToMenu(
                            logic.Name,
                            logic.Info,
                            () => menu.GetMenuScreen(MenuRef.menuScreen)));
                });

                // Because of an oversight on how is visible is set, we we need to do this
                // work around to set default visibility of the buttons
                GeneralOptionsMenu.OnBuilt += (_, _) =>
                    GeneralOptionsMenu.Find("SeedInput").isVisible = GlobalSettings.UseCustomSeed;

                //TODO: call this when its available. if Menu is already built, dont do on built instead just set
                // is visible and call MenuRef.Update
                MenuRef.OnBuilt += (_, _) =>
                {
                    logicTypes.Values.ToList().ForEach(logic =>
                        MenuRef.Find(logic.Name).isVisible = isLogicLoaded(logic));
                };

                // menu is jank it is what it is
                On.UIManager.ShowMenu -= FixButtonPositions;
                On.UIManager.ShowMenu += FixButtonPositions;
            }
            // return a MenuScreen MAPI can use.
            return MenuRef.GetMenuScreen(modListMenu);
        }

        private IEnumerator FixButtonPositions(On.UIManager.orig_ShowMenu orig, UIManager self, MenuScreen menu)
        {
            yield return orig(self, menu);

            if (MenuRef?.menuScreen != null && MenuRef.menuScreen == menu)
            {
                MenuRef.Reflow();
            }
        }
    }

}