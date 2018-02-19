using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


public class CreateAssetBundles
{
    [MenuItem( "Assets/Build AssetBundles" )]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles( "Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64 );
    }

    //[MenuItem( "Assets/Build AssetBundle" )]
    //static void ExportResource()
    //{
    //    string path = "Assets/AssetBundle/mainui.asset";
    //    Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    //    BuildPipeline.BuildAssetBundle( Selection.activeObject, selection, path );//, BuildTarget.StandaloneWindows64 );
    //}
}
#endif