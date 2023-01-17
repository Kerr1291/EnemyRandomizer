using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Game.Text.RegularExpressions
{
    public static class RegexHelper
    {
        static RegexHelper()
        {
            RegexHelper.s_needsEscaped.Add( ' ' );
            RegexHelper.s_needsEscaped.Add( '{' );
            RegexHelper.s_needsEscaped.Add( '}' );
            RegexHelper.s_needsEscaped.Add( '[' );
            RegexHelper.s_needsEscaped.Add( ']' );
            RegexHelper.s_needsEscaped.Add( '(' );
            RegexHelper.s_needsEscaped.Add( ')' );
            RegexHelper.s_needsEscaped.Add( '<' );
            RegexHelper.s_needsEscaped.Add( '>' );
            RegexHelper.s_needsEscaped.Add( '.' );
            RegexHelper.s_needsEscaped.Add( '/' );
            RegexHelper.s_needsEscaped.Add( '\\' );
            RegexHelper.s_needsEscaped.Add( '^' );
            RegexHelper.s_needsEscaped.Add( '$' );
            RegexHelper.s_needsEscaped.Add( '?' );
            RegexHelper.s_needsEscaped.Add( '+' );
            RegexHelper.s_needsEscaped.Add( '*' );
            RegexHelper.s_needsEscaped.Add( '#' );
            RegexHelper.s_needsEscaped.Add( '|' );
            RegexHelper.s_translate = new Dictionary<char, string>();
            RegexHelper.s_translate.Add( '\t', "\\t" );
            RegexHelper.s_translate.Add( '\r', "\\r" );
            RegexHelper.s_translate.Add( '\n', "\\n" );
            RegexHelper.s_translate.Add( '\v', "\\v" );
        }

        public static RegexOptions XmsOpts {
            get {
                return RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;
            }
        }

        public static RegexOptions XmsiOpts {
            get {
                return RegexHelper.XmsOpts | RegexOptions.IgnoreCase;
            }
        }

        public static bool IsValidRegexPattern( string pattern )
        {
            return RegexHelper.IsValidRegexPattern( pattern, RegexHelper.XmsOpts );
        }

        public static bool IsValidRegexPattern( string pattern, RegexOptions opts )
        {
            bool result = true;
            try
            {
                new Regex( pattern, opts );
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static string ConvertWildcardPatternToRegex( string wildcardPattern )
        {
            if( string.IsNullOrEmpty( wildcardPattern ) )
            {
                return string.Empty;
            }
            string[] array = wildcardPattern.Split( new char[]
            {
                '|'
            } );
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            stringBuilder.Append( "^" );
            foreach( string str in array )
            {
                string text = Regex.Escape( str );
                text = text.Replace( "\\[!", "[^" );
                text = text.Replace( "\\[", "[" );
                text = text.Replace( "\\]", "]" );
                text = text.Replace( "\\?", "." );
                text = text.Replace( "\\*", ".*" );
                text = text.Replace( "\\#", "\\d" );
                if( flag )
                {
                    flag = false;
                }
                else
                {
                    stringBuilder.Append( "|" );
                }
                stringBuilder.Append( "(" );
                stringBuilder.Append( text );
                stringBuilder.Append( ")" );
            }
            stringBuilder.Append( "$" );
            string text2 = stringBuilder.ToString();
            if( !RegexHelper.IsValidRegexPattern( text2 ) )
            {
                throw new ArgumentException( string.Format( "Invalid pattern: {0}", wildcardPattern ) );
            }
            return text2;
        }

        public static string ConvertSqlLikePatternToRegex( string sqlLikePattern )
        {
            string text = "^" + Regex.Escape( sqlLikePattern ) + "$";
            text = text.Replace( "\\[_\\]", "[_]" );
            text = text.Replace( "\\[\\[\\]", "\\[" );
            text = text.Replace( "\\[\\^", "[^" );
            text = text.Replace( "\\[", "[" );
            text = text.Replace( "\\]", "]" );
            text = text.Replace( "%", ".*?" );
            text = text.Replace( "_", "." );
            if( !RegexHelper.IsValidRegexPattern( text ) )
            {
                throw new ArgumentException( string.Format( "Invalid pattern: {0}", sqlLikePattern ) );
            }
            return text;
        }

        private static readonly HashSet<char> s_needsEscaped = new HashSet<char>();
        private static readonly Dictionary<char, string> s_translate;
    }
}
