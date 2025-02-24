using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSFX : MonoBehaviour
{
    AudioSource audio;
    public List<AudioClip> sounds = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WallHit()
    {
        audio.clip = sounds[0];
        audio.Play();
    }

    public void FleshHit()
    {
        audio.clip = sounds[1];
        audio.Play();
    }

}
