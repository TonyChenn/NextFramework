﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    public class SUGUIMenu : MonoBehaviour
    {
        [MenuItem("NextFramework/SUGUI/Create SText", false, 11)]
        public static SText CreateSText()
        {
            SText txt = SUGUISetting.AddText(getParent());
            txt.alignment = TextAnchor.MiddleLeft;
            txt.rectTransform.sizeDelta = new Vector2(200, 60);
            return txt;
        }

        [MenuItem("NextFramework/SUGUI/Create SImage", false, 12)]
        public static SImage CreateSImage()
        {
            SImage image = SUGUISetting.AddImage(getParent());
            return image;
        }
        [MenuItem("NextFramework/SUGUI/Create STexture", false, 13)]
        public static STexture CreateSTexture()
        {
            STexture texture = SUGUISetting.AddTexture(getParent());
            return texture;
        }

        [MenuItem("NextFramework/SUGUI/Create SButton", false, 14)]
        public static SButton CreateSButton()
        {
            SButton btn = SUGUISetting.AddButton(getParent());

            return btn;
        }

        [MenuItem("NextFramework/SUGUI/Create SInputFied", false, 15)]
        public static void CreateSInputFied()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SListView", false, 16)]
        public static void CreateSListView()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SGridView", false, 17)]
        public static void CreateSGridView()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SPageView", false, 18)]
        public static void CreateSPageView()
        {

        }

        [MenuItem("NextFramework/SUGUI/Create SChatView", false, 19)]
        public static void CreateSChatView()
        {

        }
        [MenuItem("NextFramework/SUGUI/Create Canvas", false, 20)]
        public static void CreateCanvas()
        {
            SUGUISetting.AddCanvas(null);
        }
        [MenuItem("NextFramework/SUGUI/Create SMask", false, 21)]
        public static void CreateMask()
        {
            SUGUISetting.AddMask(getParent());
        }

        public static void SetParentAndAlign(GameObject parent, GameObject child)
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

        /// <summary>
        /// return create new obj's parent
        /// if select a gameObject return it,or return Cnavas
        /// </summary>
        /// <returns></returns>
        static GameObject getParent()
        {
            GameObject parent = Selection.activeGameObject;
            if (parent == null)
                parent = SUGUISetting.GetCanvas(parent).gameObject;

            return parent;
        }
    }
}
