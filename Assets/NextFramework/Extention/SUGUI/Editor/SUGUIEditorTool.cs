using UnityEditor;
using UnityEngine;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.Collections.Generic;

namespace NextFramework.SUGUI
{
    public static class SUGUIEditorTool
    {
        static Texture2D mBackdropTex;

        static string mLastSprite = null;
        static string mEditedName = null;

        /// <summary>
        /// 加载Asset
        /// </summary>
        public static T LoadAsset<T>(string path) where T : Object
        {
            Object obj = LoadAsset(path);
            if (obj == null) return null;

            T val = obj as T;
            if (val != null) return val;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                if (obj.GetType() == typeof(GameObject))
                {
                    GameObject go = obj as GameObject;
                    return go.GetComponent(typeof(T)) as T;
                }
            }
            return null;
        }
        public static Object LoadAsset(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return AssetDatabase.LoadMainAssetAtPath(path);
        }

        /// <summary>
        /// 获取SpriteAtlas中所有图片的相对地址
        /// </summary>
        public static List<string> GetAllSprites(SpriteAtlas atlas)
        {
            Object[] objs = SpriteAtlasExtensions.GetPackables(atlas);
            List<string> result = new List<string>();
            for (int i = 0; i < objs.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(objs[i]);
                string[] sprite_guides = AssetDatabase.FindAssets("t:" + typeof(Sprite).Name, new string[] { path });
                foreach (var item in sprite_guides)
                {
                    string sprite = AssetDatabase.GUIDToAssetPath(item);
                    if (!result.Contains(sprite))
                        result.Add(sprite);
                }
            }
            return result;
        }

        /// <summary>
        /// Create an undo point for the specified objects.
        /// </summary>
        public static void RegisterUndo(string name, params Object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                Undo.RecordObjects(objects, name);

                foreach (Object obj in objects)
                {
                    if (obj == null) continue;
                    EditorUtility.SetDirty(obj);
                }
            }
        }

        /// <summary>
        /// Returns a usable texture that looks like a dark checker board.
        /// </summary>
        public static Texture2D backdropTexture
        {
            get
            {
                if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
                    new Color(0.1f, 0.1f, 0.1f, 0.5f),
                    new Color(0.2f, 0.2f, 0.2f, 0.5f));
                return mBackdropTex;
            }
        }
        /// <summary>
        /// Create a checker-background texture
        /// </summary>
        static Texture2D CreateCheckerTex(Color c0, Color c1)
        {
            Texture2D tex = new Texture2D(16, 16);
            tex.name = "[Generated] Checker Texture";
            tex.hideFlags = HideFlags.DontSave;

            for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
            for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
            for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
            for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

            tex.Apply();
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        /// <summary>
        /// Draws the tiled texture. Like GUI.DrawTexture() but tiled instead of stretched.
        /// </summary>
        public static void DrawTiledTexture(Rect rect, Texture tex)
        {
            GUI.BeginGroup(rect);
            {
                int width = Mathf.RoundToInt(rect.width);
                int height = Mathf.RoundToInt(rect.height);

                for (int y = 0; y < height; y += tex.height)
                {
                    for (int x = 0; x < width; x += tex.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                    }
                }
            }
            GUI.EndGroup();
        }

        /// <summary>
        /// Returns a blank usable 1x1 white texture.
        /// </summary>
        public static Texture2D blankTexture { get { return EditorGUIUtility.whiteTexture; } }
        /// <summary>
        /// Draw a single-pixel outline around the specified rectangle.
        /// </summary>
        public static void DrawOutline(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = blankTexture;
                GUI.color = color;
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        public static void DrawAdvancedSpriteField(SpriteAtlas atlas, string spriteName,
                                                   SpriteSelector.Callback callback,
                                                   params GUILayoutOption[] options)
        {
            if (atlas == null) return;

            // Give the user a warning if there are no sprites in the atlas
            List<string> spriteList = GetAllSprites(atlas);
            if (spriteList.Count == 0)
            {
                EditorGUILayout.HelpBox("No sprites found", MessageType.Warning);
                return;
            }

            // Sprite selection drop-down list
            GUILayout.BeginHorizontal();
            {
                if (SUGUIEditorTool.DrawPrefixButton("Sprite"))
                {
                    SUGUISetting.atlas = atlas;
                    SUGUISetting.selectedSprite = spriteName;
                    SpriteSelector.Show(callback);
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label(spriteName, "HelpBox", GUILayout.Height(18f));
                //NGUIEditorTools.DrawPadding();
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }


        static public bool DrawPrefixButton(string text)
        {
            return GUILayout.Button(text, "DropDown", GUILayout.Width(76f));
        }
    }
}

