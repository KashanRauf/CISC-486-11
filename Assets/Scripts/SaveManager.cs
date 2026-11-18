using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static SaveData CurrentSave;

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public static void CreateNewGame()
    {
        CurrentSave = new SaveData();

        CurrentSave.currentLevel = 0;
        CurrentSave.money = 0;
        CurrentSave.morale = 50;

        CurrentSave.meleeCount = 3;
        CurrentSave.rangedCount = 3;
        CurrentSave.medicCount = 1;
        CurrentSave.repairCount = 1;

        SaveGame();
    }
    public static void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData LoadGame()
    {
        string json = File.ReadAllText(SavePath);
        CurrentSave = JsonUtility.FromJson<SaveData>(json);

        return CurrentSave;
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }
}

