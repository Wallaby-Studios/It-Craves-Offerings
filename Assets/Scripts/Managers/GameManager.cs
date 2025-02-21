using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Game,
    GameEnd
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

    [SerializeField]
    private GameObject playerPrefab;

    public Player player;
    public Transform unitsParent;
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
                GameObject playerObject = Instantiate(playerPrefab, RoomManager.instance.PlayerSpawnPosition.position, Quaternion.identity, unitsParent);
                player = playerObject.GetComponent<Player>();
                RoomManager.instance.GameSetup();
                player.SetStats();
                break;
            case GameState.GameEnd:
                break;
        }

        currentGameState = newGameState;
        UIManager.instance.ChangeUIState(currentGameState);
    }

    public void ClearProjectiles() {
        for(int i = projectilesParent.childCount - 1; i >= 0; i--) {
            Destroy(projectilesParent.GetChild(i).gameObject);
        }
    }
}
