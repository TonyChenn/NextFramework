//-------------------------------------------------
//            TweenKit
// Copyright © 2020 tonychenn.cn
//-------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace NextFramework.UI
{
    /// <summary>
    /// Tween the object's color.
    /// </summary>

    [AddComponentMenu("NextFrameWork/TweenKit/Tween Color")]
    public class TweenColor : UITweener
    {
        public Color from = Color.white;
        public Color to = Color.white;

        bool mCached = false;
        MaskableGraphic mMaskableGraphic;
        Material mMat;
        Light mLight;

        void Cache()
        {
            mCached = true;
            mMaskableGraphic = GetComponent<MaskableGraphic>();
            Renderer ren = gameObject.GetComponent<Renderer>();
            if (ren != null) mMat = ren.material;
            mLight = gameObject.GetComponent<Light>();
            if (mMaskableGraphic == null && mMat == null && mLight == null)
                mMaskableGraphic = GetComponentInChildren<MaskableGraphic>();
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>
        public Color value
        {
            get
            {
                if (!mCached) Cache();
                if (mMaskableGraphic != null) return mMaskableGraphic.color;
                if (mLight != null) return mLight.color;
                if (mMat != null) return mMat.color;
                return Color.black;
            }
            set
            {
                if (!mCached) Cache();
                if (mMaskableGraphic != null) mMaskableGraphic.color = value;
                if (mMat != null) mMat.color = value;

                if (mLight != null)
                {
                    mLight.color = value;
                    mLight.enabled = (value.r + value.g + value.b) > 0.01f;
                }
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>
        protected override void OnUpdate(float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>
        static public TweenColor Begin(GameObject go, float duration, Color color)
        {
    #if UNITY_EDITOR
            if (!Application.isPlaying) return null;
    #endif
            TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
            comp.from = comp.value;
            comp.to = color;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue() { from = value; }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue() { to = value; }

        [ContextMenu("Assume value of 'From'")]
        void SetCurrentValueToStart() { value = from; }

        [ContextMenu("Assume value of 'To'")]
        void SetCurrentValueToEnd() { value = to; }
    }
}

