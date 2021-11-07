using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

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
                if (tempInfo.myTeam == myTeam)
                {
                    //add the same team members to list
                    playersInfo.Add(tempInfo);
                }
                else
                {
                    //disable the opposite team members
                    tempInfo.transform.gameObject.SetActive(false);
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
            if (tempInfo.myTeam == myTeam)
            {
                //add the same team members to list
                playersInfo.Add(tempInfo);
            }
            else
            {
                //disable the opposite team members
                tempInfo.transform.gameObject.SetActive(false);
            }
        }
    }

    void LateUpdate()
    {
        Vector2 center = new Vector2(0.0f, 0.0f);
        for (int i = 0; i < playersInfo.Count; i++)
        {
            center += playersInfo[i].currentPlayerPos;
        }
        center /= playersInfo.Count;
        for (int i = 0; i < playersInfo.Count; i++)
        {
            Vector2 dir = new Vector2(0.0f, 0.0f);
            dir = playersInfo[i].currentPlayerPos - center;
            if (dir.x >= 0.0f && dir.y >= 0.0f)
            {
                //update infoPrefab's pos
                playersInfo[i].transform.GetComponent<RectTransform>().anchoredPosition = playersInfo[i].currentPlayerPos;
                //adjust info detail pos
                playersInfo[i].infoBackGround.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOffset, heightOffset);
            }
            else if (dir.x >= 0.0f && dir.y < 0.0f)
            {
                playersInfo[i].transform.GetComponent<RectTransform>().anchoredPosition = playersInfo[i].currentPlayerPos;
                playersInfo[i].infoBackGround.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(widthOffset, -heightOffset);
            }
            else if (dir.x < 0.0f && dir.y < 0.0f)
            {
                playersInfo[i].transform.GetComponent<RectTransform>().anchoredPosition = playersInfo[i].currentPlayerPos;
                playersInfo[i].infoBackGround.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-widthOffset, -heightOffset);
            }
            else if (dir.x < 0.0f && dir.y >= 0.0f)
            {
                playersInfo[i].transform.GetComponent<RectTransform>().anchoredPosition = playersInfo[i].currentPlayerPos;
                playersInfo[i].infoBackGround.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-widthOffset, heightOffset);
            }
        }
    }
}
