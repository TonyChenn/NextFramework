﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ObjectFactory
{
    /// <summary>
    /// 创建有参的构造函数
    /// </summary>
    public static object Create(Type type, params object[] constructors)
    {
        return Activator.CreateInstance(type, constructors);
    }
    /// <summary>
    /// 泛型扩展
    /// </summary>
    public static T Create<T>(params object[] constructors)
    {
        return (T)Create(typeof(T), constructors);
    }

    /// <summary>
    /// 动态创建类的实例：创建无参/私有的构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object CreateNonPublicConstructorObject(Type type)
    {
        // 获取私有构造函数
        var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
        // 获取无参构造函数
        var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);

        if (ctor == null)
            throw new Exception("Not found Non-public Constructor int class " + type);

        return ctor.Invoke(null);
    }

    /// <summary>
    /// 创建无参/私有的构造函数  泛型扩展
    /// </summary>
    public static T CreateNonPublicConstructorObject<T>()
    {
        return (T)CreateNonPublicConstructorObject(typeof(T));
    }

    /// <summary>
    /// 创建带有初始化回调的 对象
    /// </summary>
    public static object CreateWithInitAction(Type type, Action<object> onObjectCreate,
        params object[] constructorArgs)
    {
        var obj = Create(type, constructorArgs);
        onObjectCreate(obj);
        return obj;
    }

    /// <summary>
    /// 创建带有初始化回调的 对象：泛型扩展
    /// </summary>
    public static T CreateWithInitAction<T>(Action<T> onObjectCreate,
        params object[] constructorArgs)
    {
        var obj = Create<T>(constructorArgs);
        onObjectCreate(obj);
        return obj;
    }
}
