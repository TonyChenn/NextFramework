using UnityEngine;
using UnityEditor;
using NextFramework;

public class BuildAssetBundleWnd : EditorWindow
{
    [MenuItem("Tools/AssetBundle/SetTextureABName")]
    public static void SetTextureABName()
    {
        SetTextureABNameEditor.SetTextureABName();
    }

    [MenuItem("Tools/AssetBundle/GenerateAtlas")]
    public static void GenerateAtlas()
    {
        CreateAtlasEditor.CreateAtlas();
    }

    [MenuItem("Tools/AssetBundle/Build Atlas")]
    public static void BuildAllAtlas()
    {
        ABTool.Singlton.BuildAssetBundleTotal(true, BuildABType.Atlas);
    }

    [MenuItem("Tools/AssetBundle/Build Table")]
    public static void BuildTable()
    {
        ConvertSettingWnd.SettingConvertExcel();
    }

    [MenuItem("Tools/AssetBundle/一键打包")]
    public static void BuildAll()
    {
        GetWindow<BuildAssetBundleWnd>(false, "一键打包工具", true);
    }

    Vector2 scrollPos;
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        // 配置目录
        //打包平台设置
        EditorPrefsHelper.CurBuildTarget =
            (BuildTarget)EditorGUILayout.EnumPopup("当前打包平台：", EditorPrefsHelper.CurBuildTarget);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("AB包文件夹：", GUILayout.Width(150));
        GUILayout.TextField($"{PathConfig.AssetBundleFolder}/{EditorPrefsHelper.CurBuildTarget.ToString()}");
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        // AB 包目录
        /**
        EditorPrefsHelper.DontUseSteammingAssetFolder = EditorGUILayout.BeginToggleGroup("输出AB包文件夹", EditorPrefsHelper.DontUseSteammingAssetFolder);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("...", GUILayout.Width(30)))
            EditorPrefsHelper.ABPackFolder = EditorUtility.SaveFolderPanel("选择 Art 的 ab 包目录",
                EditorPrefsHelper.ABPackFolder, EditorPrefsHelper.ABPackFolder);

        EditorPrefsHelper.ABPackFolder = GUILayout.TextField(EditorPrefsHelper.ABPackFolder);
        if (!EditorPrefsHelper.DontUseSteammingAssetFolder)
            EditorPrefsHelper.ABPackFolder = Application.dataPath + "/StreammingAsset";
        GUILayout.EndHorizontal();
        EditorGUILayout.EndToggleGroup();
        */
        GUILayout.Space(10);

        // 打表
        EditorPrefsHelper.BuildConvertTable =
            EditorGUILayout.BeginToggleGroup("导表", EditorPrefsHelper.BuildConvertTable);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(10);

        // 生成xLua
        EditorPrefsHelper.BuildXLua =
            EditorGUILayout.BeginToggleGroup("生成xLua", EditorPrefsHelper.BuildXLua);
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(10);

        GUILayout.EndScrollView();
        if (GUILayout.Button("执行", GUILayout.Height(50)))
        {
            ABTool.Singlton.BuildAssetBundleTotal(true, BuildABType.All);
        }
    }
}