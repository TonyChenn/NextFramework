using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UI
{
    public static class MessageBox
    {
        #region Wait
        /// <summary>
        /// Open or Hide WaitWnd
        /// </summary>
        public static void ShowWait(bool show)
        {
            Messenger<bool>.Broadcast(MessengerEventDef.Str_ShowWait,show);
        }

        /// <summary>
        /// force close WaitWnd
        /// </summary>
        public static void HideWaitImmediate()
        {
            Messenger.Broadcast(MessengerEventDef.Str_HideWaitImmediate);
        }
        #endregion
        
        #region Dialog
        public static void ShowDialog(string content, bool showMask = true)
        {
            ShowDialog("提示", content, showMask);
        }
        public static void ShowDialog(string title, string content, bool showMask = true)
        {
            ShowDialog(title, content, null, showMask);
        }
        public static void ShowDialog(string title, string content, Action okAction, bool showMask = true)
        {
            ShowDialog(title, content, okAction, null, showMask);
        }
        public static void ShowDialog(string title, string content, Action okAction, Action cancelAction, bool showMask = true)
        {
            ShowDialog(title, content, okAction, cancelAction, null, showMask);
        }
        public static void ShowDialog(string title, string content, Action okAction, Action cancelAction,Action closeAction, bool showMask = true)
        {
            Messenger<string, string, Action, Action, Action, bool>.Broadcast(MessengerEventDef.ShowUIDialog, title, content, okAction, cancelAction, closeAction, showMask);
        }
        #endregion
    }

    public static class Toast
    {
        #region Toast Tip
        /// <summary>
        /// Toast Tips
        /// </summary>
        public static void ShowToast(string tips, float duration = .2f)
        {
            Messenger<string, float>.Broadcast(MessengerEventDef.Str_ShowToast, tips, duration);
        }
        #endregion
    }
}