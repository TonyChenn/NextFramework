#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    
    public class DirectionWrap
    {
		public static void __Register(RealStatePtr L)
        {
		    ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
		    Utils.BeginObjectRegister(typeof(Direction), L, translator, 0, 0, 0, 0);
			Utils.EndObjectRegister(typeof(Direction), L, translator, null, null, null, null, null);
			
			Utils.BeginClassRegister(typeof(Direction), L, null, 5, 0, 0);

            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Left", Direction.Left);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Right", Direction.Right);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Buttom", Direction.Buttom);
            
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "Top", Direction.Top);
            

			Utils.RegisterFunc(L, Utils.CLS_IDX, "__CastFrom", __CastFrom);
            
            Utils.EndClassRegister(typeof(Direction), L, translator);
        }
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CastFrom(RealStatePtr L)
		{
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			LuaTypes lua_type = LuaAPI.lua_type(L, 1);
            if (lua_type == LuaTypes.LUA_TNUMBER)
            {
                translator.PushDirection(L, (Direction)LuaAPI.xlua_tointeger(L, 1));
            }
			
            else if(lua_type == LuaTypes.LUA_TSTRING)
            {

			    if (LuaAPI.xlua_is_eq_str(L, 1, "Left"))
                {
                    translator.PushDirection(L, Direction.Left);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Right"))
                {
                    translator.PushDirection(L, Direction.Right);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Buttom"))
                {
                    translator.PushDirection(L, Direction.Buttom);
                }
				else if (LuaAPI.xlua_is_eq_str(L, 1, "Top"))
                {
                    translator.PushDirection(L, Direction.Top);
                }
				else
                {
                    return LuaAPI.luaL_error(L, "invalid string for Direction!");
                }

            }
			
            else
            {
                return LuaAPI.luaL_error(L, "invalid lua type for Direction! Expect number or string, got + " + lua_type);
            }

            return 1;
		}
	}
    
}