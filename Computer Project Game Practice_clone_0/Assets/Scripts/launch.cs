using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
 
namespace mySection
{
    public class launch : MonoBehaviourPunCallbacks
    {
        // 遊戲版本的編碼, 可讓 Photon Server 做同款遊戲不同版本的區隔.
        string gameVersion = "1";
 
        [Tooltip("遊戲室玩家人數上限. 當遊戲室玩家人數已滿額, 新玩家只能新開遊戲室來進行遊戲.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        bool isConnecting = false;

        void Awake()
        {
            // 確保所有連線的玩家均載入相同的遊戲場景
            PhotonNetwork.AutomaticallySyncScene = true;
        }
 
        void Start()
        {
            // 檢查是否與 Photon Cloud 連線
            if (PhotonNetwork.IsConnected)
            {
                // 已連線, 嚐試隨機加入一個遊戲室
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 未連線, 嚐試與 Photon Cloud 連線
                Debug.Log("hi");
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        // 與 Photon Cloud 連線
        public void Connect()
        {
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN 呼叫 OnConnectedToMaster(), 已連上 Photon Cloud.");
            
            isConnecting = true;
            // 確認已連上 Photon Cloud
            // 隨機加入一個遊戲室
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN 呼叫 OnDisconnected() {0}.", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN 呼叫 OnJoinRandomFailed(), 隨機加入遊戲室失敗.");
            
            // 隨機加入遊戲室失敗. 可能原因是 1. 沒有遊戲室 或 2. 有的都滿了.    
            // 好吧, 我們自己開一個遊戲室.
            PhotonNetwork.CreateRoom("fighting room", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("PUN 呼叫 OnJoinedRoom(), 已成功進入遊戲室中.");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("我是第一個進入遊戲室的玩家");
                Debug.Log("我得主動做載入場景的動作");
                PhotonNetwork.LoadLevel("mainScene");
            }
        }
    }
}