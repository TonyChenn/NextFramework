using NextFramework;
using NextFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        InitGame();
        UIManager.LoadMsgUI();
        
        if (!GameConfig.Singlton.UseLocalAsset)
            GetUpdateInfo();

        MessageBox.ShowDialog("提示", "确定要xxx吗？", () =>
        {
            Toast.ShowToast("点击了确定");
        }, null, null);
    }

    /// <summary>
    /// 检查版本更新
    /// </summary>
    void GetUpdateInfo()
    {
        PackageInfo packageInfo = PackageConfig.Singlton.CurPackageInfo;
        string url = PathHelper.CombinePath(packageInfo.FtpServer, "/djlw/version_info.json");
        Debug.Log("Get update info from: " + url);
        WebServer.Singlton.GetDataByUrl(url, (data) =>
        {
            if(string.IsNullOrEmpty(data.error))
            {
                Log.Debug("Get update success");
                // TODO
                //JsonUtility.FromJson
                Log.Debug(data.downloadHandler.text);
            }
            else
            {
                Log.Error("Get update failed: " + data.error);
            }
        });
    }

    void InitGame()
    {
        GameDefine.Init();
    }
}
