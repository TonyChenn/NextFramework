﻿using System;
using System.Reflection;

namespace NextFramework
{
    /// <summary>
    /// 通过反射获得无参构造函数
    /// </summary>
    public class NonPublicObjectFactory<T> : IObjectFactory<T> where T : class
    {
        public T Create()
        {
            ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var constructor = Array.Find(constructors, c => c.GetParameters().Length == 0);

            if (constructor == null)
                throw new Exception("No public Constructor int class " + typeof(T));

            return constructor.Invoke(null) as T;
        }
    }
}
