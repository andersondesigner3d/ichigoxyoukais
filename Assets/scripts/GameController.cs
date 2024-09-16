using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header ("Principais")]
    public GameObject playerObject;
    public player ichigo = null;
    public Image lifeImage;
    public Image mpImage;
    [Header ("UI")]
    public BlackPanelUi blackPanelUi;
    [Header ("Points")]
    public GameObject[] pointCreator;
    public int pointNumber;
    [Header ("Others")]
    public int lifeAmount;
    public int mpAmount;
    public int swordDamage;
    public int recoveryMp;
    public int playerLevel;

    // void Awake(){
    //     DontDestroyOnLoad(gameObject);
    // }

    void Start(){
        lifeImage = GameObject.Find("life").GetComponent<Image>();
        mpImage = GameObject.Find("mp").GetComponent<Image>();
        StartCoroutine(RecoveryTime());
        StartCoroutine(ichigoCreator());
    }

    void Update(){
        FindIchigo();      
        ExitGame();
        verifyLifeAndMp();
        FindUiElements();
    }

    private void FindUiElements(){
        while(lifeImage == null || mpImage == null){
            lifeImage = GameObject.Find("life").GetComponent<Image>();
            mpImage = GameObject.Find("mp").GetComponent<Image>();
        }
    }

    private void FindIchigo(){
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

    public void CloseGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void GoToMenuPrincipal(){
        SceneManager.LoadScene("menu-principal");
    }

    private void verifyLifeAndMp(){
        if (ichigo != null){
            if(lifeImage != null){
                lifeImage.fillAmount = ichigo.lifeAmount / 100f;
            }
            if(mpImage != null){
                mpImage.fillAmount = ichigo.mpAmount / 100f;
            }
        }
    }

    public void LevelUp(){
        lifeAmount +=10;
        mpAmount +=10;
        swordDamage +=5;
        recoveryMp +=1;
    }

    public void BlackPanelUi(){
        blackPanelUi.Aparece();
    }

    public void RestartScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if(playerObject!=null){
            pointNumber = PlayerPrefs.GetInt("pointNumber");
            GameObject novoObjeto;
            switch (pointNumber)
            {
                case 1:
                    novoObjeto = Instantiate(playerObject, pointCreator[1].transform.position,pointCreator[1].transform.rotation);
                    break;
                
                case 2:
                    novoObjeto = Instantiate(playerObject, pointCreator[2].transform.position,pointCreator[2].transform.rotation);
                    break;
                
                default:
                    novoObjeto = Instantiate(playerObject, pointCreator[0].transform.position,pointCreator[0].transform.rotation);
                    break;
            }
            novoObjeto.name = playerObject.name;
            
            
        }
    }
}