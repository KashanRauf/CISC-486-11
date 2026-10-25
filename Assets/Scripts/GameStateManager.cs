using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager GSM;
    
    public GameState State { get; private set; }

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        GSM = this;
    }

    void Start()
    {
        // TODO: Dialogue and cinematic camera, automatically transition to LEVELSTART when done
        //SetGameState(GameState.CUTSCENE);

        // Temp
        SetGameState(GameState.LEVELSTART);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameState(GameState g)
    {
        this.State = g;
        // Applies changes caused, e.g. opening the pause menu
        switch (g)
        {
            case GameState.LEVELSTART:
                break;
            case GameState.CUTSCENE:
                break;
            case GameState.PAUSE:
                break;
            case GameState.BATTLE:
                break;
            case GameState.VICTORY:
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