using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UIKit
{
    public class UIWndData
    {
        public object object_data;
        public int int_data;
        public string str_data;
        public double double_data;
    }
    public abstract class UIWndBase : MonoBehaviour
    {
        protected Transform mTrans;
        protected GameObject mObj;
        protected UIType mUIID;
        protected UIType mPreUIID;

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
        public UIType UIID { get { return mUIID; } }

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

        public virtual void OnShowWnd()
        {
            RegisterMessage();
        }
        public virtual void OnHideWnd()
        {
            RemoveMessage();
        }
        public void Close()
        {

        }

        public virtual void RegisterMessage() { }
        public virtual void RemoveMessage() { }
    }
}

