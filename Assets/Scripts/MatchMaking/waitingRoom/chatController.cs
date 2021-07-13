  
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class chatController : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;

    public TextMeshProUGUI connectionState;
    public TMP_InputField msgInput;
    public TextMeshProUGUI msgArea;

    private string worldchat;
    [SerializeField] private string userID;
    
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        if(string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
        {
            Debug.LogError("No AppID Provided");
            return;
        }
        GetConnected();
        worldchat = "world";
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void GetConnected()
    {
        Debug.Log("Connecting");
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion,
            new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    public void GetDisconnected()
    {
        Debug.Log("Leaving");
        chatClient.Disconnect(ChatDisconnectCause.None);
    }

    public void SendMsg()
    {
        if(msgInput.text!="")
        {
            chatClient.PublishMessage(worldchat, msgInput.text);
            msgInput.text = "";
        }
    }
    
    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnDisconnected()
    {
        chatClient.Unsubscribe(new string[] {worldchat});
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] {worldchat});
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            msgArea.text += senders[i] + ": " + messages[i] + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            this.chatClient.PublishMessage(channel, "joined");
        }
        connectionState.text = "chatRoom is connected";
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}