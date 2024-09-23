using UnityEngine;

public class SaveGamePoint : MonoBehaviour
{
    public GameController gameController;
    public int pointNumber;
    public bool canSave = false;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public GameObject saveText;
    public GameObject disketFx;
    public GameObject localDisket;

    void Start()
    {
        gameController = GameObject.Find("GameController")?.GetComponent<GameController>();
        audioSource = GetComponent<AudioSource>();
        playSoundFire();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<player>().canSave){
            saveText.SetActive(true);
        }
    }    

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            saveText.SetActive(false);
        }
    }

    public void creatDisketFx(){
        GameObject disket = Instantiate(disketFx, new Vector3(localDisket.transform.position.x,localDisket.transform.position.y,0), Quaternion.identity);
        disket.transform.SetParent(localDisket.transform);
    }

    public void playSoundFire(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip;
        this.audioSource.Play();
    }
}