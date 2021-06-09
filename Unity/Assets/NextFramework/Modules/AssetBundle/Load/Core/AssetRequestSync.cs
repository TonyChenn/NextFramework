using System;
using UnityEngine;

namespace Common
{
    class AssetRequestSync : AssetRequest
    {
        public AssetRequestSync(string path, ILoadHandler handler, bool askRetryOnFailed)
    : base(path, handler, askRetryOnFailed)
        {
        }


        protected override void StartLoad()
        {
            string path = BundleLoader.GetAssetPathBySyncLoad(AssetPath);
            try
            {
                Bundle = AssetBundle.LoadFromFile(path);
            }
            catch (Exception ex)
            {
                // 加载失败后尝试下载
                UnityEngine.Debug.Log(string.Format("Load asset failed, asset: {0}, exception: {1}", AssetPath, ex.Message));
                TryToDownload();
                return;
            }

            OnSucceed();
        }
    }
}
