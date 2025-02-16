using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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
