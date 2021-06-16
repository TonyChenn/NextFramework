using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    string ftpUrl;

    public bool HotPatch = false;
    public bool UseLocalAsset = true;
    [SerializeField] PackageEnum packageEnum = PackageEnum.Dev;

    public PackageEnum CurPackageEnum { get { return packageEnum; } }

    public static GameConfig _instance = null;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }
    public static GameConfig Singlton { get { return _instance; } }

    public bool HotPatching
    {
        get { return Singlton ? Singlton.HotPatch : true; }
    }

    public string FtpUrl
    {
        get
        {
            if (string.IsNullOrEmpty(ftpUrl))
                ftpUrl = PackageConfig.Singlton.GetFtpUrl().GetStandardPath();

            return ftpUrl;
        }
    }
}
