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
    public class MESSAGE
    {
        // name
        public String Text1 { get; set; }
        // msg
        public String Text2 { get; set; }
        // init
        public MESSAGE() { }
    }
    public Text connectionState;
    public InputField msgInput;
    public Text msgArea;
    private ChatClient chatClient;
    [SerializeField] 
    private string userID;
    // when flag == 0, it's red team
    // when flag == 1, it's blue team
    int flag = 0;
    // when index == 1, it's teamMsg
    // when index == 0, it's worldMsg
    int index = 0;
    private string tempRedChat;
    private string tempBlueChat;
    private string tempWorldChat;
    private string redChat;
    private string blueChat;
    private string worldchat;
    // text min/max width
    private int textSizeMinWidth = 100;
    private int textSizeMaxWidth = 161;
    // msgMaxNumLine
    private int msgMaxNumLine = 20;
    
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
        redChat = "這裡是紅隊聊天室\n";
        blueChat = "這裡是藍隊聊天室\n";
        worldchat = "world\n";
        tempRedChat = "這裡是紅隊聊天室\n";
        tempBlueChat = "這裡是藍隊聊天室\n";
        tempWorldChat = "world\n";
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMsg();
        }
    }

    public void GetConnected()
    {
        Debug.Log("Connecting ChatRoom XDDDDDD");
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
        if(msgInput.text != "")
        {
            if(flag == 0 && index == 1)
            {
                chatClient.PublishMessage(redChat, msgInput.text);
                msgInput.text = "";
            }
            else if(flag == 1 && index == 1)
            {
                chatClient.PublishMessage(blueChat, msgInput.text);
                msgInput.text = "";
            }
            else
            {
                chatClient.PublishMessage(worldchat, msgInput.text);
                msgInput.text = "";
            }
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
        Debug.Log("成功連接");
        chatClient.Subscribe(new string[] {worldchat});
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        String msgContent = msgArea.text;
        for (int i = 0; i < senders.Length; i++)
        {
            // get each player team
            // when tmp == 0, it's red team
            // when tmp == 1, it's blue team
            object tmp;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if(senders[i] == player.NickName)
                {
                    player.CustomProperties.TryGetValue("_pt", out tmp);
                    if((byte)tmp == 1)
                    {
                        MESSAGE msg = new MESSAGE();
                        msg.Text1 = "<color=#00B2EE>" + senders[i] + "</color>";
                        msg.Text2 = ": " + messages[i] + "\n";
                        msgContent += msg.Text1 + msg.Text2;
                    }
                    else
                    {
                        MESSAGE msg = new MESSAGE();
                        msg.Text1 = "<color=#FF4040>" + senders[i] + "</color>";
                        msg.Text2 = ": " + messages[i] + "\n";
                        msgContent += msg.Text1 + msg.Text2;
                    }
                }
            }
        }
        // 若行數超過msgMaxNumLine, 則砍字串
        Debug.Log("msgContent的行數 = " + msgContent.Split('\n').Length);
        if(msgContent.Split('\n').Length > msgMaxNumLine)
        {
            String[] strArray = msgContent.Split('\n');
            for(int i = 1; i < msgContent.Split('\n').Length; i++)
            {
                strArray[i-1] = strArray[i];
            }
            msgContent = "";
            for(int i = 0; i < msgMaxNumLine; i++)
            {
                msgContent = msgContent + strArray[i] + "\n";
            }
        }
        // 限制寬度
        SetTextSize(msgArea, msgContent);
    }

    private void SetTextSize(Text targetText,string contentStr)
    {
        if(targetText == null)
        {
            return;
        }
        targetText.text = contentStr;
        if(targetText.preferredWidth <= textSizeMinWidth)
        {
            return;
        }
        if(targetText.preferredWidth <= textSizeMaxWidth)
        {
            targetText.rectTransform.sizeDelta = new Vector2(targetText.preferredWidth, targetText.rectTransform.sizeDelta.y);
            return;
        }
        targetText.rectTransform.sizeDelta = new Vector2(textSizeMaxWidth, targetText.rectTransform.sizeDelta.y);
        int textSizeHeight = Mathf.CeilToInt(targetText.preferredHeight);
        targetText.rectTransform.sizeDelta = new Vector2(textSizeMaxWidth, textSizeHeight);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            Debug.Log("channels = " + channel);
            chatClient.PublishMessage(channel, "( joined )");
        }
        connectionState.text = "聊天室連接成功";
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

    public void changeChannel()
    {
        Debug.Log("切換頻道成功");
        // get team
        object tmp;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("_pt", out tmp);
        if((byte)tmp == 1)
        {
            flag = 1;
        }
        else
        {
            flag = 0;
        }
        // when index == 1, it's teamMsg
        // when index == 0, it's worldMsg
        if(flag == 0)
        {
            index = (index + 1) % 2;
            if(index == 1)
            {
                tempWorldChat = msgArea.text;
                chatClient.Unsubscribe(new string[] {worldchat});
                chatClient.Subscribe(new string[] {redChat});
                msgArea.text = tempRedChat;
            }
            else
            {
                tempRedChat = msgArea.text;
                chatClient.Unsubscribe(new string[] {redChat});
                chatClient.Subscribe(new string[] {worldchat});
                msgArea.text = tempWorldChat;
            }
        }
        else
        {
            index = (index + 1) % 2;
            if(index == 1)
            {
                tempWorldChat = msgArea.text;
                chatClient.Unsubscribe(new string[] {worldchat});
                chatClient.Subscribe(new string[] {blueChat});
                msgArea.text = tempBlueChat;
            }
            else
            {
                tempBlueChat = msgArea.text;
                chatClient.Unsubscribe(new string[] {blueChat});
                chatClient.Subscribe(new string[] {worldchat});
                msgArea.text = tempWorldChat;
            }
        }
    }
}