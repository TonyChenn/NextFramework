using NextFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : Pool<T> where T : IPoolable, new()
{
    Action<T> mAction;
    static ObjectPool<T> mInstance;
    public ObjectPool()
    {
        factory = new DefaultObjectFactory<T>();
    }
    public static ObjectPool<T> Singlton
    {
        get
        {
            if (mInstance == null) mInstance = new ObjectPool<T>();
            return mInstance;
        }
    }

    public int MaxCacheCount
    {
        get { return mMaxCount; }
        set
        {
            if (dataStack == null || mMaxCount <= 0) return;

            mMaxCount = value;
            if (mMaxCount < dataStack.Count)
            {
                for (int i = 0, iMax = dataStack.Count - mMaxCount; i < iMax; i++)
                    dataStack.Pop();
            }
        }
    }

    public override T Alloc()
    {
        T result = base.Alloc();
        result.HasRecycled = false;
        return result;

    }
    public override bool Recycle(T obj)
    {
        if (obj == null || obj.HasRecycled) return false;

        //对于超出最大限制的对象返回false,然后判断删除
        if (dataStack.Count > mMaxCount) return false;

        obj.HasRecycled = true;
        obj.OnRecycle();
        dataStack.Push(obj);
        return true;
    }

    public void Init(int maxCount, int initCount)
    {
        mMaxCount = maxCount;

        if (maxCount > 0) initCount = maxCount < initCount ? maxCount : initCount;
        if(Count<initCount)
        {
            for (int i = Count; i < initCount; i++)
            {
                Recycle(Alloc());
            }
        }
    }
}
