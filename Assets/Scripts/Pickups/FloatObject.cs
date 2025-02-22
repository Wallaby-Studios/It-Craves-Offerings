using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObject : MonoBehaviour
{
    [SerializeField]
    private bool randomStart, isRotating;

    private float speed, height, randomStartOffset;
    private Vector3 pos;
    float rotZ = 0;
    private void Start() {
        speed = 3f;
        height = 0.1f;
        randomStartOffset = Random.Range(0f, 1f);

        pos = transform.position;
    }

    void Update() {
        // Calculate the new Y position based on the current time
        float newY;
        if(randomStart) {
            newY = Mathf.Sin(Time.time * speed + randomStartOffset) * height + pos.y;
        } else {
            newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        }

        // Set the object's new Y position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if(isRotating) {
            rotZ += 50f * Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotZ));
        }
    }
}
