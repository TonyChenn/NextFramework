using NextFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        if(!Application.isEditor)
            GetUpdateInfo();
    }

    /// <summary>
    /// 检查版本更新
    /// </summary>
    void GetUpdateInfo()
    {
        PackageInfo packageInfo = PackageConfig.Singlton.CurPackageInfo;
        string url = PathHelper.CombinePath(packageInfo.ServerPath, "/djlw/version_info.json");
        Debug.Log("Get update info from: " + url);
        WebServer.Singlton.GetDataByUrl(url, (data) =>
        {
            if(string.IsNullOrEmpty(data.error))
            {
                Debug.Log("Get update success");
                // TODO
                //JsonUtility.FromJson
                Debug.Log(data.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Get update failed: " + data.error);
            }

        });
    }
}
