using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlackPanel : MonoBehaviour
{
    public player ichigo = null;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        FindIchigo();
    }

    private void FindIchigo(){
        GameObject ichigoObject = GameObject.Find("ichigo");
        if (ichigoObject != null){
            ichigo = ichigoObject.GetComponent<player>();
        }
    }

    public void IchigoCanMove(){
        if(ichigo !=null){
            ichigo.startMove = true;
        }
    }

}
