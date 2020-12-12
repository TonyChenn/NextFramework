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
    /// UI脚本目录
    /// </summary>
    public static string UIScriptsFolder = RootPath + "/Scripts/UIScripts";
    /// <summary>
    /// xLua脚本目录
    /// </summary>
    public static string XLuaScriptFolder = RootPath + "/Scripts/xLua";
    /// <summary>
    /// UI预设目录
    /// </summary>
    public static string UIPrefabPath = RootPath + "/UI/UIPrefab";
    /// <summary>
    /// 转表格脚本位置(生成)
    /// </summary>
    public static string TableDefScriptFolder = RootPath + "/Scripts/Table/Define";
    /// <summary>
    /// 转表格脚本位置(自定义)
    /// </summary>
    public static string TableCustomScriptFolder = RootPath + "/Scripts/Table/Custom";
    /// <summary>
    /// 导表ScriptObject位置
    /// </summary>
    public static string TableScriptObjectFolder = RootPath + "/Asset/Table";
    /// <summary>
    /// 框架目录
    /// </summary>
    public static string NextFrameworkPath = RootPath + "/NextFramework";

    /// <summary>
    /// 所有面板配置信息
    /// </summary>
    public static string UIPanelsJsonPath= NextFrameworkPath + "/UIKit/panels.json";

}

