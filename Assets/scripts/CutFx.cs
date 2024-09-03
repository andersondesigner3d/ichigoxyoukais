using UnityEngine;

public class CutFx : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        int angle = Random.Range(1, 180);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        if(audioSource){
            playSoundCut();
        }
    }

    public void playSoundCut(){
        this.audioSource.enabled = true;
        this.audioSource.clip = audioClip;
        this.audioSource.PlayOneShot(this.audioSource.clip);
    }
}