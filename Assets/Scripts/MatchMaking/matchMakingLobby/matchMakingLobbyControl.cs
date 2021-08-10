using System.Collections;
using System.Collections.Generic;
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

    


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //after connecting to master, we can start the game
        lobbyConnectButton.SetActive(true);

        //if user doesn't enter the name, give a random name
        if (playerNameInput.text == "")
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000);
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
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString();
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
        PhotonNetwork.JoinLobby();
    }

    //when join lobby
    public override void OnJoinedLobby()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        //show player name in lobby
        Text showPlayerName = showPlayerNameInLobby.GetComponent<Text>();
        showPlayerName.text = "Hello, " + PhotonNetwork.NickName;
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

    public void CreateRoom()
    {
        Debug.Log("creating room now");
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
    }

    public void MatchmakingCancel()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
        if (PhotonNetwork.IsConnected)
        {
            lobbyConnectButton.SetActive(true);
        }
    }

    private void Start()
    {
        //initialize when back from waiting room scene or when into the start menu
        roomListings = new List<RoomInfo>();
        flashword();
    }   

    public void flashword()
    {
        LeanTween.colorText(Guideword.GetComponent<RectTransform>(), Color.yellow, 1f);
        LeanTween.color(FlourishLeft.GetComponent<RectTransform>(), Color.yellow, 1f);
        LeanTween.color(FlourishRight.GetComponent<RectTransform>(), Color.yellow, 1f);
        LeanTween.colorText(Guideword.GetComponent<RectTransform>(), Color.white, 1f) .setDelay(1f);
        LeanTween.color(FlourishLeft.GetComponent<RectTransform>(), Color.white, 1f) .setDelay(1f);
        LeanTween.color(FlourishRight.GetComponent<RectTransform>(), Color.white, 1f) .setDelay(1f).setOnComplete(flashword);
    }

}
