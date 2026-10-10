using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // Each trigger carries a reference to the JSON TextAsset that holds the dialogue
    public TextAsset data;

    private Dialogue deserializedData;

    // Can be triggered by a variety of things like scripted events, interactions, etc.
    public void TriggerDialogue()
    {
        UIManager.ShowAndGet<DialogueManager>().StartDialogue(data);

        //DialogueManager dm = FindObjectOfType<DialogueManager>();
        //dm.StartDialogue(data);
    }
}