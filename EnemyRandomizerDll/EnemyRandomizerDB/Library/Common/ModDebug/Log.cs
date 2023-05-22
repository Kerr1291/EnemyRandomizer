#if !LIBRARY
#pragma warning disable 0162
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;

//disable the unreachable code detected warning for this file
#pragma warning disable 0162    

namespace EnemyRandomizerMod
{
    /// <summary>
    /// Collection of tools, debug or otherwise, to improve the quality of life
    /// </summary>
    public class Dev
    {
        public static int BaseFunctionHeader = 3;

        public static Logger.IDevLogger Logger {
            get
            {
                return DevLogger.applicationIsQuitting ? null : DevLogger.Instance;
            }
            set
            {
                //do nothing
            }
        }

        public static Action<string> LogCallback;
        public static Action<string> UnformattedLogCallback;

        public static bool LogWarningsAsDefaultLogs = true;
        public static bool LogErrorsAsDefaultLogs = true;

        public static void InvokeLog(string s, string rawOutput, int logType)
        {
            try
            {
                if( Logger != null && !DevLogger.applicationIsQuitting )
                {
                    if( Logger.IgnoreFilters != null && Logger.IgnoreFilters.Count > 0 )
                    {
                        if( Logger.IgnoreFilters.Any( x => s.Contains( x ) ) )
                            return;

                        if( LogCallback != null )
                            LogCallback( s );
                        else
                        {
                            if( logType == 1 && !LogWarningsAsDefaultLogs )
                            {
                                UnityEngine.Debug.LogWarning( s );
                            }
                            else if( logType == 2 && !LogErrorsAsDefaultLogs )
                            {
                                UnityEngine.Debug.LogError( s );
                            }
                            else
                            {
                                UnityEngine.Debug.Log( s );
                            }
                        }

                        if( UnformattedLogCallback != null )
                        {
                            UnformattedLogCallback.Invoke( rawOutput );
                        }
                    }
                    else
                    {
                        if (LogCallback != null)
                            LogCallback(s);
                        else
                        {
                            if (logType == 1 && !LogWarningsAsDefaultLogs)
                            {
                                UnityEngine.Debug.LogWarning(s);
                            }
                            else if (logType == 2 && !LogErrorsAsDefaultLogs)
                            {
                                UnityEngine.Debug.LogError(s);
                            }
                            else
                            {
                                UnityEngine.Debug.Log(s);
                            }
                        }

                        if (UnformattedLogCallback != null)
                        {
                            UnformattedLogCallback.Invoke(rawOutput);
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.Log(DevLogger.applicationIsQuitting ? "[APPLICATION_IS_QUITTING]: " + s : s);
                }
            }
            catch(Exception)
            {
                UnityEngine.Debug.Log( s );
            }
        }

        #region Helpers
        //Unity 5.2 and onward removed ToHexStringRGB in favor of the ColorUtility class methods
        static string ColorToHex( Color color )
        {
#if UNITY_5_1
            return color.ToHexStringRGB();
#else
            return ColorUtility.ToHtmlStringRGB( color );
#endif
        }

        static string ToHexString( int val )
        {
            return val.ToString( "X" );
        }

        class GetVarNameHelper
        {
            public static Dictionary<string, string> _cached_name = new Dictionary<string, string>();
        }

        static string GetVarName( object obj )
        {
            StackFrame stackFrame = new StackTrace( true ).GetFrame( 2 );
            string fileName = stackFrame.GetFileName();
            int lineNumber = stackFrame.GetFileLineNumber();
            string uniqueId = fileName + lineNumber;
            if( GetVarNameHelper._cached_name.ContainsKey( uniqueId ) )
                return GetVarNameHelper._cached_name[ uniqueId ];
            else
            {
                string varName = obj.GetType().Name;
                System.IO.StreamReader file = null;
                try
                {
                    file = new System.IO.StreamReader( fileName );
                    for( int i = 0; i < lineNumber - 1; i++ )
                        file.ReadLine();
                    varName = file.ReadLine().Split( new char[] { '(', ')' } )[ 1 ];
                }
                catch( Exception )
                { }
                finally
                {
                    if( file != null )
                        file.Close();
                }

                GetVarNameHelper._cached_name.Add( uniqueId, varName );
                return varName;
            }
        }
        #endregion


        #region Logging


        public static void Where( int stackFrameOffset = 0 )
        {
            if(DevLogger.applicationIsQuitting)
            {
                InvokeLog(" :::: " + Dev.FunctionHeader(stackFrameOffset), UnformattedFunctionHeader(stackFrameOffset), 0);
                return;
            }

            if( Logger != null &&  !Logger.LoggingEnabled )
                return;
            string logString = string.Empty;
            try
            {
                logString = " :::: " + Dev.FunctionHeader( stackFrameOffset );
            }
            catch( Exception ) { }
            InvokeLog( logString, UnformattedFunctionHeader(stackFrameOffset), 0 );
        }

        public static void LogError( string text )
        {
            if (DevLogger.applicationIsQuitting)
            {
                InvokeLog(text, text, 2);
                return;
            }
            if ( Logger != null && !Logger.LoggingEnabled )
                return;
            string logString = string.Empty;
            try
            {
                logString = Dev.FunctionHeader() + Dev.Colorize( text, _log_error_color );
            }
            catch( Exception )
            {
                UnityEngine.Debug.LogError( text );
            }
            InvokeLog( logString, Dev.UnformattedFunctionHeader() + text, 2 );
        }

        public static void LogWarning( string text )
        {
            if (DevLogger.applicationIsQuitting)
            {
                InvokeLog(text, text, 1);
                return;
            }
            if ( Logger != null && !Logger.LoggingEnabled )
                return;

            string logString = string.Empty;
            try
            {
                logString = Dev.FunctionHeader() + Dev.Colorize( text, _log_warning_color );
            }
            catch( Exception )
            {
                UnityEngine.Debug.LogWarning( text );
            }
            InvokeLog( logString, Dev.UnformattedFunctionHeader() + text, 1 );
        }

        public static string ToLogString( string text, int frameOffset = 0 )
        {
            if( Logger != null && !Logger.LoggingEnabled )
                return text;

            string logString = string.Empty;
            try
            {
                logString = ( Dev.FunctionHeader() + Dev.Colorize( text, _log_color ) );
            }
            catch( Exception ){}
            return logString;
        }

        public static string ToLogString( string header, string text )
        {
            if( Logger != null && !Logger.LoggingEnabled )
                return text;

            string logString = string.Empty;
            try
            {
                logString = ( ColorizeHeaderString( header ) + Dev.Colorize( text, _log_color ) );
            }
            catch( Exception ) { }
            return logString;
        }

        public static void Log( string text )
        {
            if (DevLogger.applicationIsQuitting)
            {
                InvokeLog(text, text, 0);
                return;
            }
            if ( Logger != null && !Logger.LoggingEnabled )
                return;

            string logString = string.Empty;
            try
            {
#if DEBUG
                logString = ( Dev.FunctionHeader() + Dev.Colorize( text, _log_color ) );
#else
                logString = ( Dev.FunctionHeader() + text);
#endif
            }
            catch ( Exception )
            {
                UnityEngine.Debug.Log( text );
            }
            InvokeLog( logString, text, 0 );
        }

        public static void Log( string text, int r, int g, int b )
        {
            if (DevLogger.applicationIsQuitting)
            {
                InvokeLog(text, text, 0);
                return;
            }
            if ( Logger != null && !Logger.LoggingEnabled )
                return;

            string logString = string.Empty;
            try
            {
                logString = ( ( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) ) );
            }
            catch( Exception )
            {
                UnityEngine.Debug.Log( text );
            }
            InvokeLog( logString, text, 0 );
        }

        public static void Log( string text, float r, float g, float b )
        {
            if (DevLogger.applicationIsQuitting)
            {
                InvokeLog(text, text, 0);
                return;
            }
            if ( Logger != null && !Logger.LoggingEnabled )
                return;

            string logString = string.Empty;
            try
            {
                logString = ( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) );
            }
            catch( Exception )
            {
                UnityEngine.Debug.Log( text );
            }
            InvokeLog( logString, text, 0 );
        }

