using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    private bool m_bCreator = false;
    private string m_sID;
    private int m_iMaxPlayer;
    private int m_iPlayerConnected = 0;
    private string m_sPassword;
    private string m_sNameRoom;
    private float m_fStartTime;

    // Ajouter les équipes
    private Team m_tBlue = new Team(Team.TeamColor.BLUE);
    private Team m_tRed = new Team(Team.TeamColor.RED);
    private Team m_tSpectator = new Team(Team.TeamColor.SPECTATOR);

    // TODO (Romumu): Ajouter la liste de bonus

    public IEnumerator CreateRoom(string name, string password, int maxPlayer, float startTime)
    {
        if (name == "")
        {
            // Ne peut avoir un nom de salle vide
            yield break;
        }

        if (maxPlayer < 4)
        {
            // 4 joueurs sont le minimum requis
            yield break;
        }

        // récupérer la liste des salles et voir si une posséde déjà cette GUID

        // lancer une animation d'attente

        DatabaseReference database = FireBaseManager.Instance.Database;

        Task<DataSnapshot> recupRoom = database.Child("rooms").GetValueAsync();
        while (!recupRoom.IsCompleted) yield return null;

        do m_sID = Guid.NewGuid().ToString();
        while (GuidIsAlreadyUsed(recupRoom.Result, m_sID));

        m_sNameRoom = name;
        m_sPassword = password;
        m_iMaxPlayer = maxPlayer;
        m_fStartTime = startTime;

        // Creation de la salle

        Task roomCreation = database.Child("rooms").Child(m_sID).SetRawJsonValueAsync(ToJson()).ContinueWith(result => {
            if (result.IsFaulted || result.IsCanceled) {
                Debug.Log("Erreur dans la création de la salle");
                return;
            }
        });

        while (!roomCreation.IsCompleted) yield return null;

        // Ajouter les callbacks pour la gestions

        DatabaseReference room = database.Child("rooms").Child(m_sID);

        // room.Child("players").ValueChanged += PlayersCountChange;
        room.Child("teams").Child("blue").ChildAdded += AddBluePlayer;
        room.Child("teams").Child("red").ChildAdded += AddRedPlayer;
        room.Child("teams").Child("spectator").ChildAdded += AddSpecPlayer;

        m_bCreator = true;
        // finir l'animation d'attente et lancer une notification de réussite

        Debug.Log("END");
        yield break;
    }

    public IEnumerator AddPlayerToTeamInDatabase(Team.TeamColor teamColor)
    {
        //DatabaseReference database = FireBaseManager.Instance.Database;
        ////DatabaseReference room = database.Child("rooms").Child(m_sID);
        //string team;

        //switch (teamColor)
        //{
        //    case Team.TeamColor.BLUE:
        //        team = "blue";
        //        break;
        //    case Team.TeamColor.RED:
        //        team = "red";
        //        break;
        //    default:
        //        team = "spectator";
        //        break;
        //}

        //Task playerAdding = database.Child("rooms").Child(m_sID).Child("teams").Child(team).Child(UserManager.Instance.getUser().UserId).SetValueAsync(UserManager.Instance.getUser().DisplayName).ContinueWith(result => {
        //    if (result.IsFaulted || result.IsCanceled)
        //    {
        //        Debug.Log("Erreur dans la création de la salle");
        //        return;
        //    }
        //});
        //while (!playerAdding.IsCompleted) yield return null;
        yield break;
    }

    private void AddBluePlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        NewMemberOnTeam(args, m_tBlue);
    }

    private void AddRedPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        NewMemberOnTeam(args, m_tRed);
    }

    private void AddSpecPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        NewMemberOnTeam(args, m_tSpectator);
    }

    private void NewMemberOnTeam(ChildChangedEventArgs args, Team t) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        DataSnapshot data = args.Snapshot;        
        if (!t.HaveThisPlayer(data.Key)) {
            t.AddPlayer(new Player(data.Key, new GameObject().transform));
        }

        Debug.Log($"Add Player: {data.Key} on team {t.Color()}");
        // Ajouter une personne dans le visuel
    }

    public IEnumerator ConnectToRoom()
    {
        yield break;
    }

    //private string ToJsonPlayer()
    //{
    //    string json = "{";
    //    // Player informations
    //    json += $"\"{UserManager.Instance.getUser().DisplayName}\": \"{UserManager.Instance.getUser().UserId}\"";
    //    return json + '}';
    //}

    private string ToJson() {
        string json = "{";
        // Ball position
        json += "\"ballprs\":\"x¤y¤z¤x¤y¤z¤x¤y¤z\",";
        // Room informations
        json += $"\"roomName\":\"{m_sNameRoom}\", \"password\":\"{m_sPassword}\", \"players\":{m_iMaxPlayer}, \"startTime\":{m_fStartTime},";
        // Ajout des équipes
        json += "\"teams\": {\"blue\": " + m_tBlue.ToJson() + ", \"red\":" + m_tRed.ToJson() + ", \"spectator\":" + m_tSpectator.ToJson() + "}";
        return json + '}';
    }

    private void PlayersCountChange(object sender, ValueChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("PlayerCountChange: " + args.DatabaseError.Message);
            return;
        }
        // m_iPlayerConnected = (int) args.Snapshot.GetValue(true);
    }

    private bool GuidIsAlreadyUsed(DataSnapshot rooms, string guid)
    {
        foreach (DataSnapshot data in rooms.Children)
        {
            if (data.Key == guid) return true;
        }

        return false;
    }

    ~RoomManager() {
        // FireBaseManagerEssaie.Instance.Database.Child("rooms").Child(m_sID).RemoveValueAsync();
    }

    public Team[] getTeams()
    {
        return new Team[] { m_tRed, m_tBlue, m_tSpectator };
    }
}

public class Team {
    public enum TeamColor { BLUE, RED, SPECTATOR };

    private List<Player> m_lstpList = new List<Player>();
    private TeamColor m_tcColor;

    public Team(TeamColor color) { m_tcColor = color; }

    public void AddPlayer(Player p) => m_lstpList.Add(p);
    public List<Player> AllPlayer() => m_lstpList;
    public TeamColor Color() => m_tcColor;

    public bool HaveThisPlayer(string guid) {
        bool ret = false;
        m_lstpList.ForEach(p => { if (p.Guid == guid) ret = true; });
        return ret;
    }

    public string ToJson() {
        string json = "{";
        m_lstpList.ForEach(player => json += player.ToJson() + ',');
        return json + "\"score\":0}";
    }
}

public struct Player {
    public string Guid;
    public Transform MyTransform;

    public Player(string guid, Transform t) {
        Guid = guid;
        MyTransform = t;
    }

    public string ToJson() {
        return $"\"{Guid}\":" + "{\"prs\":\"x¤y¤z¤x¤y¤z¤x¤y¤z\"}";
    }
}