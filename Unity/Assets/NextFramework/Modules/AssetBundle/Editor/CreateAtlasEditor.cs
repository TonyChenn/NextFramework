using System.Collections.Generic;
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
        string atlasPath = PathConfig.UIAtlasFolder.GetRelativePath();
        //图集
        DirectoryInfo[] foldersArray = FolderHelper.GetSubFolders(PathConfig.UIAtlasFolder);
        if (foldersArray != null && foldersArray.Length > 0)
        {
            List<Object> spriteList = new List<Object>();
            for (int i = 0, iMax = foldersArray.Length; i < iMax; i++)
            {
                spriteList.Clear();
                //添加 文件 方式
                string[] textureGUIDs = AssetDatabase.FindAssets("t:texture", new string[] { foldersArray[i].FullName.GetRelativePath() });
                foreach (var guid in textureGUIDs)
                {
                    Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));

                    if (sp != null)
                        spriteList.Add(sp);
                }
                if (spriteList.Count > 0)
                {
                    string atlasName = $"{atlasPath}/{foldersArray[i].Name}.spriteatlas";

                    var atlas = mGenAtlas();
                    atlas.SetIncludeInBuild(false);
                    atlas.Add(spriteList.ToArray());

                    AssetDatabase.CreateAsset(atlas, atlasName);

                    //设置AssetBundle名
                    AssetDatabase.Refresh();
                    ABTool.Singlton.SetAssetBundleName(atlasName,
                        string.Format("ui/uiatlas/{0}", foldersArray[i].Name));

                    AssetDatabase.SaveAssets();
                }

                //添加文件夹方式
                //var obj = AssetDatabase.LoadAssetAtPath($"{atlasPath}/{foldersArray[i].Name}", typeof(object));
                //atlas.Add(new[] { obj });
            }
            Debug.Log("<color='green'>图集生成成功！</color>");
        }
    }
    static SpriteAtlas mGenAtlas()
    {
        SpriteAtlasPackingSettings atlasSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 1,
            enableRotation = false,
            enableTightPacking = false,
            padding = 2,
        };
        SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };
        
        //Default
        TextureImporterPlatformSettings compressDefault = new TextureImporterPlatformSettings()
        {
            name = "DefaultTexturePlatform",
            overridden = false,
            maxTextureSize = 2048,
            compressionQuality = 50,
            crunchedCompression = true,
            format = TextureImporterFormat.Automatic,
            textureCompression = TextureImporterCompression.Compressed,
        };
        //IOS
        TextureImporterPlatformSettings compressIOS = new TextureImporterPlatformSettings()
        {
            name = "iPhone",
            overridden = true,
            maxTextureSize = 2048,
            compressionQuality = 50,
            crunchedCompression = true,
            format = TextureImporterFormat.RGBA32,
            textureCompression = TextureImporterCompression.Compressed,
        };
        //Android
        TextureImporterPlatformSettings compressAndroid = new TextureImporterPlatformSettings()
        {
            name = "Android",
            overridden = true,
            maxTextureSize = 2048,
            compressionQuality = 50,
            crunchedCompression = true,
            format = TextureImporterFormat.Automatic,
            textureCompression = TextureImporterCompression.Compressed,
        };

        SpriteAtlas atlas = new SpriteAtlas();
        atlas.SetPackingSettings(atlasSetting);
        atlas.SetTextureSettings(textureSetting);
        atlas.SetPlatformSettings(compressDefault);
        atlas.SetPlatformSettings(compressIOS);
        atlas.SetPlatformSettings(compressAndroid);
        atlas.SetIncludeInBuild(true);
        atlas.SetIsVariant(false);

        return atlas;
    }
}

public class SetTextureABNameEditor
{
    public static void SetTextureABName()
    {
        string folderPath = PathHelper.CombinePath(PathConfig.RootPath, PathConfig.UITextureFolder);

        string[] textureGUIDs = AssetDatabase.FindAssets("t:texture", new string[] { folderPath.GetRelativePath() });

        foreach (var guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string abName = path.Replace("Assets/" + PathConfig.UITextureFolder + "/", "");
            abName = abName.Substring(0, abName.LastIndexOf("."));

            ABTool.Singlton.SetAssetBundleName(path, abName);
            AssetDatabase.Refresh();
        }

        //    FileInfo[] files = FileHelper.GetAllFiles(PathConfig.UITextureFolder);
        //if (files != null && files.Length > 0)
        //{
        //    for (int i = 0, iMax = files.Length; i < iMax; i++)
        //    {
        //        FileInfo file = files[i];
        //        if (file.Name.ToLower().EndsWith(".jpg") || file.Name.ToLower().EndsWith(".png") ||
        //           file.Name.ToLower().EndsWith(".psd"))
        //        {
        //            string filePath = file.FullName.GetRelativePath();
        //            ABTool.Singlton.SetAssetBundleName(filePath, filePath.Replace("Assets/", "").ToLower());
        //        }
        //    }
        //    AssetDatabase.Refresh();
        //}
    }
}
