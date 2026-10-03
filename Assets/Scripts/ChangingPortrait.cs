using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// A derivative replacement of the typical Image class for dialogue portraits (could be applied elsewhere maybe)
// Holds all portraits that may be used in dialogue and enables switching them through the DialogueManager
public class ChangingPortrait : Image
{
    [SerializeField]
    private Sprite[] portraits;
    private string cur_name;
    private int cur_index;
    private Hashtable namesToIndex;


    // Creates a map of names to indices of portraits
    public void Start()
    {
        cur_name = this.sprite.name;
        namesToIndex = new Hashtable();
        for (int i = 0; i < portraits.Length; i++)
        {
            if (portraits[i].name.Equals(cur_name))
            {
                cur_index = i;
            }
            namesToIndex.Add(portraits[i].name, i);
        }
    }

    // Uses the hashtable populated by Start(), takes sprite names from the DialogueManager and switches the portrait to display them
    public void SetByName(string name)
    {
        if (cur_name.Equals(name))
        {
            Debug.Log("Portrait does not change");
            return;
        }
        cur_index = (int) namesToIndex[name];
        cur_name = name;
        this.sprite = portraits[cur_index];

    }
}
