using System.Collections.Generic;
using UnityEngine;


// Singleton class for managing UIs
public class UIManager : MonoBehaviour
{
    public static UIManager singleton { get; private set; }

    // Optional default, could be null, the HUD, the Shop UI, etc.
    [SerializeField] private ManagedUI defaultUI;

    // Could make it SerializeField but not needed since Start() will populate the list
    private ManagedUI[] uis;

    private ManagedUI activeUI;

    // Pretty much just for back buttons
    private readonly Stack<ManagedUI> history = new Stack<ManagedUI>();

    // Probably redundant, but finds a specific ManagedUI by the type
    public static T GetUI<T>() where T : ManagedUI
    {
        for (int i = 0; i < singleton.uis.Length; i++)
        {
            if (singleton.uis[i] is T ui)
            {
                return ui;
            }
        }

        return null;
    }

    // Displays a specified ManagedUI
    public static void Show(ManagedUI ui, bool remember = true)
    {
        if (singleton.activeUI != null)
        {
            if (remember)
            {
                singleton.history.Push(singleton.activeUI);
            }
            singleton.activeUI.Hide();
        }

        singleton.activeUI = ui;
        ui.Show();
    }

    // Finds the ManagedUI of type T (e.g. DialogueManager) and shows
    public static void Show<T>(bool remember = true) where T : ManagedUI
    {
        for (int i = 0; i < singleton.uis.Length; i++)
        {
            if (singleton.uis[i] is T)
            {
                //if (singleton.activeUI != null)
                //{
                //    if (remember)
                //    {
                //        singleton.history.Push(singleton.activeUI);
                //    }
                //    singleton.activeUI.Hide();
                //}

                //singleton.activeUI = singleton.uis[i];
                //singleton.activeUI.Show();
                Show(singleton.uis[i], remember);
            }
        }
    }

    // Does the same as both Show() and Get() at the same time
    public static T ShowAndGet<T>(bool remember = true) where T : ManagedUI
    {
        T res = null;
        for (int i = 0; i < singleton.uis.Length; i++)
        {
            if (singleton.uis[i] is T)
            {
                Show(singleton.uis[i], remember);
                res = (T) singleton.uis[i];
            }
        }

        return res;
    }

    public static void ShowLast()
    {
        if (singleton.history.Count == 0) return;

        Show(singleton.history.Pop(), false);
    }

    private void Awake() => singleton = this;

    private void Start()
    {
        uis = (ManagedUI[]) FindObjectsByType(typeof(ManagedUI));

        foreach (var ui in uis)
        {
            Debug.Log(ui.name);
            ui.Initialize();
            ui.Hide();
        }

        if (singleton.defaultUI != null)
        {
            Show(singleton.defaultUI);
        }
    }


}
