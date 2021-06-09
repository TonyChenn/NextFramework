using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRefCounter
{
    int RefCount { get; }
    void AddRef();
    void LessRef();
}
