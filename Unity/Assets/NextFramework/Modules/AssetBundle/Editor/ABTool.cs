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

        if (!FolderHelper.ExistFolder(PathConfig.ExcelFolder))
        {
            errMsg = "配置表路径不存在";
            return false;
        }

        if (EditorPrefsHelper.CurBuildTarget == BuildTarget.NoTarget)
        {
            errMsg = "请先选择打包平台";
            return false;
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
        //当前工程Build平台
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;

        //开始打包
        switch (buildABType)
        {
            case BuildABType.None:
                break;
            case BuildABType.Atlas:
                BuildAB("ui/uiatlas", options, EditorPrefsHelper.CurBuildTarget);
                break;
            case BuildABType.UIPrefab:

                break;
            case BuildABType.Table:
                BuildAB("table", options, EditorPrefsHelper.CurBuildTarget);
                break;
            case BuildABType.All:

                BuildAssetBundleTotal(false, BuildABType.Atlas);
                BuildAssetBundleTotal(false, BuildABType.UIPrefab);
                BuildAssetBundleTotal(false, BuildABType.Table);
                break;
            default:
                break;
        }



        //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, options, activeTarget);
    }


    #region AssetBundle名称相关
    /// <summary>
    /// 删除所有AssetBundle名称
    /// </summary>
    public void ClearAllAssetBundleName()
    {
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0, iMax = abNames.Length; i < iMax; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
    }

    /// <summary>
    /// 删除指定文件夹下的所有AssetBundle名称
    /// </summary>
    public void ClearAllAssetBundleName(string folder)
    {
        //string[] abs = AssetDatabase.GetAllAssetPaths(folder);
    }

    /// <summary>
    /// 设置AssetBundle名称
    /// </summary>
    /// <param name="assetPath">Assets目录下相对路径</param>
    public void SetAssetBundleName(string assetPath, string abName, string abVariant = "u3d")
    {
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        importer.assetBundleName = abName;
        importer.assetBundleVariant = abVariant;
    }

    #endregion

    #region 打AB包
    public void BuildAB(string subFolder, BuildAssetBundleOptions options, BuildTarget activeTarget)
    {
        if (activeTarget == BuildTarget.NoTarget)
            activeTarget = EditorUserBuildSettings.activeBuildTarget;

        string folderPath = PathHelper.CombinePath(PathConfig.RootPath, PathConfig.AssetBundleFolder, activeTarget.ToString());
        folderPath.CreateFolderIfNotExist();

        Log.Info("开始打包");
        BuildPipeline.BuildAssetBundles(folderPath, options, activeTarget);
        Log.Info("打包结束");
        GenVersionInfoFile(folderPath, subFolder);
    }

    void GenVersionInfoFile(string folderPath, string subFolder)
    {
        ClientVersionConfig versionConfig = new ClientVersionConfig();

        // 打包完成后
        foreach (System.IO.FileInfo fileInfo in FileHelper.GetAllFiles(folderPath, "*.u3d"))
        {
            string md5 = MD5Helper.GetFileMD5(fileInfo.FullName);
            string path = string.IsNullOrEmpty(subFolder) ? fileInfo.Name : $"{subFolder}/{fileInfo.Name}";
            versionConfig.FileInfoDict[path] = new FileVersionInfo(fileInfo.Name, md5, fileInfo.Length);
        }

        string content = JsonUtility.ToJson(new Serialization<string, FileVersionInfo>(versionConfig.FileInfoDict));
        string versionPath = string.IsNullOrEmpty(subFolder) ? folderPath : $"{folderPath}/{subFolder}";
        versionPath.CreateFolderIfNotExist();
        FileHelper.WriteFile($"{versionPath}/version.json", content);
    }
    #endregion
}

