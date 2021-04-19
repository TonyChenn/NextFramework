using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    public class SImage : Image
    {
        [Header("勾选后不再进行渲染")]
        [Tooltip("运行时优化掉为精灵为空的图片")]
        [SerializeField] bool m_CullMask = false;

        [SerializeField] SpriteAtlas m_SpriteAtlas;
        [SerializeField] string m_SpriteName;
        string m_SpritePath;

        public SpriteAtlas Atlas
        {
            get { return m_SpriteAtlas; }
            set
            {
                if (m_SpriteAtlas != value)
                {
                    m_SpriteAtlas = value;
                    if (!string.IsNullOrEmpty(m_SpriteName))
                    {
                        string name = FileHelper.GetFileNameWithoutExtention(m_SpriteName);
                        var sp = m_SpriteAtlas.GetSprite(name);
                        sprite = sp;
                        return;
                    }
                    sprite = null;
                    m_SpriteName = "";
                }
            }
        }
        public string SpriteName
        {
            get { return m_SpriteName; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                Sprite temp = Atlas.GetSprite(FileHelper.GetFileNameWithoutExtention(value));
                if (temp)
                {
                    m_SpriteName = value;
                    overrideSprite = temp;
                }
            }
        }

        /// <summary>
        /// TODO 后续看是否有必要检查修改的图片是否在图集中
        /// </summary>
        public Sprite Sprite
        {
            get { return overrideSprite; }
            set
            {
                if (overrideSprite == value) return;

                overrideSprite = value;
                //暂时不检测是否在图集中
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            //if (!m_CullMask && color.a > 0.01 && overrideSprite != null)
                //base.OnPopulateMesh(toFill);
        }

        public override void SetNativeSize()
        {
            base.SetNativeSize();
        }
    }
}

