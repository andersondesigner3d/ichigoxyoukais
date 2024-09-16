using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveGamePoint : MonoBehaviour
{
    public GameController gameController;
    public int pointNumber;

    void Start()
    {
        gameController = GameObject.Find("GameController")?.GetComponent<GameController>();
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            PlayerPrefs.SetInt("pointNumber", pointNumber);
            PlayerPrefs.Save();
            Destroy(this.gameObject);
        }
    }
}