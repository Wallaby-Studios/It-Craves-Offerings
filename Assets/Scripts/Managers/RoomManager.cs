using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Combat,
    Heal,
    Upgrade,
    Boss
}

public class RoomManager : MonoBehaviour {
    #region Singleton Code
    // A public reference to this script
    public static RoomManager instance = null;

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

    // Spawn Positions
    [SerializeField]
    private Transform playerSpawnPosition;
    [SerializeField]
    private List<Transform> pickupSpawnPositions;
    // Doors
    [SerializeField]
    private GameObject bossDoorClosedTilemap, bossDoorOpenTilemap, nonBossRoomWalls, bossRoomWalls;
    [SerializeField]
    private List<GameObject> nonBossDoorsClosedTilemaps, nonBossDoorsOpenTilemaps;
    // Room Objects
    [SerializeField]
    private Transform roomObjectsParent;
    [SerializeField]
    private GameObject healthPickupPrefab, upgradePickupPrefab, soulPickupPrefab;
    // Stat Sprites
    [SerializeField]
    private Sprite healthSprite, damageSprite, moveSpeedSprite, attackSpeedSprite, projectileSpeedSprite;

    private RoomType currentRoomType;
    private int currentRoomIndex;
    private int bossRoomIndex;

    private Dictionary<UpgradeTier, int> tierSoulCostMap;
    private Dictionary<UpgradeTier, (float, float)> tierStatAmountMap;
    private Dictionary<Stat, Sprite> statSpriteMap;

    public Transform PlayerSpawnPosition { get { return playerSpawnPosition; } }
    public RoomType CurrentRoomType { get { return currentRoomType; } }
    public int CurrentRoomIndex { get { return currentRoomIndex; } }
    public int BossRoomIndex { get { return bossRoomIndex; } }
    public Dictionary<UpgradeTier, int> TierSoulCostMap { get { return tierSoulCostMap; } }
    public Dictionary<UpgradeTier, (float, float)> TierStatAmountMap { get { return tierStatAmountMap; } }
    public Dictionary<Stat, Sprite> StatSpriteMap { get { return statSpriteMap; } }

    private void Start() {
        SetupUpgradeRoomMappings();
    }

    public void GameSetup() {
        currentRoomType = RoomType.Combat;
        currentRoomIndex = 0;
        bossRoomIndex = 10;

        ClearRoom();
        SetupDoors();
        EnemyManager.instance.CheckForRemainingEnemies();
    }

    public void ChangeRooms(RoomType newRoomType) {
        currentRoomType = newRoomType;
        currentRoomIndex++;
        GameManager.instance.player.transform.position = playerSpawnPosition.position;

        ClearRoom();
        SetupDoors();
        GameManager.instance.ClearProjectiles();

        // Spawn in units or pickups based on the new room type
        switch(currentRoomType) {
            case RoomType.Combat:
                EnemyManager.instance.SpawnRandomEnemy();
                break;
            case RoomType.Heal:
                // Spawn a health pickup if the new room is a healing room
                Instantiate(healthPickupPrefab, pickupSpawnPositions[1].position, Quaternion.identity, roomObjectsParent.transform);
                break;
            case RoomType.Upgrade:
                // Spawn 3 upgrade pickups if the new room is an upgrade room
                for(int i = 0; i < pickupSpawnPositions.Count; i++) {
                    GameObject upgradePickUp = Instantiate(upgradePickupPrefab, pickupSpawnPositions[i].position, Quaternion.identity, roomObjectsParent.transform);
                    upgradePickUp.GetComponent<UpgradePickup>().RandomizeStats((UpgradeTier)i);
                }
                break;
            case RoomType.Boss:
                EnemyManager.instance.SpawnBoss();
                break;
        }
    }

    private void ClearRoom() {
        EnemyManager.instance.ClearEnemies();
        // Destroy all objects in the room
        for(int i = roomObjectsParent.childCount - 1; i >= 0; i--) {
            Destroy(roomObjectsParent.GetChild(i).gameObject);
        }
    }

