using UnityEngine;

public class selfDestroyTimer : MonoBehaviour
{
    public float time;

    public void Start(){
        Destroy(gameObject,time);
    }
}
