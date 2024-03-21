using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class PlayerClickActionforTeam : MonoBehaviourPun
{
    private Camera playerCam;
    public string holdMaterial;
    //public static bool bpc;     //for controlling freeze camera action
    private string team;    //which team belong to
    public GameObject showHoldMaterialCube, throwMaterialCube;
    //black hole effect block player click action
    public bool isBlackholeEffectForClick = false;
    private MeshRenderer throwMaterialMesh;
    [SerializeField]
    private LayerMask clickableMask;
    void Start()
    {
        //get player's camera(for raycast)
        this.holdMaterial = "empty";
        playerCam = GetComponentInChildren<Camera>();
        throwMaterialMesh = throwMaterialCube.GetComponent<MeshRenderer>();
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
        //DontDestroyOnLoad(gameObject);
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
            playerInteractiveUI.instance.noticePointInfoPanel.SetActive(false);
            playerInteractiveUI.instance.synthesisformulaPanel.SetActive(false);
            playerInteractiveUI.instance.materialFieldInfoPanel.SetActive(false);
        }
        if (!PlayerInputActionMode.instance.enablePlayerClickAction)
        {
            return;
        }
        //if blackhole effect is active, block player click action
        if (isBlackholeEffectForClick)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.gameObject.GetComponent<Transform>().position = gamePropsManager.instance.reSpawnPoints[gamePropsManager.instance.myRespawnPointIndex].position;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (holdMaterial == "removalToolMyself" || holdMaterial == "removalToolOther")
            {
                Debug.Log("You can't drop removal tool");
                teamGameLogicController.instance.actionWarnings("不可扔拆除工具");
            }
            else if (holdMaterial != "empty")
            {
                /*throwMaterialCube.transform.position = new Vector3(tempTransform.position.x, 1.5f, tempTransform.position.z) + tempTransform.forward * 5.0f + tempTransform.right * 3.0f;
                throwMaterialMesh.material = Resources.Load("materialTexture/Materials/" + holdMaterial) as Material;
                throwMaterialCube.tag = holdMaterial;
                PhotonNetwork.Instantiate("throwMaterialCube", throwMaterialCube.transform.position, throwMaterialCube.transform.rotation, 0);*/
                Transform tempTransform = this.transform;
                //call for instantiate throw material cube
                teamGameLogicController.instance.throwMaterialCubeInstantiate(new Vector3(tempTransform.position.x, 1.5f, tempTransform.position.z) + tempTransform.forward * 5.0f + tempTransform.right * 3.0f, holdMaterial);
                holdMaterial = "empty";
                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                //play sound effect
                AudioController.instance.actionPlaySound("throwMaterial");
            }
        }
        //player act with scene
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //check if clickable, and change cursor color
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, clickableMask.value))
        {
            if (Mathf.Pow(this.gameObject.transform.position.x - hit.point.x, 2) + Mathf.Pow(this.gameObject.transform.position.z - hit.point.z, 2) < 350)
            {
                PlayerInputActionMode.instance.fixedCenterCursorDetected();
            }
            else
            {
                PlayerInputActionMode.instance.fixedCenterCursorRest();
            }
        }
        else
        {
            PlayerInputActionMode.instance.fixedCenterCursorRest();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, clickableMask.value))
            {
                //click objcet when closing to it
                if (Mathf.Pow(this.gameObject.transform.position.x - hit.point.x, 2) + Mathf.Pow(this.gameObject.transform.position.z - hit.point.z, 2) < 350)
                {
                    Debug.Log(hit.transform.name);
                    if (hit.collider.tag == "noticePoint")
                    {
                        //check the player click the correct notice point
                        if (hit.collider.GetComponent<teamTag>().belongingTeam == team)
                        {
                            //should have building-available material
                            if (holdMaterial == "brick" || holdMaterial == "cement" || holdMaterial == "glass" || holdMaterial == "iron" || holdMaterial == "steel" || holdMaterial == "wood")
                            {
                                Debug.Log("click notice point");
                                //Get player clicked point's component to know clicked point's detail
                                noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                                //Pass detail of clicked point and player's hold material to game controller
                                teamGameLogicController.instance.playerPutThingsOnPoint(clickedPointInfo, holdMaterial, team);
                                //call master client to change the texture of notice cube of building(using RPC with notice point's photon view)
                                hit.collider.gameObject.GetComponent<PhotonView>().RPC("buildToChangeTexture", RpcTarget.All, holdMaterial);
                                //hit.collider.gameObject.GetComponent<ParticleSystem>().Play();
                                //photonView.RPC("playerPutThingsOnPoint", RpcTarget.All, clickedPointInfo, holdMaterial);
                                //after using the material, abandon the material
                                holdMaterial = "empty";
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                                //change hold material cube texture (networked)
                                showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                                //play sound effect
                                AudioController.instance.actionPlaySound("buildShape");
                            }
                            else
                            {
                                Debug.Log("You don't have any building-available materials");
                                teamGameLogicController.instance.actionWarnings("尚未擁有任何可建築建材");
                            }
                        }
                        else
                        {
                            Debug.Log("You can't build for other team");
                            teamGameLogicController.instance.actionWarnings("此為敵方建地");
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
                                //Get player clicked point's component to know clicked point's detail
                                noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                                //pass affected team to game logic controller
                                teamGameLogicController.instance.playerRemoveThing(clickedPointInfo, team);
                                //drop tool
                                holdMaterial = "empty";
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                                //change hold material cube texture (networked)
                                showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                                //play sound effect
                                AudioController.instance.actionPlaySound("removeShape");
                            }
                            else
                            {
                                Debug.Log("You can just remove your team's building");
                                teamGameLogicController.instance.actionWarnings("僅能拆除我方建築");
                            }
                        }
                        //use removal tool (remove other team)
                        else if (holdMaterial == "removalToolOther")
                        {
                            if (hit.collider.GetComponent<teamTag>().belongingTeam != team)
                            {
                                //change texture to transparent
                                hit.collider.gameObject.GetComponent<PhotonView>().RPC("removeBuildTexture", RpcTarget.All);
                                //Get player clicked point's component to know clicked point's detail
                                noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                                //pass affected team to game logic controller
                                teamGameLogicController.instance.playerRemoveThing(clickedPointInfo, hit.collider.GetComponent<teamTag>().belongingTeam);
                                //drop tool
                                holdMaterial = "empty";
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                                //change hold material cube texture (networked)
                                showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                                //play sound effect
                                AudioController.instance.actionPlaySound("removeShape");
                            }
                            else
                            {
                                Debug.Log("You can just remove other team's building");
                                teamGameLogicController.instance.actionWarnings("僅能拆除敵方建築");
                            }
                        }
                        else
                        {
                            Debug.Log("You should have removal tool");
                            teamGameLogicController.instance.actionWarnings("須擁有拆除工具");
                        }
                    }
                    else if (hit.collider.tag == "synthesis")
                    {
                        holdMaterial = Synthesis.instance.synthesis(holdMaterial);
                        teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                        //change hold material cube texture (networked)
                        showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                        if (holdMaterial != "empty" && holdMaterial != "removalToolMyself" && holdMaterial != "removalToolOther")
                        {
                            //play sound effect
                            AudioController.instance.actionPlaySound("getItem");
                        }
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
                            //play sound effect
                            AudioController.instance.actionPlaySound("getItem");
                        }
                    }
                }
                else
                {
                    Debug.Log("be more close to what you click");
                    teamGameLogicController.instance.actionWarnings("離點選物件太遠");
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
                    playerInteractiveUI.instance.showNoticePointInfo(clickedPointInfo);
                    playerInteractiveUI.instance.noticePointInfoPanel.SetActive(true);
                }
                else if (hit.collider.tag == "wood" || hit.collider.tag == "gravel" || hit.collider.tag == "iron" || hit.collider.tag == "water" || hit.collider.tag == "fire")
                {
                    Debug.Log("show material field info");
                    playerInteractiveUI.instance.materialFieldInfoPanel.SetActive(true);
                    playerInteractiveUI.instance.materialFieldInfoText.GetComponent<Text>().text = hit.collider.tag;
                    playerInteractiveUI.instance.materialFieldInfoImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("materialSprite/" + hit.collider.tag);
                }
                else if (hit.collider.tag == "synthesis")
                {
                    Debug.Log("show synthesis formula");
                    playerInteractiveUI.instance.synthesisformulaPanel.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!PlayerInputActionMode.instance.enablePlayerClickAction)
        {
            return;
        }
        //if blackhole effect is active, block player click action
        if (isBlackholeEffectForClick)
        {
            return;
        }
        PhotonView tempPV = null;
        if (other.gameObject.GetComponentInParent<PhotonView>())
        {
            tempPV = other.gameObject.GetComponentInParent<PhotonView>();
            if (tempPV.enabled == false)
            {
                return;
            }
        }
        else
        {
            return;
        }
        if (other.tag == "itembox")
        {
            //after collision, destroy the game prop (only master client destroy the networked object)
            Debug.Log(tempPV.ViewID);
            //有時候不知道為什麼會偵測到碰撞兩次道具箱的樣子(所以先用is active檢查避免)
            /*if (hit.collider.gameObject.GetComponent<PhotonView>().isActiveAndEnabled)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
                gamePropsManager.instance.clickGameProps(team);
                hit.collider.gameObject.GetComponent<PhotonView>().enabled = false;
            }
            else
            {
                Debug.Log("item box photonview has problem");
            }*/
            tempPV.RPC("destroyObject", RpcTarget.MasterClient);
            gamePropsManager.instance.clickGameProps(team);
            if (tempPV)
            {
                tempPV.enabled = false;
            }
            //play sound effect
            AudioController.instance.actionPlaySound("itemBoxGet");
        }
        else if (other.tag == "removalToolMyself")
        {
            Debug.Log(tempPV.ViewID);
            //after collision, destroy the game prop (only master client destroy the networked object)
            tempPV.RPC("destroyObject", RpcTarget.MasterClient);
            holdMaterial = "removalToolMyself";
            teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
            showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
            if (tempPV)
            {
                tempPV.enabled = false;
            }
            //play sound effect
            AudioController.instance.actionPlaySound("getItem");
        }
        else if (other.tag == "removalToolOther")
        {
            Debug.Log(tempPV.ViewID);
            //after collision, destroy the game prop (only master client destroy the networked object)
            tempPV.RPC("destroyObject", RpcTarget.MasterClient);
            holdMaterial = "removalToolOther";
            teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
            showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
            if (tempPV)
            {
                tempPV.enabled = false;
            }
            //play sound effect
            AudioController.instance.actionPlaySound("getItem");
        }
        else if (other.gameObject.layer == 2) //layer ignore raycast
        {
            if (other.tag == "brick" || other.tag == "cement" || other.tag == "fire" || other.tag == "glass" || other.tag == "gravel" || other.tag == "iron" || other.tag == "steel" || other.tag == "water" || other.tag == "wood")
            {
                holdMaterial = other.tag;
                Debug.Log(holdMaterial);
                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                //change hold material cube texture (networked)
                showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
                tempPV.RPC("destroyObject", RpcTarget.All);
                if (tempPV)
                {
                    tempPV.enabled = false;
                }
                //play sound effect
                AudioController.instance.actionPlaySound("getItem");
            }
        }
    }

    //collision to itembox, activate effect
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // if (!PlayerInputActionMode.instance.enablePlayerClickAction)
        // {
        //     return;
        // }
        // //if blackhole effect is active, block player click action
        // if (isBlackholeEffect)
        // {
        //     return;
        // }
        // PhotonView tempPV = null;
        // if (hit.collider.gameObject.GetComponentInParent<PhotonView>())
        // {
        //     tempPV = hit.collider.gameObject.GetComponentInParent<PhotonView>();
        //     if (tempPV.enabled == false)
        //     {
        //         return;
        //     }
        // }
        // else
        // {
        //     return;
        // }
        // if (hit.collider.tag == "itembox")
        // {
        //     //after collision, destroy the game prop (only master client destroy the networked object)
        //     Debug.Log(tempPV.ViewID);
        //     //有時候不知道為什麼會偵測到碰撞兩次道具箱的樣子(所以先用is active檢查避免)
        //     /*if (hit.collider.gameObject.GetComponent<PhotonView>().isActiveAndEnabled)
        //     {
        //         hit.collider.gameObject.GetComponent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
        //         gamePropsManager.instance.clickGameProps(team);
        //         hit.collider.gameObject.GetComponent<PhotonView>().enabled = false;
        //     }
        //     else
        //     {
        //         Debug.Log("item box photonview has problem");
        //     }*/
        //     tempPV.RPC("destroyObject", RpcTarget.MasterClient);
        //     gamePropsManager.instance.clickGameProps(team);
        //     if (tempPV)
        //     {
        //         tempPV.enabled = false;
        //     }
        // }
        // else if (hit.collider.tag == "removalToolMyself")
        // {
        //     Debug.Log(tempPV.ViewID);
        //     //after collision, destroy the game prop (only master client destroy the networked object)
        //     tempPV.RPC("destroyObject", RpcTarget.MasterClient);
        //     holdMaterial = "removalToolMyself";
        //     teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
        //     if (tempPV)
        //     {
        //         tempPV.enabled = false;
        //     }
        // }
        // else if (hit.collider.tag == "removalToolOther")
        // {
        //     Debug.Log(tempPV.ViewID);
        //     //after collision, destroy the game prop (only master client destroy the networked object)
        //     tempPV.RPC("destroyObject", RpcTarget.MasterClient);
        //     holdMaterial = "removalToolOther";
        //     teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
        //     if (tempPV)
        //     {
        //         tempPV.enabled = false;
        //     }
        // }
        // else if (hit.gameObject.layer == 2) //layer ignore raycast
        // {
        //     if (hit.collider.tag == "brick" || hit.collider.tag == "cement" || hit.collider.tag == "fire" || hit.collider.tag == "glass" || hit.collider.tag == "gravel" || hit.collider.tag == "iron" || hit.collider.tag == "steel" || hit.collider.tag == "water" || hit.collider.tag == "wood")
        //     {
        //         holdMaterial = hit.collider.tag;
        //         Debug.Log(holdMaterial);
        //         teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
        //         //change hold material cube texture (networked)
        //         showHoldMaterialCube.GetComponent<PhotonView>().RPC("showPlayerHoldMaterialCube", RpcTarget.All, holdMaterial);
        //         tempPV.RPC("destroyObject", RpcTarget.All);
        //         if (tempPV)
        //         {
        //             tempPV.enabled = false;
        //         }
        //     }
        // }
    }
}
