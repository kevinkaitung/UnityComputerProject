﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerClickActionforTeam : MonoBehaviourPun
{
    private Camera playerCam;
    public string holdMaterial;
    //public static bool bpc;     //for controlling freeze camera action
    private string team;    //which team belong to
    public GameObject showHoldMaterialCube;
    //black hole effect block player click action
    public bool isBlackholeEffect = false;

    void Start()
    {
        //get player's camera(for raycast)
        this.holdMaterial = "empty";
        playerCam = GetComponentInChildren<Camera>();
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        //get team
        object tmp;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("_pt", out tmp);
        if ((byte)tmp == 1)
        {
            team = "blue";
        }
        else
        {
            team = "red";
        }
    }

    void Awake()
    {
        //get player's camera(for raycast)
        playerCam = GetComponentInChildren<Camera>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            bluePrint.instance.noticePointInfoPanel.SetActive(false);
            bluePrint.instance.synthesisformulaPanel.SetActive(false);
        }
        if (!PlayerInputActionMode.instance.enablePlayerClickAction)
        {
            return;
        }
        //if blackhole effect is active, block player click action
        if (isBlackholeEffect)
        {
            return;
        }
        //player act with scene
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                //click objcet when closing to it
                if (Mathf.Pow(this.gameObject.transform.position.x - hit.point.x, 2) + Mathf.Pow(this.gameObject.transform.position.z - hit.point.z, 2) < 500)
                {
                    Debug.Log(hit.transform.name);
                    if (hit.collider.tag == "noticePoint")
                    {
                        //check the player click the correct notice point
                        if (hit.collider.GetComponent<teamTag>().belongingTeam == team)
                        {
                            if (holdMaterial != "empty")
                            {
                                Debug.Log("click notice point");
                                //Get player clicked point's component to know clicked point's detail
                                noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                                //Pass detail of clicked point and player's hold material to game controller
                                teamGameLogicController.instance.playerPutThingsOnPoint(clickedPointInfo, holdMaterial, team);
                                //call master client to change the texture of notice cube of building(using RPC with notice point's photon view)
                                hit.collider.gameObject.GetComponent<PhotonView>().RPC("buildToChangeTexture", RpcTarget.All, holdMaterial);
                                //photonView.RPC("playerPutThingsOnPoint", RpcTarget.All, clickedPointInfo, holdMaterial);
                                //after using the material, abandon the material
                                holdMaterial = "empty";
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                                //change hold material cube texture (networked)
                                showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                            }
                            else
                            {
                                Debug.Log("You don't have any materials");
                                teamGameLogicController.instance.actionWarnings("You don't have any materials");
                            }
                        }
                        else
                        {
                            Debug.Log("You can't build for other team");
                            teamGameLogicController.instance.actionWarnings("You can't build for other team");
                        }
                    }
                    //click builded building
                    else if (hit.collider.tag == "clickedNoticePoint")
                    {
                        //use removal tool (remove my team)
                        if (holdMaterial == "removalToolMyself")
                        {
                            if (hit.collider.GetComponent<teamTag>().belongingTeam == team)
                            {
                                //change texture to transparent
                                hit.collider.gameObject.GetComponent<PhotonView>().RPC("removeBuildTexture", RpcTarget.All);
                                //pass affected team to game logic controller
                                teamGameLogicController.instance.playerRemoveThing(team);
                                //drop tool
                                holdMaterial = "empty";
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                            }
                            else
                            {
                                Debug.Log("You can just remove your team's building");
                                teamGameLogicController.instance.actionWarnings("You can just remove your team's building");
                            }
                        }
                        //use removal tool (remove other team)
                        else if (holdMaterial == "removalToolOther")
                        {
                            if (hit.collider.GetComponent<teamTag>().belongingTeam != team)
                            {
                                //change texture to transparent
                                hit.collider.gameObject.GetComponent<PhotonView>().RPC("removeBuildTexture", RpcTarget.All);
                                //pass affected team to game logic controller
                                teamGameLogicController.instance.playerRemoveThing(hit.collider.GetComponent<teamTag>().belongingTeam);
                                //drop tool
                                holdMaterial = "empty";
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                            }
                            else
                            {
                                Debug.Log("You can just remove other team's building");
                                teamGameLogicController.instance.actionWarnings("You can just remove other team's building");
                            }
                        }
                        else
                        {
                            Debug.Log("You should have removal tool");
                            teamGameLogicController.instance.actionWarnings("You should have removal tool");
                        }
                    }
                    else if (hit.collider.tag == "synthesis")
                    {
                        holdMaterial = Synthesis.instance.synthesis(holdMaterial);
                        teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                        //change hold material cube texture (networked)
                        showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                    }
                    //click other material
                    else if (hit.collider.tag == "wood" || hit.collider.tag == "gravel" || hit.collider.tag == "iron" || hit.collider.tag == "water" || hit.collider.tag == "fire")
                    {
                        string temp = hit.collider.tag;
                        teamGameLogicController.instance.playerGetMaterial(temp);
                        //check if successfully get the material
                        if (teamGameLogicController.instance.isGetMat)
                        {
                            //if successful, change hold material
                            holdMaterial = temp;
                            Debug.Log(holdMaterial);
                            teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                            //change hold material cube texture (networked)
                            showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                        }
                    }
                }
                else if (hit.collider.tag != "ground")
                {
                    Debug.Log("be more close to what you click");
                    teamGameLogicController.instance.actionWarnings("Closer to click the object.");
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.transform.name);
                if (hit.collider.tag == "noticePoint")
                {
                    Debug.Log("show notice point info");
                    noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                    bluePrint.instance.showNoticePointInfo(clickedPointInfo);
                    bluePrint.instance.noticePointInfoPanel.SetActive(true);
                }
                else if (hit.collider.tag == "synthesis")
                {
                    Debug.Log("show synthesis formula");
                    bluePrint.instance.synthesisformulaPanel.SetActive(true);
                }
            }
        }
    }

    //collision to itembox, activate effect
    private void OnCollisionEnter(Collision other)
    {
        if (!PlayerInputActionMode.instance.enablePlayerClickAction)
        {
            return;
        }
        //if blackhole effect is active, block player click action
        if (isBlackholeEffect)
        {
            return;
        }
        if (other.collider.tag == "itembox")
        {
            //after collision, destroy the game prop (only master client destroy the networked object)
            other.collider.gameObject.GetComponentInParent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
            gamePropsManager.instance.clickGameProps(team);
        }
        else if (other.collider.tag == "removalToolMyself")
        {
            //after collision, destroy the game prop (only master client destroy the networked object)
            other.collider.gameObject.GetComponentInParent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
            holdMaterial = "removalToolMyself";
            teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
        }
        else if (other.collider.tag == "removalToolOther")
        {
            //after collision, destroy the game prop (only master client destroy the networked object)
            other.collider.gameObject.GetComponentInParent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
            holdMaterial = "removalToolOther";
            teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
        }
    }
}
