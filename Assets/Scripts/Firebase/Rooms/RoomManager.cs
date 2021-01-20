using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

using AuthError = Firebase.Auth.AuthError;

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
    private Team m_tBlue = new Team(TeamColor.BLUE);
    private Team m_tRed = new Team(TeamColor.RED);
    private Team m_tSpectator = new Team(TeamColor.SPECTATOR);

    // Permet de stocker un callback
    private Action m_aCallBackPlayerJoin;
    /// <summary>
    /// Fonction pour ajouter un callback pour quand un joueur est ajouté
    /// </summary>
    public void AddCallBack(Action c) => m_aCallBackPlayerJoin = c;
    /// <summary>
    /// Supprime le callback pour ne pas lancer de function inutile
    /// </summary>
    public void RemoveCallBack() => m_aCallBackPlayerJoin = null;

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
        room.Child("teams").Child("blue").Child("players").ChildAdded += AddBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildAdded += AddRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildAdded += AddSpecPlayer;

        m_bCreator = true;
        // finir l'animation d'attente et lancer une notification de réussite

        Debug.Log("END");
        yield break;
    }

    public IEnumerator AddPlayerToTeamInDatabase(TeamColor teamColor, Action action = null)
    {
        DatabaseReference database = FireBaseManager.Instance.Database;
        //DatabaseReference room = database.Child("rooms").Child(m_sID);
        string team;

        switch (teamColor)
        {
            case TeamColor.BLUE:
                team = "blue";
                break;
            case TeamColor.RED:
                team = "red";
                break;
            default:
                team = "spectator";
                break;
        }

        Task playerAdding = database.Child("rooms").Child(m_sID).Child("teams").Child(team).Child("players").Child(UserManager.Instance.getUser().UserId).SetValueAsync(UserManager.Instance.getUser().DisplayName).ContinueWith(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.Log("Erreur dans la création de la salle");
                return;
            }
        });
        while (!playerAdding.IsCompleted) yield return null;

        // Ajouter les callbacks pour la gestions

        DatabaseReference room = database.Child("rooms").Child(m_sID).Child("teams");

        room.Child("blue").Child("players").ChildAdded += AddBluePlayer;
        room.Child("red").Child("players").ChildAdded += AddRedPlayer;
        room.Child("spectator").Child("players").ChildAdded += AddSpecPlayer;

        room.Child("moveplayer").ChildAdded += PlayerChangeTeam;

        action();
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
            t.AddPlayer(new PlayerOnTeam(data.Key));
        }

        m_aCallBackPlayerJoin();
    }

    public IEnumerator ConnectToRoom()
    {
        yield break;
    }

    public IEnumerator CloseAndRemoveRoom() {
        Task rm = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).RemoveValueAsync();
        while (!rm.IsCompleted) yield return null;

        NotificationsManager.Instance.AddNotification("Salle Manager", "Suppression de la salle créé");
    }

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

    public void PlayerChangeTeam(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("PlayerChangeTeam: " + args.DatabaseError.Message);
            return;
        }

        DataSnapshot player = args.Snapshot;

        string currentTeam = player.Child("current").Value.ToString();
        string newTeam = player.Child("new").Value.ToString();

        if (newTeam == "" || currentTeam == "") {
            NotificationsManager.Instance.AddNotification("Erreur", "Impossible de changer d'équipe");
            return;
        }

        if (newTeam == "spectator") {
            StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
        }
        else if (newTeam == "red") {
            if (m_tRed.AllPlayer().Count < m_iMaxPlayer / 2) {
                StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
            }
        }
        else if (newTeam == "blue") {
            if (m_tBlue.AllPlayer().Count < m_iMaxPlayer / 2) {
                StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
            }
        }
        else
            NotificationsManager.Instance.AddNotification("Erreur", "Impossible de changer d'équipe");
    }

    private IEnumerator TeamToTeam(string team, DataSnapshot player, string newTeam) {
        DatabaseReference d = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).Child("teams");

        Task t = d.Child(team).Child("players").Child(player.Key).RemoveValueAsync();
        while (!t.IsCompleted) yield return null;

        Task t2 = d.Child(newTeam).Child("players").Child(player.Key).SetRawJsonValueAsync("");
        while (!t2.IsCompleted) yield return null;

        if (t2.IsFaulted || t2.IsCanceled) {
            AuthError error = FireBaseManager.GetAuthError(t2.Exception);

            switch (error) {
                case Firebase.Auth.AuthError.Failure:
                    NotificationsManager.Instance.AddNotification("", "");
                break;
            }
        } 
    }

    public void SetCallBackForAllPlayer() {

    }

    /// <summary>
    /// Va supprimer la salle créé
    /// </summary>
    ~RoomManager() {
        CloseAndRemoveRoom();
    }

    public Team[] getTeams()
    {
        return new Team[] { m_tRed, m_tBlue, m_tSpectator };
    }
}