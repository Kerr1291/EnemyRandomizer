using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using ModType = EnemyRandomizerMod.EnemyRandomizer;
using Object = UnityEngine.Object;
using nv;

namespace EnemyRandomizerMod.Menu
{
    //TODO: refactor menu class so the menu loading is cleaner
    public class RandomizerMenu
    {
        public static RandomizerMenu Instance { get; private set; }

        CommunicationNode comms;

        static UIManager rootUIManager;
        static RandomizerFauxUIManager optionsUIManager;

        static public MenuScreen optionsMenuScreen;
        static Selectable[] modOptions;
        static Selectable backButton;

        GameObject loadingRoot;
        GameObject loadingButton;
        GameObject loadingBar;
        Text loadingBarText;

        MenuButton enterOptionsMenuButton;
        GameObject menuTogglePrefab;
        GameObject uiManagerCanvasRoot;

        bool loadingButtonPressed;

        public static string MainMenuSceneName {
            get {
                return "Menu_Title";
            }
        }

        public static string OptionsUIManagerName {
            get {
                return "RandoOptionsUIManager";
            }
        }

        public RandomizerMenu()
        {
        }

        public void Setup()
        {
            Dev.Where();

            Instance = this;
            comms = new CommunicationNode();
            comms.EnableNode( this );

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;

            Dev.Log( "Menu Loaded!" );
        }

        public void AddLoadingButtonCallback( UnityEngine.Events.UnityAction loadingCallback )
        {
            if( loadingButton == null )
                return;

            Dev.Where();
            
            Button enableButton = loadingButton.GetComponentInChildren<Button>();
            enableButton.onClick.RemoveListener( loadingCallback );
            enableButton.onClick.AddListener( loadingCallback );
        }

        void SetLoadingBarProgress(float progress)
        {
            if( progress < 1f )
                loadingBar.SetActive( true );
            else
                loadingBar.SetActive( false );

            loadingBarText.text = "Loading Progress: " +(int)(progress * 100.0f)+"%";
        }

        public void Unload()
        {
            GameObject.Destroy( loadingBar );
            GameObject.Destroy( loadingButton );
            GameObject.Destroy( loadingRoot );
            GameObject.Destroy( optionsUIManager );

            //destroys all menu items (ModOptions)
            GameObject.Destroy( optionsMenuScreen );
            modOptions = null;
            backButton = null;

            optionsMenuScreen = null;
            optionsUIManager = null;
            loadingBar = null;
            loadingButton = null;
            loadingRoot = null;

            rootUIManager = null;

            loadingButtonPressed = false;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;

            comms.DisableNode();
            comms = null;
            Instance = null;
        }


