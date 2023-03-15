using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace EnemyRandomizerMod
{
    public static class ComponentExtensions
    {
        public static void PrintComponentType( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c == null )
                return;

            if( file != null )
            {
                file.WriteLine( componentHeader + @" \--Component: " + c.GetType().Name );
            }
            else
            {
                Dev.Log( componentHeader + @" \--Component: " + c.GetType().Name );
            }
        }

        public static void PrintTransform( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as Transform != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--GameObject layer: " + ( c as Transform ).gameObject.layer );
                    file.WriteLine( componentHeader + @" \--GameObject tag: " + ( c as Transform ).gameObject.tag );
                    file.WriteLine( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                    file.WriteLine( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                    file.WriteLine( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
                }
                else
                {
                    Debug.Log( componentHeader + @" \--GameObject layer: " + ( c as Transform ).gameObject.layer );
                    Debug.Log( componentHeader + @" \--GameObject tag: " + ( c as Transform ).gameObject.tag );
                    Debug.Log( componentHeader + @" \--Transform Position: " + ( c as Transform ).position );
                    Debug.Log( componentHeader + @" \--Transform Rotation: " + ( c as Transform ).rotation.eulerAngles );
                    Debug.Log( componentHeader + @" \--Transform LocalScale: " + ( c as Transform ).localScale );
                }
            }
        }

        public static void PrintBoxCollider2D( this Component c, string componentHeader = "", System.IO.StreamWriter file = null )
        {
            if( c as BoxCollider2D != null )
            {
                if( file != null )
                {
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Size: " + ( c as BoxCollider2D ).size );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Offset: " + ( c as BoxCollider2D ).offset );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Bounds-Min: " + ( c as BoxCollider2D ).bounds.min );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D Bounds-Max: " + ( c as BoxCollider2D ).bounds.max );
                    file.WriteLine( componentHeader + @" \--BoxCollider2D isTrigger: " + ( c as BoxCollider2D ).isTrigger );
                }
                else
                {
                    Dev.Log( componentHeader + @" \--BoxCollider2D Size: " + ( c as BoxCollider2D ).size );
                    Dev.Log( componentHeader + @" \--BoxCollider2D Offset: " + ( c as BoxCollider2D ).offset );
                    Dev.Log( componentHeader + @" \--BoxCollider2D Bounds-Min: " + ( c as BoxCollider2D ).bounds.min );
                    Dev.Log( componentHeader + @" \--BoxCollider2D Bounds-Max: " + ( c as BoxCollider2D ).bounds.max );
                    Dev.Log( componentHeader + @" \--BoxCollider2D isTrigger: " + ( c as BoxCollider2D ).isTrigger );
                }
            }
        }        

        public static void PrintComponentWithReflection(this Component c, string componentHeader = "", System.IO.StreamWriter file = null)
        {
            Type cType = c.GetType();
            var bflags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var mflags = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;

            var members = cType.GetMembers(bflags);

            foreach(var m in members)
            {
                string label = m.Name;
                string data = "Not a field or property!";

                if(m is FieldInfo)
                {
                    try
                    {
                        object fo = (m as FieldInfo).GetValue(c);
                        data = fo == null ? "null" : fo.ToString();
                    }
                    catch(Exception e)
                    {
                        Dev.Log("Failed to get field value from member field " + label);
                    }
                }
                else if(m is PropertyInfo)
                {
                    try
                    {
                        object po = (m as PropertyInfo).GetValue(c, null);
                        data = po == null ? "null" : po.ToString();
                    }
                    catch (Exception e)
                    {
                        Dev.Log("Failed to get property value from member property " + label);
                    }
                }

                Print(componentHeader, label, data, file);
            }
        }

        private static void Print(string header, string label, string data, System.IO.StreamWriter file = null)
        {
            if(file != null)
            {
                file.WriteLine(header + @" \--" + label + ": " + data);
            }
            else
            {
                Dev.Log(header + @" \--" + label + ": " + data);
            }
        }
    }
}
