using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace mySection
{
    public class gameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("Prefab- 玩家的角色")]
        public GameObject playerPrefab;
        private string playerStyle;
        //spawn point for different players
        public Transform[] spawnPoints;

        // Start is called before the first frame update
        void Start()
        {
            playerStyle = (string)PhotonNetwork.LocalPlayer.CustomProperties["playerStyle"];
            if (playerStyle == null)
            {
                playerStyle = "player";
            }
            if (playerPrefab == null)
            {
                Debug.LogError("playerPrefab 遺失, 請在 Game Manager 重新設定", this);
            }
            else
            {
                //Debug.LogFormat("動態生成玩家角色 {0}", Application.loadedLevelName);
                PhotonNetwork.Instantiate(playerStyle, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].position, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].rotation, 0);
            }
        }

        //新玩家進入時，呼叫OnPlayerEnteredRoom
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("{0} 進入遊戲室", other.NickName);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}