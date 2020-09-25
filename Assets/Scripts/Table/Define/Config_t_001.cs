/// <summary>
/// 本文件中的代码为生成的代码，不允许手动修改
/// </summary>
using System;
using UnityEngine;

[Serializable]
public partial class Item_t_001
{
	/// <summary>
	/// f_ID(ID作者: ID为主键)
	public int f_ID;
	/// <summary>
	/// <summary>
	/// f_Name(名称作者: 名称呀)
	public string f_Name;
	/// <summary>
	/// <summary>
	/// f_Color(颜色)
	public string f_Color;
	/// <summary>

}

public partial class Config_t_001 : ScriptableObject
{
    public Item_t_001[] Array;
    public static Config_t_001 Singleton { get; private set; }

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
    }

#if UNITY_EDITOR
    static void LoadFromLocal()
{
    string path = "/Asset/Table/t_001.asset";
    var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Config_t_001>(path);
    Singleton = obj as Config_t_001;
}
#endif
}