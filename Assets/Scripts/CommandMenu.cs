using UnityEngine;

public class CommandMenu : ManagedUI
{
    public static CommandMenu singleton;

    // Display all of the player's squads
    // Have them fight a certain squad or go to a capture point

    // UI Elements within the menu
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] public Sprite[] chibis;
    [SerializeField] Transform optionsParent;

    public static Sprite[] GetChibis => singleton.chibis;

    TeamManager allies;
    TeamManager enemies;
    CapturePoint[] objectives;
    SquadSelect[] options;

    // Will depend on morale or some other metric, for now just set to 3
    int commandLimit;
    int activeCommands;

    public override void Initialize() 
    {
        // Fetch game state info, this is needed to only allow opening the command menu during battle
        // As well as pausing the world while the captain issues commands
        // Also fetching squads and objectives data
        singleton = this;

        allies = GameStateManager.GetPlayerTeam;
        enemies = GameStateManager.GetEnemyTeam;
        objectives = GameStateManager.GetObjectives;

        commandLimit = 3;
        activeCommands = 0;

        title.text = $"Squads to command ({activeCommands}/{commandLimit})";

        // Selectors for squads (will need to update every time it's opened)
        RefreshSelections();
    }

    public override void Show()
    {
        base.Show();
        RefreshSelections();
    }

    public void RefreshSelections()
    {
        // Destroy old menu (maybe do this on close instead)
        Debug.Log($"options exist? {options != null}");
        if (options != null)
        {
            DestroySelections();
            Debug.Log($"Old objects destroyed {options.Length}");
        }

        // Init options
        options = new SquadSelect[allies.squads.Count];

        for (int i = 0; i < allies.squads.Count; i++)
        {
            options[i] = Instantiate(Prefabs.prefabs.squadSelect, optionsParent, false).GetComponent<SquadSelect>();
            options[i].SetTitle(allies.squads[i].behavior, i);
            options[i].SetChibis(allies.squads[i].GetMembers);
            options[i].transform.position -= new Vector3(0, 100*i, 0);
        }
    }

    public void DestroySelections()
    {
        Debug.Log($"Destroying existing SquadSelects: {options.Length}");
        foreach (var item in options)
        {
            Destroy(item.gameObject);
        }
    }

}
