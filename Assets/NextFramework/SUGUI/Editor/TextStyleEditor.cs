using UnityEditor;
using UnityEngine;

namespace NextFramework.SUGUI
{
    [CustomEditor(typeof(TextStyle))]
    public class TextStyleEditor : Editor
    {
        bool changed;
        TextStyle mTextStyle;
        TextStyle mTempStyle;

        SerializedObject mTempObj;

        SerializedProperty mFont;
        SerializedProperty mFontStyle;
        SerializedProperty mFontSize;

        void OnEnable()
        {
            mTextStyle = target as TextStyle;
            mTempStyle = Instantiate(mTextStyle);
            mTempObj = new SerializedObject(mTempStyle);

            mFont = mTempObj.FindProperty("mFont");
            mFontStyle = mTempObj.FindProperty("mFontStyle");
            mFontSize = mTempObj.FindProperty("mFontSize");
        }
        private void OnDisable()
        {
            if (changed)
            {
                bool select = EditorUtility.DisplayDialog("Text Style Changed", "配置资源属性改变，请保存!", "保存", "取消");
                if (select) saveAsset();
                changed = false;
            }
            DestroyImmediate(mTempStyle);
        }
        public override void OnInspectorGUI()
        {
            mTempObj.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(mFont);
            EditorGUILayout.PropertyField(mFontStyle);
            EditorGUILayout.PropertyField(mFontSize);

            bool isChanged = EditorGUI.EndChangeCheck();
            mTempObj.ApplyModifiedProperties();
            if (isChanged) changed = true;

            if (GUILayout.Button("Save"))
            {
                saveAsset();
                changed = false;
            }

            GUIStyle style = new GUIStyle();
            style.font = mFont.objectReferenceValue as Font;
            style.fontSize = mFontSize.intValue;
            style.fontStyle = (FontStyle)mFontStyle.intValue;
            GUILayout.Label("预览Sample", style);
        }

        void saveAsset()
        {
            serializedObject.Update();
            var font = serializedObject.FindProperty("mFont");
            var fontStyle = serializedObject.FindProperty("mFontStyle");
            var size = serializedObject.FindProperty("mFontSize");
            font.objectReferenceValue = mFont.objectReferenceValue;
            fontStyle.enumValueIndex = mFontStyle.enumValueIndex;
            size.intValue = mFontSize.intValue;
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}
