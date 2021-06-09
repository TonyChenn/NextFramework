using UnityEngine;

namespace NextFramework.UI
{
     public abstract class UIPopWnd : UIWndBase
     {
         [SerializeField] GameObject BtnClose;
         [SerializeField] GameObject BtnCancel;
         [SerializeField] GameObject BtnOK;
     
         [SerializeField] TweenScale TweenScaleRoot;
         
         public override void InitWndOnAwake()
         {
             base.InitWndOnAwake();
             if(BtnClose!=null)
                 ClickEventListener.Get(BtnClose).SetClickEventHandler(OnCloseClick);
             if (BtnCancel != null)
                 ClickEventListener.Get(BtnCancel).SetClickEventHandler(OnCancelClick);
             if(BtnOK!=null)
                 ClickEventListener.Get(BtnOK).SetClickEventHandler(OnOKClick);
         }
     
         public override void OnHideWnd()
         {
             base.OnHideWnd();
             TweenScaleRoot.PlayForward();
             TweenScaleRoot.callWhenFinished = "";
         }
     
         protected virtual void CloseWnd()
         {
             TweenScaleRoot.PlayReverse();
             TweenScaleRoot.eventReceiver = this.CachedGameObject;
         }
     
         protected virtual void OnCloseAnimationFinished()
         {
             UIManager.HideUIWnd(this.UIID);
         }
     
     
         protected virtual void OnOKClick(GameObject obj)
         {
             CloseWnd();
         }
     
         protected virtual void OnCancelClick(GameObject obj)
         {
             CloseWnd();
         }
     
         protected virtual void OnCloseClick(GameObject obj)
         {
             CloseWnd();
         }
     }   
}

