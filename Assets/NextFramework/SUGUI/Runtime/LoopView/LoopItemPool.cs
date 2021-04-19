using System;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.SUGUI
{
    public class LoopItemPool
    {
        int m_InitCount = 4;
        static int m_CurItemID = 0;
        LoopItemBase m_ItemPrefab;
        string m_PrefabName;
        RectTransform m_ParentTrans;

        Stack<LoopItemBase> m_PooledStack = new Stack<LoopItemBase>();


        public void Init(RectTransform parentTrans, LoopItemBase itemPrefab)
        {
            this.m_ItemPrefab = itemPrefab;
            this.m_PrefabName = itemPrefab.CachedGameObject.name;
            this.m_ParentTrans = parentTrans;
        }

        public LoopItemBase GetItem()
        {
            ++m_CurItemID;

            LoopItemBase item = m_PooledStack.Count > 0 ? m_PooledStack.Pop() : CreateItem();

            item.CachedGameObject.SetActive(true);
            item.ID = m_CurItemID;
            return item;
        }
        LoopItemBase CreateItem()
        {
            GameObject obj = UGUITools.AddChild(m_ParentTrans, m_ItemPrefab.CachedGameObject);
            LoopItemBase item = obj.GetComponent<LoopItemBase>();
            item.CachedTrans.localScale = Vector3.one;
            item.CachedTrans.localPosition = Vector3.zero;
            item.CachedTrans.localEulerAngles = Vector3.zero;
            item.PrefabName = m_PrefabName;

            return item;
        }

        public void RecycleItem(LoopItemBase item)
        {
            item.CachedGameObject.SetActive(false);
            m_PooledStack.Push(item);
        }

        /// <summary>
        /// 移除所有
        /// </summary>
        public void ReleaseAllItem()
        {
            while (m_PooledStack.Count > 0)
            {
                GameObject.DestroyImmediate(m_PooledStack.Pop().CachedGameObject);
            }
        }
    }
}
