#if !LIBRARY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemyRandomizerMod.Logger
{
    public interface IDevLogger
    {
		bool  ShowFileAndLineNumber { get; set; }
        bool  ShowMethodName { get; set; }
        bool  ShowMethodParameters { get; set; }
        bool  ShowClassName { get; set; }
        bool  LoggingEnabled { get; set; }
        bool  GuiLoggingEnabled { get; set; }
        bool  ColorizeText { get; set; }
        Color LogColor { get; set; }
        Color LogWarningColor { get; set; }
        Color LogErrorColor { get; set; }
        Color MethodColor { get; set; }
        Color ParamColor { get; set; }

        List<string> IgnoreFilters { get; }
    }
}
#endif