using System.Collections;
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
    private string materialTexture;
    private MeshRenderer holdMaterialMesh;

    void Start()
    {
        //get player's camera(for raycast)
        this.holdMaterial = "empty";
        holdMaterialMesh = showHoldMaterialCube.GetComponent<MeshRenderer>();
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
        if(holdMaterial == "empty")
        {
            showHoldMaterialCube.SetActive(false);            
        }
        if (Input.GetMouseButtonUp(1))
        {
            bluePrint.instance.noticePointInfoPanel.SetActive(false);
            bluePrint.instance.synthesisformulaPanel.SetActive(false);
        }
        if(!PlayerInputActionMode.instance.enablePlayerClickAction)
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
                            }
                            else
                            {
                                Debug.Log("You don't have any materials");
                            }
                        }
                        else
                        {
                            Debug.Log("You can't build for other team");
                        }
                    }
                    else if (hit.collider.tag == "synthesis")
                    {
                        holdMaterial = Synthesis.instance.synthesis(holdMaterial);
                        teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
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
                            holdMaterialMesh.material = Resources.Load("materialTexture/Materials/" + holdMaterial) as Material;
                            showHoldMaterialCube.SetActive(true);
                        }
                    }
                }
                else if (hit.collider.tag != "ground")
                {
                    Debug.Log("be more close to what you click");
                    teamGameLogicController.instance.tooFarClickNotice();
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
        if (other.collider.tag == "itembox")
        {
            //after collision, destroy the game prop (only master client destroy the networked object)
            other.collider.gameObject.GetComponentInParent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
            gamePropsManager.instance.clickGameProps(team);
        }
    }
}
