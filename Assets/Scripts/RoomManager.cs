using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Combat,
    Healing,
    Offering,
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
    Transform playerSpawnPosition;
    [SerializeField]
    List<GameObject> nonBossDoors;
    [SerializeField]
    GameObject bossDoor;

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
    }

    private void SetupDoors() {
        if(currentRoomCount < 9) {
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(true);
            }
            bossDoor.SetActive(false);
        } else if(currentRoomCount == 9) {
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(false);
            }
            bossDoor.SetActive(true);
        } else {
            foreach(GameObject door in nonBossDoors) {
                door.SetActive(false);
            }
            bossDoor.SetActive(false);
        }
    }
}
