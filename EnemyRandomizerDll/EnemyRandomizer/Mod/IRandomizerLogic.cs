using System.IO;
using Modding;
using UnityEngine.SceneManagement;
using UnityEngine;
using Language;
using On;

using System.Xml.Serialization;
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

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
        /// After the logic is created, give it a reference to the replacement functionality
        /// </summary>
        void Setup(EnemyRandomizerDatabase database);

        /// <summary>
        /// Return the title, description, and entries used by this module
        /// </summary>
        EnemyRandomizer.SubpageDef GetSubpage();

        /// <summary>
        /// invoked after the logics are completly loaded and have generated their subpages
        /// </summary>
        void InitDefaultStatesFromSettings();

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
        /// Invoked when a game is started
        /// </summary>
        void OnStartGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Invoked when a game is saved
        /// </summary>
        void OnSaveGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Configure the RNG/seed for the next thing to be replaced
        /// </summary>
        RNG GetRNG(ObjectMetadata sourceData, RNG rng, int seed);

        /// <summary>
        /// Gets the kinds of things that may be used as replacements
        /// </summary>
        List<PrefabObject> GetValidReplacements(ObjectMetadata sourceData, List<PrefabObject> validReplacementObjects);

        /// <summary>
        /// Use some kind of logic to replace things
        /// </summary>
        ObjectMetadata GetReplacement(ObjectMetadata sourceData, List<PrefabObject> validReplacementObjects, RNG rng);
        
        /// <summary>
        /// Use some kind of logic to optionally modify an object 
        /// </summary>
        ObjectMetadata ModifyObject(ObjectMetadata sourceData);

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

        protected virtual EnemyRandomizerDatabase Database { get; set; }
        protected virtual LogicSettings Settings { get => EnemyRandomizer.GlobalSettings.GetLogicSettings(Name); }

        public virtual void Setup(EnemyRandomizerDatabase database)
        {
            Database = database;
        }

        public virtual void InitDefaultStatesFromSettings()
        {
            for(int i = 0; i < ModOptions.Count; ++i)
            {
                Settings.GetOption(ModOptions[i].Name).value = ModOptions[i].DefaultState;
                Settings.SetMenuOptionState(ModOptions[i].Name, ModOptions[i].DefaultState);
            }
        }

        public virtual EnemyRandomizer.SubpageDef GetSubpage()
        {
            var subpage = new EnemyRandomizer.SubpageDef()
            {
                description = Info,
                title = Name,
                owner = this,
                entries = GetEntries()
            };

            return subpage;
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

        public virtual void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            OnStartGameWasCalled = true;
        }

        public virtual void OnSaveGame(EnemyRandomizerPlayerSettings settings)
        {
        }

        public virtual List<PrefabObject> GetValidReplacements(ObjectMetadata original, List<PrefabObject> validReplacementObjects)
        {
            return validReplacementObjects;
        }

        public virtual RNG GetRNG(ObjectMetadata sourceData, RNG rng, int seed)
        {
            return rng;
        }

        public virtual ObjectMetadata GetReplacement(ObjectMetadata original, List<PrefabObject> validReplacements, RNG rng)
        {
            return original;
        }

        public virtual ObjectMetadata ModifyObject(ObjectMetadata original)
        {
            return original;
        }

        public virtual List<PrefabObject> GetAllowedHazardReplacements(ObjectMetadata sourceData)
        {
            return Database.hazardPrefabs;
        }

        public virtual PersistentBoolData ReplacePersistentBoolItemData(PersistentBoolItem other)
        {
            return null;
        }

        public virtual (string ID, string SceneName) ReplacePersistentBoolItemSetMyID(PersistentBoolItem other)
        {
            return (null, null);
        }






        //protected const string DefaultRandomizeEnemiesOption = "Randomize Enemies";
        //protected const string DefaultRandomizeHazardsOption = "Randomize Hazards";
        //protected const string DefaultRandomizeEffectsOption = "Randomize Projectiles";

        protected virtual List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get => new List<(string, string, bool)>();
            //{
            //    (DefaultRandomizeEnemiesOption, "Should this change enemies? (Only one mod may set this to true)", true ),
            //    (DefaultRandomizeHazardsOption, "Should this change hazards? (Only one mod may set this to true)", false ),
            //    (DefaultRandomizeEffectsOption, "Should this change effects? (Only one mod may set this to true)", false ),
            //};
        }

        public virtual List<IMenuMod.MenuEntry> GetEntries()
        {
            return ModOptions.Select(x => CreateOption(x.Name, x.Info)).ToList();
        }

        public string[] toggle = new string[]
        {
            Language.Language.Get("MOH_OFF", "MainMenu"),
            Language.Language.Get("MOH_ON", "MainMenu")
        };

        public IMenuMod.MenuEntry CreateOption(string name, string info)
        {
            IMenuMod.MenuEntry entry = new IMenuMod.MenuEntry()
            {
                Name = name,
                Description = info,
                Values = this.toggle,
                Saver = (x) => { SetOptionStateFromMenu(name, x == 1); },
                Loader = () => { return Settings.GetOption(name).value ? 1 : 0; },
            };

            return entry;
        }

        public virtual void SetOptionStateFromMenu(string name, bool state)
        {
            Settings.GetOption(name).value = state;
        }
    }
}





///// <summary>
///// Gets the kinds of hazards that may be used as replacements
///// </summary>
//List<PrefabObject> GetAllowedHazardReplacements(ObjectMetadata sourceData);

///// <summary>
///// Use some kind of logic to replace/modify hazards
///// </summary>
//GameObject ReplaceHazardObject(GameObject other, List<PrefabObject> allowedReplacements, RNG rng);

///// <summary>
///// Use some kind of logic to modify a (potentially) new hazard using the given source data
///// </summary>
//ObjectMetadata ModifyHazardObject(ObjectMetadata other, ObjectMetadata sourceData);

///// <summary>
///// Gets the kinds of effects that may be used as replacements
///// </summary>
//List<PrefabObject> GetAllowedEffectReplacements(ObjectMetadata sourceData);

///// <summary>
///// Use some kind of logic to replace effects/projectiles
///// </summary>
//GameObject ReplacePooledObject(GameObject other, List<PrefabObject> allowedReplacements, RNG rng);

///// <summary>
///// Use some kind of logic to modify a (potentially) new effect/projectile using the given source data
///// </summary>
//ObjectMetadata ModifyPooledObject(ObjectMetadata other, ObjectMetadata sourceData);