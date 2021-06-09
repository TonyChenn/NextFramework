using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NextFramework.UI
{
    [Serializable]
    public class Panel
    {
        public string Name;
        public string Path;

        public Panel(){}

        public Panel(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }

    public class UIManager : NormalSingleton<UIManager>
    {
        private UIManager(){}

        // UI AssetBundle后缀名
        public const string AssetBundleSuffix = ".u";
        public const string Str_BeautifyIconFolderName = "Beautify/Icon";
        public const string Str_ItemIconFolderName = "ItemIcon/";
        public const string Str_ModeIconFolderName = "ModeIcon/";
        public const string Str_CreateRoomFolderName = "CreateRoomIcon/";
        public const string Str_RoomIconFolderName = "DanceRoomIcon/";
        public const string Str_SongIconFolderName = "SongIcon/";
        public const string Str_RoleIconFolderName = "RoleIcon/";
        public const string Str_DanceGroupIconFolderName = "DanceGroupIcon/";
        public const string Str_BGTxFolderName = "UI_BG/";
        public const string Str_SpiritImageTxFolderName = "SpiritImage/";
        public const string Str_StarSignFolderName = "StarSignImage/";
        public const string Str_HornBGFolderName = "HornBG/";
        public const string Str_RoomItemModelFlag = "RoomModelFlag/";
        public const string Str_ChallengeIcon = "ChallengeIcon/";
        public const string Str_PokedexIcon = "Pokedex/";
        public const string Str_AchievementIcon = "Achievement/";
        public const string Str_StarlightTheatreAlbumIcon = "StarlightTheatre/AlbumIcon/";
        public const string Str_TrackLineFolderName = "TrackLine";
        public const string Str_MallHotTxFolderName = "MallRecommedTx";
        public const string Str_IdolIconFolderName = "Idol";
        public const string Str_NewFlashTxFolderName = "NewFlashTx";
        public const string StrTopVipFolderName = "VIPIcon/TopVip/";
        public const string ActivityBg = "Activity/Bg/";
        public const string Str_RingIcon = "RingIcon/";
        public const string Str_ScenePay = "ScenePay/";

        public static UIFlagEnumComparer UITypeComparer = new UIFlagEnumComparer();

        // 加载过的UI池
        Dictionary<UIType, UIWndBase> m_mapAllUIWnd = new Dictionary<UIType, UIWndBase>(UITypeComparer);
        List<UGUIAssetItem> mAssetList = new List<UGUIAssetItem>();
        List<UIType> mCurCacheUIIDList = new List<UIType>();
        List<UIType> mCurShowUIIDList = new List<UIType>();

        static GameObject canvasRoot = null;
        static GameObject popCanvasRoot = null;

        public List<UIType> CurShowUIIDs
        {
            get { return mCurShowUIIDList; }
        }

        public Dictionary<UIType, UIWndBase> AllUIWnd
        {
            get { return m_mapAllUIWnd; }
        }

        public static GameObject CanvasRoot
        {
            get
            {
                if (!canvasRoot) canvasRoot = GameObject.Find("Canvas");
                return canvasRoot;
            }
        }

        public static GameObject PopCanvasRoot
        {
            get
            {
                if (popCanvasRoot == null) popCanvasRoot = GameObject.Find("PopCanvas");
                return popCanvasRoot;
            }
        }

        public GameObject MsgWaitObj { get; set; }
        public GameObject ToastObj { get; set; }
        public GameObject MessageBoxObj { get; set; }
        public GameObject LoadingObj { get; set; }
        public GameObject MarqueeObj { get; set; }
        public GameObject LittleTipObj { get; set; }

        public GameObject PrefabBillboardRankPlatformTop { get; set; }

        #region StaticFun

        public static void RegisterUI(UIWndBase uiWnd)
        {
            Singlton.RegisterWnd(uiWnd);
        }

        public static void HideUIWnd(UIType hideID)
        {
            HideUIWnd(new UIType[] {hideID});
            //检测是否需要提示新功能开启
            //GameLogic.Singleton.ShowNewFunTipWnd();
        }

        public static void HideUIWnd(UIType[] arHideID)
        {
            Singlton.HideWndSync(arHideID);
        }

        public static void ResetDataByShowUI()
        {
            for (int i = 0; i < Singlton.mCurShowUIIDList.Count; ++i)
            {
                if (Singlton.m_mapAllUIWnd.ContainsKey(Singlton.mCurShowUIIDList[i]))
                    Singlton.m_mapAllUIWnd[Singlton.mCurShowUIIDList[i]].ResetUIData();
            }
        }

        /// <summary>
        /// 同步销毁所有加载过的UIWnd
        /// </summary>
        public static void CleanAllWnd()
        {
            Singlton.CleanAllWndSync();
        }

        /// <summary>
        /// UI是否在加载中
        /// </summary>
        /// <param name="uiID"></param>
        /// <returns></returns>
        public static bool IsUICaching(UIType uiID)
        {
            if (uiID != UIType.None)
            {
                return Singlton.mCurCacheUIIDList.Contains(uiID);
            }

            return false;
        }

        public static bool IsUIShowing(UIType uiID)
        {
            if (uiID != UIType.None)
            {
                return Singlton.mCurShowUIIDList.Contains(uiID);
            }

            return false;
        }

        public static List<UIType> GetAllShowingUI()
        {
            List<UIType> result = new List<UIType>();
            for (int i = 0, iMax = Singlton.mCurShowUIIDList.Count; i < iMax; ++i)
            {
                result.Add(Singlton.mCurShowUIIDList[i]);
            }

            return result;
        }

        public static UIWndBase GetUIWnd(UIType uiID)
        {
            UIWndBase result = null;
            if (uiID != UIType.None)
            {
                Singlton.m_mapAllUIWnd.TryGetValue(uiID, out result);
            }

            return result;
        }

        // 同步
        public static void ShowUISync(UIType packageID,UIType showID, object exData)
        {
            ShowUISync(packageID, showID, UIType.None, false, exData, UIType.None);
        }

        public static void ShowUISync(UIType packageID, UIType showID, UIType preID, object exData, UIType hideID)
        {
            ShowUISync(packageID, showID, preID, false, exData, hideID);
        }

        /// <summary>
        /// 同步显示UI
        /// </summary>
        /// <param name="showID"> 显示ID</param>
        /// <param name="preID"> 前置ID</param>
        /// <param name="bReturn"> 如为true 忽略preID</param>
        /// <param name="exData"> 传给显示UI的参数</param>
        /// <param name="hideID"> 需要隐藏的UI</param>
        public static void ShowUISync(UIType packageID, UIType showID, UIType preID, bool bReturn, object exData, UIType hideID)
        {
            UICoroutine.Singlton.StartCoroutine(Singlton.ShowWndAsync(packageID, showID, preID, bReturn, exData,
                new UIType[] {hideID}));
        }

        // 异步
        public static IEnumerator ShowUIAsync(UIType packageID, UIType showID, object exData, UIType hideID)
        {
            IEnumerator itor = Singlton.ShowWndAsync(packageID, showID, UIType.None, false, exData, new UIType[] {hideID});
            while (itor.MoveNext())
            {
                yield return null;
            }
        }

        public static IEnumerator ShowUIAsync(UIType packageID, UIType showID, UIType preID, bool bReturn,
            object exData, UIType hideID)
        {
            IEnumerator itor = Singlton.ShowWndAsync(packageID, showID, preID, bReturn, exData, new UIType[] {hideID});
            while (itor.MoveNext())
            {
                yield return null;
            }
        }

        #endregion

        void RegisterWnd(UIWndBase uiWnd)
        {
            if (m_mapAllUIWnd.ContainsKey(uiWnd.UIID))
            {
                Debug.LogError("ui already register, please check : " + uiWnd.UIID.ToString());
            }
            else
            {
                m_mapAllUIWnd.Add(uiWnd.UIID, uiWnd);
                mCurCacheUIIDList.Remove(uiWnd.UIID);
            }
        }

        IEnumerator ShowWndAsync(UIType packageID, UIType showID, UIType preID, bool bReturn, object exData,
            UIType[] hideId)
        {
            //-----> 打开wait界面
            MessageBox.ShowWait(true);
            if (!IsUICaching(showID))
            {
                IEnumerator itor = PrepareUIAsync(packageID, showID);
                while (itor.MoveNext())
                    yield return null;

                itor = SwitchWndAsync(showID, preID, bReturn, exData, hideId);
                while (itor.MoveNext())
                    yield return null;
            }

            MessageBox.ShowWait(false);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 准备显示的UI预设 加载
        /// </summary>
        IEnumerator PrepareUIAsync(UIType packageID, UIType uiID)
        {
            if (uiID != UIType.None)
            {
                UIWndBase uiWnd = GetUIWnd(uiID);
                if (uiWnd == null && !IsUICaching(uiID))
                {
                    mCurCacheUIIDList.Add(uiID);

                    // load ui from bundle
                    UGUIAssetItem asset = new UGUIAssetItem(UGUIAssetItem.AssetType.Prefab, packageID, uiID);
                    mAssetList.Add(asset);
                    bool loaded = false;
                    asset.Load(() =>
                    {
                        asset.AddToCanvas();
                        loaded = true;
                    });
                    while (!loaded)
                        yield return null;

                    // 加载UI的Awake中调用了RegisterUI(UIWndBase uiWnd)方法。已加入map
                    uiWnd = GetUIWnd(uiID);
                    if (uiWnd != null)
                    {
                        uiWnd.SetActiveByRoot(true);
                    }
                    else
                    {
                        Debug.LogError($"prepare ui failed: {uiID}");
                    }
                }
            }
        }

        /// <summary>
        /// 切换界面
        /// </summary>
        IEnumerator SwitchWndAsync(UIType targetID, UIType preID, bool bReturn, object exData, UIType[] hideId)
        {
            HideWndSync(hideId);
            if (targetID != UIType.None && !IsUIShowing(targetID))
            {
                UIWndBase targetWnd = GetUIWnd(targetID);
                if (targetWnd != null)
                {
                    targetWnd.ReadyShow = false;
                    targetWnd.SetActiveByRoot(false);
                    mCurShowUIIDList.Add(targetID);

                    UIWndData targetData = new UIWndData(preID, bReturn, exData);
                    targetWnd.OnShowWnd(targetData);

                    while (!targetWnd.ReadyShow)
                    {
                        if (targetWnd == null)
                            yield break;
                        yield return null;
                    }

                    targetWnd.SetActiveByRoot(true);
                    targetWnd.OnReadShow();

                    // 小红点
                    //LittleRedPointManager.Singleton.Register(targetWnd as ILittleRedPointable);
                }
                else
                {
                    Log.Error($"ui should be prepared before switched: {targetID}");
                }
            }
        }

        private void CleanAllWndSync()
        {
            for (int i = 0, iMax = mCurShowUIIDList.Count; i < iMax; ++i)
            {
                UIType hideID = mCurShowUIIDList[i];
                UIWndBase hideWnd = GetUIWnd(hideID);
                if (hideWnd != null)
                {
                    hideWnd.OnHideWnd();
                    hideWnd.SetActiveByRoot(false);
                }
            }

            mCurShowUIIDList.Clear();
            mCurCacheUIIDList.Clear();
            var ie1 = m_mapAllUIWnd.GetEnumerator();
            while (ie1.MoveNext())
            {
                GameObject.Destroy(ie1.Current.Value.CachedGameObject);
            }

            m_mapAllUIWnd.Clear();

            // 释放所有UI资源
            for (int i = 0; i < mAssetList.Count; i++)
            {
                mAssetList[i].Destory();
            }

            mAssetList.Clear();

            //AppInterface.GUIModule.DestoryPlayerManagerGameObject();
            //AppInterface.GUIModule.DestroyBillboardUI();
        }

        public static void DestroyWnd(UIType uiID)
        {
            if (Singlton != null)
                Singlton.DestroyWndSync(uiID);
        }

        void DestroyWndSync(UIType uiID)
        {
            if (mCurShowUIIDList.Contains(uiID))
            {
                HideWndSync(new UIType[] {uiID});
            }

            UIWndBase wnd;
            if (m_mapAllUIWnd.TryGetValue(uiID, out wnd))
            {
                GameObject.Destroy(wnd.CachedGameObject);
                m_mapAllUIWnd.Remove(uiID);
            }
        }

        void HideWndSync(UIType[] arHideID)
        {
            if (arHideID != null)
            {
                for (int i = 0, hideLength = arHideID.Length; i < hideLength; ++i)
                {
                    UIType hideID = arHideID[i];
                    if (IsUIShowing(hideID))
                    {
                        mCurShowUIIDList.Remove(hideID);

                        UIWndBase hideWnd = GetUIWnd(hideID);
                        if (hideWnd != null)
                        {
                            hideWnd.OnHideWnd();
                            //hideWnd.SetActiveByRoot(false);
                            DestroyWndSync(hideID);
                        }
                    }
                }
            }
        }

        public static void LoadMsgUI()
        {
            Singlton.LoadMsg();
        }

        void LoadMsg()
        {
            LoadUIObj("UI_WaitPanel", (obj) => { MsgWaitObj = obj; });
            LoadUIObj("UI_ToastPanel", obj => ToastObj = obj);
            LoadUIObj("UI_MessageBox", obj => MessageBoxObj = obj);
            
            //LoadUIObj("UI_Loading", obj => LoadingObj = obj);
            //LoadUIObj("UI_Marquee", obj => MarqueeObj = obj);
            //LoadUIObj("UI_LittleTips", obj => LittleTipObj = obj);
        }

        void LoadUIObj(string name, Action<GameObject> onLoaded)
        {
            UGUIAssetItem asset = new UGUIAssetItem(UGUIAssetItem.AssetType.Prefab, "pkg_ui_message", name);
            asset.Load(() =>
            {
                var obj = asset.AddToPopCanvas();
                obj.SetActive(true);
                onLoaded(obj);
            });
        }
    }
}