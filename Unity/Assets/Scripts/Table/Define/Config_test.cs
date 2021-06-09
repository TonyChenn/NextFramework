/// <summary>
/// 本文件中的代码为生成的代码，不允许手动修改
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class Item_test
{
	/// <summary>
	/// f_id(ID)
	/// <summary>
	public uint f_id;
	/// <summary>
	/// f_Name(名称)
	/// <summary>
	public string f_Name;
	/// <summary>
	/// f_Level(等级)
	/// <summary>
	public int f_Level;

}

public partial class Config_test : ScriptableObject
{
    public Item_test[] Array;
    public static Config_test Singleton { get; private set; }

    public static void Init()
    {
#if UNITY_EDITOR
        if (GameConfig.Singlton.UseLocalAsset)
            LoadFromLocal();
        else
            LoadFromBundle();
#else
            LoadFromBundle();
#endif
    }

    static void LoadFromBundle()
    {
        //var item = new NormalAssetItem("/Table/test.u");
        //item.Load(() =>
        //{
        //    Singleton = item.AssetObj as Config_test;
        //});
    }

#if UNITY_EDITOR
    static void LoadFromLocal()
{
    string path = "/Asset/Table/test.asset";
    var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Config_test>(path);
    Singleton = obj as Config_test;
}
#endif
}