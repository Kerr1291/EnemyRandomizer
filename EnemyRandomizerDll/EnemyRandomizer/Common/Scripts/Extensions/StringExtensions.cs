#define UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine;

namespace nv
{
    public static class StringExtensions
    {
        public static string Colorize( this string str, string colorhex )
        {
            return "<color=#" + colorhex + ">" + "<b>" + str + "</b>" + "</color>";
        }

        public static string Colorize(this string str, Color color)
        {
            return str.Colorize( color.ColorToHex() );
        }        
        
        public static bool DeserializeXMLFromFile<T>( this string path, out T data ) where T : class
        {
            data = null;

            if( !File.Exists( path ) )
            {
                //throw new FileLoadException( "No file found at " + path );
                Dev.LogError($"No file found at {path}");
                return false;
            }

            bool returnResult = true;

            XmlSerializer serializer = new XmlSerializer( typeof( T ) );
            FileStream fstream = null;
            try
            {
                fstream = new FileStream( path, FileMode.Open );
            }
            catch( System.ArgumentException ex1 )
            {
#if UNITY_EDITOR
                Dev.LogError( "Error opening file." );
                Dev.LogError( ex1.GetType() + "/" + ex1.ParamName + "/" + ex1.Message );
#else
                Console.WriteLine( "Error opening file." );
                Console.WriteLine( ex1.GetType() + "/" + ex1.ParamName + "/" + ex1.Message );
#endif

                returnResult = false;
                fstream.Close();
                fstream = null;
            }
            catch( System.Exception ex2 )
            {
#if UNITY_EDITOR
                Dev.LogError( "Error opening file." );
#else
                Console.WriteLine( "Error opening file." );
#endif

                while( ex2 != null )
                {
#if UNITY_EDITOR
                    Dev.LogError( ex2.GetType() + "/" + ex2.Message + "/" );
#else
                    Console.WriteLine( ex2.GetType() + "/" + ex2.Message + "/" );
#endif
                    ex2 = ex2.InnerException;
                }

                returnResult = false;
                fstream.Close();
                fstream = null;
            }


            try
            {
                if( fstream != null )
                    data = serializer.Deserialize( fstream ) as T;
            }
            catch( System.Xml.XmlException ex3 )
            {
#if UNITY_EDITOR
                Dev.LogError( "Error Deserializing file." );
                Dev.LogError( ex3.Message + "/" + ex3.LineNumber + "/" + ex3.LinePosition );
#else
                Console.WriteLine( "Error Deserializing file." );
                Console.WriteLine( ex3.Message + "/" + ex3.LineNumber + "/" + ex3.LinePosition );
#endif

                returnResult = false;
            }
            catch( System.ArgumentException ex4 )
            {
#if UNITY_EDITOR
                Dev.LogError( "Error Deserializing file." );
                Dev.LogError( ex4.GetType() + "/" + ex4.ParamName + "/" + ex4.Message );
#else
                Console.WriteLine( "Error Deserializing file." );
                Console.WriteLine( ex4.GetType() + "/" + ex4.ParamName + "/" + ex4.Message );
#endif

                returnResult = false;
            }
            catch( System.Exception ex5 )
            {
#if UNITY_EDITOR
                Dev.LogError( "Error Deserializing file." );
#else
                Console.WriteLine( "Error Deserializing file." );
#endif

                while( ex5 != null )
                {
#if UNITY_EDITOR
                    Dev.LogError( ex5.GetType() + "/" + ex5.Message + "/" );
#else
                    Console.WriteLine( ex5.GetType() + "/" + ex5.Message + "/" );
#endif
                    ex5 = ex5.InnerException;
                }

                returnResult = false;
            }
            finally
            {
                if( fstream != null )
                    fstream.Close();
            }

            return returnResult;
        }


        public static bool SerializeXMLToFile<T>(this string path, T data) where T : class
        {
            bool result = false;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            FileStream fstream = null;
            try
            {
                fstream = new FileStream(path, FileMode.Create);
                serializer.Serialize(fstream, data);
                result = true;
            }
            catch (System.Exception e)
            {
                Dev.LogError(e.Message);
                //System.Windows.Forms.MessageBox.Show("Error creating/saving file "+ e.Message);
            }
            finally
            {
                fstream.Close();
            }
            return result;
        }


