using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public bool UseLocalAsset = true;
    [SerializeField] PackageType curPackageType = PackageType.Dev;
    [SerializeField] PackageEnum packageEnum;

    public PackageType CurPackageType { get { return curPackageType; } }
    public PackageEnum CurPackageEnum { get { return packageEnum; } }

    public static GameConfig _instance = null;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }
    public static GameConfig Singlton { get { return _instance; } }
}

public enum PackageType
{
    Dev,
    QA,
    Audit,
}
