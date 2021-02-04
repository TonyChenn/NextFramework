using NextFramework.UIKit;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NextFramework
{
    public class UIKitEditor
    {
        [MenuItem("Assets/生成配置", true)]
        [MenuItem("NextFramework/UIKit/生成配置", true)]
        static bool CanGenConfig()
        {
            GameObject obj = Selection.activeObject as GameObject;
            return obj != null;
        }

        [MenuItem("Assets/生成配置",false)]
        [MenuItem("NextFramework/UIKit/生成配置",false)]
        static void GenerateConfig()
        {
            StringBuilder builder = new StringBuilder();

            List<Panel> panelList = getPanelList();
            foreach (var item in panelList)
            {
                builder.Append("\n\t\t");
                builder.Append(item.Name);
                builder.Append(",");
            }
            //UIType
            string UITypePath = PathConfig.NextFrameworkPath + "/UIKit/UIType.cs";
            FileHelper.WriteUITypeFile(UITypePath, builder.ToString());
            //Json
            string json = JsonUtility.ToJson(new Serialization<Panel>(panelList));
            FileHelper.UIPanelJson = json;

            AssetDatabase.Refresh();

            //GenerateUIScript()
        }
        /// <summary>
        /// 获取所有UI面板信息
        /// UI面板必须位于包中(也就是子一级文件夹中)
        /// </summary>
        static List<Panel> getPanelList()
        {
            List<Panel> panelList = new List<Panel>();
            DirectoryInfo fileInfos = new DirectoryInfo(PathConfig.UIPrefabPath);
            var packagesDirectory = fileInfos.GetDirectories();
            foreach (var package in packagesDirectory)
            {
                getPrefabsFromFolder(ref panelList, package);
            }

            return panelList;
        }

        static void getPrefabsFromFolder(ref List<Panel> panelList, DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles("*.prefab"))
            {
                string packageName = file.Directory.Name;

                string _Name = Path.GetFileNameWithoutExtension(file.FullName);

                Panel panel = new Panel();
                panel.Name = _Name;
                panel.Path = packageName + "/" + _Name;

                panelList.Add(panel);
            }
        }

        //生成脚本
        static void GenerateUIScript(string packageName, string prefabName)
        {
            if (Selection.activeObject == null) return;

            string folderPath = Path.Combine(PathConfig.UIScriptsFolder, packageName);
            folderPath.CreateFolderIfNotExist();

            string scriptPath = Path.Combine(folderPath, prefabName, ".cs");
            if (File.Exists(scriptPath)) return;

            StringBuilder builder = new StringBuilder(TemplateString.Str_UIPanel_Script_Template);
            builder.Replace("{PREFAB_NAME}", prefabName);
            FileHelper.WriteFile(scriptPath, builder.ToString());
        }
    }
}

