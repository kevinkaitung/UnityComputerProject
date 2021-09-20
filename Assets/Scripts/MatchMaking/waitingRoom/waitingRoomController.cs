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
    private GameObject BlackPanel;
    
    [SerializeField]
    private int multiPlayerSceneIndex;  //main game scene index

    [SerializeField]
    private GameObject startButton;     //game start button, available for master client

    [SerializeField]
    private Transform playersContainerBlue; //container for holding all the blue player listings items
    [SerializeField]
    private Transform playersContainerRed;  //container for holding all the red player listings items
    [SerializeField]
    private GameObject playerListingPrefabBlue; //prefab for displayer each player in the room
    [SerializeField]
    private GameObject playerListingPrefabRed; //prefab for displayer each player in the room

    [SerializeField]
    private Text roomNameDisplay;   //display room name

    public Text showIfReadyStart;

    [SerializeField]
    private GameObject ChatroomButton;

    [SerializeField]
    private GameObject ChatPanel;

    [SerializeField]
    private GameObject CharacterButton;

    [SerializeField]
    private GameObject CharacterbackButton;

    [SerializeField]
    private GameObject CharacterPanel;

    bool Chatclicktime;
    bool Characterclicktime;

    void ClearPlayerListings()
    {
        for (int i = playersContainerBlue.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainerBlue.GetChild(i).gameObject);
        }
        for (int i = playersContainerRed.childCount - 1; i >= 0; i--)
        {
            Destroy(playersContainerRed.GetChild(i).gameObject);
        }
    }

    void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //try get player's team
            if (player.GetPhotonTeam() != null)
            {
                //set playerListing to team color
                if (player.GetPhotonTeam().Name == "Blue")
                {
                    GameObject tempListing = Instantiate(playerListingPrefabBlue, playersContainerBlue);
                    Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                    //get playerListing component - Image to change color to its team color
                    Image tempImage = tempListing.GetComponent<Image>();
                    tempText.text = player.NickName;
                    //tempImage.color = Color.blue;
                }
                else
                {
                    GameObject tempListing = Instantiate(playerListingPrefabRed, playersContainerRed);
                    Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                    //get playerListing component - Image to change color to its team color
                    Image tempImage = tempListing.GetComponent<Image>();
                    tempText.text = player.NickName;
                    //tempImage.color = Color.red;
                }
            }
        }
        int blueNum = PhotonTeamsManager.Instance.GetTeamMembersCount("Blue");
        int redNum = PhotonTeamsManager.Instance.GetTeamMembersCount("Red");
        if (((redNum - blueNum) >= -1) &&
            (redNum - blueNum) <= 1)
        {
            showIfReadyStart.text = "準備開始遊戲\n紅隊" + redNum + "人";
            showIfReadyStart.text += "\n藍隊" + blueNum + "人";
        }
        else
        {
            showIfReadyStart.text = "人數差距過大無法遊戲\n紅隊" + redNum + "人";
            showIfReadyStart.text += "\n藍隊" + blueNum + "人";
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
        else
        {
            startButton.SetActive(false);
        }
    }

    public override void OnJoinedRoom()
    {
        //after join room successfully, do some initialization
        roomNameDisplay.text = "房間名稱: " + PhotonNetwork.CurrentRoom.Name;
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
        if (teamMembersCount[0] > teamMembersCount[1])
        {
            Debug.Log("joined?" + PhotonNetwork.LocalPlayer.JoinTeam(teamCode: 2));    //team code begin from 1
        }
        else if (teamMembersCount[0] < teamMembersCount[1])
        {
            Debug.Log("joined?" + PhotonNetwork.LocalPlayer.JoinTeam(teamCode: 1));
        }
        else
        {
            Debug.Log("joined?" + PhotonNetwork.LocalPlayer.JoinTeam(teamCode: (byte)Random.Range(1, 3)));
            Debug.Log("random join team");
        }
    }

    public void ChangeTeam()
    {
        // when teamCode == 1, it's teamBlue
        // when teamCode == 2, it's teamRed
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Blue")
        {
            PhotonNetwork.LocalPlayer.SwitchTeam(teamCode: 2);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SwitchTeam(teamCode: 1);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (((playersContainerBlue.childCount - playersContainerRed.childCount) >= -1) &&
                (playersContainerBlue.childCount - playersContainerRed.childCount) <= 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.LoadLevel("mainSceneTeam");
            }
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
        Debug.Log("leave?" + PhotonNetwork.LocalPlayer.LeaveCurrentTeam());
        //callback function be called by other clients to update playerListings in container
        //need to leave team first, then leave room. otherwise, leave team fail
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }

    // Start is called before the first frame update
    void Start()
    {
        Chatclicktime = false;
        BlackPanel.SetActive(true);
        LeanTween.scale(BlackPanel, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutCubic);
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
        ClearPlayerListings();
        ListPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ChatPanel.SetActive(true);
            Chatclicktime = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChatPanel.SetActive(false);
            Chatclicktime = false;
        }
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //after let player call JointTeam, must wait server actually set custom properties and broadcast to all client
        //after finishing setting team, update the playerListings' color in container
        ClearPlayerListings();
        ListPlayers();
    }

    public void ShowChatRoomPanel()
    {
        if(Chatclicktime == false)
        {
            ChatPanel.SetActive(true);
            Chatclicktime = true;
        }
        else if(Chatclicktime == true)
        {
            ChatPanel.SetActive(false);
            Chatclicktime = false;
        }
    }

    public void ShowCharacterPanel()
    {
        if(Characterclicktime == false)
        {
            CharacterPanel.SetActive(true);
            Characterclicktime = true;
        }
        else if(Characterclicktime == true)
        {
            CharacterPanel.SetActive(false);
            Characterclicktime = false;
        }
    }
}
