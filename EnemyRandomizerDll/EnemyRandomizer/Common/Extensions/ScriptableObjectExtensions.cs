using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nv
{
    public static class ScriptableObjectExtensions
    {
#if UNITY_EDITOR
        public static T CreateAsset<T>(string assetFolderPath = null, bool useFileExplorer = true)
            where T : ScriptableObject
        {
            if (assetFolderPath == null)
                assetFolderPath = Application.dataPath;

            if (!assetFolderPath.StartsWith("Assets/"))
                throw new System.Exception("path name must start with Assets/");

            return editor.ScriptableObjectInstancer.CreateScriptableObject<T>(assetFolderPath, useFileExplorer, allowCreateAssetInPlayMode: false);
        }
#endif
    }
}