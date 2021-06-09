
namespace NextFramework
{
    /// <summary>
    /// This class is used to define all eventId.
    /// Event string must be different.
    /// </summary>
    public class MessengerEventDef
    {
        public const string ShowUIDialog = "ShowUIDialog";
        public const string Str_ShowToast = "Str_ShowToast";
        public const string Str_ShowWait = "Str_ShowWait";
        public const string Str_HideWaitImmediate = "Str_HideWaitImmediate";    //强制移除UI_Wait 窗口
    }
}