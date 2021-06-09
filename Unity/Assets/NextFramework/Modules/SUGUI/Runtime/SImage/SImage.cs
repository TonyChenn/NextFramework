using Common;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace NextFramework.UI
{
    public class SImage : Image
    {
        //[Header("勾选后不再进行渲染")]
        //[Tooltip("运行时优化掉为精灵为空的图片")]
        //[SerializeField] bool m_CullMask = false;

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
                    var sp = m_SpriteAtlas.GetSprite(m_SpriteName);
                    if (sp != null)
                    {
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
                if (string.IsNullOrEmpty(value))
                {
                    sprite = null;
                    m_SpriteName = "";
                    return;
                }
                Sprite temp = Atlas.GetSprite(value);
                if (temp)
                {
                    m_SpriteName = value;
                    sprite = temp;
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

        public void SetSprite(SpriteAtlas atlas,string spriteName)
        {
            Sprite sprite = atlas.GetSprite(spriteName);
            if(sprite!=null)
            {
                m_SpriteName = spriteName;
                overrideSprite = sprite;
            }
        }

        public void SetSprite(string atlasName, string spriteName)
        {
#if UNITY_EDITOR
            string atlasPath = $"Assets/UI/UIAtlas/{atlasName}.spriteatlas";
            Atlas = UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            SpriteName = spriteName;
#else

#endif
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

