using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scoreHandler : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    void Start()
    {
        timeText.text = $"Puntuación: {saveDestroyedPlanes.getInstance.score}";
    }
}
