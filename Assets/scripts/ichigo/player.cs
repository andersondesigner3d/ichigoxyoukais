using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class player : MonoBehaviour
{   
    [Header ("Principal")]
    public bool vivo = true;
    public int lifeAmount;
    public int mpAmount;
    public Animator anim;
    public Transform ichigoTransform;
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public GameController gameController;
    public cameraEffect cameraFx;
    [Header ("Save Game")]
    public bool canSave;
    public GameObject saveGamePoint;
    [Header ("Sword Impact")]
    public GameObject localSwordImpact;
    public GameObject temporarySwordImpact = null;
    public GameObject temporarySwordImpactAir = null;
    public GameObject swordImpact;
    [Header ("Moviment")]
    public float moveSpeed = 5f;
    public float horizontalMoviment;
    public float verticalMoviment;
    public bool isFacingRight = true;
    [Header ("Sensors")]
    public Transform frontCheck;
    public bool touchingWall;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool touchingGround;
    [Header ("Jump")]
    public float deadZone = 0.1f;
    public float jumpingPower;
    [Header ("Atacks")]
    public bool attacking;
    public bool attacking_air;
    public bool canAtack = true;
    [Header ("Visual FX")]
    public bool canDust = true;
    public GameObject dust_jump_fx;
    public GameObject dust_sword_fx;
    public GameObject dust_wind_fx;
    [Header ("Audio")]
    public AudioSource audioSource;
    public AudioClip[] audioClip;
    [Header ("Materiais and Glows")]
    public Material originalMaterial;
    public Material swordGlowMaterial;
    public Material airSwordGlowMaterial;
    public Material whiteMaterial;
    [Header ("Dash")]
    public bool dashing;
    [Header ("Damage")]
    public int damage;
    public GameObject impactPoint;
    public GameObject enemieCutFx;
    public GameObject bloodFx;
    public GameObject damageText;
    public bool sofrendoDano;
    public float forcaImpactoFracaEixoX;//inimigos pequenos
    public float forcaImpactoMedioEixoX;//inimigos medios
    public float forcaImpactoForteEixoX;//inimigos grandes
    public float forcaImpactoFracaEixoY;//inimigos pequenos
    public float forcaImpactoMedioEixoY;//inimigos medios
    public float forcaImpactoForteEixoY;//inimigos grandes
    [Header ("Others")]
    public bool CanPlaySounds = false;
    public bool startMove = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        ichigoTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        localSwordImpact = transform.Find("local_sword_impact").gameObject;
        impactPoint = transform.Find("impact_point").gameObject;
        gameController = GameObject.Find("GameController")?.GetComponent<GameController>();
        if(gameController){
            gameController.DisableCameraAudioListner();
        }
        StartCoroutine(CanPlayAllSounds());
        cameraFx = GameObject.Find("Main_Camera").GetComponent<cameraEffect>();
    }
    
    void Update()
    {        
        //print("horizontalMoviment: "+horizontalMoviment);
        //print("rb.velocity.y = "+rb.velocity.y);
        //print("verticalMoviment = "+verticalMoviment);
        print(Time.timeScale);

        if(gameController.isPaused)
            return;

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
            if(sofrendoDano || !vivo)
                return;
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

        if(touchingGround){
            DisableSwordImpactAir();
        } else {
            DisableSwordImpact1();
        }


        fixAnimationBugs();
        FixMaterialBugs();
        FixAttackBugs();
        animationControll();
        SwordImpactFixPosition();
        FixLifeAmount();
        VerifyLife();
    }

    private void VerifyLife(){
        if(lifeAmount <= 0){
            vivo = false;
            anim.SetTrigger("morto");
            Death();
        } else {
            vivo = true;
        }
    }

    public void Death(){
        StartCoroutine(CameraLentaCoroutine(3));
        playSoundDeath();
        vivo = false;
        anim.SetBool("parado",false);
        anim.SetBool("correndo",false);
        anim.SetBool("pulando-subindo",false);
        anim.SetBool("pulando-caindo",false);
        anim.SetBool("sofrendo-dano",false);
        anim.ResetTrigger("ataque1");
        anim.ResetTrigger("ataque_ar");
        anim.ResetTrigger("dash");
        StartCoroutine(ShowBlackPanel());
    }

    IEnumerator ShowBlackPanel(){
        yield return new WaitForSeconds(3f);        
        gameController.BlackPanelUi();     
    }

    IEnumerator CanPlayAllSounds(){
        yield return new WaitForSeconds(1f);        
        CanPlaySounds = true;
    }

    private IEnumerator CameraLentaCoroutine(float duracao){
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSecondsRealtime(duracao);
        print("fim da camera lenta");
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private void FixLifeAmount(){
        if(lifeAmount < 0){
            lifeAmount = 0;
        }
    }

    private void animationControll(){
        if(!attacking && !dashing && !attacking_air && !sofrendoDano){
            if(touchingGround){            
                if(horizontalMoviment == 0 || touchingWall){
                    anim.SetBool("parado",true);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                } else if(horizontalMoviment != 0 && !touchingWall && rb.velocity.x != 0) {
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",true);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                }
            } else {
                if(rb.velocity.y >= 0){
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",true);
                    anim.SetBool("pulando-caindo",false);
                    anim.SetBool("sofrendo-dano",false);
                } else {
                    anim.SetBool("parado",false);
                    anim.SetBool("correndo",false);
                    anim.SetBool("pulando-subindo",false);
                    anim.SetBool("pulando-caindo",true);
                    anim.SetBool("sofrendo-dano",false);
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
        if(sofrendoDano){
            attacking_air = false;
            attacking = false;
            canAtack = false;
        }
    }

    private void FixMaterialBugs(){
        if(!attacking && !attacking_air){
            spriteRenderer.material = originalMaterial;
        }
    }

    private void fixAnimationBugs(){
        if(!attacking && !attacking_air && !dashing && !sofrendoDano){
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

        if(sofrendoDano || !vivo || !startMove)
            return;

        horizontalMoviment = Mathf.RoundToInt(context.ReadValue<Vector2>().x);
        verticalMoviment = Mathf.RoundToInt(context.ReadValue<Vector2>().y);

        if(context.canceled){
            StartCoroutine(delayControl());
        }
    }

    IEnumerator delayControl(){
        yield return new WaitForSeconds(0.05f);        
        if(horizontalMoviment == 0){
            //print("cria fumacinha");
        }       
    }

    public void Jump(InputAction.CallbackContext context){
        if(dashing || attacking || attacking_air || sofrendoDano || !vivo || !startMove || gameController.isPaused || !gameController.canJump)
            return;

        if(context.performed && IsGrounded()){
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            playSoundJump();
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
        Vector2 boxSize1 = new Vector2(0.15f, 0.02f);
        return Physics2D.OverlapBox(groundCheck.position, boxSize1, 0f, groundLayer);
    }

    private bool canMove(){
        Vector2 boxSize2 = new Vector2(0.1f, 0.4f);
        return Physics2D.OverlapBox(frontCheck.position, boxSize2, 0f, groundLayer);
    }

    private void OnDrawGizmos() {
        //ground sensor
        Vector2 groundBoxSize = new Vector2(0.15f, 0.02f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);

        //front sensor
        Vector2 frontBoxSize = new Vector2(0.1f, 0.4f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(frontCheck.position, frontBoxSize);
    }

    private void Flip(){
        if(attacking || dashing || attacking_air || !vivo)
            return;
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Sword(InputAction.CallbackContext context){
        if(gameController.isPaused)
            return;
        if (context.phase == InputActionPhase.Started){
            if(!attacking && !attacking_air && !dashing && !sofrendoDano && vivo && startMove){
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
        if(gameController.isPaused)
            return;
        if (context.phase == InputActionPhase.Started){
            if(!dashing && !attacking && !attacking_air && !sofrendoDano && mpAmount>=10 && vivo && startMove){
                spriteRenderer.material = originalMaterial;
                anim.SetTrigger("dash");
                anim.ResetTrigger("ataque_ar");
                anim.ResetTrigger("ataque1");
                anim.SetBool("parado",false);
                anim.SetBool("correndo",false);
                anim.SetBool("pulando-subindo",false);
                anim.SetBool("pulando-caindo",false);
                anim.SetBool("sofrendo-dano",false);
                dashing = true;
                mpAmount-=10;
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

    private void SwordImpactFixPosition(){
        if(localSwordImpact){
           if(temporarySwordImpact){
                temporarySwordImpact.transform.position = localSwordImpact.transform.position;
           }
           if(temporarySwordImpactAir){
                temporarySwordImpactAir.transform.position = localSwordImpact.transform.position;
           }
        }
    }

    public void ActiveSwordImpact1(){
        DisableSwordImpact1();
        temporarySwordImpact = Instantiate(swordImpact, localSwordImpact.transform.position, Quaternion.identity);
    }

    public void ActiveSwordImpactAir(){
        DisableSwordImpactAir();
        temporarySwordImpactAir = Instantiate(swordImpact, localSwordImpact.transform.position, Quaternion.identity);
    }

    public void DisableSwordImpact1(){
        if(temporarySwordImpact != null){
            Destroy(temporarySwordImpact);
            temporarySwordImpact = null;
        }
    }

    public void DisableSwordImpactAir(){
        if(temporarySwordImpactAir != null){
            Destroy(temporarySwordImpactAir);
            temporarySwordImpactAir = null;
        }
    }

    public void EndDamage(){
        sofrendoDano = false;
        canAtack = true;
        anim.SetBool("sofrendo-dano",false);
    }

    public void EnemieCutFx(){
        Instantiate(enemieCutFx, new Vector3(impactPoint.transform.position.x, impactPoint.transform.position.y, 0), Quaternion.identity);
    }
    
    public void BloodFx(){
        Instantiate(bloodFx, new Vector3(impactPoint.transform.position.x, impactPoint.transform.position.y, 0), Quaternion.identity);
    }

    public void DamageText(string value){
        GameObject textFx = Instantiate(damageText, new Vector3(impactPoint.transform.position.x, impactPoint.transform.position.y, 0), Quaternion.identity);
        textFx.GetComponent<DamageText>().value = value;
    }

    public void StartButton(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started){
            if(canSave && saveGamePoint!=null){
                PlayerPrefs.SetInt("pointNumber", saveGamePoint.GetComponent<SaveGamePoint>().pointNumber);
                PlayerPrefs.Save();
                saveGamePoint.GetComponent<SaveGamePoint>().creatDisketFx();
                canSave = false;
            } else {
                if(gameController.isPaused){
                    gameController.ResumeGame();
                } else {
                    gameController.PauseGame();
                }
            }
        }
        
        
        
            
    }

    //================== COLISIONS =====================
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("enemie_sword") && !dashing && !sofrendoDano && vivo){
            Enemie_Skeleton_White enemieScript = other.gameObject.GetComponentInParent<Enemie_Skeleton_White>();
            if (enemieScript != null)
            {
                if(enemieScript.type == 1){
                    damage = 10;
                } else if(enemieScript.type == 2){
                    damage = 15;
                } else {
                    damage = 20;
                }
            }
            //damage
            lifeAmount-=damage;
            //pause
            if(lifeAmount>0){
                StartCoroutine( MicroPause(0.2f));
            } 
            if(lifeAmount<=0){
                anim.SetTrigger("morto");
                Death();
            } else {
                playSoundDamage();
            }
            //fix bugs
            sofrendoDano = true;
            attacking_air = false;
            attacking = false;
            canAtack = false;
            DisableSwordImpact1();
            DisableSwordImpactAir();
            if(temporarySwordImpact){
                Destroy(temporarySwordImpact);
                temporarySwordImpact = null;
            }
            if(temporarySwordImpactAir){
                Destroy(temporarySwordImpactAir);
                temporarySwordImpactAir = null;
            }
            horizontalMoviment = 0;
            verticalMoviment = 0;
            rb.velocity = new Vector2(0, 0);
            //animations
            anim.ResetTrigger("dash");
            anim.ResetTrigger("ataque1");
            anim.ResetTrigger("ataque_ar");
            anim.ResetTrigger("especial");
            anim.SetBool("sofrendo-dano",true);
            anim.SetBool("parado",false);
            anim.SetBool("correndo",false);
            anim.SetBool("pulando-subindo",false);
            anim.SetBool("pulando-caindo",false);
            //FX
            EnemieCutFx();
            BloodFx();
            DamageText(damage.ToString());
            StartCoroutine(TimerWhiteMaterial());

            if(other.transform.parent.localScale.x > 0){
                if(touchingGround){
                    rb.AddForce(new Vector2(forcaImpactoFracaEixoX, forcaImpactoFracaEixoY), ForceMode2D.Impulse);
                } else {
                    rb.AddForce(new Vector2(forcaImpactoFracaEixoX, 0), ForceMode2D.Impulse);
                }
            } else {
                if(touchingGround){
                    rb.AddForce(new Vector2(-forcaImpactoFracaEixoX, forcaImpactoFracaEixoY), ForceMode2D.Impulse);
                } else {
                    rb.AddForce(new Vector2(-forcaImpactoFracaEixoX, 0), ForceMode2D.Impulse);
                }
            }
        }

        //save game
        if(other.gameObject.CompareTag("save_game") && vivo){
            canSave = true;
            saveGamePoint = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("save_game") && vivo){
            canSave = false;  
        }
    }

    IEnumerator MicroPause(float time){
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time); //0.1 0.2 0.3 Duração da micropausa em tempo real (ajuste conforme necessário)
        
        // Restaurar o tempo ao normal
        Time.timeScale = 1f;
        OriginalMaterial();
    }

    IEnumerator TimerWhiteMaterial(){
        WhiteMaterial();
        yield return new WaitForSecondsRealtime(0.3f);
        OriginalMaterial();
    }
        
    public void WhiteMaterial(){
        spriteRenderer.material = whiteMaterial;
    }

    public void OriginalMaterial(){
        spriteRenderer.material = originalMaterial;
    }

    //================== SOUNDS =====================
    public void playSoundJump(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[0];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundLanding(){
        if(!CanPlaySounds)
            return;
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

    public void playSoundDamage(){
        this.audioSource.enabled = true;
        int rand = Random.Range(7, 9);
        this.audioSource.clip = audioClip[rand];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playSoundDeath(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip[10];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    //================= END SOUNDS ===================
}