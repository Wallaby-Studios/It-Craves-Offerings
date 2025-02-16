using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Vector3 direction;

    public Vector3 Direction { get { return direction; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAim(Vector3 lookPosition) {
        // Convert the passed in position from screen space to world space using the camera 
        Vector3 worldLookPosition = Camera.main.ScreenToWorldPoint(lookPosition);
        // Zero the z coordinate
        worldLookPosition.z = 0f;
        
        // Find the direction between the look position and player 
        Vector3 newDirection = worldLookPosition - transform.position;
        newDirection.Normalize();

        // Rotate the object by setting its up vector 
        transform.up = newDirection;
        direction = newDirection;
    }
}
