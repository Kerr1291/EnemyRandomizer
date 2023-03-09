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
        /// Is this logic currently enabled?
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// The rng used by the logic
        /// </summary>
        RNG rng { get; set; }

        /// <summary>
        /// The base seed used to randomize stuff
        /// </summary>
        int baseSeed { get; set; }

        /// <summary>
        /// After the logic is created, give it a reference to the replacement functionality
        /// </summary>
        void Setup(EnemyReplacer replacer);

        /// <summary>
        /// Will be called once the game starts or if the logic is toggled on mid-game
        /// </summary>
        void Enable();

        /// <summary>
        /// Will be called if the logic is toggled off mid-game
        /// </summary>
        void Disable();

        /// <summary>
        /// Invoked when a game is started
        /// </summary>
        void OnStartGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Invoked when a game is saved
        /// </summary>
        void OnSaveGame(EnemyRandomizerPlayerSettings settings);

        /// <summary>
        /// Use some kind of logic to replace enemies
        /// </summary>
        GameObject ReplaceEnemy(GameObject other);

        /// <summary>
        /// Use some kind of logic to replace effects/projectiles
        /// </summary>
        GameObject ReplacePooledObject(GameObject other);

        /// <summary>
        /// Use some kind of logic to replace/modify hazards
        /// </summary>
        GameObject ReplaceHazardObject(GameObject other);

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
        public int baseSeed { get; set; }
        public virtual RNG rng { get; set; }
        public abstract string Name { get; }
        public abstract string Info { get; }
        public virtual bool Enabled { get; protected set; }

        protected virtual EnemyReplacer Replacer { get; set; }
        protected virtual EnemyRandomizerDatabase Database { get; set; }

        protected virtual string GetDatabaseKey(string objectName)
        {
            return EnemyRandomizerDatabase.ToDatabaseKey(objectName);
        }

        public virtual void Setup(EnemyReplacer replacer)
        {
            Replacer = replacer;
            Database = replacer.database;
        }

        public virtual void Enable()
        {
            Enabled = true;
        }

        public virtual void Disable()
        {
            Enabled = false;
        }

        public virtual void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            baseSeed = settings.seed;
        }

        public virtual void OnSaveGame(EnemyRandomizerPlayerSettings settings)
        {
            settings.seed = baseSeed;
        }

        public virtual GameObject ReplaceEnemy(GameObject other)
        {
            return other;
        }

        public virtual GameObject ReplacePooledObject(GameObject other)
        {
            return other;
        }

        public virtual GameObject ReplaceHazardObject(GameObject other)
        {
            return other;
        }

        public virtual PersistentBoolData ReplacePersistentBoolItemData(PersistentBoolItem other)
        {
            return null;
        }

        public virtual (string ID, string SceneName) ReplacePersistentBoolItemSetMyID(PersistentBoolItem other)
        {
            return (null, null);
        }

        /// <summary>
        /// Configure the RNG/seed for the next enemy to be replaced
        /// </summary>
        protected virtual void SetupRNGForReplacement(string enemyName, string sceneName)
        {
            rng = new RNG();
        }
    }
}