using UnityEngine;

public class Enemie_Skeleton : Enemie
{   
    
    [Header ("General")]
    public float velocidade = 3f;
    public float distanciaPerseguicao = 10f;
    private float distanciaAtaque = 0.4f;
    private bool viradoParaDireita = true;

    public bool vivo = true;
    public bool andando;
    public bool atacando = false;
    public bool apanhando;

    public float distancia;

    // protected override void StartingGame()
    // {
    //     print("Humanoide iniciou o jogo.");
        
    // }

    void Update()
    {
        if(!atacando){
            Move();
        }
        VerifyLifeAmout();
        if(lifeAmount<=0){
            death();
        }
        distancia = Vector2.Distance(transform.position, ichigo.transform.position);
        Ataca();
    }

    public void Move(){

        if(!apanhando && vivo){
                    
            if (distancia < distanciaPerseguicao){

                Vector2 direcao = (ichigo.transform.position - transform.position).normalized;
                rb.velocity = new Vector2(direcao.x * velocidade, rb.velocity.y);
                
                if (direcao.x > 0 && !viradoParaDireita)
                {
                    Flip();
                }
                else if (direcao.x < 0 && viradoParaDireita)
                {
                    Flip();
                }

            } else {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if(rb.velocity.x != 0){
                anim.SetBool("parado", false);
                anim.SetBool("andando", true);
            } else {
                anim.SetBool("parado", true);
                anim.SetBool("andando", false);
            }
        }
    }

    void Ataca(){
        if((distancia < distanciaAtaque) && !apanhando && !atacando){
            rb.velocity = new Vector2(0, rb.velocity.y);
            atacando = true;
            anim.SetBool("atacando", true);
            anim.SetBool("parado", false);
            anim.SetBool("andando", false);
        }
    }

    void EndAttack(){
        atacando = false;
        apanhando = false;
        anim.SetBool("atacando", false);
    }

    void Flip(){
        if(vivo){
            viradoParaDireita = !viradoParaDireita;
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }
    
    public void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("ichigo_sword")){
            if(vivo){
                if(!atacando){
                    anim.SetBool("dano",true);
                    rb.velocity = new Vector2(0, rb.velocity.y);//stop 
                    Vector2 forceDirection = (transform.position - other.transform.position).normalized;
                    float forceMagnitude = 2.5f; // Ajuste este valor conforme necessário
                    rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
                }
                anim.SetBool("parado", false);
                anim.SetBool("andando", false);
                
                HorizontalCutFx();
                SubtractLife(30);
                StartCoroutine(MicroPause());
                apanhando = true;
            }
        }
    }

    public void endDamage(){
        apanhando = false;
        atacando = false;
        anim.SetBool("dano",false);        
    }

    public void death(){
        vivo = false;
        apanhando = false;
        anim.SetBool("morto",true);
        anim.SetBool("dano",false);
        anim.SetBool("parado", false);
        anim.SetBool("andando", false);
    }

    public void HorizontalCutFx(){
        Instantiate(horizontalCutFx, new Vector3(impactPoint.transform.position.x, impactPoint.transform.position.y, 0), Quaternion.identity);
    }

}