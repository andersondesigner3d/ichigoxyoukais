using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{   
    [Header ("Principal")]
    public Animator anim;
    public Transform ichigoTransform;
    [Header ("Moviment")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    float horizontalMoviment;
    float verticalMoviment;
    private bool isFacingRight = true;
    public bool moving;
    [Header ("Sensors")]
    public Transform frontCheck;
    public bool touchingWall;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool touchingGround;
    [Header ("Jump")]
    public bool jumpping = false;
    public float deadZone = 0.1f;
    public float jumpingPower;
    public bool velocidadeY;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ichigoTransform = GetComponent<Transform>();
    }
    
    void Update()
    {        
        //print("horizontalMoviment: "+horizontalMoviment);
        //print("rb.velocity.x = "+rb.velocity.x);
        //print("verticalMoviment = "+verticalMoviment);

        touchingGround = IsGrounded();
        touchingWall = canMove();
        verificaVelocidadeY();
        if(IsGrounded()){
            jumpping = false;
        }

        //flip
        if (!isFacingRight && horizontalMoviment > deadZone){
            Flip();
        } else if(isFacingRight && horizontalMoviment < -deadZone){
            Flip();
        }        
        

        //moviment
        if(!touchingWall){
            rb.velocity = new Vector2(horizontalMoviment * moveSpeed, rb.velocity.y);
            
        }        
        
    }

    private void FixedUpdate() {

        if(touchingGround){            
            if(horizontalMoviment == 0 || touchingWall){
                anim.SetBool("parado",true);
                anim.SetBool("correndo",false);
                anim.SetBool("pulando-subindo",false);
                anim.SetBool("pulando-caindo",false);
                anim.SetBool("sofrendo-dano",false);
                anim.SetBool("morto",false);
                anim.SetBool("ataque1",false);
                anim.SetBool("especial",false);
            } else if(horizontalMoviment != 0 && !touchingWall && rb.velocity.x != 0) {
                anim.SetBool("parado",false);
                anim.SetBool("correndo",true);
                anim.SetBool("pulando-subindo",false);
                anim.SetBool("pulando-caindo",false);
                anim.SetBool("sofrendo-dano",false);
                anim.SetBool("morto",false);
                anim.SetBool("ataque1",false);
                anim.SetBool("especial",false);
            }
        } else {
            if(rb.velocity.y >= 0){
                anim.SetBool("parado",false);
                anim.SetBool("correndo",false);
                anim.SetBool("pulando-subindo",true);
                anim.SetBool("pulando-caindo",false);
                anim.SetBool("sofrendo-dano",false);
                anim.SetBool("morto",false);
                anim.SetBool("ataque1",false);
                anim.SetBool("especial",false);
            } else {
                anim.SetBool("parado",false);
                anim.SetBool("correndo",false);
                anim.SetBool("pulando-subindo",false);
                anim.SetBool("pulando-caindo",true);
                anim.SetBool("sofrendo-dano",false);
                anim.SetBool("morto",false);
                anim.SetBool("ataque1",false);
                anim.SetBool("especial",false);
            }
        }
    }

    public void Move(InputAction.CallbackContext context){
        
        horizontalMoviment = Mathf.RoundToInt(context.ReadValue<Vector2>().x);
        verticalMoviment = Mathf.RoundToInt(context.ReadValue<Vector2>().y);

        if(context.canceled){
            StartCoroutine(delayControl());
        }

    }

    IEnumerator delayControl(){
        yield return new WaitForSeconds(0.05f);        
        if(horizontalMoviment == 0){
             print("cria fumacinha");
        }       
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

    private void Flip(){
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Sword(InputAction.CallbackContext context){
        
    }

    void verificaVelocidadeY(){
        if(rb.velocity.y != 0){
            velocidadeY = true;
        } else {
            velocidadeY = false;
        }
    }
}