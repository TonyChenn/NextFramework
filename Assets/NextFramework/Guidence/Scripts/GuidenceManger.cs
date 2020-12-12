using NextFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidenceManger : MonoBehaviour
{
    public enum GuideType { Circle, Rect }

    CircleGuidence mCircleGuidence;
    RectGuidence mRectGuidence;

    private void Awake()
    {
        if (mCircleGuidence == null)
            mCircleGuidence = gameObject.AddComponent<CircleGuidence>();

        mRectGuidence = GetComponent<RectGuidence>();
        if (mRectGuidence == null)
            mRectGuidence = gameObject.AddComponent<RectGuidence>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">遮罩类型</param>
    /// <param name="target">遮罩目标</param>
    public void ShowGuidence(GuideType type,Image target,float timer = .2f)
    {
        switch (type)
        {
            case GuideType.Circle:
                mCircleGuidence.Show(target, timer);
                break;
            case GuideType.Rect:
                mRectGuidence.Show(target, timer);
                break;
            default:
                break;
        }
    }

    public void Close()
    {

    }
}
