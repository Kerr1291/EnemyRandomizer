#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEditor;
using UnityEngine;

namespace nv.editor
{
    public static class EditorExtensions
    {
        public static T EditorGUILayoutPopup<T>(this Editor editor, List<T> data, string label, ref int selectedItem, List<string> choices = null, bool useHorizontal = true)
        {
            return data.EditorGUILayoutPopup<T>(label,ref selectedItem, choices, useHorizontal);
        }

        public static T EditorGUILayoutPopup<T>(this Editor editor, List<T> data, string label, T selectedItem = default(T), List<string> choices = null, bool useHorizontal = true)
        {
            return data.EditorGUILayoutPopup<T>(label, selectedItem, choices, useHorizontal);
        }

        public static T EditorGUILayoutPopup<T>(this List<T> data, string label, ref int selectedItem, List<string> choices = null, bool useHorizontal = true)
        {
            T currentItem = default(T);

            if(selectedItem >= 0)
                currentItem = data[selectedItem];

            var item = EditorGUILayoutPopup<T>( data, label, currentItem, choices, useHorizontal);
            selectedItem = data.IndexOf(item);
            return item;
        }

        public static T EditorGUILayoutPopup<T>(this List<T> data, string label, T selectedItem = default(T), List<string> choices = null, bool useHorizontal = true, int? width = null)
        {
            int currentIndex = 0;

            currentIndex = data.IndexOf(selectedItem);

            if(useHorizontal)
                EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.MaxWidth(120));

            int c = 0;
            string[] dataAsStrings = null;
            if(choices == null)
                data.Select(x => c++ + ": " + x.ToString()).ToArray();
            else
                dataAsStrings = choices.ToArray();

            int maxString = 0;
            for(int i = 0; i < dataAsStrings.Length; ++i)
            {
                maxString = Mathf.Max(dataAsStrings[i].Length, maxString);
            }

            int customWidth = 10 * maxString;
            if(width != null)
                customWidth = width.Value;

            width = Mathf.Max(60, customWidth);

            int prevIndex = currentIndex;
            currentIndex = EditorGUILayout.Popup(currentIndex, dataAsStrings, GUILayout.Width(width.Value));

            if(prevIndex != currentIndex)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            if(useHorizontal)
                EditorGUILayout.EndHorizontal();

            if(data.Count >= 0 && currentIndex >= 0)
            {
                T selected = data[currentIndex];
                return selected;
            }

            return default(T);
        }
    }
}
#endif