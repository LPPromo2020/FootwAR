using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScenes : MonoBehaviour
{  
    public void OnClick(string sSceneName)
    {
        SceneLoader.LoadScene(sSceneName);
    }
}