using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    private float messageTimer;
    private Color targetColor;
    private Color originalColor;

    void Start()
    {
        if (messageText != null)
        {
            messageText.text = "";
            originalColor = messageText.color;
        }
    }

    public void ShowMessage(string msg, float duration = 3f)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.color = originalColor;
        messageTimer = duration;
    }

    public void ShowColoredMessage(string msg, Color color, float duration = 3f)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.color = color;
        messageTimer = duration;
    }

    void Update()
    {
        if (messageText == null) return;

        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;

            // cuando quede menos de 1 segundo, empieza a desvanecer
            if (messageTimer < 1f)
            {
                Color c = messageText.color;
                c.a = Mathf.Clamp01(messageTimer); // transparencia suave
                messageText.color = c;
            }

            if (messageTimer <= 0)
            {
                messageText.text = "";
                Color c = messageText.color;
                c.a = 1f;
                messageText.color = c;
            }
        }
    }
}
