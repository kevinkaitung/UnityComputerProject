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
    public GameObject CharacterImage;

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
        CharacterName.text = "角色名稱 : Mily";
        CharacterFace.sprite = one;
        playerStyle = "character1";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("1", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }

    public void charactertwo()
    {
        CharacterName.text = "角色名稱 : Mary";
        CharacterFace.sprite = two;
        playerStyle = "character2";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("2", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }

    public void characterthree()
    {
        CharacterName.text = "角色名稱 : Angel";
        CharacterFace.sprite = three;
        playerStyle = "character3";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("3", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }

    public void characterfour()
    {
        CharacterName.text = "角色名稱 : Hapi";
        CharacterFace.sprite = four;
        playerStyle = "character4";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("4", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }

    public void characterfive()
    {
        CharacterName.text = "角色名稱 : Peter";
        CharacterFace.sprite = five;
        playerStyle = "character5";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("5", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }

    public void charactersix()
    {
        CharacterName.text = "角色名稱 : Alita";
        CharacterFace.sprite = six;
        playerStyle = "character6";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("6", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }

    public void characterseven()
    {
        CharacterName.text = "角色名稱 : Alan";
        CharacterFace.sprite = seven;
        playerStyle = "character7";
        playerInfo[playerStyleKeyName] = playerStyle;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerInfo);
        Sprite img = Resources.Load("7", typeof(Sprite)) as Sprite;
        CharacterFace.GetComponent<Image>().sprite = img;
    }
}