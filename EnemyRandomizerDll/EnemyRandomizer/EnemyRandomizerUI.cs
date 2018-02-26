using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;

using EnemyRandomizerMod.Menu;

namespace EnemyRandomizerMod
{
    public partial class EnemyRandomizer
    {
        //string uiBundlePath = "K:/Games/steamapps/common/Hollow Knight/hollow_knight_Data/Managed/Mods/mainui";

        string UIBundlePath {
            get {
                return Application.dataPath + "/Managed/Mods/mainui";
            }
        }

        GameObject menu = null;
        UnityEngine.UI.Slider loadingBar = null;

        RandomizerMenu settingsMenu;

        void LoadConfigUI()
        {
            settingsMenu = new RandomizerMenu();
        }

        void RestoreUI()
        {
            menu = null;
            loadingBar = null;
        }

        void ToggleBuildRandoDatabaseUI( Scene from, Scene to )
        {
            if( isLoadingDatabase )
                return;

            bool isTitleScreen = (string.Compare(to.name, "Menu_Title") == 0);
            ShowRandoDatabaseUI( isTitleScreen );
        }

        void ShowRandoDatabaseUI( bool show )
        {
            if( databaseGenerated )
                return;

            if( menu == null )
            {
                LoadRandoDatabaseUI();
            }

            if( menu != null )
            {
                menu.SetActive( show );
            }
        }

        void LoadRandoDatabaseUI()
        {
            Log( "Loading mainui bundle from: " + UIBundlePath );
            AssetBundle bundle = AssetBundle.LoadFromFile(UIBundlePath);

            string loadUIRoot = "RandoMainUI(Clone)";
            string loadingBarName = "RandoStartupLoading";
            string enableButtonName = "EnableEnemeyRando";

            if( bundle == null )
            {
                Log( "mainui bundle not found!!" );
                return;
            }

            string[] names = bundle.GetAllAssetNames();

            foreach( var s in names )
            {
                Log( "Loading asset: " + s );
                GameObject bundleObject = bundle.LoadAsset(s) as GameObject;
                GameObject newObject = null;

                if( bundleObject != null )
                {
                    newObject = GameObject.Instantiate( bundleObject );

                    if( newObject.name == loadUIRoot )
                        menu = newObject;
                }
            }

            if( menu == null )
            {
                Log( "Failed to load main randomizer ui!" );
                return;
            }

            //setup enable randomizer button
            GameObject enableRandoButton = FindGameObjectInChildren(menu, enableButtonName);
            UnityEngine.UI.Button enableButton = enableRandoButton.GetComponent<UnityEngine.UI.Button>();
            enableButton.onClick.AddListener( BuildEnemyRandomizerDatabase );

            //setup the loading bar
            if( loadingBar == null )
            {
                GameObject loadingBarObj = FindGameObjectInChildren(menu, loadingBarName);
                loadingBar = loadingBarObj.GetComponent<UnityEngine.UI.Slider>();
                loadingBar.gameObject.SetActive( false );
            }

            //keep objects we've instantiated around, but unload the rest of the bundle
            bundle.Unload( false );
        }
    }
}
