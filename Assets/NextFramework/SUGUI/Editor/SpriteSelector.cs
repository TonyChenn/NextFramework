using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D;

/// <summary>
/// Editor component used to display a list of sprites.
/// </summary>

namespace NextFramework.SUGUI
{
    public class SpriteSelector : ScriptableWizard
    {
        public static SpriteSelector Singlton;

        void OnEnable() { Singlton = this; }
        void OnDisable() { Singlton = null; }

        public delegate void Callback(string spriteName);

        SerializedObject mObject;
        SerializedProperty mProperty;

        SImage mSprite;
        Vector2 mPos = Vector2.zero;
        Callback mCallback;
        float mClickTime = 0f;

        /// <summary>
        /// Draw the custom wizard.
        /// </summary>

        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80f;

            if (SUGUISetting.atlas == null)
            {
                GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
            }
            else
            {
                SpriteAtlas atlas = SUGUISetting.atlas;
                bool close = false;
                GUILayout.Label(atlas.name + " Sprites", "LODLevelNotifyText");
                //NGUIEditorTools.DrawSeparator();

                GUILayout.BeginHorizontal();
                GUILayout.Space(84f);

                string before = SUGUISetting.partialSprite;
                string after = EditorGUILayout.TextField("", before, "SearchTextField");
                if (before != after) SUGUISetting.partialSprite = after;

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    SUGUISetting.partialSprite = "";
                    GUIUtility.keyboardControl = 0;
                }
                GUILayout.Space(84f);
                GUILayout.EndHorizontal();

                List<string> spritePathList = SUGUIEditorTool.GetAllSprites(atlas);

                float size = 80f;
                float padded = size + 10f;
                int columns = Mathf.FloorToInt(Screen.width / padded);
                if (columns < 1) columns = 1;

                int offset = 0;
                Rect rect = new Rect(10f, 0, size, size);

                GUILayout.Space(10f);
                mPos = GUILayout.BeginScrollView(mPos);
                int rows = 1;

                while (offset < spritePathList.Count)
                {
                    GUILayout.BeginHorizontal();
                    {
                        int col = 0;
                        rect.x = 10f;

                        for (; offset < spritePathList.Count; ++offset)
                        {
                            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePathList[offset]);
                            //UISpriteData sprite = atlas.GetSprite(spritePathList[offset]);
                            if (texture == null) continue;

                            // Button comes first
                            if (GUI.Button(rect, ""))
                            {
                                if (Event.current.button == 0)
                                {
                                    float delta = Time.realtimeSinceStartup - mClickTime;
                                    mClickTime = Time.realtimeSinceStartup;

                                    if (SUGUISetting.selectedSprite != texture.name)
                                    {
                                        if (mSprite != null)
                                        {
                                            SUGUIEditorTool.RegisterUndo("Atlas Selection", mSprite);
                                            mSprite.SetNativeSize();
                                            EditorUtility.SetDirty(mSprite.gameObject);
                                        }

                                        SUGUISetting.selectedSprite = texture.name;
                                        if(Singlton!=null)
                                        {
                                            Singlton.Repaint();
                                        }
                                        if (mCallback != null)
                                        {
                                            mCallback(texture.name);
                                        }
                                    }
                                    else if (delta < 0.5f) close = true;
                                }
                                else
                                {
                                    //NGUIContextMenu.AddItem("Edit", false, EditSprite, sprite);
                                    //NGUIContextMenu.AddItem("Delete", false, DeleteSprite, sprite);
                                    //NGUIContextMenu.Show();
                                }
                            }

                            if (Event.current.type == EventType.Repaint)
                            {
                                SUGUIEditorTool.DrawTiledTexture(rect, SUGUIEditorTool.backdropTexture);
                                Rect clipRect = rect;


                                GUI.DrawTexture(clipRect, texture);
                                //GUI.DrawTextureWithTexCoords(clipRect, tex, uv);

                                // Draw the selection
                                if (SUGUISetting.selectedSprite == texture.name)
                                {
                                    SUGUIEditorTool.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                                }
                            }

                            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                            GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                            GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), texture.name, "ProgressBarBack");
                            GUI.contentColor = Color.white;
                            GUI.backgroundColor = Color.white;

                            if (++col >= columns)
                            {
                                ++offset;
                                break;
                            }
                            rect.x += padded;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(padded);
                    rect.y += padded + 26;
                    ++rows;
                }
                GUILayout.Space(rows * 26);
                GUILayout.EndScrollView();

                if (close) Close();
            }
        }

        /// <summary>
        /// Property-based selection result.
        /// </summary>

        void OnSpriteSelection(string sp)
        {
            if (mObject != null && mProperty != null)
            {
                mObject.Update();
                mProperty.stringValue = sp;
                mObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Show the sprite selection wizard.
        /// </summary>

        public static void ShowSelected()
        {
            if (SUGUISetting.atlas != null)
            {
                Show(delegate (string sel) { SUGUIEditorTool.SelectSprite(sel); });
            }
        }

        /// <summary>
        /// Show the sprite selection wizard.
        /// </summary>

        public static void Show(SerializedObject ob, SerializedProperty pro, SpriteAtlas atlas)
        {
            if (Singlton != null)
            {
                Singlton.Close();
                Singlton = null;
            }

            if (ob != null && pro != null && atlas != null)
            {
                SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
                SUGUISetting.atlas = atlas;
                SUGUISetting.selectedSprite = pro.hasMultipleDifferentValues ? null : pro.stringValue;
                comp.mSprite = null;
                comp.mObject = ob;
                comp.mProperty = pro;
                comp.mCallback = comp.OnSpriteSelection;
            }
        }

        /// <summary>
        /// Show the selection wizard.
        /// </summary>

        public static void Show(Callback callback)
        {
            if (Singlton != null)
            {
                Singlton.Close();
                Singlton = null;
            }

            SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("Select a Sprite");
            comp.mSprite = null;
            comp.mCallback = callback;
        }
    }
}

