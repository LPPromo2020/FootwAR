using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private List<Events> m_listEventsToLaunch = new List<Events>();
    private Events m_eventToLaunch = null;

    [SerializeField]
    private int m_eventsNbMax;

    private LauncherPanel m_launcherPanel;

    private void Start()
    {
        InitializeEventsToLauch();
        m_eventToLaunch = m_listEventsToLaunch[0];

        m_launcherPanel.Inflate(m_listEventsToLaunch);
    }

    public void SetEventToLaunch(int _idEvent)
    {
        m_eventToLaunch = m_listEventsToLaunch[_idEvent];
    }

    //Initialise launcher's cards
    public void InitializeEventsToLauch()
    {
        for(int i = 0; i < m_eventsNbMax; i++) { AddEvent(); }
    } 

    //Add an event selected randomly 
    public void AddEvent()
    {
        if(m_listEventsToLaunch.Count == m_eventsNbMax)
        {
            Debug.Log("Votre main est deja pleine !");
            return;
        }

        List<Events> listEvents = GetComponent<FakeGameManager>().GetEvents();

        //Select random event
        int i = Random.Range(0, listEvents.Count);//Get intance       
        m_listEventsToLaunch.Add(listEvents[i]);

        //Add event at Panel
        m_launcherPanel.AddEventWidget(listEvents[i]);
    }

    public void ThrowCurrentEvent()
    {
        m_eventToLaunch.ThrowEvent();

        m_launcherPanel.RemoveWidget();
        //m_launcherPanel.RemoveEventWidget();
    }
}
