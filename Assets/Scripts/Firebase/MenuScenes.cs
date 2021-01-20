﻿using UnityEngine;
using UnityEngine.UI;

public class MenuScenes : MonoBehaviour
{
    [SerializeField] GameObject playerInList;
    [SerializeField] Transform redTeam;
    [SerializeField] Transform blueTeam;
    [SerializeField] Transform specTeam;

    void Start()
    {
        RoomManager.Instance.AddCallBack(UpdateUI);
        AddPlayerSpec();
    }

    public void OnClick(string sSceneName)
    {
        SceneLoader.LoadScene(sSceneName);
    }

    public void AddPlayerBlue()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(TeamColor.BLUE, UpdateUI));
    }
    
    public void AddPlayerRed()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(TeamColor.RED, UpdateUI));
    }
    
    public void AddPlayerSpec()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(TeamColor.SPECTATOR, UpdateUI));
    }

    public void UpdateUI()
    {
        DestroyChildTeam(redTeam);
        DestroyChildTeam(blueTeam);
        DestroyChildTeam(specTeam);

        Team[] teams = RoomManager.Instance.getTeams();
        
        InstancePlayerInList(redTeam, teams[0]);
        InstancePlayerInList(blueTeam, teams[1]);
        InstancePlayerInList(specTeam, teams[2]);
    }

    private void DestroyChildTeam(Transform team) {
        for (int i = 0; i < team.childCount; i++)
            Destroy(team.GetChild(i).gameObject);
    }

    private void InstancePlayerInList(Transform parent, Team team) {
        Transform instance;
        foreach (PlayerOnTeam player in team.AllPlayer()) {
            instance = Instantiate(playerInList, parent).transform;
            instance.GetChild(0).GetComponent<Text>().text = player.Guid;
        }
    }

    ~MenuScenes() {
        RoomManager.Instance.RemoveCallBack();
    }
}
