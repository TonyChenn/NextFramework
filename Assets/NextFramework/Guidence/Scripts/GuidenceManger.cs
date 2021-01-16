using UnityEngine;
using UnityEngine.UI;

namespace NextFramework
{
    public enum GuideType { Circle, Rect }

    public class GuidenceManger : MonoBehaviour, ICanvasRaycastFilter
    {
        [SerializeField] GuideType mGuideType = GuideType.Circle;
        [Range(0, 10)]
        [SerializeField] float mShrinkTime = .3f;
        [Range(1, 10)]
        [SerializeField] float mBorder = 10f;

        CircleGuidence mCircleGuidence;
        RectGuidence mRectGuidence;

        MaskableGraphic mTarget;

        private void Awake()
        {
            if (mCircleGuidence == null)
                mCircleGuidence = gameObject.AddComponent<CircleGuidence>();

            mRectGuidence = GetComponent<RectGuidence>();
            if (mRectGuidence == null)
                mRectGuidence = gameObject.AddComponent<RectGuidence>();
        }

        /// <summary>
        /// 显示遮罩
        /// </summary>
        /// <param name="type">遮罩类型</param>
        /// <param name="target">遮罩目标</param>
        public void ShowGuidence(MaskableGraphic target)
        {
            ShowGuidence(mGuideType, target, mBorder, mShrinkTime);
        }
        public void ShowGuidence(GuideType guideType, MaskableGraphic target, float border, float shrinkTime)
        {
            mTarget = target;

            switch (guideType)
            {
                case GuideType.Circle:
                    mCircleGuidence.Show(target, border, shrinkTime);
                    break;
                case GuideType.Rect:
                    mRectGuidence.Show(target, border, shrinkTime);
                    break;
                default:
                    break;
            }
        }

        public void Close()
        {

        }
        #region Event Filter
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (mTarget == null) return false;
            return RectTransformUtility.RectangleContainsScreenPoint(mTarget.rectTransform, sp, eventCamera);
        }
        #endregion
    }
}

