using NextFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SImageTeset : MonoBehaviour
{
    [SerializeField] private SImage Image;
    void Start()
    {
        string path = $"{Application.streamingAssetsPath}/ui/uiatlas/common.u3d";
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogError("null");
            return;
        }
        var prefab = bundle.LoadAsset<SpriteAtlas>("UI_MessageBox");
        UGUITools.AddChild(UIManager.CanvasRoot, prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
