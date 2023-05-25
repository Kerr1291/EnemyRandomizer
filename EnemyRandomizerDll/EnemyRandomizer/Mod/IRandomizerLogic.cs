using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using Language;
using On;

using System.Xml.Serialization;
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Satchel.BetterMenus;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public interface IRandomizerLogic
    {
        /// <summary>
        /// The name of the logic, used to select it from a menu
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the logic for users to understand what this logic should do
        /// </summary>
        string Info { get; }

        /// <summary>
        /// Did the mod get a chance to preload?
        /// </summary>
        bool OnStartGameWasCalled { get; }

        /// <summary>
        /// Is this logic currently enabled?
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Should this logic module be enabled the first time it's loaded by a user
        /// </summary>
        bool EnableByDefault { get; }

        /// <summary>
        /// The database reference being used by the module
        /// </summary>
        EnemyRandomizerDatabase Database { get; }

        /// <summary>
        /// The configuration settings used by the module
        /// </summary>
        LogicSettings Settings { get; }

        /// <summary>
        /// After the logic is created, give it a reference to the replacement functionality
        /// </summary>
        void Setup(EnemyRandomizerDatabase database);

        /// <summary>
        /// Return the Menu used by this module
        /// </summary>
        Menu GetSubpage();

        /// <summary>
        /// Will be called once the game starts or if the logic is toggled on mid-game
        /// </summary>
        void Enable();

        /// <summary>
        /// Will be called if the logic is toggled off mid-game
        /// </summary>
        void Disable();

        /// <summary>
        /// Will be called if a new game or save file is loaded while this logic is enabled
        /// </summary>
        void Reset();

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        bool WillFilterType(PrefabObject.PrefabType prefabType);

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        bool WillReplaceType(PrefabObject.PrefabType prefabType);

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, modify a given type?
        /// </summary>
        bool WillModifyType(PrefabObject.PrefabType prefabType);

        /// <summary>
        /// Used to optimize the randomizer, does this logic, modify rng?
        /// </summary>
        bool WillModifyRNG();

        /// <summary>
        /// Invoked when a game is started
        /// </summary>
        void OnStartGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Invoked when a game is saved
        /// </summary>
        void OnSaveGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Check the logic to see if anything should prevent this from being replaced
        /// </summary>
        bool CanReplaceObject(GameObject gameObject);

        /// <summary>
        /// Gets the kinds of things that may be used as replacements
        /// </summary>
        List<PrefabObject> GetValidReplacements(GameObject objectToModify, List<PrefabObject> validReplacementObjects);

        /// <summary>
        /// Configure the RNG/seed for the next thing to be replaced
        /// </summary>
        RNG GetRNG(GameObject objectToModify, RNG rng, int seed);

        /// <summary>
        /// Use some kind of logic to replace things, return currentPotentialReplacement as the default or something new if you want to substitute the thing that will be spawned
        /// </summary>
        GameObject GetReplacement(GameObject currentPotentialReplacement, GameObject originalObject, List<PrefabObject> validReplacementObjects, RNG rng);
        
        /// <summary>
        /// Use some kind of logic to optionally modify an object 
        /// </summary>
        void ModifyObject(GameObject objectToModify, GameObject originalObject);

        /// <summary>
        /// Use some kind of logic to create/modify persitent bool item's data
        /// For non-boss enemies they should be marked semi persistant
        /// </summary>
        PersistentBoolData ReplacePersistentBoolItemData(PersistentBoolItem other);

        /// <summary>
        /// Use some kind of logic to create/modify persitent bool item's data
        /// For non-boss enemies they should be marked semi persistant
        /// </summary>
        (string ID, string SceneName) ReplacePersistentBoolItemSetMyID(PersistentBoolItem other);
    }

    public abstract class BaseRandomizerLogic : IRandomizerLogic
    {
        public abstract string Name { get; }
        public abstract string Info { get; }
        public virtual bool Enabled { get; protected set; }
        public virtual bool OnStartGameWasCalled { get; protected set; }
        public virtual bool EnableByDefault { get => false; }

        public virtual EnemyRandomizerDatabase Database { get; protected set; }
        public virtual LogicSettings Settings { get => EnemyRandomizer.GlobalSettings.GetLogicSettings(Name); }

        public virtual void Setup(EnemyRandomizerDatabase database)
        {
            Database = database;
        }
        

        public virtual Menu GetSubpage()
        {
            var entries = GetEntries();

            var menu = new Menu(Name, entries.ToArray());

            return menu;
        }

        public virtual void Enable()
        {
            Enabled = true;
        }

        public virtual void Disable()
        {
            Enabled = false;
        }

        public virtual void Reset()
        {
            Enabled = false;
            OnStartGameWasCalled = false;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        public virtual bool WillFilterType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, replace a given type?
        /// </summary>
        public virtual bool WillReplaceType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }

        /// <summary>
        /// Used to optimize the randomizer, does this logic, if enabled, modify a given type?
        /// </summary>
        public virtual bool WillModifyType(PrefabObject.PrefabType prefabType)
        {
            return false;
        }

        public virtual bool WillModifyRNG()
        {
            return false;
        }

        public virtual void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            OnStartGameWasCalled = true;
        }

        public virtual void OnSaveGame(EnemyRandomizerPlayerSettings settings)
        {
        }

        public virtual bool CanReplaceObject(GameObject metaObject)
        {
            return true;
        }

        public virtual List<PrefabObject> GetValidReplacements(GameObject originalObject, List<PrefabObject> validReplacementObjects)
        {
            return validReplacementObjects;
        }

        public virtual RNG GetRNG(GameObject objectToModify, RNG rng, int seed)
        {
            return rng;
        }

        public virtual GameObject GetReplacement(GameObject currentPotentialReplacement, GameObject originalObject, List<PrefabObject> validReplacements, RNG rng)
        {
            return currentPotentialReplacement;
        }

        public virtual void ModifyObject(GameObject objectToModify, GameObject originalObject)
        {
        }

        public virtual PersistentBoolData ReplacePersistentBoolItemData(PersistentBoolItem other)
        {
            return null;
        }

        public virtual (string ID, string SceneName) ReplacePersistentBoolItemSetMyID(PersistentBoolItem other)
        {
            return (null, null);
        }

        protected virtual List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => new List<(string, string, bool)>();
        }

        public virtual List<Element> GetEntries()
        {
            return ModOptions.Select(x => 
                Blueprints.HorizontalBoolOption(
                    name: x.Name, 
                    description: x.Info,
                    applySetting: b => SetModOptionLoaded(x.Name, b),
                    loadSetting: () => IsModOptionLoaded(x.Name)))
                .Select(x => x as Element) // compiler doesn't like implicitly doing this cast
                .ToList();
        }

        protected virtual void SetModOptionLoaded(string optionName, bool value)
        {
            Settings.GetOption(optionName).value = value;
        }

        protected virtual bool IsModOptionLoaded(string optionName)
        {
            bool? defaultValue = null;

            //if the option entry doesn't exist in settings then it's never been loaded before
            if(!Settings.HasOption(optionName))
            {
                //make sure the mod also has a valid entry with this label
                var modOption = ModOptions.FirstOrDefault(x => x.Name == optionName);
                if (!string.IsNullOrEmpty(modOption.Name))
                    defaultValue = modOption.DefaultState;

                //if it does and has a default state, then assign it
                if(defaultValue != null)
                {
                    Settings.GetOption(optionName).value = defaultValue.Value;
                }
            }

            return Settings.GetOption(optionName).value;
        }
    }
}