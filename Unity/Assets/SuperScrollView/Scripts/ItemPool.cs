using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UI
{
    public class ItemPool : IPool<LoopListViewItem2>
    {
        ItemPrefabConfData m_ItemData;
        Stack<LoopListViewItem2> m_TmpStack = new Stack<LoopListViewItem2>();
        Stack<LoopListViewItem2> m_DataStack = new Stack<LoopListViewItem2>();
        static int mCurItemIdCount = 0;
        RectTransform mItemParent = null;
        public ItemPool() { }

        public void Init(ItemPrefabConfData data, RectTransform parent)
        {
            m_ItemData = data;
            mItemParent = parent;
            for (int i = 0; i < data.mInitCreateCount; ++i)
            {
                LoopListViewItem2 tViewItem = CreateItem();
                RecycleItemReal(tViewItem);
            }
        }

        public LoopListViewItem2 Alloc()
        {
            mCurItemIdCount++;

            LoopListViewItem2 item = m_TmpStack.Count > 0 ? m_TmpStack.Pop() : (m_DataStack.Count > 0 ? m_DataStack.Pop() : CreateItem());
            item.gameObject.SetActive(true);
            item.Padding = m_ItemData.mPadding;
            item.ItemId = mCurItemIdCount;
            return item;

        }
        public bool Recycle(LoopListViewItem2 item)
        {
            m_TmpStack.Push(item);
            return true;
        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();

            while (m_DataStack.Count > 0)
            {
                GameObject.DestroyImmediate(m_DataStack.Pop().gameObject);
            }
            m_DataStack.Clear();
        }
        LoopListViewItem2 CreateItem()
        {
            GameObject go = UGUITools.AddChild(mItemParent, m_ItemData.mItemPrefab);
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.localPosition = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            LoopListViewItem2 tViewItem = go.GetComponent<LoopListViewItem2>();
            tViewItem.ItemPrefabName = m_ItemData.mItemPrefab.name;
            tViewItem.StartPosOffset = m_ItemData.mStartPosOffset;
            return tViewItem;
        }
        void RecycleItemReal(LoopListViewItem2 item)
        {
            item.gameObject.SetActive(false);

            m_DataStack.Push(item);
        }
        public void ClearTmpRecycledItem()
        {
            if (m_TmpStack.Count == 0) return;

            while (m_TmpStack.Count > 0)
            {
                RecycleItemReal(m_TmpStack.Pop());
            }
        }
    }
}

