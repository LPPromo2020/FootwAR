using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventWall : Events
{
    private string m_name = "Wall";

    [SerializeField]
    private GameObject m_eventCardPrefab;

    [SerializeField]
    private float m_duration;

    [SerializeField]
    private float m_probality;

    public EventWall(string name)
    {
        m_name = name;
    }

    public override void ThrowEvent()
    {
        Debug.Log("WALL EVENT");

        throw new System.NotImplementedException();
    }

    public override GameObject GetEventCard() 
    {
        return m_eventCardPrefab;
    }
    
    public override string GetName()
    {
        return m_name;
    }
}
