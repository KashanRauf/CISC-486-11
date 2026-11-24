using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager GSM;
    
    public GameState State { get; private set; }

    private bool stateInitialized = false;
    private bool levelCompleted = false;
    [Header("Story Dialogue")]
    [SerializeField] private TextAsset[] waveDialogues = new TextAsset[4];
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

    public static CapturePoint NameToObjective(string name)
    {
        for (int i = 0; i < GSM.Objectives.Length; i++)
        {
            if (GSM.Objectives[i].name == name) return GSM.Objectives[i];
        }
        Debug.LogError("NameToObjective failed");
        return null;
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
                StartCoroutine(StartWaveDialogue());
                break;
            case GameState.PAUSE:
                break;
            case GameState.BATTLE:
                SaveManager.StartBattle();
                Debug.Log("Battle started. Temporary coin rewards reset.");
                break;
            
            case GameState.VICTORY:
                if (!levelCompleted && SaveManager.CurrentSave != null)
                {
                    levelCompleted = true;

                    // Permanently save coins earned during this battle.
                    SaveManager.CommitBattleRewards();

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
                        Debug.Log("Final wave completed! Loading ending.");
                        SceneManager.LoadScene("BadEnding"); // will change when other endings are created
                        return;
                    }
                }
                break;

            case GameState.DEFEAT:
                SaveManager.DiscardBattleRewards();
                Debug.Log("Battle lost. Temporary coin rewards discarded.");
                break;
            case GameState.SHOP:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(g), g, "Invalid game state provided to GSM");
        }

        OnGameStateChange?.Invoke(g);

    }

    private IEnumerator StartWaveDialogue()
    {
        // Allow UIManager to finish initializing
        yield return null;

        if (State != GameState.CUTSCENE) 
            yield break;
        
        int level = SaveManager.CurrentSave.currentLevel;

        // No dialogue configured? Start combat normally
        if (level < 0 || level >= waveDialogues.Length || waveDialogues[level] == null)
        {
            Debug.Log("No dialogue for W" + level + ". Starting battle.");
            BeginBattleAfterDialogue();
            yield break;
        }

        DialogueManager dialogueManager = UIManager.GetUI<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogWarning("Dialogue manager not found, skipping dialogue");
            BeginBattleAfterDialogue();
            yield break;
        }

        // start battle when dialogue is finished
        dialogueManager.onDialogueFinished -= BeginBattleAfterDialogue;
        dialogueManager.onDialogueFinished += BeginBattleAfterDialogue;

        UIManager.Show(dialogueManager);
        dialogueManager.StartDialogue(waveDialogues[level]);

    }

    public void BeginBattleAfterDialogue()
    {
        if (State != GameState.CUTSCENE)
        {
            return;
        }

        Debug.Log("Dialogue finished. Starting battle W" + SaveManager.CurrentSave.currentLevel);

        SetGameState(GameState.BATTLE);
    }

    public void RetryWave() 
    {
        // TODO (UI): Connect this method to the Retry button on the
        // defeat screen. GameStateManager should display the defeat UI
        // when GameState.DEFEAT is triggered.
        //
        // RetryWave() reloads the current wave without advancing the
        // saved level or keeping coins earned during the failed battle.
        if (State != GameState.DEFEAT)
        {
            Debug.LogWarning("Cannot retry: battle is not defeated.");
            return;
        }

        SaveManager.DiscardBattleRewards();

        Debug.Log("Retrying wave W" + SaveManager.CurrentSave.currentLevel);

        SceneManager.LoadScene("SampleScene");
    }

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