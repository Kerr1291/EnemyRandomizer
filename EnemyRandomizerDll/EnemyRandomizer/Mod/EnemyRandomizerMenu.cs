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
using UnityEngine.UI;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : IMenuMod
    {
        //
        // Summary:
        //     Will the toggle button (for an ITogglableMod) be inside the returned menu screen.
        //     If this is set, an `ITogglableMod` will not create the toggle entry in the main
        //     menu.
        public bool ToggleButtonInsideMenu
        {
            get
            {
                return false;
            }
        }

        public string[] toggle = new string[]
        {
            Language.Language.Get("MOH_OFF", "MainMenu"),
            Language.Language.Get("MOH_ON", "MainMenu")
        };

        public void SetLogicEnabledFromMenu(IRandomizerLogic logic, bool enabled)
        {
            if(logic.Enabled && enabled)
            {
                return;
            }

            if (enabled)
            {
                MenuScreen myModMenu = ModHooks.BuiltModMenuScreens[this];
                var logicNames = logicTypes.Select(x => x.Value.Name).Where(x => x != logic.Name);
                var menuOptions = myModMenu.GetComponentsInChildren<MenuOptionHorizontal>(true).Where(x => logicNames.Contains(x.name));
                
                menuOptions.Where(x => x.selectedOptionIndex == 1).ToList().ForEach(x => x.SetOptionTo(0));

                enemyReplacer.SetLogic(logic);
            }
            else
            {
                SetDisabled(logic);
            }
        }

        public void SetDisabled(IRandomizerLogic logic)
        {
            if (EnemyRandomizer.GlobalSettings.currentLogic == logic.Name)
                EnemyRandomizer.GlobalSettings.currentLogic = null;

            if (!logic.Enabled)
                return;

            if (enemyReplacer.IsInGameScene())
                logic.Disable();
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? menue)
        {

            var menu = new List<IMenuMod.MenuEntry>();

            menu = GetDebugEntries(menu);
            menu = GetLogics(menu);

            return menu;
        }

        public List<IMenuMod.MenuEntry> GetLogics(List<IMenuMod.MenuEntry> menu)
        {
            foreach (var logic in logicTypes)
            {
                IMenuMod.MenuEntry entry = new IMenuMod.MenuEntry()
                {
                    Name = logic.Value.Name,
                    Description = logic.Value.Info,
                    Values = this.toggle,
                    Saver = (x) => { SetLogicEnabledFromMenu(logic.Value, x == 1); },
                    Loader = () => { return logic.Value.Name == EnemyRandomizer.GlobalSettings.currentLogic ? 1 : 0; },
                };

                menu.Add(entry);
            }
            return menu;
        }

        public List<IMenuMod.MenuEntry> GetDebugEntries(List<IMenuMod.MenuEntry> menu)
        {
            menu.Add(new IMenuMod.MenuEntry
            {
                Name = "Toggle No Clip",
                Description = "Turns on no clip - use if you get stuck.",
                Values = this.toggle,
                Saver = (x) => { EnemyRandomizer.GlobalSettings.IsNoClip = x == 1; },
                Loader = () => { return EnemyRandomizer.GlobalSettings.IsNoClip ? 1 : 0; }
            });
            return menu;
        }
    }
}
