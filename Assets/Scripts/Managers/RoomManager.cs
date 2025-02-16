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

    [SerializeField]
    private RoomType currentRoomType;
    [SerializeField]
    private int currentRoomCount;
    [SerializeField]
    private Transform playerSpawnPosition;
    // Doors
    [SerializeField]
    private List<GameObject> nonBossDoors;
    [SerializeField]
    private GameObject bossDoor;
    // Room Objects
    [SerializeField]
    private GameObject healthPickupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentRoomType = RoomType.Combat;
        currentRoomCount = 0;

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

        SetupDoors();

        if(currentRoomType == RoomType.Heal) {
            Instantiate(healthPickupPrefab);
        }
    }

    private void SetupDoors() {
        if(currentRoomCount < 9) {
            // For any room before the 8th room, show all doors and lock them if the current room is a combat room
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(true);
                //door.GetComponent<Door>().Unlocked = currentRoomType != RoomType.Combat;
            }
            bossDoor.SetActive(false);
        } else if(currentRoomCount == 9) {
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
