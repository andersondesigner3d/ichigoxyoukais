using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{   
    [Header ("Principal")]
    public Animator anim;
    public Transform ichigoTransform;
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    [Header ("Sword Impact")]
    public GameObject swordImpact;
    [Header ("Moviment")]
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
    public bool jumpping;
    public float deadZone = 0.1f;
    public float jumpingPower;
    [Header ("Atacks")]
    public bool attacking;
    public bool attacking_air;
    public bool canAtack;
    [Header ("Visual FX")]
    public bool canDust = true;
    public GameObject dust_jump_fx;
    public GameObject dust_sword_fx;
    public GameObject dust_wind_fx;
    [Header ("Audio")]
    public AudioSource audioSource;
    public AudioClip[] audioClip;
    [Header ("Meteriais and Glows")]
    public Material originalMaterial;
    public Material swordGlowMaterial;
    public Material airSwordGlowMaterial;
    [Header ("Dash")]
    public bool dashing;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ichigoTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        swordImpact = transform.Find("sword_impact").gameObject;
        if(!touchingGround){
            jumpping = true;
        }
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
        if(!dashing){
            if(attacking){
                rb.velocity = new Vector2(0, rb.velocity.y);
            } else {
                if(!touchingWall && !attacking){
                    rb.velocity = new Vector2(horizontalMoviment * moveSpeed, rb.velocity.y);
                }
            }
        }

        if(dashing){
            rb.gravityScale = 0f;
        }


        fixAnimationBugs();
        FixMaterialBugs();
        FixAttackBugs();
        animationControll();
    }

    private void FixedUpdate() {

        
        
    }

    private void animationControll(){
        if(!attacking && !dashing && !attacking_air){
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

    private void FixAttackBugs(){
        if(touchingGround){
            attacking_air = false;
        } else {
            attacking = false;
        }
        if(attacking){
            attacking_air = false;
        }
        if(attacking_air){
            attacking = false;
        }
    }

    private void FixMaterialBugs(){
        if(!attacking && !attacking_air){
            spriteRenderer.material = originalMaterial;
        }
    }

    private void fixAnimationBugs(){
        //fiz animations bugs
        if(!attacking && !attacking_air && !dashing){
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
        if(dashing || attacking || attacking_air)
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
        if(attacking || dashing || attacking_air)
            return;
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Sword(InputAction.CallbackContext context){
        if (context.phase == InputActionPhase.Started){
            if(!attacking && !attacking_air && !dashing){
                if(touchingGround && canAtack){
                attacking = true;
                attacking_air = false;
                spriteRenderer.material = swordGlowMaterial;
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
                    canAtack = false;
                    StartCoroutine(TimeToNewAttack());
                    spriteRenderer.material = airSwordGlowMaterial;
                    anim.SetTrigger("ataque_ar");
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                    anim.SetBool("morto",false);
                }
            }
        }                       
    }

    IEnumerator TimeToNewAttack(){
        yield return new WaitForSeconds(0.2f);        
        canAtack = true;      
    }

    public void fimDoAtaque(){
        spriteRenderer.material = originalMaterial;
        anim.ResetTrigger("ataque1");
        anim.SetBool("parado",true);
        attacking = false;
    }

    public void fimAtaqueAr(){
        spriteRenderer.material = originalMaterial;
        anim.ResetTrigger("ataque_ar");
        anim.SetBool("pulando-caindo",true);
        attacking_air = false;
    }

    private void CriaDustFx(){
        GameObject dust = Instantiate(dust_sword_fx, transform.position, Quaternion.identity);
        Vector3 dustScale = dust.transform.localScale;
        dustScale.x = isFacingRight ? Mathf.Abs(dustScale.x) : -Mathf.Abs(dustScale.x);
        dust.transform.localScale = dustScale;
    }

    private void CriaDashWindFx(){
        GameObject dust = Instantiate(dust_wind_fx, transform.position, Quaternion.identity);
        Vector3 dustScale = dust.transform.localScale;
        dustScale.x = isFacingRight ? Mathf.Abs(dustScale.x) : -Mathf.Abs(dustScale.x);
        dust.transform.localScale = dustScale;
    }

    public void Dash(InputAction.CallbackContext context){
        if (context.phase == InputActionPhase.Started){
            if(!dashing && !attacking && !attacking_air){
                anim.SetTrigger("dash");
                anim.ResetTrigger("ataque_ar");
                anim.ResetTrigger("ataque1");
                anim.SetBool("parado",false);
                anim.SetBool("correndo",false);
                anim.SetBool("pulando-subindo",false);
                anim.SetBool("pulando-caindo",false);
                anim.SetBool("sofrendo-dano",false);
                anim.SetBool("morto",false);
                dashing = true;
                StartCoroutine(EndDash());
                
                float dashDirection = isFacingRight ? 1f : -1f;
                float dashForce = 5f;
                rb.velocity = new Vector2(dashForce * dashDirection, 0f);
                playSoundDashWind();
                int number = Random.Range(0, 2);
                if(number==1){
                    playSoundDashichigoVoice();
                }
                if(touchingGround){
                    CriaDustFx();
                } else {
                    CriaDashWindFx();
                }
                
            }
        }
    }

    IEnumerator EndDash(){
        yield return new WaitForSeconds(0.25f);        
        dashing = false;
        anim.ResetTrigger("dash");
        rb.gravityScale = 1f;
    }

    public void ActiveSwordImpact1(){
        swordImpact.SetActive(true);
    }

    public void DisableSwordImpact1(){
        swordImpact.SetActive(false);
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

    public void playSoundDashichigoVoice(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[4];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundDashWind(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[5];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    //================= END SOUNDS ===================
}