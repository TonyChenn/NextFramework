﻿namespace NextFramework
{
    /// <summary>
    /// 对象是通过New()生成的
    /// </summary>
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {

        public T Create()
        {
            return new T();
        }
    }
}

