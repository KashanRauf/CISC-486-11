using UnityEngine;

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
}