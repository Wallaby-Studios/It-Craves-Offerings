using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject bossProjectilePrefab;
    [SerializeField]
    private float health, attackTimer, projectileSpeed, attackMaxAngle;
    [SerializeField]
    private int numberOfProjectilesPerAttack;

    private float attackTimerCurrent;

    // Start is called before the first frame update
    void Start()
    {
        attackTimerCurrent = 0f;
        if(numberOfProjectilesPerAttack % 2 == 0) {
            numberOfProjectilesPerAttack++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            attackTimerCurrent += Time.deltaTime;

            if(attackTimerCurrent > attackTimer) {
                Fire();
                attackTimerCurrent = 0f;
            }
        }
    }

    private void Fire() {
        Vector2 directionToPlayer = GameManager.instance.player.gameObject.transform.position - transform.position;
        directionToPlayer.Normalize();
        // Middle Projectile - "3"
        Vector2 position = (Vector2)transform.position + directionToPlayer;
        GameObject projectileMiddle = Instantiate(bossProjectilePrefab, position, Quaternion.identity, GameManager.instance.projectilesParent);
        Vector2 projectileForce = directionToPlayer * projectileSpeed;
        projectileMiddle.GetComponent<Rigidbody2D>().AddForce(projectileForce);
        projectileMiddle.transform.right = directionToPlayer;
    }

    private void TakeDamage(float damage) {
        health -= damage;

        if(health <= 0f) {
            // If the boss is killed, move the game to the GameEnd State
            Destroy(gameObject);
            GameManager.instance.ChangeGameState(GameState.GameEnd);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject != null) {
            if(collision.gameObject.GetComponent<Projectile>() == null) {
                //Vector2 dirOfHit = collision.contacts[0].point - (Vector2)transform.position;
                //xMov = -dirOfHit.x;
                //yMov = -dirOfHit.y;
                //cooldown = 3f;
                //timeSinceLastChange = 0;
            } else {
                float damage = collision.gameObject.GetComponent<Projectile>().damage;
                TakeDamage(damage);
            }
        }
    }
}
