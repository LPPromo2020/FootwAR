using UnityEngine;

public class ChangeSceneByName : MonoBehaviour {
    public void Load(string name) {
        SceneLoader.LoadScene(name);
    }
}