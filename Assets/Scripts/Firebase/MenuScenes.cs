using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScenes : MonoBehaviour
{
    [SerializeField] GameObject playerInList;
    [SerializeField] Transform redTeam;
    [SerializeField] Transform blueTeam;
    [SerializeField] Transform specTeam;

    void Start()
    {
        AddPlayerSpec();
    }

    public void OnClick(string sSceneName)
    {
        SceneLoader.LoadScene(sSceneName);
    }

    public void AddPlayerBlue()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(Team.TeamColor.BLUE, UpdateUI));
    }
    
    public void AddPlayerRed()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(Team.TeamColor.RED, UpdateUI));
    }
    
    public void AddPlayerSpec()
    {
        StartCoroutine(RoomManager.Instance.AddPlayerToTeamInDatabase(Team.TeamColor.SPECTATOR, UpdateUI));
    }

    public void UpdateUI()
    {
        for (int i = 0; i < redTeam.childCount; i++)
        {
            Destroy(redTeam.GetChild(i).gameObject);
        }

        for (int i = 0; i < blueTeam.childCount; i++)
        {
            Destroy(blueTeam.GetChild(i).gameObject);
        }

        for (int i = 0; i < specTeam.childCount; i++)
        {
            Destroy(specTeam.GetChild(i).gameObject);
        }

        Team[] teams = RoomManager.Instance.getTeams();
        GameObject instance;

        foreach (Player player in teams[0].AllPlayer())
        {
            instance = Instantiate(playerInList, redTeam);
            instance.transform.GetChild(0).GetComponent<Text>().text = player.Guid;
        }

        foreach (Player player in teams[1].AllPlayer())
        {
            instance = Instantiate(playerInList, blueTeam);
            instance.transform.GetChild(0).GetComponent<Text>().text = player.Guid;
        }

        foreach (Player player in teams[2].AllPlayer())
        {
            instance = Instantiate(playerInList, specTeam);
            instance.transform.GetChild(0).GetComponent<Text>().text = player.Guid;
        }
    }
}