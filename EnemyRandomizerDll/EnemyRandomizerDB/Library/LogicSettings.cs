using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.Events;

#if !LIBRARY
using Dev = EnemyRandomizerMod.Dev;
#else
using Dev = Modding.Logger;
#endif


//EnemyRandomizerMod.EnemyRandomizer.DebugSpawnEnemy("Fly",null);

namespace EnemyRandomizerMod
{
    /// <summary>
    /// Settings container used by modules to contain the options used by a logic module
    /// </summary>
    public class LogicSettings
    {
        public string name;
        public List<LogicOption> options = new List<LogicOption>();
    }


    /// <summary>
    /// Option container used by modules to the value of thier toggle settings
    /// </summary>
    public class LogicOption
    {
        public string name;
        public bool value;
    }

    /// <summary>
    /// Methods to interact with the various logic options
    /// </summary>
    public static class LogicSettingsMethods
    {
        /// <summary>
        /// Checks if the logic settings container exists and if not then it creates one.
        /// Will always return a valid settings container even if an invalid name is passed.
        /// </summary>
        public static LogicSettings GetLogicSettings(this EnemyRandomizerSettings self, string logicName)
        {
            if (!self.logicSettings.Any(x => x.name == logicName))
            {
                self.logicSettings.Add(new LogicSettings() { name = logicName });
            }
            return self.logicSettings.FirstOrDefault(x => x.name == logicName);
        }

        /// <summary>
        /// Checks if the option reference container exists and if not then it creates one.
        /// Will always return an option container even if an invalid name is passed.
        /// </summary>
        public static LogicOption GetOption(this LogicSettings settings, string optionName, bool? defaultValue = null)
        {
            if (!settings.options.Any(x => x.name == optionName))
            {
                var newOption = new LogicOption() { name = optionName };
                if (defaultValue != null)
                {
                    newOption.value = defaultValue.Value;
                }
                settings.options.Add(newOption);
            }

            return settings.options.FirstOrDefault(x => x.name == optionName);
        }

        /// <summary>
        /// Checks it the option exists
        /// </summary>
        public static bool HasOption(this LogicSettings settings, string optionName)
        {
            return settings.options.Any(x => x.name == optionName);
        }

        /// <summary>
        /// Returns null if the option name is invalid or uninitialized
        /// </summary>
        public static bool? GetOptionValue(this LogicSettings settings, string optionName)
        {
            if (settings.HasOption(optionName))
            {
                return settings.GetOption(optionName).value;
            }

            return null;
        }
    }

}
