using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    [SerializeField] GameObject ItemPrefab;

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var item = Instantiate(ItemPrefab, transform);
            item.transform.localScale = Vector3.one;

            item.transform.localPosition = new Vector3(0, -(150 * i + 50), 0);
            item.SetActive(true);
        }
    }
}
