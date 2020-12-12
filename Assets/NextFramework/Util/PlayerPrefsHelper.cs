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
    [MenuItem("NextFramework/Prefs/Clear All PlayerPrefs")]
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
