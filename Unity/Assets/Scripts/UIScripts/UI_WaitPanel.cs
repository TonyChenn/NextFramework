using System;
using System.Collections;
using System.Collections.Generic;
using NextFramework;
using NextFramework.UI;
using UnityEngine;

public class UI_WaitPanel : MonoBehaviour
{
    [SerializeField] private Transform Mask;

    private TweenAlpha m_TweenAlpha;
    private int m_OpenCount = 0;

    public void Awake()
    {
        m_TweenAlpha = Mask.GetComponent<TweenAlpha>();
        m_TweenAlpha.@from = 0;
        m_TweenAlpha.to = 1;
        Mask.SetActive(false);
        
        Messenger<bool>.AddListener(MessengerEventDef.Str_ShowWait,OnShowWait);
        Messenger.AddListener(MessengerEventDef.Str_HideWaitImmediate,OnHideImmediate);
    }

    private void OnDestroy()
    {
        Messenger<bool>.RemoveListener(MessengerEventDef.Str_ShowWait,OnShowWait);
        Messenger.RemoveListener(MessengerEventDef.Str_HideWaitImmediate,OnHideImmediate);
    }
    private void OnShowWait(bool show)
    {
        if (show)
            ShowOne();
        else
            HideOne();
    }
    private void OnHideImmediate()
    {
        m_OpenCount = 0;
        HideOne();
    }
    
    void ShowOne()
    {
        if (m_OpenCount <= 0)
        {
            m_OpenCount = 0;
            Mask.SetActive(true);
        }
        ++m_OpenCount;
    }

    void HideOne()
    {
        --m_OpenCount;
        if (m_OpenCount <= 0)
        {
            m_OpenCount = 0;
            Mask.SetActive(false);
        }
    }
}
