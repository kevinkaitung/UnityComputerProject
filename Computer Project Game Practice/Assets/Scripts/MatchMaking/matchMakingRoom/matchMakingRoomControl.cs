using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class matchMakingRoomControl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiPlayerSceneIndex;  //main game scene index

    [SerializeField]
    private GameObject lobbyPanel;  //lobby panel, show available room list and create room button
    [SerializeField]
    private GameObject roomPanel;   //room panel, show in-room players

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
            tempText.text = player.NickName;
        }
    }

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;
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
        PhotonNetwork.JoinLobby();
    }

    public void BackOnClick()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }

    /*5/9 test*/
    public void backToWaitingRoomOnClick()
    {
        PhotonNetwork.LoadLevel("launch");
        roomPanel.SetActive(true);
        ClearPlayerListings();
        ListPlayers();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}
