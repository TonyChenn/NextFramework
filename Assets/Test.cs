using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] Image Btn;
    void Start()
    {
        Btn.SetActive(false);
    }
}
