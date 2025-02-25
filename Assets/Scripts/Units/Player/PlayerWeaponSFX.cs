using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSFX : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> sounds = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WallHit()
    {
        audioSource.clip = sounds[0];
        audioSource.Play();
    }

    public void FleshHit()
    {
        audioSource.clip = sounds[1];
        audioSource.Play();
    }

}
