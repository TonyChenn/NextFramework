using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPool<T>
{
    T Alloc();

    bool Recycle(T obj);
}
public interface IPoolable
{
    bool HasRecycled { get; set; }

    /// <summary>
    /// 初始化,删除Texture等资源
    /// </summary>
    void OnRecycle();
}
public interface IPoolType
{
    void Recycle2Cache();
}

public abstract class Pool<T> : IPool<T>
{
    protected Stack<T> dataStack = new Stack<T>();
    protected int mMaxCount = 10;
    protected IObjectFactory<T> factory;

    /// <summary>
    /// 获取
    /// </summary>
    public virtual T Alloc()
    {
        return Count == 0 ? factory.Create() : dataStack.Pop();
    }

    /// <summary>
    /// 回收
    /// </summary>
    public abstract bool Recycle(T obj);

    public int Count { get { return dataStack.Count; } }
}

