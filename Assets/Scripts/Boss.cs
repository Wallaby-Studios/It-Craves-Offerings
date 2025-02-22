using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject bossProjectilePrefab;
    [SerializeField]
    private float attackTimer;
    [SerializeField]
    private int numberOfProjectilesPerAttack;

    private float attackTimerCurrent;

    // Start is called before the first frame update
    void Start()
    {
        attackTimerCurrent = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            attackTimerCurrent += Time.deltaTime;

            if(attackTimerCurrent > attackTimer) {

            }
        }
    }

    private void Fire() {
        
    }
}
