//TODO: move define into precompiler option
#define USE_HK_MODLOG
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System;

#if USE_HK_MODLOG
using Mod = EnemyRandomizerMod.EnemyRandomizer;
#endif



//disable the unreachable code detected warning for this file
#pragma warning disable 0162

namespace nv
{
    public class DevLog : GameSingleton<DevLog>
    {
        public static new DevLog Instance {
            get {
                DevLog log = GameSingleton<DevLog>.Instance;
                if( log.logRoot == null)
                    log.Setup();
                return log;
            }
        }

        struct LogString
        {
            public string text;
            public GameObject obj;
        }

        Queue<LogString> content = new Queue<LogString>();

        [SerializeField]
        GameObject logRoot = null;

        [SerializeField]
        GameObject logWindow = null;

        [SerializeField]
        GameObject logTextPrefab = null;

        public void SetupPrefabs()
        {
            if(logTextPrefab == null)
            {
                logTextPrefab = new GameObject( "DebugLogTextElement" );
                Text text = logTextPrefab.AddComponent<Text>();
                text.color = Color.red;
                logTextPrefab.SetActive( false );
            }
            if( logRoot == null )
            {
                logRoot = new GameObject( "DebugLogRoot" );
                Canvas canvas = logRoot.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.gameObject.GetOrAddComponent<RectTransform>().sizeDelta = new Vector2( 1920f * .5f, 1080f * 20f );
                CanvasScaler canvasScaler = logRoot.AddComponent<CanvasScaler>();
                canvasScaler.referenceResolution = new Vector2( 1920f, 1080f );
            }
            if( logWindow == null )
            {
                logWindow = new GameObject( "DebugLogWindow" );
                logWindow.transform.SetParent( logRoot.transform );
                CanvasRenderer canvas = logWindow.AddComponent<CanvasRenderer>();

                //create a window that fills its parent
                canvas.gameObject.GetOrAddComponent<RectTransform>().anchorMax = Vector2.one;
                canvas.gameObject.GetOrAddComponent<RectTransform>().anchorMin = Vector2.zero;
                canvas.gameObject.GetOrAddComponent<RectTransform>().sizeDelta = Vector2.zero;
                canvas.gameObject.GetOrAddComponent<RectTransform>().anchoredPosition = Vector2.zero;

                //add background image
                Image bg = logWindow.AddComponent<Image>();

                //mostly black/dark grey transparent background
                bg.color = new Color( .1f, .1f, .1f, .4f );
            }
            GameObject.DontDestroyOnLoad( logTextPrefab );
            GameObject.DontDestroyOnLoad( logRoot );
            GameObject.DontDestroyOnLoad( logWindow );
        }

        public void Hide()
        {
            logRoot.SetActive( false );
        }

        public void Show( bool show = true )
        {
            logRoot.SetActive( show );
        }

        float LineSize()
        {
            return (float)logTextPrefab.GetComponent<Text>().fontSize + logTextPrefab.GetComponent<Text>().lineSpacing;
        }

        void UpdateLog()
        {
            float line_size = LineSize();
            float total_size = content.Count * line_size;
            float max_size = logWindow.GetComponent<RectTransform>().rect.height;
            while( total_size > max_size )
            {
                LogString lString = content.Dequeue();
                Destroy( lString.obj.gameObject );
                total_size -= line_size;
            }
        }

        public void Log( string s )
        {
            if( logTextPrefab == null )
                return;
            if( logWindow == null )
                return;
            if( logRoot == null )
                return;

            LogString str = new LogString() { text = s, obj = GameObject.Instantiate( logTextPrefab, logWindow.transform ) as GameObject };
            str.obj.SetActive( true );
            str.obj.transform.localScale = Vector3.one;
            str.obj.GetComponent<Text>().text = s;
            content.Enqueue( str );
            UpdateLog();
        }

