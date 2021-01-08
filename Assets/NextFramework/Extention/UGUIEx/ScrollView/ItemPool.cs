using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UGUIExtention
{
    public class ItemPool:ObjectPool<ScrollViewItem>
    {
        static int mCurItemIndex = 0;

        GameObject mItemPrefab;
        RectTransform mParentTrans;
        float mPadding = 0;
        float mFirstItemOffset = 0;


        public void InitItemPool(RectTransform parent, GameObject prefab, float firstItemOffset, float padding, int initCount)
        {
            mItemPrefab = prefab;
            mParentTrans = parent;
            mPadding = padding;
            mFirstItemOffset = firstItemOffset;
        }

        public override ScrollViewItem Alloc()
        {
            base.Alloc();

            GameObject go = GameObject.Instantiate(mItemPrefab, Vector3.zero, Quaternion.identity, mParentTrans);

            var itemTrans = go.GetComponent<RectTransform>();
            itemTrans.localPosition = Vector3.zero;
            itemTrans.localEulerAngles = Vector3.zero;
            itemTrans.localScale = Vector3.one;

            var result = go.GetComponent<ScrollViewItem>();
            result.PrefabName = mItemPrefab.name;
            result.FirstItemOffset = mFirstItemOffset;

            result.HasRecycled = false;

            go.SetActive(false);

            return result;
        }

        public override bool Recycle(ScrollViewItem obj)
        {
            return base.Recycle(obj);
        }



        //public ScrollViewItem GetItem()
        //{
        //    mCurItemIndex++;
        //    var item = Create();
        //    item.name =
        //    RectTransform itemTrans = item.GetComponent<RectTransform>();

        //    item.gameObject.SetActive(true);
        //    return item;
        //}

        //ScrollViewItem CreateItem()
        //{

        //}

        //public override bool Recycle(ScrollViewItem obj)
        //{
        //    return base.Recycle(obj);
        //}
    }
}
