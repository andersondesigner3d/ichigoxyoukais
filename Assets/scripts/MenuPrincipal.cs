using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public Button newGameButton;
    public Button continueButton;
    public Button exitGameButton;
    public AudioSource audioSource;
    public AudioSource audioSourceTheme;
    public AudioClip[] audioClips;

    private void Awake() {
        if(Time.timeScale==0){
            Time.timeScale = 1f;
        }
    }
    
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        playTheme();
    }

    void Update()
    {
        endGame();
    }

    public void NewGame(){
        playStart();
        continueButton.interactable = false;
        exitGameButton.interactable = false;
        StartCoroutine(TimeNewGame());
    }

    IEnumerator TimeNewGame(){
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("castle-intro");
    }

    public void CloseGame()
    {
        playExit();
        continueButton.interactable = false;
        newGameButton.interactable = false;
        StartCoroutine(TimeCloseGame());
    }

    IEnumerator TimeCloseGame(){
        yield return new WaitForSeconds(1);
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void endGame(){
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public void playTheme(){
        this.audioSourceTheme.enabled = true;
        this.audioSourceTheme.clip = audioClips[0];
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
