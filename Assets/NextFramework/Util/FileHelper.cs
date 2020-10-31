using System.IO;

public class FileHelper
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
public class FolderHelper
{
    public static void CreateFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }
}
