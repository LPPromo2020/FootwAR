using System.Collections;
using System.Collections.Generic;
using UnityEngine;


abstract public class Events
{
    private GameObject m_eventCardPrefab;
    private float m_duration;
    private float m_probality;

    public abstract void ThrowEvent();
}
