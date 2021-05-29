using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
//using ExitGames.Client.Photon;

public class playerStyleSelectionControl : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject lobbyPanel;  //lobby panel, show available room list and create room action
    [SerializeField]
    private GameObject playerStyleSelectionPanel;  //lobby panel, show available room list and create room action
    private string playerStyle; //player style name that choosed

    ExitGames.Client.Photon.Hashtable playerInfo;

    [SerializeField]
    private Text playerStyleDisplay;   //display player style

    private string playerStyleKeyName = "playerStyle";  //const string, store hashtable player style's key 

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = new ExitGames.Client.Photon.Hashtable();
        object styleNameOutput;
        //if back from main game, get local player's style
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(playerStyleKeyName, out styleNameOutput))
        {
            playerStyle = (string)styleNameOutput;
            playerStyleDisplay.text = "style: " + (string)styleNameOutput;
        }
        //if never set style
        else
        {
            playerStyle = "default";
            playerStyleDisplay.text = "style: default";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playerStyleSelectionOnClick()
    {
        lobbyPanel.SetActive(false);
        playerStyleSelectionPanel.SetActive(true);
    }

    public void playerStyleSelectionBackOnClick()
    {
        //if added to hashtable before
        if (playerInfo.ContainsKey(playerStyleKeyName))
        {
            playerInfo[playerStyleKeyName] = playerStyle;
        }
        //if never added to hashtable
        else
        {
            playerInfo.Add(playerStyleKeyName, playerStyle);
        }
        //add or update player's custom properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        lobbyPanel.SetActive(true);
        playerStyleSelectionPanel.SetActive(false);
        playerStyleDisplay.text = "style: " + (string)PhotonNetwork.LocalPlayer.CustomProperties[playerStyleKeyName];
    }

    public void playerStyleOneOnClick()
    {
        playerStyle = "playerStyleOne";
    }

    public void playerStyleTwoOnClick()
    {
        playerStyle = "playerStyleTwo";
    }
}
