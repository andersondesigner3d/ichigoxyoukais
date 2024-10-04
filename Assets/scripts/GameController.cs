using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [Header ("Principais")]
    public GameObject playerObject;
    public player ichigo = null;
    public Image lifeImage;
    public Image mpImage;
    public AudioListener cameraAudioListner;
    public AudioSource audioSource;
    public AudioSource audioSourceTheme;
    public AudioClip[] audioClips;
    public float timeToCreateIchigo;
    public float enemieDistanceRespawn;
    [Header ("Buttons")]
    public Button restarGameButton;
    public Button exitGameButton;
    public Button resumeButton;
    public Button menuPrincipalButton;
    [Header ("UI")]
    public BlackPanelUi blackPanelUi; //gameover
    public GameObject blackPanelStartUi; //inicio
    public GameObject panelPause;
    public GameObject resumeGameButton;
    [Header ("Points")]
    public GameObject[] pointCreator;
    public int pointNumber;
    [Header ("Others")]
    public int lifeAmount;
    public int mpAmount;
    public int swordDamage;
    public int recoveryMp;
    public int playerLevel;
    public bool isPaused = false;
    public bool canPause;
    public bool canJump;

    // void Awake(){
    //     DontDestroyOnLoad(gameObject);
    // }

    void Start(){
        lifeImage = GameObject.Find("life").GetComponent<Image>();
        mpImage = GameObject.Find("mp").GetComponent<Image>();
        StartCoroutine(RecoveryTime());
        StartCoroutine(ichigoCreator());
        playTheme1();
        blackPanelStartUi.SetActive(true);
    }

    void Update(){
        FindIchigo();      
        ExitGame();
        //PauseGame();
        verifyLifeAndMp();
        FindUiElements();
        //fixBugPause();
    }

    private void fixBugPause(){
        if(!isPaused){
            //Time.timeScale = 1f;
        }
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

    public void EnableCameraAudioListner(){
        cameraAudioListner.enabled = true;
    }
    public void DisableCameraAudioListner(){
        cameraAudioListner.enabled = false;
    }

    public void ExitGame(){
        if (Input.GetKey("escape") && ichigo!=null && ichigo.vivo)
        {
            if(!isPaused){
                if(canPause){
                    PauseGame();
                }                
            } else {
                return;
            }
        }
    }

    public void PauseGame(){
        canPause = false;
        canJump = false;
        StartCoroutine(libertCanPause());
        panelPause.SetActive(true);
        playStart();
        EventSystem.current.SetSelectedGameObject(resumeGameButton.gameObject);
        if (!isPaused)
        {
            Time.timeScale = 0f; // Pausa o tempo
            isPaused = true;
        }
    }

    IEnumerator libertCanPause(){
        yield return new WaitForSecondsRealtime(0.5f);
        canPause = true;
    }

    public void ResumeGame(){
        playStart();
        panelPause.SetActive(false);
        if (isPaused)
        {
            Time.timeScale = 1f; // Pausa o tempo
            isPaused = false;
        }
        StartCoroutine(NowCanJump());
    }

    IEnumerator NowCanJump(){
        yield return new WaitForSecondsRealtime(0.1f);
        canJump = true;
    }

    public void CloseGame()
    {
        CancelAllCoroutinesInScene();
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void GoToMenuPrincipal(){
        playExit();
        restarGameButton.interactable = false;
        resumeButton.interactable = false;
        StartCoroutine(TimeGoToMenuPrincipal());
    }

    IEnumerator TimeGoToMenuPrincipal(){
        yield return new WaitForSecondsRealtime(1);
        CancelAllCoroutinesInScene();
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
        this.audioSourceTheme.Stop();
        DesativarAudioDosFilhos();
        playThemeGameOver();
        blackPanelUi.Aparece();
    }

    public void RestartScene(){
        playStart();
        exitGameButton.interactable = false;
        StartCoroutine(TimeRestarScene());
    }

    IEnumerator TimeRestarScene(){
        yield return new WaitForSeconds(1);
        CancelAllCoroutinesInScene();
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
        yield return new WaitForSeconds(timeToCreateIchigo);
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

    public void DesativarAudioDosFilhos(){
        // Percorre todos os objetos filhos do objeto pai
        foreach (Transform child in transform)
        {
            // Verifica se o nome do objeto começa com "pointCreator"
            if (child.name.StartsWith("pointCreator"))
            {
                // Tenta pegar o componente AudioSource no filho
                AudioSource audioSource = child.GetComponent<AudioSource>();

                // Se o AudioSource for encontrado, desativa
                if (audioSource != null)
                {
                    audioSource.enabled = false;
                }
            }
        }
    }

    public void CancelAllCoroutinesInScene(){
        // Encontra todos os GameObjects ativos na cena
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Para cada objeto ativo
        foreach (GameObject obj in allObjects)
        {
            // Obtem todos os componentes MonoBehaviour do objeto
            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();

            // Para cada MonoBehaviour encontrado, cancela suas corrotinas
            foreach (MonoBehaviour script in scripts)
            {
                script.StopAllCoroutines();
            }
        }
        Debug.Log("Todas as corrotinas foram canceladas.");
    }

    public void playTheme1(){
        this.audioSourceTheme.enabled = true;
        this.audioSourceTheme.clip = audioClips[0];
        this.audioSourceTheme.loop = true;
        this.audioSourceTheme.Play();
    }

    public void playThemeGameOver(){
        this.audioSourceTheme.enabled = true;
        this.audioSourceTheme.clip = audioClips[4];
        this.audioSourceTheme.loop = true;
        this.audioSourceTheme.Play();
    }

    public void playStart(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClips[1];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playExit(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClips[2];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }

    public void playChangeMenu(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClips[3];
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }
}