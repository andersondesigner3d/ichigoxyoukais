using System.Collections;
using UnityEngine;

public class Enemie : MonoBehaviour {

    [Header ("Principal")]
    public Animator anim;
    public AudioSource audioSource;
    public SpriteRenderer spriteRenderer;
    public Transform myTransform;
    public Rigidbody2D rb;
    public player ichigo;
    public GameObject impactPoint;
    [Header ("Others")]
    public int lifeAmount;
    public string swordDamage;
    public GameObject ItemDroped;
    [Header ("Audio")]
    public AudioClip soundAttack;
    public AudioClip soundDeath;
    [Header ("FX")]
    public GameObject horizontalCutFx;
    public GameObject damageText;
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ichigo = FindObjectOfType<player>();
        audioSource = GetComponent<AudioSource>();
        impactPoint = transform.Find("impactPoint").gameObject;
        StartingGame();
    }

    protected virtual void StartingGame(){
        //faz alguma coisa quando o objeto é criado
    }

    public void FindInchigo(){
        GameObject ichigoObject = GameObject.Find("ichigo");
        if (ichigoObject != null){
            ichigo = ichigoObject.GetComponent<player>();
        }
    }

    public void PlaySoundAttack(){
        this.audioSource.enabled = true;
        this.audioSource.clip = soundAttack;
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void PlaySoundDeath(){
        this.audioSource.enabled = true;
        this.audioSource.clip = soundDeath;
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void SubtractLife(int valor){
        lifeAmount -= valor;
    }

    public void VerifyLifeAmout(){
        if(lifeAmount < 0){
            lifeAmount = 0;
        }
    }

    public IEnumerator MicroPause(){
        if(ichigo.lifeAmount > 0){
            // Reduzir o tempo para criar o efeito de micropausa
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.2f); // Duração da micropausa em tempo real (ajuste conforme necessário)
            
            // Restaurar o tempo ao normal
            Time.timeScale = 1f;
        }
    }
}
