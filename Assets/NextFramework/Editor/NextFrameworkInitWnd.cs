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


            #region 需要打AB包

            //UI预设文件夹
            PathConfig.UIPrefabPath.CreateFolderIfNotExist();
            //UITexture
            PathConfig.UITextureFolder.CreateFolderIfNotExist();
            //UIAtlas
            PathConfig.UIAtlasFolder.CreateFolderIfNotExist();

            //转表资源
            PathConfig.TableScriptObjectFolder.CreateFolderIfNotExist();
            //TextStyle,ColorStyle
            PathConfig.TextStyleFolder.CreateFolderIfNotExist();
            PathConfig.ColorStyleFolder.CreateFolderIfNotExist();

            #endregion

            AssetDatabase.Refresh();
        }

        [MenuItem("NextFramework/Tools/取消MipMaps")]
        static void cancelMipMaps()
        {
            string[] files = Directory.GetFiles(PathConfig.UIAtlasFolder, "*.*",
                SearchOption.AllDirectories);
            int totalCount = files.Length;
            if (totalCount <= 0) return;

            int curIndex = 0;
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < totalCount; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("处理中...", files[i], (float)curIndex / totalCount))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }

                if (isPicture(files[i]))
                {
                    string temp = files[i].Replace(Application.dataPath, "Assets/");
                    TextureImporter texture = (TextureImporter)AssetImporter.GetAtPath(temp);
                    if (texture && texture.mipmapEnabled)
                    {
                        builder.AppendLine(files[i]);
                        texture.mipmapEnabled = false;
                        AssetDatabase.ImportAsset(files[i]);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("已取消MipMaps列表", builder.ToString(), "好的");
        }

        static bool isPicture(string path)
        {
            if(File.Exists(path))
            {
                return path.ToLower().EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".psd");
            }
            return false;
        }
    }
}