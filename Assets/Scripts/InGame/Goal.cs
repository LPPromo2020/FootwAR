using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Nom des auteurs : Marc et Dorian
 Principe du script : instanciation des différents éléments de jeux
 Utilisation : à placer sur le stade avec tout les points de spawn qu'il faudra référencer dans l'inspector
 */

public class Goal : MonoBehaviour
{
    private BoxCollider m_BCGoal;
    void Start()
    {
        m_BCGoal = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            Debug.Log("Goal.cs : Goal, need to implement function for server score");
        }
    }
}
