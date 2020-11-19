using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSmoke : Events
{
    private string m_name = "Smoke";

    [SerializeField]
    private GameObject m_eventCardPrefab;

    [SerializeField]
    private float m_duration;

    [SerializeField]
    private float m_probality;

    public EventSmoke(string name)
    {
        m_name = name;
    }

    public override void ThrowEvent()
    {
        Debug.Log("SMOKE EVENT");
    }

    public override GameObject GetEventCard()
    {
        return m_eventCardPrefab;
    }

    public override void SetEventCard(GameObject _eventCardPrefab)
    {
        m_eventCardPrefab = _eventCardPrefab;
    }

    public override string GetName()
    {
        return m_name;
    }
}
