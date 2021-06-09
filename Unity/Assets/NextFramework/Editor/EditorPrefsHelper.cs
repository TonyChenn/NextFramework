using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using NextFramework;
using NextFramework.UI;
using UnityEngine.U2D;

public interface IEditorPrefs
{
    /// <summary>
    /// 释放EditorPrefs
    /// </summary>
    void ReleaseEditorPrefs();
}

public class EditorPrefsHelper : NormalSingleton<EditorPrefsHelper>, IEditorPrefs
{
    public EditorPrefsHelper() { }

    [MenuItem("Tools/Prefs/Clear All EditorPrefs")]
    static void ClearAllEditorPrefs()
    {
        Assembly assembly = Assembly.Load("Assembly-CSharp-Editor");
        System.Type[] types = assembly.GetTypes();
        for (int i = 0, iMax = types.Length; i < iMax; i++)
        {
            //如果是接口
            if (types[i].IsInterface) continue;

            System.Type[] ins = types[i].GetInterfaces();
            foreach (var item in ins)
            {
                if (item == typeof(IEditorPrefs))
                {
                    object o = System.Activator.CreateInstance(types[i]);
                    MethodInfo method = item.GetMethod("ReleaseEditorPrefs");
                    method.Invoke(o, null);
                    break;
                }
            }
        }
    }

    #region Key
    static string Key { get { return Application.dataPath; } }
    static string key_excelfolder => Key + "_excel_folder";
    static string key_abfolder => Key + "_ab_pack_folder";
    static string key_dontusesttreammingasset => Key + "_dont_use_streammingasset_folder";

    static string key_build_platform_type => Key + "_build_platform_type";

    static string key_packagekit_json => Key + "_packagekit_json";
    static string key_packagekit_version => Key + "_packagekit_version";

    static string key_build_table => Key + "_build_table";
    static string key_build_xlua => Key + "_build_xlua";

    static List<string> key_obj_list = new List<string>();
    #endregion

    /// <summary>
    /// 当前选择的AB包路径
    /// </summary>
    public static string ABPackFolder
    {
        get { return GetString(key_abfolder, ""); }
        set { SetString(key_abfolder, value); }
    }
    /// <summary>
    /// 当前打包平台
    /// </summary>
    public static BuildTarget CurBuildTarget
    {
        get { return (BuildTarget)EditorPrefs.GetInt(key_build_platform_type); }
        set { EditorPrefs.SetInt(key_build_platform_type, (int)value); }
    }

    /// <summary>
    /// PackageKit下载的json信息
    /// </summary>
    public static string PackageKitJson
    {
        get { return GetString(key_packagekit_json, ""); }
        set { SetString(key_build_platform_type, value); }
    }

    /// <summary>
    /// 当前包版本
    /// </summary>
    public static float PackageKitVersion
    {
        get { return GetFloat(key_packagekit_version, 0f); }
        set { SetFloat(key_packagekit_version, value); }
    }

    /// <summary>
    /// 打包执行重新生成xLua
    /// </summary>
    public static bool BuildXLua
    {
        get { return GetBool(key_build_xlua, true); }
        set { SetBool(key_build_xlua, value); }
    }
    /// <summary>
    /// 打包重新生成配表
    /// </summary>
    public static bool BuildConvertTable
    {
        get { return GetBool(key_build_table, true); }
        set { SetBool(key_build_table, value); }
    }

    #region Generic Get and Set methods
    // bool
    public static bool GetBool(string prefs_key, bool defaultValue) { return EditorPrefs.GetBool(prefs_key, defaultValue); }
    public static void SetBool(string prefs_key, bool val) { EditorPrefs.SetBool(prefs_key, val); }

    // int
    public static int GetInt(string prefs_key, int defaultValue) { return EditorPrefs.GetInt(prefs_key, defaultValue); }
    public static void SetInt(string prefs_key, int val) { EditorPrefs.SetInt(prefs_key, val); }

    // float
    public static float GetFloat(string prefs_key, float defaultValue) { return EditorPrefs.GetFloat(prefs_key, defaultValue); }
    public static void SetFloat(string prefs_key, float val) { EditorPrefs.SetFloat(prefs_key, val); }

    // color
    public static Color GetColor(string prefs_key, Color c)
    {
        string strVal = GetString(prefs_key, c.r + " " + c.g + " " + c.b + " " + c.a);
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
    public static void SetColor(string prefs_key, Color c) { SetString(prefs_key, c.r + " " + c.g + " " + c.b + " " + c.a); }

    // enum
    public static T GetEnum<T>(string prefs_key, T defaultValue)
    {
        string val = GetString(prefs_key, defaultValue.ToString());
        string[] names = System.Enum.GetNames(typeof(T));
        System.Array values = System.Enum.GetValues(typeof(T));

        for (int i = 0; i < names.Length; ++i)
        {
            if (names[i] == val)
                return (T)values.GetValue(i);
        }
        return defaultValue;
    }
    public static void SetEnum(string prefs_key, System.Enum val) { SetString(prefs_key, val.ToString()); }

    // string
    public static string GetString(string prefs_key, string defaultValue) { return EditorPrefs.GetString(prefs_key, defaultValue); }
    public static void SetString(string prefs_key, string val) { EditorPrefs.SetString(prefs_key, val); }

    //Object
    public static T Get<T>(string prefs_key, T defaultValue) where T : Object
    {
        if (!key_obj_list.Contains(prefs_key))
            key_obj_list.Add(prefs_key);

        string path = EditorPrefs.GetString(prefs_key);
        if (string.IsNullOrEmpty(path)) return null;

        T retVal = SUGUIEditorTool.LoadAsset<T>(path);

        if (retVal == null)
        {
            int id;
            if (int.TryParse(path, out id))
                return EditorUtility.InstanceIDToObject(id) as T;
        }
        return retVal;
    }
    public static void SetObject(string prefs_key, Object obj)
    {
        if (!key_obj_list.Contains(prefs_key))
            key_obj_list.Add(prefs_key);

        if (obj == null)
        {
            EditorPrefs.DeleteKey(prefs_key);
        }
        else
        {
            if (obj != null)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                EditorPrefs.SetString(prefs_key, string.IsNullOrEmpty(path) ? obj.GetInstanceID().ToString() : path);
            }
            else EditorPrefs.DeleteKey(prefs_key);
        }
    }
    #endregion

    public static void DeleteKey(string prefs_key)
    {
        EditorPrefs.DeleteKey(prefs_key);
    }

    #region IEditorPrefs
    public void ReleaseEditorPrefs()
    {
        EditorPrefs.DeleteKey(key_excelfolder);
        EditorPrefs.DeleteKey(key_abfolder);
        EditorPrefs.DeleteKey(key_dontusesttreammingasset);
        EditorPrefs.DeleteKey(key_build_platform_type);
        EditorPrefs.DeleteKey(key_build_table);
        EditorPrefs.DeleteKey(key_build_xlua);
        EditorPrefs.DeleteKey(key_build_platform_type);

        for (int i = 0, iMax = key_obj_list.Count; i < iMax; i++)
            EditorPrefs.DeleteKey(key_obj_list[i]);
    }
    #endregion
}