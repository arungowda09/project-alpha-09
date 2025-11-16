using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class SaveData
{
    public int rows;
    public int cols;
    public List<int> faceIndexes;  //sprite index is used for each card
    public List<bool> matched;     //each card is matched or not
    public int score;
}

public static class SaveLoadManager
{

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");


    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game Saved to " + SavePath);
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found at " + SavePath);
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("Game Loaded from " + SavePath);
        return data;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted from " + SavePath);
        }
        else
        {
            Debug.LogWarning("No save file to delete at " + SavePath);
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }


}
