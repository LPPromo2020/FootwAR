using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public static Launcher _instance;

    private List<Events> m_listEventsToLaunch = new List<Events>();
    private Events m_eventToLaunch = null;

    private List<Events> m_listEvents = new List<Events>();

    [SerializeField]
    private int m_eventsNbMax = 3;

    [SerializeField]
    private LauncherPanel m_launcherPanel;

    public void EnterState()
    {
        InitializeEventsToLauch();
        //m_eventToLaunch = m_listEventsToLaunch[0];

        //m_launcherPanel.Inflate(m_listEventsToLaunch);
    }

    #region Singleton
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
    #endregion

    public void SetEventToLaunch(Events _event/*int _idEvent*/)
    {
        m_eventToLaunch = _event;
        Debug.Log("carte a lancer  : " + m_eventToLaunch.GetName());
    }


    //Initialise launcher's cards
    public void InitializeEventsToLauch()
    {
        for(int i = 0; i < m_eventsNbMax; i++) 
        {
            AddEvent();
        }
    } 

    //Add an event selected randomly 
    public void AddEvent()
    {
        if (m_launcherPanel.ListEventWidget.Count == m_eventsNbMax)
        {
            Debug.Log("Votre main est deja pleine !");
            return;
        }

        //m_listEvents = GetComponent<FakeGameManager>().GetEvents();
        m_listEvents = FakeGameManager._instance.GetEvents();

        //Select random event
        int i = Random.Range(0, m_listEvents.Count);//Get intance       
        //m_listEventsToLaunch.Add(m_listEvents[i]);

        //Add event at Panel
        m_launcherPanel.AddEventWidget(m_listEvents[i]);
    }

    public void ThrowCurrentEvent()
    {
        if(m_eventToLaunch != null)
        {
            m_launcherPanel.RemoveWidget();

            m_launcherPanel.DisableCards();

            m_eventToLaunch.ThrowEvent();
            //m_launcherPanel.RemoveEventWidget();
        }
        else
        {
            Debug.Log("Aucune carte selectionnée !");
        }
        m_eventToLaunch = null;
    }
}
