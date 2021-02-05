using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BrakeButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool m_bBrake;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_bBrake = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_bBrake = false;
    }
}
