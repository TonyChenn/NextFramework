using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PackageEnum
{
    QA,
    QQ,
    Mi,
    HuaWei,
    IOS,
    GooglePlay,
}

[System.Serializable]
public class PackageInfo
{
    public PackageEnum PackageEnum;
    public string PackageName;
    public string ServerPath;       // 服务器地址
}

[CreateAssetMenu]
public class PackageConfig : ScriptableObject
{
    public PackageInfo[] PackageList;

    PackageInfo m_CurPackageInfo;

    public PackageInfo CurPackageInfo { get { return m_CurPackageInfo; } }

    static PackageConfig _instance = null;
    public static PackageConfig Singlton
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<PackageConfig>("Config/PackageConfig");
                _instance.init();
            }

            return _instance;
        }
    }

    void init()
    {
        for (int i = 0,iMax= PackageList.Length; i < iMax; i++)
        {
            if (PackageList[i].PackageEnum == GameConfig.Singlton.CurPackageEnum)
            {
                m_CurPackageInfo = PackageList[i];
                break;
            }
        }
    }
}
