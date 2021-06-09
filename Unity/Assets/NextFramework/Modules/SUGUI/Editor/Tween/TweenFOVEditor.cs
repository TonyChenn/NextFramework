//-------------------------------------------------
//            TweenKit
// Copyright © 2020 tonychenn.cn
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace NextFramework.UI
{
    [CustomEditor(typeof(TweenFOV))]
    public class TweenFOVEditor : UITweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            TweenKitEditorTools.SetLabelWidth(120f);

            TweenFOV tw = target as TweenFOV;
            GUI.changed = false;

            float from = EditorGUILayout.Slider("From", tw.from, 1f, 180f);
            float to = EditorGUILayout.Slider("To", tw.to, 1f, 180f);

            if (GUI.changed)
            {
                TweenKitEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                TweenKitTools.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }
}