    private void SetupDoors() {
        // Setup Non-Boss Doors (Healing, Combat, and Upgrade Doors)
        if(currentRoomIndex == bossRoomIndex) {
            // For the boss room, hide all doors
            foreach(GameObject openDoorTilemap in nonBossDoorsOpenTilemaps) {
                openDoorTilemap.SetActive(false);
            }
            foreach(GameObject closedDoorTilemap in nonBossDoorsClosedTilemaps) {
                closedDoorTilemap.SetActive(false);
            }
        } else if(currentRoomIndex == bossRoomIndex - 1) {
            // For the room immediately before the boss room, hide all non-boss doors 
            foreach(GameObject openDoorTilemap in nonBossDoorsOpenTilemaps) {
                openDoorTilemap.SetActive(false);
            }
            foreach(GameObject closedDoorTilemap in nonBossDoorsClosedTilemaps) {
                closedDoorTilemap.SetActive(false);
            }
        } else {
            // For any room before the last room before the boss room, show all non-boss doors and lock them if the current room is a combat room
            foreach(GameObject openDoorTilemap in nonBossDoorsOpenTilemaps) {
                openDoorTilemap.SetActive(currentRoomType != RoomType.Combat);
            }
            foreach(GameObject closedDoorTilemap in nonBossDoorsClosedTilemaps) {
                closedDoorTilemap.SetActive(currentRoomType == RoomType.Combat);
            }
        }

        // Setup Boss Door
        if(currentRoomIndex == bossRoomIndex - 1) {
            // Show the boss door and lock it only if the room before the boss room is a combat room
            bossDoorOpenTilemap.SetActive(currentRoomType != RoomType.Combat);
            bossDoorClosedTilemap.SetActive(currentRoomType == RoomType.Combat);
        } else {
            // Otherwise hide the boss door
            bossDoorOpenTilemap.SetActive(false);
            bossDoorClosedTilemap.SetActive(false);
        }

        // Show the correct walls
        nonBossRoomWalls.SetActive(currentRoomIndex < bossRoomIndex);
        bossRoomWalls.SetActive(currentRoomIndex == bossRoomIndex);

        // Update Room Text
        UIManager.instance.UpdateBossCountdownText(currentRoomIndex, bossRoomIndex);
    }

    #region Combat Room Methods
    public void CombatRoomCleared() {
        // Exit early if the current room is not a combat room
        if(currentRoomType != RoomType.Combat) {
            return;
        }

        if(currentRoomIndex < bossRoomIndex - 1) {
            // Unlock all (normal) doors
            foreach(GameObject openDoorTilemap in nonBossDoorsOpenTilemaps) {
                openDoorTilemap.SetActive(true);
            }
            foreach(GameObject closedDoorTilemap in nonBossDoorsClosedTilemaps) {
                closedDoorTilemap.SetActive(false);
            }

        } else if(currentRoomIndex == bossRoomIndex - 1) {
            // Unlock the boss door
            bossDoorOpenTilemap.SetActive(true);
            bossDoorClosedTilemap.SetActive(false);
        }
    }

    public void SpawnSoul(Vector2 spawnPosition) {
        Instantiate(soulPickupPrefab, spawnPosition, Quaternion.identity, roomObjectsParent);
    }
    #endregion Combat Room Methods

    private void SetupUpgradeRoomMappings() {
        tierStatAmountMap = new Dictionary<UpgradeTier, (float, float)>();
        tierSoulCostMap = new Dictionary<UpgradeTier, int>();
        // Tier 1 (Weak): 20% buff, 20% nerf, costs 1 souls
        tierStatAmountMap.Add(UpgradeTier.Weak, (1.2f, 0.8f));
        tierSoulCostMap.Add(UpgradeTier.Weak, 1);
        // Tier 2 (Average): 40% buff, 20% nerf, costs 3 souls
        tierStatAmountMap.Add(UpgradeTier.Average, (1.4f, 0.8f));
        tierSoulCostMap.Add(UpgradeTier.Average, 3);
        // Tier 3 (Strong): 80% buff, 20% nerf, costs 5 souls
        tierStatAmountMap.Add(UpgradeTier.Strong, (1.8f, 0.8f));
        tierSoulCostMap.Add(UpgradeTier.Strong, 5);

        // Stat Sprite Map
        statSpriteMap = new Dictionary<Stat, Sprite>();
        statSpriteMap.Add(Stat.MaxHealth, healthSprite);
        statSpriteMap.Add(Stat.Damage, damageSprite);
        statSpriteMap.Add(Stat.MoveSpeed, moveSpeedSprite);
        statSpriteMap.Add(Stat.AttackTime, attackSpeedSprite);
        statSpriteMap.Add(Stat.ProjectileSpeed, projectileSpeedSprite);
    }
}
