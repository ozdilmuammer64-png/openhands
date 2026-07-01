using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/medieval_realm_save.json";

    public static void SaveGame(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Game saved successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public static GameData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                GameData data = JsonUtility.FromJson<GameData>(json);
                Debug.Log("Game loaded successfully!");
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load game: " + e.Message);
        }
        return null;
    }

    public static bool HasSavedGame()
    {
        return File.Exists(savePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted!");
        }
    }
}