        void CreateMainMenuUIElements()
        {
            if( rootUIManager == null )
                return;

            GameObject go = new GameObject( OptionsUIManagerName );
            optionsUIManager = go.AddComponent<RandomizerFauxUIManager>();

            Vector2 buttonPos = new Vector2( -1400f, 400f );
            Vector2 barPos = new Vector2( -1400f, 200f );

            Vector2 buttonSize = new Vector2( 400f, 200f );
            Vector2 barSize = new Vector2( 400f, 40f );

            loadingRoot = new GameObject( "Loading Root" );

            Canvas canvas = loadingRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.GetOrAddComponent<RectTransform>().sizeDelta = new Vector2( 1920f, 1080f );
            CanvasScaler canvasScaler = loadingRoot.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2( 1920f, 1080f );

            //rootUIManager.gameObject.PrintSceneHierarchyTree();

            Vector3 pos = rootUIManager.gameObject.FindGameObjectInChildren( "StartGameButton" ).transform.position;

            loadingButton = GameObject.Instantiate(rootUIManager.gameObject.FindGameObjectInChildren( "StartGameButton" ) );
            loadingButton.transform.SetParent( rootUIManager.gameObject.FindGameObjectInChildren( "MainMenuScreen" ).transform );
            loadingButton.gameObject.SetActive( true );
            loadingButton.transform.localScale = Vector3.one;
            loadingButton.transform.position = pos;
            loadingButton.transform.Translate( new Vector3( -4.8f, 4f ) );

            //Dev.Log( "Finished loader ui elements" );

            GameObject.DestroyImmediate( loadingButton.GetComponent<MenuButton>() );
            GameObject.DestroyImmediate( loadingButton.GetComponent<EventTrigger>() );
            GameObject.DestroyImmediate( loadingButton.GetComponentInChildren<AutoLocalizeTextUI>() );

            Text loadingButtonText = loadingButton.GetComponentInChildren<Text>();
            loadingButtonText.text = "[Load Enemy Randomizer]";
            loadingButtonText.color = Color.white;
            loadingButtonText.horizontalOverflow = HorizontalWrapMode.Overflow;
            loadingButtonText.verticalOverflow = VerticalWrapMode.Overflow;

            Button b = loadingButton.AddComponent<Button>();
            b.targetGraphic = loadingButtonText;
            b.interactable = true;
            ColorBlock cb = new ColorBlock
            {
                highlightedColor = Color.yellow,
                pressedColor = Color.red,
                disabledColor = Color.black,
                normalColor = Color.white,
                colorMultiplier = 2f
            };
            b.colors = cb;
            b.onClick.AddListener( LoadingButtonClicked );

            AddLoadingButtonCallback( EnemyRandomizerLoader.Instance.BuildEnemyRandomizerDatabase );



            loadingBar = GameObject.Instantiate( rootUIManager.gameObject.FindGameObjectInChildren( "StartGameButton" ) );
            loadingBar.transform.SetParent( rootUIManager.gameObject.FindGameObjectInChildren( "MainMenuScreen" ).transform );
            loadingBar.gameObject.SetActive( true );
            loadingBar.transform.localScale = Vector3.one;
            loadingBar.transform.position = pos;
            loadingBar.transform.Translate( new Vector3( -4.8f, 3.7f ) );

            GameObject.DestroyImmediate( loadingBar.GetComponent<MenuButton>() );
            GameObject.DestroyImmediate( loadingBar.GetComponent<EventTrigger>() );
            GameObject.DestroyImmediate( loadingBar.GetComponentInChildren<AutoLocalizeTextUI>() );

            loadingBarText = loadingBar.GetComponentInChildren<Text>();
            loadingBarText.text = "Loading Progress: ";
            loadingBarText.color = Color.white;
            loadingBarText.horizontalOverflow = HorizontalWrapMode.Overflow;
            loadingBarText.verticalOverflow = VerticalWrapMode.Overflow;
            loadingBar.gameObject.SetActive( false );

            GameObject.DontDestroyOnLoad( loadingRoot );
        }

        void LoadingButtonClicked()
        {
            loadingButtonPressed = true;
            loadingButton.gameObject.SetActive( false );
        }

        Contractor showDatabaseUIWhenReady = new Contractor();

        void ShowRandoDatabaseUI( bool show )
        {
            if( loadingButtonPressed )
                return;

            if( loadingRoot != null )
            {
                loadingRoot.SetActive( show );
            }
            else
            {
                showDatabaseUIWhenReady.OnUpdate = DoShowRandoDatabaseUI;
                showDatabaseUIWhenReady.Looping = true;
                showDatabaseUIWhenReady.Start();
            }
        }

        void DoShowRandoDatabaseUI()
        {
            if( loadingRoot == null )
            {
                CreateMainMenuUIElements();
            }

            if( loadingRoot != null )
            {
                showDatabaseUIWhenReady.Reset();
                loadingRoot.SetActive( true );
            }
        }

        //called by a contractor at the end of the SceneLoaded function
        void ShowOptionsUI()
        {
            uiManagerCanvasRoot.SetActive( true );
        }

        void SceneLoaded( Scene scene, LoadSceneMode lsm )
        {
            bool isTitleScreen = (string.Compare( scene.name, MainMenuSceneName ) == 0);

            if( !isTitleScreen )
            {
                ShowNonInGameOptions( false );
                return;
            }

            ShowRandoDatabaseUI( isTitleScreen );
            ShowNonInGameOptions( true );

            //don't try to "enable" the UI after it's already "enabled"
            if( rootUIManager == null )
            {
                nv.Contractor enableUI = new nv.Contractor( LoadOptionsMenu, 0.4f );
                enableUI.Start();
            }
        }

        void ShowNonInGameOptions( bool show )
        {
            //Dev.LogVar( "show",show );

            if( mainMenuOnlyItems == null )
                return;

            for( int i = 0; i < mainMenuOnlyItems.Count; )
            {
                if( mainMenuOnlyItems[i] == null )
                {
                    mainMenuOnlyItems.RemoveAt( i );
                    i = 0;
                    continue;
                }

                mainMenuOnlyItems[i].SetActive( show );
                ++i;
            }
        }

