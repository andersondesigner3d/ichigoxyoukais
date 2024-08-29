using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Rigidbody2D rb;
    public Rigidbody2D rbIchigo;
    public player playerScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbIchigo = GetComponentInParent<Rigidbody2D>();
        playerScript = GetComponentInParent<player>();
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Ground") && rbIchigo.velocity.y < 0){
            playerScript.playSoundLanding();
            playerScript.dustInGround();
        }
    }
}
