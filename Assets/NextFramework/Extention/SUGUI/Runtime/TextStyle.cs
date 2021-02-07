using UnityEngine;

namespace NextFramework.SUGUI
{
    [CreateAssetMenu(fileName = "TextStyle", menuName ="New TextStyle", order =1)]
    public class TextStyle : ScriptableObject
    {
        [SerializeField] Font mFont;
        [SerializeField] FontStyle mFontStyle;
        [Range(1, 300)]
        [SerializeField] int mFontSize;


        public void Apply(UnityEngine.UI.Text text)
        {
            if (text == null) return;

            text.font = mFont;
            text.fontStyle = mFontStyle;
            text.fontSize = mFontSize;
        }
    }
}

