using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EventWidget : MonoBehaviour
{
    private int m_idWidget;
    private Events m_eventLinked;  
    private bool m_isSelected = false;

    [SerializeField]
    private GameObject m_outline;

    public int IdWidget { get => m_idWidget; set => m_idWidget = value; }

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

        //Setup(_event);
    }

    /*public void Setup(Events _event)
    {
        m_eventPrefab = _event.GetEventCard();
    }*/

    public void OnSelection()
    {
        if (!m_isSelected)
        {
            //GetComponent<Launcher>().SetEventToLaunch(m_idWidget);

            LauncherPanel._instance.DisableCards();

            m_isSelected = true;
            //LauncherPanel._instance.OnWidgetSelected();

            m_outline.SetActive(true);




            //Tell the Lauchner what event to launch
            Launcher._instance.SetEventToLaunch(m_eventLinked);

            //Reference the Id of the current selected widget to the panel
            LauncherPanel._instance.IdWidgetSelected = m_idWidget;

            Debug.Log(LauncherPanel._instance.IdWidgetSelected);
        }
    }

    public void OffSelection()
    {
        if(this != null)
        {
            m_outline.SetActive(false);
            m_isSelected = false;
        }

    }

    /*public int GetId()
    {
        return m_idWidget;
    }*/

    public Events GetEvent()
    {
        return m_eventLinked;
    }

    public bool GetIsSelected()
    {
        return m_isSelected;
    }

    public void SetIsSelected(bool _isSelected)
    {
        m_isSelected = _isSelected;

        if (!_isSelected)
        {
            OffSelection();
        }
    }
}