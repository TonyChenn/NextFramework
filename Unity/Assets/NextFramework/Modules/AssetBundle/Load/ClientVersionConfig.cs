using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileVersionInfo
{
    public string FileName;
    public string MD5;
    public long Size;
    public FileVersionInfo(string fileName, string md5,long fileSize)
    {
        this.FileName = fileName;
        this.MD5 = md5;
        this.Size = fileSize;
    }
}

public class ClientVersionConfig
{
    /// <summary>
    /// Key: SubFolderName+"_"+"FileName"
    /// </summary>
    public Dictionary<string, FileVersionInfo> FileInfoDict = new Dictionary<string, FileVersionInfo>();
}
