using NextFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidenceTest : MonoBehaviour
{
    [SerializeField] Button GuideBtn;
    [SerializeField] Button Btn2;

    GuidenceManger GuidenceManger;
    void Start()
    {
        MaskableGraphic target = GuideBtn.GetComponent<MaskableGraphic>();
        GuidenceManger = GetComponent<GuidenceManger>();
        GuidenceManger.ShowGuidence(target);

        GuideBtn.onClick.AddListener(() =>
        {
            Debug.Log("引导按钮");
        });

        Btn2.onClick.AddListener(() =>
        {
            Debug.Log("另一个按钮");
        });
    }
}
