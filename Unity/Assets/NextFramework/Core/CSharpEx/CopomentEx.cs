using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentEx
{
    public static T SetActive<T>(this T component, bool show) where T : Component
    {
        if(component!=null)
            component.gameObject.SetActive(show);

        return component;
    }
    public static bool IsActive<T>(this T component) where T : Component
    {
        return component.gameObject.activeSelf;
    }

    public static T SetEnable<T>(this T behavior, bool enable) where T : Behaviour
    {
        if(behavior!=null)
            behavior.enabled = enable;

        return behavior;
    }

    public static bool IsEnable<T>(this T behavior) where T : Behaviour
    {
        return behavior.enabled;
    }
}

public static class ObjectEx
{
    public static T Instantiate<T>(this T gameObject) where T : Object
    {
        return Object.Instantiate(gameObject);
    }

    public static T Instantiate<T>(this T gameObject, Vector3 position, Quaternion rotation) where T: Object
    {
        return Object.Instantiate(gameObject, position, rotation);
    }
    public static T Instantiate<T>(this T gameObject, Vector3 position, Quaternion rotation,Transform parent) where T : Object
    {
        return Object.Instantiate(gameObject, position, rotation, parent);
    }
}
