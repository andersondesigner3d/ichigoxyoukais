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
    public bool jumpping = true;
    public float deadZone = 0.1f;
    public float jumpingPower;
    [Header ("Atacks")]
    public bool attacking;
    public bool attacking_air;
    [Header ("Visual FX")]
    public bool canDust = true;
    public GameObject attack1_fx;
    public GameObject attack_air_fx;
    public GameObject dust_jump_fx;
    [Header ("Audio")]
    public AudioSource audioSource;
    public AudioClip[] audioClip;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ichigoTransform = GetComponent<Transform>();
    }
    
    void Update()
    {        
        //print("horizontalMoviment: "+horizontalMoviment);
        //print("rb.velocity.y = "+rb.velocity.y);
        //print("verticalMoviment = "+verticalMoviment);

        touchingGround = IsGrounded();
        touchingWall = canMove();

        //flip
        if (!isFacingRight && horizontalMoviment > deadZone){
            Flip();
        } else if(isFacingRight && horizontalMoviment < -deadZone){
            Flip();
        }        
        
        //movement
        if(attacking){
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else {
            if(!touchingWall && !attacking){
                rb.velocity = new Vector2(horizontalMoviment * moveSpeed, rb.velocity.y);
            }
        }

        fixAnimationBugs();
        
    }

    private void FixedUpdate() {

        animationControll();       
        
    }

    private void animationControll(){
        if(!attacking){
            if(touchingGround){            
                if(horizontalMoviment == 0 || touchingWall){
                    anim.SetBool("parado",true);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                    anim.SetBool("morto",false);
                } else if(horizontalMoviment != 0 && !touchingWall && rb.velocity.x != 0) {
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",true);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                    anim.SetBool("morto",false);
                }
            } else {
                if(rb.velocity.y >= 0){
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",true);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                    anim.SetBool("morto",false);
                } else {
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",true);
                    anim.SetBool("sofrendo-dano",false);
                    anim.SetBool("morto",false);
                }
            }
        }
    }

    private void fixAnimationBugs(){
        //fiz animations bugs
        if(touchingGround){
            anim.SetBool("pulando-subindo",false);
            anim.SetBool("pulando-caindo",false);
            anim.ResetTrigger("ataque_ar");
            attacking_air = false;
        } else {
            anim.SetBool("parado",false);
            anim.SetBool("correndo",false);
            anim.ResetTrigger("ataque1");
            attacking = false;
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
        if(attacking)
            return;

        if(context.performed && IsGrounded()){
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            playSoundJump();
            if(!jumpping){
                jumpping = true;
            }
            if(canDust){
                dustInGround();
            }
        }

        if(context.canceled && rb.velocity.y > 0f){
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        anim.SetBool("parado",false);
        anim.SetBool("pulando-subindo",true);
        
    }

    public void dustInGround(){
        Instantiate(dust_jump_fx, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        canDust = false;
        StartCoroutine(nowCanDust());
    }

    IEnumerator nowCanDust(){
        yield return new WaitForSeconds(0.5f);        
        canDust = true;    
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
        if(attacking)
            return;
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Sword(InputAction.CallbackContext context){
        if(attacking || attacking_air)
            return;
        if(touchingGround){
            attacking = true;
            attacking_air = false;
            anim.SetTrigger("ataque1");
            anim.SetBool("parado",false);
            anim.SetBool("correndo",false);
            anim.SetBool("pulando-subindo",false);
            anim.SetBool("pulando-caindo",false);
            anim.SetBool("sofrendo-dano",false);
            anim.SetBool("morto",false);
        } else {
            attacking_air = true;
            attacking = false;
            anim.SetTrigger("ataque_ar");
            anim.SetBool("parado",false);
            anim.SetBool("correndo",false);
            anim.SetBool("pulando-subindo",false);
            anim.SetBool("pulando-caindo",false);
            anim.SetBool("sofrendo-dano",false);
            anim.SetBool("morto",false);
        }
              
    }

    public void createAttack1_fx(){
        var temporary_attack1_fx = Instantiate(attack1_fx, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        temporary_attack1_fx.transform.parent = gameObject.transform;

        // adjusts the direction of the effect according to the player's direction
        Vector3 fxScale = temporary_attack1_fx.transform.localScale;
        fxScale.x *= isFacingRight ? 1f : -1f;
        temporary_attack1_fx.transform.localScale = fxScale;

        temporary_attack1_fx.transform.parent = null;
    }

    public void createAttack_air_fx(){
        var temporary_attack_air_fx = Instantiate(attack_air_fx, new Vector3(transform.position.x-0.021f, transform.position.y+0.326f, 0), Quaternion.identity);
        temporary_attack_air_fx.transform.parent = gameObject.transform;

        // adjusts the direction of the effect according to the player's direction
        Vector3 fxScale = temporary_attack_air_fx.transform.localScale;
        fxScale.x *= isFacingRight ? 1f : -1f;
        temporary_attack_air_fx.transform.localScale = fxScale;

        //temporary_attack_air_fx.transform.parent = null;
    }

    public void fimAtaque(){
        anim.ResetTrigger("ataque1");
        anim.SetBool("parado",true);
        attacking = false;
    }

    public void fimAtaqueAr(){
        anim.ResetTrigger("ataque_ar");
        anim.SetBool("pulando-caindo",true);
        attacking_air = false;
    }

    //================== SOUNDs =====================
    public void playSoundJump(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[0];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundLanding(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[1];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundSwordAttack1(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[2];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundIchigoVoice1(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[3];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    //================= END SOUNDS ===================
}