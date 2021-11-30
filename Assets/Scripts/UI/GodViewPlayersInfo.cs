using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class GodViewPlayersInfo : MonoBehaviourPunCallbacks
{
    //GodViewPlayersInfo Singleton
    //Only create NodeManager once
    private static GodViewPlayersInfo s_Instance = null;
    public static GodViewPlayersInfo instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GodViewPlayersInfo)) as GodViewPlayersInfo;

                if (s_Instance == null)
                    Debug.Log("Could not locate a GodViewPlayersInfo " +
                              "object. \n You have to have exactly " +
                              "one GodViewPlayersInfo in the scene.");
            }
            return s_Instance;
        }
    }

    public GameObject godCamera;
    public GameObject canvas;
    public GameObject GodViewPlayersInfoPanel;
    public GameObject GodViewInfoPanel;
    //to get the width and height of prefab
    public RectTransform playerUIprefab;
    public float widthOffset = 55.0f, heightOffset = 25.0f;
    private string myTeam;
    //order is not corespond to child order in hierarchy
    public List<PlayerInfoUI> playersInfo;
    private int okCount = 0;
    // Start is called before the first frame update
    void Awake()
    {
        playersInfo = new List<PlayerInfoUI>();
    }
    public void instantiateOk()
    {
        //width = playerUIprefab.sizeDelta.x;
        //height = playerUIprefab.sizeDelta.y;
        okCount++;
        //if all instantiated-infoPrefab ready, start to get all prefab's data
        if (okCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            myTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;
            int count = GodViewPlayersInfoPanel.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                PlayerInfoUI tempInfo = GodViewPlayersInfoPanel.transform.GetChild(i).GetComponent<PlayerInfoUI>();
                Text tempNameText = tempInfo.PlayerNameText;
                RectTransform tempRect = tempInfo.transform.GetComponent<RectTransform>();
                float offset = 10.0f;
                if (tempInfo.myTeam == myTeam)
                {
                    //add the same team members to list
                    //playersInfo.Add(tempInfo);
                    //adjust playerUI size to fit the player's name and image (image size is const, 30)
                    tempRect.sizeDelta = new Vector2(tempNameText.preferredWidth + offset + 30.0f, tempRect.sizeDelta.y);
                    //adjust player name text size to fit
                    tempNameText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(tempNameText.preferredWidth + offset, tempNameText.preferredHeight);
                    //set player name text anchoredPos to align the left edge
                    tempNameText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((tempNameText.preferredWidth + offset) / 2.0f, 0.0f);
                }
                else
                {
                    //disable the opposite team members's hold material
                    tempInfo.containerImage.SetActive(false);
                    //adjust playerUI size to fit the player's name
                    tempRect.sizeDelta = new Vector2(tempNameText.preferredWidth + offset, tempRect.sizeDelta.y);
                    //adjust player name text size to fit
                    tempNameText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(tempNameText.preferredWidth + offset, tempNameText.preferredHeight);
                    tempNameText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((tempNameText.preferredWidth + offset) / 2.0f, 0.0f);

                }
            }
        }
        else
        {
            Debug.Log("wait for other players instantiate character objects");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //if any player left room, adjust list
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playersInfo.Clear();
        int count = GodViewPlayersInfoPanel.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            PlayerInfoUI tempInfo = GodViewPlayersInfoPanel.transform.GetChild(i).GetComponent<PlayerInfoUI>();
            Text tempNameText = tempInfo.PlayerNameText;
            RectTransform tempRect = tempInfo.transform.GetComponent<RectTransform>();
            float offset = 10.0f;
            if (tempInfo.myTeam == myTeam)
            {
                //add the same team members to list
                //playersInfo.Add(tempInfo);
                //adjust playerUI size to fit the player's name and image (image size is const, 30)
                tempRect.sizeDelta = new Vector2(tempNameText.preferredWidth + offset + 30.0f, tempRect.sizeDelta.y);
                //adjust player name text size to fit
                tempNameText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(tempNameText.preferredWidth + offset, tempNameText.preferredHeight);
                //set player name text anchoredPos to align the left edge
                tempNameText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((tempNameText.preferredWidth + offset) / 2.0f, 0.0f);
            }
            else
            {
                //disable the opposite team members's hold material
                tempInfo.containerImage.SetActive(false);
                //adjust playerUI size to fit the player's name
                tempRect.sizeDelta = new Vector2(tempNameText.preferredWidth + offset, tempRect.sizeDelta.y);
                //adjust player name text size to fit
                tempNameText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(tempNameText.preferredWidth + offset, tempNameText.preferredHeight);
                tempNameText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((tempNameText.preferredWidth + offset) / 2.0f, 0.0f);

            }
        }
    }

    //if game finish, clear the playersInfo list and close the info panel
    public void gameFinishGodviewDoing()
    {
        playersInfo.Clear();
        GodViewInfoPanel.SetActive(false);
    }

    void LateUpdate()
    {

    }
}
