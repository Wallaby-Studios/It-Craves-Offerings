using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public GameObject impactParticle;

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
            if(collision.gameObject.tag == "Player") {
                collision.gameObject.GetComponent<Player>().TakeDamage(damage);
                
            }
            GameObject p = Instantiate(impactParticle);
            p.transform.position = collision.contacts[0].point;
            Destroy(p, 1);
            if(gameObject.layer != 10 && collision.gameObject.layer != 8) {
                Destroy(gameObject);
            }
        }
    }
}
