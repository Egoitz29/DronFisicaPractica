using UnityEngine;
using TMPro;

public class ControlsPanel : MonoBehaviour
{
    [TextArea(3, 6)]
    public string controlsInfo = "🕹️ CONTROLES\nTAB → Alternar modo de vuelo\nClic Izquierdo → Disparar\nWASD → Mover\nESPACIO / CTRL → Subir / Bajar";

    public TextMeshProUGUI controlsText;

    void Start()
    {
        if (controlsText != null)
            controlsText.text = controlsInfo;
    }
}
