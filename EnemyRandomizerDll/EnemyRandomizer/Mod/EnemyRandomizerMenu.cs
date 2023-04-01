using System.Collections;
using System.Collections.Generic;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Linq;
using UnityEngine.UI;
using Modding.Menu;
using System;
using Modding.Menu.Config;
using static Modding.IMenuMod;
using Modding;
using UniRx;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer : ICustomMenuMod
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

            LogicSettingsMethods.SetSubpageMenuEnabled(logic.Name, enabled);

            if (enabled)
            {
                //disabled -- used to allow only one logic to run exlusively
                //the code below will toggle all the other options off
                //MenuScreen myModMenu = ModHooks.BuiltModMenuScreens[this];
                //var logicNames = logicTypes.Select(x => x.Value.Name).Where(x => x != logic.Name);
                //var menuOptions = myModMenu.GetComponentsInChildren<MenuOptionHorizontal>(true).Where(x => logicNames.Contains(x.name));                
                //menuOptions.Where(x => x.selectedOptionIndex == 1).ToList().ForEach(x => x.SetOptionTo(0));
                enemyReplacer.EnableLogic(logic);
            }
            else
            {
                enemyReplacer.DisableLogic(logic);
            }
        }

        private static UIManager _uim => UIManager.instance;
        UnityEngine.UI.InputField customSeedInput = null;
        public Selectable customSeedInputOption;
        GameObject menuTogglePrefab;
        GameObject useCustomSeedOption;
        string useCustomSeedOptionName = "Use custom seed for new game";
        void SetupMenuTogglePrefab()
        {
            if (menuTogglePrefab != null)
                return;

            //find it
            menuTogglePrefab = GameObjectExtensions.FindGameObjectInChildrenWithName(_uim.gameObject, "VSyncOption");

            //remove default menu behaviors
            GameObject.DestroyImmediate(menuTogglePrefab.GetComponent<MenuOptionHorizontal>());
            GameObject.DestroyImmediate(menuTogglePrefab.GetComponent<MenuSetting>());
        }

        void OnCustomSeed(string seed)
        {
            if (seed.Length <= 0)
                return;

            //EnemyRandomizer.Instance.GlobalSettings.IntValues[ "CustomSeed" ] = Int32.Parse( seed );
            if (Int32.TryParse(seed, out var s))
            {
                GlobalSettings.seed = s;

                //customSeedInput.text = seed;
            }
        }

        void SaveCustomSeed()
        {
            if (customSeedInput != null)
            {
                string seed = customSeedInput.text;
                if (Int32.TryParse(seed, out var s))
                {
                    GlobalSettings.seed = s;
                }
            }
        }

        void LoadCustomSeed()
        {
            //if (customSeedInput == null)
            //    SetupSeedEntry();

            if (customSeedInput != null)
            {
                customSeedInput.text = GlobalSettings.seed.ToString();
            }
        }

        void SetupSeedEntry()
        {
            if (customSeedInput != null)
                return;

            //find a toggle-able menu element to use for our mod option prefab
            SetupMenuTogglePrefab();
            //logicListSubpage.content
            //logicListSubpage

            GameObject menuItemParent = UnityEngine.Object.Instantiate(menuTogglePrefab);

            string optionName = "CustomSeed";

            customSeedInput = menuItemParent.AddComponent<UnityEngine.UI.InputField>();
            customSeedInput.textComponent = menuItemParent.transform.GetChild(1).GetComponent<Text>();

            //TODO: fix me in the morning :(
            Text t = UnityEngine.Object.Instantiate(customSeedInput.textComponent) as Text;
            t.transform.SetParent(customSeedInput.transform);
            customSeedInput.placeholder = t;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.text = "Click to type a custom seed";
            t.transform.Translate(new Vector3(500f, 0f, 0f));
            t.alignment = TextAnchor.LowerCenter;

            customSeedInput.caretColor = Color.white;
            customSeedInput.contentType = InputField.ContentType.IntegerNumber;
            //customSeedInput.onValueChanged.AddListener( OnCustomSeed );
            customSeedInput.onEndEdit.AddListener(OnCustomSeed);
            customSeedInput.navigation = Navigation.defaultNavigation;
            customSeedInput.caretWidth = 8;
            customSeedInput.characterLimit = 11;

            ColorBlock cb = new ColorBlock();
            cb.highlightedColor = Color.yellow;
            cb.pressedColor = Color.red;
            cb.disabledColor = Color.black;
            cb.normalColor = Color.white;
            cb.colorMultiplier = 2f;

            customSeedInput.colors = cb;


            customSeedInputOption = customSeedInput;


            string optionLabel = "Randomizer Seed";

            UnityEngine.Object.Destroy(menuItemParent.transform.FindChild("Label").GetComponent<AutoLocalizeTextUI>());
            menuItemParent.transform.FindChild("Label").GetComponent<Text>().text = optionLabel;

            menuItemParent.name = optionName;

            RectTransform rt = menuItemParent.GetComponent<RectTransform>();

            rt.SetParent(logicListSubpage.content.transform);
            rt.localScale = new Vector3(2, 2, 2);

            int i = 0;

            rt.sizeDelta = new Vector2(960, 120);
            rt.anchoredPosition = new Vector2(0, (766 / 2) - 90 - (150 * i));
            rt.anchorMin = new Vector2(0.5f, 1.0f);
            rt.anchorMax = new Vector2(0.5f, 1.0f);

            useCustomSeedOption = _uim.gameObject.FindGameObjectInChildrenWithName(useCustomSeedOptionName);


            var placeholder = _uim.gameObject.FindGameObjectInChildrenWithName("THE_PLACEHOLDER");
            placeholder.SetActive(false);
            customSeedInput.transform.SetParent(placeholder.transform.parent);
            customSeedInput.transform.localScale = placeholder.transform.localScale;
            customSeedInput.transform.localPosition = placeholder.transform.localPosition;
            customSeedInput.transform.localPosition = customSeedInput.transform.localPosition - new Vector3(0f, 40f, 0f);
            //customSeedInput.transform.localScale = new Vector3(customSeedInput.transform.localScale.x,
            //    customSeedInput.transform.localScale.y * .75f,
            //    customSeedInput.transform.localScale.z);

            var empty = _uim.gameObject.FindGameObjectInChildrenWithName("EMPTY_SPACE");
            empty.SetActive(false);
            //var back = logicListSubpage.defaultHighlight.FindSelectableOnUp();
            //var back2 = back.FindSelectableOnUp();
            //var next = logicListSubpage.defaultHighlight.FindSelectableOnDown();
            //var next2 = next.FindSelectableOnDown();

            //Dev.Log($"placeholder? k:{k++}");

            //Dev.Log($"custom seed highlight k:{k++}");
            //logicListSubpage.defaultHighlight = customSeedInputOption;
            //Navigation nav2 = back.navigation;
            //nav2.selectOnUp = back2;
            //nav2.selectOnDown = logicListSubpage.defaultHighlight;
            //back.navigation = nav2;

            LoadCustomSeed();

            customSeedInputOption.gameObject.SetActive(false);
        }

        public void SetCustomSeedInputVisible(bool enabled)
        {
            if (customSeedInput != null)
                customSeedInput.gameObject.SetActive(enabled);
        }

        public void SetCustomSeedVisible(bool enabled)
        {
            if(useCustomSeedOption != null)
                useCustomSeedOption.gameObject.SetActive(enabled);
            SetCustomSeedInputVisible(enabled);
        }

        //public void SetDisabled(IRandomizerLogic logic)
        //{
        //    if (GlobalSettings.currentLogic.Contains(logic.Name))
        //        GlobalSettings.currentLogic.Remove(logic.Name);

        //    if (!logic.Enabled)
        //        return;

        //    if (enemyReplacer.IsInGameScene())
        //        logic.Disable();
        //}

        //private MenuScreen CreateModMenu(ModToggleDelegates? toggleDelegates)
        //{
        //    var mod = this as ICustomMenuMod;
        //    IMenuMod.MenuEntry? toggleEntry = null;
        //    if (toggleDelegates is ModToggleDelegates)
        //    {
        //        toggleEntry = new IMenuMod.MenuEntry
        //          {
        //              Name = this.Name,
        //              Values = toggle,
        //              Saver = v => toggleDelegates.Value.SetModEnabled(v == 1),
        //              Loader = () => toggleDelegates.Value.GetModEnabled() ? 1 : 0,
        //          };
        //    }


        //    var name = this.Name;
        //    var entries = GetDefaultMenuData(toggleEntry);
        //    return MenuUtils.CreateMenuScreen(name, entries, this.screen);
        //}

        public List<IMenuMod.MenuEntry> GetDefaultMenuData()
        {
            var menu = new List<IMenuMod.MenuEntry>();

            menu = GetDebugEntries(menu);
            menu = GetLogics(menu);

            return menu;
        }

        public List<IMenuMod.MenuEntry> GetLogics(List<IMenuMod.MenuEntry> menu)
        {
            Dev.Log("loading menu mod logics");
            LoadLogics();

            if (GlobalSettings.loadedLogics == null)
                GlobalSettings.loadedLogics = new List<string>();

            foreach (var logic in logicTypes)
            {
                Dev.Log("creating entry for "+logic.Key);
                IMenuMod.MenuEntry entry = new IMenuMod.MenuEntry()
                {
                    Name = logic.Value.Name,
                    Description = logic.Value.Info,
                    Values = this.toggle,
                    Saver = (x) => { SetLogicEnabledFromMenu(logic.Value, x == 1); },
                    Loader = () => { return GlobalSettings.loadedLogics.Contains(logic.Value.Name) ? 1 : 0; },
                };

                menu.Add(entry);
            }
            return menu;
        }

        public int SetUseCustomSeed(bool useCustomSeed)
        {
            EnemyRandomizer.GlobalSettings.UseCustomSeed = useCustomSeed;
            SetCustomSeedInputVisible(EnemyRandomizer.GlobalSettings.UseCustomSeed);
            UpdateModVersionLabel();
            return EnemyRandomizer.GlobalSettings.UseCustomSeed ? 1 : 0;
        }

        public int SetUseCustomSeed()
        {
            SetCustomSeedInputVisible(EnemyRandomizer.GlobalSettings.UseCustomSeed);
            return EnemyRandomizer.GlobalSettings.UseCustomSeed ? 1 : 0;
        }

        public List<IMenuMod.MenuEntry> GetDebugEntries(List<IMenuMod.MenuEntry> menu)
        {
            menu.Add(new IMenuMod.MenuEntry
            {
                Name = "THE_PLACEHOLDER",
                Description = "The seed",
                Values = new string[] { "-1" },
                Saver = (x) => SaveCustomSeed(),
                Loader = () => { LoadCustomSeed(); return 0; },
            });
            menu.Add(new IMenuMod.MenuEntry
            {
                Name = "EMPTY_SPACE",
                Description = "EMPTY",
                Values = new string[] { "-1" },
                Saver = (x) => { },
                Loader = () => { return 0; },
            });
            menu.Add(new IMenuMod.MenuEntry
            {
                Name = useCustomSeedOptionName,
                Description = "Applies to new files or files first loaded without a saved seed",
                Values = this.toggle,
                Saver = (x) => { SetUseCustomSeed( x == 1 ); },
                Loader = () => { return SetUseCustomSeed(); }
            });
            menu.Add(new IMenuMod.MenuEntry
            {
                Name = "Allow Boss Replacement",
                Description = "If disabled, bosses will never be touched by the mod",
                Values = this.toggle,
                Saver = (x) => { EnemyRandomizer.GlobalSettings.RandomizeBosses = x == 1; },
                Loader = () => { return EnemyRandomizer.GlobalSettings.RandomizeBosses ? 1 : 0; }
            });

            return menu;
        }

        public class SubpageDef
        {
            public string title;
            public string description;
            public List<IMenuMod.MenuEntry> entries;
            public IRandomizerLogic owner;
            public ReactiveProperty<MenuButton> activationButton;
            public ReactiveProperty<MenuScreen> subpageMenu;

            public SubpageDef()
            {
                title = string.Empty;
                description = string.Empty;
                entries = new List<MenuEntry>();
                owner = null;
                activationButton = new ReactiveProperty<MenuButton>();
                subpageMenu = new ReactiveProperty<MenuScreen>();
            }
        }

        public MenuScreen logicListSubpage;
        public List<SubpageDef> Subpages = new List<SubpageDef>();

        MenuScreen ICustomMenuMod.GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            //LogicSettingsMethods.disabledRoot = new GameObject("__Disabled");
            //GameObject.DontDestroyOnLoad(LogicSettingsMethods.disabledRoot);
            //LogicSettingsMethods.disabledRoot.SetActive(false);
            LoadLogics();
            return GetConfigMenuScreen("Enemy Randomizer Submodule Settings", modListMenu, toggleDelegates);
        }

        public MenuScreen GetConfigMenuScreen(string configScreenName, MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            ModMenuScreenBuilder builder = new ModMenuScreenBuilder(configScreenName, modListMenu);

            logicListSubpage = builder.AddSubpage("Enemy Randomizer Modules", "Select which modules to enable", GetDefaultMenuData());
            
            foreach (SubpageDef def in Subpages)
            {
                def.subpageMenu.Value = builder.AddSubpage(def.title, def.description, def.entries);
            }

            var createdMenu = builder.CreateMenuScreen();

            //createdMenu.gameObject.PrintSceneHierarchyTree(true, null);

            foreach (var button in createdMenu.GetComponentsInChildren<MenuButton>())
            {
                var found = Subpages.FirstOrDefault(x => x.title == button.name);

                if (found == null)
                {
                    if (button.name == "Enemy Randomizer Modules")
                    {
                        var le = button.gameObject.AddComponent<LayoutElement>();
                        var label = button.GetComponentsInChildren<Text>(true).FirstOrDefault(x => x.name == "Label");
                        var desc = button.GetComponentsInChildren<Text>(true).FirstOrDefault(x => x.name == "Description");

                        label.alignment = TextAnchor.MiddleCenter;
                        desc.alignment = TextAnchor.UpperCenter;

                        float scale = 1f;// desc.preferredWidth > 1000f ? desc.preferredWidth / 1000f : 1f;
                        float labelHeight = label.preferredHeight;
                        float descHeight = desc.preferredHeight * scale;

                        float finalSize = descHeight + labelHeight + 12f;
                        float labelPos = 0.5f * finalSize - labelHeight * .5f;
                        float descPos = -0.5f * finalSize + descHeight * .5f - 12f;

                        {
                            label.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                            label.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
                            label.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                            label.GetComponent<RectTransform>().pivot = new Vector2(0.5f, .5f);
                            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, labelPos);
                            //label.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }
                        {
                            desc.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                            desc.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
                            desc.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                            desc.GetComponent<RectTransform>().pivot = new Vector2(0.5f, .5f);
                            desc.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, descPos);
                            //label.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }
                        //label.rectTransform.transform.localPosition += new Vector3(0f, labelPos, 0f);
                        //desc.rectTransform.transform.localPosition += new Vector3(0f, descPos, 0f);

                        le.preferredHeight = finalSize;
                        le.preferredWidth = button.GetComponentsInChildren<Text>(true).Max(x => x.preferredWidth);
                        le.minHeight = le.preferredHeight;
                        le.minWidth = 1200f;

                        ////float descHeight = button.GetComponentsInChildren<Text>(true).FirstOrDefault(x => x.name == "Description").preferredHeight;
                        //le.preferredHeight = descHeight + button.GetComponentsInChildren<Text>(true).Max(x => x.preferredHeight);
                        //le.preferredWidth = button.GetComponentsInChildren<Text>(true).Max(x => x.preferredWidth);
                        //le.minHeight = 42.75f;
                        //le.minWidth = 1200f;
                        le.layoutPriority = 10;
                    }
                }
                
                if(found != null)
                {
                    found.activationButton.Value = button;
                    var label = button.GetComponentsInChildren<Text>(true).FirstOrDefault(x => x.name == "Label");
                    var desc = button.GetComponentsInChildren<Text>(true).FirstOrDefault(x => x.name == "Description");
                    var le = button.gameObject.AddComponent<LayoutElement>();

                    label.alignment = TextAnchor.MiddleCenter;
                    desc.alignment = TextAnchor.UpperCenter;

                    float scale = 1f;// desc.preferredWidth > 1000f ? desc.preferredWidth / 1000f : 1f;
                    float labelHeight = label.preferredHeight;
                    float descHeight = desc.preferredHeight * scale;

                    float finalSize = descHeight + labelHeight + 12f;
                    float labelPos = 0.5f * finalSize - labelHeight * .5f;
                    float descPos = -0.5f * finalSize + descHeight * .5f - 12f;


                    //{
                    //    var csf = label.gameObject.AddComponent<ContentSizeFitter>();
                    //    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    //    csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    //}

                    //{
                    //    var csf = desc.gameObject.AddComponent<ContentSizeFitter>();
                    //    csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    //    csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    //}

                    {
                        label.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                        label.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
                        label.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                        label.GetComponent<RectTransform>().pivot = new Vector2(0.5f, .5f);
                        label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, labelPos);
                        //label.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    }
                    {
                        desc.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                        desc.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
                        desc.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                        desc.GetComponent<RectTransform>().pivot = new Vector2(0.5f, .5f);
                        desc.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, descPos);
                        //label.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    }
                    //label.rectTransform.transform.localPosition += new Vector3(0f, labelPos, 0f);
                    //desc.rectTransform.transform.localPosition += new Vector3(0f, descPos, 0f);

                    le.preferredHeight = finalSize;
                    le.preferredWidth = button.GetComponentsInChildren<Text>(true).Max(x => x.preferredWidth);
                    le.minHeight = le.preferredHeight;
                    le.minWidth = 1200f;
                }
            }

            var scrollingPane = createdMenu.gameObject.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(x => x.name == "ScrollingPane").gameObject;
            var vlg = scrollingPane.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.spacing = 2f;

            SetupSeedEntry();

            return createdMenu;
        }


        public class ModMenuScreenBuilder
        {
            private readonly MenuScreen returnScreen;
            private readonly Dictionary<string, MenuScreen> MenuScreens = new Dictionary<string, MenuScreen>();
            public readonly MenuBuilder menuBuilder;
            public readonly MenuButton backButton;

            // Defer creating the menu screen until we know whether we will need a scroll pane
            public List<Action<ContentArea>> buildActions = new List<Action<ContentArea>>();
            private void ApplyBuildActions(ContentArea c)
            {
                foreach (Action<ContentArea> action in buildActions)
                {
                    action(c);
                }
            }

            public ModMenuScreenBuilder(string title, MenuScreen returnScreen)
            {
                this.returnScreen = returnScreen;
                this.menuBuilder = CreateMenuBuilder(title, returnScreen, out this.backButton);
            }

            public MenuScreen CreateMenuScreen()
            {
                if (buildActions.Count > 5)
                {
                    menuBuilder.AddContent(new NullContentLayout(), c => c.AddScrollPaneContent(
                        new ScrollbarConfig
                        {
                            CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                            Navigation = new Navigation
                            {
                                mode = Navigation.Mode.Explicit,
                                selectOnUp = backButton,
                                selectOnDown = backButton
                            },
                            Position = new AnchoredPosition
                            {
                                ChildAnchor = new Vector2(0f, 1f),
                                ParentAnchor = new Vector2(1f, 1f),
                                Offset = new Vector2(-310f, 0f)
                            }
                        },
                        new RelLength(buildActions.Count * 105f),
                        RegularGridLayout.CreateVerticalLayout(105f),
                        ApplyBuildActions
                    ));
                }
                else
                {
                    menuBuilder.AddContent(
                        RegularGridLayout.CreateVerticalLayout(105f),
                        ApplyBuildActions
                    );
                }

                return this.menuBuilder.Build();
            }

            /// <summary>
            /// Adds a button which proceeds to a subpage consisting of a list of MenuOptionHorizontals.
            /// </summary>
            public MenuScreen AddSubpage(string title, string description, IReadOnlyList<IMenuMod.MenuEntry> entries)
            {
                MenuScreen screen = CreateMenuScreen(title, this.menuBuilder.Screen, entries);
                AddSubpage(title, description, screen);
                return screen;
            }

            /// <summary>
            /// Adds a button which proceeds to a subpage.
            /// </summary>
            public void AddSubpage(string title, string description, MenuScreen screen)
            {
                MenuScreens.Add(title, screen);
                MenuButtonConfig config = new MenuButtonConfig()
                {
                    Label = title,
                    Description = new DescriptionInfo()
                    {
                        Text = description
                    },
                    Proceed = true,
                    SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(MenuScreens[title]),
                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(this.menuBuilder.Screen),
                };

                this.buildActions.Add(c => c.AddMenuButton(title, config));
            }

            /// <summary>
            /// Adds a horizontal option.
            /// </summary>
            /// <param name="entry">The struct containing the data for the menu entry.</param>
            public void AddHorizontalOption(IMenuMod.MenuEntry entry)
            {
                DescriptionInfo? desc = null;
                if (string.IsNullOrEmpty(entry.Description))
                {
                    desc = new DescriptionInfo()
                    {
                        Text = entry.Description
                    };
                }

                HorizontalOptionConfig config = new HorizontalOptionConfig()
                {
                    ApplySetting = (_, i) => entry.Saver(i),
                    RefreshSetting = (s, _) => s.optionList.SetOptionTo(entry.Loader()),
                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(this.returnScreen),
                    Description = desc,
                    Label = entry.Name,
                    Options = entry.Values,
                    Style = HorizontalOptionStyle.VanillaStyle
                };

                this.buildActions.Add(c =>
                {
                    c.AddHorizontalOption(entry.Name, config, out MenuOptionHorizontal option);
                    option.menuSetting.RefreshValueFromGameSettings();
                });
            }

            /// <summary>
            /// Adds a clickable button which executes a custom action on click.
            /// </summary>
            public void AddButton(string title, string description, Action onClick)
            {
                MenuButtonConfig config = new MenuButtonConfig()
                {
                    Label = title,
                    Description = new DescriptionInfo()
                    {
                        Text = description
                    },
                    Proceed = false,
                    SubmitAction = _ => onClick(),
                    CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(this.returnScreen),
                };

                this.buildActions.Add(c => c.AddMenuButton(title, config));
            }

            public static MenuBuilder CreateMenuBuilder(string title, MenuScreen returnScreen, out MenuButton backButton)
            {
                MenuBuilder builder = new MenuBuilder(title);
                builder.CreateTitle(title, MenuTitleStyle.vanillaStyle);
                builder.CreateContentPane(RectTransformData.FromSizeAndPos(
                        new RelVector2(new Vector2(1920f, 903f)),
                        new AnchoredPosition(
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0.5f, 0.5f),
                            new Vector2(0f, -60f)
                        )
                    ));
                builder.CreateControlPane(RectTransformData.FromSizeAndPos(
                    new RelVector2(new Vector2(1920f, 259f)),
                    new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -502f)
                    )
                ));
                builder.SetDefaultNavGraph(new ChainedNavGraph());

                MenuButton _back = null;
                builder.AddControls(
                    new SingleContentLayout(new AnchoredPosition(
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f),
                        new Vector2(0f, -64f)
                    )),
                    c => c.AddMenuButton(
                        "BackButton",
                        new MenuButtonConfig
                        {
                            Label = Language.Language.Get("OPT_MENU_BACK_BUTTON", "UI"),
                            CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                            SubmitAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                            Style = MenuButtonStyle.VanillaStyle,
                            Proceed = true
                        },
                        out _back
                    ));

                backButton = _back;
                return builder;
            }

            public static MenuScreen CreateMenuScreen(string title, MenuScreen returnScreen, IReadOnlyList<IMenuMod.MenuEntry> entries)
            {
                MenuBuilder builder = CreateMenuBuilder(title, returnScreen, out MenuButton backButton);

                if (entries.Count > 5)
                {
                    builder.AddContent(new NullContentLayout(), c => c.AddScrollPaneContent(
                        new ScrollbarConfig
                        {
                            CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                            Navigation = new Navigation
                            {
                                mode = Navigation.Mode.Explicit,
                                selectOnUp = backButton,
                                selectOnDown = backButton
                            },
                            Position = new AnchoredPosition
                            {
                                ChildAnchor = new Vector2(0f, 1f),
                                ParentAnchor = new Vector2(1f, 1f),
                                Offset = new Vector2(-310f, 0f)
                            }
                        },
                        new RelLength(entries.Count * 105f),
                        RegularGridLayout.CreateVerticalLayout(105f),
                        d => AddMenuEntriesToContentArea(d, entries, returnScreen)
                    ));
                }
                else
                {
                    builder.AddContent(
                        RegularGridLayout.CreateVerticalLayout(105f),
                        c => AddMenuEntriesToContentArea(c, entries, returnScreen)
                    );
                }

                return builder.Build();
            }

            public static void AddMenuEntriesToContentArea(ContentArea c, IReadOnlyList<IMenuMod.MenuEntry> entries, MenuScreen returnScreen)
            {
                foreach (IMenuMod.MenuEntry entry in entries)
                {
                    DescriptionInfo? desc = null;
                    if (!string.IsNullOrEmpty(entry.Description))
                    {
                        desc = new DescriptionInfo()
                        {
                            Text = entry.Description
                        };
                    }

                    HorizontalOptionStyle styleToUse = HorizontalOptionStyle.VanillaStyle;
                    

                    HorizontalOptionConfig config = new HorizontalOptionConfig()
                    {
                        ApplySetting = (_, i) => entry.Saver(i),
                        RefreshSetting = (s, _) => s.optionList.SetOptionTo(entry.Loader()),
                        CancelAction = _ => UIManager.instance.UIGoToDynamicMenu(returnScreen),
                        Description = desc,
                        Label = entry.Name,
                        Options = entry.Values,
                        Style = styleToUse
                    };

                    c.AddHorizontalOption(entry.Name, config, out MenuOptionHorizontal option);
                    option.menuSetting.RefreshValueFromGameSettings();
                }
            }
        }
    } 
}
