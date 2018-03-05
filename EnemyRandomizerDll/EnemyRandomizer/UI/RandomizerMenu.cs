using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Modding;
using EnemyRandomizerMod;

namespace EnemyRandomizerMod.Menu
{
    public class RandomizerMenu
    {
        private static UIManager _uim;
        private static RandomizerFauxUIManager _fauxUim;
        public static MenuScreen ModMenuScreen;

        MenuButton modButton;
        GameObject menuTogglePrefab;
        GameObject uiManagerCanvasRoot = null;

        public static Selectable[] ModOptions;
        public static Selectable Back;

        public RandomizerMenu()
        {
            EnemyRandomizer.Instance.Log( "Initializing RandomizerMenu" );

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
            GameObject go = new GameObject();
            _fauxUim = go.AddComponent<RandomizerFauxUIManager>();

            EnemyRandomizer.Instance.Log( "Initialized RandomizerMenu" );
        }

        public void DataDump(GameObject go, int depth)
        {
            Log(new string('-', depth) + go.name);
            foreach (Component comp in go.GetComponents<Component>())
            {
                switch (comp.GetType().ToString())
                {
                    case "UnityEngine.RectTransform":
                        Log( new string('+', depth) + comp.GetType() + " : " + ((RectTransform)comp).sizeDelta + ", " + ((RectTransform)comp).anchoredPosition + ", " + ((RectTransform)comp).anchorMin + ", " + ((RectTransform)comp).anchorMax);
                        break;
                    case "UnityEngine.UI.Text":
                        Log( new string('+', depth) + comp.GetType() + " : " + ((Text)comp).text);
                        break;
                    default:
                        Log( new string('+', depth) + comp.GetType());
                        break;
                }
            }
            foreach (Transform child in go.transform)
            {
                DataDump(child.gameObject, depth + 1);
            }
        }


        public static Sprite NullSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadRawTextureData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
        }

