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

    [Header("Password Reference")]
    [SerializeField] private GameObject m_goPassword;
    [SerializeField] private Button m_bPassword;
    [SerializeField] private InputField m_ifPassword;

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

        m_ifSearchInput.onValueChanged.AddListener(roomName => StartCoroutine(Search(roomName)));
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
        if ((bool)room.Child("isStart").Value) return;

        GameObject go = Instantiate(m_goPrefab, m_tParent);
        string n = (string) room.Child("roomName").Value, guid = room.Key;

        if ((string) room.Child("password").Value == "") {
            DestroyImmediate(go.transform.GetChild(0).GetChild(0).gameObject);
        }

        go.transform.GetChild(1).GetComponent<Text>().text = n;
        go.transform.GetChild(2).GetComponent<Text>().text = room.Child("players").Value.ToString();

        Room r = new Room(n, guid, room.Child("password").Value.ToString(), go);
        m_listRooms.Add(r);

        Button button = go.GetComponent<Button>();
        button.onClick.AddListener(() => ConnectToRoom(r));
    }

    private void ConnectToRoom(Room room) {
        if (room.Password != "") {
            m_goPassword.SetActive(true);
            
            m_bPassword.onClick.RemoveAllListeners();
            m_bPassword.onClick.AddListener(() =>
                ConnectToRoomWithPassword(room)
            );
            return;
        }

        StartCoroutine(RoomManager.Instance.ConnectToRoom(room.Guid, b => {
            if (b) SceneLoader.LoadScene("Lobby");
        }));
    }

    /// <summary>
    /// Function to connect room with password
    /// </summary>
    /// <param name="room">Room Reference</param>
    private void ConnectToRoomWithPassword(Room room) {
        if (room.Password != m_ifPassword.text) {
            NotificationsManager.Instance.AddNotification("Erreur", "Mauvais mot de passe", 2);
            return;
        }

        Debug.Log(room.Guid);
        StartCoroutine(RoomManager.Instance.ConnectToRoom(room.Guid, b => {
            if (b) SceneLoader.LoadScene("Lobby");
        }));
    }

    private void RoomIsRemove(object sender, ChildChangedEventArgs args)
    {
        m_listRooms.ForEach(room => {
            if (room.Guid != args.Snapshot.Key) return;
            
            Destroy(room.View);
            m_listRooms.Remove(room);
        });
    }

    private void RoomIsCreate(object sender, ChildChangedEventArgs args)
    {
        CreateRoom(args.Snapshot);
    }

    /// <summary>
    /// Struct for save Room data
    /// </summary>
    private struct Room
    {
        private string m_sName;
        private string m_sGuid;
        private string m_sPassword;
        private GameObject m_goView;

        public string Name => m_sName;
        public string Guid => m_sGuid;
        public string Password => m_sPassword;
        public GameObject View => m_goView;

        public Room(string name, string guid, string password, GameObject view)
        {
            m_sName = name;
            m_sGuid = guid;
            m_sPassword = password;
            m_goView = view;
        }
    }
}
