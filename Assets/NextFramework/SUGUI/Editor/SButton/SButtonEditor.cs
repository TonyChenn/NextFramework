using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace NextFramework.SUGUI
{
    [CustomEditor(typeof(SButton), true)]
    [CanEditMultipleObjects]
    public class SButtonEditor : SelectableEditor
    {
        GameObject Current;
        ButtonScale BtnScaleCompoment;

        SerializedProperty m_TweenScale;

        SerializedProperty m_OnClickProperty;
        //SerializedProperty m_OnDoubleClickProperty;
        //SerializedProperty m_OnLongClickProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            Current = ((SButton)target).gameObject;
            BtnScaleCompoment = Current.GetComponent<ButtonScale>();
            if (BtnScaleCompoment == null)
                BtnScaleCompoment = Current.AddComponent<ButtonScale>();


            m_TweenScale = serializedObject.FindProperty("m_TweenScale");

            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            //m_OnDoubleClickProperty = serializedObject.FindProperty("m_OnDoubleClick");
            //m_OnLongClickProperty = serializedObject.FindProperty("m_OnLongClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_TweenScale);
            bool tween = SUGUIEditorTool.GetBoolPropertyValue(m_TweenScale);

            if (BtnScaleCompoment != null)
                BtnScaleCompoment.enabled = tween;

            EditorGUILayout.PropertyField(m_OnClickProperty);
            //EditorGUILayout.PropertyField(m_OnDoubleClickProperty);
            //EditorGUILayout.PropertyField(m_OnLongClickProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