        void Setup()
        {
            //logRoot = new GameObject();
            //Dev.GetOrAddComponent<Canvas>( logRoot );
            //Dev.GetOrAddComponent<RectTransform>( logRoot ).sizeDelta = new Vector2( 1024, 680 ); ;

            SetupPrefabs();
        }
    }


    /// <summary>
    /// Collection of tools, debug or otherwise, to improve the quality of life
    /// </summary>
    public partial class Dev
    {
        #region Internal

        static string GetFunctionHeader( int frame = 0 )
        {
            //get stacktrace info
            StackTrace stackTrace = new StackTrace();
            string class_name = stackTrace.GetFrame( BaseFunctionHeader + frame ).GetMethod().ReflectedType.Name;

            //build parameters string
            System.Reflection.ParameterInfo[] parameters = stackTrace.GetFrame( 3 + frame ).GetMethod().GetParameters();
            string parameters_name = "";
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

            //build function header
            string function_name = stackTrace.GetFrame( BaseFunctionHeader + frame ).GetMethod().Name + "(" + parameters_name + ")";
            return class_name + "." + function_name;
        }

        static string Colorize( string text, string colorhex )
        {
            string str = "<color=#" + colorhex + ">" + "<b>" + text + "</b>" + "</color>";
            return str;
        }

        static string FunctionHeader( int frame = 0 )
        {
            return Dev.Colorize( Dev.GetFunctionHeader( frame ), Dev._method_color ) + " :::: ";
        }

        #endregion

        #region Settings
        
        public static int BaseFunctionHeader = 3;

        static string _method_color = Dev.ColorToHex( Color.cyan );
        static string _log_color = Dev.ColorToHex( Color.white );
        static string _param_color = Dev.ColorToHex( Color.green );

        public class Settings
        {

            public static void SetMethodColor( int r, int g, int b ) { Dev._method_color = ColorStr( r, g, b ); }
            public static void SetMethodColor( float r, float g, float b ) { Dev._method_color = ColorStr( r, g, b ); }
            public static void SetMethodColor( Color c ) { Dev._method_color = Dev.ColorToHex( c ); }

            public static void SetLogColor( int r, int g, int b ) { Dev._log_color = ColorStr( r, g, b ); }
            public static void SetLogColor( float r, float g, float b ) { Dev._log_color = ColorStr( r, g, b ); }
            public static void SetLogColor( Color c ) { Dev._log_color = Dev.ColorToHex( c ); }

            public static void SetParamColor( int r, int g, int b ) { Dev._param_color = ColorStr( r, g, b ); }
            public static void SetParamColor( float r, float g, float b ) { Dev._param_color = ColorStr( r, g, b ); }
            public static void SetParamColor( Color c ) { Dev._param_color = Dev.ColorToHex( c ); }

        }
        #endregion

        #region Logging


        public static void Where()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log( " :::: " + Dev.FunctionHeader() );
#elif USE_HK_MODLOG
            Mod.Instance.Log( " :::: " + Dev.FunctionHeader() );
#endif
        }

        public static void LogError( string text )
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError( Dev.FunctionHeader() + Dev.Colorize( text, ColorToHex( Color.red ) ) );
#elif USE_HK_MODLOG
            Mod.Instance.LogError( Dev.FunctionHeader() + Dev.Colorize( text, ColorToHex( Color.red ) ) );
#else
            DevLog.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, ColorToHex(Color.red) ) );
#endif
        }

        public static void LogWarning( string text )
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning( Dev.FunctionHeader() + Dev.Colorize( text, ColorToHex(Color.yellow) ) );
#elif USE_HK_MODLOG
#else
            DevLog.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, ColorToHex(Color.yellow) ) );
#endif
        }

        public static void Log( string text )
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( text, _log_color ) );
#elif USE_HK_MODLOG
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, _log_color ) );
#else
            DevLog.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, _log_color ) );