        public static void Log( string text, Color color )
        {
            if(DevLogger.applicationIsQuitting)
            {
                InvokeLog(text, text, 0);
                return;
            }

            if( Logger != null && !Logger.LoggingEnabled )
                return;

            string logString = string.Empty;
            try
            {
                logString = ( Dev.FunctionHeader() + Dev.Colorize( text, ColorToHex( color ) ) );
            }
            catch( Exception )
            {
                UnityEngine.Debug.Log( text );
            }
            InvokeLog( logString, text, 0 );
        }

        /// <summary>
        /// Print the value of the variable in a simple and clean way... 
        /// ONLY USE FOR QUICK AND TEMPORARY DEBUGGING (will not work as expected outside the editor)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var"></param>
        //public static void LogVar<T>( T var )
        //{
        //    if( Logger != null && !Logger.LoggingEnabled )
        //        return;

        //    string var_name = string.Empty;

        //    try
        //    {
        //        var_name = GetVarName( var );
        //    }
        //    catch( Exception )
        //    {
        //        var_name = var == null ? "Null" : var.GetType().Name;
        //    }

        //    string var_value = Convert.ToString( var );

        //    string logString = string.Empty;
        //    try
        //    {
        //        logString = ( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
        //    }
        //    catch( Exception ) { }
        //    InvokeLog( logString, var_name + " = " + var_value, 0 );
        //}

