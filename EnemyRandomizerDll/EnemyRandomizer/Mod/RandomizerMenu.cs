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
using UniRx;
using UniRx.Triggers;
using UniRx.Operators;
using UnityEngine.Events;

namespace EnemyRandomizerMod
{
    public class LogicMenu : MenuElement, IContainer
    {
        public static ReactiveCollection<LogicMenu> loadedMenus;
        public static ReactiveCollection<LogicMenu> LoadedMenus
        {
            get
            {
                if (loadedMenus == null)
                    loadedMenus = new ReactiveCollection<LogicMenu>();
                return loadedMenus;
            }
        }

        private int Columns = 1;
        private int Index = 0;
        private LogicMenu Instance;
        private RelVector2 ItemAdvance = new RelVector2(new Vector2(0.0f, -105f));
        private AnchoredPosition Start = new AnchoredPosition
        {
            ChildAnchor = new Vector2(0.5f, 1f),
            ParentAnchor = new Vector2(0.5f, 1f),
            Offset = default
        };
        public List<GameObjectRow> MenuOrder = new List<GameObjectRow>();

        public ReactiveProperty<IRandomizerLogic> LogicModel { get; protected set; }
        public ReactiveProperty<MenuScreen> LogicMenuScreen { get; protected set; }
        public ReactiveProperty<MenuScreen> LogicPreviousScreen { get; protected set; }
        public ReactiveProperty<MenuButton> LogicEnableButton { get; protected set; }
        public ReactiveCollection<MenuEntry> LogicOptions { get; protected set; }

        public ReactiveProperty<string> LogicMenuTitle { get; protected set; }
        public ReactiveProperty<string> LogicMenuInfo { get; protected set; }

        public ReactiveCollection<Element> Elements { get; protected set; }

        public ReactiveDictionary<string, Element> ElementDict { get; protected set; }

        //allows CancelAction to be set by outsider
        public UnityEvent CancelAction;

        public LogicMenu()
            :base()
        {
            SetupObservables();
        }

        public LogicMenu(string name, Element[] elements) 
            : this(name)
        {
            SetupObservables();
            foreach (var elem in elements)
            {
                AddElement(elem);
            }
        }

        public LogicMenu(string name)
            :base()
        {
            SetupObservables();
            Name.Value = name;
            Instance = this;
            MenuOrder.Clear();
            ResetPositioners();
            On.UIManager.ShowMenu += ShowMenu;
        }

        private IEnumerator ShowMenu(On.UIManager.orig_ShowMenu orig, UIManager self, MenuScreen menu)
        {
            if (menu == this.LogicMenuScreen.Value)
            {
                menu.screenCanvasGroup.alpha = 0f;
                menu.screenCanvasGroup.gameObject.SetActive(true);
                DoUpdate.Invoke();
                menu.screenCanvasGroup.gameObject.SetActive(false);
            }
            yield return orig(self, menu);
        }

        protected virtual void SetupObservables()
        {
            if (LogicMenuTitle != null)
                return;
            LogicMenuTitle = new ReactiveProperty<string>();
            LogicMenuInfo = new ReactiveProperty<string>();
            LogicOptions = new ReactiveCollection<MenuEntry>();
            LogicModel = new ReactiveProperty<IRandomizerLogic>();
            LogicEnableButton = new ReactiveProperty<MenuButton>();
            LogicPreviousScreen = new ReactiveProperty<MenuScreen>();
            LogicMenuScreen = new ReactiveProperty<MenuScreen>();
            Elements = new ReactiveCollection<Element>();
            ElementDict = new ReactiveDictionary<string, Element>();

            Elements.ObserveAdd().Subscribe(x => ElementDict[x.Value.Id.Value] = x.Value);
        }

        public static LogicMenu Create(string title, string info)
        {
            LogicMenu newMenu = new LogicMenu();
            newMenu.LogicMenuTitle.Value = title;
            newMenu.LogicMenuInfo.Value = info;
            LoadedMenus.Add(newMenu);
            return newMenu;
        }

        public void AddElement(Element elem)
        {
            Elements.Add(elem);
        }

        protected virtual void SetupReturnScreen(MenuScreen returnScreen)
        {
            LogicPreviousScreen.Value = returnScreen;
            CancelAction.AsObservable().Subscribe(_ => EnemyRandomizer.GoToMenuScreen(LogicPreviousScreen.Value)).AddTo(disposables);
        }

        protected virtual void ApplyElementVisibility(Element elem)
        {


            if (elem.gameObject == null)
            {
                if (elem is IShadowElement)
                {
                    var elems = ((IShadowElement)elem).GetElements();
                    foreach (var e in elems)
                    {
                        ApplyElementVisibility(e);
                    }
                }
                else
                {
                    Satchel.Instance.LogError($"No GameObject for {elem.GetType()} {elem.Name}");
                }
            }
            else
            {
                elem.gameObject.SetActive(elem.isVisible);
            }
        }

