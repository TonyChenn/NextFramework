/**
#########################
#
# Author:TonyChenn
# Date:2020/6/29 15:14:24
#
#########################
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NextFramework.Util;
using System.Text;

namespace NextFramework
{
    public class NextFrameworkInitWnd : EditorWindow
    {
        static string AppDataPath = "";                     //工程目录
        static string uiScriptsFolderPath = "";             //UI脚本
        static string uiPrefabPath = "";                    //UI预设
        static string TableDefScriptFolder = "";            //转表格脚本位置(生成)
        static string TableCustomScriptFolder = "";         //转表格脚本位置(自定义)


        static string NextFrameworkPath = "/NextFramework";

        static void InitPath()
        {
            AppDataPath = Application.dataPath;
            uiScriptsFolderPath = AppDataPath + "/UI/UIScripts";
            uiPrefabPath = AppDataPath + "/UI/UIPrefab";
            TableDefScriptFolder = AppDataPath + "/Scripts/Table/Define";
            TableCustomScriptFolder = AppDataPath + "/Scripts/Table/Custom";
        }

        [MenuItem("NextFramework/Init")]
        static void StartInit()
        {
            InitPath();

            //UI脚本文件夹
            if (!Directory.Exists(uiScriptsFolderPath))
                Directory.CreateDirectory(uiScriptsFolderPath);
            //UI预设文件夹
            if (!Directory.Exists(uiPrefabPath))
                Directory.CreateDirectory(uiPrefabPath);

            //StreammingAssets文件夹
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            //Resources 文件夹
            string resourcesPath = AppDataPath + "/Resources";
            if (!Directory.Exists(resourcesPath))
                Directory.CreateDirectory(resourcesPath);

            //创建UIType脚本
            string UIKitPath = Application.dataPath + NextFrameworkPath + "/UIKit";
            string content = TemplateString.Str_UIType.Replace("{ENUM}", "");
            FileHelper.WriteFile(UIKitPath + "/UIType.cs", content);

            //创建表格相关文件夹
            if (!Directory.Exists(TableDefScriptFolder))
                Directory.CreateDirectory(TableDefScriptFolder);
            if (!Directory.Exists(TableCustomScriptFolder))
                Directory.CreateDirectory(TableCustomScriptFolder);

            AssetDatabase.Refresh();
        }
    }
}

