using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherPanel : MonoBehaviour
{
    [SerializeField] 
    private GameObject eventWidgetGO = null;

    private List<EventWidget> m_listEventWidget = new List<EventWidget>();

    private int m_idWidgetSelected;

    //Make the link between the UI and the events's datas
    public void Inflate(List<Events> _listEventsToLaunch)
    {
        //Initialize the widget list

        //Setup the widgets
        for (int i = 0; i < _listEventsToLaunch.Count; i++)
        {
            EventWidget eventWidget = null;
            m_listEventWidget.Add(eventWidget);
            //m_listEventWidget[i].SetEvent(_listEventsToLaunch[i]);
        }  
    }

    public void AddEventWidget(Events _event)
    {
        GameObject _eventWidgetGO = Instantiate(GetCardToBuild(_event), transform);

        /*GameObject _eventWidgetGO = Instantiate(eventWidgetGO, transform);

        EventWidget eventWidget = _eventWidgetGO.GetComponent<EventWidget>();

        eventWidget.SetEvent(_event);
        m_listEventWidget.Add(eventWidget);*/

    }

    public GameObject GetCardToBuild(Events _event)
    {
        GameObject cardToBuild = null;
        FakeGameManager instance = FakeGameManager._instance;

        switch (_event.GetName())
        {
            case "Wall":
                cardToBuild = instance.GetEventWall();
                break;

            case "Smoke":
                cardToBuild = instance.GetEventSmoke();
                break;

            case "Bomb":
                cardToBuild = instance.GetEventBomb();
                break;
        }

        return cardToBuild;
    }

    public void RemoveEventWidget(int _id)
    {
        Destroy(m_listEventWidget[_id].GetEvent().GetEventCard());
        m_listEventWidget.Remove(m_listEventWidget[_id]);
    }
    
    public void OnWidgetSelected()//Listener
    {
        for(int i = 0; i < m_listEventWidget.Count; i++)
        {
            if (m_listEventWidget[i].GetIsSelected())
            {
                m_idWidgetSelected = m_listEventWidget[i].GetId();
            }
        }
    }

    public void DisableCards()
    {
        for (int i = 0; i < m_listEventWidget.Count; i++)
        {
            m_listEventWidget[i].SetIsSelected(false);
        }
    }

    public void RemoveWidget()
    {
        RemoveEventWidget(m_idWidgetSelected);
    }

    /*public int GetIdWidgetSelected()
    {
        return m_idWidgetSelected;
    }*/
}