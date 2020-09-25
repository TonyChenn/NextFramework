using System;
using System.Reflection;

namespace NextFramework
{
    public interface ISingleton
    {
        void InitSingleton();
    }

    public abstract class NormalSingleton<T> : ISingleton
        where T : NormalSingleton<T>
    {
        static object mLock = new object();
        static T mInstance = null;
        public static T Singlton
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = NormalSingletonCreater.CreateSingleton<T>();
                    }
                }
                return mInstance;
            }
        }

        public virtual void InitSingleton() { }
        public virtual void Dispose() { mInstance = null; }
    }

    public static class NormalSingletonCreater
    {
        public static T CreateSingleton<T>() where T : class, ISingleton
        {
            // ��ȡ˽�й��캯��
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            // ��ȡ�޲ι��캯��
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
            }

            // ͨ�����캯��������ʵ��
            var retInstance = ctor.Invoke(null) as T;
            retInstance.InitSingleton();

            return retInstance;
        }
    }

    public static class NormalSingltonProperty<T> where T : class, ISingleton
    {
        static T mInstance;
        static readonly object mLock = new object();
        public static T Singlton
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                        mInstance = NormalSingletonCreater.CreateSingleton<T>();
                }
                return mInstance;
            }
        }
        public static void Dispose() { mInstance = null; }
    }

}

