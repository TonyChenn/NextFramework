using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace NextFramework.UI
{
    public class AtlasSelector : ScriptableWizard
    {
        Vector2 pos = Vector2.zero;
        public Action<SpriteAtlas> SelectAtlasFunc;


        public static void ShowWind(Action<SpriteAtlas> selectAtlas)
        {
            AtlasSelector window = DisplayWizard<AtlasSelector>("图集选择");
            window.minSize = new Vector2(800, 500);

            window.SelectAtlasFunc = selectAtlas;
        }

        private void OnGUI()
        {
            GUIStyle style = GUISkinHelper.TextStyle(UnityEngine.TextAnchor.MiddleCenter, Color.white, 23, UnityEngine.FontStyle.Bold);
            GUIStyle titleStyle = GUISkinHelper.TextStyle(Color.black, FontStyle.Bold);
            GUIStyle itemStyle = GUISkinHelper.TextStyle(Color.red, FontStyle.Bold);

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("图集", style);

            var infos = FileHelper.GetAllFiles(PathConfig.UIAtlasFolder, "*.spriteatlas", System.IO.SearchOption.TopDirectoryOnly);
            pos = EditorGUILayout.BeginScrollView(pos);
            if (infos.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("名称", titleStyle, GUILayout.Width(200));
                EditorGUILayout.LabelField("路径", titleStyle);
                EditorGUILayout.LabelField("选择", titleStyle, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                for (int i = 0, iMax = infos.Length; i < iMax; i++)
                {
                    string path = infos[i].FullName.GetRelativePath();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(infos[i].Name.Replace(".spriteatlas", ""), itemStyle, GUILayout.Width(200));
                    EditorGUILayout.LabelField(path, itemStyle);
                    if (GUILayout.Button("选择", GUILayout.Width(100)))
                    {
                        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                        if (SelectAtlasFunc != null && atlas)
                            SelectAtlasFunc(atlas);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
