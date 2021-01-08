using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UGUIExtention
{
    public class ScrollViewItem : MonoBehaviour, IPoolable, IPoolType
    {
        public string PrefabName;
        public float FirstItemOffset;

        #region IPoolable
        public bool HasRecycled { get; set; }

        public void OnRecycle()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IPoolType

        public static ScrollViewItem Alloc()
        {
            return ObjectPool<ScrollViewItem>.Singlton.Alloc();
        }

        public void Recycle2Cache()
        {
            ObjectPool<ScrollViewItem>.Singlton.Recycle(this);
        }
        #endregion
    }
}

