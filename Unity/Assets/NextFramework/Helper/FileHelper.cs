using NextFramework;
using System.IO;
using System.Text.RegularExpressions;

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

    /// <summary>
    /// 返回路径的最后一个文件夹名称
    /// </summary>
    public static string GetFolderName(string path)
    {
        if (ExistFolder(path))
            return Path.GetFileNameWithoutExtension(path);

        string[] temp = path.Replace("\\", "/").Split('/');
        return temp[temp.Length - 1];
    }

    /// <summary>
    /// 获取所有子文件夹路径
    /// </summary>
    public static string[] GetDirectories(string folderPath)
    {
        return Directory.GetDirectories(folderPath);
    }
}

public static class PathHelper
{
    /// <summary>
    /// 组合路径，并标准化
    /// </summary>
    public static string CombinePath(params string[] paths)
    {
        var builder = StringBuilderPool.Alloc();
        for (int i = 0, iMax = paths.Length; i < iMax; i++)
        {
            builder.Append(paths[i].GetStandardPath().TrimStart('/'));
            if (i != iMax-1)
                builder.Append("/");
        }
        string result = builder.ToString();
        builder.Recycle();

        return result;
    }

    /// <summary>
    /// 获取标准转义符的地址
    /// </summary>
    public static string GetStandardPath(this string path)
    {
        if (string.IsNullOrEmpty(path)) return path;

        return path.Replace("\\", "/").TrimEnd('/');
    }

    /// <summary>
    /// 返回以"Assets"开头的相对地址
    /// </summary>
    public static string GetRelativePath(this string fullPath)
    {
        return fullPath.GetStandardPath().Substring(fullPath.IndexOf("Assets"));
    }

    /// <summary>
    /// 返回不以"Assets"开头的相对地址
    /// </summary>
    public static string GetRelativePathWithoutAssets(this string fullPath)
    {
        return fullPath.GetStandardPath().Replace(PathConfig.RootPath, "");
    }

    /// <summary>
    /// 根据路径返回文件名（带拓展名）
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileNameByPath(this string filePath)
    {
        Match match = Regex.Match(filePath.GetStandardPath(), @"([^<>/\\\|:""\*\?]+)\.\w+$");
        return match.Value;
    }

    /// <summary>
    /// 根据路径返回文件名（不带拓展名）
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileNameByPathWithoutExtention(this string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return filePath;

        string path = filePath.GetFileNameByPath();
        return path.Substring(0, path.LastIndexOf("."));
    }
}
