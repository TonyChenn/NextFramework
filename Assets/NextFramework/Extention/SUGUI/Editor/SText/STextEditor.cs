using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    [CustomEditor(typeof(SText), true)]
    [CanEditMultipleObjects]
    public class STextEditor : GraphicEditor
    {
        protected SerializedProperty m_Text;
        protected SerializedProperty m_FontData;
        protected FontDataEditor fontDrawer;

        SerializedProperty m_Style;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            fontDrawer = new FontDataEditor();
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_Style = serializedObject.FindProperty("m_Style");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // Text
            EditorGUILayout.PropertyField(m_Text);
            //TextStyle
            SUGUIEditorTool.DrawObjectField<TextStyle>(m_Style, "Text Style");
            // Material & Color
            AppearanceControlsGUI();
            // Raycast Target
            RaycastControlsGUI();

            EditorGUILayout.Space();
            // Character & Paragraph
            DrawFont();

            serializedObject.ApplyModifiedProperties();

        }

        // Character & Paragraph
        public virtual void DrawFont()
        {
            float h = fontDrawer.GetPropertyHeight(m_FontData, new GUIContent(""));
            Rect rect = EditorGUILayout.GetControlRect(false, h);
            fontDrawer.OnGUI(rect, m_FontData, new GUIContent(""));
        }
    }

    public class FontDataEditor
    {
        static protected class Styles
        {
            public static GUIStyle alignmentButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            public static GUIStyle alignmentButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
            public static GUIStyle alignmentButtonRight = new GUIStyle(EditorStyles.miniButtonRight);

            public static GUIContent m_EncodingContent;

            public static GUIContent m_LeftAlignText;
            public static GUIContent m_CenterAlignText;
            public static GUIContent m_RightAlignText;
            public static GUIContent m_TopAlignText;
            public static GUIContent m_MiddleAlignText;
            public static GUIContent m_BottomAlignText;

            public static GUIContent m_LeftAlignTextActive;
            public static GUIContent m_CenterAlignTextActive;
            public static GUIContent m_RightAlignTextActive;
            public static GUIContent m_TopAlignTextActive;
            public static GUIContent m_MiddleAlignTextActive;
            public static GUIContent m_BottomAlignTextActive;

            static Styles()
            {
                m_EncodingContent = EditorGUIUtility.TrTextContent("Rich Text", "Use emoticons and colors");

                // Horizontal Alignment Icons
                m_LeftAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left", "Left Align");
                m_CenterAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center", "Center Align");
                m_RightAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right", "Right Align");
                m_LeftAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left_active", "Left Align");
                m_CenterAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center_active", "Center Align");
                m_RightAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right_active", "Right Align");

                // Vertical Alignment Icons
                m_TopAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top", "Top Align");
                m_MiddleAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center", "Middle Align");
                m_BottomAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom", "Bottom Align");
                m_TopAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top_active", "Top Align");
                m_MiddleAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center_active", "Middle Align");
                m_BottomAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom_active", "Bottom Align");

                FixAlignmentButtonStyles(alignmentButtonLeft, alignmentButtonMid, alignmentButtonRight);
            }

            static void FixAlignmentButtonStyles(params GUIStyle[] styles)
            {
                foreach (GUIStyle style in styles)
                {
                    style.padding.left = 2;
                    style.padding.right = 2;
                }
            }
        }

        private enum VerticalTextAlignment
        {
            Top,
            Middle,
            Bottom
        }

        private enum HorizontalTextAlignment
        {
            Left,
            Center,
            Right
        }

        protected const int kAlignmentButtonWidth = 20;

        static int s_TextAlignmentHash = "DoTextAligmentControl".GetHashCode();

        private SerializedProperty m_SupportEncoding;
        private SerializedProperty m_Font;
        private SerializedProperty m_FontSize;
        private SerializedProperty m_LineSpacing;
        private SerializedProperty m_FontStyle;
        private SerializedProperty m_ResizeTextForBestFit;
        private SerializedProperty m_ResizeTextMinSize;
        private SerializedProperty m_ResizeTextMaxSize;
        private SerializedProperty m_HorizontalOverflow;
        private SerializedProperty m_VerticalOverflow;
        private SerializedProperty m_Alignment;
        protected SerializedProperty m_AlignByGeometry;

        //private float m_FontFieldfHeight = 0f;
        //private float m_FontStyleHeight = 0f;
        //private float m_FontSizeHeight = 0f;
        private float m_LineSpacingHeight = 0f;
        protected float m_EncodingHeight = 0f;
        protected float m_ResizeTextForBestFitHeight = 0f;
        private float m_ResizeTextMinSizeHeight = 0f;
        private float m_ResizeTextMaxSizeHeight = 0f;
        private float m_HorizontalOverflowHeight = 0f;
        private float m_VerticalOverflowHeight = 0f;
        private float m_AlignByGeometryHeight = 0f;

        public virtual void Init(SerializedProperty property)
        {
            m_SupportEncoding = property.FindPropertyRelative("m_RichText");
            m_Font = property.FindPropertyRelative("m_Font");
            m_FontSize = property.FindPropertyRelative("m_FontSize");
            m_LineSpacing = property.FindPropertyRelative("m_LineSpacing");
            m_FontStyle = property.FindPropertyRelative("m_FontStyle");
            m_ResizeTextForBestFit = property.FindPropertyRelative("m_BestFit");
            m_ResizeTextMinSize = property.FindPropertyRelative("m_MinSize");
            m_ResizeTextMaxSize = property.FindPropertyRelative("m_MaxSize");
            m_HorizontalOverflow = property.FindPropertyRelative("m_HorizontalOverflow");
            m_VerticalOverflow = property.FindPropertyRelative("m_VerticalOverflow");
            m_Alignment = property.FindPropertyRelative("m_Alignment");
            m_AlignByGeometry = property.FindPropertyRelative("m_AlignByGeometry");
        }

        public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);
            //m_FontFieldfHeight = EditorGUI.GetPropertyHeight(m_Font);
            //m_FontStyleHeight = EditorGUI.GetPropertyHeight(m_FontStyle);
            //m_FontSizeHeight = EditorGUI.GetPropertyHeight(m_FontSize);
            m_LineSpacingHeight = EditorGUI.GetPropertyHeight(m_LineSpacing);
            m_EncodingHeight = EditorGUI.GetPropertyHeight(m_SupportEncoding);
            m_ResizeTextForBestFitHeight = EditorGUI.GetPropertyHeight(m_ResizeTextForBestFit);
            m_ResizeTextMinSizeHeight = EditorGUI.GetPropertyHeight(m_ResizeTextMinSize);
            m_ResizeTextMaxSizeHeight = EditorGUI.GetPropertyHeight(m_ResizeTextMaxSize);
            m_HorizontalOverflowHeight = EditorGUI.GetPropertyHeight(m_HorizontalOverflow);
            m_VerticalOverflowHeight = EditorGUI.GetPropertyHeight(m_VerticalOverflow);
            m_AlignByGeometryHeight = EditorGUI.GetPropertyHeight(m_AlignByGeometry);

            var height = m_LineSpacingHeight
                + m_EncodingHeight
                + m_ResizeTextForBestFitHeight
                + m_HorizontalOverflowHeight
                + m_VerticalOverflowHeight
                + EditorGUIUtility.singleLineHeight * 3
                + EditorGUIUtility.standardVerticalSpacing * 10
                + m_AlignByGeometryHeight;

            if (m_ResizeTextForBestFit.boolValue)
            {
                height += m_ResizeTextMinSizeHeight
                    + m_ResizeTextMaxSizeHeight
                    + EditorGUIUtility.standardVerticalSpacing * 2;
            }
            return height;
        }

        public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(rect, "Character", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            ++EditorGUI.indentLevel;
            {
                rect.height = m_LineSpacingHeight;
                EditorGUI.PropertyField(rect, m_LineSpacing);

                OnDrawRichText(ref rect);
            }
            --EditorGUI.indentLevel;

            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, "Paragraph", EditorStyles.boldLabel);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            ++EditorGUI.indentLevel;
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                DoTextAligmentControl(rect, m_Alignment);

                OnDrawOtherAligmentControl(ref rect);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_HorizontalOverflowHeight;
                OnDrawAlignByGeometry(rect);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_HorizontalOverflowHeight;
                EditorGUI.PropertyField(rect, m_HorizontalOverflow);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_VerticalOverflowHeight;
                EditorGUI.PropertyField(rect, m_VerticalOverflow);

                OnDrawBestFit(ref rect);
            }
            --EditorGUI.indentLevel;
        }

        public virtual void OnDrawRichText(ref Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = m_EncodingHeight;
            EditorGUI.PropertyField(rect, m_SupportEncoding, Styles.m_EncodingContent);
        }

        public virtual void OnDrawAlignByGeometry(Rect rect)
        {
            EditorGUI.PropertyField(rect, m_AlignByGeometry);
        }

        public virtual void OnDrawOtherAligmentControl(ref Rect rect)
        {

        }

        public virtual void OnDrawBestFit(ref Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = m_ResizeTextMaxSizeHeight;
            EditorGUI.PropertyField(rect, m_ResizeTextForBestFit);

            if (m_ResizeTextForBestFit.boolValue)
            {
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_ResizeTextMinSizeHeight;
                EditorGUI.PropertyField(rect, m_ResizeTextMinSize);

                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = m_ResizeTextMaxSizeHeight;
                EditorGUI.PropertyField(rect, m_ResizeTextMaxSize);
            }
        }

        private void DoTextAligmentControl(Rect position, SerializedProperty alignment)
        {
            GUIContent alingmentContent = EditorGUIUtility.TrTextContent("Alignment");

            int id = EditorGUIUtility.GetControlID(s_TextAlignmentHash, FocusType.Keyboard, position);

            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            EditorGUI.BeginProperty(position, alingmentContent, alignment);
            {
                Rect controlArea = EditorGUI.PrefixLabel(position, id, alingmentContent);

                float width = kAlignmentButtonWidth * 3;
                float spacing = Mathf.Clamp(controlArea.width - width * 2, 2, 10);

                Rect horizontalAligment = new Rect(controlArea.x, controlArea.y, width, controlArea.height);
                Rect verticalAligment = new Rect(horizontalAligment.xMax + spacing, controlArea.y, width, controlArea.height);

                DoHorizontalAligmentControl(horizontalAligment, alignment);
                DoVerticalAligmentControl(verticalAligment, alignment);
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        private static void DoHorizontalAligmentControl(Rect position, SerializedProperty alignment)
        {
            TextAnchor ta = (TextAnchor)alignment.intValue;
            HorizontalTextAlignment horizontalAlignment = GetHorizontalAlignment(ta);

            bool leftAlign = (horizontalAlignment == HorizontalTextAlignment.Left);
            bool centerAlign = (horizontalAlignment == HorizontalTextAlignment.Center);
            bool rightAlign = (horizontalAlignment == HorizontalTextAlignment.Right);

            if (alignment.hasMultipleDifferentValues)
            {
                foreach (var obj in alignment.serializedObject.targetObjects)
                {
                    Text text = obj as Text;
                    horizontalAlignment = GetHorizontalAlignment(text.alignment);
                    leftAlign = leftAlign || (horizontalAlignment == HorizontalTextAlignment.Left);
                    centerAlign = centerAlign || (horizontalAlignment == HorizontalTextAlignment.Center);
                    rightAlign = rightAlign || (horizontalAlignment == HorizontalTextAlignment.Right);
                }
            }

            position.width = kAlignmentButtonWidth;

            EditorGUI.BeginChangeCheck();
            EditorToggle(position, leftAlign, leftAlign ? Styles.m_LeftAlignTextActive : Styles.m_LeftAlignText, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAlignment.Left);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, centerAlign, centerAlign ? Styles.m_CenterAlignTextActive : Styles.m_CenterAlignText, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAlignment.Center);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, rightAlign, rightAlign ? Styles.m_RightAlignTextActive : Styles.m_RightAlignText, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetHorizontalAlignment(alignment, HorizontalTextAlignment.Right);
            }
        }

        private static void DoVerticalAligmentControl(Rect position, SerializedProperty alignment)
        {
            TextAnchor ta = (TextAnchor)alignment.intValue;
            VerticalTextAlignment verticalTextAlignment = GetVerticalAlignment(ta);

            bool topAlign = (verticalTextAlignment == VerticalTextAlignment.Top);
            bool middleAlign = (verticalTextAlignment == VerticalTextAlignment.Middle);
            bool bottomAlign = (verticalTextAlignment == VerticalTextAlignment.Bottom);

            if (alignment.hasMultipleDifferentValues)
            {
                foreach (var obj in alignment.serializedObject.targetObjects)
                {
                    Text text = obj as Text;
                    TextAnchor textAlignment = text.alignment;
                    verticalTextAlignment = GetVerticalAlignment(textAlignment);
                    topAlign = topAlign || (verticalTextAlignment == VerticalTextAlignment.Top);
                    middleAlign = middleAlign || (verticalTextAlignment == VerticalTextAlignment.Middle);
                    bottomAlign = bottomAlign || (verticalTextAlignment == VerticalTextAlignment.Bottom);
                }
            }


            position.width = kAlignmentButtonWidth;

            // position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, topAlign, topAlign ? Styles.m_TopAlignTextActive : Styles.m_TopAlignText, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAlignment.Top);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, middleAlign, middleAlign ? Styles.m_MiddleAlignTextActive : Styles.m_MiddleAlignText, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAlignment.Middle);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, bottomAlign, bottomAlign ? Styles.m_BottomAlignTextActive : Styles.m_BottomAlignText, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetVerticalAlignment(alignment, VerticalTextAlignment.Bottom);
            }
        }

        protected static bool EditorToggle(Rect position, bool value, GUIContent content, GUIStyle style)
        {
            int hashCode = "AlignToggle".GetHashCode();
            int id = EditorGUIUtility.GetControlID(hashCode, FocusType.Keyboard, position);
            Event evt = Event.current;

            // Toggle selected toggle on space or return key
            if (EditorGUIUtility.keyboardControl == id && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
            {
                value = !value;
                evt.Use();
                GUI.changed = true;
            }

            if (evt.type == EventType.KeyDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = id;
                EditorGUIUtility.editingTextField = false;
                HandleUtility.Repaint();
            }

            bool returnValue = GUI.Toggle(position, id, value, content, style);

            return returnValue;
        }

        private static HorizontalTextAlignment GetHorizontalAlignment(TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                case TextAnchor.LowerCenter:
                    return HorizontalTextAlignment.Center;

                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    return HorizontalTextAlignment.Right;

                case TextAnchor.UpperLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.LowerLeft:
                    return HorizontalTextAlignment.Left;
            }

            return HorizontalTextAlignment.Left;
        }

        private static VerticalTextAlignment GetVerticalAlignment(TextAnchor ta)
        {
            switch (ta)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    return VerticalTextAlignment.Top;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return VerticalTextAlignment.Middle;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return VerticalTextAlignment.Bottom;
            }

            return VerticalTextAlignment.Top;
        }

        // We can't go through serialized properties here since we're showing two controls for a single SerializzedProperty.
        private static void SetHorizontalAlignment(SerializedProperty alignment, HorizontalTextAlignment horizontalAlignment)
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                Text text = obj as Text;
                VerticalTextAlignment currentVerticalAlignment = GetVerticalAlignment(text.alignment);
                Undo.RecordObject(text, "Horizontal Alignment");
                text.alignment = GetAnchor(currentVerticalAlignment, horizontalAlignment);
                EditorUtility.SetDirty(obj);
            }
        }

        private static void SetVerticalAlignment(SerializedProperty alignment, VerticalTextAlignment verticalAlignment)
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                Text text = obj as Text;
                HorizontalTextAlignment currentHorizontalAlignment = GetHorizontalAlignment(text.alignment);
                Undo.RecordObject(text, "Vertical Alignment");
                text.alignment = GetAnchor(verticalAlignment, currentHorizontalAlignment);
                EditorUtility.SetDirty(obj);
            }
        }

        private static TextAnchor GetAnchor(VerticalTextAlignment verticalTextAlignment, HorizontalTextAlignment horizontalTextAlignment)
        {
            TextAnchor ac = TextAnchor.UpperLeft;

            switch (horizontalTextAlignment)
            {
                case HorizontalTextAlignment.Left:
                    switch (verticalTextAlignment)
                    {
                        case VerticalTextAlignment.Bottom:
                            ac = TextAnchor.LowerLeft;
                            break;
                        case VerticalTextAlignment.Middle:
                            ac = TextAnchor.MiddleLeft;
                            break;
                        default:
                            ac = TextAnchor.UpperLeft;
                            break;
                    }
                    break;
                case HorizontalTextAlignment.Center:
                    switch (verticalTextAlignment)
                    {
                        case VerticalTextAlignment.Bottom:
                            ac = TextAnchor.LowerCenter;
                            break;
                        case VerticalTextAlignment.Middle:
                            ac = TextAnchor.MiddleCenter;
                            break;
                        default:
                            ac = TextAnchor.UpperCenter;
                            break;
                    }
                    break;
                default:
                    switch (verticalTextAlignment)
                    {
                        case VerticalTextAlignment.Bottom:
                            ac = TextAnchor.LowerRight;
                            break;
                        case VerticalTextAlignment.Middle:
                            ac = TextAnchor.MiddleRight;
                            break;
                        default:
                            ac = TextAnchor.UpperRight;
                            break;
                    }
                    break;
            }
            return ac;
        }
    }
}

