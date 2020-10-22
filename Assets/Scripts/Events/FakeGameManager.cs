using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGameManager : MonoBehaviour
{
    public static FakeGameManager _instance;

    [SerializeField]
    private Launcher launcher;

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

    private void Start()
    {
        Initialization();
        launcher.EnterState();     
    }

    private void Update()
    {
        
    }

    public void Initialization()
    {
        eventWall = new EventWall("Wall");
        eventSmoke = new EventSmoke("Smoke");
        eventBomb = new EventBomb("Bomb");

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

    public GameObject GetEventWall()
    {
        return eventWallPrefab;
    }

    public GameObject GetEventBomb()
    {
        return eventBombPrefab;
    }

    public GameObject GetEventSmoke()
    {
        return eventSmokePrefab;
    }
}
