using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public BoxCollider2D boxCollider2D;

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();    
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            boxCollider2D.isTrigger = false;
        }    
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            boxCollider2D.isTrigger = true;
        }  
    }
}
