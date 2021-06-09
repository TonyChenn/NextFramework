using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDefine
{
    public static void Init()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        var _ = AudioManger.Singlton;

        //初始化本地化
        //TODO Localize

        //初始化游戏画质
        SetGameQuality(PlayerPrefsHelper.GameQuality);
    }

    public static void SetGameQuality(int quality)
    {
        PlayerPrefsHelper.GameQuality = quality;
    }
}
