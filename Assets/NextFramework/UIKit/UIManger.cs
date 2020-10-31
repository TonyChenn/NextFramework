using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UIKit
{
    [Serializable]
    public class Panel
    {
        public string Name;
        public string Path;

        public Panel() { }
        public Panel(string name,string path)
        {
            Name = name;
            Path = path;
        }
    }

    public class UIManger : NormalSingleton<UIManger>
    {
        Dictionary<UIType, UIWndBase> mAllUIWndDict = new Dictionary<UIType, UIWndBase>();
        Dictionary<UIType, string> mPanelPathDict = new Dictionary<UIType, string>();
        List<UIType> mCurShowUIList = new List<UIType>();
        List<UIType> mCurCacheUIList = new List<UIType>();

        public UIManger()
        {
#if UNITY_EDITOR
            if (GameConfig.UseLocalAsset)
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

        #region 静态方法
        public static void ShowUI(UIType showID,object data)
        {
            ShowUI(showID, data, UIType.None);
        }
        public static void ShowUI(UIType showID, object data,UIType hideID)
        {
            ShowUI(showID, data, new UIType[] { hideID });
        }
        public static void ShowUI(UIType showID, object data, UIType[] hideIDs)
        {

        }
        public static void HideUI(UIType hideID)
        {

        }
        public static void HideUI(UIType[] hideID)
        {

        }

        #endregion

        #region 加载资源
        void LoadResFromAsset()
        {

        }
        void LoadResFromAssetBundle()
        {

        }
        #endregion
    }
}

