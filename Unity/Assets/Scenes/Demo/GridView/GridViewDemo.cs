using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NextFramework.UI;

public class GridViewDemo : MonoBehaviour
{
    [SerializeField] GridView gridView;
    List<string> dataCenter = new List<string>();
    private void Awake()
    {
        for (int i = 0; i < 1000; i++)
        {
            dataCenter.Add(i + " qaxdsfsgsdgf");
        }
    }

    void Start()
    {
        gridView.InitListView(dataCenter.Count, OnGetItemByIndex);
    }

    private LoopListViewItem2 OnGetItemByIndex(LoopViewBase listView, int rowIndex)
    {
        if (rowIndex < 0) return null;
        //create one row
        LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
        ItemRow itemScript = item.GetComponent<ItemRow>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
        }
        itemScript.Init(rowIndex, dataCenter);
        return item;
    }
}
