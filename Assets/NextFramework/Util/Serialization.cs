using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// List<T>
[Serializable]
public class Serialization<T>
{

    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}


// Dictionary<TKey, TValue>
[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}



/**
// List<T> -> Json ( 例 : List<Enemy> )
string str = JsonUtility.ToJson(new Serialization<Enemy>(enemies)); 
// 输出 : {"target":[{"name":"怪物1,"skills":["攻击"]},{"name":"怪物2","skills":["攻击","恢复"]}]}

// Json-> List<T>
List<Enemy> enemies = JsonUtility.FromJson<Serialization<Enemy>>(str).ToList();

// Dictionary<TKey,TValue> -> Json( 例 : Dictionary<int, Enemy> )
string str = JsonUtility.ToJson(new Serialization<int, Enemy>(enemies)); 
// 输出 : {"keys":[1000,2000],"values":[{"name":"怪物1","skills":["攻击"]},{"name":"怪物2","skills":["攻击","恢复"]}]}

// Json -> Dictionary<TKey,TValue>
Dictionary<int, Enemy> enemies = JsonUtility.FromJson<Serialization<int, Enemy>>(str).ToDictionary();

**/