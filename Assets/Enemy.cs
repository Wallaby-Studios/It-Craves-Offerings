using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;

    public float moveSpeed = 3;
    public Transform target;
    public GameObject anchor;

    enum Behavior
    {
        Wander,
        Rush
    }

    [SerializeField]
    Behavior currentBehavior;

    float xMov;
    float yMov;

    float timeSinceLastChange = 0;
    float cooldown = 1;
    // Start is called before the first frame update
    void Start()
    {

        PickRandomBehavior();

        yMov = 1;
        xMov = 1;

        cooldown = Random.Range(0.75f, 1.5f);

        timeSinceLastChange = cooldown;

        rb = GetComponent<Rigidbody2D>();
        //pick new direction to move in randomly
        //PickNewDirection();
    }

    // Update is called once per frame
    void Update()
    {
        PerformBehavior();
        LookAtTarget();
    }

    public void Wander()
    {
        Vector2 mov = new Vector2(xMov, yMov).normalized;
        rb.velocity = mov * moveSpeed;

        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= cooldown)
        {
            PickNewDirection();
            timeSinceLastChange = 0;
        }
    }

    public void RushAtTarget()
    {
        Vector2 targetDir = (target.transform.position - transform.position).normalized;
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
}
