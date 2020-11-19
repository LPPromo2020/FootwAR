using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBomb : Events
{
    private string m_name = "Bomb";

    [SerializeField]
    private GameObject m_eventCardPrefab;

    [SerializeField]
    private float m_duration;

    [SerializeField]
    private float m_probality;

    public EventBomb(string name)
    {
        m_name = name;
    }

    public override void ThrowEvent()
    {
        Debug.Log("BOMB EVENT");
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
