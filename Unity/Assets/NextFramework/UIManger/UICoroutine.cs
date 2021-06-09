using UnityEngine;

namespace NextFramework.UI
{
    /// <summary>
    /// 仅用于开启UI界面Coroutine。
    /// 该脚本对象会在UIMgr.DestroyAllUI()里面停止所有该脚本开启的Coroutine。
    /// </summary>
    public class UICoroutine : MonoSinglton<UICoroutine> { }
}
