/// <summary>
/// 本文件中的代码为生成的代码，不允许手动修改
/// </summary>
using System;
using UnityEngine;

[Serializable]
public partial class Item_t_item
{
	/// <summary>
	/// f_ID(ID)
	/// <summary>
	public uint f_ID;
	/// <summary>
	/// f_Name(名称)
	/// <summary>
	public string f_Name;
	/// <summary>
	/// f_Price(步骤ID)
	/// <summary>
	public uint f_Price;

}

public partial class Config_t_item : ScriptableObject
{
    public Item_t_item[] Array;
    public static Config_t_item Singleton { get; private set; }

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
        //var item = new NormalAssetItem("/Table/t_item.u");
        //item.Load(() =>
        //{
        //    Singleton = item.AssetObj as Config_t_item;
        //});
    }

#if UNITY_EDITOR
    static void LoadFromLocal()
{
    string path = "/Asset/Table/t_item.asset";
    var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Config_t_item>(path);
    Singleton = obj as Config_t_item;
}
#endif
}