using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SelectCharacterScript : MonoBehaviour
{
    private string playerStyle = ""; //player style name that choosed
    ExitGames.Client.Photon.Hashtable playerInfo;
    private string playerStyleKeyName = "playerStyle";  //const string, store hashtable player style's key 

    [SerializeField]
    private GameObject Character1;
    [SerializeField]
    private GameObject Character2;
    [SerializeField]
    private GameObject Character3;
    [SerializeField]
    private GameObject Character4;
    [SerializeField]
    private GameObject Character5;
    [SerializeField]
    private GameObject Character6;
    [SerializeField]
    private GameObject Character7;
    [SerializeField]
    private Text CharacterName;
    [SerializeField]
    private Image CharacterFace;

    [SerializeField]
    private Sprite one;
    [SerializeField]
    private Sprite two;
    [SerializeField]
    private Sprite three;
    [SerializeField]
    private Sprite four;
    [SerializeField]
    private Sprite five;
    [SerializeField]
    private Sprite six;
    [SerializeField]
    private Sprite seven;

    private void Start()
    {
        playerInfo = new ExitGames.Client.Photon.Hashtable();
        playerInfo.Add(playerStyleKeyName, playerStyle);
        object styleNameOutput;
        //if back from main game, get local player's style
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(playerStyleKeyName, out styleNameOutput))
        {
            playerStyle = (string)styleNameOutput;
            CharacterName.text = playerStyle;
        }
        //if never set style
        else
        {
            CharacterName.text = "character1";
            playerStyle = "character1";
            playerInfo[playerStyleKeyName] = playerStyle;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        }
    }

    public void characterOne()
    {
        CharacterName.text = "character1";
        CharacterFace.sprite = one;
        playerStyle = "character1";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
    }

    public void charactertwo()
    {
        CharacterName.text = "character2";
        CharacterFace.sprite = two;
        playerStyle = "character2";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);

    }

    public void characterthree()
    {
        CharacterName.text = "character3";
        CharacterFace.sprite = three;
        playerStyle = "character3";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);

    }

    public void characterfour()
    {
        CharacterName.text = "character4";
        CharacterFace.sprite = four;
        playerStyle = "character4";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);

    }

    public void characterfive()
    {
        CharacterName.text = "character5";
        CharacterFace.sprite = five;
        playerStyle = "character5";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);

    }

    public void charactersix()
    {
        CharacterName.text = "character6";
        CharacterFace.sprite = six;
        playerStyle = "character6";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);

    }

    public void characterseven()
    {
        CharacterName.text = "character7";
        CharacterFace.sprite = seven;
        playerStyle = "character7";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);

    }


}
