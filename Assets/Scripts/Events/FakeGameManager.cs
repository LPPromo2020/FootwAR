using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGameManager : MonoBehaviour
{
    [SerializeField]
    private List<Events> m_listEvents = new List<Events>();

    [System.Serializable]//usefull?
    public enum m_enumEvents
    {
        bomb,
        wall,
        smoke
    }

    public List<Events> GetEvents()
    {
        return m_listEvents;
    }
}
