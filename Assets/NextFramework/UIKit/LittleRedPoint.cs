using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ILittleRedPoint
{
    IEnumerable<LittleRedPoint> GetLittleRedPoint();
}
public class LittleRedPoint
{
    Action<bool> mCondition;
    LittleRedPointWidget mWidget;


}
public class LittleRedPointWidget:MonoBehaviour
{
    MaskableGraphic mRedPointIamge;
    Location mLocation;
    Vector3 mOffset;

    LittleRedPointWidget mWidgetPrefab;

    private void OnEnable()
    {
        mRedPointIamge = GetComponent<MaskableGraphic>();
    }

    public void LoadPrefab()
    {
        var go = new GameObject("[LittleRedPoint]");
        RawImage image = go.AddComponent<RawImage>();
        image.texture = Resources.Load("unity_builtin_extra/Knob") as Texture;
        image.color = Color.red;
        mWidgetPrefab = go.AddComponent<LittleRedPointWidget>();
        mWidgetPrefab.gameObject.SetActive(false);
    }

    public LittleRedPointWidget Create(MaskableGraphic parent,Vector3 pos)
    {
        var obj = Create(parent);
        obj.transform.localPosition = pos;
        return obj;
    }
    LittleRedPointWidget Create(MaskableGraphic parent)
    {
        return Instantiate(mWidgetPrefab, parent.transform);
    }

    public enum Location
    {
        TopLeft,TopCenter,TopRight,
        CenterLeft,Center,CenterRight,
        ButtomLeft,ButtomCenter,ButtomRight,
    }
}
