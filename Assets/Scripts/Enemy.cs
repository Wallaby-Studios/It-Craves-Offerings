using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    enum Behavior {
        Wander,
        Rush
    }

    //enemy's targeting child obj
    [SerializeField]
    private GameObject anchor;
    [SerializeField]
    private Behavior currentBehavior;
    [SerializeField]
    private float health;

    private Rigidbody2D rb;
    private Transform target;

    private float moveSpeed, xMov, yMov;

    //for dir changs on Wander state
    private float timeSinceLastChange, cooldown;

    public float Health { get { return health; } }

    // Start is called before the first frame update
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameManager.instance.player.transform;
        moveSpeed = Random.Range(2, 3);

        //PickRandomBehavior();
        currentBehavior = Behavior.Wander;

        PickNewDirection();

        cooldown = Random.Range(1.5f, 3f);
        timeSinceLastChange = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            PerformBehavior();
            LookAtTarget();
        }
    }

    public void Wander()
    {
        timeSinceLastChange += Time.deltaTime;

        Vector2 mov = new Vector2(xMov, yMov).normalized;
        rb.velocity = mov * moveSpeed;

        if (timeSinceLastChange >= cooldown)
        {
            PickNewDirection();
            timeSinceLastChange = 0;
            cooldown = Random.Range(1.5f, 3f);
        }
    }

    public void RushAtTarget()
    {
        //get dir to target
        Vector2 targetDir = (target.transform.position - transform.position).normalized;

        //rush directly at it
        rb.velocity = targetDir * moveSpeed;
    }

    public void PerformBehavior()
    {
        if (currentBehavior == Behavior.Wander)
        {
            Wander();
        }
        else if (currentBehavior == Behavior.Rush)
        {
            RushAtTarget();
        }
    }

    public void LookAtTarget()
    {
        //force aiming anchor to look at target transform
        anchor.transform.up = (target.transform.position - anchor.transform.position);
    }

    public void PickNewDirection()
    { 
        xMov = Random.Range(-1, 1);
        yMov = Random.Range(-1, 1);
    }

    public void PickRandomBehavior()
    {
        int r = Random.Range(-1, 3);

        if (r == 0) {
            currentBehavior = Behavior.Rush;
        } else {
            currentBehavior = Behavior.Wander;
        }
    }

    private void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0f) {
            // If the enemy has lost all health,
            // Destroy it, spawn souls where it died, and check if the room is cleared
            Destroy(gameObject);
            RoomManager.instance.SpawnSoul(transform.position);
            EnemyManager.instance.CheckForRemainingEnemies();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject != null) {
            if(collision.gameObject.GetComponent<Projectile>() == null) {
                Vector2 dirOfHit = collision.contacts[0].point - (Vector2)transform.position;
                xMov = -dirOfHit.x;
                yMov = -dirOfHit.y;
                cooldown = 3f;
                timeSinceLastChange = 0;
            } else {
                float damage = collision.gameObject.GetComponent<Projectile>().damage;
                TakeDamage(damage);
            }
        }
    }
}
