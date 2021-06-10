using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class waitingRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiPlayerSceneIndex;  //main game scene index

    [SerializeField]
    private GameObject startButton;     //game start button, available for master client

    [SerializeField]
    private Transform playersContainer; //container for holding all the player listings items
    [SerializeField]
    private GameObject playerListingPrefab; //prefab for displayer each player in the room

    [SerializeField]
    private Text roomNameDisplay;   //display room name

    void ClearPlayerListings()
    {
        for (int i = playersContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainer.GetChild(i).gameObject);
        }
    }

    void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject tempListing = Instantiate(playerListingPrefab, playersContainer);
            Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
            //get playerListing component - Image to change color to its team color
            Image tempImage = tempListing.GetComponent<Image>();
            tempText.text = player.NickName;
            //try get player's team
            if (player.GetPhotonTeam() != null)
            {
                //set playerListing to team color
                if (player.GetPhotonTeam().Name == "Blue")
                {
                    tempImage.color = Color.blue;
                }
                else
                {
                    tempImage.color = Color.red;
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ClearPlayerListings();
        ListPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        //after join room successfully, do some initialization
        roomNameDisplay.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        //only master client can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
        //re-display current room player list
        ClearPlayerListings();
        ListPlayers();
        
        //already in room, and player joins team
        PhotonTeam[] availableTeam = PhotonTeamsManager.Instance.GetAvailableTeams();
        int[] teamMembersCount = new int[2];
        teamMembersCount[0] = PhotonTeamsManager.Instance.GetTeamMembersCount(availableTeam[0]);
        teamMembersCount[1] = PhotonTeamsManager.Instance.GetTeamMembersCount(availableTeam[1]);
        Debug.Log("Blue:" + teamMembersCount[0]);
        Debug.Log("Red:" + teamMembersCount[1]);
        if(teamMembersCount[0] > teamMembersCount[1])
        {
            Debug.Log("joined?"+PhotonNetwork.LocalPlayer.JoinTeam(teamCode:2));    //team code begin from 1
        }
        else if(teamMembersCount[0] < teamMembersCount[1])
        {
            Debug.Log("joined?"+PhotonNetwork.LocalPlayer.JoinTeam(teamCode:1));
        }
        else
        {
            Debug.Log("joined?"+PhotonNetwork.LocalPlayer.JoinTeam(teamCode:(byte)Random.Range(1,3)));
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("mainScene");
        }
    }

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("launch");
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick()
    {
        //local player leave team
        Debug.Log("leave?"+PhotonNetwork.LocalPlayer.LeaveCurrentTeam());
        //callback function be called by other clients to update playerListings in container
        //need to leave team first, then leave room. otherwise, leave team fail
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //after let player call JointTeam, must wait server actually set custom properties and broadcast to all client
        //after finishing setting team, update the playerListings' color in container
        ClearPlayerListings();
        ListPlayers();
    }
}
