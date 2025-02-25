using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public GameObject impactParticle;
    public PlayerWeaponSFX audio;

    private void Awake() {
        audio = GameManager.instance.PlayerWeaponSFX;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider != null) {
            if(collision.gameObject.tag == "Player") {
                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
                audio.FleshHit();
            }
            
            if(gameObject.layer == 10) {
                // If the projectile is a BossProjectile,
                // destroy the boss projectile if it collides with
                // another collider that is NOT a basic projectile
                if(collision.gameObject.layer != 8) {
                    audio.WallHit();
                    ProjectileImpact(collision);
                    Destroy(gameObject);
                }
            } else {
                // If the projectile is a normal projectile
                audio.WallHit();
                ProjectileImpact(collision);
                Destroy(gameObject);
            }
        }
    }

    private void ProjectileImpact(Collision2D collision) {
        Vector2 position = collision.contacts[0].point;
        GameObject particle = Instantiate(impactParticle, position, Quaternion.identity, GameManager.instance.projectilesParent);
        Destroy(particle, 1);
    }
}
