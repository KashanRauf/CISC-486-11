using Unity.Cinemachine;
using UnityEngine;

public class CommandMenu : ManagedUI
{
    public static CommandMenu singleton;

    // Display all of the player's squads
    // Have them fight a certain squad or go to a capture point

    // UI Elements within the menu
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] TMPro.TextMeshProUGUI error;
    [SerializeField] public Sprite[] chibis;
    [SerializeField] Transform optionsParent;
    [SerializeField] CinemachineCamera camera;
    CinemachinePositionComposer posComp;

    private Transform originalLook;
    private Transform originalFollow;

    public static Sprite[] GetChibis => singleton.chibis;

    TeamManager allies;
    TeamManager enemies;
    CapturePoint[] objectives;
    SquadSelect[] options;
    Squad selected = null;

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

        //RefreshSelections();

        // Camera
        originalLook = camera.LookAt;
        originalFollow = camera.Follow;
        posComp = camera.GetComponent<CinemachinePositionComposer>();
    }

    public override void Show()
    {
        error.text = "";
        RefreshSelections();
        base.Show();
        selected = null;
    }

    public override void Hide()
    {
        // Reset camera
        camera.LookAt = originalLook;
        camera.Follow = originalFollow;
        //CameraOffset(.5f, .5f);
        base.Hide();
        selected = null;
    }

    public void RefreshSelections()
    {
        // Destroy old menu (maybe do this on close instead)
        //Debug.Log($"options exist? {options != null}");
        if (options != null)
        {
            DestroySelections();
            //Debug.Log($"Old objects destroyed {options.Length}");
        }

        // Init options
        options = new SquadSelect[allies.squads.Count];

        for (int i = 0; i < allies.squads.Count; i++)
        {
            options[i] = Instantiate(Prefabs.prefabs.squadSelect, optionsParent, false).GetComponent<SquadSelect>();
            options[i].squad = allies.squads[i];
            options[i].SetParent(this);
            options[i].SetTitle(allies.squads[i].behavior, i);
            options[i].SetChibis(allies.squads[i].GetMembers);
            options[i].transform.position -= new Vector3(0, 100*i, 0);
        }
    }

    public void DestroySelections()
    {
        //Debug.Log($"Destroying existing SquadSelects: {options.Length}");
        foreach (var item in options)
        {
            Destroy(item.gameObject);
        }
    }

    // Camera functionality
    public void SelectSquad(Squad s)
    {
        selected = s;

        Transform t = s.GetSquadLeader().transform;
        camera.LookAt = t;
        camera.Follow = t;
        //CameraOffset(.7f, .5f);
    }

    public void CameraOffset(float x, float y)
    {
        if (posComp == null) return;
        posComp.Composition.ScreenPosition = new Vector2(x, y);
    }

    public void CommandFood()
    {
        DoCommand(0);
    }

    public void CommandStern()
    {
        DoCommand(1);
    }

    public void CommandArmoury()
    {
        DoCommand(2);
    }

    public void DoCommand(int i)
    {
        if (selected == null)
        {
            error.text = "Choose a squad before issuing a command!";
            return;
        }

        selected.behavior.CommandHolding(objectives[i]);
        UIManager.CloseCurrent();
    }
}
