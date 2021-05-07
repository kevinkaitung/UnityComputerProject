using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class gameLogicController : MonoBehaviourPunCallbacks/*, IPunObservable*/
{
    //gameLogicController Singleton
    //Only create NodeManager once
    private static gameLogicController s_Instance = null;
    public static gameLogicController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(gameLogicController)) as gameLogicController;

                if (s_Instance == null)
                    Debug.Log("Could not locate a gameLogicController " +
                              "object. \n You have to have exactly " +
                              "one gameLogicController in the scene.");
            }
            return s_Instance;
        }
    }

    //how many point to put in this stage
    private int thisStageCount;
    //how many points have been clicked by player (already build that point)
    private int thisStagePutCount;
    //current stage
    private int currentStageNumber;
    //game data nodes' length
    private int dataNodeLen;
    //index for access game data nodes
    private int index;
    //calculate accuracy
    private float accuracy;
    //count correct step
    private int correctCount;

    //enable and disable timer
    bool startTimer = false;
    //local player timer
    double timerIncrementValue;
    //master client start time shared with others
    double startTime;
    //countdown time
    [SerializeField] double timer = 20;
    //store master client start time to room custom properties
    ExitGames.Client.Photon.Hashtable CustomValue;
    [SerializeField] Text timerText;

    public void Start()
    {
        currentStageNumber = 1;
        dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        index = 0;
        accuracy = 0.0f;
        correctCount = 0;
        //regist for the custom data type serialization
        customTypes.register();
        if (PhotonNetwork.IsMasterClient)
        {
            //set timer
            CustomValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
            //setting first stage
            photonView.RPC("otherSettingStage", RpcTarget.AllBuffered, currentStageNumber);
        }
        else
        {
            //except master client, other players get start time of master client
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //count time and check
        if (!startTimer) return;
        timerIncrementValue = PhotonNetwork.Time - startTime;
        timerText.text = (timer - timerIncrementValue).ToString("0:00");
        if (timerIncrementValue >= timer)
        {
            //time's up, game finish
            Debug.Log("time's up");
            gameFinishDoing();
        }
    }

    void settingStage(int stageNumber)
    {
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                GameObject clone = Instantiate(Resources.Load("noticePoint", typeof(GameObject)), nodeManager.instance.dataRoot.gameDataNodes[index].position, Quaternion.identity) as GameObject;
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                index++;
                thisStageCount++;
                if (index == dataNodeLen)
                {
                    break;
                }
            }
        }
        else
        {
            gameFinishDoing();
            Debug.Log("Game Finish!!!");
        }
    }

    //only master client need to load settingStage()
    [PunRPC]
    void otherSettingStage(int stageNumber)
    {
        //dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                GameObject clone = Instantiate(Resources.Load("noticePoint", typeof(GameObject)), nodeManager.instance.dataRoot.gameDataNodes[index].position, Quaternion.identity) as GameObject;
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                index++;
                thisStageCount++;
                if (index == dataNodeLen)
                {
                    break;
                }
            }
        }
        else
        {
            gameFinishDoing();
            Debug.Log("Game Finish!!!");
        }
    }

    public void playerPutThingsOnPoint(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
        //Put right game object on the player clicked point
        Debug.Log("obj shape: " + pointInfo.objShap + " obj pos: " + pointInfo.pos);
        //create part of the building
        GameObject partOfBuildingClone = Instantiate(Resources.Load(pointInfo.objShap, typeof(GameObject)), pointInfo.pos, Quaternion.Euler(pointInfo.rot)) as GameObject;
        //set the texture to handyMaterial
        Renderer cloneRend = partOfBuildingClone.GetComponent<Renderer>();
        cloneRend.material.mainTexture = Resources.Load(handyMaterial) as Texture;
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //call others to deal with the game logic
        photonView.RPC("otherPlayerPutThingOnPoint", RpcTarget.Others, pointInfo, handyMaterial);
        //if all notice points have been clicked (put), go next stage
        if (PhotonNetwork.IsMasterClient)
        {
            if (thisStagePutCount == thisStageCount)
            {
                currentStageNumber++;
                Debug.Log("go next stage: " + currentStageNumber);
                settingStage(currentStageNumber);
                photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    [PunRPC]
    public void otherPlayerPutThingOnPoint(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
        //create part of the building
        GameObject partOfBuildingClone = Instantiate(Resources.Load(pointInfo.objShap, typeof(GameObject)), pointInfo.pos, Quaternion.Euler(pointInfo.rot)) as GameObject;
        //set the texture to handyMaterial
        Renderer cloneRend = partOfBuildingClone.GetComponent<Renderer>();
        cloneRend.material.mainTexture = Resources.Load(handyMaterial) as Texture;
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //the thing that player put on point was created by that player, don't need to put it again!
        if (PhotonNetwork.IsMasterClient)
        {
            if (thisStagePutCount == thisStageCount)
            {
                currentStageNumber++;
                Debug.Log("go next stage: " + currentStageNumber);
                settingStage(currentStageNumber);
                photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    void gameFinishDoing()
    {
        //disable timer
        startTimer = false;
        accuracy = (float)correctCount / (float)dataNodeLen;
        Debug.Log("accuracy:" + accuracy);
    }
}
