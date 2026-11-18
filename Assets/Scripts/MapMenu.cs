using UnityEngine;

public class MapMenu : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;

    private void Start()
    {
        mapPanel.SetActive(false);
    }

    public void CloseMap()
    {
        mapPanel.SetActive(false);
    }
}