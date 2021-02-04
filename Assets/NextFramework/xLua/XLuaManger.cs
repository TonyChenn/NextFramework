using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NextFramework;
using XLua;
using System.IO;

public class XLuaManger : IDisposable
{
    LuaEnv mLuaEnv;
    public LuaEnv LuaEnv { get { return mLuaEnv; } }
    
    static XLuaManger _instance = null;
    public static XLuaManger Singlton { get { return _instance ?? new XLuaManger(); } }

    public XLuaManger()
    {
        mLuaEnv = new LuaEnv();
        LuaEnv.CustomLoader loader;
#if UNITY_EDITOR
        if (GameConfig.Singlton.UseLocalAsset)
            loader = OriginLoader;
        else
            loader = BundleLoader;
#else
        loader = BundleLoader;
#endif
        mLuaEnv.AddLoader(loader);
    }
    ~XLuaManger()
    {
        Dispose(false);
    }

    private byte[] OriginLoader(ref string fileName)
    {
        string luafile = PathConfig.XLuaScriptFolder + "/" + fileName + ".lua";
        if(!File.Exists(luafile))
        {
            Debug.LogError("cant find file:" + fileName + " in path:" + luafile);
            return null;
        }
        return File.ReadAllBytes(luafile);
    }
    private byte[] BundleLoader(ref string fileName)
    {
        return null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    void Dispose(bool dispose)
    {
        if (dispose)
        {
            if (mLuaEnv != null)
            {
                mLuaEnv.GC();
                mLuaEnv.Dispose();
            }
        }
        mLuaEnv = null;
    }
}
