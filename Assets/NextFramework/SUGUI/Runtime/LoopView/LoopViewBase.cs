using SuperScrollView;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    [System.Serializable]
    public class LoopItemPrefab
    {
        public string Tag = "";
        public LoopItemBase itemPrefab = null;
    }

    public enum HorArrangeType { LeftToRight, RightToLeft };
    public enum VerArrangeType { TopToButtom, ButtomToTop };

    [RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
    public class LoopViewBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] bool m_VerticalMove = true;
        [SerializeField] HorArrangeType m_HorArrangeType = HorArrangeType.LeftToRight;
        [SerializeField] VerArrangeType m_VerArrangeType = VerArrangeType.TopToButtom;
        [SerializeField] float m_FirstOffset = 0f;
        [SerializeField] float m_PrefabPadding = 0f;
        [SerializeField] List<LoopItemPrefab> m_TemplatePrefab = new List<LoopItemPrefab>();
        [SerializeField] bool m_ShowScrollBar = true;
        [SerializeField] Scrollbar m_ScrollBar = null;

        ClickEventListener m_ScrollBarClickListener = null;

        /// <summary>
        /// 存放不同的对象池
        /// </summary>
        Dictionary<string, LoopItemPool> m_PoolDict = new Dictionary<string, LoopItemPool>();
        List<LoopItemBase> m_ItemList = new List<LoopItemBase>();

        int m_ItemTotalCount = 0;


        ScrollRect m_ScrollRect = null;
        RectTransform m_ContainterRectTrans = null;
        RectTransform m_ViewportRectTrans = null;

        private void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
            m_ContainterRectTrans = m_ScrollRect.content;
            m_ViewportRectTrans = m_ScrollRect.viewport;

            // Init ScrollBar
            if(m_ShowScrollBar && m_ScrollBar!=null)
            {
                m_ScrollBarClickListener = ClickEventListener.Get(m_ScrollBar.gameObject);
                m_ScrollBarClickListener.SetPointerDownHandler(OnPointDownScrollBar);
                m_ScrollBarClickListener.SetPointerUpHandler(OnPointUpScrollBar);
            }
        }
        public bool IsVerticalMove { get { return m_VerticalMove; } }
        public int ItemAllCount { get { return m_ItemTotalCount; } }
        public UnityEngine.UI.ScrollRect ScrollRect { get { return m_ScrollRect; } }
        public RectTransform ContainterTrans { get { return m_ContainterRectTrans; } }
        public RectTransform ViewportTrans { get { return m_ViewportRectTrans; } }
        public bool ShowScrollBar { get { return m_ShowScrollBar; } }


        public virtual void InitLoopView(int totalCount)
        {
            
        }
        #region IBeginDragHandler, IEndDragHandler, IDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region ScrollBar Pointer Down/Up
        private void OnPointUpScrollBar(GameObject obj)
        {
            throw new NotImplementedException();
        }

        private void OnPointDownScrollBar(GameObject obj)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
