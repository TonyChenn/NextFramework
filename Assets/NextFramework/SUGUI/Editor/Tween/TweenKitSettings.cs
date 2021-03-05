//-------------------------------------------------
//            TweenKit
// Copyright © 2020 tonychenn.cn
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

namespace NextFramework
{
    /// <summary>
    /// Unity doesn't keep the values of static variables after scripts change get recompiled. One way around this
    /// is to store the references in EditorPrefs -- retrieve them at start, and save them whenever something changes.
    /// </summary>

    public class TweenKitSettings
    {
	    public enum ColorMode
	    {
		    Orange,
		    Green,
		    Blue,
	    }

    #region Generic Get and Set methods
	    /// <summary>
	    /// Save the specified boolean value in settings.
	    /// </summary>

	    public static void SetBool (string name, bool val) { EditorPrefs.SetBool(name, val); }

	    /// <summary>
	    /// Save the specified integer value in settings.
	    /// </summary>

	    public static void SetInt (string name, int val) { EditorPrefs.SetInt(name, val); }

	    /// <summary>
	    /// Save the specified float value in settings.
	    /// </summary>

	    public static void SetFloat (string name, float val) { EditorPrefs.SetFloat(name, val); }

	    /// <summary>
	    /// Save the specified string value in settings.
	    /// </summary>

	    public static void SetString (string name, string val) { EditorPrefs.SetString(name, val); }

	    /// <summary>
	    /// Save the specified color value in settings.
	    /// </summary>

	    public static void SetColor (string name, Color c) { SetString(name, c.r + " " + c.g + " " + c.b + " " + c.a); }

	    /// <summary>
	    /// Save the specified enum value to settings.
	    /// </summary>

	    public static void SetEnum (string name, System.Enum val) { SetString(name, val.ToString()); }

	    /// <summary>
	    /// Save the specified object in settings.
	    /// </summary>

	    public static void Set (string name, Object obj)
	    {
		    if (obj == null)
		    {
			    EditorPrefs.DeleteKey(name);
		    }
		    else
		    {
			    if (obj != null)
			    {
				    string path = AssetDatabase.GetAssetPath(obj);

				    if (!string.IsNullOrEmpty(path))
				    {
					    EditorPrefs.SetString(name, path);
				    }
				    else
				    {
					    EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
				    }
			    }
			    else EditorPrefs.DeleteKey(name);
		    }
	    }

	    /// <summary>
	    /// Get the previously saved boolean value.
	    /// </summary>

	    public static bool GetBool (string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }

	    /// <summary>
	    /// Get the previously saved integer value.
	    /// </summary>

	    public static int GetInt (string name, int defaultValue) { return EditorPrefs.GetInt(name, defaultValue); }

	    /// <summary>
	    /// Get the previously saved float value.
	    /// </summary>

	    public static float GetFloat (string name, float defaultValue) { return EditorPrefs.GetFloat(name, defaultValue); }

	    /// <summary>
	    /// Get the previously saved string value.
	    /// </summary>

	    public static string GetString (string name, string defaultValue) { return EditorPrefs.GetString(name, defaultValue); }

	    /// <summary>
	    /// Get a previously saved color value.
	    /// </summary>

	    public static Color GetColor (string name, Color c)
	    {
		    string strVal = GetString(name, c.r + " " + c.g + " " + c.b + " " + c.a);
		    string[] parts = strVal.Split(' ');

		    if (parts.Length == 4)
		    {
			    float.TryParse(parts[0], out c.r);
			    float.TryParse(parts[1], out c.g);
			    float.TryParse(parts[2], out c.b);
			    float.TryParse(parts[3], out c.a);
		    }
		    return c;
	    }

	    /// <summary>
	    /// Get a previously saved enum from settings.
	    /// </summary>

	    public static T GetEnum<T> (string name, T defaultValue)
	    {
		    string val = GetString(name, defaultValue.ToString());
		    string[] names = System.Enum.GetNames(typeof(T));
		    System.Array values = System.Enum.GetValues(typeof(T));

		    for (int i = 0; i < names.Length; ++i)
		    {
			    if (names[i] == val)
				    return (T)values.GetValue(i);
		    }
		    return defaultValue;
	    }

	    /// <summary>
	    /// Get a previously saved object from settings.
	    /// </summary>

	    public static T Get<T> (string name, T defaultValue) where T : Object
	    {
		    string path = EditorPrefs.GetString(name);
		    if (string.IsNullOrEmpty(path)) return null;

		    T retVal = TweenKitEditorTools.LoadAsset<T>(path);

		    if (retVal == null)
		    {
			    int id;
			    if (int.TryParse(path, out id))
				    return EditorUtility.InstanceIDToObject(id) as T;
		    }
		    return retVal;
	    }
    #endregion

    #region Convenience accessor properties

	    public static bool showTransformHandles
	    {
		    get { return GetBool("Tweener Transform Handles", false); }
		    set { SetBool("TweenKit Transform Handles", value); }
	    }

	    public static bool minimalisticLook
	    {
		    get { return GetBool("TweenKit Minimalistic", false); }
		    set { SetBool("TweenKit Minimalistic", value); }
	    }

	    public static bool unifiedTransform
	    {
		    get { return GetBool("TweenKit Unified", false); }
		    set { SetBool("TweenKit Unified", value); }
	    }

	    public static Color color
	    {
		    get { return GetColor("TweenKit Color", Color.white); }
		    set { SetColor("TweenKit Color", value); }
	    }

	    public static Color foregroundColor
	    {
		    get { return GetColor("TweenKit FG Color", Color.white); }
		    set { SetColor("TweenKit FG Color", value); }
	    }

	    public static Color backgroundColor
	    {
		    get { return GetColor("TweenKit BG Color", Color.black); }
		    set { SetColor("TweenKit BG Color", value); }
	    }

	    public static ColorMode colorMode
	    {
		    get { return GetEnum("TweenKit Color Mode", ColorMode.Blue); }
		    set { SetEnum("TweenKit Color Mode", value); }
	    }
    #endregion
    }

}
