using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script used on room gestion scene (Lobby)
/// </summary>
public class MenuScenes : MonoBehaviour
{
    [SerializeField] GameObject playerInList;
    [SerializeField] Transform redTeam;
    [SerializeField] Transform blueTeam;
    [SerializeField] Transform specTeam;

    /// <summary>
    /// Initialise all callback and refresh visual
    /// </summary>
    void Start()
    {
        RoomManager.Instance.SetCallBackPlayerAdd(UpdateUI);
        RoomManager.Instance.SetCallBackPlayerQuit(UpdateUI);
        AddPlayerSpec();
        UpdateUI();
    }

    /// <summary>
    /// Load cene
    /// </summary>
    /// <param name="sSceneName">Name scene</param>
    public void OnClick(string sSceneName)
    {
        SceneLoader.LoadScene(sSceneName);
    }

    /// <summary>
    /// Function to change team to blue
    /// </summary>
    public void AddPlayerBlue()
    {
        StartCoroutine(RoomManager.Instance.ChangeTeam("blue", RoomManager.Instance.GUID));
    }
    
    /// <summary>
    /// Function to change team to blue
    /// </summary>
    public void AddPlayerRed()
    {
        StartCoroutine(RoomManager.Instance.ChangeTeam("red", RoomManager.Instance.GUID));
    }
    
    /// <summary>
    /// Function to change team to spectator
    /// </summary>
    public void AddPlayerSpec()
    {
        StartCoroutine(RoomManager.Instance.ChangeTeam("spectator", RoomManager.Instance.GUID));
    }

    /// <summary>
    /// Function to update UI
    /// </summary>
    private void UpdateUI()
    {
        // remove all visual to all teams
        DestroyChildTeam(redTeam);
        DestroyChildTeam(blueTeam);
        DestroyChildTeam(specTeam);

        // get teams
        Team[] teams = RoomManager.Instance.getTeams();

        // Create all visual for all teams
        InstancePlayerInList(redTeam, teams[0]);
        InstancePlayerInList(blueTeam, teams[1]);
        InstancePlayerInList(specTeam, teams[2]);
    }

    /// <summary>
    /// Destroy all child of this parent
    /// </summary>
    /// <param name="team">Parent trasnform</param>
    private void DestroyChildTeam(Transform team) {
        for (int i = 0; i < team.childCount; i++)
            DestroyImmediate(team.GetChild(i).gameObject);
    }

    /// <summary>
    /// Add visual for all player to a team
    /// </summary>
    /// <param name="parent">Parent</param>
    /// <param name="team">Team</param>
    private void InstancePlayerInList(Transform parent, Team team) {
        foreach (PlayerOnTeam player in team.AllPlayer) {
            Transform instance = Instantiate(playerInList, parent).transform;
            instance.GetChild(0).GetComponent<Text>().text = player.Guid;
        }
    }

    /// <summary>
    /// Function for clear team of player and quit Team and
    /// remove room if is creator
    /// </summary>
    public void QuitRoom() {
        RoomManager.Instance.RemoveTeamPlayer();
        RoomManager.Instance.DisconnectToRoom();

        if (!RoomManager.Instance.IsCreator || RoomManager.Instance.GUID == string.Empty) return;
        StartCoroutine(RoomManager.Instance.CloseAndRemoveRoom(b => {
            if (b) SceneLoader.LoadScene("MainMenu");
        }));
    }

    /// <summary>
    /// Remove Callback
    /// </summary>
    /// <value></value>
    ~MenuScenes() {
        RoomManager.Instance.RemoveCallBackPlayerAdd();
        RoomManager.Instance.RemoveCallBackPlayerQuit();
    }
}
