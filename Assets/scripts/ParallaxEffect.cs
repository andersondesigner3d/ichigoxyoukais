using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform cam;
    public float relativeMove = 0.3f;
    public bool locky = false;
    public float correctionY = 0;

    void Update(){
        if(locky){
            transform.position = new Vector2(cam.position.x * relativeMove, transform.position.y);
        } else {
            transform.position = new Vector2(cam.position.x * relativeMove, (cam.position.y * relativeMove) + correctionY);
        }
    }



}
