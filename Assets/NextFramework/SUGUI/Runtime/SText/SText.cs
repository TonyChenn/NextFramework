using UnityEngine;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    //[ExecuteAlways]
    [AddComponentMenu("SUGUI/SText", 10)]
    public class SText : Text
    {

        [SerializeField] TextStyle m_Style;

        public TextStyle textStyle
        {
            get { return m_Style; }
            set
            {
                if (m_Style == value) return;
                m_Style = value;
                UpdateStyle();
            }
        }

        public override string text
        {
            get
            {
                return base.text;
            }
            set
            {
                base.text = value;
            }
        }


        protected override void Start()
        {
            base.Start();
            UpdateStyle();
        }

        private void UpdateStyle()
        {
            if (m_Style)
            {
                m_Style.Apply(this);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!IsActive())
            {
                base.OnValidate();
                return;
            }

            UpdateStyle();

            base.OnValidate();
        }
#endif
        internal void AssignDefaultFont()
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }


}
