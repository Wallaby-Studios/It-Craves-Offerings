using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject spawner;
    public GameObject bulletPrefab;
    public float bulletForce;
    AudioSource audio;
    // Start is called before the first frame update
    private void Start()
    {
        audio = GetComponent<AudioSource>();    
    }
    public void PerformAttack()
    {
        audio.Play();
        GameObject g = Instantiate(bulletPrefab);
        Vector3 recoilOffset = new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f), 0);
        g.transform.position = spawner.transform.position + recoilOffset;
        g.transform.right = transform.up;
        g.GetComponent<Rigidbody2D>().AddForce(g.transform.right * bulletForce);
    }

    public IEnumerator StrafingAttack(int burstSize)
    {
        for (int i = 0; i < burstSize; i++)
        {
            PerformAttack();
            yield return new WaitForSeconds(0.05f);
        }
        
    }
}