        /// <summary>
        /// Print the value of the variable in a simple and clean way
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var"></param>
        //public static void LogVar<T>( string label, T var )
        //{
        //    if( Logger != null && !Logger.LoggingEnabled )
        //        return;
        //    string var_name = label;
        //    string var_value = Convert.ToString( var );


        //    string logString = string.Empty;
        //    try
        //    {
        //        logString = ( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
        //    }
        //    catch( Exception ) { }
        //    InvokeLog( logString, var_name + " = " + var_value, 0 );
        //}

        /// <summary>
        /// Print the content of the array passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        //public static void LogVarArray<T>( IList<T> array )
        //{
        //    if( Logger != null && !Logger.LoggingEnabled )
        //        return;
        //    int size = array.Count;
        //    for( int i = 0; i < size; ++i )
        //    {
        //        try
        //        {
        //            string vname = array.GetType().Name + " of " + typeof( T ).GetType().Name + " " + "[" + Dev.Colorize( Convert.ToString( i ), _log_color ) + "]";
        //            string vname_noformat = array.GetType().Name + " of " + typeof( T ).GetType().Name + " " + "[" + Convert.ToString( i ) + "]";
        //            string logString = Dev.FunctionHeader() + Dev.Colorize( vname, _param_color ) + " = " + Dev.Colorize( Convert.ToString( array[ i ] ), _log_color );
        //            InvokeLog( logString, vname_noformat + " = " + Convert.ToString( array[ i ] ), 0 );
        //        }
        //        catch( Exception ) { }
        //    }
        //}

        /// <summary>
        /// Print the content of the array passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        //public static void LogVarArray<T>( string label, IList<T> array )
        //{
        //    if( Logger != null && !Logger.LoggingEnabled )
        //        return;
        //    int size = array.Count;
        //    for( int i = 0; i < size; ++i )
        //    {
        //        try
        //        {
        //            string vname = label + "[" + Dev.Colorize( Convert.ToString( i ), _log_color ) + "]";
        //            string logString = Dev.FunctionHeader() + Dev.Colorize( vname, _param_color ) + " = " + Dev.Colorize( Convert.ToString( array[ i ] ), _log_color );
        //            string vname_noformat = array.GetType().Name + " of " + typeof( T ).GetType().Name + " " + "[" + Convert.ToString( i ) + "]";
        //            InvokeLog( logString, vname_noformat + " = " + Convert.ToString( array[ i ] ), 0 );
        //        }
        //        catch( Exception ) { }
        //    }
        //}
#endregion

#region Internal

        public static string ColorStr( int r, int g, int b )
        {
            return ToHexString( r ) + ToHexString(g) + ToHexString(b);
        }

        public static string ColorStr(float r, float g, float b)
        {
            return ToHexString(((int)(255.0f * Mathf.Clamp01(r)))) + ToHexString(((int)(255.0f * Mathf.Clamp01(g)))) + ToHexString(((int)(255.0f * Mathf.Clamp01(b))));
        }

        static string Colorize( string text, string colorhex )
        {
            if( Logger == null )
                return text;

            string str = text;
            try
            {
                if( Dev.Logger.ColorizeText )
                    str = "<color=#" + colorhex + ">" + "<b>" + text + "</b>" + "</color>";
            }
            catch( Exception )
            { }
            return str;
        }

        public static string FunctionHeader( int frameOffset = 0 )
        {
            if(Dev.Logger == null)
            {
                return Dev.GetFunctionHeader(frameOffset);
            }

            if(!Dev.Logger.ShowMethodName)
            {
                string fileLineHeader = "";
                if (Dev.Logger.ShowFileAndLineNumber)
                {
                    StackTrace stackTrace = new StackTrace(true);
                    StackFrame stackFrame = stackTrace.GetFrame((BaseFunctionHeader-1) + frameOffset);
                    string file = System.IO.Path.GetFileName(stackFrame.GetFileName());
                    int line = stackFrame.GetFileLineNumber();
                    fileLineHeader = file + "(" + line + "):";
                }

                return fileLineHeader;
            }

            return Dev.Colorize( Dev.GetFunctionHeader( frameOffset, false ), Dev._method_color );
        }

        public static string UnformattedFunctionHeader( int frameOffset = 0 )
        {
            return Dev.GetFunctionHeader(frameOffset, false);// false : Dev.Logger.ShowFileAndLineNumber );
        }

