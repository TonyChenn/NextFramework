using UnityEngine;
using System.Collections;

namespace NextFramework.UI
{
    public class UI_ToastPanel : MonoBehaviour
    {
        [SerializeField] SImage Image;
        [SerializeField] SText Tip;

        private TweenScale m_TweenScale = null;

        private void Awake()
        {
            Image.gameObject.SetActive(false);
            Messenger<string, float>.AddListener(MessengerEventDef.Str_ShowToast, ShowToast);
        }

        private void OnDestroy()
        {
            Messenger<string, float>.RemoveListener(MessengerEventDef.Str_ShowToast, ShowToast);
        }

        private void ShowToast(string msg, float duration)
        {
            Tip.text = msg;
            if (m_TweenScale == null)
            {
                m_TweenScale = TweenScale.Begin(Image.gameObject, duration, Vector3.one);
                m_TweenScale.cachedTransform.localScale = new Vector3(1f, 0.01f, 1f);
                m_TweenScale.SetStartToCurrentValue();
                m_TweenScale.SetOnFinished(() => { StartCoroutine(ShowFinishHandler(duration)); });
            }
            else
            {
                m_TweenScale.ResetToBeginning();
            }

            if (!Image.gameObject.activeSelf)
                Image.gameObject.SetActive(true);
            m_TweenScale.PlayForward();
        }

        IEnumerator ShowFinishHandler(float duration)
        {
            yield return new WaitForSeconds(duration);
            Image.gameObject.SetActive(false);
        }
    }  
}

