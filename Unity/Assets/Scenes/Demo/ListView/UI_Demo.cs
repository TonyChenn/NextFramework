using NextFramework.UI;
using System.Collections.Generic;
using UnityEngine;

public class UI_Demo : MonoBehaviour
{
    [SerializeField] LoopViewBase listView;

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
        listView.InitListView(dataCenter.Count, OnGetItemByIndex);
    }

    private LoopListViewItem2 OnGetItemByIndex(LoopViewBase listView, int index)
    {
        if (index < 0 || index >= dataCenter.Count) return null;

        if (dataCenter[index] == null) return null;

        LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
        UI_item compoment = item.GetComponent<UI_item>();

        if (!item.IsInitHandlerCalled)
        {
            item.IsInitHandlerCalled = true;
        }
        compoment.Init(dataCenter[index]);

        return item;
    }
}
