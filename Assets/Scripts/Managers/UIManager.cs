using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private TMP_Text bossCountdownText;

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
                UpdateBossCountdownText(RoomManager.instance.RoomsBeforeBossRoom);
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

    public void UpdateBossCountdownText(int roomsUntilBossRoom) {
        if(roomsUntilBossRoom <= 5) {
            string roomText;
            switch(roomsUntilBossRoom) {
                case 1:
                    roomText = "1 Room";
                    break;
                case 2:
                    roomText = "2 Rooms";
                    break;
                case 3:
                    roomText = "3 Rooms";
                    break;
                case 4:
                    roomText = "4 Rooms";
                    break;
                case 5:
                    roomText = "5 Rooms";
                    break;
                default:
                    roomText = "# Rooms";
                    break;
            }

            bossCountdownText.gameObject.SetActive(true);
            bossCountdownText.text = string.Format("{0} until Boss", roomText);
        } else {
            bossCountdownText.gameObject.SetActive(false);
        }
    }
}
