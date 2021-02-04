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
    private string m_sNameRoom = string.Empty;
    private float m_fStartTime;

    // Getter
    public string GUID => m_sID;
    public bool IsCreator => m_bCreator;

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
    /// <param name="c"></param>
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
    /// <param name="c">Function</param>
    public void SetCallBackPlayerQuit(Action c) => m_aCallBackPlayerLeftTeam = c;
    /// <summary>
    /// Remove callback
    /// </summary>
    public void RemoveCallBackPlayerQuit() => m_aCallBackPlayerLeftTeam = null;

    // Callback for end Connection
    private Action m_aCallBackConnectRoom;
    /// <summary>
    /// Fonction pour ajouter un callback pour quand on se connecte à une salle
    /// </summary>
    /// <param name="c"></param>
    public void SetCallBackRoomConnect(Action c) => m_aCallBackConnectRoom = c;
    /// <summary>
    /// Remove callback
    /// </summary>
    public void RemoveCallBackRoomConnect() => m_aCallBackConnectRoom = null;
    
    /// <summary>
    /// Create Room on Reatime database
    /// </summary>
    /// <param name="name">Nom de la salle</param>
    /// <param name="password">Mot de passe</param>
    /// <param name="maxPlayer">Nombre maximum de joueur</param>
    /// <param name="startTime">Le temps de jeu</param>
    /// <param name="callBack"></param>
    /// <returns></returns>
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

        DatabaseReference database = FireBaseManager.Instance.Database;

        // Get all room guid
        Task<DataSnapshot> recupRoom = database.Child("rooms").GetValueAsync();
        while (!recupRoom.IsCompleted) yield return null;
        
        // Créer un GUID pour la salle et vérifie que ce GUID n'est pas déjà donné à une autre salle
        // sinon il relance la génération jusqu'à en avoir une qui va bien
        do m_sID = Guid.NewGuid().ToString();
        while (GuidIsAlreadyUsed(recupRoom.Result, m_sID));

        // Save information of the room on variables
        m_sNameRoom = name;
        m_sPassword = password;
        m_iMaxPlayer = maxPlayer;
        m_fStartTime = startTime;

        // Creation de la salle
        Task roomCreation = database.Child("rooms").Child(m_sID).SetRawJsonValueAsync(ToJson()).ContinueWith(result => {
            if (result.IsFaulted || result.IsCanceled) {
                Debug.Log("Erreur dans la création de la salle");
            }
        });

        // wait while the room is not create
        while (!roomCreation.IsCompleted) yield return null;
        
        // get room reference of Database
        DatabaseReference room = database.Child("rooms").Child(m_sID);

        // Use function to set callback
        SetCallBackDatabase(room);
        
        // Add Callback when player want to move
        room.Child("teams").Child("moveplayer").ChildAdded += PlayerChangeTeam;
        room.Child("teams").Child("moveplayer").ChildChanged += PlayerChangeTeam;

        // Set creator to true
        m_bCreator = true;

        // Invoke callback
        callBack?.Invoke();
    }

    /// <summary>
    /// His used to set callback of the database
    /// </summary>
    /// <param name="room"></param>
    private void SetCallBackDatabase(DatabaseReference room) {
        // Add player join team callback
        room.Child("teams").Child("blue").Child("players").ChildAdded += AddBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildAdded += AddRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildAdded += AddSpecPlayer;

        // Callback when player left team
        room.Child("teams").Child("blue").Child("players").ChildRemoved += RemoveBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildRemoved += RemoveRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildRemoved += RemoveSpecPlayer;
    }

    #region Member Join Team
    /// <summary>
    /// Function for blue team when player join
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AddBluePlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }
        
        if (args.Snapshot.Key == UserManager.Instance.getUser().UserId)
            m_sCurrentTeam = "blue";

        NewMemberOnTeam(args, m_tBlue);
    }

    /// <summary>
    /// Function for red team when player join
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AddRedPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }
        
        if (args.Snapshot.Key == UserManager.Instance.getUser().UserId)
            m_sCurrentTeam = "red";

        NewMemberOnTeam(args, m_tRed);
    }

    /// <summary>
    /// Function for spectator team when player join
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AddSpecPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Key == UserManager.Instance.getUser().UserId)
            m_sCurrentTeam = "spectator";
        
        NewMemberOnTeam(args, m_tSpectator);
    }

    /// <summary>
    /// Basic function call when a player is add on a team
    /// </summary>
    /// <param name="args"></param>
    /// <param name="t"></param>
    private void NewMemberOnTeam(ChildChangedEventArgs args, Team t) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        DataSnapshot data = args.Snapshot;
        // checked if this does not have this player
        if (!t.HaveThisPlayer(data.Key)) {
            t.AddPlayer(new PlayerOnTeam(data.Key));
            
            Debug.Log($"Add player {t}: {data.Key}");
        }

        // invoke the callback
        m_aCallBackPlayerJoin?.Invoke();
    }
    #endregion
    
    #region Member Quit Team
    /// <summary>
    /// Function utilisant MemberQuitTeam mais utilisé pour l'équipe des bleus
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void RemoveBluePlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        MemberQuitTeam(args, m_tBlue);
    }

    /// <summary>
    /// Function utilisant MemberQuitTeam mais utilisé pour l'équipe des rouges
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void RemoveRedPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        MemberQuitTeam(args, m_tRed);
    }

    /// <summary>
    /// Function utilisant MemberQuitTeam mais utilisé pour l'équipe des spectateurs
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void RemoveSpecPlayer(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        MemberQuitTeam(args, m_tSpectator);
    }
    
    /// <summary>
    /// Function Basique quand une personne quitte une équipe
    /// </summary>
    /// <param name="args"></param>
    /// <param name="t"></param>
    private void MemberQuitTeam(ChildChangedEventArgs args, Team t) {
        // Check if we have a error
        if (args.DatabaseError != null) {
            Debug.LogError("NewMemberOnTeam: " + args.DatabaseError.Message);
            return;
        }

        DataSnapshot data = args.Snapshot;
        // Checked if player is on the team      
        if (t.HaveThisPlayer(data.Key)) {
            t.RemovePlayer(data.Key);

            Debug.Log($"Remove player: {data.Key}");
        }

        // Invoke Callback
        m_aCallBackPlayerLeftTeam?.Invoke();
    }
    #endregion

    /// <summary>
    /// Connect to a room with the GUID
    /// </summary>
    /// <param name="roomGuid">Room Guid</param>
    /// <returns></returns>
    public IEnumerator ConnectToRoom(string roomGuid, Action<bool> callback = null) {
        // get user GUID
        string playerGuid = UserManager.Instance.getUser().UserId;
        // Create json information
        string json = '{' + $"\"current\": \"\",\"new\": \"spectator\"" + '}';
        // start function to create
        Task t = FireBaseManager.Instance.Database.Child("rooms").Child(roomGuid).Child("teams").Child("moveplayer").Child(playerGuid).SetRawJsonValueAsync(json);
        while (!t.IsCompleted) yield return null;

        // get spectator reference
        DatabaseReference spectatorPlayers = FireBaseManager.Instance.Database
                .Child("rooms").Child(roomGuid).Child("teams").Child("spectator").Child("players");
        
        // set id room
        m_sID = roomGuid;

        // Add Callback
        spectatorPlayers.ChildAdded += FirstCallBack;
        
        // Callback
        callback?.Invoke(!t.IsCanceled && !t.IsFaulted);
    }

    /// <summary>
    /// First function for connect to room
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void FirstCallBack(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            return;
        }

        // get data
        DataSnapshot data = args.Snapshot;

        // if is not the current player left function 
        if (data.Key != UserManager.Instance.getUser().UserId) return;

        // get spectator reference and remove callback
        DatabaseReference room = FireBaseManager.Instance.Database
                .Child("rooms").Child(m_sID);
        
        // remove first callback
        room.Child("teams").Child("spectator").Child("players").ChildAdded -= FirstCallBack;

        // End Connection
        EndConnection(room);
    }

    /// <summary>
    /// Function when connection to database is finish
    /// </summary>
    /// <param name="room">room database reference</param>
    private void EndConnection(DatabaseReference room) {
        // Set Callback for this room
        SetCallBackDatabase(room);

        // Invoke callback
        m_aCallBackConnectRoom?.Invoke();
    }

    /// <summary>
    /// Disconnect player
    /// </summary>
    public void DisconnectToRoom() {
        if (m_sID == "") return;

        DatabaseReference refDatabase = FireBaseManager.Instance.Database;
        string playerID = UserManager.Instance.getUser().UserId;
        
        refDatabase.Child("rooms").Child(m_sID).Child("teams")
            .Child(m_sCurrentTeam).Child("players").Child(playerID).RemoveValueAsync();
    }

    /// <summary>
    /// Va permettre de supprimer la salle et de supprimer touts les callbacks
    /// donnée
    /// </summary>
    /// <returns></returns>
    public IEnumerator CloseAndRemoveRoom(Action<bool> callback = null) {
        // if not the creator no delete Room
        if (!m_bCreator || m_sNameRoom == string.Empty) yield break;
        
        // Get Database reference
        DatabaseReference database = FireBaseManager.Instance.Database;

        // Get room database reference
        DatabaseReference room = database.Child("rooms").Child(m_sID);

        // Remove all callback player join team
        room.Child("teams").Child("blue").Child("players").ChildAdded -= AddBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildAdded -= AddRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildAdded -= AddSpecPlayer;

        // Remove callback when player quit team
        room.Child("teams").Child("blue").Child("players").ChildRemoved -= RemoveBluePlayer;
        room.Child("teams").Child("red").Child("players").ChildRemoved -= RemoveRedPlayer;
        room.Child("teams").Child("spectator").Child("players").ChildRemoved -= RemoveSpecPlayer;

        if (m_bCreator) {
            // Remove callback when player want to change team
            room.Child("teams").Child("moveplayer").ChildAdded -= PlayerChangeTeam;
            room.Child("teams").Child("moveplayer").ChildChanged -= PlayerChangeTeam;
        }

        // Remove the room on the database
        Task rm = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).SetRawJsonValueAsync("{}");
        while (!rm.IsCompleted) yield return null;
        
        // Reset room name
        m_sNameRoom = string.Empty;

        // send notification
        NotificationsManager.Instance.AddNotification("Salle Manager", "Suppression de la salle");
        
        // CallBack and if delete is not canceled or faulted
        // the remove is good
        callback?.Invoke(!rm.IsCanceled && !rm.IsFaulted);
    }

    /// <summary>
    /// Remove player of All team
    /// </summary>
    public void RemoveTeamPlayer() {
        m_tBlue.ClearTeam();
        m_tRed.ClearTeam();
        m_tSpectator.ClearTeam();
    }

    /// <summary>
    /// Permet de mettre toute les informations au format JSON
    /// et le retourne
    /// 
    /// Utilisé dans la création de la salle, en association avec la base de donnée firebase
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// return true when he found a rooms with the same GUID
    /// </summary>
    /// <param name="rooms">List of room</param>
    /// <param name="guid">Guid to compare</param>
    /// <returns></returns>
    private bool GuidIsAlreadyUsed(DataSnapshot rooms, string guid) {
        return rooms.Children.Any(data => data.Key == guid);
    }

    /// <summary>
    /// Add the player on the database for say to database he want
    /// to change team
    /// </summary>
    /// <param name="newTeam"></param>
    /// <param name="guidRoom"></param>
    /// <returns></returns>
    public IEnumerator ChangeTeam(string newTeam, string guidRoom) {
        // if newTeam have the same name of the current quit function
        if (m_sCurrentTeam == newTeam) yield break;

        // Get Player GUID
        string playerGuid = UserManager.Instance.getUser().UserId;
        // create data to change team
        string json = '{' + $"\"current\": \"{CurrentTeam}\",\"new\": \"{newTeam}\"" + '}';

        // Start to change team
        Task t = FireBaseManager.Instance.Database.Child("rooms").Child(guidRoom).Child("teams").Child("moveplayer").Child(playerGuid).SetRawJsonValueAsync(json);
        while (!t.IsCompleted) yield return null;
    }

    /// <summary>
    /// Basic function call when a player want to change team
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void PlayerChangeTeam(object sender, ChildChangedEventArgs args) {
        // if the current RoomManager is not the creator
        // quit the function
        if (!m_bCreator) return;

        // checked if we have a error
        if (args.DatabaseError != null) {
            Debug.LogError("PlayerChangeTeam: " + args.DatabaseError.Message);
            return;
        }

        // get snapshot data
        DataSnapshot player = args.Snapshot;
        
        // get current team name
        string currentTeam = player.Child("current").Value.ToString();
        // get new team name
        string newTeam = player.Child("new").Value.ToString();

        // if the new team is spectator move player on the team
        if (newTeam == "spectator") {
            StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
        }
        else if (newTeam == "red") {
            // verif if red have the place to get player
            if (m_tRed.AllPlayer.Count < m_iMaxPlayer / 2) {
                StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
            }
        }
        else if (newTeam == "blue") {
            // verif if blue have place to get player
            if (m_tBlue.AllPlayer.Count < m_iMaxPlayer / 2) {
                StartCoroutine(TeamToTeam(currentTeam, player, newTeam));
            }
        }
        else {
            // Send notification
            NotificationsManager.Instance.AddNotification("Erreur", "Impossible de changer d'équipe");
            // remove player request to change team
            RemovePlayerRequest(player.Key);
        }

        Debug.Log(player.Key);
    }

    /// <summary>
    /// Fonction qui change le joueur de l'équipe initiale vers la nouvelle équipe
    /// </summary>
    /// <param name="team">Current team name of player</param>
    /// <param name="player">Player GUID</param>
    /// <param name="newTeam">The new team name</param>
    /// <returns></returns>
    private IEnumerator TeamToTeam(string team, DataSnapshot player, string newTeam) {
        // get request
        DatabaseReference d = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).Child("teams");
        
        // si le nom de l'équipe est vide ne lance par la
        if (team != string.Empty) {
            Task t = d.Child(team).Child("players").Child(player.Key).RemoveValueAsync();
            while (!t.IsCompleted) yield return null;
        }

        // rajoute le joueur dans la nouvelle équipe
        Task t2 = d.Child(newTeam).Child("players").Child(player.Key).Child("prs").SetValueAsync("x¤y¤z¤x¤y¤z¤w¤x¤y¤z");
        while (!t2.IsCompleted) yield return null;

        // regarde si une erreur est survenue
        if (t2.IsFaulted || t2.IsCanceled) {
            AuthError error = FireBaseManager.GetAuthError(t2.Exception);
            
            if (error == AuthError.Failure) {
                NotificationsManager.Instance.AddNotification("Erreur", "N'a pas pus Changer les valeurs");
            }
        }

        // change l'équipe courante si le joueur déplacé est le joueur connécté
        if (player.Key == UserManager.Instance.getUser().UserId) {
            m_sCurrentTeam = newTeam;
        }

        // remove request
        RemovePlayerRequest(player.Key);
    }

    /// <summary>
    /// Function to remove
    /// </summary>
    /// <param name="playerGuid"></param>
    private void RemovePlayerRequest(string playerGuid) {
        DatabaseReference moveplayers = FireBaseManager.Instance.Database.Child("rooms").Child(m_sID).Child("teams").Child("moveplayer");
        moveplayers.Child(playerGuid).RemoveValueAsync();
    }

    /// <summary>
    /// Call when application quit
    /// On remove Room only is creator and is a room
    /// is create
    /// </summary>
    private void OnApplicationQuit() {
        StartCoroutine(CloseAndRemoveRoom());
    }

    /// <summary>
    /// Get all teams on array
    /// Red - Blue - Spectator
    /// </summary>
    /// <returns></returns>
    public Team[] getTeams()
    {
        return new [] { m_tRed, m_tBlue, m_tSpectator };
    }
}