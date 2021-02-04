using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using OfficeOpenXml;
using System.Diagnostics;
using System.Text;
using NextFramework.Util;
using NextFramework;

public class ConvertTable : ScriptableObject
{
    public static void GenCSharp(List<SettingItem> list)
    {
        if (Directory.Exists(EditorPrefsHelper.ExcelFolder) && !string.IsNullOrEmpty(EditorPrefsHelper.ExcelFolder))
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            foreach (var item in list)
            {
                if (item.ToAsset)
                {
                    ExcelData data = GetTableData(item);
                    string str_csharp = GetCSharpString(data);
                    string path = string.Format("{0}/Scripts/Table/Define/Config_{1}.cs", Application.dataPath, data.tableName);
                    FileHelper.WriteFile(path, str_csharp);
                }
            }
            /// TODO 生成ConfigManger
            AssetDatabase.Refresh();
            watch.Stop();
            EditorUtility.DisplayDialog("提示", "生成CSharp,耗时" + TimeHelper.GetTimeString((int)(watch.ElapsedMilliseconds / 1000)), "好的");
            AssetDatabase.Refresh();
        }
        else
        {
            if (EditorUtility.DisplayDialog("提示", "配表路径有问题，请检查！！", "这就去"))
            {
                EditorWindow.GetWindow<BuildAssetBundleWnd>();
            }
        }
    }

    /// <summary>
    /// 获取表格数据
    /// </summary>
    public static ExcelData GetTableData(SettingItem item)
    {
        return GetTableData(item.Name);
    }
    public static ExcelData GetTableData(string itemName)
    {
        var decoder = new DecodeExcel();
        return decoder.Decode(EditorPrefsHelper.ExcelFolder + "/" + itemName);
    }
    static string GetCSharpString(ExcelData excelData)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < excelData._fieldNameList.Count; i++)
        {
            sb.AppendLine("\t/// <summary>");
            sb.Append("\t/// " + excelData._fieldNameList[i]);
            if (!string.IsNullOrEmpty(excelData._commentList[i]))
                sb.AppendFormat("({0})", excelData._commentList[i]);
            sb.AppendLine();
            sb.AppendLine("\t/// <summary>");
            sb.Append("\tpublic ");
            switch (excelData._typeNameList[i].ToLower())
            {
                case "string"   : sb.Append("string "); break;
                case "bool"     : sb.Append("bool "); break;
                case "int"      :
                case "int32"    : sb.Append("int "); break;
                case "uint32"   : sb.Append("uint "); break;
                case "int64"    :
                case "long"     : sb.Append("long "); break;
                case "datetime" : sb.Append("DateTime "); break;
            }
            sb.Append(excelData._fieldNameList[i]);
            sb.AppendLine(";");
        }


        StringBuilder builder = new StringBuilder(NextFramework.TemplateString.String_Config_Template);
        builder.Replace("{FILE_NAME}", excelData.tableName);
        builder.Replace("{ITEM_CLASS_VARIABLE}", sb.ToString());
        return builder.ToString();
    }
}
