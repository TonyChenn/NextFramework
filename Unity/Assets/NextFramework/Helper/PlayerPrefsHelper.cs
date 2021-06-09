using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum PlayerPrefsType
{
    /// <summary>
    /// 设备
    /// </summary>
    Divice = 0,
    /// <summary>
    /// 账号
    /// </summary>
    Account = 1,
}
public class PlayerPrefsHelper
{
    const string key_game_quality = "key_game_quality";

#if UNITY_EDITOR
    [MenuItem("Tools/Prefs/Clear All PlayerPrefs")]
#endif
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 游戏质量
    /// </summary>
    public static int GameQuality
    {
        get { return PlayerPrefs.GetInt(key_game_quality, 2); }
        set
        {
            PlayerPrefs.SetInt(key_game_quality, value);
            QualitySettings.SetQualityLevel(value, true);
        }
    }
}
