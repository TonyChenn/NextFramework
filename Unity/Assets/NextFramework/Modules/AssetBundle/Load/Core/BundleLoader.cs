using NextFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


namespace Common
{
    public class BundleLoader : NormalSingleton<BundleLoader>, AssetRequest.ILoadHandler
    {
        static StringBuilder s_getPathResult = new StringBuilder();
        static string s_tmpPath = string.Empty;

        Dictionary<string, AssetRequest> m_cacheRequests = new Dictionary<string, AssetRequest>();
        Dictionary<string, AssetRequest> m_updateRequests = new Dictionary<string, AssetRequest>();
        List<string> m_completeRequestPaths = new List<string>();


        #region static

        // 资源同步加载路径（无 file:///）
        public static string GetAssetPathBySyncLoad(string path)
        {
            // 先尝试从 persist 目录加载
            if (GameConfig.Singlton.HotPatching)
            {
                s_tmpPath = GetAssetBundlePersistPath(path);
                if (File.Exists(s_tmpPath))
                    return s_tmpPath;
            }
            s_getPathResult.Length = 0;
            s_getPathResult.Append(Application.streamingAssetsPath);
            s_getPathResult.Append("/");
            s_getPathResult.Append(path);
            s_tmpPath = s_getPathResult.ToString();
            return s_tmpPath;
        }

        public static string GetAssetBundleStreamingPath(string path)
        {
            s_getPathResult.Length = 0;
            s_getPathResult.Append(Application.streamingAssetsPath);
            s_getPathResult.Append("/");
            s_getPathResult.Append(path);
            s_tmpPath = s_getPathResult.ToString();
            return s_tmpPath;
        }

        public static string GetAssetBundlePath(string path)
        {
            if (GameConfig.Singlton.HotPatching)
            {
                //TODO
                if (true /*ClientData.ClientLogin.Singleton.IsLogined*/)
                    return GetAssetBundlePersistUrl(path);
                else
                {
                    string tempPath = GetAssetBundlePersistPath(path);
                    if (File.Exists(tempPath))
                        return GetAssetBundlePersistUrl(path);
                }

            }

            return GetAssetBundleStreamingUrl(path);
        }

        public static string GetAssetBundlePersistPath(string path)
        {
            s_getPathResult.Length = 0;
            s_getPathResult.Append(Application.persistentDataPath);
            s_getPathResult.Append("/");
            s_getPathResult.Append(path);
            return s_getPathResult.ToString();
        }

        public static string GetAssetBundlePersistUrl(string path)
        {
            s_getPathResult.Length = 0;
            s_getPathResult.Append("file:///");
            s_getPathResult.Append(Application.persistentDataPath);
            s_getPathResult.Append("/");
            s_getPathResult.Append(path);
            return s_getPathResult.ToString();
        }

        public static string GetAssetBundleStreamingUrl(string path)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            s_getPathResult.Length = 0;
            s_getPathResult.Append("file://");
            s_getPathResult.Append(Application.streamingAssetsPath);
            s_getPathResult.Append("/");
            s_getPathResult.Append(path.TrimStart('/'));
            s_tmpPath = s_getPathResult.ToString();
#elif UNITY_IOS
            s_getPathResult.Length = 0;
            s_getPathResult.Append("file://");
            s_getPathResult.Append(Application.dataPath);
            s_getPathResult.Append("/Raw/");
            s_getPathResult.Append(path.TrimStart('/'));
            s_tmpPath = s_getPathResult.ToString();
#elif UNITY_ANDROID
            s_getPathResult.Length = 0;
            s_getPathResult.Append("jar:file://");
            s_getPathResult.Append(Application.dataPath);
            s_getPathResult.Append("!/assets/");
            s_getPathResult.Append(path.TrimStart('/'));
            s_tmpPath = s_getPathResult.ToString();
#endif
            return s_tmpPath;
        }

        #endregion


        public void Update()
        {
            var e = m_updateRequests.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.Update();
            }

            // 将complete中的请求从update中删除
            if (m_completeRequestPaths.Count > 0)
            {
                for (int i = 0; i < m_completeRequestPaths.Count; i++)
                {
                    string key = m_completeRequestPaths[i];
                    AssetRequest v;
                    if (m_updateRequests.TryGetValue(key, out v))
                    {
                        v.InvokeFinisheHandle();
                        m_updateRequests.Remove(key);
                    }
                }
                m_completeRequestPaths.Clear();
            }
        }

        //加载资源，若加载失败，则尝试下载
        public AssetRequest LoadAssetBundle(string path, Action<AssetRequest> callback, bool async = true, bool askRetryOnFail = false)
        {
            path = path.ToLower();
            AssetRequest request;
            if (!m_cacheRequests.TryGetValue(path, out request))
            {
                if (async)
                    request = new AssetRequestAsync(path, this, askRetryOnFail);
                else
                    request = new AssetRequestSync(path, this, askRetryOnFail);
                request.TryToRequest();             // 准备加载

                // 缓存
                m_cacheRequests[path] = request;
                m_updateRequests[path] = request;
            }
            request.RefCount++;         // 引用+1
            request.RegisterFinishHandle(callback);  // 记录回调
            return request;
        }

        public void DeleteAssetBundle(string path)
        {
            path = path.ToLower();
            AssetRequest request;
            if (m_cacheRequests.TryGetValue(path, out request))
            {
                request.RefCount--;
                if (request.RefCount < 1)
                {
                    // 取消它，如果在更新
                    if (m_updateRequests.ContainsKey(path))
                    {
                        request.Cancel();
                        m_completeRequestPaths.Add(path);
                    }

                    // 从缓存中去掉
                    request.Dispose();
                    m_cacheRequests.Remove(path);
                }
            }
        }


        // AssetRequest.IHandler
        // 加载完成后从 update 列表中删除它，并且执行回调
        void AssetRequest.ILoadHandler.OnLoadSuccess(AssetRequest request)
        {
            m_completeRequestPaths.Add(request.AssetPath);
        }

        // 加载失败后删除它
        void AssetRequest.ILoadHandler.OnLoadFailed(AssetRequest request)
        {
            m_cacheRequests.Remove(request.AssetPath);
            m_completeRequestPaths.Add(request.AssetPath);
        }
    }
}
