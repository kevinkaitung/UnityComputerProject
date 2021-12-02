using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class teamProcessing : MonoBehaviourPunCallbacks
{
    //to control the team
    public string team;
    //how many point to put in this stage
    private int thisStageCount;
    //how many points have been clicked by player (already build that point)
    private int thisStagePutCount;
    private int totalPutCount;
    //current stage
    public int currentStageNumber;
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
    //store every stage not finished objShape (type: string)
    List<List<string>> everyStageBuildProgress;
    // Start is called before the first frame update
    void Start()
    {
        currentStageNumber = 1;
        totalPutCount = 0;
        dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        //initiate with capacity length(add 1 because stage number start with 1 not 0)
        everyStageBuildProgress = new List<List<string>>(nodeManager.instance.dataRoot.gameDataNodes[dataNodeLen - 1].stage + 1);
        //add a empty list as index 0
        everyStageBuildProgress.Add(new List<string>());
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
        //temp list for this stage's obj shapes
        List<string> tempList = new List<string>();
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
                bluePrint.instance.autoSettoCurrentStageBluePrint(stageNumber);
            }
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                tempList.Add(nodeManager.instance.dataRoot.gameDataNodes[index].objShape);
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
                    //                    bluePrint.instance.bluePrintText.text += "The material of " + tmp.objShap + " is " + tmp.materialNam + "\n";
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
            //add this stage's list
            everyStageBuildProgress.Add(tempList);
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
        //temp list for this stage's obj shapes
        List<string> tempList = new List<string>();
        viewidIndex = 0;
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //change blue print if my team set new stage
            if (playerTeam == team)
            {
                //show notice points info on blueprint
                bluePrint.instance.autoSettoCurrentStageBluePrint(stageNumber);
            }
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                tempList.Add(nodeManager.instance.dataRoot.gameDataNodes[index].objShape);
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
                    //                    bluePrint.instance.bluePrintText.text += "The material of " + tmp.objShap + " is " + tmp.materialNam + "\n";
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
            //add this stage's list
            everyStageBuildProgress.Add(tempList);
        }
        else
        {
            isFinish = true;
        }
    }

    public void playerPutThing(noticePoint pointInfo, string handyMaterial)
    {
        //the player build the shape, remove it from this stage's list
        everyStageBuildProgress[pointInfo.stag].Remove(pointInfo.objShap);
        thisStagePutCount++;
        //total put count add 1, play the building progress bar anim
        totalPutCount++;
        buildingProgressBarAnim(totalPutCount);
        //if player finish the stage (the stage list is empty), play stage complete animation (maybe not in the current stage)
        if (everyStageBuildProgress[pointInfo.stag].Count == 0)
        {
            stageCompleteAnim(pointInfo.stag);
        }
        //Put right game object on the player clicked point
        Debug.Log("obj shape: " + pointInfo.objShap + " obj pos: " + pointInfo.pos);
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //call others to deal with the game logic
        photonView.RPC("otherPlayerPut", RpcTarget.Others, pointInfo, handyMaterial);
        //if all notice points have been clicked (put), go next stage (load new shapes)
        if (thisStagePutCount == thisStageCount)
        {
            currentStageNumber++;
            if (PhotonNetwork.IsMasterClient)
            {
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
        //the player build the shape, remove it from this stage's list
        everyStageBuildProgress[pointInfo.stag].Remove(pointInfo.objShap);
        thisStagePutCount++;
        //total put count add 1, play the building progress bar anim
        totalPutCount++;
        buildingProgressBarAnim(totalPutCount);
        //if player finish the stage (the stage list is empty), play stage complete animation (maybe not in the current stage)
        if (everyStageBuildProgress[pointInfo.stag].Count == 0)
        {
            stageCompleteAnim(pointInfo.stag);
        }
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //the thing that player put on point was created by that player, don't need to put it again!
        if (thisStagePutCount == thisStageCount)
        {
            currentStageNumber++;
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("go next stage: " + currentStageNumber);
                settingStage(currentStageNumber);
                //photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    //stage complete animation (parameter: finish stage number)
    void stageCompleteAnim(int finishStage)
    {
        Debug.Log("stage complete");
    }

    //stage cancel complete animation (parameter: cancel stage number)
    void stageCancelAnim(int cancelStage)
    {
        Debug.Log("stage cancel complete");
    }

    //building progress bar animation (parameter: total count of shapes)
    void buildingProgressBarAnim(int nowTotalPutCount)
    {

    }

    //if player use removal tool, revise the put counts
    public void playerRemoveBuildingTexture(noticePoint pointInfo)
    {
        //if this stage is previous complete (this stage's list is empty), cancel this stage completion status
        if (everyStageBuildProgress[pointInfo.stag].Count == 0)
        {
            stageCancelAnim(pointInfo.stag);
        }
        //add the removal shape to this stage's list
        everyStageBuildProgress[pointInfo.stag].Add(pointInfo.objShap);
        thisStagePutCount--;
        totalPutCount--;
        buildingProgressBarAnim(totalPutCount);
        photonView.RPC("otherPlayerRemoveBuildingTexture", RpcTarget.Others, pointInfo);
    }

    [PunRPC]
    public void otherPlayerRemoveBuildingTexture(noticePoint pointInfo)
    {
        //if this stage is previous complete (this stage's list is empty), cancel this stage completion status
        if (everyStageBuildProgress[pointInfo.stag].Count == 0)
        {
            stageCancelAnim(pointInfo.stag);
        }
        //add the removal shape to this stage's list
        everyStageBuildProgress[pointInfo.stag].Add(pointInfo.objShap);
        thisStagePutCount--;
        totalPutCount--;
        buildingProgressBarAnim(totalPutCount);
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
