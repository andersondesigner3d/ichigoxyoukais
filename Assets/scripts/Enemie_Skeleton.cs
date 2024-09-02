using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemie_Skeleton : Enemie
{   
    // protected override void StartingGame()
    // {
    //     print("Humanoide iniciou o jogo.");
        
    // }
    [Header ("General")]
    public float velocidade = 3f;
    public float distanciaPerseguicao = 10f;
    private bool viradoParaDireita = true;

    public bool vivo = true;
    public bool andando;
    public bool atacando;
    public bool apanhando;

    void Update()
    {
        move();
        verifyLifeAmout();
        if(lifeAmount<=0){
            death();
        }
    }

    public void move(){

        if(!apanhando && vivo){
            float distancia = Vector2.Distance(transform.position, ichigo.transform.position);        
            if (distancia < distanciaPerseguicao)
            {
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

    void Flip(){
        if(vivo){
            viradoParaDireita = !viradoParaDireita;
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("ichigo_sword") && !apanhando && vivo){
            apanhando = true;
            anim.SetBool("dano",true);
            anim.SetBool("parado", false);
            anim.SetBool("andando", false);
            Vector2 forceDirection = (transform.position - other.transform.position).normalized;
            float forceMagnitude = 3f; // Ajuste este valor conforme necessário
            rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
            HorizontalCutFx();
            subtractLife(30);
            StartCoroutine(MicroPause());
        }
    }

    public void endDamage(){
        apanhando = false;
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