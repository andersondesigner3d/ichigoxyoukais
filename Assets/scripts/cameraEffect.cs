using System.Collections;
using UnityEngine;

public class cameraEffect : MonoBehaviour
{
    public Transform player = null;  
    public float smoothTime = 0.3f;  
    private Vector3 velocity = Vector3.zero;
    public GameController gameController;

    // Variáveis para o efeito de screenshake
    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float shakeFrequency = 25f; // Novo: controla a velocidade do tremor

    // Novo: Variável para ajustar o deslocamento vertical da câmera
    public float yOffset = 0f;

    void Update()
    {
        if(player != null)
        {
            // Adicione yOffset à posição Y do jogador
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y + yOffset, transform.position.z);

            if(isShaking)
            {
                // Usando Perlin Noise para um tremor mais rápido e suave
                float shakeOffsetX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f;
                float shakeOffsetY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * 2f;
                Vector3 shakeOffset = new Vector3(shakeOffsetX, shakeOffsetY, 0f) * shakeMagnitude;

                targetPosition += shakeOffset;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            FindInchigo();
        }
    }

    private void FindInchigo()
    {
        GameObject ichigoObject = GameObject.Find("ichigo");
        if (ichigoObject != null)
        {
            player = ichigoObject.transform;
        }
    }

    // Função para iniciar o screenshake
    public void Shake(float strength, float duration)
    {
        shakeMagnitude = strength;
        shakeDuration = duration;

        if(!isShaking)
            StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isShaking = false;
    }
}