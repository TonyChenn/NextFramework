using NextFramework.UI;
using System;
using UnityEngine;

namespace NextFramework.UI
{
    public class UI_MessageBoxWnd : MonoBehaviour
    {
        [SerializeField] Transform RootTrans;

        [SerializeField] Transform Mask;
        [SerializeField] SText TitleText;
        [SerializeField] SText TipText;
        [SerializeField] SButton BtnClose;
        [SerializeField] Transform OneButtonGroup;
        [SerializeField] SButton BtnSingle;

        [SerializeField] Transform TwoButtonGroup;
        [SerializeField] SButton BtnLeft;
        [SerializeField] SButton BtnRight;

        private Action m_CancelAction;
        private Action m_OkAction;
        private Action m_CloseAction;

        public void Awake()
        {
            RootTrans.SetActive(false);
            BtnLeft.OnClick.AddListener(OnOKClick);
            BtnRight.OnClick.AddListener(OnCancelClick);
            BtnSingle.OnClick.AddListener(OnOKClick);
            BtnClose.OnClick.AddListener(OnCloseClick);
            
            Messenger<string, string, Action, Action, Action, bool>.AddListener(MessengerEventDef.ShowUIDialog, Show);
        }

        private void OnDestroy()
        {
            Messenger<string, string, Action, Action, Action, bool>.RemoveListener(MessengerEventDef.ShowUIDialog, Show);
        }
        
        private void Show(string title, string content, Action ok, Action cancel, Action close, bool showMask)
        {
            CleanAction();
            
            this.TitleText.text = title;
            this.TipText.text = content;
            this.Mask.SetActive(showMask);
            this.m_CloseAction = close;

            if (ok == null && cancel == null)
            {
                OneButtonGroup.SetActive(false);
                TwoButtonGroup.SetActive(false);
            }
            else if (ok != null && cancel == null)
            {
                m_OkAction = ok;

                OneButtonGroup.SetActive(true);
                TwoButtonGroup.SetActive(false);
            }
            else if (ok != null)
            {
                m_OkAction = ok;
                m_CancelAction = cancel;

                OneButtonGroup.SetActive(false);
                TwoButtonGroup.SetActive(true);
            }

            RootTrans.SetActive(true);
        }


        protected void OnCloseClick()
        {
            RootTrans.SetActive(false);
            m_CloseAction?.Invoke();
            CleanAction();
        }

        protected void OnCancelClick()
        {
            RootTrans.SetActive(false);
            m_CancelAction?.Invoke();
            CleanAction();
        }

        protected void OnOKClick()
        {
            RootTrans.SetActive(false);
            m_OkAction?.Invoke();
        }

        private void CleanAction()
        {
            m_CloseAction = null;
            m_CancelAction = null;
            m_OkAction = null;
        }
    }
}