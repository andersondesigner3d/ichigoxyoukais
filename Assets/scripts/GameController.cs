using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header ("Principais")]
    public GameObject playerObject;
    public player ichigo = null;
    public Image lifeImage;
    public Image mpImage;
    [Header ("Points")]
    public GameObject pointCreator;
    public int lifeAmount;
    public int mpAmount;
    public int swordDamage;
    public int recoveryMp;
    public int playerLevel;

    void Start()
    {
        lifeImage = GameObject.Find("life").GetComponent<Image>();
        mpImage = GameObject.Find("mp").GetComponent<Image>();
        pointCreator = GameObject.Find("pointCreator");
        StartCoroutine(RecoveryTime());
        StartCoroutine(ichigoCreator());
    }

    void Update()
    {
        FindInchigo();      
        ExitGame();
        verifyLifeAndMp();
    }

    private void FindInchigo(){
        GameObject ichigoObject = GameObject.Find("ichigo");
        if (ichigoObject != null){
            ichigo = ichigoObject.GetComponent<player>();
        }
    }

    public void ExitGame(){
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void verifyLifeAndMp(){
        if (ichigo != null)
        {
            if(lifeImage != null){
                lifeImage.fillAmount = ichigo.lifeAmount / 100f;

            }
            if(mpImage != null){
                mpImage.fillAmount = ichigo.mpAmount / 100f;
            }
        }
    }

    public void LevelUp()
    {
        lifeAmount +=10;
        mpAmount +=10;
        swordDamage +=5;
        recoveryMp +=1;
    }

    IEnumerator RecoveryTime(){
        yield return new WaitForSeconds(1);        
        if(ichigo!=null && ichigo.vivo && ichigo.mpAmount < mpAmount){
            ichigo.mpAmount += recoveryMp;
        }
        StartCoroutine(RecoveryTime());
    }

    IEnumerator ichigoCreator(){
        yield return new WaitForSeconds(2);
        print("criou");
        if(playerObject!=null){
            GameObject novoObjeto = Instantiate(playerObject, pointCreator.transform.position,pointCreator.transform.rotation);
            novoObjeto.name = playerObject.name;
        }
    }
}
