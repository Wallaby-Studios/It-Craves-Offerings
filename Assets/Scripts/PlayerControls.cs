using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public PlayerInputControls playerInputControls;

    private InputAction move, look, fire;

    public Vector2 MoveDirection { get { return move.ReadValue<Vector2>(); } }
    public Vector2 LookPosition { get { return look.ReadValue<Vector2>(); } }

    private void Awake() {
        playerInputControls = new PlayerInputControls();
    }

    private void OnEnable() {
        move = playerInputControls.Player.Move;
        move.Enable();
        look = playerInputControls.Player.Look;
        look.Enable();
        fire = playerInputControls.Player.Move;
        fire.Enable();
    }

    private void OnDisable() {
        move.Disable();
        look.Disable();
        fire.Disable();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }
}
