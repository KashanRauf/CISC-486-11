using UnityEngine;


// A parent empty will have the UI's script component, and the UI elements as children (e.g. DialogueManager extends ManagedUI, and is the parent element of all Dialogue UI elements)
public abstract class ManagedUI : MonoBehaviour
{
    // Sometimes you can just leave the definition empty, only exists because it could be useful
    public abstract void Initialize();

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
}
