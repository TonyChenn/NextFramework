using NextFramework;
using NextFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Common
{
    // 主要用于下载，使用时才尝试下载的AB包
    public partial class BundleDownloader : NormalSingleton<BundleDownloader>
    {
        List<ToDownloadBundle> m_toDownloadSingleBundles = new List<ToDownloadBundle>();
        List<ToDownloadBundle> m_downloadingSingleBundles = new List<ToDownloadBundle>();


        #region ToDownloadBundle

        class ToDownloadBundle
        {
            public string Path { get; private set; }
            public Action<bool, string> FinishedHandler { get; private set; }
            public string Url { get; private set; }
            public BundleItem BundleObj { get; private set; }

            static string GetUrl(string path)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path");

                path = path.TrimStart('/').ToLower();
                string url = BundleDownloader.Singlton.RootUrl + path;
                return url;
            }

            public ToDownloadBundle(string path, Action<bool, string> onFinished)
            {
                Path = path;
                FinishedHandler = onFinished;
                Url = GetUrl(path);
                BundleObj = BundleDownloader.Singlton.GetBundleObj(path);
            }

            public void AddFinishHandler(Action<bool, string> onFinished)
            {
                if (onFinished != null)
                    FinishedHandler += onFinished;
            }

            public void StartDownload()
            {
                int length = BundleObj != null ? BundleObj.Length : 0;
                var etor = EtorDownload();
                UICoroutine.Singlton.StartCoroutine(etor);
            }

            IEnumerator EtorDownload()
            {
                /*
                总结：
                自己写的超时逻辑，超时一定要用 Abort() 来终结，调用 Abort 后会触发错误，但有时却不触发错误，此时可能会导致协和永远死循环，因此用 time out 来做第二层保护
                */
                using (UnityWebRequest uwrItem = UnityWebRequest.Get(Url))
                {
                    int uwrTimeout = Mathf.Max(60, BundleObj.Length / AllowMinSpeed);
                    uwrItem.timeout = uwrTimeout;
                    uwrItem.SendWebRequest();

                    float blockTimer = TimeOut;
                    float timer = uwrTimeout + 30;
                    ulong downloadedBytes = 0;

                    while (!uwrItem.isDone)
                    {
                        if (timer < 0)
                        {
                            uwrItem.Abort();
                            OnFailed("Manual timeout");
                            yield break;
                        }
                        else
                        {
                            timer -= Time.deltaTime;
                        }

                        if (uwrItem.isHttpError || uwrItem.isNetworkError)
                        {
                            OnFailed("Time out, " + uwrItem.error);
                            yield break;
                        }

                        if (downloadedBytes == uwrItem.downloadedBytes)
                        {
                            // 检测是否超时
                            //Debug.Log("block time: " + blockTimer);
                            if (blockTimer < 0)
                                uwrItem.Abort();
                            else
                                blockTimer -= Time.deltaTime;
                        }
                        else
                        {
                            downloadedBytes = uwrItem.downloadedBytes;
                            blockTimer = TimeOut;
                            //Debug.Log(string.Format("#1 {0}   {1}   {2}", uwrItem.downloadedBytes, DownloadedBytes, item.Name));
                        }

                        yield return null;
                    }

                    if (uwrItem.isHttpError || uwrItem.isNetworkError)
                    {
                        OnFailed(uwrItem.error);
                        yield break;
                    }

                    // 写入磁盘
                    string localPath = BundleLoader.GetAssetBundlePersistPath(BundleObj.Name);
                    string writeError;
                    bool writeSucceed = WriteBytes(localPath, uwrItem.downloadHandler.data, out writeError);

                    if (writeSucceed)
                        OnSucceed();
                    else
                        OnFailed(uwrItem.error);
                }
            }

            void OnFailed(string error)
            {
                Debug.Log(string.Format("Download {0} failed, error: {1}", Url, error));
                bool succeed = false;
                BundleDownloader.Singlton.OnSingleABDownloadFinished(succeed, this);
                if (FinishedHandler != null)
                    FinishedHandler(succeed, error);
            }

            void OnSucceed()
            {
                Debug.Log(string.Format("Download {0} succeed", Url));
                bool succeed = true;
                BundleDownloader.Singlton.OnSingleABDownloadFinished(succeed, this);
                if (FinishedHandler != null)
                    FinishedHandler(succeed, null);
            }
        }

        #endregion


        // 当加载失败后下载
        public void DownloadSingleAB(string path, Action<bool, string> callback)
        {
            Debug.Log(string.Format("Start download: {0}", path));
            ToDownloadBundle b = m_toDownloadSingleBundles.Find(t => t.Path == path);
            if (b != null)
            {
                b.AddFinishHandler(callback);
            }
            else
            {
                b = new ToDownloadBundle(path, callback);
                m_toDownloadSingleBundles.Add(b);
            }
        }

        BundleItem GetBundleObj(string path)
        {
            for (int i = 0; i < m_dicBundles.Length; i++)
            {
                var dicBundle = m_dicBundles[i];
                if (dicBundle == null)
                    continue;
                BundleItem bi;
                dicBundle.TryGetValue(path, out bi);
                if (bi != null)
                    return bi;
            }

            return null;
        }

        // 下载完成后的回调
        void OnSingleABDownloadFinished(bool succeed, ToDownloadBundle b)
        {
            int i = m_downloadingSingleBundles.FindIndex(t => t == b);
            if (i >= 0)
            {
                m_downloadingSingleBundles.RemoveAt(i);
            }

            if (succeed)
            {
                // 将资源的状态改为已下载（NoChange）
                b.BundleObj.State = BundleState.NoChange;
                m_bundleDownloaded.Add(b.BundleObj);

                string error;
                if (m_bundle2Download.Count > CountToWriteAllList)
                    WriteAllListFile(out error);        // 保存所有的 list 文件，此时会删除已下载资源缓存文件，和清空已下载资源列表
                else
                    WriteListFile4DownloadedB(out error);           // 写入缓存文件
            }
        }

        // 是否正在下载中，或是否需要下载
        public bool SingleABNeedDownload(string path)
        {
            // 下载中
            ToDownloadBundle obj = m_toDownloadSingleBundles.Find(t => t.Path == path);
            if (obj != null)
                return true;

            // 属于加载时再下载的资源
            BundleItem bundleObj = GetBundleObj(path);
            return bundleObj != null && bundleObj.State == BundleState.DownloadWhenUse;
        }

        public void Update()
        {
            // 当正在下载的数量不超过最大数量时，从队列中取出下一个目标并下载
            if (m_toDownloadSingleBundles.Count > 0 && m_downloadingSingleBundles.Count < MaxDownloadThreadsCount)
            {
                var b = m_toDownloadSingleBundles[0];
                b.StartDownload();
                m_toDownloadSingleBundles.RemoveAt(0);
                m_downloadingSingleBundles.Add(b);
            }
        }

    }
}
