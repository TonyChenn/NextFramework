using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UI
{
    public class UIWndData
    {
        /// <summary>
        /// 上一级界面ID
        /// </summary>
        public UIType PreUIID
        {
            get { return m_PreUIID; }
        }
        /// <summary>
        /// 扩展参数
        /// </summary>
        public object ExData
        {
            get { return m_ExData; }
        }
        public bool IsReturn
        {
            get { return m_bReturn; }
        }

        private UIType m_PreUIID = UIType.None;
        private bool m_bReturn;
        private object m_ExData = null;

        public UIWndData(UIType preID, bool bReturn, object exData)
        {
            m_PreUIID = preID;
            m_bReturn = bReturn;
            m_ExData = exData;
        }
    }

    public abstract class UIWndBase : MonoBehaviour
    {
        [SerializeField] protected bool IsPopWnd = false;
        
        bool readyShow = false;

        protected Transform mTrans;
        protected GameObject mObj;
        protected UIType mUIID = UIType.None;
        protected UIType mPreUIID = UIType.None;

        public Transform CachedTransform
        {
            get
            {
                if (mTrans == null)
                    mTrans = this.transform;
                return mTrans;
            }
        }
        public GameObject CachedGameObject
        {
            get
            {
                if (mObj == null)
                    mObj = this.gameObject;
                return mObj;
            }
        }
        /// <summary>
        /// 界面ID
        /// </summary>
        public UIType UIID { get { return mUIID; } }
        /// <summary>
        /// 上一级界面ID
        /// </summary>
        public UIType PreUIID { get { return mPreUIID; } }

        public bool ReadyShow
        {
            get { return readyShow; }
            set { readyShow = value; }
        }

        protected abstract void SetUIType();
        protected virtual void Awake()
        {
            mTrans = this.transform;
            mObj = this.gameObject;
            SetUIType();
            InitWndOnAwake();
        }
        public virtual void InitWndOnAwake() { }

        protected virtual void Start()
        {
            InitWndOnShart();
        }
        public virtual void InitWndOnShart() { }

        public virtual void OnShowWnd(UIWndData wndData)
        {
            RegisterMessage();
            if (!wndData.IsReturn)
                mPreUIID = wndData.PreUIID;
            readyShow = true;
        }

        /// <summary>
        /// //gameobject设置为true之后
        /// </summary>
        public virtual void OnReadShow() { }
        public virtual void OnHideWnd()
        {
            RemoveMessage();
        }
        public void Close()
        {
            UIManager.HideUIWnd(mUIID);
        }

        public virtual void RegisterMessage() { }
        public virtual void RemoveMessage() { }

        /// <summary>
        ///  重连后重置ui数据
        /// </summary>
        public virtual void ResetUIData() { }

        public virtual void SetActiveByRoot(bool state)
        {
            if (mObj != null)
                mObj.SetActive(state);
        }
    }
}

