using UnityEngine;

namespace NextFramework.SUGUI
{
    [RequireComponent(typeof(RectTransform))]
    public class LoopItemBase : MonoBehaviour
    {
        string m_ItemName;
        int m_ItemId = -1;
        int m_ItemIndex = -1;
        GameObject cachedGameObject = null;
        RectTransform cachedTrans = null;
        LoopViewBase m_LoopView = null;

        public GameObject CachedGameObject
        {
            get
            {
                if (cachedGameObject == null) cachedGameObject = gameObject;

                return cachedGameObject;
            }
        }
        public RectTransform CachedTrans
        {
            get
            {
                if (cachedTrans == null) cachedTrans = CachedGameObject.GetComponent<RectTransform>();

                return cachedTrans;
            }
        }
        public int ID { get { return m_ItemId; }set { m_ItemId = value; } }
        public int Index { get { return m_ItemIndex; } set { m_ItemIndex = value; } }
        public string PrefabName { get { return m_ItemName; } set { m_ItemName = value; } }
        
        public LoopViewBase ParentLoopView { get { return m_LoopView; } set { m_LoopView = value; } }
        
        public Vector2 ItemSize { get { return CachedTrans.rect.size; } }

        public void Init(int totalCount)
        {

        }
    }
}
