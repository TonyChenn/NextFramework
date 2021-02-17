using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace NextFramework.SUGUI
{
    public class SButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField] bool m_TweenScale = true;

        [SerializeField] ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        //[SerializeField] ButtonDoubleClickEvent m_OnDoubleClick = new ButtonDoubleClickEvent();
        //[SerializeField] ButtonLongClickEvent m_OnLongClick = new ButtonLongClickEvent();


        public ButtonClickedEvent OnClick { get { return m_OnClick; } set { m_OnClick = value; } }
        //public ButtonDoubleClickEvent OnDoubleClick { get { return m_OnDoubleClick; } set { m_OnDoubleClick = value; } }
        //public ButtonLongClickEvent OnLongClick { get { return m_OnLongClick; } set { m_OnLongClick = value; } }

        /// <summary>
        /// 是否开启点击缩放
        /// </summary>
        public bool TweenScale
        {
            get { return m_TweenScale; }
            set
            {
                ButtonScale compoment = GetComponent<ButtonScale>();
                if (value && compoment == null)
                    gameObject.AddComponent<ButtonScale>();

                if (compoment) compoment.enabled = value;
            }
        }

        #region IPointerClickHandler
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }
        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }
        #endregion

        #region ISubmitHandler
        public void OnSubmit(BaseEventData eventData)
        {
            Press();

            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }
        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
        #endregion

        #region UnityEvent
        [Serializable] public class ButtonClickedEvent : UnityEvent { }
        [Serializable] public class ButtonDoubleClickEvent : UnityEvent { }
        [Serializable] public class ButtonLongClickEvent : UnityEvent { }
        #endregion
    }
}

