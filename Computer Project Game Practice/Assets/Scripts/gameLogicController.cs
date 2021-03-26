using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class gameLogicController : MonoBehaviourPunCallbacks
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


    //public bool[] isDoneInThisStage;
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
    // Start is called before the first frame update
    void Start()
    {
        currentStageNumber = 1;
        index = 0;
        dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        //setting first stage
        settingStage(currentStageNumber);
    }

    // Update is called once per frame
    void Update()
    {

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
                GameObject clone = PhotonNetwork.Instantiate("noticePoint", nodeManager.instance.dataRoot.gameDataNodes[index].position, Quaternion.identity);
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                index++;
                thisStageCount++;
                if(index == dataNodeLen)
                {
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Game Finish!!!");
        }
    }

    public void playerPutThingsOnPoint(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
        //Put right game object on the player clicked point
        PhotonNetwork.Instantiate(pointInfo.objShap, pointInfo.pos, Quaternion.Euler(pointInfo.rot));
        //if all notice points have been clicked (put), go next stage
        if (thisStagePutCount == thisStageCount)
        {
            currentStageNumber++;
            Debug.Log("go next stage: " + currentStageNumber);
            settingStage(currentStageNumber);
        }
    }
}
