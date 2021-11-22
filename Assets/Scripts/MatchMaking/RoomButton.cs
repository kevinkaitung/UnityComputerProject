using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text sizeText;

    private string roomName;
    private int roomSize;
    private int playerCount;

    public void JoinRoomOnClick()
    {
        //before join room, play the scene change anim
        LobbyChangeSceneAnim.instance.blackPanel.SetActive(true);
        LeanTween.scale(LobbyChangeSceneAnim.instance.blackPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(changeSceneAnimation);
    }

    void changeSceneAnimation()
    {
        PhotonNetwork.JoinRoom(roomName);
        //join room 之後，會自動sync場景而跟房主的場景一樣
    }

    public void SetRoom(string nameInput, int sizeInput, int countInput)
    {
        roomName = nameInput;
        roomSize = sizeInput;
        playerCount = countInput;
        nameText.text = nameInput;
        sizeText.text = countInput + "/" + sizeInput;
    }
}
