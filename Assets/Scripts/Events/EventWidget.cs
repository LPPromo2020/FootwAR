using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EventWidget : MonoBehaviour
{
    private int m_id;

    private Events m_eventLinked;

    [SerializeField]
    private GameObject m_eventPrefab;

    public enum enumEvents
    {
        bomb,
        wall,
        smoke
    }


    public void SetEvent(Events _event)
    {
        switch (_event.GetName())
        {
            case "Wall":
                m_eventLinked = new EventWall("Wall");
                break;

            case "Smoke":
                m_eventLinked = new EventSmoke("Smoke");
                break;

            case "Bomb":
                m_eventLinked = new EventBomb("Bomb");
                break;
        }

        Setup(_event);
    }

    public void Setup(Events _event)
    {
        m_eventPrefab = _event.GetEventCard();
    }

    public void OnSelection()
    {
        GetComponent<Launcher>().SetEventToLaunch(m_id);

        //Element visuel
    }

    public void OffSelection()
    {
        //
    }

    public int GetId()
    {
        return m_id;
    }

    public Events GetEvent()
    {
        return m_eventLinked;
    }
}