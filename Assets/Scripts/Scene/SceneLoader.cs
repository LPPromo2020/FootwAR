using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName, Action<AsyncOperation> callback = null) {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (ao == null) {
            Debug.LogError("impossible de charger la scene " + sceneName);
            return;
        }

        ao.completed += callback;
        ao.completed += a => Debug.Log("Fin du chargement de la scene " + sceneName);
    }
}
