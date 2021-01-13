using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeTest : MonoBehaviour
{
    private void Awake()
    {
        GameObject debugPanelGO = GameObject.Find("DebugPanel");

        DebugPanel debugPanel = debugPanelGO.GetComponent<DebugPanel>();
        debugPanel.DebugText.text = "CUBE CREE " + debugPanel.cubeNB;
        
    }
}
