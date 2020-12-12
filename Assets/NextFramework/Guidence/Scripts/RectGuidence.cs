using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 矩形引导组件
/// </summary>
public class RectGuidence : MonoBehaviour
{
    /// <summary>
    /// 高亮显示的目标
    /// </summary>
    [SerializeField] Image mTarget;
    [Range(0, 1)]
    [SerializeField] float mAnimTime = 0.5f;
    [Range(0, 10)]
    [SerializeField] float mBorder = 10f;

    Vector3[] mCorners = new Vector3[4];

    /// <summary>
    /// 最终的偏移值X
    /// </summary>
    float mFinalOffsetX = 0f;
    float mFinalOffsetY = 0f;

    /// <summary>
    /// 当前的偏移值X
    /// </summary>
    float mCurOffsetX = 0f;
    float mCurOffsetY = 0f;

    /// <summary>
    /// 遮罩材质
    /// </summary>
    Material mMaterial;
    RectTransform mRectRrans;
    Canvas mCanvas;
    Camera mCanvasCamera;

    void Awake()
    {
        mCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        mRectRrans = (mCanvas.transform as RectTransform);
        mCanvasCamera = mCanvas.GetComponent<Camera>();
    }

    private float shrinkSpeedX = 0f;
    private float shrinkSpeedY = 0f;

    private void Update()
    {
        //从当前偏移值到目标偏移值差值显示收缩动画
        float valueX = Mathf.SmoothDamp(mCurOffsetX, mFinalOffsetX, ref shrinkSpeedX, mAnimTime);
        float valueY = Mathf.SmoothDamp(mCurOffsetY, mFinalOffsetY, ref shrinkSpeedY, mAnimTime);
        if (!Mathf.Approximately(valueX, mCurOffsetX))
        {
            mCurOffsetX = valueX;
            mMaterial.SetFloat("_RangeX", mCurOffsetX);
        }

        if (!Mathf.Approximately(valueY, mCurOffsetY))
        {
            mCurOffsetY = valueY;
            mMaterial.SetFloat("_RangeY", mCurOffsetY);
        }
    }


    public void Show(Image target, float time = .2f)
    {
        if (target == null) { Debug.LogError("target is null"); return; }

        this.mAnimTime = time;
        this.mTarget = target;

        Image image = GetComponent<Image>();
        if (image == null)
            image = gameObject.AddComponent<Image>();
        if (mMaterial == null)
        {
            mMaterial = new Material(Shader.Find("NextFramework/Guidence/Rectangle"));
            mMaterial.color = new Color(0f, 0f, 0f, .5f);
        }
        image.material = mMaterial;


        mTarget.rectTransform.GetWorldCorners(mCorners);
        mFinalOffsetX = Vector2.Distance(WorldToCanvasPos(mCanvas, mCorners[0]), WorldToCanvasPos(mCanvas, mCorners[3])) / 2f + mBorder;
        mFinalOffsetY = Vector2.Distance(WorldToCanvasPos(mCanvas, mCorners[0]), WorldToCanvasPos(mCanvas, mCorners[1])) / 2f + mBorder;
        //计算高亮显示区域的中心
        float x = mCorners[0].x + ((mCorners[3].x - mCorners[0].x) / 2f);
        float y = mCorners[0].y + ((mCorners[1].y - mCorners[0].y) / 2f);
        Vector2 center = WorldToCanvasPos(mCanvas, new Vector3(x, y, 0));

        //设置遮罩材料中中心变量
        mMaterial.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));

        ResetCurOffset(center);
    }

    void ResetCurOffset(Vector3 center)
    {
        if (mRectRrans != null)
        {
            mRectRrans.GetWorldCorners(mCorners);
            //求偏移初始值
            for (int i = 0; i < mCorners.Length; i++)
            {
                if (i % 2 == 0)
                    mCurOffsetX = Mathf.Max(Vector3.Distance(WorldToCanvasPos(mCanvas, mCorners[i]), center), mCurOffsetX);
                else
                    mCurOffsetY = Mathf.Max(Vector3.Distance(WorldToCanvasPos(mCanvas, mCorners[i]), center), mCurOffsetY);
            }
        }
        //设置遮罩材质中当前偏移的变量
        mMaterial.SetFloat("_RangeX", mCurOffsetX);
        mMaterial.SetFloat("_RangeY", mCurOffsetY);
    }

    Vector2 WorldToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectRrans, world, mCanvasCamera, out position);
        return position;
    }
}
