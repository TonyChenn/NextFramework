using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectFactory<T>
{
    T Create();
}

public class ObjectFactory<T> : IObjectFactory<T> where T : new()
{
    public T Create()
    {
        return new T();
    }
}
