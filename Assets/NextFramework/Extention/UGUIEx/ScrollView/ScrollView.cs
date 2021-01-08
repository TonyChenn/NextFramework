using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NextFramework.UGUIExtention
{
    public enum ArrangeType
    {
        TopToButtom,
        ButtomToTop,
        LeftToRight,
        RightToLEft,
    }

    public class ScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] ArrangeType arrangeType = ArrangeType.TopToButtom;
        [SerializeField] bool ScrollBar;

        int mTotalDataCount = 0;
        bool mIsVertical = false;
        ScrollRect mScrollRect = null;
        RectTransform mViewPortTrans = null;
        RectTransform mContainterTrans = null;
        List<ScrollViewItem> mItemList = new List<ScrollViewItem>();
        List<ItemPool> mObjPoolList = new List<ItemPool>();
        Dictionary<string, ItemPool> mObjPoolDict = new Dictionary<string, ItemPool>();

        private void Start()
        {
            mScrollRect = GetComponent<ScrollRect>();
            if (mScrollRect == null) Debug.LogError("ScrollView need ScrollRect Compoment");

            mViewPortTrans = mScrollRect.viewport;
            mContainterTrans = mScrollRect.content;

            mIsVertical = arrangeType == ArrangeType.TopToButtom ||
                          arrangeType == ArrangeType.ButtomToTop;
        }

        public void Init(int totalDataCount)
        {
            mContainterTrans.localPosition = Vector3.zero;
            initItemPool();
        }

        void initItemPool()
        {
            
        }

        public int TotalCount { get { return mItemList.Count; } }

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
    }
}

