using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NextFramework;
using XLua;
using System.IO;
using System.Linq;

public class XLuaManger : NormalSingleton<XLuaManger>, IDisposable
{
    LuaEnv mLuaEnv;
    public LuaEnv LuaEnv { get { return mLuaEnv; } }

    private XLuaManger()
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
        var builder = StringBuilderPool.Alloc();
        builder.Append(PathConfig.XLuaScriptFolder);
        builder.Append("/");
        builder.Append(fileName);
        builder.Append(".lua");
        string luafile = builder.ToString();
        if(File.Exists(luafile))
            return File.ReadAllBytes(luafile);

        builder.Append(".txt");
        luafile = builder.ToString();
        if (File.Exists(luafile))
            return File.ReadAllBytes(luafile);

        Debug.LogError("cant find file:" + fileName + " in path:" + luafile);
        return null;
    }
    private byte[] BundleLoader(ref string fileName)
    {
        return null;
    }

    #region Dispose
    public override void Dispose()
    {
        Dispose(true);
        base.Dispose();
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
    #endregion

    public LuaTable GetLuaTable(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            Debug.LogError("string.IsNullOrEmpty(tableName)");
            return null;
        }

        LuaTable table = LuaEnv.Global.Get<LuaTable>(tableName);
        if (table == null)
        {
            LoadLuaTable(tableName);
            table = LuaEnv.Global.Get<LuaTable>(tableName);
        }
        return table;
    }
    public void LoadLuaTable(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            Debug.LogError("string.IsNullOrEmpty(name)");
            return;
        }

        string code = string.Format("require '{0}'", tableName);
        LuaEnv.DoString(code);
    }

    #region Static Methods

    /// <summary>
    /// 获取指定游戏对象上挂的lua组件
    /// </summary>
    public static LuaTable GetLuaComponent(Transform transform)
    {
        if (transform == null)
        {
            Debug.LogError("Get Lua Compoment, transform == null");
            return null;
        }

        var loaders = transform.GetComponents<LuaCompoment>();
        var rightLoader = loaders.FirstOrDefault(lt => lt.ScriptEnv != null);

        return rightLoader != null ? rightLoader.ScriptEnv : null;
    }

    /// <summary>
    /// 获取指定游戏对象上挂的lua组件
    /// </summary>
    public static LuaTable GetLuaComponent(GameObject gameObject)
    {
        return GetLuaComponent(gameObject.transform);
    }

    /// <summary>
    /// 获取指定游戏对象上挂的lua组件
    /// </summary>
    /// <param name="type">lua组件表名</param>
    public static LuaTable GetLuaComponent(Transform transform, string scriptName)
    {
        if (transform == null)
            throw new ArgumentNullException("transform");
        if (string.IsNullOrEmpty(scriptName))
            throw new ArgumentException("scriptName");

        var loaders = transform.GetComponents<LuaCompoment>();
        var rightLoader = loaders.FirstOrDefault(lt => lt.ScriptEnv != null && lt.ScriptName == scriptName);

        return rightLoader != null ? rightLoader.ScriptEnv : null;
    }

    /// <summary>
    /// 获取指定游戏对象上挂的表名为type的lua组件
    /// </summary>
    public static LuaTable GetLuaComponent(GameObject gameObject, string scriptName)
    {
        return GetLuaComponent(gameObject.transform, scriptName);
    }

    /// <summary>
    /// 为游戏对象添加lua组件
    /// </summary>
    /// <param name="type">lua组件名</param>
    public static LuaTable AddLuaComponent(GameObject gameObject, string scriptName)
    {
        var loader = gameObject.AddComponent<LuaCompoment>();
        loader.ScriptName = scriptName;
        return loader.ScriptEnv;
    }

    #endregion
}
