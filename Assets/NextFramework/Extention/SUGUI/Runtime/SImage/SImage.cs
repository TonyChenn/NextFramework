using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    public enum MirrorType
    {
        /// <summary>
        /// 不做处理
        /// </summary>
        None,
        /// <summary>
        /// 水平
        /// </summary>
        Horizontal,
        /// <summary>
        /// 垂直
        /// </summary>
        Vertival,
        /// <summary>
        /// 四分图
        /// </summary>
        Quarter,
    }

    [AddComponentMenu("SUI/SImage", 11)]
    public class SImage : Image
    {
        [Tooltip("运行时优化掉为精灵为空的图片")]
        [SerializeField] bool m_CullNoneSprite = false;
        [SerializeField] SpriteAtlas m_SpriteAtlas;
        [SerializeField] string m_SpriteName;
        [SerializeField] MirrorType m_MirrorType = MirrorType.None;

        public MirrorType MirrorType
        {
            get { return m_MirrorType; }
            set
            {
                if (m_MirrorType != value)
                {
                    m_MirrorType = value;
                    SetVerticesDirty();
                }

            }
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if ((overrideSprite == null && m_CullNoneSprite))
            {
                toFill.Clear();
                return;
            }
            base.OnPopulateMesh(toFill);
        }

        public override void SetNativeSize()
        {
            base.SetNativeSize();
        }
    }
}

