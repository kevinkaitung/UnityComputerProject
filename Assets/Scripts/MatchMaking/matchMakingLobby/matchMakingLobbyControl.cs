using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class matchMakingLobbyControl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject lobbyConnectButton;  //button to enter lobby
    [SerializeField]
    private GameObject lobbyPanel;  //lobby panel, show available room list and create room action
    [SerializeField]
    private GameObject mainPanel;   //start menu panel (before enter lobby)
    public GameObject exitPanel;

    public InputField playerNameInput;  //get start menu panel player name input

    public GameObject showPlayerNameInLobby; //show player name in lobby

    private string roomName;    //for saving created room name
    private int roomSize;   //for saving created room size

    private List<RoomInfo> roomListings;    //list of current room, change with ui container (record ui container's child obj)
    [SerializeField]
    private Transform roomsContainer;   //container for holding all the room listings items
    [SerializeField]
    private GameObject roomListingPrefab;   //prefab for displayer each room in the lobby

    [SerializeField]
    private Text Guideword;
    [SerializeField]
    private Image FlourishRight;
    [SerializeField]
    private Image FlourishLeft;
    [SerializeField]
    private GameObject wordPanel;
    [SerializeField]
    private GameObject buttonBackground;
    [SerializeField]
    private GameObject buttonPanel;
    [SerializeField]
    private GameObject BlackPanel;
    [SerializeField]
    private GameObject RoomListPanel;
    [SerializeField]
    private GameObject InputErrorPanel;
    public bool isJoinLobby = false;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //after connecting to master, we can start the game
        lobbyConnectButton.SetActive(true);

        //if user doesn't enter the name, give a random name
        if (playerNameInput.text == "")
        {
            PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000);
        }
        //get user's input as player name
        else
        {
            PhotonNetwork.NickName = playerNameInput.text;
        }
    }

    //if text field changed, update the player name
    public void PlayerNameUpdate(string nameInput)
    {
        //if user doesn't enter the name, give a random name
        if (playerNameInput.text == "")
        {
            PhotonNetwork.NickName = "Player " + UnityEngine.Random.Range(0, 1000).ToString();
        }
        //get user's input as player name
        else
        {
            PhotonNetwork.NickName = playerNameInput.text;
        }
    }

    //when click join lobby button, join the lobby
    public void JoinLobbyOnClick()
    {
        isJoinLobby = true;
        PhotonNetwork.JoinLobby();
    }

    //when join lobby
    public override void OnJoinedLobby()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        isJoinLobby = true;
        //show player name in lobby
        Text showPlayerName = showPlayerNameInLobby.GetComponent<Text>();
        showPlayerName.text = "Hello, " + PhotonNetwork.NickName;
        LobbyPanelAnimation();
    }

    //when room list update
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        Debug.Log("OnroomListupdate called");
        foreach (RoomInfo room in roomList)
        {
            //if roomlisting list is not empty (null), assign this room's index in roomlisting list to tempIndex
            if (roomListings != null)
            {
                //tempIndex = this room's index
                //if can't find this room, return -1
                tempIndex = roomListings.FindIndex(ByName(room.Name));
                Debug.Log(roomListings.Count);
                Debug.Log("tmpindex!" + tempIndex + " room name: " + room.Name);
            }
            else
            {
                tempIndex = -1;
            }
            //remove listing item first?
            if (tempIndex != -1)
            {
                Debug.Log("remove " + tempIndex);
                roomListings.RemoveAt(tempIndex);
                Debug.Log("after remove count" + roomListings.Count);
                Destroy(roomsContainer.GetChild(tempIndex).gameObject);
            }
            if (room.PlayerCount > 0 && room.IsOpen && room.IsVisible)
            {
                Debug.Log(roomListings + "   " + tempIndex);
                Debug.Log("room name:  " + room.Name);
                roomListings.Add(room);
                ListRoom(room);
            }
        }
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        roomSize = int.Parse(sizeIn);
    }
    IEnumerator ShowInputErrorPanel(string warningText)
    {
        InputErrorPanel.SetActive(true);
        InputErrorPanel.GetComponentInChildren<Text>().text = warningText;
        yield return new WaitForSeconds(1);
        InputErrorPanel.SetActive(false);
    }

    public void CreateRoom()
    {
        Debug.Log("creating room now");
        if (roomName == null)
            roomName = "Room " + UnityEngine.Random.Range(1, 100).ToString();
        if (roomSize == 0)
            roomSize = 8;
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        if (PhotonNetwork.CreateRoom(roomName, roomOps))
        {
            //6/9 是不是在OnJoinedRoom被呼叫之後再載入場景較佳(萬一載入場景比下一個場景的OnJoinedRoom還要慢完成(叫不到OnJoinedRoom了))
            PhotonNetwork.LoadLevel("waitingRoomScene");
        }
    }

    //if failed join room
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room failed");
        //join room failed, remove black panel (change scene anim)
        LeanTween.scale(BlackPanel, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(joinRoomFailedWarnings);
    }

    //called by join room failed callback function
    void joinRoomFailedWarnings()
    {
        StartCoroutine(ShowInputErrorPanel("加入房間失敗，請稍候重新嘗試!"));
    }

    //when joined room (new create room), load waitingRoomScene
    //other clients join room by button, and once they join room, autosync scene
    //so they don't call this callback function (in launch scene)
    public override void OnJoinedRoom()
    {
        //master client of room need to join room
        //PhotonTeam[] availableTeam = PhotonTeamsManager.Instance.GetAvailableTeams();
        //Debug.Log("Blue:" + PhotonTeamsManager.Instance.GetTeamMembersCount(availableTeam[0]));
        //Debug.Log("Red:" + PhotonTeamsManager.Instance.GetTeamMembersCount(availableTeam[1]));
        //Debug.Log("joined?" + PhotonNetwork.LocalPlayer.JoinTeam(teamCode: (byte)Random.Range(1, 3)));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room failed");
        //create room failed, remove black panel (change scene anim)
        LeanTween.scale(BlackPanel, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(createRoomFailedWarnings);
    }

    //called by create room failed callback function
    void createRoomFailedWarnings()
    {
        StartCoroutine(ShowInputErrorPanel("創建房間失敗，請稍候重新嘗試!"));
    }

    public void MatchmakingCancel()
    {
        PhotonNetwork.LeaveLobby();
        if (PhotonNetwork.IsConnected)
        {
            lobbyConnectButton.SetActive(true);
        }
        LeanTween.scale(wordPanel, Vector3.zero, 0.1f);
        LeanTween.scale(buttonBackground, Vector3.zero, 0.1f);
        LeanTween.scale(buttonPanel, Vector3.zero, 0.1f);
        isJoinLobby = false;
        RoomListPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    private void Start()
    {
        //initialize when back from waiting room scene or when into the start menu
        roomListings = new List<RoomInfo>();
        flashword();
        exitPanel.SetActive(false);
        InputErrorPanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && isJoinLobby == false)
        {
            JoinLobbyOnClick();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            exitPanel.SetActive(true);
        }
    }

    public void flashword()
    {
        LeanTween.colorText(Guideword.GetComponent<RectTransform>(), Color.yellow, 1f);
        LeanTween.color(FlourishLeft.GetComponent<RectTransform>(), Color.yellow, 1f);
        LeanTween.color(FlourishRight.GetComponent<RectTransform>(), Color.yellow, 1f);
        LeanTween.colorText(Guideword.GetComponent<RectTransform>(), Color.white, 1f).setDelay(1f);
        LeanTween.color(FlourishLeft.GetComponent<RectTransform>(), Color.white, 1f).setDelay(1f);
        LeanTween.color(FlourishRight.GetComponent<RectTransform>(), Color.white, 1f).setDelay(1f).setOnComplete(flashword);
    }

    void LobbyPanelAnimation()
    {
        LeanTween.scale(wordPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(buttonBackground, Vector3.one, 0.5f).setEase(LeanTweenType.easeInCubic);
        LeanTween.scale(buttonPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {
            RoomListPanel.SetActive(true);
        });
    }

    public void CreateRoomChangeScene()
    {
        //if create room btn clicked, check the input room info
        if (!roomSize.ToString().All(char.IsDigit) || roomSize < 0 || roomSize > 8)
        {
            StartCoroutine(ShowInputErrorPanel("請輸入正確的數字\n(房間玩家最高人數8人)!"));
            return;
        }
        //if input room info no problem, call create room function
        BlackPanel.SetActive(true);
        LeanTween.scale(BlackPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(CreateRoom);
    }
    public void exitgame()
    {
        Application.Quit();
    }
    public void backtogame()
    {
        exitPanel.SetActive(false);
    }
}
