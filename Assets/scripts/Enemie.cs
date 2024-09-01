using UnityEngine;

public class Enemie : MonoBehaviour
{
    [Header ("Principal")]
    public Animator anim;
    public AudioSource audioSource;
    public SpriteRenderer spriteRenderer;
    public Transform myTransform;
    public Rigidbody2D rb;
    public player ichigo;
    [Header ("Others")]
    public int lifeAmount;
    public GameObject ItemDroped;
    [Header ("Audio")]
    public AudioClip soundAttack;
    public AudioClip soundDeath;
    // [Header ("FX")]
    // public GameObject windFx;
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ichigo = FindObjectOfType<player>();
        audioSource = GetComponent<AudioSource>();
        StartingGame();
    }

    protected virtual void StartingGame(){
        //faz alguma coisa quando o objeto é criado
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
}