        //public static string FunctionHeader( int frameOffset, bool showFileAndLineNumber )
        //{
        //    return Dev.Colorize( Dev.GetFunctionHeader( frameOffset, showFileAndLineNumber ), Dev._method_color );
        //}

        public static string ColorizeHeaderString(string header)
        {
#if DEBUG
            return Dev.Logger == null ? header : Dev.Colorize( header, Dev._method_color );
#else
            return header;
#endif
        }

        static string GetFunctionHeader(int frameOffset = 0, bool fileInfo = false)
        {
            try
            {
                if( frameOffset <= -BaseFunctionHeader )
                    frameOffset = -BaseFunctionHeader;

                //get stacktrace info
                StackTrace stackTrace = new StackTrace( true );
                StackFrame stackFrame = stackTrace.GetFrame( BaseFunctionHeader + frameOffset );
                System.Reflection.MethodBase method = stackFrame.GetMethod();
                Type classType = method.ReflectedType;
                string class_name = classType.Name;

                bool isIEnumerator = false;

                //we're in a coroutine, get a better set of info
                if( class_name.Contains( ">c__Iterator" ) )
                {
                    isIEnumerator = true;
                    classType = method.ReflectedType.DeclaringType;
                    class_name = classType.Name;
                }

                //build parameters string
                string parameters_name = "";

                if(Dev.Logger != null && Dev.Logger.ShowMethodParameters )
                {
                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                    bool add_comma = false;
                    foreach( System.Reflection.ParameterInfo parameter in parameters )
                    {
                        if( add_comma )
                        {
                            parameters_name += ", ";
                        }

                        parameters_name += Dev.Colorize( parameter.ParameterType.Name, _param_color );
                        parameters_name += " ";
                        parameters_name += Dev.Colorize( parameter.Name, _log_color );

                        add_comma = true;
                    }
                }

                //build function header
                string function_name = "";

                if( isIEnumerator )
                {
                    string realMethodName = method.ReflectedType.Name.Substring( method.ReflectedType.Name.IndexOf( '<' ) + 1, method.ReflectedType.Name.IndexOf( '>' ) - 1 );

                    function_name = "IEnumerator:" + realMethodName;
                }
                else
                {
                    function_name = method.Name + "(" + parameters_name + ")";
                }

                //string file = stackFrame.GetFileName().Remove(0, Application.dataPath.Length);
                string file = System.IO.Path.GetFileName( stackFrame.GetFileName() );
                int line = stackFrame.GetFileLineNumber();

                string fileLineHeader = "";
                if( fileInfo )
                    fileLineHeader = file + "(" + line + "):";

                if(Dev.Logger == null || Dev.Logger.ShowClassName )
                    return fileLineHeader + class_name + "." + function_name + " ";
                else
                    return fileLineHeader + function_name + " ";
            }
            catch(Exception)
            {
                return string.Empty;
            }
        }

#endregion


#region Settings

        static string _method_color {
            get {
                return ColorToHex( Logger == null ? Color.cyan : Dev.Logger.MethodColor );
            }
            set {
                if( Logger == null )
                    return;
                Color temp;
                ColorUtility.TryParseHtmlString( value, out temp );
                Dev.Logger.MethodColor = temp;
            }
        }

        static string _log_color {
            get {
                return ColorToHex( Logger == null ? Color.white : Dev.Logger.LogColor );
            }
            set {
                if( Logger == null )
                    return;
                Color temp;
                ColorUtility.TryParseHtmlString( value, out temp );
                Dev.Logger.LogColor = temp;
            }
        }

        static string _log_warning_color {
            get {
                return ColorToHex( Logger == null ? Color.yellow : Dev.Logger.LogWarningColor );
            }
            set {
                if( Logger == null )
                    return;
                Color temp;
                ColorUtility.TryParseHtmlString( value, out temp );
                Dev.Logger.LogWarningColor = temp;
            }
        }

        static string _log_error_color {
            get {
                return ColorToHex( Logger == null ? Color.red : Dev.Logger.LogErrorColor );
            }
            set {
                if( Logger == null )
                    return;
                Color temp;
                ColorUtility.TryParseHtmlString( value, out temp );
                Dev.Logger.LogErrorColor = temp;
            }
        }

        static string _param_color {
            get {
                return ColorToHex( Logger == null ? Color.green : Dev.Logger.ParamColor );
            }
            set {
                if( Logger == null )
                    return;
                Color temp;
                ColorUtility.TryParseHtmlString( value, out temp );
                Dev.Logger.ParamColor = temp;
            }
        }
#endregion
    }

}

#pragma warning restore 0162
#endif