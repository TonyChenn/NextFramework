using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D;
using System;

/// <summary>
/// Editor component used to display a list of sprites.
/// </summary>

namespace NextFramework.UI
{
    public class SpriteSelector : ScriptableWizard
    {
        Action<SpriteAtlas, string> mCallback;


        SerializedObject mObject;
        SerializedProperty mProperty;

        SImage mSprite;
        Vector2 mPos = Vector2.zero;
        float mClickTime = 0f;

        /// <summary>
        /// Draw the custom wizard.
        /// </summary>

        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80f;

            if (SUGUISetting.atlas == null)
            {
                GUILayout.Label("没有选择图集", "LODLevelNotifyText");
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
                //Debug.Log("----->before:" + before);
                string after = EditorGUILayout.TextField("", before, "SearchTextField");
                //Debug.Log("----->after:" + after);
                if (before != after) SUGUISetting.partialSprite = after;

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    SUGUISetting.partialSprite = "";
                    GUIUtility.keyboardControl = 0;
                }
                GUILayout.Space(84f);
                GUILayout.EndHorizontal();

                List<string> spritePathList = SUGUIEditorTool.GetAllSprites(atlas, SUGUISetting.partialSprite);

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
                            if (texture == null) continue;

                            // Button comes first
                            if (GUI.Button(rect, ""))
                            {
                                if (Event.current.button == 0)
                                {
                                    float delta = Time.realtimeSinceStartup - mClickTime;
                                    mClickTime = Time.realtimeSinceStartup;

                                    string spriteName = spritePathList[offset].GetFileNameByPath();
                                    if (SUGUISetting.selectedSprite != spriteName)
                                    {
                                        if (mSprite != null)
                                        {
                                            SUGUIEditorTool.RegisterUndo("Atlas Selection", mSprite);
                                            mSprite.SetNativeSize();
                                            EditorUtility.SetDirty(mSprite.gameObject);
                                        }

                                        SUGUISetting.selectedSprite = spriteName;
                                        if (mCallback != null)
                                        {
                                            mCallback(atlas, spriteName);
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
                                if (SUGUISetting.selectedSprite.GetFileNameByPathWithoutExtention() == texture.name)
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

        void OnSpriteSelection(SpriteAtlas atlas, string sp)
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
                Show(delegate (SpriteAtlas atlas, string sel) { SUGUIEditorTool.SelectSprite(sel); });
            }
        }

        /// <summary>
        /// Show the sprite selection wizard.
        /// </summary>

        public static void Show(SerializedObject ob, SerializedProperty pro, SpriteAtlas atlas)
        {
            if (ob != null && pro != null && atlas != null)
            {
                SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("精灵选择");
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

        public static void Show(Action<SpriteAtlas, string> callback)
        {
            SpriteSelector comp = ScriptableWizard.DisplayWizard<SpriteSelector>("精灵选择");
            comp.mSprite = null;
            comp.mCallback = callback;
        }
    }
}

