using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace nv.editor
{
    [CustomPropertyDrawer(typeof(Tags))]
    public class TagsDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;
            
            EditorGUILayout.PropertyField(property.FindPropertyRelative("matching"), new GUIContent("Matching Style"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("_tags"), new GUIContent("List"), true);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif

namespace nv
{
    [System.Serializable]
    /// <summary>
    /// Convenient methods to manage a collection of identifier strings.
    /// </summary>
    public struct Tags : IEquatable<Tags>, IEnumerable<string>, IEqualityComparer
    {
        public enum Matching
        {
            Any
            , All
        };
        public Matching matching;

        [SerializeField]
        string[] _tags;
        public string[] tags
        {
            get
            {
                return _tags;
            }
        }

        public Tags(params string[] tags)
        {
            this.matching = Matching.Any;

            if(tags == null)
                _tags = new string[0];
            else
                _tags = tags;
        }

        public Tags(Matching matching, params string[] tags)
        {
            this.matching = matching;

            if(tags == null)
                _tags = new string[0];
            else
                _tags = tags;
        }

        public static implicit operator string[] (Tags t)
        {
            return t.tags;
        }

        public static implicit operator Tags(string[] tags)
        {
            if(tags == null)
                return new Tags();
            return new Tags(tags);
        }

        public int Count
        {
            get
            {
                return tags == null ? 0 : tags.Length;
            }
        }

        public bool Contains(string s)
        {
            if(tags == null)
            {
                return string.IsNullOrEmpty(s);
            }
            else
            {
                return tags.Contains(s);
            }
        }

        public int IndexOf(string s)
        {
            return tags.ToList().IndexOf(s);
        }

        public override string ToString()
        {
            if(tags == null || tags.Length <= 0)
                return string.Empty;

            string result = "";
            tags.Select(x => { result += x + ", "; return x; });
            return matching + " " + result.TrimEnd(',', ' ');
        }
        
        public bool Equals(string[] tags)
        {
            return Equals(new Tags(tags));
        }

        public bool Equals(Tags other)
        {
            return ToString() == other.ToString();
        }

        public override bool Equals( object other )
        {
            return ToString() == other.ToString();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)tags).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)tags).GetEnumerator();
        }

        public new bool Equals(object x, object y)
        {
            return ((Tags)x).Equals((Tags)y);
        }

        public int GetHashCode(object obj)
        {
            return ((Tags)obj).ToString().GetHashCode();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Tags x, Tags y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Tags x, Tags y)
        {
            return !x.Equals(y);
        }

        public static bool operator ==(Tags x, string[] y)
        {
            return x.Equals(new Tags(y));
        }

        public static bool operator !=(Tags x, string[] y)
        {
            return !x.Equals(new Tags(y));
        }

        public bool Matches(string[] tags)
        {
            return Matches(new Tags(tags));
        }

        public bool Matches(Tags other)
        {
            if(tags.Length <= 0 && other.tags.Length <= 0)
                return true;
            if(tags.Length > 0 && other.tags.Length <= 0)
                return false;
            if(tags.Length <= 0 && other.tags.Length > 0)
                return false;

            if(matching == Tags.Matching.Any)
            {
                foreach(string s in tags)
                {
                    if(other.tags.Contains(s))
                        return true;
                }
            }
            else if(matching == Tags.Matching.All)
            {
                if(ToString() == other.ToString())
                    return true;
            }
            return false;
        }
    }
}