        protected virtual void AddModMenuContent(ReactiveCollection<Element> AllMenuOptions, ContentArea c)
        {
            //go through the list given to us by user
            foreach (var menuOption in AllMenuOptions)
            {
                menuOption.Parent.Value = this;
                menuOption.Create(c, Instance);
                ApplyElementVisibility(menuOption);
            }
        }

        /// <summary>
        /// Creates a new MenuScreen from the Menu to be used by Modding API to create mod menu.
        /// </summary>
        /// <param name="_returnScreen">the "previous" menu screen. It is the screen the game will return to to on back button press or esc</param>
        /// <returns>The MenuScreen returned is what needs to be given to the Modding API to have a modmenu</returns>
        public MenuScreen GetMenuScreen(MenuScreen returnScreen)
        {
            SetupReturnScreen(returnScreen);

            MenuBuilder Menu = EnemyRandomizer.CreateMenuBuilder(LogicMenuTitle.Value); //create main screen
            UnityEngine.UI.MenuButton backButton = null; //just so we can use it in scroll bar
            Menu.AddContent(new NullContentLayout(), c => c.AddScrollPaneContent(
                new ScrollbarConfig
                {
                    CancelAction = _ => CancelAction.Invoke(),
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
                new RelLength(LogicOptions.Count * 105f),
                RegularGridLayout.CreateVerticalLayout(105f),
                d => AddModMenuContent(Elements, d)
            ));

            Menu.AddBackButton(Instance, out backButton); // add a back button
            LogicMenuScreen.Value = Menu.Build();
            TriggerBuiltEvent();
            return menuScreen;
        }

        private void ResetPositioners()
        {
            ItemAdvance = new RelVector2(new Vector2(0.0f, -105f));
            Start = new AnchoredPosition
            {
                ChildAnchor = new Vector2(0.5f, 1f),
                ParentAnchor = new Vector2(0.5f, 1f),
                Offset = default
            };

            Index = 0;
        }
    }


    public partial class EnemyRandomizer : ICustomMenuMod
    {
        GameObject customSeedInputObjectPrefab;
        InputField customSeedInput;
        Selectable customSeedInputOption;
        GameObject useCustomSeedOption;
        MenuScreen logicModuleMenu;

        string coreLogicMenuName = "Enemy Randomizer Submodule Settings";
        string useCustomSeedOptionName = "Use custom seed for new game";

        public bool ToggleButtonInsideMenu
        {
            get
            {
                return false;
            }
        }

        public string[] MenuOnOffToggles = new string[]
        {
            "OFF", "ON"
        };

        public void SetLogicEnabledFromMenu(IRandomizerLogic logic, bool enabled)
        {
            if (logic.Enabled && enabled)
            {
                return;
            }

            LogicSettingsMethods.SetSubpageMenuEnabled(logic.Name, enabled);

            if (enabled)
            {
                enemyReplacer.EnableLogic(logic);
            }
            else
            {
                enemyReplacer.DisableLogic(logic);
            }
        }

        public void SetCustomSeedInputVisible(bool enabled)
        {
            if (customSeedInput != null)
                customSeedInput.gameObject.SetActive(enabled);
        }

        public void SetCustomSeedVisible(bool enabled)
        {
            if (useCustomSeedOption != null)
                useCustomSeedOption.gameObject.SetActive(enabled);
            SetCustomSeedInputVisible(enabled);
        }


        public List<IMenuMod.MenuEntry> GetDefaultMenuData()
        {
            var menu = new List<IMenuMod.MenuEntry>();

            menu = GetDebugEntries(menu);
            menu = GetLogics(menu);

            return menu;
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
                Values = this.MenuOnOffToggles,
                Saver = (x) => { SetUseCustomSeed(x == 1); },
                Loader = () => { return SetUseCustomSeed(); }
            });
            menu.Add(new IMenuMod.MenuEntry
            {
                Name = "Allow Boss Replacement",
                Description = "If disabled, bosses will never be touched by the mod",
                Values = this.MenuOnOffToggles,
                Saver = (x) => { EnemyRandomizer.GlobalSettings.RandomizeBosses = x == 1; },
                Loader = () => { return EnemyRandomizer.GlobalSettings.RandomizeBosses ? 1 : 0; }
            });

            return menu;
        }

