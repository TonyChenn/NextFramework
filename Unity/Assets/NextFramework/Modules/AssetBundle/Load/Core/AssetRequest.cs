using System;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Common
{
    public abstract class AssetRequest
    {
        Action<AssetRequest> finishAction;
        ILoadHandler loadHandler;
        bool askReTryOnFail;
        int retryDownloadCount;

        protected AssetRequestState assetState;


        public static int RetryLimit = 3;
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount = 0;

        public string AssetPath { get; private set; }
        public AssetBundle Bundle { get; protected set; }

        public AssetRequest(string path, ILoadHandler handler, bool askReTryOnFail)
        {
            this.AssetPath = path;
            this.loadHandler = handler;
            this.askReTryOnFail = askReTryOnFail;
        }

        // 尝试去加载
        public void TryToRequest()
        {
            retryDownloadCount = 0;

            if (GameConfig.Singlton.HotPatching && BundleDownloader.Singlton.SingleABNeedDownload(AssetPath))
            {
                // 如果此资源已经在下载过程中，则把回调注册进去
                BundleDownloader.Singlton.DownloadSingleAB(AssetPath, OnDownloadFinished);
                assetState = AssetRequestState.Downloading;
            }
            else
            {
                // 去加载
                ToLoad();
            }
        }

        // 开始加载
        void ToLoad()
        {
            Dispose();      // 加载前释放资源
            assetState = AssetRequestState.Loading;
            StartLoad();
        }

        protected void OnSucceed()
        {
            // 加载成功
            assetState = AssetRequestState.LoadSuccess;
            loadHandler.OnLoadSuccess(this);
        }

        // 尝试去下载
        protected void TryToDownload()
        {
            if (!GameConfig.Singlton.HotPatching)
            {
                Fail();
            }
            else if (retryDownloadCount < RetryLimit)
            {
                assetState = AssetRequestState.Downloading;
                BundleDownloader.Singlton.DownloadSingleAB(AssetPath, OnDownloadFinished);       // 去下载
                retryDownloadCount++;
                Debug.Log(string.Format("Retry count: {0}", retryDownloadCount));
            }
            else if (askReTryOnFail)
            {
                string msg = $"Download asset:{AssetPath} fail，Try again?";
                
                //TODO
                //UIMessageMgr.ShowMessageBoxOnlyOK(msg, TryToRequest);
            }
            else
            {
                Fail();
            }
        }

        void Fail()
        {
            Debug.Log(string.Format("Failed to download, hot patching: {0}, retry count: {1}, max retry count: {2}",
                     GameConfig.Singlton.HotPatching, retryDownloadCount, RetryLimit));
            assetState = AssetRequestState.Fail;
            loadHandler.OnLoadFailed(this);
            Dispose();
        }

        // 下载完成后的回调
        void OnDownloadFinished(bool succeed, string error)
        {
            if (succeed)
                ToLoad();           // 如果下载成功，则去加载
            else
                TryToDownload();    // 如果下载失败，则再去尝试下载
        }

        // 取消
        public void Cancel()
        {
            assetState = AssetRequestState.Cancel;
        }

        // 注册此资源的回调
        public void RegisterFinishHandle(Action<AssetRequest> callback)
        {
            if (callback != null)
                finishAction += callback;

            if (assetState == AssetRequestState.LoadSuccess)
                InvokeFinisheHandle();
        }

        // 执行回调
        public void InvokeFinisheHandle()
        {
            if (finishAction != null)
            {
                if (assetState == AssetRequestState.LoadSuccess)
                {
                    InvokeFinishHandleProtected(this);
                }
                else if (assetState == AssetRequestState.Cancel)
                {
                    Log.Debug($"Load Asset url:{AssetPath} cancel, state:{assetState}");
                    InvokeFinishHandleProtected(null);
                }
                else
                {
                    Log.Debug($"AssetRequest url:{AssetPath} fail, state:{assetState}");
                    InvokeFinishHandleProtected(null);
                }
                finishAction = null;
            }
        }

        // 尝试执行回调，并抓取异常
        void InvokeFinishHandleProtected(AssetRequest param)
        {
            try
            {
                finishAction(param);
            }
            catch (Exception ex)
            {
                Log.Error($"Invoke finish handle error, path: {AssetPath}, messenge: {ex.Message}, stack: {ex.StackTrace}");
            }
        }

        public virtual void Dispose()
        {
            if (Bundle != null)
            {
                Bundle.Unload(true);
                Bundle = null;
            }

            assetState = AssetRequestState.Dispose;
        }

        #region Load Asset
        public UObject Load(string name)
        {
            if (Bundle)
                return Bundle.LoadAsset(name);
            return null;
        }
        public UObject Load(string name, Type type)
        {
            if (Bundle)
                return Bundle.LoadAsset(name, type);
            return null;
        }
        public UObject[] LoadAll()
        {
            if (Bundle)
                return Bundle.LoadAllAssets();
            return null;
        }
        public UObject[] LoadAll(Type type)
        {
            if (Bundle)
                return Bundle.LoadAllAssets(type);
            return null;
        }

        public AssetBundleRequest LoadAsync(string name, Type type)
        {
            if (Bundle)
                return Bundle.LoadAssetAsync(name, type);
            return null;
        }
        #endregion

        public virtual void Update() { }
        #region abstract Method
        protected abstract void StartLoad();
        #endregion

        #region Enum
        public enum AssetRequestState
        {
            None,
            Loading,
            Downloading,
            LoadSuccess,
            Fail,
            Cancel,
            Dispose,
        }
        #endregion

        #region Interface
        public interface ILoadHandler
        {
            void OnLoadSuccess(AssetRequest request);
            void OnLoadFailed(AssetRequest request);
        }
        #endregion
    }
}
