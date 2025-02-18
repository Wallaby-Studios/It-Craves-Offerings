using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat {
    MoveSpeed,
    MaxHealth,
    Damage,
    AttackTime,
    ProjectileSpeed
}

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerAim playerAim;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float baseMoveSpeed, baseMaxHealth, baseDamage, baseAttackTime, baseProjectileSpeed;
    [SerializeField]
    private float currentHealth, currentFireTimer;

    private Dictionary<Stat, float> stats;
    private Rigidbody2D rb;
    private PlayerControls controls;

    private Vector2 moveDirection, lookPosition;

    public Dictionary<Stat, float> Stats { get { return stats; } }
    public float CurrentHealth { get { return currentHealth; } }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        stats = new Dictionary<Stat, float>();
        stats.Add(Stat.MaxHealth, baseMaxHealth);
        stats.Add(Stat.MoveSpeed, baseMoveSpeed);
        stats.Add(Stat.Damage, baseDamage);
        stats.Add(Stat.AttackTime, baseAttackTime);
        stats.Add(Stat.ProjectileSpeed, baseProjectileSpeed);

        currentHealth = stats[Stat.MaxHealth];
        currentFireTimer = 0f;

        controls = GetComponent<PlayerControls>();
    }

    private void Update() {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            moveDirection = controls.MoveDirection;
            lookPosition = controls.LookPosition;

            playerAim.UpdateAim(lookPosition);
        }
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(
            moveDirection.x * stats[Stat.MoveSpeed] * Time.deltaTime, 
            moveDirection.y * stats[Stat.MoveSpeed] * Time.deltaTime);

        currentFireTimer += Time.deltaTime;
    }

    public void Fire() {
        if(currentFireTimer >= stats[Stat.AttackTime]) {
            // Reset timer
            currentFireTimer = 0f;

            Vector3 projectilePosition = transform.position + playerAim.Direction;
            GameObject projectile = Instantiate(projectilePrefab, projectilePosition, Quaternion.identity, GameManager.instance.projectilesParent);
            // Give the projectile damage
            projectile.GetComponent<Projectile>().damage = stats[Stat.Damage];
            // Apply a force to the projectile (in the direction it was fired)
            Vector2 projectileForce = playerAim.Direction;
            projectileForce.Normalize();
            projectileForce *= stats[Stat.ProjectileSpeed];
            projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce);
        }
    }

    public void TakeDamage(float amount) {
        if(amount < 0) {
            return;
        }

        currentHealth -= amount;

        UIManager.instance.UpdateStats();

        if(currentHealth < 0) {
            GameManager.instance.ChangeGameState(GameState.GameEnd);
            Destroy(gameObject);
        }
    }

    public void Heal() {
        currentHealth = baseMaxHealth;
        UIManager.instance.UpdateStats();
    }

    public void Buff(Stat stat, float statChangeAmount) {
        switch(stat) {
            case Stat.MoveSpeed:
                stats[Stat.MoveSpeed] *= statChangeAmount;
                break;
            case Stat.MaxHealth:
                stats[Stat.MaxHealth] *= statChangeAmount;
                currentHealth *= statChangeAmount;
                break;
            case Stat.Damage:
                stats[Stat.Damage] *= statChangeAmount;
                break;
            case Stat.AttackTime:
                // Invert the value since a shorter attack time is better
                stats[Stat.AttackTime] *= (1f / statChangeAmount);
                break;
            case Stat.ProjectileSpeed:
                stats[Stat.ProjectileSpeed] *= statChangeAmount;
                break;
        }

        UIManager.instance.UpdateStats();
    }
}
