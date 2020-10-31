using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[System.Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class LuaCompoment : MonoBehaviour
{
    public string luaScript;
    public Injection[] injections;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    void Awake()
    {
        if(string.IsNullOrEmpty(luaScript))
        {
            Debug.LogError("Lua script Name is not set,please check!!!");
            return;
        }

        scriptEnv = XLuaManger.Singlton.LuaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = XLuaManger.Singlton.LuaEnv.NewTable();
        meta.Set("__index", XLuaManger.Singlton.LuaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        foreach (var injection in injections)
            scriptEnv.Set(injection.name, injection.value);

        XLuaManger.Singlton.LuaEnv.DoString(string.Format("require '{0}'", luaScript));

        Action luaAwake = scriptEnv.Get<Action>("Awake");
        scriptEnv.Get("Start", out luaStart);
        scriptEnv.Get("Update", out luaUpdate);
        scriptEnv.Get("OnDestroy", out luaOnDestroy);

        if (luaAwake != null)
            luaAwake();
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
            luaStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
            luaUpdate();

        if (Time.time - lastGCTime > GCInterval)
        {
            XLuaManger.Singlton.LuaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
            luaOnDestroy();

        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
