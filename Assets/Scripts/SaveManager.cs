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
        CurrentSave.medicCount = 3;
        CurrentSave.repairCount = 0;

        CurrentSave.foodStorageLevel = 0;
        CurrentSave.weaponStorageLevel = 0;

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

    public static void AddMoney(int amount)
    {
        if (CurrentSave == null)
        {
            return;
        }

        CurrentSave.money += amount;
        SaveGame();
    }

    public static bool SpendMoney(int amount)
    {
        if (CurrentSave == null || amount < 0)
        {
            return false;
        }

        if(CurrentSave.money < amount)
        {
            return false;
        }

        CurrentSave.money -= amount;
        SaveGame();
        return true;
    }

    public static void ChangeMorale(int amount)
    {
        if (CurrentSave == null)
        {
            return;
        }

        CurrentSave.morale = Mathf.Clamp(CurrentSave.morale + amount, 0, 100);
        SaveGame();
    }

    public static void AddCrew(string crewType)
    {
        if (CurrentSave == null)
        {
            return;
        }

        switch (crewType)
        {
            case "Melee":
                CurrentSave.meleeCount++;
                break;
            case "Ranged":
                CurrentSave.rangedCount++;
                break;
            case "Medic":
                CurrentSave.medicCount++;
                break;
            case "Repair":
                CurrentSave.repairCount++;
                break;
            default:
                Debug.LogWarning($"Unknown crew type: {crewType}");
                return;
        }
        SaveGame();
    }
        
    public static void SetLevel(int level)
    {
        if (CurrentSave == null)
        {
            return;
        }

        CurrentSave.currentLevel = Mathf.Clamp(level, 0, 4);
        SaveGame();
    }

}


