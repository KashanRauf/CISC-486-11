using UnityEngine;
using UnityEngine.SceneManagement;

public class UIOpenSettings : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;

    public void OpenSettings()
    {
        Debug.Log("Open Settings");

        UIManager.Show<SettingsMenu>();
    }

    public void OpenMap()
    {
        Debug.Log("Open Map");

        mapPanel.SetActive(true);
    }

    public void GoToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void GoToHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }
}