using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGameManager : MonoBehaviour
{
    [SerializeField]
    private List<Events> m_listEvents = new List<Events>();

    public EventWall eventWall;
    public EventSmoke eventSmoke;
    public EventBomb eventBomb;

    private void Start()
    {
        Initialization();
    }

    private void Update()
    {
        
    }

    public void Initialization()
    {
        m_listEvents.Add(eventWall);
        m_listEvents.Add(eventSmoke);
        m_listEvents.Add(eventBomb);
    }

    /*[System.Serializable]//usefull?
    public enum enumEvents
    {
        bomb,
        wall,
        smoke
    }*/

    public List<Events> GetEvents()
    {
        return m_listEvents
    }
}
