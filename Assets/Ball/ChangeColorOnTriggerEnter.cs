using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cléry Chassagnon
//Utilité: Aucune utilité
public class ChangeColorOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Color randomlySelectColor = GetRandomColorWithAlpha();

        GetComponent<Renderer>().material.color = randomlySelectColor;
    }

    private Color GetRandomColorWithAlpha()
    {
        return new Color(
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f),
                0.25f);
    }
}
