using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowawaySound : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> clips = new List<AudioClip>();   
    // Start is called before the first frame update

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        int i = Random.Range(0, clips.Count);
        audioSource.clip = clips[i];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 10);
    }
}