#endif
        }

        public static void Log( string text, int r, int g, int b )
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) );
#elif USE_HK_MODLOG
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) );
#else
            DevLog.Instance.Log( ( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) ) );
#endif
        }
        public static void Log( string text, float r, float g, float b )
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) );
#elif USE_HK_MODLOG
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) );
#else
            DevLog.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorStr( r, g, b ) ) );
#endif
        }

        public static void Log( string text, Color color )
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorToHex( color ) ) );
#elif USE_HK_MODLOG
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorToHex( color ) ) );
#else
            DevLog.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( text, Dev.ColorToHex( color ) ) );
#endif
        }

        /// <summary>
        /// Print the value of the variable in a simple and clean way... 
        /// ONLY USE FOR QUICK AND TEMPORARY DEBUGGING (will not work outside editor)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var"></param>
        public static void LogVar<T>( T var )
        {
#if UNITY_EDITOR
            string var_name = GetVarName(var);// var.GetType().
            string var_value = Convert.ToString( var );
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
#elif USE_HK_MODLOG
            string var_name = GetVarName(var);// var.GetType().
            string var_value = Convert.ToString( var );
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
#else
        //in the case of a release build, this will be called instead
        LogVar( "--DEBUG VAR (Change this call to see a meaningful label)--", var );
#endif
        }

        /// <summary>
        /// Print the value of the variable in a simple and clean way
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="var"></param>
        public static void LogVar<T>( string label, T var )
        {
            string var_name = label;
            string var_value = Convert.ToString( var );
#if UNITY_EDITOR
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
#elif USE_HK_MODLOG
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
#else
            DevLog.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
#endif
        }

        /// <summary>
        /// Print the content of the array passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void LogVarArray<T>( string label, IList<T> array )
        {
#if UNITY_EDITOR
            int size = array.Count;
            for( int i = 0; i < size; ++i )
            {
                string vname = label + "[" + Dev.Colorize( Convert.ToString( i ), _log_color ) +"]";
                UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( vname, _param_color ) + " = " + Dev.Colorize( Convert.ToString( array[ i ] ), _log_color ) );
            }
#elif USE_HK_MODLOG
            int size = array.Count;
            for( int i = 0; i < size; ++i )
            {
                string vname = label + "[" + Dev.Colorize( Convert.ToString( i ), _log_color ) +"]";
                Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( vname, _param_color ) + " = " + Dev.Colorize( Convert.ToString( array[ i ] ), _log_color ) );
            }
#endif
        }

        public static void LogVarOnlyThis<T>( string label, T var, string input_name, string this_name )
        {
#if UNITY_EDITOR
            if( this_name != input_name )
                return;

            string var_name = label;
            string var_value = Convert.ToString( var );
            UnityEngine.Debug.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );
            
#elif USE_HK_MODLOG

            if( this_name != input_name )
                return;

            string var_name = label;
            string var_value = Convert.ToString( var );
            Mod.Instance.Log( Dev.FunctionHeader() + Dev.Colorize( var_name, _param_color ) + " = " + Dev.Colorize( var_value, _log_color ) );            
