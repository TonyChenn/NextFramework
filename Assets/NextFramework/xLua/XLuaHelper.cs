using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XLuaHelper
{
    /// <summary>
    /// 供lua创建数组
    /// </summary>
    /// <param name="itemType"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Array CreateArray(Type itemType, int count)
    {
        return Array.CreateInstance(itemType, count);
    }
    
    /// <summary>
    /// 供lua创建泛型列表
    /// </summary>
    public static IList CreateList(Type itemType)
    {
        return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
    }

    /// <summary>
    /// 供lua创建字典
    /// </summary>
    public static IDictionary CreateDictionary(Type keyType, Type valueType)
    {
        return (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(new[] {keyType, valueType}));
    }
}
