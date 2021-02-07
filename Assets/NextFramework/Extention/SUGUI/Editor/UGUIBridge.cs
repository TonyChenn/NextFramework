using NextFramework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NextFramework.SUGUI
{
    public class UGUIBridge
    {
        Assembly assembly;
        System.Type[] types;
        public UGUIBridge()
        {
            assembly = Assembly.Load("Assembly-CSharp-Editor");
            types = assembly.GetTypes();

        }
        public static void AddCanvas()
        {
            
        }
    }
}


