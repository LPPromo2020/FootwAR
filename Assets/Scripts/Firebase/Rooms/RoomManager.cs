using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

using AuthError = Firebase.Auth.AuthError;

/// <summary>
/// Classe permettant de faire la création d'une salle
/// </summary>
public class RoomManager : Singleton<RoomManager>
{
    // Les informations de la salle
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

    private string m_sCurrentTeam = "";
    public string CurrentTeam => m_sCurrentTeam;

    // Permet de stocker un callback
    private Action m_aCallBackPlayerJoin;
    /// <summary>
    /// Fonction pour ajouter un callback pour quand un joueur est ajouté
    /// </summary>
    public void SetCallBackPlayerAdd(Action c) => m_aCallBackPlayerJoin = c;
    /// <summary>
    /// Supprime le callback pour ne pas lancer de function inutile
    /// </summary>
    public void RemoveCallBackPlayerAdd() => m_aCallBackPlayerJoin = null;

    // Callback pour quand une personne quitte une équipe
    private Action m_aCallBackPlayerLeftTeam;
    /// <summary>
    /// Set callback when player quit room
    /// </summary>
    /// <param name="c"></param>
    public void SetCallBackPlayerQuit(Action c) => m_aCallBackPlayerLeftTeam = c;
    /// <summary>
    /// Remove callback
    /// </summary>
    public void RemoveCallBackPlayerQuit() => m_aCallBackPlayerLeftTeam = null;
    
    public IEnumerator CreateRoom(string name, string password, int maxPlayer, float startTime, Action callBack = null)
    {
        if (name == "")
        {
            // Ne peut avoir un nom de salle vide
            NotificationsManager.Instance.AddNotification("Salle", "Impossible d'initialiser une salle sans nom");
            yield break;
        }

        if (maxPlayer < 4)
        {
            // 4 joueurs sont le minimum requis
            NotificationsManager.Instance.AddNotification("Salle", "nombre de joueur max en dessous de la limite autorisé");
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
        
        DatabaseReference room = database.Child("rooms").Child(m_sID);

        // room.Child("players").ValueChanged += PlayersCountChange;
        room.Child("teams").Child("blue").Child("players").ChildAdded += AddBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildAdded += AddRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildAdded += AddSpecPlayer;

        room.Child("teams").Child("blue").Child("players").ChildRemoved += RemoveBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildRemoved += RemoveRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildRemoved += RemoveSpecPlayer;
        
        room.Child("teams").Child("moveplayer").ChildAdded += PlayerChangeTeam;
        room.Child("teams").Child("moveplayer").ChildChanged += PlayerChangeTeam;

        m_bCreator = true;
        // finir l'animation d'attente et lancer une notification de réussite
        callBack?.Invoke();
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
            if (result.IsFaulted || result.IsCanceled) {
                Debug.Log("Erreur dans la création de la salle");
            }
        });
        while (!playerAdding.IsCompleted) yield return null;

        // Ajouter les callbacks pour la gestions

        DatabaseReference room = database.Child("rooms").Child(m_sID).Child("teams");

        room.Child("blue").Child("players").ChildAdded += AddBluePlayer;
        room.Child("red").Child("players").ChildAdded += AddRedPlayer;
        room.Child("spectator").Child("players").ChildAdded += AddSpecPlayer;

        room.Child("moveplayer").ChildAdded += PlayerChangeTeam;
        room.Child("moveplayer").ChildChanged += PlayerChangeTeam;

        action?.Invoke();
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

        m_aCallBackPlayerJoin?.Invoke();
    }
    
    private void RemoveBluePlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        MemberQuitTeam(args, m_tBlue);
    }

    private void RemoveRedPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        MemberQuitTeam(args, m_tRed);
    }

    private void RemoveSpecPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        MemberQuitTeam(args, m_tSpectator);
    }
    
    private void MemberQuitTeam(ChildChangedEventArgs args, Team t) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        DataSnapshot data = args.Snapshot;        
        if (t.HaveThisPlayer(data.Key)) {
            t.RemovePlayer(data.Key);
        }

        m_aCallBackPlayerLeftTeam?.Invoke();
    }

    public IEnumerator ConnectToRoom()
    {
        yield break;
    }

    private IEnumerator CloseAndRemoveRoom() {
        Task rm = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).RemoveValueAsync();
        while (!rm.IsCompleted) yield return null;

        NotificationsManager.Instance.AddNotification("Salle Manager", "Suppression de la salle créé");
    }

    private string ToJson() {
        string json = "{";
        // Ball position
        json += "\"ballprs\":\"x¤y¤z¤x¤y¤z¤x¤y¤z\",\"isStart\":false,";
        // Room informations
        json += $"\"roomName\":\"{m_sNameRoom}\", \"password\":\"{m_sPassword}\", \"players\":{m_iMaxPlayer}, \"startTime\":{m_fStartTime},";
        // Ajout des équipes
        json += "\"teams\": {\"blue\": " + m_tBlue.ToJson() + ", \"red\":" + m_tRed.ToJson() + ", \"spectator\":" + m_tSpectator.ToJson() + ", \"moveplayer\":\"\"}";
        return json + '}';
    }

    private void PlayersCountChange(object sender, ValueChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("PlayerCountChange: " + args.DatabaseError.Message);
            return;
        }
        // m_iPlayerConnected = (int) args.Snapshot.GetValue(true);
    }

    private bool GuidIsAlreadyUsed(DataSnapshot rooms, string guid) {
        return rooms.Children.Any(data => data.Key == guid);
    }

    public IEnumerator ChangeTeam(string newTeam) {
        string playerGuid = UserManager.Instance.getUser().UserId;
        string json = '{' + $"\"current\": \"{CurrentTeam}\",\"new\": \"{newTeam}\"" + '}';
        Task t = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).Child("teams").Child("moveplayer").Child(playerGuid).SetRawJsonValueAsync(json);
        while (!t.IsCompleted) yield return null;
    }

    private void PlayerChangeTeam(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("PlayerChangeTeam: " + args.DatabaseError.Message);
            return;
        }

        DataSnapshot player = args.Snapshot;
        
        string currentTeam = player.Child("current").Value.ToString();
        string newTeam = player.Child("new").Value.ToString();

        if (newTeam == "spectator") {
            StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
        }
        else if (newTeam == "red") {
            if (m_tRed.AllPlayer.Count < m_iMaxPlayer / 2) {
                StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
            }
        }
        else if (newTeam == "blue") {
            if (m_tBlue.AllPlayer.Count < m_iMaxPlayer / 2) {
                StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
            }
        }
        else
            NotificationsManager.Instance.AddNotification("Erreur", "Impossible de changer d'équipe");
        
        
    }

    private IEnumerator TeamToTeam(string team, DataSnapshot player, string newTeam) {
        DatabaseReference d = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).Child("teams");
        if (team != "") {
            Task t = d.Child(team).Child("players").Child(player.Key).RemoveValueAsync();
            while (!t.IsCompleted) yield return null;
        }

        Task t2 = d.Child(newTeam).Child("players").Child(player.Key).Child("prs").SetValueAsync("x¤y¤z¤x¤y¤z¤w¤x¤y¤z");
        while (!t2.IsCompleted) yield return null;

        if (t2.IsFaulted || t2.IsCanceled) {
            AuthError error = FireBaseManager.GetAuthError(t2.Exception);

            switch (error) {
                case Firebase.Auth.AuthError.Failure:
                    NotificationsManager.Instance.AddNotification("Erreur", "N'a pas pus Changer les valeurs");
                break;
            }
        }

        m_sCurrentTeam = newTeam;
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