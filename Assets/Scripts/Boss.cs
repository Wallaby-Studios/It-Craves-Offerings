using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject bossProjectilePrefab;
    [SerializeField]
    private float health, damage, moveSpeed, attackTimer, projectileSpeed, attackMaxAngle;
    [SerializeField]
    private int numberOfProjectilesPerAttack;

    private Rigidbody2D rb;
    private float attackTimerCurrent;
    private float xMov, yMov, cooldown, timeSinceLastChange;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimerCurrent = 0f;
        cooldown = Random.Range(3f, 5f);
        timeSinceLastChange = cooldown;

        if(numberOfProjectilesPerAttack % 2 == 0) {
            numberOfProjectilesPerAttack++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            Wander();
        }
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

    public void Wander() {
        timeSinceLastChange += Time.deltaTime;

        Vector2 mov = new Vector2(xMov, yMov).normalized;
        rb.velocity = mov * moveSpeed;

        if(timeSinceLastChange >= cooldown) {
            PickNewDirection();
            timeSinceLastChange = 0;
            cooldown = Random.Range(3f, 5f);
        }
    }

    public void PickNewDirection() {
        xMov = Random.Range(-1, 1);
        yMov = Random.Range(-1, 1);
    }

    private void Fire() {
        Vector2 directionToPlayer = GameManager.instance.player.gameObject.transform.position - transform.position;
        directionToPlayer.Normalize();

        Vector2 position = (Vector2)transform.position + directionToPlayer;
        GameObject projectile = Instantiate(bossProjectilePrefab, position, Quaternion.identity, GameManager.instance.projectilesParent);
        projectile.GetComponent<Projectile>().damage = damage;
        Vector2 projectileForce = directionToPlayer * projectileSpeed;
        projectile.GetComponent<Rigidbody2D>().velocity = projectileForce;
        projectile.transform.right = directionToPlayer;
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
                Vector2 dirOfHit = collision.contacts[0].point - (Vector2)transform.position;
                xMov = -dirOfHit.x;
                yMov = -dirOfHit.y;
            } else {
                float damage = collision.gameObject.GetComponent<Projectile>().damage;
                TakeDamage(damage);
            }
        }
    }
}
