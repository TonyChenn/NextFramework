using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Test_001 : MonoBehaviour
{
    private void OnEnable()
    {
        SpriteAtlasManager.atlasRequested += RequestAtlas;
    }
    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= RequestAtlas;
    }

    /// <summary>
    /// 加载图集
    /// </summary>
    private void RequestAtlas(string tag, Action<SpriteAtlas> callback)
    {
        AssetBundle ab = AssetBundle.LoadFromFile($"{Application.streamingAssetsPath}/ui/uiatlas/{tag}.u3d");
        SpriteAtlas atlas = ab.LoadAsset<SpriteAtlas>(tag);
        callback(atlas);
    }
}
