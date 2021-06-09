using System;

namespace Common
{
    public abstract class LoadBundle
    {
        bool deleteOnLoad;
        protected Action loadFinishAction;

        public bool IsLoadComplete { get; private set; }


        #region abstract Method
        public abstract string FullPath { get; }
        protected abstract void OnLoadComplele(AssetRequest req);
        protected abstract void Dispose();
        #endregion


        #region virtual Method
        public virtual void Load(Action callback)
        {
            this.loadFinishAction = callback;
            Load();
        }
        protected virtual bool AsyncMode { get { return true; } }

        protected virtual bool AskReTryOnFail { get { return false; } }
        #endregion
    
        protected void Load()
        {
            BundleLoader.Singlton.LoadAssetBundle(FullPath, LoadFinish, AsyncMode, AskReTryOnFail);
        }

        void LoadFinish(AssetRequest req)
        {
            if(!deleteOnLoad)
            {
                IsLoadComplete = true;
                OnLoadComplele(req);
                if (loadFinishAction != null)
                    loadFinishAction();
            }
            else
            {
                Dispose();
            }
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        public void Destory()
        {
            if(IsLoadComplete)
            {
                Dispose();
                BundleLoader.Singlton.DeleteAssetBundle(FullPath);
            }
            else
            {
                deleteOnLoad = true;
            }
        }
    }
}

