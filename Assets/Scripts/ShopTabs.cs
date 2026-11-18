
using UnityEngine;
using UnityEngine.UI;

public class ShopTabs : MonoBehaviour
{
    [Header("Shop Content")]
    [SerializeField] private GameObject crewContent;
    [SerializeField] private GameObject captureContent;
    [SerializeField] private GameObject moraleContent;

    [Header("Scroll View")]
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        ShowCrew();
    }

    public void ShowCrew()
    {
        ShowContent(crewContent);
    }

    public void ShowCapturePoints()
    {
        ShowContent(captureContent);
    }

    public void ShowMorale()
    {
        ShowContent(moraleContent);
    }

    private void ShowContent(GameObject selectedContent)
    {
        crewContent.SetActive(selectedContent == crewContent);
        captureContent.SetActive(selectedContent == captureContent);
        moraleContent.SetActive(selectedContent == moraleContent);

        scrollRect.content = selectedContent.GetComponent<RectTransform>();

        scrollRect.verticalNormalizedPosition = 1f;
    }
}
