using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    enum Behavior {
        Wander,
        Rush,
        Dash
    }

    //enemy's targeting child obj
    [SerializeField]
    private GameObject anchor, deathPrefab;
    [SerializeField]
    private Behavior currentBehavior;
    [SerializeField]
    private float health;

    private Rigidbody2D rb;
    private Transform target;

    private float moveSpeed, xMov, yMov;

    //for dir changs on Wander state
    private float timeSinceLastChange, cooldown;

    //Attack Variables
    [Header("Attack Fields")]
    public float strafingAttackCooldown = 2f;
    float timeSinceLastStrafingAttack = 0;
    EnemyShooting eShooting;
    public float stationaryTimeBetweenShots = 0.2f;
    float timeSinceLastStationaryShot = 0;

    public GameObject deathSound;
    public GameObject gruntSound;
    public float Health { get { return health; } }

    // Start is called before the first frame update
    void Start() 
    {
        health = RoomManager.instance.CurrentRoomCount * 2;
        rb = GetComponent<Rigidbody2D>();
        target = GameManager.instance.player.transform;
        moveSpeed = Random.Range(1, 2);

        //PickRandomBehavior();
        currentBehavior = Behavior.Wander;

        PickNewDirection();

        cooldown = Random.Range(1.5f, 3f);
        timeSinceLastChange = cooldown;
        eShooting = GetComponentInChildren<EnemyShooting>();
        timeSinceLastStrafingAttack = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            PerformBehavior();
            LookAtTarget();
            HandleStrafe();

            if (xMov == 0 && yMov == 0)
            {
                timeSinceLastStationaryShot += Time.deltaTime;
                if (timeSinceLastStationaryShot > stationaryTimeBetweenShots)
                {
                    eShooting.PerformAttack();
                    timeSinceLastStationaryShot = 0;
                }
            }
        }
    }

    public void HandleStrafe()
    {
        timeSinceLastStrafingAttack += Time.deltaTime;
        if (timeSinceLastStrafingAttack > strafingAttackCooldown)
        {
            if (!(xMov ==0 && yMov == 0) && timeSinceLastChange > 0.5f)
            {
                int burstSize = Random.Range(3, 5);
                //timeSinceLastChange = 0;
                StartCoroutine(eShooting.StrafingAttack(burstSize));
                timeSinceLastStrafingAttack = 0;
                strafingAttackCooldown = Random.Range(1.5f, 3);
            }
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
            cooldown = Random.Range(1.5f, 2f);
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
        GameObject g = Instantiate(gruntSound);
        
        if (health <= 0f) {
            // If the enemy has lost all health,
            // Destroy it, spawn souls where it died, and check if the room is cleared
            GameObject s = Instantiate(deathSound);
            Instantiate(deathPrefab, gameObject.transform.position, Quaternion.identity, GameManager.instance.projectilesParent);
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
