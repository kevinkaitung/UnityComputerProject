using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

namespace mySection
{
    public class gameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("Prefab- 玩家的角色")]
        public GameObject playerPrefab;
        private string playerStyle;
        //spawn point for different players
        public Transform[] spawnPoints;
        //spawnPoints[0] and spawnPoints[1] is blueTeam
        //controll blueTeam spawnPoints
        private int i = 0;
        //spawnPoints[2] and spawnPoints[3] is blueTeam
        //controll redTeam spawnPoints
        private int j = 2;

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
                if(PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Blue")
                {
                    PhotonNetwork.Instantiate(playerStyle, spawnPoints[i].position, spawnPoints[i].rotation, 0);
                    i = i + 1;
                }
                else
                {
                    PhotonNetwork.Instantiate(playerStyle, spawnPoints[j].position, spawnPoints[j].rotation, 0);
                    j = j + 1;
                }
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