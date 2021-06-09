using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework
{

    public class TemplateString
    {
        /// <summary>
        /// UIType 模板
        /// </summary>
        public const string Str_UIType = @"namespace NextFramework.UI
{
    public enum UIType
    {
        None,{ENUM}
    }
}";

        /// <summary>
        /// Excel转CSharp脚本模板
        /// </summary>
        public const string String_Config_Template = @"/// <summary>
/// 本文件中的代码为生成的代码，不允许手动修改
/// </summary>
using System;
using UnityEngine;

[Serializable]
public partial class Item_{FILE_NAME}
{
{ITEM_CLASS_VARIABLE}
}

public partial class Config_{FILE_NAME} : ScriptableObject
{
    public Item_{FILE_NAME}[] Array;
    public static Config_{FILE_NAME} Singleton { get; private set; }

    public static void Init()
    {
#if UNITY_EDITOR
        if (GameConfig.Singlton.UseLocalAsset)
            LoadFromLocal();
        else
            LoadFromBundle();
#else
            LoadFromBundle();
#endif
    }

    static void LoadFromBundle()
    {
        var item = new NormalAssetItem(""/Table/{FILE_NAME}.u"");
        item.Load(() =>
        {
            Singleton = item.AssetObj as Config_{FILE_NAME};
        });
    }

#if UNITY_EDITOR
    static void LoadFromLocal()
{
    string path = ""/Asset/Table/{FILE_NAME}.asset"";
    var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Config_{FILE_NAME}>(path);
    Singleton = obj as Config_{FILE_NAME};
}
#endif
}";

        public const string Str_UIPanel_Script_Template = @"/// <summary>
/// 脚本仅第一次自动生成，可修改
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextFramework.UI
{
    public class {PREFAB_NAME} : UIWndBase
    {
        protected override void SetUIType()
        {
            this.mUIType = UIType.{PREFAB_NAME};
        }

        public override void InitWndOnAwake()
        {
            base.InitWndOnAwake();
        }

        public override void InitWndOnShart()
        {
            base.InitWndOnShart();
        }


        public override void OnShowWnd()
        {
            base.OnShowWnd();
        }

        public override void OnHideWnd()
        {
            base.OnHideWnd();
        }

        #region Messenger
        public override void RegisterMessage()
        {
            base.RegisterMessage();
        }
        public override void RemoveMessage()
        {
            base.RemoveMessage();
        }
        #endregion
    }
}
";
    }



}

