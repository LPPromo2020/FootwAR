using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherPanel : MonoBehaviour
{
    /*[SerializeField] 
    private GameObject eventWidgetGO = null;*/

    public static LauncherPanel _instance;

    private List<EventWidget> m_listEventWidget = new List<EventWidget>();

    private int m_idWidgetSelected = -1;

    private Launcher launcher;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
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

    public void AddEventWidget(Events _event)
    {
        GameObject _eventWidgetGO = Instantiate(GetCardToBuild(_event), transform);

        EventWidget eventWidget = _eventWidgetGO.GetComponent<EventWidget>();

        eventWidget.SetEvent(_event);
        m_listEventWidget.Add(eventWidget);

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
            if (m_listEventWidget[i].GetIsSelected() == true)
            {
                m_idWidgetSelected = m_listEventWidget[i].GetId();
                launcher.SetEventToLaunch(m_idWidgetSelected);
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