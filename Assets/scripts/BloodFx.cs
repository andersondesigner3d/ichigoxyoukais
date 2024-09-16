using UnityEngine;

public class BloodFx : MonoBehaviour
{
    public Animator animator;
    public Transform objTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        objTransform = GetComponent<Transform>();
        if(objTransform!=null){
            float number0 = Random.Range(0.5f, 1);
            objTransform.localScale = new Vector3(number0, number0,0);
        }
    }

    
    void Update()
    {
        int number = Random.Range(1, 3);
        switch (number)
        {
            case 1:
            animator.SetBool("blood1",true);
            break;

            case 2:
            animator.SetBool("blood2",true);
            break;
        }
    }

    public void selfDestroy(){
        Destroy(this.gameObject);
    }
}
