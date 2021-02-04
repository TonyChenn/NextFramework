using System.Collections.Generic;
using System.IO;

public static class FileHelper
{
    public static string ReadFile(string path)
    {
        string str = "";
        if (File.Exists(path))
            str = File.ReadAllText(path);
        else
            UnityEngine.Debug.LogError("File is not Exists：" + path);
        return str;
    }
    public static void WriteFile(string path, string content)
    {
        if (!File.Exists(path))
            File.Create(path).Dispose();
        File.WriteAllText(path, content);
    }

    /// <summary>
    /// 获取文件名（包含拓展名）
    /// </summary>
    public static string GetFileName(string filePath)
    {
        filePath = filePath.Trim().Replace("\\", "/");
        return filePath.Substring(filePath.LastIndexOf('/') + 1);
    }

    /// <summary>
    /// 获取文件名（不包含拓展名）
    /// </summary>
    public static string GetFileNameWithoutExtention(string filePath)
    {
        filePath = GetFileName(filePath);
        if (filePath.Contains("."))
            return filePath.Substring(0, filePath.LastIndexOf('.'));
        return filePath;
    }

    /// <summary>
    /// 删除文件如果已存在
    /// </summary>
    /// <param name="filePath"></param>
    public static void DeleteFileIfExist(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    /// <summary>
    /// 获取文件夹下所有文件
    /// </summary>
    public static FileInfo[] GetAllFiles(string folderPath, string filter = "*", SearchOption option = SearchOption.AllDirectories)
    {
        if (FolderHelper.ExistFolder(folderPath))
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            return GetAllFiles(info, filter, option);
        }
        return null;
    }
    public static FileInfo[] GetAllFiles(DirectoryInfo folderInfo, string filter = "*", SearchOption option = SearchOption.AllDirectories)
    {
        return folderInfo.GetFiles(filter, option);
    }

    public static void WriteUITypeFile(string filePath, string content = "")
    {
        string fileContent = NextFramework.TemplateString.Str_UIType.Replace("{ENUM}", content);

        WriteFile(filePath, fileContent);
    }

    public static string UIPanelJson
    {
        get
        {
            if (File.Exists(PathConfig.UIPanelsJsonPath))
                return ReadFile(PathConfig.UIPanelsJsonPath);
            else
                return "[]";
        }
        set
        {
            WriteFile(PathConfig.UIPanelsJsonPath, value);
        }
    }
}
public static class FolderHelper
{
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="folderPath"></param>
    public static void CreateFolderIfNotExist(this string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }
    /// <summary>
    /// 删除文件夹及文件夹中文件
    /// </summary>
    /// <param name="folderPath"></param>
    public static void DeleteFolderIfExist(this string folderPath)
    {
        if (Directory.Exists(folderPath))
            Directory.Delete(folderPath, true);
    }

    /// <summary>
    /// 清空文件夹中所有文件
    /// </summary>
    public static void ClearFolder(this string folderPath)
    {
        if (Directory.Exists(folderPath))
            Directory.Delete(folderPath, true);

        Directory.CreateDirectory(folderPath);
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="folderPath"></param>
    public static void OpenFolder(string folderPath)
    {
#if UNITY_STANDALONE_OSX
        System.Diagnostics.Process.Start("open", folderPath);
#elif UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", folderPath);
#endif
    }

    /// <summary>
    /// 判断文件夹是否存在
    /// </summary>
    public static bool ExistFolder(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// 获取所有子文件夹
    /// </summary>
    /// <returns></returns>
    public static DirectoryInfo[] GetSubFolders(string folderPath)
    {
        if (ExistFolder(folderPath))
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            return info.GetDirectories();
        }
        return null;
    }
}

public static class PathHelper
{
    /// <summary>
    /// 获取标准转义符的地址
    /// </summary>
    public static string GetStandardPath(this string path)
    {
        return path.Replace("\\", "/").TrimEnd('/');
    }

    /// <summary>
    /// 返回以"Asset"开头的地址
    /// </summary>
    public static string GetUnityPath(this string fullPath)
    {
        return fullPath.GetStandardPath().Replace(UnityEngine.Application.dataPath, "Assets");
    }
}
