using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace NextFramework.SUGUI
{
    public static class UGUITools
    {
        static Dictionary<System.Type, string> mTypeNames = new Dictionary<System.Type, string>();

        public static GameObject AddChild(this GameObject parent, bool undo = true)
        {
            return AddChild(parent.transform, undo);
        }
        public static GameObject AddChild(this Transform parent, bool undo = true)
        {
            var go = new GameObject();
#if UNITY_EDITOR
            if (undo && !Application.isPlaying)
                UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.ResetGameObject();
            }
            return go;
        }

        public static GameObject AddChild(this GameObject parent, GameObject prefab)
        {
            return AddChild(parent.transform, prefab);
        }
        public static GameObject AddChild(this Transform parent, GameObject prefab)
        {
            var go = UnityEngine.Object.Instantiate(prefab, parent.transform);
            var t = go.transform;
            t.parent = parent;
            t.ResetGameObject();
            return go;
        }

        public static T AddChild<T>(this GameObject parent, bool undo = true) where T : Component
        {
            GameObject go = AddChild(parent, undo);
            string name;
            if (!mTypeNames.TryGetValue(typeof(T), out name) || name == null)
            {
                string[] temp = typeof(T).ToString().Split('.');
                name = temp[temp.Length - 1];
                mTypeNames[typeof(T)] = name;
            }
            go.name = name;
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = go;
#endif
            return go.AddComponent<T>();
        }

        /// <summary>
        /// Destory All Childs
        /// </summary>
        public static void DestoryChilds(this GameObject gameObject)
        {
            DestoryChilds(gameObject.transform);
        }
        public static void DestoryChilds(this Transform transform)
        {
            bool isPlaying = Application.isPlaying;
            while (transform.childCount > 0)
            {
                var child = transform.GetChild(0);
                if (isPlaying)
                {
                    child.parent = null;
                    Object.Destroy(child.gameObject);
                }
                else Object.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// Reset GameObject's pos, rotation, scale
        /// </summary>
        public static GameObject ResetGameObject(this GameObject gameObject)
        {
            return ResetGameObject(gameObject.transform);
        }
        public static GameObject ResetGameObject(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            return transform.gameObject;
        }

        /// <summary>
        /// Finds the specified component on the game object or one of its parents.
        /// This function has become obsolete with Unity 4.3.
        /// </summary>

        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;

            return FindInParents<T>(go.transform);
        }
        static public T FindInParents<T>(Transform trans) where T : Component
        {
            if (trans == null) return null;

            UnityEngine.Profiling.Profiler.BeginSample("Editor-only GC allocation (GetComponent)");
            var comp = trans.GetComponentInParent<T>();
            UnityEngine.Profiling.Profiler.EndSample();
#if UNITY_FLASH
		return (T)comp;
#else
            return comp;
#endif
        }


        /// <summary>
        /// return child obj is child of parent obj
        /// </summary>
        public static bool IsChild(Transform parent, Transform child)
        {
            return child.IsChildOf(parent);
        }


        public static void SetActive(GameObject gameObject, bool show)
        {
            if (gameObject)
            {
                if (gameObject.activeSelf != show)
                {
                    gameObject.SetActive(show);
                }
            }
        }

        /// <summary>
        /// Find all active objects of specified type.
        /// </summary>
        static public T[] FindActive<T>() where T : Component
        {
            return GameObject.FindObjectsOfType(typeof(T)) as T[];
        }

        /// <summary>
        /// Helper function that returns whether the specified MonoBehaviour is active.
        /// </summary>
        static public bool GetActive(GameObject go)
        {
            return go && go.activeInHierarchy;
        }
    }
}
