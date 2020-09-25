using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SettingWnd : EditorWindow
{
    private void OnGUI()
    {
        // 配置目录
        GUILayout.BeginHorizontal();
        GUILayout.Label("配置目录：", GUILayout.Width(60));
        if (GUILayout.Button("...", GUILayout.Width(30)))
            EditorPrefsHelper.ExcelFolder = EditorUtility.SaveFolderPanel("选择 excel 配置目录", EditorPrefsHelper.ExcelFolder, EditorPrefsHelper.ExcelFolder);
        EditorPrefsHelper.ExcelFolder = GUILayout.TextField(EditorPrefsHelper.ExcelFolder);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // AB 包目录
        GUILayout.Label("AB 包文件夹:(默认:StreammingAsset)", GUILayout.Width(240));

        EditorPrefsHelper.DontUseSteammingAssetFolder = EditorGUILayout.BeginToggleGroup("不使用 StreamingAsset 文件夹", EditorPrefsHelper.DontUseSteammingAssetFolder);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("...", GUILayout.Width(30)))
            EditorPrefsHelper.ABPackFolder = EditorUtility.SaveFolderPanel("选择 Art 的 ab 包目录", EditorPrefsHelper.ABPackFolder, EditorPrefsHelper.ABPackFolder);

        EditorPrefsHelper.ABPackFolder = GUILayout.TextField(EditorPrefsHelper.ABPackFolder);
        if (!EditorPrefsHelper.DontUseSteammingAssetFolder)
            EditorPrefsHelper.ABPackFolder = Application.dataPath + "/StreammingAsset";
        GUILayout.EndHorizontal();
        EditorGUILayout.EndToggleGroup();

        GUILayout.Space(10);

        //打包平台设置
        EditorPrefsHelper.CurBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("打包平台：", EditorPrefsHelper.CurBuildTarget);
    }

    [MenuItem("NextFramework/Setting &s")]
    static void ShowSettingWnd()
    {
        var wnd = GetWindow<SettingWnd>(false, "设置", true);
    }
}
