using NextFramework;
using NextFramework.UI;
using System;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Common
{
    public class UGUIAssetItem : LoadBundle
    {
        public enum AssetType
        {
            None,
            Prefab,     //预设
            Atlas,      //图集
            Texture,    //散图
        }
        AssetType assetType;
        string packageName;
        string uiName;
        UObject uiPrefab;

        public UGUIAssetItem(AssetType assetType, UIType packageType, string uiName)
        {
            this.assetType = assetType;
            this.packageName = packageType.ToString();
            this.uiName = System.IO.Path.GetFileNameWithoutExtension(uiName);
        }
        public UGUIAssetItem(AssetType assetType, string packageName, string uiName)
        {
            this.assetType = assetType;
            this.packageName = System.IO.Path.GetFileNameWithoutExtension(packageName);
            this.uiName = System.IO.Path.GetFileNameWithoutExtension(uiName);
        }
        public UGUIAssetItem(AssetType assetType, UIType packageType, UIType uiType)
        {
            this.assetType = assetType;
            this.packageName = packageType.ToString();
            this.uiName = uiType.ToString();
        }

        protected override bool AsyncMode => false;
        public override string FullPath
        {
            get
            {
                var builder = StringBuilderPool.Alloc();
                builder.Append("ui/");
                switch (assetType)
                {
                    case AssetType.Prefab:
                        builder.Append("uiprefab/");
                        break;
                    case AssetType.Atlas:
                        builder.Append("uiatlas/");
                        break;
                    case AssetType.Texture:
                        builder.Append("uitexture/");
                        break;
                }
                builder.Append(packageName);
                builder.Append(".u3d");
                string result = builder.ToString();
                builder.Recycle();
                return result;
            }
        }
        protected override void Dispose()
        {
            uiPrefab = null;
        }
        protected override void OnLoadComplele(AssetRequest req)
        {
#if UNITY_EDITOR
            if (GameConfig.Singlton.UseLocalAsset)
            {
                string path = $"Assets/UI/UIPrefab/{packageName}/{uiName}.prefab";
                uiPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<UObject>(path);
                if (uiPrefab)
                {
                    Log.Debug($"Load {uiName} from local");
                    return;
                }
            }
#endif
            uiPrefab = req.Load(uiName);
        }

        public UObject Prefab { get { return uiPrefab; } }

        /// <summary>
        /// 添加到Canvas下
        /// </summary>
        public GameObject AddToCanvas()
        {
            var obj = UGUITools.AddChild(UIManager.CanvasRoot, (GameObject)uiPrefab);
            obj.name = uiPrefab.name;
            return obj;
        }

        public GameObject AddToPopCanvas()
        {
            var obj = UGUITools.AddChild(UIManager.PopCanvasRoot, (GameObject)uiPrefab);
            obj.name = uiPrefab.name;
            return obj;
        }
    }
}
