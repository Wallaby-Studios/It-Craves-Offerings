using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Stat {
    MoveSpeed,
    MaxHealth,
    Damage,
    AttackTime,
    ProjectileSpeed
}

public class Player : MonoBehaviour {
    [SerializeField]
    private PlayerAim playerAim;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite baseSprite, combatSprite, healthSprite, upgradeSprite;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float baseMoveSpeed, baseMaxHealth, baseDamage, baseAttackTime, baseProjectileSpeed;
    [SerializeField]
    private float currentHealth, currentFireTimer;

    private Dictionary<Stat, float> stats;
    private Rigidbody2D rb;
    private PlayerControls controls;
    private int soulsCount;
    private float tempSpriteTimerCurrent;

    private Vector2 moveDirection, lookPosition;
    AudioSource audio;
    public AudioClip fleshHit;
    public Dictionary<Stat, float> Stats { get { return stats; } }
    public float CurrentHealth { get { return currentHealth; } }
    public int SoulsCount { get { return soulsCount; } }
    public List<AudioClip> castSFX = new List<AudioClip>();
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
    }

    private void Update() {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            moveDirection = controls.MoveDirection;
            lookPosition = controls.LookPosition;
            
            // Convert the look position from screen space to world space using the camera 
            Vector3 worldLookPosition = Camera.main.ScreenToWorldPoint(lookPosition);
            // Flip the sprite if the look position is less than the player's X position
            spriteRenderer.flipX = worldLookPosition.x < transform.position.x;

            playerAim.UpdateAim(worldLookPosition);
        }
    }

    private void FixedUpdate() {
        if(GameManager.instance.CurrentGameState == GameState.Game) {
            rb.velocity = new Vector2(
                moveDirection.x * stats[Stat.MoveSpeed] * Time.deltaTime,
                moveDirection.y * stats[Stat.MoveSpeed] * Time.deltaTime);

            currentFireTimer += Time.deltaTime;

            if(tempSpriteTimerCurrent > 0f) {
                tempSpriteTimerCurrent -= Time.deltaTime;
            } else if(tempSpriteTimerCurrent <= 0f) {
                if(spriteRenderer.sprite != baseSprite) {
                    spriteRenderer.sprite = baseSprite;
                }
            }
        }
    }

    public void SetStats() {
        stats = new Dictionary<Stat, float>();
        stats.Add(Stat.MaxHealth, baseMaxHealth);
        stats.Add(Stat.MoveSpeed, baseMoveSpeed);
        stats.Add(Stat.Damage, baseDamage);
        stats.Add(Stat.AttackTime, baseAttackTime);
        stats.Add(Stat.ProjectileSpeed, baseProjectileSpeed);

        currentHealth = stats[Stat.MaxHealth];
        currentFireTimer = 0f;
        soulsCount = 0;

        controls = GetComponent<PlayerControls>();
    }

    public void Fire() {
        if(GameManager.instance.CurrentGameState == GameState.Game
            && currentFireTimer >= stats[Stat.AttackTime]) {
            // Reset timer
            currentFireTimer = 0f;

            Vector3 projectilePosition = transform.position + playerAim.Direction / 2f;
            GameObject projectile = Instantiate(projectilePrefab, projectilePosition, Quaternion.identity, GameManager.instance.projectilesParent);
            // Give the projectile damage
            projectile.GetComponent<Projectile>().damage = stats[Stat.Damage];
            // Apply a force to the projectile (in the direction it was fired)
            Vector2 projectileForce = playerAim.Direction;
            projectileForce.Normalize();
            projectileForce *= stats[Stat.ProjectileSpeed];
            projectile.GetComponent<Rigidbody2D>().AddForce(projectileForce);
            projectile.transform.right = playerAim.transform.up;
            int choice = Random.Range(0, castSFX.Count - 1);
            audio.clip = castSFX[choice];
            audio.Play();
        }
    }

    public void TakeDamage(float amount) {
        if(amount < 0) {
            return;
        }
        audio.clip = fleshHit;
        audio.Play();
        currentHealth -= amount;

        UIManager.instance.UpdateStats();

        if(currentHealth < 0) {
            GameManager.instance.ChangeGameState(GameState.GameEnd);
            Destroy(gameObject);
        }
    }

    public void Heal() {
        currentHealth = stats[Stat.MaxHealth];
        TemporarilySwapSprite(healthSprite, 2.0f);
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

        TemporarilySwapSprite(upgradeSprite, 2.0f);
        UIManager.instance.UpdateStats();
    }

    public void GiveSouls(int souls) {
        soulsCount += souls;
        TemporarilySwapSprite(combatSprite, 2.0f);
        UIManager.instance.UpdateStats();
    }

    public bool SpendSouls(int souls) {
        if(soulsCount >= souls) {
            soulsCount -= souls;
            UIManager.instance.UpdateStats();
            return true;
        }

        return false;
    }

    private void TemporarilySwapSprite(Sprite tempSprite, float duration) {
        tempSpriteTimerCurrent = duration;
        spriteRenderer.sprite = tempSprite;
    }
}
