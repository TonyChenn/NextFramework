using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    public class SUGUIMenu : MonoBehaviour
    {
        [MenuItem("NextFramework/SUGUI/Create SText", false,11)]
        public static void CreateSText()
        {
            GameObject go;
        }

        [MenuItem("NextFramework/SUGUI/Create SImage", false, 12)]
        public static void CreateSImage()
        {
            GameObject parent = Selection.activeGameObject;
            if(parent==null)
            {
                //parent= 
            }

            GameObject go = new GameObject("SImage");
            go.AddComponent<SImage>();

        }

        [MenuItem("NextFramework/SUGUI/Create SButton", false, 13)]
        public static void CreateSButton()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SInputFied", false, 14)]
        public static void CreateSInputFied()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SListView", false, 15)]
        public static void CreateSListView()
        {
            
        }

        [MenuItem("NextFramework/SUGUI/Create SGridView", false, 16)]
        public static void CreateSGridView()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SPageView", false, 6)]
        public static void CreateSPageView()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SChatView", false, 6)]
        public static void CreateSChatView()
        {

        }

        public static void CreateCanvas()
        {
            GameObject go = new GameObject("Canvas");
            Canvas canvas = go.AddComponent<Canvas>();
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            
        }

        public static void SetParentAndAlign(GameObject parent,GameObject child)
        {
            if (parent == null)
                return;

            Undo.SetTransformParent(child.transform, parent.transform, "");

            RectTransform rectTransform = child.transform as RectTransform;
            if (rectTransform)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            }
            else
            {
                child.transform.localPosition = Vector3.zero;
            }
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            SetLayerRecursively(child, parent.layer);
        }
        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }
    }
}

