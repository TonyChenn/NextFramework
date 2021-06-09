using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NextFramework.UI
{
    [CustomEditor(typeof(GridView))]
    public class GridViewEditor : Editor
    {
        SerializedProperty mArrangeType;
        SerializedProperty mShowScrollBar;
        SerializedProperty mPrefabData;
        SerializedProperty mPerRowCount;


        GUIContent HorizontalTip = new GUIContent("Row Count");
        GUIContent VerticalTip = new GUIContent("Colum Count");

        private void OnEnable()
        {
            mArrangeType = serializedObject.FindProperty("m_ArrangeType");
            mPerRowCount = serializedObject.FindProperty("m_PerRowCount");
            mShowScrollBar = serializedObject.FindProperty("m_ShowScrollBar");
            mPrefabData = serializedObject.FindProperty("m_PrefabData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mArrangeType);
            if (mArrangeType.enumValueIndex == (int)ArrangeType.TopToBottom || mArrangeType.intValue == (int)ArrangeType.BottomToTop)
                EditorGUILayout.PropertyField(mPerRowCount, VerticalTip);
            else
                EditorGUILayout.PropertyField(mPerRowCount, HorizontalTip);

            EditorGUILayout.PropertyField(mShowScrollBar);
            EditorGUILayout.PropertyField(mPrefabData);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

