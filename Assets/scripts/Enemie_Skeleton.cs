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

    public bool vivo;
    public bool andando;
    public bool atacando;

    void Update()
    {
        move();
        
    }

    public void move(){

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

    void animationControl(){

        }

    void Flip(){
        viradoParaDireita = !viradoParaDireita;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}