        public static List<int> AllIndexesOf( this string str, string value )
        {
            if( String.IsNullOrEmpty( value ) )
                throw new ArgumentException( "the string to find may not be empty", "value" );
            List<int> indexes = new List<int>();
            for( int index = 0; ; index += value.Length )
            {
                index = str.IndexOf( value, index );
                if( index == -1 )
                    return indexes;
                indexes.Add( index );
            }
        }

        public static string Remove( this string str, string toRemove )
        {
            int length = str.Length;
            if( str.Contains( toRemove ) )
            {
                int toRemoveLength = toRemove.Length;
                int toRemoveIndex = str.IndexOf( toRemove );
                return str.Substring( 0, toRemoveIndex ) +
                       str.Substring( toRemoveIndex + toRemoveLength );
            }
            else
                return str;
        }

        public static string Replace( this string str, string toReplace, string replacement )
        {
            int length = str.Length;
            if( str.Contains( toReplace ) )
            {
                int toRemoveLength = toReplace.Length;
                int toRemoveIndex = str.IndexOf( toReplace );
                return str.Substring( 0, toRemoveIndex ) + replacement +
                       str.Substring( toRemoveIndex + toRemoveLength );
            }
            else
                return str;
        }

        public static string TrimEnd( this string str, string toRemove )
        { 
                int length = str.Length;
                int toRemoveLength = toRemove.Length;
                if( str.Contains( toRemove ) )
                    return str.Substring( 0, length - toRemoveLength );
                else
                    return str;
        }

        public static string TrimStart( this string str, string toRemove )
        {
            int toRemoveLength = toRemove.Length;
            if( str.Contains( toRemove ) )
                return str.Substring( toRemoveLength );
            else
                return str;
        }

        public static string Substring( this string str, char startChar )
        {
            return str.Substring( str.IndexOf( startChar ) );
        }

        public static string Substring( this string str, char startChar, char endCharacter )
        {
            return str.Substring( str.IndexOf( startChar ), str.IndexOf( endCharacter ) );
        }

        public static string Substring( this string str, int startIndex, char endCharacter )
        {
            return str.Substring( startIndex, str.IndexOf( endCharacter ) );
        }

        public static string Slice( this string s, int startIndex, int? endIndex = null )
        {
            if( s == null )
            {
                return null;
            }
            int num = startIndex;
            int num2 = ( endIndex == null ) ? s.Length : endIndex.Value;
            if( num < 0 )
            {
                num = s.Length + startIndex;
            }
            if( num2 < 0 )
            {
                num2 = s.Length + num2;
            }
            if( num < 0 )
            {
                num = 0;
            }
            if( num > s.Length )
            {
                num = s.Length;
            }
            if( num2 < 0 )
            {
                num2 = 0;
            }
            if( num2 > s.Length )
            {
                num2 = s.Length;
            }
            int num3 = num2 - num;
            if( num3 < 0 )
            {
                num3 = 0;
            }
            if( num + num3 > s.Length )
            {
                return s.Substring( num );
            }
            return s.Substring( num, num3 );
        }

        public static string SubstringBefore( this string s, string innerString )
        {
            if( s == null )
            {
                return null;
            }
            int num = s.IndexOf( innerString );
            return ( num >= 0 ) ? s.Substring( 0, num ) : null;
        }

        public static string SubstringAfter( this string s, string innerString )
        {
            if( s == null )
            {
                return null;
            }
            int num = s.IndexOf( innerString );
            return ( num >= 0 ) ? s.Substring( num + 1 ) : null;
        }

        public static string Repeat( this string s, int repetitions )
        {
            if( s == null )
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for( int i = 0; i < repetitions; i++ )
            {
                stringBuilder.Append( s );
            }
            return stringBuilder.ToString();
        }

