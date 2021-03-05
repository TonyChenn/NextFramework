using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    public class SImage : Image
    {
        [Tooltip("运行时优化掉为精灵为空的图片")]
        //[SerializeField] bool m_CullNoneSprite = false;
        [SerializeField] SpriteAtlas m_SpriteAtlas;
        [SerializeField] string m_SpriteName;

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
                        var sp = m_SpriteAtlas.GetSprite(m_SpriteName);
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

                Sprite temp = Atlas.GetSprite(value);
                if (temp)
                {
                    m_SpriteName = FileHelper.GetFileNameWithoutExtention(value);
                    sprite = temp;
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
        }

        public override void SetNativeSize()
        {
            base.SetNativeSize();
        }
    }
}

