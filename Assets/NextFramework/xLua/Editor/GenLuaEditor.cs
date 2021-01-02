using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenLua
{
    [MenuItem("Assets/生成Lua")]
    public static void GenLuaFile()
    {
        var selectObj = Selection.activeGameObject;
        if (selectObj != null && selectObj.GetType() == typeof(GameObject))
        {
            string path = AssetDatabase.GetAssetPath(selectObj);
        }
    }
}
