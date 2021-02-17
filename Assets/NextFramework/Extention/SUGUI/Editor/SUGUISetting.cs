﻿using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using NextFramework.SUGUI;


public class SUGUISetting:IEditorPrefs
{
    #region Add UICompoment

    // Create Canvas
    public static Canvas AddCanvas(GameObject gameObject)
    {
        GameObject go = new GameObject("Canvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);

        go.AddComponent<GraphicRaycaster>();

        if (gameObject) go.transform.SetParent(gameObject.transform);
        UGUITools.ResetGameObject(go);

        CreateEventSystem();
        return canvas;
    }
    public static void CreateEventSystem()
    {
        var eventSys = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSys == null)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            obj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    // Create SText
    public static SText AddText(GameObject gameObject)
    {
        SText txt = UGUITools.AddChild<SText>(gameObject);
        txt.textStyle = textStyle;
        txt.text = "New Text";
        txt.color = Color.black;
        txt.rectTransform.sizeDelta = new Vector2(120, 200);
        UGUITools.ResetGameObject(txt.gameObject);

        return txt;
    }

    // Create SImage
    public static SImage AddImage(GameObject gameObject)
    {
        SImage image = UGUITools.AddChild<SImage>(gameObject);

        image.Atlas = atlas;
        image.SpriteName = partialSprite;
        UGUITools.ResetGameObject(image.gameObject);

        return image;
    }

    // Create SButton
    public static SButton AddButton(GameObject gameObject)
    {
        var img = AddImage(gameObject);
        var txt = AddText(img.gameObject);
        txt.rectTransform.anchorMin = Vector2.zero;
        txt.rectTransform.anchorMax = Vector2.one;
        txt.rectTransform.sizeDelta = Vector2.zero;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.textStyle = textStyle;
        img.gameObject.name = "SButton";
        UGUITools.ResetGameObject(img.gameObject);

        return img.gameObject.AddComponent<SButton>();
    }


    // Get Canvas
    public static Canvas GetCanvas(GameObject gameObject = null)
    {
        Canvas canvas = (gameObject != null) ? UGUITools.FindInParents<Canvas>(gameObject) : null;
        if (canvas == null)
            canvas = GameObject.FindObjectOfType<Canvas>();

        // If no root found, create one
        if (canvas == null) canvas = AddCanvas(gameObject);

        return canvas;
    }
    #endregion

    #region IEditorPrefs
    public void ReleaseEditorPrefs()
    {
        EditorPrefsHelper.DeleteKey("SUGUI_TextStyle");
        EditorPrefsHelper.DeleteKey("SUGUI_Atlas");
        EditorPrefsHelper.DeleteKey("SUGUI_Partial");
        EditorPrefsHelper.DeleteKey("SUGUI_Sprite");
    }
    #endregion

    #region SUGUI EditorPrefs
    /// <summary>
    /// TextStyle
    /// </summary>
    public static TextStyle textStyle
    {
        get { return EditorPrefsHelper.Get<TextStyle>("SUGUI_TextStyle", null); }
        set { EditorPrefsHelper.SetObject("SUGUI_TextStyle", value); }
    }
    /// <summary>
    /// SpriteAtlas
    /// </summary>
    public static SpriteAtlas atlas
    {
        get { return EditorPrefsHelper.Get<SpriteAtlas>("SUGUI_Atlas", null); }
        set { EditorPrefsHelper.SetObject("SUGUI_Atlas", value); }
    }
    /// <summary>
    /// partialSprite
    /// </summary>
    public static string partialSprite
    {
        get { return EditorPrefsHelper.GetString("SUGUI_Partial", null); }
        set { EditorPrefsHelper.SetString("SUGUI_Partial", value); }
    }

    /// <summary>
    /// selectedSprite
    /// </summary>
    public static string selectedSprite
    {
        get { return EditorPrefsHelper.GetString("SUGUI_Sprite", null); }
        set { EditorPrefsHelper.SetString("SUGUI_Sprite", value); }
    }

    #endregion
}

