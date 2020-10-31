using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [SerializeField] public static bool UseLocalAsset = true;
    [SerializeField] public PackageType CurPackageType = PackageType.Dev;

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
