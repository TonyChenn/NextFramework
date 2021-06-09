using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectFactory<T>
{
    T Create();
}
