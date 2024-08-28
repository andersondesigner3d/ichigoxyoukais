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
    [Header ("Atack1")]
    public bool attacking;
    [Header ("Visual FX")]
    public bool canDust = true;
    public GameObject attack1_fx;
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
        verificaVelocidadeY();
        if(touchingGround){
            jumpping = false;
        }

        //flip
        if (!isFacingRight && horizontalMoviment > deadZone){
            Flip();
        } else if(isFacingRight && horizontalMoviment < -deadZone){
            Flip();
        }        
        
        //moviment
        if(attacking){
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else {
            if(!touchingWall && !attacking){
                rb.velocity = new Vector2(horizontalMoviment * moveSpeed, rb.velocity.y);
            }
        }

        fixAnimationBugs();
        
        
    }

    private void fixAnimationBugs(){
        //fiz animations bugs
        if(touchingGround){
            anim.SetBool("pulando-subindo",false);
            anim.SetBool("pulando-caindo",false);
        } else {
            anim.SetBool("parado",false);
            anim.SetBool("correndo",false);
        }
    }

    private void FixedUpdate() {
        //animations control
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

        //verify dust on falling
        dustInGround();
        
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
            playSoundJump(0);
            if(canDust && !jumpping){
                Instantiate(dust_jump_fx, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                canDust = false;
                jumpping = true;
                StartCoroutine(nowCanDust());
            }
        }

        if(context.canceled && rb.velocity.y > 0f){
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        anim.SetBool("parado",false);
        anim.SetBool("pulando-subindo",true);

        
        
        
    }

    private void dustInGround(){
        if(rb.velocity.y < -0.5f && canDust && !jumpping && touchingGround){
            playSoundLanding(1);
            Instantiate(dust_jump_fx, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            canDust = false;
            StartCoroutine(nowCanDust());
        }
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
        if(attacking || !touchingGround)
            return;
        attacking = true;
        anim.SetTrigger("ataque1");        
        anim.SetBool("parado",false);
        anim.SetBool("correndo",false);
        anim.SetBool("pulando-subindo",false);
        anim.SetBool("pulando-caindo",false);
        anim.SetBool("sofrendo-dano",false);
        anim.SetBool("morto",false);        
    }

    public void createAttack1_fx(){
        var temporary_attack1_fx = Instantiate(attack1_fx, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        temporary_attack1_fx.transform.parent = gameObject.transform;

        // Ajusta a direção do efeito de acordo com a direção do player
        Vector3 fxScale = temporary_attack1_fx.transform.localScale;
        fxScale.x *= isFacingRight ? 1f : -1f;
        temporary_attack1_fx.transform.localScale = fxScale;

        temporary_attack1_fx.transform.parent = null;
    }

    public void fimAtaque(){
        anim.ResetTrigger("ataque1");
        anim.SetBool("parado",true);
        attacking = false;
        print(attacking);
    }

    void verificaVelocidadeY(){
        if(rb.velocity.y != 0){
            velocidadeY = true;
        } else {
            velocidadeY = false;
        }
    }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     if(other.gameObject.CompareTag("ground")){

    //     }
    // }

    //================== SOUNDs =====================
    public void playSoundJump(int position){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[position];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundLanding(int position){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[position];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundSwordAttack1(int position){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[position];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundIchigoVoice1(int position){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[position];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    //================= END SOUNDS ===================
}