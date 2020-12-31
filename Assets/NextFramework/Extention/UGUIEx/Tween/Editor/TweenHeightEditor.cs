//-------------------------------------------------
//            TweenKit
// Copyright © 2020 tonychenn.cn
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace NextFramework
{
    [CustomEditor(typeof(TweenHeight))]
    public class TweenHeightEditor : UITweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            TweenKitEditorTools.SetLabelWidth(120f);

            TweenHeight tw = target as TweenHeight;
            GUI.changed = false;

            int from = EditorGUILayout.IntField("From", tw.from);
            int to = EditorGUILayout.IntField("To", tw.to);

            if (from < 0) from = 0;
            if (to < 0) to = 0;

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

