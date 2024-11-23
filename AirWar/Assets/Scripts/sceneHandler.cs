using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneHandler : MonoBehaviour
{
    public static void changeScene(int scene) {
        SceneManager.LoadScene(scene);
    }

    public static void retryChangeScene(int scene) {
        saveDestroyedPlanes.getInstance.planes = new List<planeInfo>();
        saveDestroyedPlanes.getInstance.score = 0;
        SceneManager.LoadScene(scene);
    }
}
