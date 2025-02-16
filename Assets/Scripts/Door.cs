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

    public bool Unlocked { 
        get { return unlocked; } 
        set { unlocked = value; } 
    }

    // Start is called before the first frame update
    void Start()
    {
        unlocked = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider != null
            && collision.gameObject.tag == "Player"
            && unlocked) {
                RoomManager.instance.ChangeRooms(roomType);
        }
    }
}
