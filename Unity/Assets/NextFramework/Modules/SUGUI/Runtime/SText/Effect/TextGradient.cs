using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NextFramework.UI
{
    /// <summary>
    /// 颜色渐变
    /// </summary>
    public class TextGradient : BaseMeshEffect
    {
        enum Direction { Horizontal, Vertical }

        [SerializeField] Direction m_Direction = Direction.Horizontal;
        [Range(-1f, 1f)]
        [SerializeField] float m_Center = 0f;
        [SerializeField] Gradient gradient;

        List<UIVertex> vertexList = new List<UIVertex>();

        protected override void Start()
        {
            gradient = new Gradient();
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0)
            {
                return;
            }

            vertexList.Clear();
            vh.GetUIVertexStream(vertexList);

            int nCount = vertexList.Count;
            switch (m_Direction)
            {
                case Direction.Vertical:
                    {
                        float fBottomY = vertexList[0].position.y;
                        float fTopY = vertexList[0].position.y;
                        float fYPos = 0f;

                        for (int i = nCount - 1; i >= 1; --i)
                        {
                            fYPos = vertexList[i].position.y;
                            if (fYPos > fTopY)
                                fTopY = fYPos;
                            else if (fYPos < fBottomY)
                                fBottomY = fYPos;
                        }

                        float fUIElementHeight = 1f / (fTopY - fBottomY);
                        UIVertex v = new UIVertex();

                        for (int i = 0; i < vh.currentVertCount; i++)
                        {
                            vh.PopulateUIVertex(ref v, i);
                            v.color = gradient.Evaluate((v.position.y - fBottomY) *
                            fUIElementHeight - m_Center);
                            vh.SetUIVertex(v, i);
                        }
                    }
                    break;
                case Direction.Horizontal:
                    {
                        float fLeftX = vertexList[0].position.x;
                        float fRightX = vertexList[0].position.x;
                        float fXPos = 0f;

                        for (int i = nCount - 1; i >= 1; --i)
                        {
                            fXPos = vertexList[i].position.x;
                            if (fXPos > fRightX)
                                fRightX = fXPos;
                            else if (fXPos < fLeftX)
                                fLeftX = fXPos;
                        }

                        float fUIElementWidth = 1f / (fRightX - fLeftX);
                        UIVertex v = new UIVertex();

                        for (int i = 0; i < vh.currentVertCount; i++)
                        {
                            vh.PopulateUIVertex(ref v, i);
                            v.color = gradient.Evaluate((v.position.x - fLeftX) *
                            fUIElementWidth - m_Center);
                            vh.SetUIVertex(v, i);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
