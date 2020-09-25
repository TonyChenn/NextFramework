using UnityEngine;
using UnityEditor;

public class EditorPrefsHelper
{
    static string Key { get { return Application.dataPath; } }
    static string key_excelfolder => Key + "_excel_folder";
    static string key_abfolder => Key + "_ab_pack_folder";
    static string key_dontusesttreammingasset => Key + "_dont_use_streammingasset_folder";

    static string key_build_platform_type => Key + "_build_platform_type";

    static string key_packagekit_json => Key + "_packagekit_json";
    static string key_packagekit_version => Key + "_packagekit_version";


    /// <summary>
    /// 配置表文件夹
    /// </summary>
    public static string ExcelFolder
    {
        get
        {
            if (EditorPrefs.HasKey(key_excelfolder))
                return EditorPrefs.GetString(key_excelfolder);
            return "";
        }
        set
        {
            EditorPrefs.SetString(key_excelfolder, value);
        }
    }

    /// <summary>
    /// 不使用StreammingAsset文件夹
    /// </summary>
    public static bool DontUseSteammingAssetFolder
    {
        get
        {
            if (EditorPrefs.HasKey(key_dontusesttreammingasset))
                return EditorPrefs.GetBool(key_dontusesttreammingasset);
            return false;
        }
        set
        {
            EditorPrefs.SetBool(key_dontusesttreammingasset, value);
        }
    }
    /// <summary>
    /// 当前选择的AB包路径
    /// </summary>
    public static string ABPackFolder
    {
        get
        {
            if (EditorPrefs.HasKey(key_abfolder))
                return EditorPrefs.GetString(key_abfolder);
            return "";
        }
        set
        {
            EditorPrefs.SetString(key_abfolder, value);
        }
    }
    /// <summary>
    /// 当前打包平台
    /// </summary>
    public static BuildTarget CurBuildTarget
    {
        get
        {
            if (EditorPrefs.HasKey(key_build_platform_type))
                return (BuildTarget)EditorPrefs.GetInt(key_build_platform_type);
            return BuildTarget.NoTarget;
        }
        set
        {
            EditorPrefs.SetInt(key_build_platform_type, (int)value);
        }
    }

    /// <summary>
    /// PackageKit下载的json信息
    /// </summary>
    public static string PackageKitJson
    {
        get
        {
            if (EditorPrefs.HasKey(key_packagekit_json))
                return EditorPrefs.GetString(key_packagekit_json);
            return null;
        }
        set
        {
            EditorPrefs.SetString(key_build_platform_type, value);
        }
    }

    /// <summary>
    /// 当前包版本
    /// </summary>
    public static double PackageKitVersion
    {
        get
        {
            if (EditorPrefs.HasKey(key_packagekit_version))
                return double.Parse(EditorPrefs.GetString(key_packagekit_version));
            return 0f;
        }
        set
        {
            EditorPrefs.SetString(key_packagekit_version, value.ToString());
        }
    }

    [MenuItem("NextFramework/Prefs/Clear All EditorPrefs")]
    static void ClearAllEditorPrefs()
    {
        EditorPrefs.DeleteKey(key_excelfolder);
        EditorPrefs.DeleteKey(key_abfolder);
        EditorPrefs.DeleteKey(key_dontusesttreammingasset);
        EditorPrefs.DeleteKey(key_build_platform_type);
    }
}
