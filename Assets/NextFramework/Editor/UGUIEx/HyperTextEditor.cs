using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(HyperText), true)]
[CanEditMultipleObjects]
public class HyperTextEditor : GraphicEditor
{
    GUIContent inputContent;
    GUIContent outputContent;
    GUIContent hyperColorContent;

    SerializedProperty _text;
    SerializedProperty m_Text;
    SerializedProperty m_FontData;
    SerializedProperty hyperTextColor;

    HyperText hyperText;
    string _lastText;

    protected override void OnEnable()
    {
        base.OnEnable();
        inputContent = new GUIContent("Input Text");
        outputContent = new GUIContent("Output Text");
        hyperColorContent = new GUIContent("Hyper Color");

        _text = serializedObject.FindProperty("_text");
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontData = serializedObject.FindProperty("m_FontData");
        hyperTextColor = serializedObject.FindProperty("hyperTextColor");

        hyperText = target as HyperText;
        _lastText = "";
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(hyperTextColor, hyperColorContent);
        EditorGUILayout.PropertyField(_text, inputContent);
        EditorGUILayout.PropertyField(m_Text, outputContent);
        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();

        //更新字符
        if (hyperText != null && _lastText != _text.stringValue)
        {
            hyperText.text = _text.stringValue;
            _lastText = _text.stringValue;
        }
    }
}
