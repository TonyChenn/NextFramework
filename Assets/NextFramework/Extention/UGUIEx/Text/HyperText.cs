using System;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Events;

# region HyperData
public class HyperData
{
    /// <summary>
    /// 顶点开始索引
    /// </summary>
    public int StartIndex;
    /// <summary>
    /// 顶点结束索引
    /// </summary>
    public int EndIndex;
    public LinkType HyperType;
    public string HyperContent;
    public string Value;
    public readonly List<Rect> Boxes = new List<Rect>();
}
#endregion
public enum LinkType
{
    None,           //
    HyperLink,      //超链接URL
    //Emoji,          //emoji表情
    Custom,         //自定义类型
    //Item,           //Item表
}
public class HyperText : Text, IPointerClickHandler
{
    [TextArea(3, 10)]
    [SerializeField]
    public string _text = string.Empty;

    [SerializeField]
    Color hyperTextColor = Color.blue;
    /// <summary>
    /// 超链接Pattern
    /// </summary>
    /// <a href="https://www.baidu.com">12345</a>
    /// <emoji>::happy::</emoji>
    /// <button data="abcdefg">zxcvbnm</button>

    const string pattern = "<a\\s+type\\s*?=\\s*\"(.*?)\"\\s+data\\s*?=\\s*\"(.*?)\">([\\s\\S]*?)</a>";

    StringBuilder resultBuilder = new StringBuilder();
    StringBuilder builder = new StringBuilder();

    [Serializable]
    public class HrefClickEvent : UnityEvent<object[]> { }
    public HrefClickEvent OnHrefClick = new HrefClickEvent();

    List<HyperData> hyperDataList = new List<HyperData>();

    UIVertex tempVertex = UIVertex.simpleVert;
    #region Override
    protected override void OnEnable()
    {
        base.OnEnable();
        supportRichText = true;
        alignByGeometry = true;
    }
    protected override void Start()
    {
        base.Start();

        m_Text = ParseString(_text);
        SetVerticesDirty();
        SetLayoutDirty();
    }
    public override string text
    {
        get { return m_Text; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(m_Text)) return;

                m_Text = ParseString(value);
                SetVerticesDirty();
            }
            else if (_text != value)
            {
                m_Text = ParseString(value);
                SetVerticesDirty();
                SetLayoutDirty();
            }
#if UNITY_EDITOR
            //编辑器赋值无所谓
            else
            {
                m_Text = ParseString(value);
                SetVerticesDirty();
                SetLayoutDirty();
            }
#endif
            _text = value;
        }
    }
    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
    }
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (font == null) return;

        base.OnPopulateMesh(toFill);

        m_DisableFontTextureRebuiltCallback = true;
        //更新顶点位置
        //updateVertex(toFill);
        //处理超链接信息
        dealHyperLink(toFill);

        m_DisableFontTextureRebuiltCallback = false;
    }
    #endregion

    string ParseString(string inputString)
    {
        hyperDataList.Clear();

        resultBuilder.Remove(0, resultBuilder.Length);

        if (string.IsNullOrEmpty(inputString)) return "";

        int index = 0;
        string temp = "";

        //匹配超链接
        foreach (Match linkItem in Regex.Matches(_text, pattern))
        {
            string str_type = linkItem.Groups[1].Value.ToLower();
            if (str_type == "link" || str_type == "custom")
            {
                HyperData data = new HyperData();
                data.HyperType = str_type == "link" ? LinkType.HyperLink : LinkType.Custom;
                data.HyperContent = linkItem.Groups[2].Value;
                data.Value = linkItem.Groups[3].Value;

                temp = inputString.Substring(index, linkItem.Index - index);
                resultBuilder.Append(temp);
                resultBuilder.Append("<color=#");
                resultBuilder.Append(GetColorString(hyperTextColor));
                resultBuilder.Append(">");
                data.StartIndex = resultBuilder.Length * 4;
                resultBuilder.Append(data.Value);
                data.EndIndex = resultBuilder.Length * 4 - 1;
                resultBuilder.Append("</color>");

                hyperDataList.Add(data);
            }
            //else if (str_type == "emoji" || str_type == "item")
            //{
            //    var pic_index = linkItem.Index + linkItem.Length - 1;
            //    var end_index = pic_index * 4 + 3;
            //    imgVertexIndexList.Add(end_index);
            //}
            index = linkItem.Index + linkItem.Length;
        }
        resultBuilder.Append(inputString.Substring(index, inputString.Length - index));
        return resultBuilder.ToString();
    }

    void dealHyperLink(VertexHelper toFill)
    {
        if (hyperDataList.Count > 0)
        {
            for (int i = 0; i < hyperDataList.Count; i++)
            {
                hyperDataList[i].Boxes.Clear();

                int startIndex = hyperDataList[i].StartIndex;
                int endIndex = hyperDataList[i].EndIndex;

                if (startIndex >= toFill.currentVertCount)
                    continue;

                toFill.PopulateUIVertex(ref tempVertex, startIndex);
                // 将超链接里面的文本顶点索引坐标加入到包围框  
                var pos = tempVertex.position;
                var bounds = new Bounds(pos, Vector3.zero);
                for (int j = startIndex + 1; j < endIndex; j++)
                {
                    if (j >= toFill.currentVertCount)
                    {
                        break;
                    }
                    toFill.PopulateUIVertex(ref tempVertex, j);
                    pos = tempVertex.position;
                    if (pos.x < bounds.min.x)
                    {
                        // 换行重新添加包围框  
                        hyperDataList[i].Boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(pos); // 扩展包围框  
                    }
                }
                //添加包围盒
                hyperDataList[i].Boxes.Add(new Rect(bounds.min, bounds.size));
            }
        }
    }



    #region 点击处理
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out pos);
        foreach (var item in hyperDataList)
        {
            var boxes = item.Boxes;
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].Contains(pos))
                {
                    OnHrefClick.Invoke(new object[] { item.HyperType, item.HyperContent, item.Value });
                    return;
                }
            }
        }
    }

    #endregion

    #region UNITY_EDITOR