        public static Sprite CreateSprite(byte[] data, int x, int y, int w, int h)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(data);
            tex.anisoLevel = 0;
            return Sprite.Create(tex, new Rect(x, y, w, h), Vector2.zero);
        }

        //TODO: add params for button text and position
        void AddModMenuButtonToOptionMenu()
        {
            //ADD MODS TO OPTIONS MENU
            MenuButton defButton = (MenuButton)_uim.optionsMenuScreen.defaultHighlight;
            modButton = Object.Instantiate(defButton.gameObject).GetComponent<MenuButton>();

            Navigation nav = modButton.navigation;

            nav.selectOnUp = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();
            nav.selectOnDown = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();

            modButton.navigation = nav;

            nav = modButton.FindSelectableOnUp().navigation;
            nav.selectOnDown = modButton;
            modButton.FindSelectableOnUp().navigation = nav;

            nav = modButton.FindSelectableOnDown().navigation;
            nav.selectOnUp = modButton;
            modButton.FindSelectableOnDown().navigation = nav;

            modButton.name = "EnemyRandomizer";

            modButton.transform.SetParent( modButton.FindSelectableOnUp().transform.parent );

            modButton.transform.localPosition = new Vector2( 0, -240 );
            modButton.transform.localScale = modButton.FindSelectableOnUp().transform.localScale;

            Object.Destroy( modButton.gameObject.GetComponent<AutoLocalizeTextUI>() );
            modButton.gameObject.transform.FindChild( "Text" ).GetComponent<Text>().text = "Enemy Randomizer";
            //ADD MODS TO OPTIONS MENU
        }

        void InstantiateModMenuScreenGameObject()
        {
            if( ModMenuScreen != null )
                return;

            GameObject go = Object.Instantiate(_uim.optionsMenuScreen.gameObject);
            ModMenuScreen = go.GetComponent<MenuScreen>();
            ModMenuScreen.title = ModMenuScreen.gameObject.transform.FindChild( "Title" ).GetComponent<CanvasGroup>();
            ModMenuScreen.topFleur = ModMenuScreen.gameObject.transform.FindChild( "TopFleur" ).GetComponent<Animator>();
            ModMenuScreen.content = ModMenuScreen.gameObject.transform.FindChild( "Content" ).GetComponent<CanvasGroup>();
        }

        void SetMenuTitle(string title)
        {
            if( ModMenuScreen == null )
                return;

            if( ModMenuScreen.title == null )
                ModMenuScreen.title = ModMenuScreen.gameObject.transform.FindChild( "Title" ).GetComponent<CanvasGroup>();

            ModMenuScreen.title.gameObject.GetComponent<Text>().text = title;

            //remove the localization component for our custom mod menu
            if( ModMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>() != null )
                GameObject.Destroy( ModMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>() );
        }

        void InsertMenuIntoGameHierarchy()
        {
            ModMenuScreen.transform.SetParent( _uim.optionsMenuScreen.gameObject.transform.parent );
            ModMenuScreen.transform.localPosition = _uim.optionsMenuScreen.gameObject.transform.localPosition;
            ModMenuScreen.transform.localScale = _uim.optionsMenuScreen.gameObject.transform.localScale;
        }

        void RemoveGarbageMenuOptions()
        {
            //Log( "Deleting "+  ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Log( "Deleting " + ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Log( "Deleting " + ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Log( "Deleting " + ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Log( "Deleting " + ModMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject );

            //GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject );
        }

        void SetupMenuTogglePrefab()
        {
            if( menuTogglePrefab != null )
                return;

            //find it
            menuTogglePrefab = EnemyRandomizer.FindGameObjectInChildren( _uim.gameObject, "VSyncOption" );

            //remove default menu behaviors
            GameObject.DestroyImmediate( menuTogglePrefab.GetComponent<MenuOptionHorizontal>() );
            GameObject.DestroyImmediate( menuTogglePrefab.GetComponent<MenuSetting>() );
        }

        void SetupOptionsMenu()
        {
            //SETUP MOD MENU
            InstantiateModMenuScreenGameObject();

            SetMenuTitle( "Enemy Randomizer" );

            InsertMenuIntoGameHierarchy();
            
            //TODO: refactor to be more clear, this grabs the "first" menu option which
            //will then be used as a reference point from which to setup/modify our mod's option menu
            ModMenuScreen.defaultHighlight = ModMenuScreen.content.gameObject.transform.GetChild( 0 ).GetChild( 0 ).GetComponent<MenuButton>();
            
            RemoveGarbageMenuOptions();
            
            Back = ModMenuScreen.defaultHighlight.FindSelectableOnUp();
                        
            //find a toggle-able menu element to use for our mod option prefab
            SetupMenuTogglePrefab();
            
            //TODO: figure out what these are doing?
            GameObject.DestroyImmediate( ModMenuScreen.content.GetComponent<VerticalLayoutGroup>() );
            GameObject.Destroy( ModMenuScreen.defaultHighlight.gameObject.transform.parent.gameObject );

            Log( "Printing UI state BEFORE option generation" );
            EnemyRandomizer.DebugPrintObjectTree( _uim.gameObject, true );

            GenerateListOfModOptions();

            //Log( "Printing UI state AFTER option generation" );
            //EnemyRandomizer.DebugPrintObjectTree( _uim.gameObject, true );

            //((Patches.MenuSelectable)Back).cancelAction = Patches.CancelAction.QuitModMenu;
            ( (MenuSelectable)Back ).cancelAction = CancelAction.DoNothing;
            EventTrigger backEvents = Back.gameObject.GetComponent<EventTrigger>();

            backEvents.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry backSubmit = new EventTrigger.Entry {eventID = EventTriggerType.Submit};
            backSubmit.callback.AddListener( data => { _fauxUim.UIquitModMenu(); } );
            backEvents.triggers.Add( backSubmit );

            EventTrigger.Entry backClick = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            backClick.callback.AddListener( data => { _fauxUim.UIquitModMenu(); } );
            backEvents.triggers.Add( backClick );

            //SETUP MOD MENU
        }

        void OnCustomSeed(string seed)
        {
            if( seed.Length <= 0 )
                return;

            //EnemyRandomizer.Instance.GlobalSettings.IntValues[ "CustomSeed" ] = Int32.Parse( seed );
            EnemyRandomizer.Instance.LoadedBaseSeed = Int32.Parse( seed );
            randoSeedMenuItem.OptionList[ 0 ] = seed;
            randoSeedMenuItem.OptionText.text = seed;
        }

        RandomizerFauxMenuOptionHorizontal randoSeedMenuItem;
        UnityEngine.UI.InputField customSeedInput = null;

        List<GameObject> mainMenuOnlyItems = new List<GameObject>();

        void GenerateListOfModOptions()
        {
            List<string> modOptions = EnemyRandomizer.Instance.GlobalSettings.BoolValues.Select(x=>x.Key).ToList();
            
            //TODO: may not be needed....
            modOptions.Remove( "RandomizeDisabledEnemies" );

            //TODO: get this key value a cleaner way....
            modOptions.Insert( 0, "BaseSeed" );
            modOptions.Insert( 0, "CustomSeed" );

            try
            {
                if( modOptions.Count > 0 )
                {
                    ModOptions = new Selectable[ modOptions.Count ];

                    for( int i = 0; i < modOptions.Count; i++ )
                    {
                        GameObject menuItemParent = Object.Instantiate(menuTogglePrefab);

                        string optionName = modOptions[i];
                        string optionLabel = "No Label";

                        if( optionName == "CustomSeed" )
                        {
                            customSeedInput = menuItemParent.AddComponent<UnityEngine.UI.InputField>();
                            customSeedInput.textComponent = menuItemParent.transform.GetChild( 1 ).GetComponent<Text>();

                            //TODO: fix me in the morning :(
                            Text t = Object.Instantiate( customSeedInput.textComponent ) as Text;
                            t.transform.SetParent( customSeedInput.transform );
                            customSeedInput.placeholder = t;
                            t.horizontalOverflow = HorizontalWrapMode.Overflow;
                            t.text = "Click to type a custom seed";                            
                            t.transform.Translate( new Vector3( 500f, 0f, 0f ) );

                            customSeedInput.caretColor = Color.white;
                            customSeedInput.contentType = InputField.ContentType.IntegerNumber;
                            //customSeedInput.onValueChanged.AddListener( OnCustomSeed );
                            customSeedInput.onEndEdit.AddListener( OnCustomSeed );
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
                            

                            ModOptions[ i ] = customSeedInput;

                            mainMenuOnlyItems.Add( customSeedInput.gameObject );

                            optionLabel = "Custom Seed";
                            //EnemyRandomizer.Instance.GlobalSettings.StringValues.TryGetValue( "CustomSeed", out optionLabel );
                        }
                        else
                        {
                            RandomizerFauxMenuOptionHorizontal menuItem = menuItemParent.AddComponent<RandomizerFauxMenuOptionHorizontal>();
                            menuItem.navigation = Navigation.defaultNavigation;
                            EnemyRandomizer.Instance.GlobalSettings.StringValues.TryGetValue( optionName, out optionLabel );

                            if( optionName == "BaseSeed" )
                            {
                                randoSeedMenuItem = menuItem;
                                mainMenuOnlyItems.Add( menuItem.gameObject );
                            }

                            //Manages what should happen when the menu option changes (the user clicks and the mod is toggled On/Off)
                            menuItem.OnUpdate += optionIndex =>
                            {
                                if( optionName == "CustomSeed" )
                                {
                                }
                                else if( EnemyRandomizer.Instance.GlobalSettings.IntValues.ContainsKey( optionName ) )
                                {
                                    int seed = nv.GameRNG.Randi();
                                    EnemyRandomizer.Instance.GlobalSettings.IntValues[ optionName ] = seed;

                                    EnemyRandomizer.Instance.LoadedBaseSeed = seed;
                                        

                                    menuItem.OptionList[ 0 ] = seed.ToString();
                                    customSeedInput.text = "";
                                }
                                else
                                {
                                    if( optionIndex == 1 )
                                    {
                                        EnemyRandomizer.Instance.GlobalSettings.BoolValues[ optionName ] = false;

                                        if( optionName == "RNGChaosMode" )
                                            EnemyRandomizer.Instance.ChaosRNG = false;
                                        if( optionName == "RNGRoomMode" )
                                            EnemyRandomizer.Instance.RoomRNG = false;
                                        //if( optionName == "RandomizeDisabledEnemies" )
                                        //    EnemyRandomizer.Instance.RandomizeDisabledEnemies = false;
                                    }
                                    else
                                    {
                                        EnemyRandomizer.Instance.GlobalSettings.BoolValues[ optionName ] = true;

                                        if( optionName == "RNGChaosMode" )
                                            EnemyRandomizer.Instance.ChaosRNG = true;
                                        if( optionName == "RNGRoomMode" )
                                            EnemyRandomizer.Instance.RoomRNG = true;
                                        //if( optionName == "RandomizeDisabledEnemies" )
                                        //    EnemyRandomizer.Instance.RandomizeDisabledEnemies = true;
                                    }
                                }

                                EnemyRandomizer.Instance.SaveGlobalSettings();
                            };

                            menuItem.OptionText = menuItem.gameObject.transform.GetChild( 1 ).GetComponent<Text>();
                            if( EnemyRandomizer.Instance.GlobalSettings.IntValues.ContainsKey( optionName ) )
                            {
                                menuItem.OptionList = new[] { EnemyRandomizer.Instance.GlobalSettings.IntValues[ optionName ].ToString() };
                                menuItem.SelectedOptionIndex = 0;
                            }
                            else
                            {
                                menuItem.OptionList = new[] { "On", "Off" };
                                menuItem.SelectedOptionIndex = EnemyRandomizer.Instance.GlobalSettings.BoolValues[ optionName ] ? 0 : 1;
                            }

                            menuItem.LocalizeText = false;
                            menuItem.SheetTitle = optionName;
                            menuItem.leftCursor = menuItem.transform.FindChild( "CursorLeft" ).GetComponent<Animator>();
                            menuItem.rightCursor = menuItem.transform.FindChild( "CursorRight" ).GetComponent<Animator>();
                            menuItem.cancelAction = CancelAction.DoNothing;

                            ModOptions[ i ] = menuItem;
                        }
                        
                        
                        Object.DestroyImmediate( menuItemParent.transform.FindChild( "Label" ).GetComponent<AutoLocalizeTextUI>() );
                        menuItemParent.transform.FindChild( "Label" ).GetComponent<Text>().text = optionLabel;
                        
                        menuItemParent.name = optionName;
                        
                        RectTransform rt = menuItemParent.GetComponent<RectTransform>();

                        rt.SetParent( ModMenuScreen.content.transform );
                        rt.localScale = new Vector3( 2, 2, 2 );

                        rt.sizeDelta = new Vector2( 960, 120 );
                        rt.anchoredPosition = new Vector2( 0, ( 766 / 2 ) - 90 - ( 150 * i ) );
                        rt.anchorMin = new Vector2( 0.5f, 1.0f );
                        rt.anchorMax = new Vector2( 0.5f, 1.0f );

                        //Image img = menuItem.AddComponent<Image>();
                        //img.sprite = nullSprite();

                        //TODO: test...


                        //AutoLocalizeTextUI localizeUI = modArray[i].GetComponent<AutoLocalizeTextUI>();
                        //modArray[i].transform.GetChild(0).GetComponent<Text>().text = mods[i];
                        //GameObject.Destroy(localizeUI);
                    }

                    Navigation[] navs = new Navigation[ModOptions.Length];
                    for( int i = 0; i < ModOptions.Length; i++ )
                    {
                        navs[ i ] = new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnUp = i == 0 ? Back : ModOptions[ i - 1 ],
                            selectOnDown = i == ModOptions.Length - 1 ? Back : ModOptions[ i + 1 ]
                        };

                        ModOptions[ i ].navigation = navs[ i ];
                    }

                    ModMenuScreen.defaultHighlight = ModOptions[ 1 ];
                    Navigation nav2 = Back.navigation;
                    nav2.selectOnUp = ModOptions[ ModOptions.Length - 1 ];
                    nav2.selectOnDown = ModOptions[ 1 ];
                    Back.navigation = nav2;
                }
            }
            catch( Exception ex )
            {
                Log( "Exception: "+ ex.Message );
            }

        }

        void SetupButtonEvents()
        {
            EnemyRandomizer.Instance.Log( "About to add the events to the menu option" );
            //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
            EventTrigger events = modButton.gameObject.GetComponent<EventTrigger>();

            events.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry submit = new EventTrigger.Entry {eventID = EventTriggerType.Submit};
            submit.callback.AddListener( data => { _fauxUim.UIloadModMenu(); } );
            events.triggers.Add( submit );

            EventTrigger.Entry click = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            click.callback.AddListener( data => { _fauxUim.UIloadModMenu(); } );
            events.triggers.Add( click );

            //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
        }

        private void SceneLoaded(Scene scene, LoadSceneMode lsm)
        {
            if( scene.name != "Menu_Title" )
            {
                ShowNonInGameOptions( false );
                return;
            }

            ShowNonInGameOptions( true );

            //don't try to "enable" the UI after it's already "enabled"
            if( _uim == null )
            {
                nv.Contractor enableUI = new nv.Contractor(LoadRandomizerMenu, 0.4f);
                enableUI.Start();
            }
        }

        void ShowNonInGameOptions(bool show)
        {
            foreach(GameObject go in mainMenuOnlyItems )
            {
                go.SetActive( show );
            }
        }

        void LoadRandomizerMenu()
        {
            try
            {
                if( _uim != null || EnemyRandomizer.Instance == null || EnemyRandomizer.Instance.GlobalSettings == null || UIManager.instance == null ) return;
            }
            catch( NullReferenceException )
            {
                //Do Nothing.  Something inside of UIManager.instance breaks even if you try to check for null on it. 
                return;
            }

            Log( "Creating mod options menu..." );

            _uim = UIManager.instance;
            uiManagerCanvasRoot = EnemyRandomizer.FindGameObjectInChildren( _uim.gameObject, "UICanvas" );

            AddModMenuButtonToOptionMenu();

            SetupOptionsMenu();

            SetupButtonEvents();

            //hack to fix an odd font bug that was scrambling the title screen text (probably due to how unity UI "handles" layout components)
            if( uiManagerCanvasRoot != null )
            {
                uiManagerCanvasRoot.SetActive( false );

                nv.Contractor reEnableUI = new nv.Contractor(ReEnableUI, 0.4f);
                reEnableUI.Start();
            }
        }

        //called by a contractor at the end of the SceneLoaded function
        void ReEnableUI()
        {
            uiManagerCanvasRoot.SetActive( true );
        }

        void Log(string s)
        {
            EnemyRandomizer.Instance.Log( s );
        }
    }
}
