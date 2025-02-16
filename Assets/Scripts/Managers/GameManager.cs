using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Game,
    GameEnd
}

public enum Stat {
    MoveSpeed, 
    Health, 
    Damage, 
    AttackTime, 
    ProjectileSpeed
}

public class GameManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static GameManager instance = null;

    // Awake is called even before start 
    // (I think its at the very beginning of runtime)
    private void Awake() {
        // If the reference for this script is null, assign it this script
        if(instance == null)
            instance = this;
        // If the reference is to something else (it already exists)
        // than this is not needed, thus destroy it
        else if(instance != this)
            Destroy(gameObject);
    }
    #endregion

    public Player player;
    public Transform projectilesParent;

    private GameState currentGameState;

    public GameState CurrentGameState { get { return currentGameState; } }

    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void ChangeGameState(GameState newGameState) {
        switch(newGameState) {
            case GameState.MainMenu:
                break;
            case GameState.Game:
                break;
            case GameState.GameEnd:
                break;
        }

        currentGameState = newGameState;
    }
}
