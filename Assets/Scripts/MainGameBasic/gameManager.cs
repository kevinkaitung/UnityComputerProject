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
            object playerStyleOutput;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerStyle", out playerStyleOutput))
            {
                playerStyle = (string)playerStyleOutput;
            }
            else
            {
                playerStyle = "character1";
            }
            if (playerPrefab == null)
            {
                Debug.LogError("playerPrefab 遺失, 請在 Game Manager 重新設定", this);
            }
            else
            {
                //count my team members to decide which spawn point to choose
                int sameTeamMemberCount;
                string myTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;
                if (myTeam == "Blue")
                {
                    //spawn point 0~3 for blue team
                    sameTeamMemberCount = 0;
                }
                else
                {
                    //spawn point 4~7 for red team
                    sameTeamMemberCount = 4;
                }
                foreach (Player aPlayer in PhotonNetwork.PlayerList)
                {
                    if (aPlayer.GetPhotonTeam().Name == myTeam)
                    {
                        if (aPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            //assign to my team's spawn points
                            PhotonNetwork.Instantiate(playerStyle, spawnPoints[sameTeamMemberCount].position, spawnPoints[sameTeamMemberCount].rotation, 0);
                            gamePropsManager.instance.myRespawnPointIndex = sameTeamMemberCount;
                            break;
                        }
                        sameTeamMemberCount++;
                    }
                }
                //Debug.LogFormat("動態生成玩家角色 {0}", Application.loadedLevelName);
            }

            //voice speaker
            //PhotonNetwork.Instantiate("VoiceSpeakerPrefab", new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
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