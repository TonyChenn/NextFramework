local HotFixHelper = class("HotFixHelper")
-- 创建数组
function HotFixHelper:new_array(itemType, count)
    return CS.XLuaHelper.CreateArray(itemType, count)
end

-- 创建List
function HotFixHelper:new_list(itemType)
    return CS.XLuaHelper.CreateList(itemType)
end
-- 列表迭代
function HotFixHelper:list_ipair(cs_list)
    return self.list_iter, cs_list, -1
end
function HotFixHelper:list_iter(cs_list, index)
    index = index + 1
    if index < cs_list.count then
        return index, cs_list[index]
    end
end

-- 创建字典
function HotFixHelper:new_dict(keyType, valueType)
    return CS.XLuaHelper.CreateDictionary(keyType, valueType)
end
function HotFixHelper:try_get_value(cs_dict, key)
    local hasFind, value = dict:TryGetValue(key)
    return hasFind and value or nil
end
-- 字典迭代
function HotFixHelper:dict_ipair(cs_dict)
    local enumerator=cs_dict:GetEnumerator()
    return self.dict_iter, enumerator
end
local function dict_iter(cs_enumerator)
    if cs_enumerator:MoveNext() then
         local current = cs_enumerator.Current
        return current.Key, current.Value
     end
 end


-- 创建泛型Action
function HotFixHelper:new_action() 
    -- TODO
end
function HotFixHelper:new_delegate()
    -- TODo
end
return HotFixHelper