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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.collider != null) {
            Debug.Log(collision.gameObject.name);

            if(collision.gameObject.tag == "Player") {
                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
                audio.FleshHit();
            }
            audio.WallHit();
            GameObject p = Instantiate(impactParticle);
            p.transform.position = collision.contacts[0].point;
            
            Destroy(p, 1);
            // If the projectile is a BossProjectile
            if(gameObject.layer == 10) {
                if(collision.gameObject.layer != 8) {
                    Destroy(gameObject);
                }
            } else {
                Destroy(gameObject);
            }
        }
    }
}
