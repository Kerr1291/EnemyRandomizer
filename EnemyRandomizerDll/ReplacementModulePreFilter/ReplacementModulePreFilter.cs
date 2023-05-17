using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyRandomizerMod
{
    public class ReplacementModulePreFilter : BaseRandomizerLogic
    {
        public override string Name => "Enemy Filter";

        public override string Info => "Control which enemies will be replaced";

        public override bool EnableByDefault => false;

        List<(string Name, string Info, bool DefaultState)> ModuleOptions = null;

        List<(string Name, string Info, bool DefaultState)> EnemyOptions
        {
            get
            {
                if (ModuleOptions == null)
                {
                    try
                    {
                        ModuleOptions = Database.Enemies.OrderBy(x => x.Value.prefabName).Select(x =>
                        {
                            return (x.Value.prefabName, "", true);
                        }).ToList();
                    }
                    catch (Exception e)
                    {
                        Dev.LogError("Null database or database without enemies was loaded! EnemyEnabler will not load any menu options.");
                        ModuleOptions = new List<(string Name, string Info, bool DefaultState)>();
                    }
                }

                return ModuleOptions;
            }
        }

        List<(string Name, string Info, bool Default)> CustomOptions = new List<(string, string, bool)>()
        {
            ("Enable All", "Enable all options", false),
            ("Disable All", "Disable all options", false),
        };

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
        }

        public virtual bool OptionEnabled(string name)
        {
            return Settings.GetOptionValue(name) ?? false;
        }

        //public virtual bool OptionEnabled(int index)
        //{
        //    int i = Mathf.Clamp(index, 0, ModuleOptions.Count);
        //    return OptionEnabled(ModuleOptions[i].Name);
        //}

        protected override void SetModOptionLoaded(string optionName, bool value)
        {
            if (optionName == CustomOptions[0].Name && value)
            {
                EnemyOptions.ForEach(x => SetModOptionLoaded(x.Name, true));
                return;
            }
            if (optionName == CustomOptions[1].Name && value)
            {
                EnemyOptions.ForEach(x => SetModOptionLoaded(x.Name, false));
                return;
            }

            base.SetModOptionLoaded(optionName, value);
        }

        protected override List<(string Name, string Info, bool DefaultState)> ModOptions
        {
            get
            {
                if (ModuleOptions == null)
                {
                    try
                    {
                        ModuleOptions = Database.Enemies.OrderBy(x => x.Value.prefabName).Select(x =>
                        {
                            return (x.Value.prefabName, "", true);
                        }).ToList();
                    }
                    catch (Exception e)
                    {
                        Dev.LogError("Null database or database without enemies was loaded! EnemyEnabler will not load any menu options.");
                        ModuleOptions = new List<(string Name, string Info, bool DefaultState)>();
                    }
                }

                return CustomOptions.Concat(ModuleOptions).ToList();
            }
        }

        /// <summary>
        /// Invoked when a game is started
        /// </summary>
        public override void OnStartGame(EnemyRandomizerPlayerSettings settings)
        {
            base.OnStartGame(settings);
        }

        /// <summary>
        /// Gets the kinds of things that may be used as replacements
        /// </summary>
        public override bool CanReplaceObject(ObjectMetadata sourceData)
        {
            if (sourceData.ObjectType == PrefabObject.PrefabType.Enemy)
            {
                return OptionEnabled(sourceData.ObjectPrefab.prefabName);
            }

            return true;
        }
    }
}
