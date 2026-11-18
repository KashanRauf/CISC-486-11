using UnityEngine;

public class SaveTest : MonoBehaviour
{
    void Start()
    {
        SaveManager.CreateNewGame();

        SaveManager.CurrentSave.money = 250;
        SaveManager.CurrentSave.morale = 65;

        SaveManager.SaveGame();

        SaveData loadedData = SaveManager.LoadGame();

        Debug.Log("Loaded Money: " + loadedData.money);
        Debug.Log("Loaded Morale: " + loadedData.morale);
    }
}