using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Common
{
    public class AssetRequestAsync : AssetRequest
    {
        UnityWebRequest m_request;


        public AssetRequestAsync(string path, ILoadHandler handler, bool askRetryOnFailed)
            : base(path, handler, askRetryOnFailed)
        {
        }


        public override void Update()
        {
            // 如果正在加载，监听加载的结果
            if (assetState == AssetRequestState.Loading && m_request.isDone)
            {
                bool loadedSucceed = !(m_request.isHttpError || m_request.isNetworkError);
                if (loadedSucceed)
                {
                    var bytes = m_request.downloadHandler.data;
                    try
                    {
                        Bundle = AssetBundle.LoadFromMemory(bytes);
                    }
                    catch (Exception ex)
                    {
                        // 加载失败后尝试下载
                        Debug.Log(string.Format("Load asset failed, asset: {0}, exception: {1}", AssetPath, ex.Message));
                        TryToDownload();
                    }

                    DisposeRequest();
                    OnSucceed();
                }
                else
                {
                    // 加载失败后尝试下载
                    Debug.Log(string.Format("Load asset failed, asset: {0}, exception: {1}", AssetPath, m_request.error));
                    TryToDownload();
                }
            }
        }

        // 开始加载
        protected override void StartLoad()
        {
            string path = BundleLoader.GetAssetBundlePath(AssetPath);

            m_request = UnityWebRequest.Get(path);
            m_request.SendWebRequest();
        }

        public override void Dispose()
        {
            base.Dispose();
            DisposeRequest();
        }

        void DisposeRequest()
        {
            if (m_request != null)
            {
                m_request.Dispose();
                m_request = null;
            }
        }

    }
}