        public List<IMenuMod.MenuEntry> GetLogics(List<IMenuMod.MenuEntry> menu)
        {
            LoadLogics();

            if (GlobalSettings.loadedLogics == null)
                GlobalSettings.loadedLogics = new List<string>();

            foreach (var logic in logicTypes)
            {
                IMenuMod.MenuEntry entry = new IMenuMod.MenuEntry()
                {
                    Name = logic.Value.Name,
                    Description = logic.Value.Info,
                    Values = this.MenuOnOffToggles,
                    Saver = (x) => { SetLogicEnabledFromMenu(logic.Value, x == 1); },
                    Loader = () => { return GlobalSettings.loadedLogics.Contains(logic.Value.Name) ? 1 : 0; },
                };

                menu.Add(entry);
            }
            return menu;
        }

        void OnCustomSeed(string seed)
        {
            if (seed.Length <= 0)
                return;

            if (Int32.TryParse(seed, out var s))
            {
                GlobalSettings.seed = s;
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
            if (customSeedInput != null)
            {
                customSeedInput.text = GlobalSettings.seed.ToString();
            }
        }

        void SetupCustomSeedInputField()
        {
            if (customSeedInput != null)
                return;

            if (customSeedInputObjectPrefab != null)
                return;

            //find it
            customSeedInputObjectPrefab = GameObjectExtensions.FindGameObjectInChildrenWithName(UIManager.instance.gameObject, "VSyncOption");

            //remove default menu behaviors
            GameObject.DestroyImmediate(customSeedInputObjectPrefab.GetComponent<MenuOptionHorizontal>());
            GameObject.DestroyImmediate(customSeedInputObjectPrefab.GetComponent<MenuSetting>());

            GameObject menuItemParent = UnityEngine.Object.Instantiate(customSeedInputObjectPrefab);

            string optionName = "CustomSeed";

            customSeedInput = menuItemParent.AddComponent<UnityEngine.UI.InputField>();
            customSeedInput.textComponent = menuItemParent.transform.GetChild(1).GetComponent<Text>();

            Text t = UnityEngine.Object.Instantiate(customSeedInput.textComponent) as Text;
            t.transform.SetParent(customSeedInput.transform);
            customSeedInput.placeholder = t;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.text = "Click to type a custom seed";
            t.transform.Translate(new Vector3(500f, 0f, 0f));
            t.alignment = TextAnchor.LowerCenter;

            customSeedInput.caretColor = Color.white;
            customSeedInput.contentType = InputField.ContentType.IntegerNumber;
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

            rt.SetParent(logicModuleMenu.content.transform);
            rt.localScale = new Vector3(2, 2, 2);

            int i = 0;

            rt.sizeDelta = new Vector2(960, 120);
            rt.anchoredPosition = new Vector2(0, (766 / 2) - 90 - (150 * i));
            rt.anchorMin = new Vector2(0.5f, 1.0f);
            rt.anchorMax = new Vector2(0.5f, 1.0f);

            useCustomSeedOption = UIManager.instance.gameObject.FindGameObjectInChildrenWithName(useCustomSeedOptionName);

            var placeholder = UIManager.instance.gameObject.FindGameObjectInChildrenWithName("THE_PLACEHOLDER");
            placeholder.SetActive(false);
            customSeedInput.transform.SetParent(placeholder.transform.parent);
            customSeedInput.transform.localScale = placeholder.transform.localScale;
            customSeedInput.transform.localPosition = placeholder.transform.localPosition;
            customSeedInput.transform.localPosition = customSeedInput.transform.localPosition - new Vector3(0f, 40f, 0f);

            var empty = UIManager.instance.gameObject.FindGameObjectInChildrenWithName("EMPTY_SPACE");
            empty.SetActive(false);

            LoadCustomSeed();

            customSeedInputOption.gameObject.SetActive(false);
        }

        int SetUseCustomSeed(bool useCustomSeed)
        {
            EnemyRandomizer.GlobalSettings.UseCustomSeed = useCustomSeed;
            SetCustomSeedInputVisible(EnemyRandomizer.GlobalSettings.UseCustomSeed);
            UpdateModVersionLabel();
            return EnemyRandomizer.GlobalSettings.UseCustomSeed ? 1 : 0;
        }

        int SetUseCustomSeed()
        {
            SetCustomSeedInputVisible(EnemyRandomizer.GlobalSettings.UseCustomSeed);
            return EnemyRandomizer.GlobalSettings.UseCustomSeed ? 1 : 0;
        }

        MenuScreen ICustomMenuMod.GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            LoadLogics();

            var coreLogicMenu = BuildCoreLogicMenu(modListMenu, toggleDelegates);


            return coreLogicMenu;
        }

        public MenuScreen BuildCoreLogicMenu(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            var coreLogicMenuBuilder = CreateMenuBuilder(coreLogicMenuName);

        }
    }

    public interface IShadowElement
    {
        IEnumerable<Element> ContainedElements { get; }

        Element Find(string Id);
    }
}
