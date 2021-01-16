using System;
using UnityEngine;
using UnityEngine.UI;

namespace NextFramework
{
    public class CircleGuidence : MonoBehaviour
    {
        [SerializeField] public MaskableGraphic mTarget;
        [Range(0, 1)]
        [SerializeField] public float mAnimTime = .3f;         //遮罩收缩速度
        [Range(0, 10)]
        [SerializeField] float mBorder = 10f;


        Camera mCanvasCamera;
        Vector3[] mCorners = new Vector3[4];
        float mCurRadius;                       //当前镂空区域半径
        float mFinalRadius;                     //最终镂空区域半径
        Material mMaterial;
        RectTransform rectTransform;
        Canvas mCanvas;

        private void Awake()
        {
            mCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            rectTransform = mCanvas.transform as RectTransform;
            mCanvasCamera = mCanvas.GetComponent<Camera>();
        }

        float shrinkSpeed = 0f;
        void Update()
        {
            //从当前半径到目标半径差值显示收缩动画
            float value = Mathf.SmoothDamp(mCurRadius, mFinalRadius, ref shrinkSpeed, mAnimTime);
            if (!Mathf.Approximately(value, mCurRadius))
            {
                mCurRadius = value;
                mMaterial.SetFloat("_Range", mCurRadius);
            }
        }

        public void Show(MaskableGraphic target, float border = 10f, float time = .2f)
        {
            if (target == null && mTarget == null) { Debug.LogError("target is null"); return; }

            this.mBorder = border;
            this.mAnimTime = time;
            this.mTarget = target;

            Image image = GetComponent<Image>();
            if (image == null)
                image = gameObject.AddComponent<Image>();
            if (mMaterial == null)
            {
                mMaterial = new Material(Shader.Find("NextFramework/Guidence/Circle"));
                mMaterial.color = new Color(0f, 0f, 0f, .5f);
            }
            image.material = mMaterial;


            //target的四个顶点世界坐标
            mTarget.rectTransform.GetWorldCorners(mCorners);
            mFinalRadius = Vector2.Distance(WorldToCanvasPoint(mCanvas, mCorners[0]),
                                    WorldToCanvasPoint(mCanvas, mCorners[2])) / 2f + mBorder;
            float width = mCorners[3].x - mCorners[0].x;
            float height = mCorners[1].y - mCorners[0].y;
            //镂空圆心坐标
            Vector2 centerPos = WorldToCanvasPoint(mCanvas,
                                new Vector2(mCorners[0].x + width / 2f, mCorners[0].y + height / 2f));
            mMaterial.SetVector("_Center", new Vector4(centerPos.x, centerPos.y, 0, 0));

            ResetCurOffset(centerPos);
        }
        void ResetCurOffset(Vector3 center)
        {
            //计算canvas四个顶点到圆心的最大坐标，作为镂空区域最大半径
            if (rectTransform != null)
            {
                rectTransform.GetWorldCorners(mCorners);
                foreach (Vector3 corner in mCorners)
                {
                    mCurRadius = Math.Max(Vector3.Distance(WorldToCanvasPoint(mCanvas, corner), center), mCurRadius);
                }
                mMaterial.SetFloat("_Range", mCurRadius);
            }
        }

        Vector3 WorldToCanvasPoint(Canvas canvas, Vector3 worldPos)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, worldPos, mCanvasCamera, out position);
            return position;
        }
    }
}

