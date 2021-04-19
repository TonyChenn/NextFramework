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
    [SerializeField] string luaScript;
    public Injection[] injections;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;

    public string ScriptName { get { return luaScript; } set { luaScript = value; } }
    public LuaTable ScriptEnv { get { return scriptEnv; } }

    public void CallLuaFunction(string functionName)
    {
        if (string.IsNullOrEmpty(functionName) || scriptEnv == null)
            return;

        Action func = scriptEnv.Get<Action>(functionName);
        if (func != null)
            func.Invoke();
    }

    void Awake()
    {
        if (string.IsNullOrEmpty(luaScript))
        {
            Debug.LogError("Lua script Name is not set,please check!!!");
            return;
        }

        scriptEnv = XLuaManger.Singlton.GetLuaTable(luaScript);

        if (scriptEnv == null) return;

        scriptEnv.Set<string, Transform>("transform", transform);
        scriptEnv.Set<string, GameObject>("gameObject", gameObject);

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
        if (scriptEnv != null && luaStart != null)
            luaStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptEnv != null && luaUpdate != null)
            luaUpdate();

        if (Time.time - lastGCTime > GCInterval)
        {
            XLuaManger.Singlton.LuaEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (scriptEnv != null && luaOnDestroy != null)
            luaOnDestroy();

        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
