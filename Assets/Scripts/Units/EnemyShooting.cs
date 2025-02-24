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
    public List<AudioClip> firingSounds = new List<AudioClip>();
    // Start is called before the first frame update
    private void Start()
    {
        audio = GetComponent<AudioSource>();    
    }
    public void PerformAttack()
    {
        int i = Random.Range(0, firingSounds.Count - 1);
        audio.clip = firingSounds[i];
        audio.Play();
        Vector3 recoilOffset = new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f), 0);
        Vector2 position = spawner.transform.position + recoilOffset;
        GameObject g = Instantiate(bulletPrefab, position, Quaternion.identity, GameManager.instance.projectilesParent);
        g.transform.right = transform.up;
        g.GetComponent<Rigidbody2D>().AddForce(g.transform.right * bulletForce);
    }

    public IEnumerator StrafingAttack(int burstSize)
    {
        for (int i = 0; i < burstSize; i++)
        {
            PerformAttack();
            yield return new WaitForSeconds(0.15f);
        }
        
    }
}
