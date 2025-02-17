using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Combat,
    Heal,
    Upgrade,
    Boss
}

public class RoomManager : MonoBehaviour
{
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
    private List<GameObject> nonBossDoors;
    [SerializeField]
    private GameObject bossDoor;
    // Room Objects
    [SerializeField]
    private Transform roomObjectsParent;
    [SerializeField]
    private GameObject healthPickupPrefab, upgradePickupPrefab;

    private RoomType currentRoomType;
    private int currentRoomCount;
    private int roomsBeforeBossRoom;

    public int RoomsBeforeBossRoom { get { return roomsBeforeBossRoom; } }

    // Start is called before the first frame update
    void Start()
    {
        currentRoomType = RoomType.Combat;
        currentRoomCount = 0;
        roomsBeforeBossRoom = 10;

        SetupDoors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeRooms(RoomType newRoomType) {
        currentRoomType = newRoomType;
        currentRoomCount++;
        GameManager.instance.player.transform.position = playerSpawnPosition.position;

        // Destroy all objects in the room
        for(int i = roomObjectsParent.childCount - 1; i >= 0; i--) {
            Destroy(roomObjectsParent.GetChild(i).gameObject);
        }

        SetupDoors();
        UIManager.instance.UpdateBossCountdownText(roomsBeforeBossRoom - currentRoomCount);

        switch(currentRoomType) {
            case RoomType.Combat:
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
                break;
        }
    }

    private void SetupDoors() {
        if(currentRoomCount < roomsBeforeBossRoom - 1) {
            // For any room before the 8th room, show all doors and lock them if the current room is a combat room
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(true);
                //door.GetComponent<Door>().Unlocked = currentRoomType != RoomType.Combat;
            }
            bossDoor.SetActive(false);
        } else if(currentRoomCount == roomsBeforeBossRoom - 1) {
            // For the 9th room, hide all non-boss doors 
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(false);
            }
            // Show the boss door and lock it only if the 9th room is a combat room
            bossDoor.SetActive(true);
            //bossDoor.GetComponent<Door>().Unlocked = currentRoomType != RoomType.Combat;
        } else {
            // For the 10th room, hide all doors
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(false);
            }
            bossDoor.SetActive(false);
        }
    }
}
