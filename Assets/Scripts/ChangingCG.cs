using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// A derivative of/replacement for the existing Image class for dialogue cgs
// Holds all cgs that may be used in dialogue and enables switching them through the DialogueManager
public class ChangingCG : Image
{
    [SerializeField]
    private Sprite[] cgs;
    private string cur_name;
    private int cur_index;
    private Hashtable namesToIndex;


    // Creates a map of names to indices of cgs
    public void Start()
    {
        cur_name = this.sprite.name;
        namesToIndex = new Hashtable();
        for (int i = 0; i < cgs.Length; i++)
        {
            if (cgs[i].name.Equals(cur_name))
            {
                cur_index = i;
            }
            namesToIndex.Add(cgs[i].name, i);
        }
    }

    // Uses the hashtable populated by Start(), takes sprite names from the DialogueManager and switches the portrait to display them
    public void SetByName(string name)
    {
        if (cur_name.Equals(name))
        {
            //Debug.Log("Portrait does not change");
            return;
        }
        cur_index = (int) namesToIndex[name];
        cur_name = name;
        this.sprite = cgs[cur_index];

    }
}
