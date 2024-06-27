using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class SaveManager
{
    public static void Save<T>(string key, T saveData)
    {
        string jsonDataString = JsonConvert.SerializeObject(saveData);
        File.WriteAllText(key, jsonDataString);
    }

    public static T Load<T>(string key) where T : new()
    {
        if (!File.Exists(key)) return new T();

        string jsonDataString = File.ReadAllText(key);
        return JsonConvert.DeserializeObject<T>(jsonDataString);

    }
}
