using UnityEditor;

public class BuildAllUI
{
    static string UIOutputPath => PathConfig.RootPath + "/StreamingAssets/UI/";

    [MenuItem("Tools/BuildAllUI")]
    static void BuildUI()
    {
        string[] fontFolders = new string[] { "Assets/UI/Font" };
        string[] fontsGUID = AssetDatabase.FindAssets("t:Font", fontFolders);

        string[] prefabFolders = FolderHelper.GetDirectories("Assets/UI/UIPrefab/");

        //if (activeTarget == BuildTarget.NoTarget)
        BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;

        //字体
        for (int i = 0; i < fontsGUID.Length; i++)
        {
            string fontPath = AssetDatabase.GUIDToAssetPath(fontsGUID[i]);
            int startindex = fontPath.LastIndexOf('/') + 1;
            int length = fontPath.LastIndexOf('.') - startindex;
            string bundleName = fontPath.Substring(startindex, length) + ".u3d";

            BuildPipeline.BuildAssetBundles(UIOutputPath + bundleName, BuildAssetBundleOptions.None, activeTarget);
        }

        //遍历所有ui预设的子目录，逐个打包
    }
}
