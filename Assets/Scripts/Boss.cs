using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject bossProjectilePrefab;
    [SerializeField]
    private float health, damage, moveSpeed, attackTimer, projectileSpeed, attackMaxAngle;
    [SerializeField]
    private int numberOfProjectilesPerAttack;

    private Rigidbody2D rb;
    private float attackTimerCurrent;
    private float xMov, yMov, cooldown, timeSinceLastChange, wanderDuration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimerCurrent = 0f;
        cooldown = Random.Range(3f, 5f);
        timeSinceLastChange = cooldown;
        wanderDuration = Random.Range(1.5f, 3f);

        PickNewDirection();

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
        wanderDuration -= Time.deltaTime;

        if(wanderDuration > 0f) {
            Vector2 mov = new Vector2(xMov, yMov).normalized;
            rb.velocity = mov * moveSpeed;
        }

        if(timeSinceLastChange >= cooldown) {
            PickNewDirection();
            timeSinceLastChange = 0;
            cooldown = Random.Range(3f, 5f);
            wanderDuration = Random.Range(1.5f, 3f);
        }
    }

    public void PickNewDirection() {
        xMov = Random.Range(-1, 1);
        yMov = Random.Range(-1, 1);

        spriteRenderer.flipX = xMov < 0f;
    }

    private void Fire() {
        Vector2 directionToPlayer = GameManager.instance.player.gameObject.transform.position - transform.position;
        CreateProjectile(directionToPlayer);

        float angle = 30f;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 newDirection = rotation * directionToPlayer;
        CreateProjectile(newDirection);

        Quaternion rotationNeg = Quaternion.AngleAxis(-angle, Vector3.forward);
        Vector3 newDirectionNeg = rotationNeg * directionToPlayer;
        CreateProjectile(newDirectionNeg);
    }

    private void CreateProjectile(Vector2 direction) {
        direction.Normalize();
        Vector2 position = (Vector2)transform.position + direction;
        GameObject projectile = Instantiate(bossProjectilePrefab, position, Quaternion.identity, GameManager.instance.projectilesParent);
        projectile.GetComponent<Projectile>().damage = damage;
        Vector2 projectileForce = direction * projectileSpeed;
        projectile.GetComponent<Rigidbody2D>().velocity = projectileForce;
        projectile.transform.right = direction;
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
