﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NextFramework.UI
{
    public class ClickEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public static ClickEventListener Get(GameObject obj)
        {
            ClickEventListener listener = obj.GetComponent<ClickEventListener>();
            if (listener == null)
            {
                listener = obj.AddComponent<ClickEventListener>();
            }
            return listener;
        }

        Action<GameObject> mClickedHandler = null;
        Action<GameObject> mDoubleClickedHandler = null;
        Action<GameObject> mOnPointerDownHandler = null;
        Action<GameObject> mOnPointerUpHandler = null;
        bool mIsPressed = false;

        public bool IsPressd { get { return mIsPressed; } }


        #region IPointerClickHandler,IPointerDownHandler, IPointerUpHandler
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                if (mDoubleClickedHandler != null)
                {
                    mDoubleClickedHandler(gameObject);
                }
            }
            else
            {
                if (mClickedHandler != null)
                {
                    mClickedHandler(gameObject);
                }
            }

        }
        public void OnPointerDown(PointerEventData eventData)
        {
            mIsPressed = true;
            if (mOnPointerDownHandler != null)
            {
                mOnPointerDownHandler(gameObject);
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            mIsPressed = false;
            if (mOnPointerUpHandler != null)
            {
                mOnPointerUpHandler(gameObject);
            }
        }
        #endregion
        public void SetClickEventHandler(Action<GameObject> handler)
        {
            mClickedHandler = handler;
        }

        public void SetDoubleClickEventHandler(Action<GameObject> handler)
        {
            mDoubleClickedHandler = handler;
        }

        public void SetPointerDownHandler(Action<GameObject> handler)
        {
            mOnPointerDownHandler = handler;
        }

        public void SetPointerUpHandler(Action<GameObject> handler)
        {
            mOnPointerUpHandler = handler;
        }


    }

}
