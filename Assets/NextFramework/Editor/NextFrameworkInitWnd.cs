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
            PathConfig.UIScriptsFolder.CreateFolderIfNotExist();
            //UI预设文件夹
            PathConfig.UIPrefabPath.CreateFolderIfNotExist();

            //StreammingAssets文件夹
            Application.streamingAssetsPath.CreateFolderIfNotExist();
            //Resources 文件夹
            string resourcesPath = PathConfig.RootPath + "/Resources";
            resourcesPath.CreateFolderIfNotExist();

            //创建UIType脚本
            string UITypePath = PathConfig.NextFrameworkPath + "/UIKit/UIType.cs";
            FileHelper.WriteUITypeFile(UITypePath, "");

            //创建表格相关文件夹
            PathConfig.TableDefScriptFolder.CreateFolderIfNotExist();
            PathConfig.TableCustomScriptFolder.CreateFolderIfNotExist();
            PathConfig.TableScriptObjectFolder.CreateFolderIfNotExist();

            //TextStyle,ColorStyle
            PathConfig.TextStyleFolder.CreateFolderIfNotExist();
            PathConfig.ColorStyleFolder.CreateFolderIfNotExist();

            AssetDatabase.Refresh();
        }
    }
}

