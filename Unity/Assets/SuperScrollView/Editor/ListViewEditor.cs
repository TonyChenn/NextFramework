using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NextFramework.UI
{
    [CustomEditor(typeof(ListView))]
    public class ListViewEditor : Editor
    {
        SerializedProperty mArrangeType;
        SerializedProperty mShowScrollBar;
        SerializedProperty mPrefabData;
        
        private void OnEnable()
        {
            mArrangeType = serializedObject.FindProperty("m_ArrangeType");
            mShowScrollBar = serializedObject.FindProperty("m_ShowScrollBar");
            mPrefabData = serializedObject.FindProperty("m_PrefabData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mArrangeType);
            EditorGUILayout.PropertyField(mShowScrollBar);
            EditorGUILayout.PropertyField(mPrefabData);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

