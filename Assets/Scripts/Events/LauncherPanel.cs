using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherPanel : MonoBehaviour
{
    private List<EventWidget> m_listEventWidget = new List<EventWidget>();

    //Make the link between the UI and the events's datas
    public void Inflate(List<Events> _listEventsToLaunch)
    {
        for(int i = 0; i < _listEventsToLaunch.Count; i++)
        {
            m_listEventWidget[i].Setup(_listEventsToLaunch[i]);
        }
        
    }

    public void AddEventWidget()
    {

    }

    public void RemoveEventWidget()
    {

    }
}
