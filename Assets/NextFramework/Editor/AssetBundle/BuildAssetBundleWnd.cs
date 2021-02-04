using UnityEngine;
using UnityEditor;
using NextFramework;

public class BuildAssetBundleWnd : EditorWindow
{
    [MenuItem("NextFramework/AssetBundle/SetTextureABName")]
    public static void SetTextureABName()
    {
        SetTextureABNameEditor.SetTextureABName();
    }

    [MenuItem("NextFramework/AssetBundle/CreateAtlas")]
    public static void CreateAtlas()
    {
        CreateAtlasEditor.CreateAtlas();
    }

    [MenuItem("NextFramework/AssetBundle/Build Atlas")]
    public static void BuildAllAtlas()
    {
        ABTool.Singlton.BuildAssetBundleTotal(true, BuildABType.Atlas);
    }

    [MenuItem("NextFramework/AssetBundle/Build UIPrefab")]
    public static void BuildAllUI()
    {
        ABTool.Singlton.BuildAssetBundleTotal(true, BuildABType.UIPrefab);
    }
    [MenuItem("NextFramework/AssetBundle/Build Table")]
    public static void BuildTable()
    {
        ConvertSettingWnd.SettingConvertExcel();
    }

    [MenuItem("NextFramework/AssetBundle/一键打包")]
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
        GUILayout.Label("配置目录：", GUILayout.Width(150));
        if (GUILayout.Button("...", GUILayout.Width(30)))
            EditorPrefsHelper.ExcelFolder = EditorUtility.SaveFolderPanel("选择 excel 配置目录",
                EditorPrefsHelper.ExcelFolder, EditorPrefsHelper.ExcelFolder);
        EditorPrefsHelper.ExcelFolder = GUILayout.TextField(EditorPrefsHelper.ExcelFolder);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        // AB 包目录
        EditorPrefsHelper.DontUseSteammingAssetFolder =
            EditorGUILayout.BeginToggleGroup("自定义输出AB包文件夹", EditorPrefsHelper.DontUseSteammingAssetFolder);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("...", GUILayout.Width(30)))
            EditorPrefsHelper.ABPackFolder = EditorUtility.SaveFolderPanel("选择 Art 的 ab 包目录",
                EditorPrefsHelper.ABPackFolder, EditorPrefsHelper.ABPackFolder);

        EditorPrefsHelper.ABPackFolder = GUILayout.TextField(EditorPrefsHelper.ABPackFolder);
        if (!EditorPrefsHelper.DontUseSteammingAssetFolder)
            EditorPrefsHelper.ABPackFolder = Application.dataPath + "/StreammingAsset";
        GUILayout.EndHorizontal();
        EditorGUILayout.EndToggleGroup();
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