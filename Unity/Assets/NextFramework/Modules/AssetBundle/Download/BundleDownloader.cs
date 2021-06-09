using NextFramework;
using NextFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Common
{
    public partial class BundleDownloader : NormalSingleton<BundleDownloader>
    {
        public static int MaxRetryCount = 2,
                          MaxDownloadThreadsCount = 10,
                          MaxDecompressThreadsCount = 10,
                          TimeOut = 30,
                          AllowMinSpeed = 10 * 1024,
                          CountToWriteAllList = 300;

        public static string AppVersionKey = "GameDemo.App.Version",
                             VersionName = "version.dat",
                             ListInfoName = "listinfo.dat",
                             DownloadedBundlesFile = "finishedbundles.dat";

        static string[] ListFileNames = { "main.dat", "art.dat", "music.dat" };

        string m_versionContent;
        List<BundleItem>[] m_bundles = new List<BundleItem>[ListFileNames.Length];
        Dictionary<string, BundleItem>[] m_dicBundles = new Dictionary<string, BundleItem>[ListFileNames.Length];
        Queue<BundleItem> m_bundle2Download = new Queue<BundleItem>();
        List<BundleItem> m_bundleDownloaded = new List<BundleItem>();
        ulong m_downloadedBytes;
        StringBuilder m_sb = new StringBuilder();
        DateTime m_startDownloadTime;
        List<int> m_downloadingThreads;
        int m_maxThreadID;
        Dictionary<int, int> m_retryCount = new Dictionary<int, int>();
        Dictionary<int, Stage> m_threadStages = new Dictionary<int, Stage>();
        DownloadStyle m_downloadStyle = DownloadStyle.DownloadPartial;

        int m_decompressedCount;
        int m_decompressTotalCount;


        #region BundleItem & BundleState & Stage & DownloadStyle

        class BundleItem
        {
            public string Name;
            public string Hash;
            public int Length;
            public bool DownloadWhenUse;
            public BundleState State;

            public string StrDownloadWhenUse
            {
                get { return DownloadWhenUse ? "0" : "1"; }
            }

            public bool NeedDownload
            {
                get
                {
                    bool needDownload = State == BundleState.Added || State == BundleState.Modified;
                    if (BundleDownloader.Singlton.m_downloadStyle == DownloadStyle.ForceCheckAndDownloadAll)
                    {
                        needDownload = needDownload || State == BundleState.DownloadWhenUse;
                    }

                    return needDownload;
                }
            }

            public BundleItem(string name, string hash, int length, string strDownloadType)
            {
                Name = name;
                Hash = hash;
                Length = length > 0 ? length : 1024 * 100;
                DownloadWhenUse = strDownloadType == "0";            // "0"表示动态下载，其它为登陆时下载
                State = BundleState.None;
            }
        }

        enum BundleState
        {
            None,
            Added,              // 增
            Removed,            // 删
            Modified,           // 改
            NoChange,           // 不变
            DownloadWhenUse,    // 使用时再下载
        }

        public enum Stage
        {
            Decompressing,          // 解压中
            DecompressFinished,     // 解压完成

            Checking,               // 检查是否有更新
            CheckSucceed,           // 检查完毕
            CheckFailed,            // 检查失败

            Downloading,            // 下载中
            DownloadFailed,         // 下载失败
            DownloadSucceed,        // 所有文件下载完成
            WriteFileFailed,        // 写文件失败

            Retry,                  // 重新连接中
        }

        public enum DownloadStyle
        {
            DownloadPartial,                // 只下载非动态加载的资源
            ForceCheckAndDownloadAll,       // 强制检查所有list文件，并下载所有资源
        }

        #endregion


        #region Properties

        bool NeedDecompress
        {
            get
            {
                string savedVersion = PlayerPrefs.GetString(AppVersionKey);
                bool firstInstall = savedVersion != Application.version;
                if (firstInstall)
                    return true;

                string persistVersionPath = BundleLoader.GetAssetBundlePersistPath(VersionName);
                return !File.Exists(persistVersionPath);
            }
        }

        public float DecompressProcessPercent
        {
            get { return (float)m_decompressedCount / m_decompressTotalCount; }
        }

        string RootUrl
        {
            get
            {
                return string.Format("{0}/AssetBundle/{1}/", GameConfig.Singlton.FtpUrl, GameConfig.Singlton.CurPackageEnum.ToString());
            }
        }

        public Stage CurrentStage
        {
            get
            {
                Stage mainThreadStage;
                if (m_threadStages.TryGetValue(0, out mainThreadStage))
                    return mainThreadStage;
                return Stage.Checking;
            }
        }

        public string Message
        {
            set;
            get;
        }

        public ulong NeedDownloadBytes
        {
            private set;
            get;
        }

        public ulong DownloadedBytes
        {
            private set
            {
                m_downloadedBytes = value;
                if (NeedDownloadBytes > 0)
                    DownloadingProgress = (float)m_downloadedBytes / NeedDownloadBytes;
            }

            get
            {
                return m_downloadedBytes;
            }
        }

        public float DownloadingProgress
        {
            set;
            get;
        }

        public float CheckingProgress
        {
            set;
            get;
        }

        public bool NeedDownload
        {
            get { return m_bundle2Download != null && m_bundle2Download.Count > 0; }
        }

        #endregion


        #region static

        public static void ClearAssetBundleCache()
        {
            UICoroutine.Singlton.StopAllCoroutines();

            string path = Application.persistentDataPath;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static void ReadLocalVersion(Action<string> callback)
        {
            if (callback == null)
                return;

            IEnumerator etor = YieldReadLocalVersion(callback);
            UICoroutine.Singlton.StartCoroutine(etor);
        }

        static IEnumerator YieldReadLocalVersion(Action<string> callback)
        {
            if (callback == null)
                yield break;

            string url = BundleLoader.GetAssetBundlePath(VersionName);
            WebServer.Singlton.GetDataByPath(VersionName, (data) =>
            {
                if (string.IsNullOrEmpty(data.error))
                {
                    string result = data.downloadHandler.text;
                    Log.Debug(result);
                    callback(result);
                }
            });
            //using (WWW www = new WWW(url))
            //{
            //    yield return www;
            //    string version = string.IsNullOrEmpty(www.error) ? www.text : "1.1.1";
            //    callback(version);
            //}
        }

        /// <summary>
        /// 比较 version 文件
        /// </summary>
        /// <returns>list 是否需要重新下载</returns>
        static bool[] CompareVersion(string netVersion, string localVersion)
        {
            string[] netVersions = netVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            bool[] result = new bool[3] { true, true, true };

            if (string.IsNullOrEmpty(localVersion))
                return result;

            string[] locVersions = localVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < 3; i++)
            {
                string locV = i < locVersions.Length ? locVersions[i] : null;
                string netV = netVersions[i];
                result[i] = netV != locV;
            }

            return result;
        }

        // 解析 ftp 服务器上的 list 文件
        static List<BundleItem> ReadNetListFile(string content, ref Dictionary<string, BundleItem> dic)
        {
            var items = new List<BundleItem>();

            if (string.IsNullOrEmpty(content))
                return items;

            string[] lines = content.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] words = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string name = words.Length > 0 ? words[0] : null;
                if (!string.IsNullOrEmpty(name))
                {
                    string hash = words.Length > 1 ? words[1] : "";
                    string lenString = words.Length > 2 ? words[2] : "";
                    int len;
                    int.TryParse(lenString, out len);
                    string strDownloadType = words.Length > 3 ? words[3] : "";
                    BundleItem item = new BundleItem(name, hash, len, strDownloadType);
                    items.Add(item);
                    if (dic != null)
                        dic[item.Name] = item;
                }
            }

            return items;
        }

        // 解析本地的 list 文件
        static List<BundleItem> ReadLocalListFile(string content, ref Dictionary<string, BundleItem> dic, Dictionary<string, BundleItem> dicDownloadedB)
        {
            var items = new List<BundleItem>();

            if (string.IsNullOrEmpty(content))
                return items;

            string[] lines = content.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] words = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string name = words.Length > 0 ? words[0] : null;
                if (!string.IsNullOrEmpty(name))
                {
                    string hash = words.Length > 1 ? words[1] : "";
                    string strDownloadType = words.Length > 2 ? words[2] : "";
                    BundleItem item = new BundleItem(name, hash, 0, strDownloadType);

                    if (strDownloadType == "1")                         // "1" 表示此资源已OK
                    {
                        item.State = BundleState.NoChange;
                    }
                    else
                    {
                        BundleItem b;
                        dicDownloadedB.TryGetValue(item.Name, out b);
                        if (b != null)
                        {
                            // 说明此资源已下好，只是没更新 list 文件
                            item.State = BundleState.NoChange;
                            item.Hash = b.Hash;
                        }
                        else
                        {
                            item.State = BundleState.DownloadWhenUse;
                        }
                    }

                    items.Add(item);
                    if (dic != null)
                        dic[item.Name] = item;
                }
            }

            return items;
        }

        // 解析已下载资源缓存文件
        static Dictionary<string, BundleItem> ReadDownloadedBundleFile()
        {
            var dic = new Dictionary<string, BundleItem>();
            string path = BundleLoader.GetAssetBundlePersistPath(DownloadedBundlesFile);
            if (!File.Exists(path))
                return dic;

            string content = File.ReadAllText(path);
            string[] lines = content.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] words = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string name = words.Length > 0 ? words[0] : null;
                if (!string.IsNullOrEmpty(name))
                {
                    string hash = words.Length > 1 ? words[1] : "";
                    BundleItem item = new BundleItem(name, hash, 0, null);
                    dic[item.Name] = item;

                }
            }
            return dic;
        }

        // 对比 list 文件，标记 bundle 状态
        static List<BundleItem> MarkBundles(string netListContent, string persistListContent, out Dictionary<string, BundleItem> dicBundles, Dictionary<string, BundleItem> downloadedBundles)
        {
            if (string.IsNullOrEmpty(netListContent))
                throw new ArgumentNullException("netListContent");

            dicBundles = new Dictionary<string, BundleItem>();
            var netList = ReadNetListFile(netListContent, ref dicBundles);
            if (string.IsNullOrEmpty(persistListContent))
            {
                for (int i = 0; i < netList.Count; i++)
                {
                    var netItem = netList[i];
                    netItem.State = netItem.DownloadWhenUse ? BundleState.DownloadWhenUse : BundleState.Added;
                }
                return netList;
            }

            var dicPersist = new Dictionary<string, BundleItem>();
            var persistList = ReadLocalListFile(persistListContent, ref dicPersist, downloadedBundles);
            for (int i = 0; i < netList.Count; i++)
            {
                BundleItem netItem = netList[i];
                BundleItem persistItem;
                dicPersist.TryGetValue(netItem.Name, out persistItem);

                if (persistItem == null)
                {
                    netItem.State = netItem.DownloadWhenUse ? BundleState.DownloadWhenUse : BundleState.Added;
                }
                else if (persistItem.Hash != netItem.Hash)
                {
                    netItem.State = netItem.DownloadWhenUse ? BundleState.DownloadWhenUse : BundleState.Modified;
                }
                else
                {
                    netItem.State = netItem.DownloadWhenUse ? persistItem.State : BundleState.NoChange;
                }
            }

            for (int i = 0; i < persistList.Count; i++)
            {
                BundleItem localItem = persistList[i];
                BundleItem netItem;
                dicBundles.TryGetValue(localItem.Name, out netItem);
                if (netItem == null)
                {
                    localItem.State = BundleState.Removed;
                    netList.Add(localItem);
                    dicBundles[localItem.Name] = localItem;
                }
            }

            return netList;
        }

        // 保存 list 文件，将保存所有的资源的信息
        static bool WriteListFile(string listFileName, List<BundleItem> items, StringBuilder sb, out string error)
        {
            error = null;
            sb.Length = 0;
            for (int i = 0; i < items.Count; i++)
            {
                BundleItem item = items[i];
                string isOk;
                string hash;

                switch (item.State)
                {
                    case BundleState.None:
                        string errorThrow = string.Format("Unexptected bundle state, bundle: {0}, state: {1}", item.Name, item.State);
                        throw new InvalidOperationException(errorThrow);

                    case BundleState.Added:
                    case BundleState.Modified:
                        isOk = "0";
                        hash = "?";
                        break;

                    case BundleState.DownloadWhenUse:
                        isOk = "0";
                        hash = item.Hash;
                        break;

                    case BundleState.NoChange:
                        isOk = "1";                 // "1" 表示资源已OK
                        hash = item.Hash;
                        break;

                    case BundleState.Removed:       // 此资源已删除，跳过
                        continue;

                    default:
                        throw new InvalidOperationException("Unknown state: " + item.State);
                }

                sb.AppendFormat("{0},{1},{2};", item.Name, hash, isOk);
            }

            string path = BundleLoader.GetAssetBundlePersistPath(listFileName);

            try
            {
                File.WriteAllText(path, sb.ToString());
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return true;
        }

        // 保存 bundle
        public static bool WriteBytes(string path, byte[] bytes, out string error)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception ex)
            {
                error = string.Format("Write bytes failed, path: {0}, error: {1}", path, ex.Message);
                Debug.LogError(error);
                return false;
            }

            error = null;
            return true;
        }

        #endregion


        public void CheckAsync(DownloadStyle style)
        {
            m_downloadStyle = style;
            CheckAsync();
        }

        public void CheckAsync()
        {
            IEnumerator etor = EtorCheck();
            UICoroutine.Singlton.StartCoroutine(etor);

            // clean
            Clean();
        }

        void Clean()
        {
            CheckingProgress = 0;
            DownloadingProgress = 0;
            m_downloadingThreads = null;
            m_bundleDownloaded.Clear();
        }

        public bool DownloadAsync()
        {
            // 开始下载
            if (NeedDownload)
            {
                m_threadStages.Clear();
                m_retryCount.Clear();
                SetState(Stage.Downloading, 0, null, null, NeedDownloadBytes, DownloadedBytes);

                if (m_downloadingThreads == null)
                {
                    m_startDownloadTime = DateTime.Now;
                    m_downloadingThreads = new List<int>();
                    m_maxThreadID = 1;
                }

                int count = MaxDownloadThreadsCount - m_downloadingThreads.Count;
                for (int i = 0; i < count; i++)
                    OpenDownloadThread(m_maxThreadID++);
                return true;
            }

            return false;
        }

        void OpenDownloadThread(int threadId)
        {
            if (!m_downloadingThreads.Contains(threadId))
                m_downloadingThreads.Add(threadId);
            IEnumerator etor = EtorDownload(threadId);
            UICoroutine.Singlton.StartCoroutine(etor);
        }

        IEnumerator EtorOpenDecompressThread(Queue<BundleItem> queueBundles)
        {
            while (queueBundles.Count > 0)
            {
                BundleItem bundle = queueBundles.Dequeue();
                string url = BundleLoader.GetAssetBundleStreamingUrl(bundle.Name);


                //WebServer.Singlton.GetDataByUrl(url, (data) =>
                //{
                //    if (string.IsNullOrEmpty(data.error))
                //    {
                //        string persistPath = BundleLoader.GetAssetBundlePersistPath(bundle.Name);
                //        string error;
                //        if (WriteBytes(persistPath, data.downloadHandler.data, out error))
                //            m_decompressedCount++;
                //        else
                //        {
                //            queueBundles.Enqueue(bundle);
                //            Debug.LogError(string.Format("Decompress failed, name:{0}  error:{1}", bundle.Name, error));
                //        }
                //    }
                //    else
                //    {
                //        queueBundles.Enqueue(bundle);
                //        Debug.LogError(string.Format("Decompress failed, name:{0}  error:{1}", bundle.Name, data.error));
                //    }
                //});



                using (WWW wwwBundle = new WWW(url))
                {
                    yield return wwwBundle;

                    if (string.IsNullOrEmpty(wwwBundle.error))
                    {
                        string persistPath = BundleLoader.GetAssetBundlePersistPath(bundle.Name);
                        string error;
                        if (WriteBytes(persistPath, wwwBundle.bytes, out error))
                            m_decompressedCount++;
                        else
                        {
                            queueBundles.Enqueue(bundle);
                            Debug.LogError(string.Format("Decompress failed, name:{0}  error:{1}", bundle.Name, error));
                        }
                    }
                    else
                    {
                        queueBundles.Enqueue(bundle);
                        Debug.LogError(string.Format("Decompress failed, name:{0}  error:{1}", bundle.Name, wwwBundle.error));
                    }
                }
            }
        }

        IEnumerator EtorDecompress()
        {
            m_decompressTotalCount = 0;
            SetState(Stage.Decompressing, 0);

            DateTime startTime = DateTime.Now;
            Dictionary<string, BundleItem> tempDic = null;

            // 读取 streaming 下的 list 文件
            List<BundleItem>[] allStreamingBundles = new List<BundleItem>[ListFileNames.Length];
            Queue<BundleItem> queueBundles = new Queue<BundleItem>();
            for (int i = 0; i < ListFileNames.Length; i++)
            {
                string listFileName = ListFileNames[i];
                string streamingListUrl = BundleLoader.GetAssetBundleStreamingUrl(listFileName);
                WWW wwwStreamingList = new WWW(streamingListUrl);

                yield return wwwStreamingList;

                var streamingBundles = ReadNetListFile(wwwStreamingList.text, ref tempDic);
                if (!string.IsNullOrEmpty(wwwStreamingList.error))
                    Debug.Log(string.Format("Get streaming list failed: {0}, file: {1}", wwwStreamingList.error, listFileName));
                wwwStreamingList.Dispose();

                allStreamingBundles[i] = streamingBundles;
                m_decompressTotalCount += streamingBundles.Count;
                for (int j = 0; j < streamingBundles.Count; j++)
                {
                    var sb = streamingBundles[j];
                    queueBundles.Enqueue(sb);
                    sb.State = BundleState.NoChange;
                }
            }

            // 解压
            m_decompressedCount = 0;
            for (int i = 0; i < MaxDecompressThreadsCount; i++)
            {
                IEnumerator etor = EtorOpenDecompressThread(queueBundles);
                UICoroutine.Singlton.StartCoroutine(etor);
            }

            while (m_decompressedCount < m_decompressTotalCount)
            {
                yield return null;
            }

            // list文件与缓存中的合并
            var downloadedBundles = ReadDownloadedBundleFile();
            for (int i = 0; i < ListFileNames.Length; i++)
            {
                string listFileName = ListFileNames[i];

                // 加载 persist 中的缓存
                string persistListUrl = BundleLoader.GetAssetBundlePersistUrl(listFileName);
                WWW wwwPersistList = new WWW(persistListUrl);
                yield return wwwPersistList;
                var persistBundles = ReadLocalListFile(wwwPersistList.text, ref tempDic, downloadedBundles);
                if (!string.IsNullOrEmpty(wwwPersistList.error))
                    Debug.Log(string.Format("Get persist list failed: {0}, file: {1}", wwwPersistList.error, listFileName));
                wwwPersistList.Dispose();

                // 与缓存中的list文件合并
                var streamingBundles = allStreamingBundles[i];
                var currentBundles = new List<BundleItem>(streamingBundles);
                for (int j = 0; j < persistBundles.Count; j++)
                {
                    BundleItem bundle = persistBundles[j];
                    if (currentBundles.All(cb => cb.Name != bundle.Name))
                        currentBundles.Add(bundle);
                }

                string writeError;
                bool succeed = WriteListFile(listFileName, currentBundles, m_sb, out writeError);
                if (!succeed)
                {
                    SetState(Stage.WriteFileFailed, 0, listFileName, writeError, 0, 0);
                    yield break;
                }
            }

            // 解压 version 文件
            string versionStreamingPath = BundleLoader.GetAssetBundleStreamingUrl(VersionName);
            WWW wwwVersion = new WWW(versionStreamingPath);
            yield return wwwVersion;
            string versionPersistPath = BundleLoader.GetAssetBundlePersistPath(VersionName);
            string error2;
            bool writeVersionSucceed = WriteBytes(versionPersistPath, wwwVersion.bytes, out error2);
            wwwVersion.Dispose();
            if (!writeVersionSucceed)
            {
                SetState(Stage.WriteFileFailed, 0, versionPersistPath, error2, 0, 0);
                yield break;
            }

            // 只在首次安装时解压
            PlayerPrefs.SetString(AppVersionKey, Application.version);
            PlayerPrefs.Save();

            SetState(Stage.DecompressFinished, 0);

            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - startTime;
            Debug.Log(string.Format("Decompress, start time: {0}, end time: {1}, total seconds: {2}, total count: {3}", startTime, endTime, ts.TotalSeconds, m_decompressTotalCount));
        }

        IEnumerator EtorCheck()
        {
            m_threadStages.Clear();

            if (NeedDecompress)
            {
                IEnumerator etor = EtorDecompress();
                while (etor.MoveNext())
                    yield return etor.Current;

                if (CurrentStage != Stage.DecompressFinished)
                    yield break;
            }

            SetState(Stage.Checking, 0, null, null, 0, 0);

            DateTime startTime = DateTime.Now;

            // 1. 下载version
            string versionUrl = RootUrl + VersionName + "?" + System.DateTime.Now.ToString();
            Debug.Log("Downloading version, url: " + versionUrl);
            using (UnityWebRequest uwrVersion = UnityWebRequest.Get(versionUrl))
            {
                yield return uwrVersion.SendWebRequest();
                if (uwrVersion.isHttpError || uwrVersion.isNetworkError)
                {
                    SetState(Stage.CheckFailed, 0, versionUrl, uwrVersion.error, 0, 0);
                    yield break;
                }
                m_versionContent = uwrVersion.downloadHandler.text;
                if (string.IsNullOrEmpty(m_versionContent))
                {
                    SetState(Stage.CheckFailed, 0, versionUrl, "Nothing in version", 0, 0);
                    yield break;
                }

                OnWWWSucceed(0);
            }

            // 2. 与本地version比较
            bool[] listNeedDownload;
            if (m_downloadStyle == DownloadStyle.ForceCheckAndDownloadAll)
            {
                listNeedDownload = new bool[ListFileNames.Length];
                for (int i = 0; i < listNeedDownload.Length; i++)
                {
                    listNeedDownload[i] = true;
                    m_bundles[i] = null;
                    m_dicBundles[i] = null;
                }
            }
            else
            {
                string localVersionUrl = BundleLoader.GetAssetBundlePersistUrl(VersionName);
                WWW wwwLocalVersion = new WWW(localVersionUrl);
                yield return wwwLocalVersion;
                if (!string.IsNullOrEmpty(wwwLocalVersion.error))
                    Debug.Log("Get local version failed: " + wwwLocalVersion.error);
                listNeedDownload = CompareVersion(m_versionContent, wwwLocalVersion.text);
                wwwLocalVersion.Dispose();
            }
            bool needDownloadList = listNeedDownload.Any(obj => obj);


            // 下载 listinfo
            int listinfoTotalSize = 0;
            if (needDownloadList)
            {
                string listinfoUrl = RootUrl + ListInfoName;
                Debug.Log("Downloading listinfo.dat, url: " + listinfoUrl);
                using (UnityWebRequest uwrItem = UnityWebRequest.Get(listinfoUrl))
                {
                    yield return uwrItem.SendWebRequest();
                    if (uwrItem.isHttpError || uwrItem.isNetworkError)
                    {
                        SetState(Stage.CheckFailed, 0, listinfoUrl, uwrItem.error, 0, 0);
                        yield break;
                    }
                    string text = uwrItem.downloadHandler.text;
                    OnDowloadedListInfo(text, listNeedDownload, out listinfoTotalSize);
                }
            }

            // 3. 下载有修改的list文件，并标记。或读取本地的list文件。
            var downloadedBundles = ReadDownloadedBundleFile();
            float listinfoDownloadedSize = 0;
            for (int i = 0; i < ListFileNames.Length; i++)
            {
                string listFileName = ListFileNames[i];
                string netListContent = null;
                bool needDownload = listNeedDownload[i];

                if (m_bundles[i] != null)
                    continue;

                if (needDownload)
                {
                    string listFileUrl = RootUrl + listFileName;
                    Debug.Log("Download list file, url: " + listFileUrl);

                    using (UnityWebRequest uwrListFile = UnityWebRequest.Get(listFileUrl))
                    {
                        uwrListFile.timeout = 60;
                        uwrListFile.SendWebRequest();

                        float blockTimer = TimeOut;
                        ulong downloadedBytes = 0;

                        while (!uwrListFile.isDone)
                        {
                            if (uwrListFile.isHttpError || uwrListFile.isNetworkError)
                            {
                                SetState(Stage.CheckFailed, 0, "Time out, " + listFileUrl, uwrListFile.error, 0, 0);
                                yield break;
                            }

                            if (downloadedBytes == uwrListFile.downloadedBytes)
                            {
                                // 检测是否超时
                                //Debug.Log("block time: " + blockTimer);
                                if (blockTimer < 0)
                                    uwrListFile.Abort();
                                else
                                    blockTimer -= Time.deltaTime;
                            }
                            else
                            {
                                downloadedBytes = uwrListFile.downloadedBytes;
                                blockTimer = TimeOut;
                                //Debug.Log(string.Format("#1 {0}   {1}   {2}", uwrItem.downloadedBytes, DownloadedBytes, item.Name));

                                // 刷新进度
                                CheckingProgress = (listinfoDownloadedSize + uwrListFile.downloadedBytes) / listinfoTotalSize;
                            }

                            yield return null;
                        }

                        if (uwrListFile.isHttpError || uwrListFile.isNetworkError)
                        {
                            SetState(Stage.CheckFailed, 0, listFileUrl, uwrListFile.error, 0, 0);
                            yield break;
                        }

                        // 刷新进度
                        listinfoDownloadedSize += (float)uwrListFile.downloadedBytes;
                        CheckingProgress = listinfoDownloadedSize / listinfoTotalSize;

                        // 下载的文本
                        netListContent = uwrListFile.downloadHandler.text;

                        OnWWWSucceed(0);

                        // compare and mark
                        string persistListUrl = BundleLoader.GetAssetBundlePersistUrl(listFileName);
                        List<BundleItem> netList;
                        Dictionary<string, BundleItem> dicNet;

                        using (WWW wwwPersistList = new WWW(persistListUrl))
                        {
                            yield return wwwPersistList;
                            if (!string.IsNullOrEmpty(wwwPersistList.error))
                                Debug.Log(string.Format("Get persist list failed: {0}, file: {1}", wwwPersistList.error, listFileName));
                            string newListContent = netListContent ?? wwwPersistList.text;
                            netList = MarkBundles(newListContent, wwwPersistList.text, out dicNet, downloadedBundles);
                        }
                        m_bundles[i] = netList;
                        m_dicBundles[i] = dicNet;
                        Debug.Log(string.Format("list file: {0}, count: {1}", listFileName, netList.Count));
                    }
                }
                else
                {
                    string persistListUrl = BundleLoader.GetAssetBundlePersistUrl(listFileName);
                    using (WWW wwwPersistList = new WWW(persistListUrl))
                    {
                        yield return wwwPersistList;
                        if (string.IsNullOrEmpty(wwwPersistList.error))
                        {
                            Dictionary<string, BundleItem> dicPersist = new Dictionary<string, BundleItem>();
                            var persistList = ReadLocalListFile(wwwPersistList.text, ref dicPersist, downloadedBundles);
                            m_bundles[i] = persistList;
                            m_dicBundles[i] = dicPersist;
                        }
                        else
                        {
                            Debug.Log(string.Format("Get persist list failed: {0}, file: {1}", wwwPersistList.error, listFileName));
                        }
                    }
                }
            }

            // 4. 准备下载，删除该删除的，获得要下载的总大小
            m_bundle2Download.Clear();

            NeedDownloadBytes = 0;
            for (int i = 0; i < m_bundles.Length; i++)
            {
                var listBundle = m_bundles[i];
                for (int j = 0; j < listBundle.Count; j++)
                {
                    BundleItem item = listBundle[j];
                    if (item.State == BundleState.Removed)
                    {
                        string localPath = BundleLoader.GetAssetBundlePersistPath(item.Name);
                        if (File.Exists(localPath))
                            File.Delete(localPath);
                    }
                    else if (item.NeedDownload)
                    {
                        NeedDownloadBytes += (ulong)item.Length;
                        m_bundle2Download.Enqueue(item);
                    }
                }
            }

            // 5. 保存List文件
            string writeError;
            bool succeed = WriteAllListFile(out writeError);
            if (!succeed)
            {
                SetState(Stage.WriteFileFailed, 0, "list file", writeError, 0, 0);
                yield break;
            }

            // 6. 如果不需要下载就保存版本文件
            if (!NeedDownload)
            {
                succeed = WriteVersion(out writeError);
                if (succeed)
                    SetState(Stage.CheckSucceed, 0, null, "Check finished.", NeedDownloadBytes, 0);
                else
                    SetState(Stage.CheckFailed, 0, null, writeError, NeedDownloadBytes, 0);
            }
            else
            {
                SetState(Stage.CheckSucceed, 0, null, "Check finished.", NeedDownloadBytes, 0);
            }

            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - startTime;
            Debug.Log(string.Format("Check version, start time: {0}, end time: {1}, total seconds: {2}", m_startDownloadTime, endTime, ts.TotalSeconds));
        }

        // 当下载完 listinfo.dat
        void OnDowloadedListInfo(string text, bool[] listNeedDownload, out int listinfoTotalSize)
        {
            listinfoTotalSize = 0;
            string[] listinfoArray = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < listinfoArray.Length; i++)
            {
                bool toDownload = listNeedDownload[i];
                if (toDownload)
                {
                    int size = 0;
                    string str = listinfoArray[i];
                    int.TryParse(str, out size);
                    listinfoTotalSize += size;
                }
            }
            OnWWWSucceed(0);
        }

        // 下载协程
        IEnumerator EtorDownload(int threadId)
        {
            SetState(Stage.Downloading, threadId, null, null, NeedDownloadBytes, DownloadedBytes);

            // 5. 开始下载
            while (m_bundle2Download.Count > 0)
            {
                BundleItem item = m_bundle2Download.Dequeue();
                if (!item.NeedDownload)
                    continue;

                string itemUrl = RootUrl + item.Name;

                /*
                 总结：
                 自己写的超时逻辑，超时一定要用 Abort() 来终结，调用 Abort 后会触发错误，但有时却不触发错误，此时可能会导致协和永远死循环，因此用 time out 来做第二层保护
                 */
                using (UnityWebRequest uwrItem = UnityWebRequest.Get(itemUrl))
                {
                    int uwrTimeout = Mathf.Max(60, item.Length / AllowMinSpeed);
                    uwrItem.timeout = uwrTimeout;
                    uwrItem.SendWebRequest();

                    ulong temp = 0;
                    float blockTimer = TimeOut;
                    float timer = uwrTimeout + 30;

                    #region 下载中

                    while (!uwrItem.isDone)
                    {
                        // 超时，自己计时
                        if (timer < 0)
                        {
                            uwrItem.Abort();
                            DownloadedBytes -= uwrItem.downloadedBytes;
                            m_bundle2Download.Enqueue(item);
                            m_downloadingThreads.Remove(threadId);
                            SetState(Stage.DownloadFailed, threadId, item.Name, "Manual timeout", NeedDownloadBytes, DownloadedBytes);
                            yield break;
                        }
                        else
                        {
                            timer -= Time.deltaTime;
                        }

                        // 出错
                        if (uwrItem.isHttpError || uwrItem.isNetworkError)
                        {
                            DownloadedBytes -= uwrItem.downloadedBytes;
                            m_bundle2Download.Enqueue(item);
                            m_downloadingThreads.Remove(threadId);
                            SetState(Stage.DownloadFailed, threadId, item.Name, "Time out, " + uwrItem.error, NeedDownloadBytes, DownloadedBytes);
                            yield break;
                        }

                        ulong offset = uwrItem.downloadedBytes - temp;
                        if (offset > 0)
                        {
                            temp = uwrItem.downloadedBytes;
                            DownloadedBytes += offset;
                            blockTimer = TimeOut;
                            //Debug.Log(string.Format("# {0:P0}\t{1}\t{2}\t{3}", uwrItem.downloadProgress, item.Name, item.Length / 1024f, threadId));
                        }
                        else
                        {
                            // 检测是否超时
                            //Debug.Log("block time: " + blockTimer);
                            if (blockTimer < 0)
                            {
                                uwrItem.Abort();
                                Debug.Log(string.Format("Aborted: {0}, thread id: {1}", item.Name, threadId));
                                blockTimer = TimeOut;
                            }
                            else
                            {
                                blockTimer -= Time.deltaTime;
                            }
                        }

                        yield return null;
                    }
                    #endregion

                    #region 下载完成

                    DownloadedBytes += (uwrItem.downloadedBytes - temp);

                    // 出错
                    if (uwrItem.isHttpError || uwrItem.isNetworkError)
                    {
                        DownloadedBytes -= uwrItem.downloadedBytes;
                        m_bundle2Download.Enqueue(item);
                        m_downloadingThreads.Remove(threadId);
                        SetState(Stage.DownloadFailed, threadId, item.Name, uwrItem.error, NeedDownloadBytes, DownloadedBytes);
                        yield break;
                    }

                    // 写入磁盘
                    string localPath = BundleLoader.GetAssetBundlePersistPath(item.Name);
                    string writeError;
                    bool writeSucceed = WriteBytes(localPath, uwrItem.downloadHandler.data, out writeError);
                    if (!writeSucceed)
                    {
                        DownloadedBytes -= uwrItem.downloadedBytes;
                        m_bundle2Download.Enqueue(item);
                        m_downloadingThreads.Remove(threadId);
                        SetState(Stage.WriteFileFailed, threadId, item.Name, writeError, NeedDownloadBytes, DownloadedBytes);
                        yield break;
                    }

                    // 状态
                    item.State = BundleState.NoChange;
                    OnWWWSucceed(threadId);

                    // 保存 list 文件
                    m_bundleDownloaded.Add(item);
                    if (m_bundleDownloaded.Count > CountToWriteAllList)
                        writeSucceed = WriteAllListFile(out writeError);
                    else
                        writeSucceed = WriteListFile4DownloadedB(out writeError);      // 保存已下载的资源包列表

                    //Debug.Log("Download succeed: " + item.Name);

                    if (!writeSucceed)
                    {
                        Debug.Log(string.Format("Write list file failed, error: " + writeError));
                    }

                    #endregion
                }
            }

            // 6. 所有的都下完了，保存版本文件和 List 文件
            bool allCompleted = AllBundleDownloadedComplete();
            if (allCompleted)
            {
                // 保存 List 文件
                string writeError;
                bool succeed = WriteAllListFile(out writeError);        // 保存 list 文件
                if (!succeed)
                {
                    SetState(Stage.WriteFileFailed, 0, "list file", writeError, 0, 0);
                    yield break;
                }

                // 保存 Version 文件
                succeed = WriteVersion(out writeError);
                if (succeed)
                    OnDownloadedComplete();
                else
                    SetState(Stage.WriteFileFailed, 0, VersionName, writeError, NeedDownloadBytes, DownloadedBytes);

                DateTime endTime = DateTime.Now;
                TimeSpan ts = endTime - m_startDownloadTime;
                Debug.Log(string.Format("All download finished, start time: {0}, end time: {1}, total seconds: {2}, total size: {3} KB", m_startDownloadTime, endTime, ts.TotalSeconds, NeedDownloadBytes / 1024f));
            }
        }

        // 下载完成
        void OnDownloadedComplete()
        {
            SetState(Stage.DownloadSucceed, 0, VersionName, "Download succeed!", NeedDownloadBytes, DownloadedBytes);


        }

        bool AllBundleDownloadedComplete()
        {
            if (m_bundle2Download.Count > 0)
                return true;

            for (int i = 0; i < m_bundles.Length; i++)
            {
                var listBundle = m_bundles[i];
                for (int j = 0; j < listBundle.Count; j++)
                {
                    var item = listBundle[j];
                    if (item.NeedDownload)
                        return false;
                }
            }

            return true;
        }

        // 保存所有的 list 文件
        bool WriteAllListFile(out string error)
        {
            for (int i = 0; i < m_bundles.Length; i++)
            {
                string listName = ListFileNames[i];
                var listBundle = m_bundles[i];
                bool succeed = WriteListFile(listName, listBundle, m_sb, out error);        // 保存 list 文件
                if (!succeed)
                    return false;
            }

            // 删除已下载资源缓存文件
            string path = BundleLoader.GetAssetBundlePersistPath(DownloadedBundlesFile);
            File.Delete(path);
            Debug.Log("Delete file: " + DownloadedBundlesFile);

            // 清除已下载资源 list
            m_bundleDownloaded.Clear();

            Debug.Log("Write all list file, progress: " + DownloadingProgress.ToString("P2"));

            error = null;
            return true;
        }

        void OnWWWSucceed(int threadId)
        {
            m_retryCount[threadId] = 0;
        }

        // 0 是主线程
        void SetState(Stage stage, int threadId, string url = null, string msg = null, ulong totalBytes = 0, ulong downloadedBytes = 0)
        {
            // retry
            m_threadStages[threadId] = stage;
            if (stage == Stage.CheckFailed || stage == Stage.DownloadFailed)
            {
                if (m_retryCount.ContainsKey(threadId))
                    m_retryCount[threadId]++;
                else
                    m_retryCount[threadId] = 1;

                if (m_retryCount[threadId] > MaxRetryCount)
                    m_retryCount[threadId] = 0;
                else
                {
                    m_threadStages[threadId] = Stage.Retry;
                    if (stage == Stage.CheckFailed)
                        CheckAsync();
                    else
                        OpenDownloadThread(threadId);
                }
            }

            if (threadId > 0 && m_threadStages[threadId] == Stage.DownloadFailed)
            {
                m_threadStages[0] = Stage.DownloadFailed;
                Debug.Log("All download thread failed.");
            }

            Message = msg;
            NeedDownloadBytes = totalBytes;
            DownloadedBytes = downloadedBytes;

            Debug.Log(string.Format("Thread id: {0}, stage: {1}, message: {2}, totalBytes: {3}, downloadedBytes: {4}, url:{5}, Rectry count: {6}, ",
                threadId,
                m_threadStages[threadId],
                Message,
                NeedDownloadBytes,
                DownloadedBytes,
                url,
                m_retryCount.ContainsKey(threadId) ? m_retryCount[threadId] : 0));
        }

        // 记录已下载完的资源名
        bool WriteListFile4DownloadedB(out string error)
        {
            error = null;

            if (m_bundleDownloaded == null || m_bundleDownloaded.Count < 1)
                return true;

            m_sb.Length = 0;
            for (int i = 0; i < m_bundleDownloaded.Count; i++)
            {
                var item = m_bundleDownloaded[i];
                m_sb.AppendFormat("{0},{1};", item.Name, item.Hash);
            }

            string path = BundleLoader.GetAssetBundlePersistPath(DownloadedBundlesFile);

            try
            {
                File.WriteAllText(path, m_sb.ToString());
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return true;
        }

        // 保存版本文件
        bool WriteVersion(out string error)
        {
            error = null;
            string path = BundleLoader.GetAssetBundlePersistPath(VersionName);
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                File.WriteAllText(path, m_versionContent);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            return true;
        }

    }
}
