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
    private Button playButton, gameEndToMainMenuButton;
    [SerializeField]
    private TMP_Text bossCountdownText;
    [SerializeField]
    private TMP_Text healthText, moveSpeedText, damageText, attackTimeText, projectileSpeedText;

    // Start is called before the first frame update
    void Start()
    {
        SetupButtons();
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
                UpdateStats();
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
        playButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.Game));
        gameEndToMainMenuButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.MainMenu));
    }

    public void UpdateBossCountdownText(int roomsUntilBossRoom) {
        if(roomsUntilBossRoom <= 5) {
            string roomText;

            if(roomsUntilBossRoom == 1) {
                roomText = "1 Room";
            } else if(roomsUntilBossRoom > 1) {
                roomText = string.Format("{0} Rooms", roomsUntilBossRoom);
            } else {
                roomText = "# Rooms";
            }

            bossCountdownText.gameObject.SetActive(true);
            bossCountdownText.text = string.Format("{0} until Boss", roomText);
        } else {
            bossCountdownText.gameObject.SetActive(false);
        }
    }

    public void UpdateStats() {
        foreach(Stat stat in GameManager.instance.player.Stats.Keys) {
            switch(stat) {
                case Stat.MoveSpeed:
                    int reducedMoveSpeedNum = (int)(GameManager.instance.player.Stats[stat] / 10f);
                    moveSpeedText.text = reducedMoveSpeedNum.ToString();
                    break;
                case Stat.MaxHealth:
                    int currentHealth = (int)GameManager.instance.player.CurrentHealth;
                    healthText.text = string.Format("{0} / {1}", currentHealth, (int)GameManager.instance.player.Stats[stat]);
                    break;
                case Stat.Damage:
                    damageText.text = ((int)GameManager.instance.player.Stats[stat]).ToString("F1");
                    break;
                case Stat.AttackTime:
                    attackTimeText.text = string.Format("{0}s", GameManager.instance.player.Stats[stat].ToString("F2"));
                    break;
                case Stat.ProjectileSpeed:
                    int reducedProjectileSpeedNum = (int)(GameManager.instance.player.Stats[stat] / 10f);
                    projectileSpeedText.text = reducedProjectileSpeedNum.ToString();
                    break;
            }
        }
    }
}
