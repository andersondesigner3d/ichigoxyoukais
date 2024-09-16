using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlackPanelUi : MonoBehaviour
{
    public Animator animator;
    public GameObject panelMenu;
    public Button lastSaveGame;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Aparece(){
        animator.SetBool("aparece",true);
    }

    public void ShowMenu(){
        panelMenu.SetActive(true);
        SelectLastSaveButton();
    }

    public void SelectLastSaveButton(){
        EventSystem.current.SetSelectedGameObject(lastSaveGame.gameObject);
    }

    
}