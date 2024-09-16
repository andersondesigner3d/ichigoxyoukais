using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public Button newGameButton;
    
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    
    void Update()
    {
        endGame();
    }

    public void NewGame(){
        SceneManager.LoadScene("castle-intro");
    }

    public void CloseGame()
    {
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
}
