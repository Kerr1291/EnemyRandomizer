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

namespace EnemyRandomizerMod
{
    //NOTE: This code was mostly pulled in from a previous version of the ported enemy rando mod
    //      TODO: make an updated/better menu one day....

    public partial class EnemyRandomizer : IMenuMod
    {
        public string[] toggle = new string[]
        {
            Language.Language.Get("MOH_ON", "MainMenu"),
            Language.Language.Get("MOH_OFF", "MainMenu")
        };

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

        //old menu TODO::: update this
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? menue)
        {
            int Loader(bool value)
            {
                return value ? 0 : 1;
            }

            return new List<IMenuMod.MenuEntry>
            {
                new IMenuMod.MenuEntry
                {
                    Name = "Chaos Mode",
                    Description = "Each enemy will be randomized each time you enter a new room. Expect literally anything.",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverChao(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.RNGChaosMode)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Room Mode",
                    Description = "Each enemy type will be re-randomized each time you enter a new room, but it will still change every enemy of that type.",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverRoom(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.RNGRoomMode)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Randomize Geo",
                    Description = " - Randomizes amount of geo dropped by enemies",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverGeo(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.RandomizeGeo)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Custom Enemies",
                    Description = " - Allows custom enemies to be added to the randomizer",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverCustom(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.CustomEnemies)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Godmaster Enemies",
                    Description = " Allows enemies from the Godmaster expansion to be included in the randomizer. This includes Absolute Radiance, Pure Vessel, Winged Nosk, Mato, Oro, Sheo, Sly and Eternal Ordeal enemies.",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverGod(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.GodmasterEnemies)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Can Spawn Bosses",
                    Description = "Allow bosses to spawn anywhere.",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverBosses(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.AllowBossSpawns)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Can Spawn Hard Enemies",
                    Description = "Allow enemies like kingsmould to be spawned anywhere",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverHard(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.AllowHardSpawns)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Try Matching Replacements",
                    Description = "Try to replace enemies with ones of similar type/difficulty",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverMatch(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.TryMatchingReplacements)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Toggle No Clip",
                    Description = "Turns on no clip - use if you get stuck.",
                    Values = this.toggle,
                    Saver = delegate(int i)
                    {
                        this.SaverNoClip(i);
                    },
                    Loader = () => Loader(EnemyRandomizer.GlobalSettings.NoClip)
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Toggle Debug Log",
                    Description = "Dev helper thing",
                    Values = this.toggle,
                    Saver = (x) => DevLogger.Instance.Show(x == 0),
                    Loader = () => { return DevLogger.Instance.LogWindow.SafeIsActive() ? 0 : 1; }
                },
                new IMenuMod.MenuEntry
                {
                    Name = "Toggle Debug Input",
                    Description = "",
                    Values = this.toggle,
                    Saver = (x) => EnemyRandomizer.SetDebugInput(x == 0),
                    Loader = () => { return EnemyRandomizer.debugInputRoutine.IsRunning ? 0 : 1; }
                }
            };
        }
        




        public void SaverChao(int i)
        {
            EnemyRandomizer.GlobalSettings.RNGChaosMode = i == 0;
        }

        public void SaverRoom(int i)
        {
            EnemyRandomizer.GlobalSettings.RNGRoomMode = i == 0;
        }

        public void SaverGeo(int i)
        {
            EnemyRandomizer.GlobalSettings.RandomizeGeo = i == 0;
        }

        public void SaverGod(int i)
        {
            EnemyRandomizer.GlobalSettings.GodmasterEnemies = i == 0;
        }

        public void SaverBosses(int i)
        {
            EnemyRandomizer.GlobalSettings.AllowBossSpawns = i == 0;
        }

        public void SaverHard(int i)
        {
            EnemyRandomizer.GlobalSettings.AllowHardSpawns = i == 0;
        }

        public void SaverMatch(int i)
        {
            EnemyRandomizer.GlobalSettings.TryMatchingReplacements = i == 0;
        }

        public void SaverCustom(int i)
        {
            EnemyRandomizer.GlobalSettings.CustomEnemies = i == 0;
        }

        public void SaverNoClip(int i)
        {
            EnemyRandomizer.GlobalSettings.NoClip = i == 0;
        }
    }
}
