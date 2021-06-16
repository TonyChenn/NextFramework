using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_item : MonoBehaviour
{
    [SerializeField] Text Info;

    public void Init(string info)
    {
        Info.text = info;
    }
}
