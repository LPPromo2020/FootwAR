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

    [SerializeField]
    private float m_reloadTime = 10f;

    private bool m_canAddEvent = false;
    private bool m_isHandFull = false;

    private float m_newEventTimer;
    private float m_timer;

    public void EnterState()
    {
        InitializeEventsToLaunch();
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

    private void Update()
    {
        //Check if hand is full 
        if (!m_isHandFull)
        {
            m_timer += Time.deltaTime;
        }
        else
        {
            m_timer = 0f;
        }

        //Check if reload time is past
        if (m_timer - m_newEventTimer >= m_reloadTime && m_canAddEvent)
        {
            m_canAddEvent = false;
            AddEvent();
            m_timer = m_newEventTimer;
            m_canAddEvent = true;
        }

        #region Debug

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("EVENT TIMER : " + m_newEventTimer);
            Debug.Log(m_reloadTime);
            Debug.Log(m_timer + m_newEventTimer);
        }
        #endregion
    }


    public void SetEventToLaunch(Events _event/*int _idEvent*/)
    {
        m_eventToLaunch = _event;
        Debug.Log("carte a lancer  : " + m_eventToLaunch.GetName());
    }


    //Initialise launcher's cards
    public void InitializeEventsToLaunch()
    {
        for(int i = 0; i < m_eventsNbMax; i++) 
        {
            AddEvent();          
        }
    } 

    //Add an event selected randomly 
    public void AddEvent()
    {
        if (m_isHandFull)
        {
            Debug.Log("Votre main est deja pleine !");
            return;
        }
        else
        {
            //m_listEvents = GetComponent<FakeGameManager>().GetEvents();
            m_listEvents = FakeGameManager._instance.GetEvents();

            //Select random event
            int i = Random.Range(0, m_listEvents.Count);

            //Add event at Panel
            m_launcherPanel.AddEventWidget(m_listEvents[i]);

            if (m_launcherPanel.ListEventWidget.Count < m_eventsNbMax)
            {
                m_isHandFull = false;
            }
            else
            {
                m_isHandFull = true;
            }

            //Reset Timer before getting a new card event
            m_newEventTimer = Time.deltaTime;
            //m_timer = m_newEventTimer;
        }
    }

    public void ThrowCurrentEvent()
    {
        if(m_eventToLaunch != null)
        {
            m_launcherPanel.RemoveWidget();

            m_launcherPanel.DisableCards();

            m_eventToLaunch.ThrowEvent();
            //m_launcherPanel.RemoveEventWidget();

            m_isHandFull = false;
        }
        else
        {
            Debug.Log("Aucune carte selectionnée !");
        }
        m_eventToLaunch = null;

        m_canAddEvent = true;

    }

    private IEnumerator ReloadEvent(float _reloadTime)
    {
        yield return new WaitForSeconds(_reloadTime);
        AddEvent();
    }
}