        void LoadOptionsMenu()
        {
            try
            {
                if( rootUIManager != null || ModType.Instance == null || ModType.Instance.GlobalSettings == null || UIManager.instance == null ) return;
            }
            catch( NullReferenceException )
            {
                //Do Nothing.  Something inside of UIManager.instance breaks even if you try to check for null on it. 
                return;
            }

            Dev.Log( "Creating mod options menu..." );

            rootUIManager = UIManager.instance;

            //TODO: for testing
            //rootUIManager.gameObject.PrintSceneHierarchyTree(true);

            uiManagerCanvasRoot = rootUIManager.gameObject.FindGameObjectInChildren( "UICanvas" );

            AddModMenuButtonToOptionMenu();

            SetupOptionsMenu();

            SetupButtonEvents();

            //hack to fix an odd font bug that was scrambling the title screen text (probably due to how unity UI "handles" layout components)
            if( uiManagerCanvasRoot != null )
            {
                uiManagerCanvasRoot.SetActive( false );

                nv.Contractor reEnableUI = new nv.Contractor( ShowOptionsUI, 0.4f );
                reEnableUI.Start();
            }
        }

        //TODO: add params for button text and position
        void AddModMenuButtonToOptionMenu()
        {
            //ADD MODS TO OPTIONS MENU
            MenuButton defButton = (MenuButton)rootUIManager.optionsMenuScreen.defaultHighlight;
            enterOptionsMenuButton = Object.Instantiate( defButton.gameObject ).GetComponent<MenuButton>();

            Navigation nav = enterOptionsMenuButton.navigation;

            nav.selectOnUp = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();
            nav.selectOnDown = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();

            enterOptionsMenuButton.navigation = nav;

            nav = enterOptionsMenuButton.FindSelectableOnUp().navigation;
            nav.selectOnDown = enterOptionsMenuButton;
            enterOptionsMenuButton.FindSelectableOnUp().navigation = nav;

            nav = enterOptionsMenuButton.FindSelectableOnDown().navigation;
            nav.selectOnUp = enterOptionsMenuButton;
            enterOptionsMenuButton.FindSelectableOnDown().navigation = nav;

            enterOptionsMenuButton.name = ModType.Instance.GetName();

            enterOptionsMenuButton.transform.SetParent( enterOptionsMenuButton.FindSelectableOnUp().transform.parent );

            enterOptionsMenuButton.transform.localPosition = new Vector2( 0, -240 );
            enterOptionsMenuButton.transform.localScale = enterOptionsMenuButton.FindSelectableOnUp().transform.localScale;

            Object.Destroy( enterOptionsMenuButton.gameObject.GetComponent<AutoLocalizeTextUI>() );
            enterOptionsMenuButton.gameObject.transform.FindChild( "Text" ).GetComponent<Text>().text = "Enemy Randomizer";
            //ADD MODS TO OPTIONS MENU
        }

        void InstantiateModMenuScreenGameObject()
        {
            if( optionsMenuScreen != null )
                return;

            GameObject go = Object.Instantiate( rootUIManager.optionsMenuScreen.gameObject );
            optionsMenuScreen = go.GetComponent<MenuScreen>();
            optionsMenuScreen.title = optionsMenuScreen.gameObject.transform.FindChild( "Title" ).GetComponent<CanvasGroup>();
            optionsMenuScreen.topFleur = optionsMenuScreen.gameObject.transform.FindChild( "TopFleur" ).GetComponent<Animator>();
            optionsMenuScreen.content = optionsMenuScreen.gameObject.transform.FindChild( "Content" ).GetComponent<CanvasGroup>();
        }

        void SetMenuTitle( string title )
        {
            if( optionsMenuScreen == null )
                return;

            if( optionsMenuScreen.title == null )
                optionsMenuScreen.title = optionsMenuScreen.gameObject.transform.FindChild( "Title" ).GetComponent<CanvasGroup>();

            optionsMenuScreen.title.gameObject.GetComponent<Text>().text = title;

            //remove the localization component for our custom mod menu
            if( optionsMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>() != null )
                GameObject.Destroy( optionsMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>() );
        }

        void InsertMenuIntoGameHierarchy()
        {
            optionsMenuScreen.transform.SetParent( rootUIManager.optionsMenuScreen.gameObject.transform.parent );
            optionsMenuScreen.transform.localPosition = rootUIManager.optionsMenuScreen.gameObject.transform.localPosition;
            optionsMenuScreen.transform.localScale = rootUIManager.optionsMenuScreen.gameObject.transform.localScale;
        }

