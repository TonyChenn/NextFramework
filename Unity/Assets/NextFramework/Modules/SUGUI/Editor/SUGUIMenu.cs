using System;
using UnityEditor;
using UnityEngine;

namespace NextFramework.UI
{
    public class SUGUIMenu
    {
        #region Scene 窗口右键菜单

        [InitializeOnLoadMethod]
        static void Init()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView obj)
        {
            if (Selection.gameObjects.Length != 1) return;

            GameObject go = Selection.gameObjects[0];
            if (go.GetComponentInParent<Canvas>() == null)
                return;


            Event e = Event.current;
            if (e != null && e.button == 1 && e.type == EventType.MouseUp)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create/SText"), false, () => CreateSText());
                menu.AddItem(new GUIContent("Create/SImage"), false, () => CreateSImage());
                menu.AddItem(new GUIContent("Create/STexture"), false, () => CreateSTexture());
                menu.AddItem(new GUIContent("Create/SButton"), false, () => CreateSButton());
                menu.AddItem(new GUIContent("Create/SInputFied"), false, () => CreateSInputFied());
                menu.AddItem(new GUIContent("Create/SListView"), false, () => CreateSListView());
                menu.AddItem(new GUIContent("Create/SGridView"), false, () => CreateSGridView());
                menu.AddItem(new GUIContent("Create/SPageView"), false, () => CreateSPageView());
                menu.AddItem(new GUIContent("Create/SChatView"), false, () => CreateSChatView());
                menu.AddItem(new GUIContent("Create/SMask"), false, () => CreateMask());
                menu.ShowAsContext();
            }
        }
        #endregion

        [MenuItem("Tools/SUGUI/Create SText", false, 11)]
        public static SText CreateSText()
        {
            SText txt = SUGUISetting.AddText(getParent());
            txt.alignment = TextAnchor.MiddleLeft;
            txt.rectTransform.sizeDelta = new Vector2(200, 60);
            return txt;
        }

        [MenuItem("Tools/SUGUI/Create SImage", false, 12)]
        public static SImage CreateSImage()
        {
            SImage image = SUGUISetting.AddImage(getParent());
            return image;
        }
        [MenuItem("Tools/SUGUI/Create STexture", false, 13)]
        public static STexture CreateSTexture()
        {
            STexture texture = SUGUISetting.AddTexture(getParent());
            return texture;
        }

        [MenuItem("Tools/SUGUI/Create SButton", false, 14)]
        public static SButton CreateSButton()
        {
            SButton btn = SUGUISetting.AddButton(getParent());

            return btn;
        }

        [MenuItem("Tools/SUGUI/Create SInputFied", false, 15)]
        public static void CreateSInputFied()
        {

        }

        [MenuItem("Tools/SUGUI/Create SListView", false, 16)]
        public static void CreateSListView()
        {

        }

        [MenuItem("Tools/SUGUI/Create SGridView", false, 17)]
        public static void CreateSGridView()
        {

        }

        [MenuItem("Tools/SUGUI/Create SPageView", false, 18)]
        public static void CreateSPageView()
        {

        }

        [MenuItem("Tools/SUGUI/Create SChatView", false, 19)]
        public static void CreateSChatView()
        {

        }
        [MenuItem("Tools/SUGUI/Create Canvas", false, 20)]
        public static void CreateCanvas()
        {
            SUGUISetting.AddCanvas(null);
        }
        [MenuItem("Tools/SUGUI/Create SMask", false, 21)]
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

