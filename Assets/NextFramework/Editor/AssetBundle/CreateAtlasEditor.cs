using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class CreateAtlasEditor
{
    /// <summary>
    /// 创建图集
    /// </summary>
    public static void CreateAtlas()
    {
        string atlasPath = PathConfig.UIAtlasFolder.Replace(Application.dataPath, "Assets");
        //图集
        DirectoryInfo[] foldersArray = FolderHelper.GetSubFolders(PathConfig.UIAtlasFolder);
        if (foldersArray != null && foldersArray.Length > 0)
        {
            SpriteAtlasPackingSettings atlasSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = true,
                enableTightPacking = true,
                padding = 2,
            };
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings()
            {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                crunchedCompression = true,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            };

            for (int i = 0, iMax = foldersArray.Length; i < iMax; i++)
            {
                string atlasName = string.Format("{0}/{1}.spriteatlas", atlasPath, foldersArray[i].Name);

                SpriteAtlas atlas = new SpriteAtlas();
                atlas.SetPackingSettings(atlasSetting);
                atlas.SetTextureSettings(textureSetting);
                atlas.SetPlatformSettings(platformSettings);

                AssetDatabase.CreateAsset(atlas, atlasName);

                //添加文件夹
                var obj = AssetDatabase.LoadAssetAtPath(string.Format("{0}/{1}", atlasPath, foldersArray[i].Name), typeof(object));
                atlas.Add(new[] { obj });

                //添加文件
                //var files = FileHelper.GetAllFiles(foldersArray[i]);
                //for (int j = 0, jMax = files.Length; j < jMax; j++)
                //{
                //    if (files[j].Name.ToLower().EndsWith(".jpg") || files[j].Name.ToLower().EndsWith(".png") ||
                //       files[j].Name.ToLower().EndsWith(".psd"))
                //    {
                //        var temp = AssetDatabase.LoadAssetAtPath<Sprite>(string.Format("{0}/{1}/{2}", atlasPath, foldersArray[i].Name, files[j].Name));
                //        atlas.Add(new[] { temp });
                //    }
                //}


                //设置AssetBundle名
                AssetDatabase.Refresh();
                ABTool.Singlton.SetAssetBundleName(atlasName,
                    string.Format("ui/uiatlas/{0}", foldersArray[i].Name));
                AssetDatabase.Refresh();
            }
            Debug.Log("<color='green'>图集生成成功！</color>");
        }
    }
}

public class SetTextureABNameEditor
{
    public static void SetTextureABName()
    {
        string textureFolderPath = PathConfig.UITextureFolder.Replace(Application.dataPath, "Assets");
        FileInfo[] files = FileHelper.GetAllFiles(PathConfig.UITextureFolder);
        if(files != null && files.Length > 0)
        {
            for (int i = 0,iMax=files.Length; i < iMax; i++)
            {
                FileInfo file = files[i];
                if (file.Name.ToLower().EndsWith(".jpg") || file.Name.ToLower().EndsWith(".png") ||
                   file.Name.ToLower().EndsWith(".psd"))
                {
                    string filePath = file.FullName.GetUnityPath();
                    ABTool.Singlton.SetAssetBundleName(filePath, filePath.Replace("Assets/", "").ToLower());
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
