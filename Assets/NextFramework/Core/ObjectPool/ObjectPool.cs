using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextFramework
{
    /// <summary>
    /// 普通对象池
    /// </summary>
    public class ObjectPool<T> : Pool<T>
    {
        Action<T> mResetAction;

        public ObjectPool(Func<T> factoryFunc, Action<T> resetAction,int initCount=0)
        {
            factory = new CustomObjectFactory<T>(factoryFunc);
            mResetAction = resetAction;
            for (int i = 0,iMax=initCount; i < iMax; i++)
            {
                dataStack.Push(factory.Create());
            }
        }
        public override bool Recycle(T obj)
        {
            if (mResetAction != null) mResetAction.Invoke(obj);

            dataStack.Push(obj);

            return true;
        }
    }
}
