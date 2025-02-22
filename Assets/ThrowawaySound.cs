using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowawaySound : MonoBehaviour
{
    AudioSource audio;
    public List<AudioClip> clips = new List<AudioClip>();   
    // Start is called before the first frame update

    private void Start()
    {        
            audio = GetComponent<AudioSource>();
            int i = Random.Range(0, clips.Count);
            audio.clip = clips[i];
            audio.Play();
        

    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 3);
    }
}
