//-------------------------------------------------
//            TweenKit
// Copyright © 2020 tonychenn.cn
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the widget's size.
/// </summary>
namespace NextFramework
{
    [AddComponentMenu("NextFrameWork/TweenKit/Tween Width")]
    public class TweenWidth : UITweener
    {
        public int from = 100;
        public int to = 100;

        RectTransform mRect;

        public RectTransform cachedRect { get { if (mRect == null) mRect = GetComponent<RectTransform>(); return mRect; } }

        [System.Obsolete("Use 'value' instead")]
        public int width { get { return this.value; } set { this.value = value; } }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public int value { get { return (int)cachedRect.sizeDelta.x; } set { cachedRect.sizeDelta = new Vector2(value, cachedRect.sizeDelta.y); } }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.RoundToInt(from * (1f - factor) + to * factor);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public TweenWidth Begin(RectTransform rect, float duration, int width)
        {
            TweenWidth comp = UITweener.Begin<TweenWidth>(rect.gameObject, duration);
            comp.from = (int)rect.sizeDelta.x;
            comp.to = width;

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

