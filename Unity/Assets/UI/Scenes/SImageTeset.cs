using Common;
using NextFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SImageTeset : MonoBehaviour
{
    [SerializeField] private SImage Image;

    SpriteAtlas CommonAtlas;
    void Start()
    {
        //string path = $"{Application.streamingAssetsPath}/ui/uiatlas/common.u3d";
        //AtlasAssetItem atlas = new AtlasAssetItem("common");
        //atlas.Load(() =>
        //{
        //    CommonAtlas = atlas.AtlasObject;
        //    Image.SetSprite(CommonAtlas, "icon_1");
        //});
        //CommonAtlas = LoadAtlas(path);

        
    }

    SpriteAtlas LoadAtlas(string path)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogError("null");
            return null;
        }
        return bundle.LoadAsset<SpriteAtlas>("common");
    }
}
