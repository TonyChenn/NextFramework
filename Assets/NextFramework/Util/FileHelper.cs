using System.IO;

namespace NextFramework.Util
{
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
    }
}
