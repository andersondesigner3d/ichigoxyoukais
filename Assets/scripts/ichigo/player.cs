using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    float horizontalMoviment;
    private bool isFacingRight = true;

    public Transform frontCheck;
    public bool touchingWall;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool touchingGround;

    public float deadZone = 0.1f;

    public float jumpingPower;


    void Start()
    {
        
    }

    
    void Update()
    {       
        if (!isFacingRight && horizontalMoviment > deadZone){
            Flip();
        } else if(isFacingRight && horizontalMoviment < -deadZone){
            Flip();
        }        
    }

    private void FixedUpdate() {
        touchingGround = IsGrounded();
        touchingWall = canMove();

        if((isFacingRight && horizontalMoviment > 0 && touchingWall) || (!isFacingRight && horizontalMoviment < 0 && touchingWall)){
            return;
        }
        rb.velocity = new Vector2(horizontalMoviment * moveSpeed, rb.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context){
        if(context.performed && IsGrounded()){
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if(context.canceled && rb.velocity.y > 0f){
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private bool IsGrounded(){
        Vector2 boxSize1 = new Vector2(0.2f, 0.02f);
        return Physics2D.OverlapBox(groundCheck.position, boxSize1, 0f, groundLayer);
    }

    private bool canMove(){
        Vector2 boxSize2 = new Vector2(0.1f, 0.4f);
        return Physics2D.OverlapBox(frontCheck.position, boxSize2, 0f, groundLayer);
    }

    private void OnDrawGizmos() {
        //ground sensor
        Vector2 groundBoxSize = new Vector2(0.2f, 0.02f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);

        //front sensor
        Vector2 frontBoxSize = new Vector2(0.1f, 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(frontCheck.position, frontBoxSize);
    }

    public void Move(InputAction.CallbackContext context){
        horizontalMoviment = context.ReadValue<Vector2>().x;
    }

    private void Flip(){
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}