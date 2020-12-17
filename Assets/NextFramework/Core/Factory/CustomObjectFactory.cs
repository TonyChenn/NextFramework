using System;

namespace NextFramework
{
    /// <summary>
    /// 对象是自定义生成的(如：GameObject)
    /// </summary>
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        protected Func<T> mMethod;

        public CustomObjectFactory(Func<T> method)
        {
            this.mMethod = method;
        }
        public T Create()
        {
            return mMethod();
        }
    }
}

