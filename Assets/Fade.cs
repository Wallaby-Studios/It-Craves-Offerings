using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color color;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
        float r = color.r;
        float g = color.g;
        float b = color.b;
        float alpha = 255;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
