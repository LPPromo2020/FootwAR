using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Nom des auteurs : Marc et Dorian
 Principe du script : instanciation des différents éléments de jeux
 Utilisation : à placer sur le stade avec tout les points de spawn qu'il faudra référencer dans l'inspector
 */




public class ItemsSpawner : MonoBehaviour
{
    //public properties
    public Transform m_TRedCarSpawn;
    public Transform m_TBlueCarSpawn;
    public Transform m_TBallSpawn;

    public BoxCollider m_BCRedGaol;
    public BoxCollider m_BCBlueGoal;

    //public 

    //Ces propriétés sont temporaires pour les phases de tests, il faudrat delete lors de l'intégration du réseaux
    public Mesh m_MtempCar;


    //Private properties


    private void Start()
    {

    }
    //Fonction a appeller pour faire spawnner l'ensemble de spawners ( à la création du stade )
    private void InitArena()
    {
       
    }

}
