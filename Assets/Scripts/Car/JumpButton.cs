/*
Nom de l’auteur : Rémy CRESPE
    Principe du script : Permet de faire sauter la voiture
    Utilisation : Tu met le prefab et il mis dedans
    */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public bool m_bPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_bPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_bPressed = false;
    }

}
