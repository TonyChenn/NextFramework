using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NextFramework;
using System.Text;

public enum BuildABType
{
    None = 0,       //无操作
    All = 1,        //打包所有
    Atlas = 2,      //打包图集
    UIPrefab = 3,   //打包预设
    Table,          //打包配置表
}
public class ABTool : NormalSingleton<ABTool>
{
    private ABTool() { }

    /// <summary>
    /// 检查是否可以打包
    /// </summary>
    /// <param name="errMsg"></param>
    /// <returns></returns>
    bool CheckCanBuild(out string errMsg)
    {
        errMsg = null;
        if (string.IsNullOrEmpty(EditorPrefsHelper.ExcelFolder))
        {
            errMsg = "配置表路径不能为空";
            return false;
        }

        if (!FolderHelper.ExistFolder(EditorPrefsHelper.ExcelFolder))
        {
            errMsg = "配置表路径不存在";
            return false;
        }

        if (EditorPrefsHelper.DontUseSteammingAssetFolder)
        {
            if (string.IsNullOrEmpty(EditorPrefsHelper.ABPackFolder))
            {
                errMsg = "AB包输出路径不能为空";
                return false;
            }
            else
            {
                if (!FolderHelper.ExistFolder(EditorPrefsHelper.ExcelFolder))
                {
                    errMsg = "AB包输出路径不存在，请检查";
                    return false;
                }
            }

        }

        return true;
    }

    /// <summary>
    /// 一键打包
    /// </summary>
    public void BuildAssetBundleTotal(bool showTip, BuildABType buildABType = BuildABType.None)
    {
        IEnumerator etor = DoBuild(showTip, buildABType);
        while (etor.MoveNext())
        {
            Debug.Log(etor.Current);
        }
    }

    IEnumerator DoBuild(bool showTip, BuildABType buildABType = BuildABType.None)
    {
        if (showTip)
        {
            if (!EditorUtility.DisplayDialog("一键打包工具", "确定执行吗？", "执行"))
                yield break;
        }

        string errMsg;
        if (!CheckCanBuild(out errMsg))
        {
            if (showTip)
                EditorUtility.DisplayDialog("错误", errMsg, "好的");
            yield break;
        }

        //开始打包
        switch (buildABType)
        {
            case BuildABType.None:
                break;
            case BuildABType.Atlas:
                break;
            case BuildABType.UIPrefab:
                break;
            case BuildABType.Table:
                break;
            case BuildABType.All:
                //xLua
                CSObjectWrapEditor.Generator.GenAll();

                BuildAssetBundleTotal(false, BuildABType.Atlas);
                BuildAssetBundleTotal(false, BuildABType.UIPrefab);
                BuildAssetBundleTotal(false, BuildABType.Table);
                break;
            default:
                break;
        }

        //当前工程Build平台
        BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, options, activeTarget);
    }


    /// <summary>
    /// 设置AssetBundle名称
    /// </summary>
    public void SetAssetBundleName(string assetFolderPath, string assetName,string abName,string abVariant="ab")
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(assetFolderPath.TrimEnd('/'));
        builder.Append("/");
        builder.Append(assetName.TrimStart('/'));

        SetAssetBundleName(builder.ToString(), abName, abVariant);
    }
    /// <summary>
    /// 设置AssetBundle名称
    /// </summary>
    public void SetAssetBundleName(string assetPath,string abName, string abVariant = "ab")
    {
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        importer.assetBundleName = abName;
        importer.assetBundleVariant = abVariant;
    }
}

