using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public int currentLevel;
    public int currentCoins;
}

public class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(int level, int coins)
    {
        GameData data = new GameData { currentLevel = level, currentCoins = coins };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}