        void RemoveGarbageMenuOptions()
        {
            //Log( "Deleting "+  ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject );

            //GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject );
        }

        void SetupMenuTogglePrefab()
        {
            if( menuTogglePrefab != null )
                return;

            //find it
            menuTogglePrefab = rootUIManager.gameObject.FindGameObjectInChildren( "VSyncOption" );

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
            optionsMenuScreen.defaultHighlight = optionsMenuScreen.content.gameObject.transform.GetChild( 0 ).GetChild( 0 ).GetComponent<MenuButton>();

            RemoveGarbageMenuOptions();

            backButton = optionsMenuScreen.defaultHighlight.FindSelectableOnUp();

            //find a toggle-able menu element to use for our mod option prefab
            SetupMenuTogglePrefab();

            GameObject.DestroyImmediate( optionsMenuScreen.content.GetComponent<VerticalLayoutGroup>() );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.gameObject.transform.parent.gameObject );

            GenerateListOfModOptions();

            ((MenuSelectable)backButton).cancelAction = CancelAction.DoNothing;
            EventTrigger backEvents = backButton.gameObject.GetComponent<EventTrigger>();

            backEvents.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry backSubmit = new EventTrigger.Entry { eventID = EventTriggerType.Submit };
            backSubmit.callback.AddListener( data => { optionsUIManager.UIquitModMenu(); } );
            backEvents.triggers.Add( backSubmit );

            EventTrigger.Entry backClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            backClick.callback.AddListener( data => { optionsUIManager.UIquitModMenu(); } );
            backEvents.triggers.Add( backClick );

            //SETUP MOD MENU
        }

        void OnCustomSeed( string seed )
        {
            if( seed.Length <= 0 )
                return;

            //EnemyRandomizer.Instance.GlobalSettings.IntValues[ "CustomSeed" ] = Int32.Parse( seed );
            EnemyRandomizer.Instance.OptionsMenuSeed = Int32.Parse( seed );
            randoSeedMenuItem.OptionList[ 0 ] = seed;
            randoSeedMenuItem.OptionText.text = seed;
        }

        RandomizerFauxMenuOptionHorizontal randoSeedMenuItem;
        UnityEngine.UI.InputField customSeedInput = null;

        List<GameObject> mainMenuOnlyItems = new List<GameObject>();

