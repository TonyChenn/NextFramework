using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace NextFramework
{
    [Serializable]
    public class packages
    {
        public string name;
        public string author;
        public string version;
        public string url;
    }
    [Serializable]
    public class PackageDao
    {
        public float version;
        public List<packages> packageList = new List<packages>();
    }


    public class PackageMangerWnd : EditorWindow
    {
        UnityWebRequest webRequest;
        Vector2 scrollPos;
        IEnumerator thread;

        [MenuItem("NextFramework/PackageManger")]
        static void ShowPackageMangerWnd()
        {
            var packageMangerWnd = GetWindow<PackageMangerWnd>("包管理器", false);
            packageMangerWnd.position = new Rect(50, 100, 1000, 800);
            
        }
        private void OnEnable()
        {
            thread = DownLoadPackageJson("https://tonychenn.cn/list1.json");

            while (thread.MoveNext()) { }
        }
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("刷新"))
            {
                OnEnable();
            }
        }

        #region 下载包信息
        string mPackageJson = null;
        IEnumerator DownLoadPackageJson(string url)
        {
            webRequest = UnityWebRequest.Get(url);
            Debug.Log("正在检查版本信息");
            webRequest.SendWebRequest();

            if (webRequest == null) yield break;

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("包配置下载出错：" + webRequest.error);
                Debug.Log("尝试读取本地配置");
                mPackageJson = EditorPrefsHelper.PackageKitJson;

                if (string.IsNullOrEmpty(mPackageJson))
                {
                    Debug.Log("未找到本地缓存，连接到网络再试试吧");
                    yield break;
                }
            }
            else
            {
                if (webRequest.isDone)
                {
                    mPackageJson = webRequest.downloadHandler.text;
                    Debug.Log(mPackageJson);

                    var obj = JsonUtility.FromJson<PackageDao>(mPackageJson);
                    Debug.Log("服务器版本：" + obj.version);

                    if (obj.version > EditorPrefsHelper.PackageKitVersion)
                    {
                        EditorPrefsHelper.PackageKitJson = mPackageJson;
                        EditorPrefsHelper.PackageKitVersion = obj.version;
                        Debug.Log("更新成功,当前版本：" + obj.version);
                    }
                }
            }

        }
        #endregion
    }
}

