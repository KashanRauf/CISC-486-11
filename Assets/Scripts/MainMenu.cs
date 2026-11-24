using UnityEngine;
using UnityEngine.SceneManagement;

// Basic menu functions
public class MainMenu : ManagedUI
{
    
    public void StartNewGame()
    {
        Debug.Log("New Game");
        // If save data exists, then ask for confirmation before starting with a popup box
        // Else just start a new one

        SaveManager.CreateNewGame();
        SceneManager.LoadScene("HomeScene");
    }

    public void ContinueGame()
    {
        Debug.Log("Continue Game");
        // Button disabled if there isn't any save data
        // Otherwise should go to the beginning of the next uncompleted level

        if (SaveManager.SaveExists())
        {
            // SaveManager.LoadGame();
            // if (SaveManager.CurrentSave.currentLevel == 0)
            // {
            //     SceneManager.LoadScene("HomeScene");
            // }
            // else if (SaveManager.CurrentSave.currentLevel < 4)
            // {
            //     SceneManager.LoadScene("ShopScene");
            // }
            // else
            // {
            //     Debug.Log("Game completed (ending transition not yet implemented)");
            //     SceneManager.LoadScene("HomeScene");
            // }

            SaveData save = SaveManager.LoadGame();

            if (save.currentLevel == 0)
            {
                // tutorial hasn't beebn completed, skip shop
                SceneManager.LoadScene("HomeScene");
            }
            else if (save.currentLevel < 4)
            {
                // resume at the shop before the next wave
                SceneManager.LoadScene("ShopScene");
            }
            else
            {
                // final wave completed
                // TODO: connect ending
                Debug.Log("Game completed. Ending not yet implemented");
                SceneManager.LoadScene("HomeScene");
            }
        }
        else
        {
            Debug.Log("No save data found.");
        }
    }

    public void OpenSettings()
    {
        UIManager.Show<SettingsMenu>();
        Debug.Log("Open Settings");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public override void Initialize() {}
}