        void GenerateListOfModOptions()
        {
            List<string> modOptions = EnemyRandomizer.Instance.GlobalSettings.BoolValues.Select( x => x.Key ).ToList();

            Dev.LogVarArray( "modOptions", modOptions );

            modOptions.Insert( 0, EnemyRandomizerSettingsVars.Seed ); //option 1
            modOptions.Insert( 0, EnemyRandomizerSettingsVars.CustomSeed ); //option 0
            modOptions.Add( EnemyRandomizerSettingsVars.CheatNoclip ); //last option

            try
            {
                if( modOptions.Count > 0 )
                {
                    RandomizerMenu.modOptions = new Selectable[ modOptions.Count ];

                    for( int i = 0; i < modOptions.Count; i++ )
                    {
                        GameObject menuItemParent = Object.Instantiate( menuTogglePrefab );

                        string optionName = modOptions[ i ];
                        string optionLabel = "No Label";

                        Dev.Log( "Setting up " + optionName );

                        //create input field for a custom seed -- TODO refactor this into a method to create an input field element easier
                        if( optionName == EnemyRandomizerSettingsVars.CustomSeed )
                        {
                            customSeedInput = menuItemParent.AddComponent<UnityEngine.UI.InputField>();
                            customSeedInput.textComponent = menuItemParent.transform.GetChild( 1 ).GetComponent<Text>();
                            
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

                            ColorBlock cb = new ColorBlock
                            {
                                highlightedColor = Color.yellow,
                                pressedColor = Color.red,
                                disabledColor = Color.black,
                                normalColor = Color.white,
                                colorMultiplier = 2f
                            };

                            customSeedInput.colors = cb;


                            RandomizerMenu.modOptions[ i ] = customSeedInput;

                            mainMenuOnlyItems.Add( customSeedInput.gameObject );

                            optionLabel = "Custom Seed";
                            //EnemyRandomizer.Instance.GlobalSettings.StringValues.TryGetValue( "CustomSeed", out optionLabel );
                        }
                        else
                        {
                            RandomizerFauxMenuOptionHorizontal menuItem = menuItemParent.AddComponent<RandomizerFauxMenuOptionHorizontal>();
                            menuItem.navigation = Navigation.defaultNavigation;
                            EnemyRandomizer.Instance.GlobalSettings.StringValues.TryGetValue( optionName, out optionLabel );

                            if( optionName == EnemyRandomizerSettingsVars.Seed )
                            {
                                randoSeedMenuItem = menuItem;
                                mainMenuOnlyItems.Add( menuItem.gameObject );
                                optionLabel = "Seed (Click for new)";
                            }

                            if( optionName == EnemyRandomizerSettingsVars.CheatNoclip )
                            {
                                optionLabel = EnemyRandomizerSettingsVars.CheatNoclip;
                            }

                            //Manages what should happen when the menu option changes (the user clicks and the mod is toggled On/Off)
                            menuItem.OnUpdate += optionIndex =>
                            {
                                if( optionName == EnemyRandomizerSettingsVars.Seed )
                                {
                                    int seed = nv.GameRNG.Randi();
                                    EnemyRandomizer.Instance.OptionsMenuSeed = seed;

                                    menuItem.OptionList[ 0 ] = seed.ToString();
                                    if( customSeedInput != null )
                                       customSeedInput.text = "";
                                }
                                else
                                {
                                    if( optionIndex == 1 )
                                    {
                                        if( EnemyRandomizer.Instance.GlobalSettings.BoolValues.ContainsKey(optionName) )
                                            EnemyRandomizer.Instance.GlobalSettings.BoolValues[ optionName ] = false;

                                        if( optionName == EnemyRandomizerSettingsVars.RNGChaosMode )
                                            EnemyRandomizer.Instance.ChaosRNG = false;
                                        if( optionName == EnemyRandomizerSettingsVars.RNGRoomMode )
                                            EnemyRandomizer.Instance.RoomRNG = false;
                                        if( optionName == EnemyRandomizerSettingsVars.CheatNoclip )
                                            EnemyRandomizer.Instance.SetNoclip( false );
                                        if( optionName == EnemyRandomizerSettingsVars.RandomizeGeo )
                                            EnemyRandomizer.Instance.RandomizeGeo = false;


                                    }
                                    else
                                    {
                                        if( EnemyRandomizer.Instance.GlobalSettings.BoolValues.ContainsKey( optionName ) )
                                            EnemyRandomizer.Instance.GlobalSettings.BoolValues[ optionName ] = true;

                                        if( optionName == EnemyRandomizerSettingsVars.RNGChaosMode )
                                            EnemyRandomizer.Instance.ChaosRNG = true;
                                        if( optionName == EnemyRandomizerSettingsVars.RNGRoomMode )
                                            EnemyRandomizer.Instance.RoomRNG = true;
                                        if( optionName == EnemyRandomizerSettingsVars.CheatNoclip )
                                            EnemyRandomizer.Instance.SetNoclip( true );
                                        if( optionName == EnemyRandomizerSettingsVars.RandomizeGeo )
                                            EnemyRandomizer.Instance.RandomizeGeo = true;
                                    }
                                }

                                EnemyRandomizer.Instance.SaveGlobalSettings();
                            };

                            menuItem.OptionText = menuItem.gameObject.transform.GetChild( 1 ).GetComponent<Text>();
                            if( optionName == EnemyRandomizerSettingsVars.Seed )
                            {
                                menuItem.OptionList = new[] { EnemyRandomizer.Instance.OptionsMenuSeed.ToString() };
                                menuItem.SelectedOptionIndex = 0;
                            }
                            else if( optionName == EnemyRandomizerSettingsVars.CheatNoclip )
                            {
                                menuItem.OptionList = new[] { "On", "Off" };
                                menuItem.SelectedOptionIndex = EnemyRandomizer.Instance.NoClipState ? 0 : 1;
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

                            RandomizerMenu.modOptions[ i ] = menuItem;
                        }


                        Object.DestroyImmediate( menuItemParent.transform.FindChild( "Label" ).GetComponent<AutoLocalizeTextUI>() );
                        menuItemParent.transform.FindChild( "Label" ).GetComponent<Text>().text = optionLabel;

                        menuItemParent.name = optionName;

                        RectTransform rt = menuItemParent.GetComponent<RectTransform>();

                        rt.SetParent( optionsMenuScreen.content.transform );
                        rt.localScale = new Vector3( 2, 2, 2 );

                        rt.sizeDelta = new Vector2( 960, 120 );
                        rt.anchoredPosition = new Vector2( 0, (766 / 2) - 90 - (150 * i) );
                        rt.anchorMin = new Vector2( 0.5f, 1.0f );
                        rt.anchorMax = new Vector2( 0.5f, 1.0f );
                    }

                    Navigation[] navs = new Navigation[ RandomizerMenu.modOptions.Length ];
                    for( int i = 0; i < RandomizerMenu.modOptions.Length; i++ )
                    {
                        navs[ i ] = new Navigation
                        {
                            mode = Navigation.Mode.Explicit,
                            selectOnUp = i == 0 ? backButton : RandomizerMenu.modOptions[ i - 1 ],
                            selectOnDown = i == RandomizerMenu.modOptions.Length - 1 ? backButton : RandomizerMenu.modOptions[ i + 1 ]
                        };

                        RandomizerMenu.modOptions[ i ].navigation = navs[ i ];
                    }

                    optionsMenuScreen.defaultHighlight = RandomizerMenu.modOptions[ 1 ];
                    Navigation nav2 = backButton.navigation;
                    nav2.selectOnUp = RandomizerMenu.modOptions[ RandomizerMenu.modOptions.Length - 1 ];
                    nav2.selectOnDown = RandomizerMenu.modOptions[ 1 ];
                    backButton.navigation = nav2;
                }
            }
            catch( Exception ex )
            {
                Dev.Log( "Exception: " + ex.Message );
            }

        }

        void SetupButtonEvents()
        {
            Dev.Log( "About to add the events to the menu option" );
            //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
            EventTrigger events = enterOptionsMenuButton.gameObject.GetComponent<EventTrigger>();

            events.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry submit = new EventTrigger.Entry { eventID = EventTriggerType.Submit };
            submit.callback.AddListener( data => { optionsUIManager.UIloadModMenu(); } );
            events.triggers.Add( submit );

            EventTrigger.Entry click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            click.callback.AddListener( data => { optionsUIManager.UIloadModMenu(); } );
            events.triggers.Add( click );

            //SETUP MOD BUTTON TO RESPOND TO SUBMIT AND CANCEL EVENTS CORRECTLY
        }

        static Sprite NullSprite(Vector2 size, Color32 c)
        {
            Texture2D tex = new Texture2D( (int)size.x, (int)size.y );
            tex.LoadRawTextureData( new byte[] { c.r, c.g, c.b, c.a } );
            tex.Apply();
            return Sprite.Create( tex, new Rect( 0, 0, size.x, size.y ), Vector2.zero );
        }

        static Sprite NullSprite()
        {
            Texture2D tex = new Texture2D( 1, 1 );
            tex.LoadRawTextureData( new byte[] { 0xFF, 0xFF, 0xFF, 0xFF } );
            tex.Apply();
            return Sprite.Create( tex, new Rect( 0, 0, 1, 1 ), Vector2.zero );
        }

        static Sprite CreateSprite( byte[] data, int x, int y, int w, int h )
        {
            Texture2D tex = new Texture2D( 1, 1 );
            tex.LoadImage( data );
            tex.anisoLevel = 0;
            return Sprite.Create( tex, new Rect( x, y, w, h ), Vector2.zero );
        }

        static void DataDump( GameObject go, int depth )
        {
            Dev.Log( new string( '-', depth ) + go.name );
            foreach( Component comp in go.GetComponents<Component>() )
            {
                switch( comp.GetType().ToString() )
                {
                    case "UnityEngine.RectTransform":
                    Dev.Log( new string( '+', depth ) + comp.GetType() + " : " + ((RectTransform)comp).sizeDelta + ", " + ((RectTransform)comp).anchoredPosition + ", " + ((RectTransform)comp).anchorMin + ", " + ((RectTransform)comp).anchorMax );
                    break;
                    case "UnityEngine.UI.Text":
                    Dev.Log( new string( '+', depth ) + comp.GetType() + " : " + ((Text)comp).text );
                    break;
                    default:
                    Dev.Log( new string( '+', depth ) + comp.GetType() );
                    break;
                }
            }
            foreach( Transform child in go.transform )
            {
                DataDump( child.gameObject, depth + 1 );
            }
        }

        [CommunicationCallback]
        public void HandleLoadingProgressEvent( LoadingProgressEvent e, object randoDatabase )
        {
            SetLoadingBarProgress( e.progress );
        }
    }
}
