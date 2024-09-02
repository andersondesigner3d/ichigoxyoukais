using UnityEngine;

public class CutFx : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
