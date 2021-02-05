/*
Nom de l’auteur : Rémy CRESPE
    Principe du script : Permet de faire accelérer la voiture
    Utilisation : Tu met le prefab et il mis dedans
    */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AcceletrateButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    //[HideInInspector]
    public bool m_bAccelerate;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_bAccelerate = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_bAccelerate = false;
    }
}
