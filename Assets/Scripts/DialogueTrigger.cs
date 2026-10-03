using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // Each trigger carries a reference to the JSON TextAsset that holds the dialogue
    public TextAsset data;

    // Can be triggered by a variety of things like scripted events, interactions, etc.
    public void TriggerDialogue()
    {
        // TODO: Change to use singleton later
        FindObjectOfType<DialogueManager>().StartDialogue(data);
    }
}