using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerAim playerAim;
    [SerializeField]
    private float moveSpeed, health, damage, attackTime, projectileSpeed;

    private Rigidbody2D rb;
    private PlayerControls controls;

    private Vector2 moveDirection, lookPosition;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        controls = GetComponent<PlayerControls>();
    }

    private void Update() {
        moveDirection = controls.MoveDirection;
        lookPosition = controls.LookPosition;

        playerAim.UpdateAim(lookPosition);
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed * Time.deltaTime, moveDirection.y * moveSpeed * Time.deltaTime);
    }
}
