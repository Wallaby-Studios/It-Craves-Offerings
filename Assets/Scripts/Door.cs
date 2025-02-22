using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private RoomType roomType;
    [SerializeField]
    private bool unlocked;

    public RoomType RoomType { get { return roomType; } }

    public bool Unlocked { get { return unlocked; } }

    // Start is called before the first frame update
    void Start()
    {
        unlocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // If the player collides with the unlocked door, advance the room
        if(collision.collider != null
            && collision.gameObject.tag == "Player"
            && unlocked) {
                RoomManager.instance.ChangeRooms(roomType);
        }
    }
}
