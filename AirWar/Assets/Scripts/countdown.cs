using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class countdown : MonoBehaviour
{
    private float timeRemaining = 90f;
    public TextMeshProUGUI timeText;
    void Start()
    {
        StartCoroutine(updateTime());   
    }

    private IEnumerator updateTime() {
        while(timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            updateTextTime();

            yield return null;
        }

        timeRemaining = 0;

        foreach(planeInfo item in saveDestroyedPlanes.getInstance.planes) {
            Debug.Log($"{item.planeGUID}");
        }

        MergeSortPlaneInfo.MergeSort(saveDestroyedPlanes.getInstance.planes);

        SceneManager.LoadScene(2);
    }

    private void updateTextTime()
    {
        int minutos = Mathf.FloorToInt(timeRemaining / 60);
        int segundos = Mathf.FloorToInt(timeRemaining % 60);

        if (timeText != null)
            timeText.text = $"{minutos:D2}:{segundos:D2}";
    }
}
