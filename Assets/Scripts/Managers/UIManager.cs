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
    private TMP_Text soulsText, healthText, damageText, moveSpeedText, attackTimeText, projectileSpeedText;
    [SerializeField]
    private TMP_Text gameEndText;

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
                UpdateBossCountdownText(RoomManager.instance.CurrentRoomIndex, RoomManager.instance.BossRoomIndex);
                break;
            case GameState.GameEnd:
                mainMenuUIParent.SetActive(false);
                gameUIParent.SetActive(false);
                gameEndUIParent.SetActive(true);
                
                // Update the game end text based on if the player succeeded
                if(GameManager.instance.player.CurrentHealth > 0) {
                    gameEndText.text = "IT IS PLEASED...";
                } else {
                    gameEndText.text = "IT IS DISPLEASED...";
                }
                break;
        }
    }

    private void SetupButtons() {
        playButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.Game));
        gameEndToMainMenuButton.onClick.AddListener(() => GameManager.instance.ChangeGameState(GameState.MainMenu));
    }

    public void UpdateBossCountdownText(int currentRoomIndex, int bossRoomIndex) {
        // Display text if the boss room is within 5 rooms AND the current room is NOT a boss room
        if(bossRoomIndex - currentRoomIndex <= 5 && currentRoomIndex != bossRoomIndex) {
            string roomText;
            if(currentRoomIndex == bossRoomIndex - 1) {
                roomText = "Prepare for Boss";
            } else {
                roomText = string.Format("{0} Rooms until Boss", bossRoomIndex - currentRoomIndex - 1);
            }

            bossCountdownText.gameObject.SetActive(true);
            bossCountdownText.text = roomText;
        } else {
            bossCountdownText.gameObject.SetActive(false);
        }
    }

    public void UpdateStats() {
        // Update souls text
        soulsText.text = GameManager.instance.player.SoulsCount.ToString();
        // Update each stat text
        foreach(Stat stat in GameManager.instance.player.Stats.Keys) {
            switch(stat) {
                case Stat.MaxHealth:
                    int currentHealth = (int)GameManager.instance.player.CurrentHealth;
                    healthText.text = string.Format("{0} / {1}", currentHealth, (int)GameManager.instance.player.Stats[stat]);
                    break;
                case Stat.Damage:
                    damageText.text = GameManager.instance.player.Stats[stat].ToString("F1");
                    break;
                case Stat.MoveSpeed:
                    int reducedMoveSpeedNum = (int)(GameManager.instance.player.Stats[stat] / 10f);
                    moveSpeedText.text = reducedMoveSpeedNum.ToString();
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
