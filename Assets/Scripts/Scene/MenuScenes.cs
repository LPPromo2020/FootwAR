using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScenes : MonoBehaviour
{
    [SerializeField] GameObject playerInList;
    [SerializeField] GameObject redTeam;
    [SerializeField] GameObject blueTeam;
    [SerializeField] GameObject specTeam;

    public void OnClick(string sSceneName)
    {
        SceneLoader.LoadScene(sSceneName);
    }

    public void AddPlayerBlue()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(Team.TeamColor.BLUE));
        UpdateUI();
    }
    
    public void AddPlayerRed()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(Team.TeamColor.RED));
        UpdateUI();
    }
    
    public void AddPlayerSpec()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(Team.TeamColor.SPECTATOR));
        UpdateUI();
    }

    public void UpdateUI()
    {
        Team[] teams = RoomManager.Instance.getTeams();
        GameObject instance;
        foreach (Player player in teams[0].AllPlayer())
        {
            instance = Instantiate(playerInList, redTeam.transform);
            instance.transform.GetChild(0).GetComponent<Text>().text = player.Guid;
        }
        foreach (Player player in teams[1].AllPlayer())
        {
            instance = Instantiate(playerInList, blueTeam.transform);
            instance.transform.GetChild(0).GetComponent<Text>().text = player.Guid;
        }
        foreach (Player player in teams[2].AllPlayer())
        {
            instance = Instantiate(playerInList, specTeam.transform);
            instance.transform.GetChild(0).GetComponent<Text>().text = player.Guid;
        }
    }
}