#endif
        }
        #endregion

        #region Helpers

        public static string ColorString( string input, Color color )
        {
            return Dev.Colorize( input, Dev.ColorToHex( color ) );
        }

        public static void PrintHideFlagsInChildren( GameObject parent, bool print_nones = false )
        {
#if UNITY_EDITOR
            bool showed_where = false;

            if( print_nones )
            {
                Dev.Where();
                showed_where = true;
            }

            foreach( Transform child in parent.GetComponentsInChildren<Transform>() )
            {
                if( print_nones && child.gameObject.hideFlags == HideFlags.None )
                    UnityEngine.Debug.Log( Dev.Colorize( child.gameObject.name, Dev.ColorToHex( Color.white ) ) + ".hideflags = " + Dev.Colorize( Convert.ToString( child.gameObject.hideFlags ), _param_color ) );
                else if( child.gameObject.hideFlags != HideFlags.None )
                {
                    if( !showed_where )
                    {
                        Dev.Where();
                        showed_where = true;
                    }
                    UnityEngine.Debug.Log( Dev.Colorize( child.gameObject.name, Dev.ColorToHex( Color.white ) ) + ".hideflags = " + Dev.Colorize( Convert.ToString( child.gameObject.hideFlags ), _param_color ) );
                }
            }
#elif USE_HK_MODLOG
            bool showed_where = false;

            if( print_nones )
            {
                Dev.Where();
                showed_where = true;
            }

            foreach( Transform child in parent.GetComponentsInChildren<Transform>() )
            {
                if( print_nones && child.gameObject.hideFlags == HideFlags.None )
                    Mod.Instance.Log( Dev.Colorize( child.gameObject.name, Dev.ColorToHex( Color.white ) ) + ".hideflags = " + Dev.Colorize( Convert.ToString( child.gameObject.hideFlags ), _param_color ) );
                else if( child.gameObject.hideFlags != HideFlags.None )
                {
                    if( !showed_where )
                    {
                        Dev.Where();
                        showed_where = true;
                    }
                    Mod.Instance.Log( Dev.Colorize( child.gameObject.name, Dev.ColorToHex( Color.white ) ) + ".hideflags = " + Dev.Colorize( Convert.ToString( child.gameObject.hideFlags ), _param_color ) );
                }
            }
#endif
        }

        public static void ClearHideFlagsInChildren( GameObject parent )
        {
            foreach( Transform child in parent.GetComponentsInChildren<Transform>() )
            {
                child.gameObject.hideFlags = HideFlags.None;
            }
        }

#if UNITY_EDITOR
        class GetVarNameHelper
        {
            public static Dictionary<string, string> _cached_name = new Dictionary<string, string>();
        }

        static string GetVarName( object obj )
        {
            StackFrame stackFrame = new StackTrace(true).GetFrame(2);
            string fileName = stackFrame.GetFileName();
            int lineNumber = stackFrame.GetFileLineNumber();
            string uniqueId = fileName + lineNumber;
            if( GetVarNameHelper._cached_name.ContainsKey( uniqueId ) )
                return GetVarNameHelper._cached_name[ uniqueId ];
            else
            {
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                for( int i = 0; i < lineNumber - 1; i++ )
                    file.ReadLine();
                string varName = file.ReadLine().Split(new char[] { '(', ')' })[1];
                GetVarNameHelper._cached_name.Add( uniqueId, varName );
                return varName;
            }
        }
#elif USE_HK_MODLOG
        class GetVarNameHelper
        {
            public static Dictionary<string, string> _cached_name = new Dictionary<string, string>();
        }

        static string GetVarName( object obj )
        {
            //TODO: see if i can encode a path to the local assembly dump
            StackFrame stackFrame = new StackTrace(true).GetFrame(2);
            string fileName = stackFrame.GetFileName();
            //fileName = System.IO.Path.GetFileName( fileName );
            int lineNumber = stackFrame.GetFileLineNumber();
            string uniqueId = fileName + lineNumber;
            if( GetVarNameHelper._cached_name.ContainsKey( uniqueId ) )
            {
                return GetVarNameHelper._cached_name[ uniqueId ];
            }
            else
            {
                string varName = "DevLog::GetVarName() Failed. Var: Type ="+ obj.GetType().Name +" at "+lineNumber+" :: Value ";
                try
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                    for( int i = 0; i < lineNumber - 1; i++ )
                        file.ReadLine();
                    varName = file.ReadLine().Split(new char[] { '(', ')' })[1];
                }
                catch(Exception)
                {
                    ///...eat the exception, we don't care if it failed to find the file or something went wrong
                }
                GetVarNameHelper._cached_name.Add( uniqueId, varName );
                return varName;
            }
        }
#endif
        #endregion
    }

}

#pragma warning restore 0162
