using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public List<Events> m_listEventsToLaunch = new List<Events>();
    private Events m_eventToLaunch = null;

    private LauncherPanel launcherPanel;

    [SerializeField]
    private int m_eventsNbMax;

    //Initialise launcher's cards
    public void InitializeEventsToLauch()
    {
        for(int i = 0; i<m_eventsNbMax; i++) { AddEvent(); }
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

        int i = Random.Range(0, listEvents.Count);//Get intance
        m_listEventsToLaunch.Add(listEvents[i]);

        //AddAvent Panel
    }

    public void ThrowCurrentEvent()
    {
        m_eventToLaunch.ThrowEvent();
    }
}
