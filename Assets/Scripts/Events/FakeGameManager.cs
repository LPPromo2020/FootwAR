using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGameManager : MonoBehaviour
{
    [SerializeField]
    private List<Events> m_listEvents = new List<Events>();

    [SerializeField]
    private GameObject eventWallPrefab;
    [SerializeField]
    private GameObject eventSmokePrefab;
    [SerializeField]
    private GameObject eventBombPrefab;

    private EventWall eventWall;
    private EventSmoke eventSmoke;
    private EventBomb eventBomb;

    private void Start()
    {
        Initialization();
    }

    private void Update()
    {
        
    }

    public void Initialization()
    {
        eventWall.SetEventCard(eventWallPrefab);
        eventSmoke.SetEventCard(eventSmokePrefab);
        eventBomb.SetEventCard(eventBombPrefab);

        m_listEvents.Add(eventWall);
        m_listEvents.Add(eventSmoke);
        m_listEvents.Add(eventBomb);
    }

    public List<Events> GetEvents()
    {
        return m_listEvents;
    }
}
