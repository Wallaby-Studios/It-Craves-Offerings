using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;

    //inspector variables
    public float moveSpeed = 3;
    //enemy points at this transform each frame
    public Transform target;
    //enemy's targeting child obj
    public GameObject anchor;

    enum Behavior
    {
        Wander,
        Rush
    }

    [SerializeField]
    Behavior currentBehavior;

    [SerializeField]
    private float health;

    //determines Wander movement dir
    float xMov;
    float yMov;

    //for dir changs on Wander state
    float timeSinceLastChange = 0;
    float cooldown = 1;

    public float Health { get { return health; } }

    // Start is called before the first frame update
    void Start()
    {
        target = GameManager.instance.player.transform;
        moveSpeed = Random.Range(2, 3);
        //PickRandomBehavior();
        currentBehavior = Behavior.Wander;

        PickNewDirection();

        cooldown = Random.Range(1.5f, 3f);

        timeSinceLastChange = cooldown;

        rb = GetComponent<Rigidbody2D>();
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

        if (r == 0)
        {
            currentBehavior = Behavior.Rush;
        }
        else
        {
            currentBehavior = Behavior.Wander;
        }
    }

    private void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0f) {
            Destroy(gameObject);
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
