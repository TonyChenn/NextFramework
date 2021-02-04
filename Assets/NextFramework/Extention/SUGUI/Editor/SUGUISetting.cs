using NextFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SUGUISetting : IEditorPrefs
{
    public static SpriteAtlas atlas
    {
        get { return EditorPrefsHelper.Get<SpriteAtlas>("SUGUI_Atlas", null); }
        set { EditorPrefsHelper.SetObject("SUGUI_Atlas", value); }
    }
    public static string partialSprite
    {
        get { return EditorPrefsHelper.GetString("SUGUI_Partial", null); }
        set { EditorPrefsHelper.SetString("SUGUI_Partial", value); }
    }

    static public string selectedSprite
    {
        get { return EditorPrefsHelper.GetString("SUGUI_Sprite", null); }
        set { EditorPrefsHelper.SetString("SUGUI_Sprite", value); }
    }

    public void ReleaseEditorPrefs()
    {
        EditorPrefsHelper.DeleteKey("SUGUI_Atlas");
        EditorPrefsHelper.DeleteKey("SUGUI_Partial");
        EditorPrefsHelper.DeleteKey("SUGUI_Sprite");
    }
}
