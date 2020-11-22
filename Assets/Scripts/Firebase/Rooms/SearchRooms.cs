using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Threading.Tasks;

public class SearchRooms : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_goPrefab;
    [SerializeField] private Transform m_tParent;

    [Header("Settings")]
    [SerializeField] private InputField m_ifSearchInput;
    [SerializeField] private bool m_bPrintAllRooms = false;
    [Range(5, 30)]
    [SerializeField] private int m_iCountPrint = 10;
    [Range(10, 150)]
    [SerializeField] private int m_iSearchCountBeforeNewtFrame = 10;


    private DatabaseReference m_drRooms;
    private List<Room> m_listRooms = new List<Room>();

    private void Start()
    {
        m_drRooms = FireBaseManager.Instance.Database.Child("rooms");

        m_drRooms.ChildAdded += RoomIsCreate;
        m_drRooms.ChildRemoved += RoomIsRemove;

        StartCoroutine(GetAllRooms());

        m_ifSearchInput.onValueChanged.AddListener(roomName => StartCoroutine(Search(roomName)));
    }

    private IEnumerator GetAllRooms()
    {
        Task<DataSnapshot> getRooms = m_drRooms.GetValueAsync();
        while (!getRooms.IsCompleted) yield return null;

        DataSnapshot rooms = getRooms.Result;
        int count = m_iCountPrint;
        foreach (DataSnapshot room in getRooms.Result.Children)
        {
            CreateRoom(room);

            if (m_bPrintAllRooms && (count--) <= 0) yield break;

            yield return null;
        }
    }

    private IEnumerator Search(string roomName)
    {
        int count = m_iSearchCountBeforeNewtFrame;
        foreach (Room room in m_listRooms)
        {
            room.View.SetActive(room.Name.Contains(roomName));

            if ((count--) <= 0)
            {
                count = m_iSearchCountBeforeNewtFrame;
                yield return null;
            }
        }
    }

    private void CreateRoom(DataSnapshot room)
    {
        GameObject go = Instantiate(m_goPrefab, m_tParent);
        string n = (string) room.Child("roomName").Value, guid = room.Key;

        if ((string) room.Child("password").Value == "")
        {
            DestroyImmediate(go.transform.GetChild(0).GetChild(0).gameObject);
        }

        go.transform.GetChild(1).GetComponent<Text>().text = n;
        go.transform.GetChild(2).GetComponent<Text>().text = room.Child("players").Value.ToString();

        m_listRooms.Add(new Room(n, guid, go));
    }

    private void RoomIsRemove(object sender, ChildChangedEventArgs args)
    {

    }

    private void RoomIsCreate(object sender, ChildChangedEventArgs args)
    {

    }

    private struct Room
    {
        private string m_sName;
        private string m_sGuid;
        private GameObject m_goView;

        public string Name => m_sName;
        public string Guid => m_sGuid;
        public GameObject View => m_goView;

        public Room(string name, string guid, GameObject view)
        {
            m_sName = name;
            m_sGuid = guid;
            m_goView = view;
        }
    }
}
