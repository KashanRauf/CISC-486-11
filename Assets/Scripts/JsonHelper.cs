using System;
using Unity.VisualScripting;
using UnityEngine;

// A helper class for serializing/deserializing JSON TextAssets like Dialogue
public static class JsonHelper
{
    public static T[] FromJSON<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJSON<T>(T[] arr)
    {
        Wrapper<T> wrap = new Wrapper<T>();
        wrap.Items = arr;
        return JsonUtility.ToJson(wrap, true);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
