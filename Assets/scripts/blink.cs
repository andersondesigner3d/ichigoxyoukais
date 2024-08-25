using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class blink : MonoBehaviour
{
    public TilemapRenderer tilemapRenderer;
    public float blinkInterval = 0.5f;

    void Start()
    {
        StartCoroutine(Blinking());
    }

    IEnumerator Blinking()
    {
        while (true)
        {
            tilemapRenderer.enabled = !tilemapRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
