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
        [MenuItem("NextFramework/Init")]
        static void StartInit()
        {
            //UI脚本文件夹
            if (!Directory.Exists(PathConfig.UIScriptsFolder))
                Directory.CreateDirectory(PathConfig.UIScriptsFolder);
            //UI预设文件夹
            if (!Directory.Exists(PathConfig.UIPrefabPath))
                Directory.CreateDirectory(PathConfig.UIPrefabPath);

            //StreammingAssets文件夹
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            //Resources 文件夹
            string resourcesPath = PathConfig.RootPath + "/Resources";
            if (!Directory.Exists(resourcesPath))
                Directory.CreateDirectory(resourcesPath);

            //创建UIType脚本
            string UITypePath = PathConfig.NextFrameworkPath + "/UIKit/UIType.cs";
            FileHelper.WriteUITypeFile(UITypePath, "");

            //创建表格相关文件夹
            if (!Directory.Exists(PathConfig.TableDefScriptFolder))
                Directory.CreateDirectory(PathConfig.TableDefScriptFolder);
            if (!Directory.Exists(PathConfig.TableCustomScriptFolder))
                Directory.CreateDirectory(PathConfig.TableCustomScriptFolder);
            if(!Directory.Exists(PathConfig.TableScriptObjectFolder))
                Directory.CreateDirectory(PathConfig.TableScriptObjectFolder);

            AssetDatabase.Refresh();
        }
    }
}

