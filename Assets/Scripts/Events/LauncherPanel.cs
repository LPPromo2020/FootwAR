using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherPanel : MonoBehaviour
{
    /*[SerializeField] 
    private GameObject eventWidgetGO = null;*/

    public static LauncherPanel _instance;

    private List<EventWidget> m_listEventWidget = new List<EventWidget>();

    private List<GameObject> m_listWidgetGO = new List<GameObject>();

    private int m_idWidgetSelected = -1;

    public int IdWidgetSelected { get => m_idWidgetSelected; set => m_idWidgetSelected = value; }
    public List<EventWidget> ListEventWidget { get => m_listEventWidget; set => m_listEventWidget = value; }

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
        GameObject eventWidgetGO = Instantiate(GetCardToBuild(_event), transform);

        EventWidget eventWidget = eventWidgetGO.GetComponent<EventWidget>();
        
        //Link event to the eventWidget
        eventWidget.SetEvent(_event);

        //Set wigdetID
        eventWidget.IdWidget = m_listEventWidget.Count;

        //Add the the eventWidget to the list
        m_listEventWidget.Add(eventWidget);
        m_listWidgetGO.Add(eventWidgetGO);
    }

    public void RemoveEventWidget(int _id)
    {
        m_listEventWidget.Remove(m_listEventWidget[_id]);

            //Restart the selection by default 
                //m_listEventWidget[0].OnSelection();

        Destroy(m_listWidgetGO[_id]);

        m_listWidgetGO.Remove(m_listWidgetGO[_id]);

        //Update the events's id
        UpdateEventsID();
    }
    
    /*public void OnWidgetSelected()//Listener
    {
        for(int i = 0; i < m_listEventWidget.Count; i++)
        {
            if (m_listEventWidget[i].GetIsSelected() == true)
            {
                Debug.Log(m_listEventWidget[i].name);
                m_idWidgetSelected = m_listEventWidget[i].GetId();
                Launcher._instance.SetEventToLaunch(m_idWidgetSelected);
            }
        }
    }*/

    public void DisableCards()
    {
        for (int i = 0; i < m_listEventWidget.Count; i++)
        {
            m_listEventWidget[i].SetIsSelected(false);
        }
        m_idWidgetSelected = -1;
    }

    public void RemoveWidget()
    {
        if(m_idWidgetSelected != -1)
        {
            RemoveEventWidget(m_idWidgetSelected);
        }
        else
        {
            Debug.Log("Remove impossible ! ");
        }
    }

    public void UpdateEventsID()
    {
        int size = m_listEventWidget.Count;

        /*if(size == 0)
        {
            m_listEventWidget[0].IdWidget = 0;
        }
        else
        {*/
            for (int i = 0; i < m_listEventWidget.Count; i++)
            {
                m_listEventWidget[i].IdWidget = i;
            }
        /*}*/
    }

    /*public int GetIdWidgetSelected()
    {
        return m_idWidgetSelected;
    }*/
}