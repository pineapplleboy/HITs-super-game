using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveManager
{
    public static void Save<T>(string key, T saveData) where T : SaveData.World
    {
        Debug.Log(saveData.world[0, 0].name);
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        string jsonDataString = JsonConvert.SerializeObject(saveData, Formatting.Indented, settings);


        PlayerPrefs.SetString(key, jsonDataString);
        Debug.Log(JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key)).world[0,0].name);
    }

    public static T Load<T>(string key) where T : new()
    {
        if (PlayerPrefs.HasKey(key))
        {
            string loadedString = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<T>(loadedString);
        }
        else
        {
            return new T();
        }
    }
}