#if UNITY_EDITOR
    //辅助线框
    Vector3[] _textWolrdVertexs = new Vector3[4];

    private void OnDrawGizmos()
    {
        //text
        rectTransform.GetWorldCorners(_textWolrdVertexs);
        GizmosDrawLine(Color.white, _textWolrdVertexs);

        //preferred size
        Vector2 pivot = GetTextAnchorPivot(alignment);
        Rect rect = new Rect();
        Vector2 size = rectTransform.sizeDelta - new Vector2(preferredWidth, preferredHeight);
        rect.position = new Vector2(pivot.x * size.x, pivot.y * size.y) - new Vector2(rectTransform.sizeDelta.x * rectTransform.pivot.x, rectTransform.sizeDelta.y * rectTransform.pivot.y);
        rect.width = preferredWidth;
        rect.height = preferredHeight;
        _textWolrdVertexs[0] = TransformPoint2World(transform, new Vector3(rect.x, rect.y));
        _textWolrdVertexs[1] = TransformPoint2World(transform, new Vector3(rect.x + rect.width, rect.y));
        _textWolrdVertexs[2] = TransformPoint2World(transform, new Vector3(rect.x + rect.width, rect.y + rect.height));
        _textWolrdVertexs[3] = TransformPoint2World(transform, new Vector3(rect.x, rect.y + rect.height));

        //href
        for (int i = 0; i < hyperDataList.Count; i++)
        {
            for (int j = 0; j < hyperDataList[i].Boxes.Count; j++)
            {
                rect = hyperDataList[i].Boxes[j];
                _textWolrdVertexs[0] = TransformPoint2World(transform, rect.position);
                _textWolrdVertexs[1] = TransformPoint2World(transform, new Vector3(rect.x + rect.width, rect.y));
                _textWolrdVertexs[2] = TransformPoint2World(transform, new Vector3(rect.x + rect.width, rect.y + rect.height));
                _textWolrdVertexs[3] = TransformPoint2World(transform, new Vector3(rect.x, rect.y + rect.height));

                GizmosDrawLine(Color.green, _textWolrdVertexs);
            }
        }
    }

    //划线
    private void GizmosDrawLine(Color color, Vector3[] pos)
    {
        Gizmos.color = color;

        Gizmos.DrawLine(pos[0], pos[1]);
        Gizmos.DrawLine(pos[1], pos[2]);
        Gizmos.DrawLine(pos[2], pos[3]);
        Gizmos.DrawLine(pos[3], pos[0]);
    }
#endif
    #endregion

    #region Util
    public Vector3 TransformPoint2World(Transform transform, Vector3 point)
    {
        return transform.localToWorldMatrix.MultiplyPoint(point);
    }

    /// <summary>
    /// RGB转颜色字符串
    /// </summary>
    string GetColorString(Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }
    #endregion
}
