using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConfig
{
    /// <summary>
    /// 工程根目录
    /// </summary>
    public static string RootPath = Application.dataPath;

    /// <summary>
    /// 表格路径
    /// </summary>
    public const string ExcelFolder = "../Excel";

    /// <summary>
    /// AB包库路径，{0}为平台
    /// </summary>
    public const string AssetBundleFolder = "../../AssetBundleDatabase";

    /// <summary>
    /// UI脚本目录
    /// </summary>
    public static string UIScriptsFolder = RootPath + "/Scripts/UIScripts";

    /// <summary>
    /// xLua脚本目录
    /// </summary>
    public static string XLuaScriptFolder = RootPath + "/Scripts/xLua";

    /// <summary>
    /// 转表格脚本位置(生成)
    /// </summary>
    public static string TableDefScriptFolder = RootPath + "/Scripts/Table/Define";

    /// <summary>
    /// 转表格脚本位置(自定义)
    /// </summary>
    public static string TableCustomScriptFolder = RootPath + "/Scripts/Table/Custom";

    /// <summary>
    /// 框架目录
    /// </summary>
    public static string NextFrameworkPath = RootPath + "/NextFramework";

    /// <summary>
    /// 所有面板配置信息
    /// </summary>
    public static string UIPanelsJsonPath = NextFrameworkPath + "/UIManger/panels.json";


    #region 需要打AB包

    /// <summary>
    /// 导表ScriptObject位置(相对路径)
    /// </summary>
    public static string TableScriptObjectFolder = RootPath + "/Asset/Table";

    /// <summary>
    /// TextStyle, ColorStyle(相对路径)
    /// </summary>
    public static string TextStyleFolder = RootPath + "/Asset/TextStyle";

    public static string ColorStyleFolder = RootPath + "/Asset/ColorStyle";

    /// <summary>
    /// UI预设目录
    /// </summary>
    public static string UIPrefabPath = RootPath + "/UI/UIPrefab";

    /// <summary>
    /// UITexture文件夹
    /// </summary>
    public static string UITextureFolder = "UI/UITexture";
    /// <summary>
    /// UIAtlas文件夹
    /// </summary>
    public static string UIAtlasFolder = RootPath + "/UI/UIAtlas";
    #endregion
}