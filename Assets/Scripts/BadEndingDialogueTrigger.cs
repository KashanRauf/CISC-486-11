using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BadEndingDialogue : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public TextAsset BadEndingDialogueFile;

    IEnumerator Start()
    {
        Debug.Log("Bad Ending Trigger Started");

        // Wait for UIManager to initialize
        yield return null;

        Debug.Log("Waiting for dialogue to finish...");

        // Register callback
        dialogueManager.onDialogueFinished = GoToStartMenu;

        // Show dialogue
        UIManager.Show(dialogueManager);

        // Start ending narrative
        dialogueManager.StartDialogue(BadEndingDialogueFile);
    }

    public void GoToStartMenu()
    {
        Debug.Log("Loading StartMenu scene now!");

        SceneManager.LoadScene("StartMenu");
    }
}