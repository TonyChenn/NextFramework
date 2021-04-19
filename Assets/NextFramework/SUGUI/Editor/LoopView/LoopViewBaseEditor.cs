using UnityEditor;
using UnityEngine;

namespace NextFramework.SUGUI
{
    [CustomEditor(typeof(LoopViewBase), true)]
    public class LoopViewBaseEditor : Editor
    {
        GameObject Current;
        UnityEngine.UI.ScrollRect scrollRect;

        SerializedProperty m_VerticalMove;
        SerializedProperty m_HorArrangeType;
        SerializedProperty m_VerArrangeType;

        SerializedProperty m_ShowScrollBar;
        SerializedProperty m_ScrollBar;


        private void OnEnable()
        {
            Current = ((LoopViewBase)target).gameObject;
            scrollRect = Current.GetComponent<UnityEngine.UI.ScrollRect>();

            m_VerticalMove = serializedObject.FindProperty("m_VerticalMove");
            m_HorArrangeType = serializedObject.FindProperty("m_HorArrangeType");
            m_VerArrangeType = serializedObject.FindProperty("m_VerArrangeType");

            m_ShowScrollBar = serializedObject.FindProperty("m_ShowScrollBar");
            m_ScrollBar = serializedObject.FindProperty("m_ScrollBar");
        }
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_VerticalMove);
            bool verticalMove = SUGUIEditorTool.GetBoolPropertyValue(m_VerticalMove);
            if (verticalMove)
                EditorGUILayout.PropertyField(m_VerArrangeType);
            else
                EditorGUILayout.PropertyField(m_HorArrangeType);
            scrollRect.horizontal = !verticalMove;
            scrollRect.vertical = verticalMove;


            EditorGUILayout.PropertyField(m_ShowScrollBar);
            bool showScrollBar = SUGUIEditorTool.GetBoolPropertyValue(m_ShowScrollBar);

            if (!showScrollBar)
            {
                scrollRect.horizontalScrollbar = null;
                scrollRect.verticalScrollbar = null;

                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.PropertyField(m_ScrollBar);
            Object scroll_bar = m_ScrollBar.objectReferenceValue;

            if (scroll_bar == null)
            {
                scrollRect.horizontalScrollbar = null;
                scrollRect.verticalScrollbar = null;

                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (m_VerticalMove.boolValue)
            {
                scrollRect.verticalScrollbar = (UnityEngine.UI.Scrollbar)scroll_bar;
                scrollRect.verticalScrollbarVisibility = UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHide;

                scrollRect.horizontalScrollbar = null;
            }
            else
            {
                scrollRect.verticalScrollbar = null;

                scrollRect.horizontalScrollbar = (UnityEngine.UI.Scrollbar)scroll_bar;
                scrollRect.horizontalScrollbarVisibility = UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHide;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}

