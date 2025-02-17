using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static UIManager instance = null;

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
    private GameObject mainMenuUIParent, gameUIParent, gameEndUIParent;
    [SerializeField]
    private Button playerButton;

    // Start is called before the first frame update
    void Start()
    {
        SetupButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeUIState(GameState newGameState) {
        switch(newGameState) {
            case GameState.MainMenu:
                mainMenuUIParent.SetActive(true);
                gameUIParent.SetActive(false);
                gameEndUIParent.SetActive(false);
                break;
            case GameState.Game:
                mainMenuUIParent.SetActive(false);
                gameUIParent.SetActive(true);
                gameEndUIParent.SetActive(false);
                break;
            case GameState.GameEnd:
                mainMenuUIParent.SetActive(false);
                gameUIParent.SetActive(false);
                gameEndUIParent.SetActive(true);
                break;
        }
    }

    private void SetupButtons() {
        playerButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.Game));
    }
}
