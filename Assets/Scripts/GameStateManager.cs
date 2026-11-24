using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager GSM;
    
    public GameState State { get; private set; }

    private bool stateInitialized = false;
    private bool levelCompleted = false;
    private bool enemiesHaveSpawned = false;

    [SerializeField] TeamManager PlayerTeam;
    [SerializeField] TeamManager EnemyTeam;

    [SerializeField] CapturePoint[] Objectives = new CapturePoint[3];

    public static event Action<GameState> OnGameStateChange;

    public static CapturePoint[] GetObjectives => GSM.Objectives;

    public static TeamManager GetPlayerTeam => GSM.PlayerTeam;

    public static TeamManager GetEnemyTeam => GSM.EnemyTeam;
    

    private void Awake()
    {
        GSM = this;
    }

    void Start()
    {
        if (SaveManager.CurrentSave == null)
        {
            if (SaveManager.SaveExists())
            {
                SaveManager.LoadGame();
            }
            else
            {
                Debug.LogWarning(
                    "No save found. Creating a new game."
                );

                SaveManager.CreateNewGame();
            }
        }

        Debug.Log(
            "Starting wave W" +
            SaveManager.CurrentSave.currentLevel +
            " with " +
            SaveManager.CurrentSave.money +
            " coins and " +
            SaveManager.CurrentSave.morale +
            " morale."
        );

        // TODO: Dialogue and cinematic camera, automatically transition to LEVELSTART when done
        //SetGameState(GameState.CUTSCENE);

        SetGameState(GameState.LEVELSTART);
    }

    // Update is called once per frame
    void Update()
    {
        if (State != GameState.BATTLE)
        {
            return;
        }

        // track whether enemies have appeared in this battle
        if (EnemyTeam != null && EnemyTeam.squads.Count > 0)
        {
            enemiesHaveSpawned = true;
        }

        // defeat if all ship objectives are captured 
        if (AllObjectivesCaptured())
        {
            SetGameState(GameState.DEFEAT);
            return;
        }

        // victory when every spawned enemy squad is defeated 
        if (enemiesHaveSpawned &&
            EnemyTeam != null &&
            EnemyTeam.squads.Count == 0)
        {
            Debug.Log("All enemy squads defeated! Victory!");
            SetGameState(GameState.VICTORY);
        }
        
    }

    public void SetGameState(GameState g)
    {
        if (stateInitialized && State == g)
        {
            return;
        }
        stateInitialized = true;
        this.State = g;
        // Applies changes caused, e.g. opening the pause menu
        switch (g)
        {
            case GameState.LEVELSTART:
                // Do any preconditions to starting the level like playing a cutscene or dialogue
                SetGameState(GameState.CUTSCENE);
                break;
            case GameState.CUTSCENE:
                // Wait until the cutscene or dialogue ends
                SetGameState(GameState.BATTLE);
                break;
            case GameState.PAUSE:
                break;
            case GameState.BATTLE:
                break;
            
            case GameState.VICTORY:
                if (!levelCompleted && SaveManager.CurrentSave != null)
                {
                    levelCompleted = true;

                    int currentLevel = SaveManager.CurrentSave.currentLevel;

                    if (currentLevel < 3)
                    {
                        SaveManager.SetLevel(currentLevel + 1);

                        Debug.Log("Wave W" + currentLevel +
                            " completed. Next wave: W" +
                            SaveManager.CurrentSave.currentLevel);

                        OpenShop();
                        return;
                    }
                    else if (currentLevel == 3)
                    {
                        SaveManager.SetLevel(4);
                        Debug.Log("Final wave completed! Ending required.");
                    }
                }
                break;

            case GameState.DEFEAT:
                break;
            case GameState.SHOP:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(g), g, "Invalid game state provided to GSM");
        }

        OnGameStateChange?.Invoke(g);

    }

    # if UNITY_EDITOR
    [ContextMenu("TEST - Trigger Victory")]
    private void TestVictory()
    {
        Debug.Log("TEST: Manually triggering victory");
        SetGameState(GameState.VICTORY);
    }
    #endif

    public void OpenShop()
    {
        SetGameState(GameState.SHOP);
        SceneManager.LoadScene("ShopScene");
    }

    private bool AllObjectivesCaptured()
    {
        if(Objectives == null || Objectives.Length == 0)
        {
            return false;
        }

        foreach (CapturePoint objective in Objectives)
        {
            if(objective == null || objective.owner != Team.Attack)
            {
                return false;
            }
        }
        return true;
    }

}

public enum GameState
{
    LEVELSTART,
    CUTSCENE,
    PAUSE,
    BATTLE,
    VICTORY,
    DEFEAT,
    SHOP,
}