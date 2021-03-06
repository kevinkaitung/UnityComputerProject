using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class teamProcessing : MonoBehaviourPunCallbacks
{
    //to control the team
    public string team;
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
    //index for notice points' photon view ID array
    private int viewidIndex = 0;
    //calculate accuracy
    private float accuracy;
    //count correct step
    private int correctCount;
    //check if finish building
    private bool isFinish;
    private string playerTeam;


    // Start is called before the first frame update
    void Start()
    {
        currentStageNumber = 1;
        dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        index = 0;
        accuracy = 0.0f;
        correctCount = 0;
        isFinish = false;
        //get local player team
        object tmp;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("_pt", out tmp);
        if ((byte)tmp == 1)
        {
            playerTeam = "blue";
        }
        else
        {
            playerTeam = "red";
        }
        if (PhotonNetwork.IsMasterClient)
        {
            settingStage(currentStageNumber);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void settingStage(int stageNumber)
    {
        //notice points' photon view ID list
        List<int> viewIDs = new List<int>();
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //change blue print if my team set new stage
            if (playerTeam == team)
            {
                //show notice points info on blueprint
                bluePrint.instance.bluePrintText.text = "stage : " + stageNumber.ToString() + "\n";
            }
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                //use part of building cube as notice cube
                GameObject clone = Instantiate(Resources.Load("house/" + nodeManager.instance.dataRoot.gameDataNodes[index].objShape, typeof(GameObject))) as GameObject;
                //set the material transparent
                Renderer cloneRend = clone.GetComponent<Renderer>();
                cloneRend.material = Resources.Load("transparent") as Material;
                //assign every part of building to the team field, set the correct position
                clone.transform.parent = this.gameObject.transform;
                clone.transform.localPosition = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                clone.transform.localRotation = Quaternion.Euler(nodeManager.instance.dataRoot.gameDataNodes[index].rotation);
                //set teamtag to identify this part of building belongs to which team
                teamTag teamtag = clone.GetComponent<teamTag>();
                teamtag.belongingTeam = team;
                //set its info to this part of building
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                //change blue print if my team set new stage
                if (playerTeam == team)
                {
                    //show each notice point info
                    bluePrint.instance.bluePrintText.text += "The material of " + tmp.objShap + " is " + tmp.materialNam + "\n";
                }
                //assing photon view ID to sync
                PhotonView PV = clone.GetComponent<PhotonView>();
                if (PhotonNetwork.AllocateViewID(PV))
                {
                    viewIDs.Add(PV.ViewID);
                }
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
            isFinish = true;
        }
        //when passing parameter via network, need to change list to array
        photonView.RPC("otherSettingStage", RpcTarget.OthersBuffered, currentStageNumber, viewIDs.ToArray());
    }

    //only master client need to load settingStage()
    [PunRPC]
    void otherSettingStage(int stageNumber, int[] viewIDs)
    {
        viewidIndex = 0;
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //change blue print if my team set new stage
            if (playerTeam == team)
            {
                //show notice points info on blueprint
                bluePrint.instance.bluePrintText.text = "stage : " + stageNumber.ToString() + "\n";
            }
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                //use part of building cube as notice cube
                GameObject clone = Instantiate(Resources.Load("house/" + nodeManager.instance.dataRoot.gameDataNodes[index].objShape, typeof(GameObject))) as GameObject;
                //set the material transparent
                Renderer cloneRend = clone.GetComponent<Renderer>();
                cloneRend.material = Resources.Load("transparent") as Material;
                //assign every part of building to the team field, set the correct position
                clone.transform.parent = this.gameObject.transform;
                clone.transform.localPosition = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                clone.transform.localRotation = Quaternion.Euler(nodeManager.instance.dataRoot.gameDataNodes[index].rotation);
                //set teamtag to identify this part of building belongs to which team
                teamTag teamtag = clone.GetComponent<teamTag>();
                teamtag.belongingTeam = team;
                //set its info to this part of building
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                //change blue print if my team set new stage
                if (playerTeam == team)
                {
                    //show each notice point info
                    bluePrint.instance.bluePrintText.text += "The material of " + tmp.objShap + " is " + tmp.materialNam + "\n";
                }
                PhotonView PV = clone.GetComponent<PhotonView>();
                PV.ViewID = viewIDs[viewidIndex];
                viewidIndex++;
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
            isFinish = true;
        }
    }

    public void playerPutThing(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
        //Put right game object on the player clicked point
        Debug.Log("obj shape: " + pointInfo.objShap + " obj pos: " + pointInfo.pos);
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //call others to deal with the game logic
        photonView.RPC("otherPlayerPut", RpcTarget.Others, pointInfo, handyMaterial);
        //if all notice points have been clicked (put), go next stage
        if (PhotonNetwork.IsMasterClient)
        {
            if (thisStagePutCount == thisStageCount)
            {
                currentStageNumber++;
                Debug.Log("go next stage: " + currentStageNumber);
                settingStage(currentStageNumber);
                //photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    [PunRPC]
    public void otherPlayerPut(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
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
                //photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    public bool isThisTeamFinish()
    {
        return isFinish;
    }

    public float accuracyCount()
    {
        accuracy = (float)correctCount / (float)dataNodeLen;
        return accuracy;
    }
}
