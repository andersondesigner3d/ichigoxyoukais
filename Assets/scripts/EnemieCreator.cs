using System.Collections;
using UnityEngine;

public class EnemieCreator : MonoBehaviour
{
    public player ichigo;
    public float distancia;
    public float distanciaCriacao;
    public GameObject enemieType;
    public GameObject enemie;
    public GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if(gameController != null){
            distanciaCriacao = gameController.enemieDistanceRespawn;
        }
        ichigo = FindObjectOfType<player>();
        enemie = Instantiate(enemieType, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        StartCoroutine(CaculateDistance());
        StartCoroutine(RespawnEnemie());
    }
    
    void Update()
    {
        FindInchigo();
    }

    public void FindInchigo(){
        GameObject ichigoObject = GameObject.Find("ichigo");
        if (ichigoObject != null){
            ichigo = ichigoObject.GetComponent<player>();
        }
    }

    IEnumerator RespawnEnemie(){
        yield return new WaitForSecondsRealtime(6f);
        if((distancia > distanciaCriacao) && enemie == null){
            enemie = Instantiate(enemieType, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        }
        StartCoroutine(RespawnEnemie());
    }

    IEnumerator CaculateDistance(){        
        yield return new WaitForSecondsRealtime(5f);
        if(ichigo!=null){
            distancia = Vector2.Distance(transform.position, ichigo.transform.position);
        }
        StartCoroutine(CaculateDistance());
    }

    
}