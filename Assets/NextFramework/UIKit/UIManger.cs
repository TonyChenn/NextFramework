using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UIKit
{
    public class UIManger : NormalSingleton<UIManger>
    {
        Dictionary<UIType, UIWndBase> mAllUIWndDict = new Dictionary<UIType, UIWndBase>();
        Dictionary<UIType, string> mPanelPathDict = new Dictionary<UIType, string>();
        List<UIType> mCurShowUIList = new List<UIType>();
        List<UIType> mCurCacheUIList = new List<UIType>();

        public UIManger()
        {
#if UNITY_EDITOR
            if (GameConfig.Singlton.UseLocalAsset)
                LoadResFromAsset();
            else
                LoadResFromAssetBundle();
#else
            LoadResFromAssetBundle();
#endif

        }

        public List<UIType> CurShowUIList { get { return mCurShowUIList; } }
        public List<UIType> CurCacheUIList { get { return mCurCacheUIList; } }
        public Dictionary<UIType, UIWndBase> AllUIWndDict { get { return mAllUIWndDict; } }

        void LoadResFromAsset()
        {

        }
        void LoadResFromAssetBundle()
        {

        }
    }
}

