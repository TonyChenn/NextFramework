using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace NextFramework
{
    public class WebServer : MonoSinglton<WebServer>
    {
        private WebServer() { }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerPath
        {
            get { return PackageConfig.Singlton.CurPackageInfo.ServerPath; }
        }
        #region Get Data
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url">绝对地址</param>
        /// <param name="result"></param>
        public void GetDataByUrl(string url, Action<UnityWebRequest> result)
        {
            StartCoroutine(GetData(url, result));
        }
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <param name="result"></param>
        public void GetDataByPath(string path, Action<UnityWebRequest> result)
        {
            GetDataByUrl(PathHelper.CombinePath(Singlton.ServerPath, path), result);
        }
        IEnumerator GetData(string url, Action<UnityWebRequest> result)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
                Debug.Log(string.Format("Get error: {0}, url: {1}", request.error, url));
            else
            {
                if (result != null) result(request);
            }
        }
        #endregion


        #region Post Data
        public void Post(string url, Hashtable data, Action<UnityWebRequest> result)
        {
            WWWForm form = new WWWForm();
            foreach (string key in data)
                form.AddField(key, data[key].ToString());

            StartCoroutine(PostData(url, form, result));
        }
        public void PostFile(string url, Hashtable data, byte[] file,Action<UnityWebRequest> result)
        {
            WWWForm form = new WWWForm();
            foreach (string key in data)
                form.AddField(key, data[key].ToString());

            form.AddBinaryData("file",file);

            StartCoroutine(PostData(url, form, result));
        }

        IEnumerator PostData(string url, WWWForm form, Action<UnityWebRequest> result)
        {
            var request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
                Debug.Log(string.Format("Post error: {0}, url: {1}", request.error, url));
            else
            {
                if (result != null) result(request);
            }
        }
        #endregion
    }
}