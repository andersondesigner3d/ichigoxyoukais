using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private TextMeshPro damageText;
    public string value;

    void Start()
    {
        damageText = GetComponent<TextMeshPro>();

        if (damageText != null)
        {
            damageText.text = value;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI não encontrado!");
        }
    }

    public void SelfDestroy(){
        Destroy(gameObject);
    }
}