using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Linq;

// TODO: Should probably be a singleton but I'm not gonna "fix" something that isn't broken
public class DialogueManager : ManagedUI
{
    // The Queue contains the remaining lines of a dialogue
    public Queue<Dialogue> dialogue;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI activeDialogue;
    public ChangingPortrait portrait;


    // For making the "typing" effect of the text feel nicer
    private char[] punctuation = ".,…!?:;~-".ToCharArray();

    void Start()
    {
        dialogue = new Queue<Dialogue>();
    }

    // Called by the DialogueTrigger
    public void StartDialogue(TextAsset data)
    {
        Debug.Log("Loading dialogue: " + data.name);
        dialogue.Clear();
        BuildDialogue(data);
        this.Show();
    }

    private void BuildDialogue(TextAsset data)
    {
        // Load the JSON data
        // Each item becomes a dialogue that is enqueued
        foreach (var item in JsonHelper.FromJSON<Dialogue>(data.text))
        {
            dialogue.Enqueue(item);
        }

        // Display the first piece of dialogue
        DisplayNextSentence();

        // JSON structure stuff
        //Dialogue[] dia = new Dialogue[2];
        //dia[0] = new Dialogue();
        //dia[0].text = "hi im dialogue";
        //dia[0].speaker = "speaker";
        //dia[0].portrait = "portrait";
        //dia[1] = new Dialogue();
        //dia[1].text = "hi im another dialogue";
        //dia[1].speaker = "speaker2";
        //dia[1].portrait = "portrait2";
        //Debug.Log(JsonHelper.ToJSON<Dialogue>(dia));
    }

    // May be called automatically or by the player using the ContinueButton
    public void DisplayNextSentence()
    {
        if (dialogue.Count == 0)
        {
            TerminateDialogue();
            return;
        }
        
        // Gets the next sentence and updates the speaker name, text, and portrait
        Dialogue cur = dialogue.Dequeue();
        speakerName.text = cur.speaker;
        // activeDialogue.text = cur.text; // Directly place text without typing effect
        // Typing effect
        StopAllCoroutines(); // Terminates typing if player skips before it ends
        StartCoroutine(TypeSentence(cur.text, 1));
        portrait.SetByName(cur.portrait);

    }

    // Creates the "typing" effect of dialogue
    IEnumerator TypeSentence(string sentence, float speed)
    {
        // TODO: Make speed a setting that can be changed by the player, -1 means instant
        if (speed == -1)
        {
            activeDialogue.text = sentence;
            yield return null;
        }

        activeDialogue.text = "";
        foreach (char c in sentence.ToCharArray())
        {
            // Types
            activeDialogue.text += c;

            // Delay between each new character, punctuation has a longer delay because I like it that way
            float delay = speed * 0.05f;
            delay *= punctuation.Contains<char>(c) ? 5 : 1;
            //Debug.Log(speed + " " + delay + " " + c);

            yield return new WaitForSeconds(delay);
        }
    }

    private void TerminateDialogue()
    {
        //Debug.Log("Conversation ended");
        this.Hide();
    }

    public override void Initialize() {}
}
