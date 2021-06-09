using UnityEditor;
using UnityEngine;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.Collections.Generic;
using System.Reflection;

namespace NextFramework.UI
{
    public class SUGUIEditorTool
    {
        static Texture2D mBackdropTex;

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
        public static List<string> GetAllSprites(SpriteAtlas atlas, string match = "")
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
                    {
                        if (string.IsNullOrEmpty(match))
                            result.Add(sprite);
                        else
                        {
                            if (sprite.GetFileNameByPathWithoutExtention().Contains(match))
                                result.Add(sprite);
                        }
                    }
                }
            }
            return result;
        }

        public static void SelectSprite(string spriteName)
        {
            if (SUGUISetting.atlas != null)
            {
                SUGUISetting.selectedSprite = spriteName;
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

        public static GameObject SelectedRoot(bool createIfMissing)
        {
            GameObject go = Selection.activeGameObject;

            // Only use active objects
            if (go != null && !go.activeInHierarchy) go = null;

            // Try to find a panel
            Canvas canvas = (go != null) ? go.GetComponentInParent<Canvas>() : null;

            // No selection? Try to find the root automatically
            if (canvas == null)
            {
                Canvas[] cans = GameObject.FindObjectsOfType<Canvas>();
                if (cans.Length > 0) go = cans[0].gameObject;
            }

            if (createIfMissing && go == null)
            {
                // No object specified -- find the first panel
                if (go == null)
                {
                    Canvas can = GameObject.FindObjectOfType<Canvas>();
                    if (can != null) go = can.gameObject;
                }

                // No UI present -- create a new one
                if (go == null)
                {

                }
                //SUGUIMenu.CreateCanvas(null);
                //UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D);
            }
            return go;
        }

        /// <summary>
        /// Create an undo point for the specified objects.
        /// </summary>
        public static void RegisterUndo(string name, Object obj) { if (obj != null) UnityEditor.Undo.RecordObject(obj, name); }
        public static void RegisterUndo(string name, params Object[] objects) { if (objects != null && objects.Length > 0) UnityEditor.Undo.RecordObjects(objects, name); }

        #region Draw Field
        /// <summary>
        /// 按钮
        /// </summary>
        public static bool DrawPrefixButton(string text)
        {
            return DrawPrefixButton(text, GUILayout.Width(76f));
        }
        public static bool DrawPrefixButton(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, "DropDown", options);
        }

        /// <summary>
        /// 下拉框
        /// </summary>
        public static int DrawList(int index, string[] list, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(index, list, "DropDown", options);
        }
        public static int DrawList(string text, int index, string[] list, params GUILayoutOption[] options)
        {
            return EditorGUILayout.Popup(text, index, list, "DropDown", options);
        }

        public static void DrawObjectField<T>(SerializedProperty property, string text)
        {
            EditorGUILayout.ObjectField(property, typeof(T), new GUIContent(text));
        }

        public static void DrawAtlasSpriteField(SpriteAtlas atlas, string spriteName,
                                           System.Action<SpriteAtlas, string> callback,
                                           params GUILayoutOption[] options)
        {
            if (atlas == null) return;

            // Give the user a warning if there are no sprites in the atlas
            List<string> spriteList = GetAllSprites(atlas);
            if (spriteList.Count == 0)
            {
                EditorGUILayout.HelpBox("这张图集中并没有图片", MessageType.Warning);
                return;
            }

            // Sprite selection drop-down list
            GUILayout.BeginHorizontal();
            {
                if (DrawPrefixButton("Sprite"))
                {
                    SUGUISetting.atlas = atlas;
                    SUGUISetting.selectedSprite = spriteName;
                    SpriteSelector.Show(callback);
                }
                GUILayout.Label(spriteName, "HelpBox", GUILayout.Height(18f));

                if (GUILayout.Button("Edit", GUILayout.Width(60f)))
                {
                    Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(spriteName);
                    EditorApplication.ExecuteMenuItem("Window/2D/Sprite Editor");
                    Selection.activeObject = sp;
                }
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region  GUID->obj or obj->GUID
        /// <summary>
        /// 返回object的GUID
        /// </summary>
        public static string ObjectToGUID(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return (!string.IsNullOrEmpty(path)) ? AssetDatabase.AssetPathToGUID(path) : null;
        }

        static MethodInfo s_GetInstanceIDFromGUID;

        /// <summary>
        /// 通过GUID获取object
        /// </summary>
        public static Object GUIDToObject(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;

            if (s_GetInstanceIDFromGUID == null)
            {
                var type = typeof(AssetDatabase);

                // Unity 3, 4, 5 and 2017
                s_GetInstanceIDFromGUID = type.GetMethod("GetInstanceIDFromGUID", BindingFlags.Static | BindingFlags.NonPublic);

                // Unity 2018+
                if (s_GetInstanceIDFromGUID == null) s_GetInstanceIDFromGUID = type.GetMethod("GetMainAssetInstanceID", BindingFlags.Static | BindingFlags.NonPublic);
                if (s_GetInstanceIDFromGUID == null) return null;
            }

            int id = (int)s_GetInstanceIDFromGUID.Invoke(null, new object[] { guid });
            if (id != 0) return EditorUtility.InstanceIDToObject(id);
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return null;
            return AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        }

        /// <summary>
        /// 通过GUID获取指定类型object
        /// </summary>
        public static T GUIDToObject<T>(string guid) where T : Object
        {
            Object obj = GUIDToObject(guid);
            if (obj == null) return null;

            System.Type objType = obj.GetType();
            if (objType == typeof(T) || objType.IsSubclassOf(typeof(T))) return obj as T;

            if (objType == typeof(GameObject) && typeof(T).IsSubclassOf(typeof(Component)))
            {
                GameObject go = obj as GameObject;
                return go.GetComponent(typeof(T)) as T;
            }
            return null;
        }
        #endregion

        #region Get Property Value
        public static int GetIntPropertyValue(SerializedProperty property)
        {
            return property.intValue;
        }
        public static float GetFloatPropertyValue(SerializedProperty property)
        {
            return property.floatValue;
        }
        public static int GetEnumPropertyValue(SerializedProperty property)
        {
            return property.enumValueIndex;
        }

        public static bool GetBoolPropertyValue(SerializedProperty property)
        {
            return property.boolValue;
        }
        #endregion
    }
}

