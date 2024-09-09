using UnityEngine;

public class EnemieHitBoxAttack : MonoBehaviour
{
    public float timedoSeldDestroy;
    private GameObject localHitBox;
    public bool samePositionFatherObject;

    void Start()
    {
        localHitBox = transform.parent.Find("local_hit_box").gameObject;
        Destroy(this.gameObject,timedoSeldDestroy);
    }

    
    void Update()
    {
        if(samePositionFatherObject && localHitBox != null){
            transform.position = localHitBox.transform.position;
        }
    }
}
