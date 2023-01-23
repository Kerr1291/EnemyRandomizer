using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace nv
{
    public static class ReflectionHelpers
    {
        public static BindingFlags PubInst {
            get {
                return BindingFlags.Public | BindingFlags.Instance;
            }
        }

        public static BindingFlags NonPubInst {
            get {
                return BindingFlags.NonPublic | BindingFlags.Instance;
            }
        }

        public static BindingFlags PubStatic {
            get {
                return BindingFlags.Public | BindingFlags.Static;
            }
        }

        public static BindingFlags NonPubStatic {
            get {
                return BindingFlags.NonPublic | BindingFlags.Static;
            }
        }

        public static BindingFlags Inst {
            get {
                return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            }
        }

        public static BindingFlags Static {
            get {
                return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            }
        }

        public static TValueType GetPropertyValue<TSourceType, TValueType>( string memberName )
        {
            return (TValueType)typeof( TSourceType ).GetProperty( memberName, Static ).GetValue( null, null );
        }

        public static TValueType GetPropertyValue<TSourceType, TValueType>( TSourceType source, string memberName, BindingFlags flags )
        {
            return (TValueType)typeof( TSourceType ).GetProperty( memberName, flags ).GetValue( source, null );
        }

        public static TValueType GetPropertyValue<TSourceType, TValueType>( this object source, string memberName )
        {
            return (TValueType)typeof( TSourceType ).GetProperty( memberName, Inst ).GetValue( source, null );
        }

        public static TValueType GetPropertyValue<TValueType>( this object source, string memberName )
        {
            return (TValueType)source.GetType().GetProperty( memberName, Inst ).GetValue( source, null );
        }

        public static TValueType GetFieldValue<TSourceType, TValueType>( string memberName )
        {
            return (TValueType)typeof( TSourceType ).GetField( memberName, Static ).GetValue( null );
        }

        public static TValueType GetFieldValue<TSourceType,TValueType>( TSourceType source, string memberName, BindingFlags flags )
        {
            return (TValueType)typeof( TSourceType ).GetField( memberName, flags ).GetValue( source );
        }

        public static TValueType GetFieldValue<TSourceType, TValueType>( this object source, string memberName )
        {
            return (TValueType)typeof( TSourceType ).GetField( memberName, Inst ).GetValue( source );
        }

        public static TValueType GetFieldValue<TValueType>( this object source, string memberName )
        {
            return (TValueType)source.GetType().GetField( memberName, Inst ).GetValue( source );
        }

        public static TValueType GetField<TSourceType, TValueType>(string memberName)
        {
            return (TValueType)typeof(TSourceType).GetField(memberName, Static).GetValue(null);
        }

        public static TValueType GetField<TSourceType, TValueType>(TSourceType source, string memberName, BindingFlags flags)
        {
            return (TValueType)typeof(TSourceType).GetField(memberName, flags).GetValue(source);
        }

        public static TValueType GetField<TSourceType, TValueType>(this object source, string memberName)
        {
            return (TValueType)typeof(TSourceType).GetField(memberName, Inst).GetValue(source);
        }

        public static TValueType GetField<TValueType>(this object source, string memberName)
        {
            return (TValueType)source.GetType().GetField(memberName, Inst).GetValue(source);
        }

        public static void Invoke<TSourceType>( TSourceType source, string methodName, BindingFlags flags, params object[] parameters)
        {
            typeof( TSourceType ).GetMethod( methodName, flags ).Invoke( source, parameters );
        }

        public static void Invoke<TSourceType>( this object source, string methodName, params object[] parameters )
        {
            typeof( TSourceType ).GetMethod( methodName, Inst ).Invoke( source, parameters );
        }

        public static TReturnType Invoke<TSourceType, TReturnType>( TSourceType source, string methodName, BindingFlags flags, params object[] parameters )
        {
            return (TReturnType)typeof( TSourceType ).GetMethod( methodName, flags ).Invoke( source, parameters );
        }

        public static TReturnType Invoke<TSourceType, TReturnType>( this object source, string methodName, params object[] parameters )
        {
            return (TReturnType)typeof( TSourceType ).GetMethod( methodName, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance ).Invoke( source, parameters );
        }

        public static TReturnType InvokeThis<TReturnType>( this object source, string methodName, params object[] parameters )
        {
            return (TReturnType)source.GetType().GetMethod( methodName, Inst ).Invoke( source, parameters );
        }

        public static void InvokeThis( this object source, string methodName, params object[] parameters )
        {
            source.GetType().GetMethod( methodName, Inst ).Invoke( source, parameters );
        }

        public static void SetPropertyValue<TSourceType>( string memberName, object value )
        {
            typeof( TSourceType ).GetProperty( memberName, Static ).SetValue( null, value, null );
        }

        public static void SetPropertyValue<TSourceType>( TSourceType source, string memberName, object value, BindingFlags flags )
        {
            typeof( TSourceType ).GetProperty( memberName, flags ).SetValue( source, value, null );
        }

        public static void SetPropertyValue<TSourceType>( this object source, string memberName, object value )
        {
            typeof( TSourceType ).GetProperty( memberName, Inst ).SetValue( source, value, null );
        }

        public static void SetPropertyValue( this object source, string memberName, object value )
        {
            source.GetType().GetProperty( memberName, Inst ).SetValue( source, value, null );
        }

        public static void SetFieldValue<TSourceType>( string memberName, object value )
        {
            typeof( TSourceType ).GetField( memberName, Static ).SetValue( null, value );
        }

        public static void SetFieldValue<TSourceType>( TSourceType source, string memberName, object value, BindingFlags flags )
        {
            typeof( TSourceType ).GetField( memberName, flags ).SetValue( source, value );
        }

        public static void SetFieldValue<TSourceType>( this object source, string memberName, object value )
        {
            typeof( TSourceType ).GetField( memberName, Inst ).SetValue( source, value );
        }

        public static void SetFieldValue( this object source, string memberName, object value )
        {
            source.GetType().GetField( memberName, Inst ).SetValue( source, value );
        }
    }
}
