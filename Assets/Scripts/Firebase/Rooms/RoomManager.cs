using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    private string m_sID;
    public string Password;
    public string RoomName;

    public int PlayerNumber;

    public float StartTime = 240.0f;

    private List<int> m_lbBonusList = new List<int>();

    private Teams m_tTeams = new Teams();

    public void CreateRoom(string roomName, string password, int nbPlayer, float duration = 260)
    {
        RoomName = roomName;
        Password = password;
        PlayerNumber = nbPlayer;
        StartTime = duration;

        m_sID = Guid.NewGuid().ToString();

        StartCoroutine(CreateRoomOnFirebase());
    }

    private IEnumerator CreateRoomOnFirebase()
    {
        string jsonRoom = JsonUtility.ToJson(this);
        FireBaseManager.Instance.RealTimeDatabse().Child("rooms").Child(m_sID).SetRawJsonValueAsync(jsonRoom).ContinueWith(task =>
        {
            Debug.Log("End one");

            string jsonTeam = JsonUtility.ToJson(m_tTeams);
            FireBaseManager.Instance.RealTimeDatabse().Child("rooms").Child(m_sID).Child("Teams").SetRawJsonValueAsync(jsonTeam).ContinueWith(task2 =>
            {
                Debug.Log("End two");
            });
        });
        
        yield break;
        // Afficher que la salle de jeu est créé
    }

    public IEnumerator DeleteRoom()
    {
        yield break;
    }
}

[Serializable]
public class Teams
{
    private string m_sID;
    public Team blue;
    public Team red;
}

[Serializable]
public class Team
{
    public List<PRS> Players;

    public int Score = 0;
}

[Serializable]
public class PRS
{
    public string prs = "x¤y¤z¤x¤y¤z¤x¤y¤z";
}