        public static string FormatWith( this string s, params object[] args )
        {
            if( s == null )
            {
                return null;
            }
            return string.Format( s, args );
        }

        public static string FormatWith( this string s, IFormatProvider provider, params object[] args )
        {
            if( s == null )
            {
                return null;
            }
            return string.Format( provider, s, args );
        }

        public static string ToCSharpEscapedString( this string s )
        {
            string text = s.Replace( "\\", "\\\\" );
            text = text.Replace( "\"", "\\\"" );
            text = text.Replace( "\0", "\\0" );
            text = text.Replace( "\a", "\\a" );
            text = text.Replace( "\b", "\\b" );
            text = text.Replace( "\f", "\\f" );
            text = text.Replace( "\n", "\\n" );
            text = text.Replace( "\r", "\\r" );
            text = text.Replace( "\t", "\\t" );
            return text.Replace( "\v", "\\v" );
        }

        public static string Reverse( this string input )
        {
            char[] array = input.ToCharArray();
            Array.Reverse( array );
            return new string( array );
        }

        public static bool IsNullOrWhitespace( this string input )
        {
            return StringExtensions.IsNullOrWhiteSpace( input );
        }

        public static bool IsNullOrWhiteSpace( string value )
        {
            if( value != null )
            {
                for( int i = 0; i < value.Length; i++ )
                {
                    if( !char.IsWhiteSpace( value[ i ] ) )
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string[] SplitLines( this string that )
        {
            return that.Replace( "\r\n", "\n" ).Replace( '\r', '\n' ).Split( new char[]
            {
                '\n'
            } );
        }

        public static string Translate( this string str, string translationFromString, string translationToString )
        {
            if( string.IsNullOrEmpty( str ) )
            {
                return str;
            }
            if( translationFromString == null )
            {
                throw new ArgumentNullException( "translationFromString" );
            }
            if( translationToString == null )
            {
                throw new ArgumentNullException( "translationToString" );
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach( char value in str )
            {
                int num = translationFromString.IndexOf( value );
                if( num < 0 )
                {
                    stringBuilder.Append( value );
                }
                else if( num < translationToString.Length )
                {
                    stringBuilder.Append( translationToString[ num ] );
                }
            }
            return stringBuilder.ToString();
        }


        public static int IndexOfAny( this string baseString, string[] anyOf )
        {
            int num = -1;
            foreach( string value in anyOf )
            {
                num = baseString.IndexOf( value );
                if( num >= 0 )
                {
                    return num;
                }
            }
            return num;
        }

        public static string Replace( this string s, string oldValue, string newValue, StringComparison comparisonType )
        {
            if( s == null )
            {
                return null;
            }
            if( string.IsNullOrEmpty( oldValue ) || newValue == null )
            {
                return s;
            }
            int i = s.IndexOf( oldValue, comparisonType );
            if( i < 0 )
            {
                return s;
            }
            StringBuilder stringBuilder = new StringBuilder();
            int length = oldValue.Length;
            int num = 0;
            while( i >= 0 )
            {
                stringBuilder.Append( s, num, i - num );
                stringBuilder.Append( newValue );
                num = i + length;
                i = s.IndexOf( oldValue, num, comparisonType );
            }
            stringBuilder.Append( s, num, s.Length - num );
            return stringBuilder.ToString();
        }

        public static string Left( this string s, int length )
        {
            if( s == null )
            {
                return null;
            }
            if( length > s.Length )
            {
                length = s.Length;
            }
            else if( length < 0 )
            {
                length = 0;
            }
            return s.Substring( 0, length );
        }

        public static string Right( this string s, int length )
        {
            if( s == null )
            {
                return null;
            }
            if( length > s.Length )
            {
                length = s.Length;
            }
            else if( length < 0 )
            {
                length = 0;
            }
            return s.Substring( s.Length - length, length );
        }

        public static string Substring( this string s, int startIndex, int length, bool neverFail )
        {
            if( !neverFail )
            {
                return s.Substring( startIndex, length );
            }
            startIndex = startIndex.ConstrainBetween( 0, s.Length );
            if( length < 0 )
            {
                length = 0;
            }
            if( startIndex + length > s.Length )
            {
                length = s.Length - startIndex;
            }
            return s.Substring( startIndex, length );
        }

        public static string Truncate( this string s, int maxLength, string truncationSuffix = "..." )
        {
            if( s == null )
            {
                return null;
            }
            if( maxLength < 0 )
            {
                return string.Empty;
            }
            if( s.Length <= maxLength )
            {
                return s;
            }
            if( maxLength < truncationSuffix.Length )
            {
                return s.Left( maxLength );
            }
            return s.Left( maxLength - truncationSuffix.Length ) + truncationSuffix;
        }

        public static bool Contains( this string s, string value, StringComparison comparisonType )
        {
            return s.IndexOf( value, comparisonType ) >= 0;
        }

        public static bool Contains( this string s, char[] values, StringComparison comparisonType )
        {
            return s.Contains( ( from c in values
                                 select new string( c, 1 ) ).ToArray<string>(), comparisonType );
        }

        public static bool Contains( this string s, string[] values )
        {
            return s.Contains( values, StringComparison.Ordinal );
        }

        public static bool Contains( this string s, string[] values, StringComparison comparisonType )
        {
            if( s == null )
            {
                return false;
            }
            foreach( string text in values )
            {
                if( text != null )
                {
                    bool flag = s.IndexOf( text, comparisonType ) >= 0;
                    if( flag )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string NormalizeWhiteSpace( this string s )
        {
            if( s == null )
            {
                return null;
            }
            if( StringExtensions.s_regexNormalizeSpace == null )
            {
                StringExtensions.s_regexNormalizeSpace = new Regex( "\\s+", RegexHelper.XmsOpts | RegexOptions.Compiled );
            }
            return StringExtensions.s_regexNormalizeSpace.Replace( s.Trim(), " " );
        }

        public static string NormalizeNewlines( this string s )
        {
            if( s == null )
            {
                return null;
            }
            return s.Replace( "\r\n", "\n" ).Replace( "\r", "\n" ).Replace( "\n", Environment.NewLine );
        }

        public static string TrimEnd( this string s )
        {
            if( s == null )
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder( s );
            while( stringBuilder.Length > 0 && char.IsWhiteSpace( stringBuilder[ stringBuilder.Length - 1 ] ) )
            {
                stringBuilder.Remove( stringBuilder.Length - 1, 1 );
            }
            return stringBuilder.ToString();
        }

        public static string TrimStart( this string s )
        {
            if( s == null )
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder( s );
            while( stringBuilder.Length > 0 && char.IsWhiteSpace( stringBuilder[ 0 ] ) )
            {
                stringBuilder.Remove( 0, 1 );
            }
            return stringBuilder.ToString();
        }

        public static bool IsWildcardMatch( this string s, string wildcardPattern )
        {
            if( string.IsNullOrEmpty( wildcardPattern ) )
            {
                return string.IsNullOrEmpty( s );
            }
            string pattern = RegexHelper.ConvertWildcardPatternToRegex( wildcardPattern );
            return Regex.IsMatch( s, pattern, RegexHelper.XmsiOpts );
        }

        public static string[] Chunk( this string s, int chunkSize )
        {
            List<string> list = new List<string>();
            int num = 0;
            while( s != null && num < s.Length )
            {
                int num2 = ( chunkSize >= s.Length - num ) ? ( s.Length - num ) : chunkSize;
                list.Add( s.Substring( num, num2 ) );
                num += num2;
            }
            return list.ToArray();
        }

        public static string MakeTitleCase( this string value )
        {
            if( value == null )
            {
                throw new ArgumentNullException();
            }
            TextInfo textInfo = new CultureInfo( "en-US", false ).TextInfo;
            string input = textInfo.ToTitleCase( value.ToLower() );
            MatchEvaluator evaluator = ( Match m ) => m.Groups[ 1 ].Value + m.Groups[ 2 ].Value.ToLower();
            return Regex.Replace( input, "([0-9])([A-Z])", evaluator );
        }

        private static Regex s_regexNormalizeSpace;
    }
}