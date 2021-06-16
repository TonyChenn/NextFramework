using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRow : MonoBehaviour
{
    [SerializeField] UI_item[] itemArray;

    public void Init(int rowIndex, List<string> data)
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            int index = rowIndex * itemArray.Length + i;
            if (index < data.Count)
            {
                itemArray[i].Init(data[index]);
                itemArray[i].SetActive(true);
            }
            else
            {
                itemArray[i].SetActive(false);
            }
        }
    }
}
