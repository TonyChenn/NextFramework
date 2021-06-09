using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTest : MonoBehaviour
{
    [SerializeField] HyperText hyperText;

    void OnEnable()
    {
        hyperText.OnHrefClick.AddListener(OnHrefClick);
    }
    private void Start()
    {
        hyperText._text = "<a type=\"link\" data=\"https://tonychenn.cn\">TonyChenn</a>123sdhdfgh<a type=\"custom\" data=\"1234567\">测试按钮</a><a type=\"item\" data=\"1234567\">测试按钮</a>";
    }

    private void OnDisable()
    {
        hyperText.OnHrefClick.RemoveListener(OnHrefClick);
    }

    private void OnHrefClick(object[] args)
    {
        LinkType type = (LinkType)args[0];
        string data = args[1] as string;
        string nodeValue = args[2] as string;
        switch (type)
        {
            case LinkType.None:
                break;
            case LinkType.HyperLink:
                Application.OpenURL(data);
                break;
            case LinkType.Custom:
                Debug.Log("自定义方法：" + data);
                break;
            default:
                break;
        }
    }

}
