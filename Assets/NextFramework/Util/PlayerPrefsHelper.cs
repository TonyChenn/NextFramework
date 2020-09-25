using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPrefsHelper
{
    [MenuItem("NextFramework/Prefs/Clear All PlayerPrefs")]
    static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
