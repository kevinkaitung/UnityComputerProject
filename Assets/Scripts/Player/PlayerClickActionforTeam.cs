using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerClickActionforTeam : MonoBehaviourPun
{
    private static PlayerClickActionforTeam s_Instance = null;
    public static PlayerClickActionforTeam instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(PlayerClickActionforTeam)) as PlayerClickActionforTeam;

                if (s_Instance == null)
                    Debug.Log("Could not locate a PlayerClickActionforTeam " +
                              "object. \n You have to have exactly " +
                              "one PlayerClickActionforTeam in the scene.");
            }
            return s_Instance;
        }
    }
    private Camera playerCam;
    public string holdMaterial;
    public static bool bpc;     //for controlling freeze camera action
    private string team;    //which team belong to

    void Start()
    {
        //get player's camera(for raycast)
        this.holdMaterial = "empty";
        playerCam = GetComponentInChildren<Camera>();
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        bpc = false;
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

        bluePrint.instance.closeBlueprintButton.onClick.AddListener(delegate
        {
            bluePrint.instance.blueprintPanel.SetActive(false);
            bpc = false;
            Cursor.lockState = CursorLockMode.Locked;
        });


        //player act with scene
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (!bpc)
            {
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
                                    Debug.Log("click notice point");
                                    //Get player clicked point's component to know clicked point's detail
                                    noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                                    //Pass detail of clicked point and player's hold material to game controller
                                    teamGameLogicController.instance.playerPutThingsOnPoint(clickedPointInfo, holdMaterial, team);
                                    //call master client to change the texture of notice cube of building(using RPC with notice point's photon view)
                                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("buildToChangeTexture", RpcTarget.All, holdMaterial);
                                    //photonView.RPC("playerPutThingsOnPoint", RpcTarget.All, clickedPointInfo, holdMaterial);
                                }
                                else
                                {
                                    Debug.Log("You can't build for other team");
                                }
                            }
                            else if (hit.collider.tag == "bluePrint")
                            {
                                bluePrint.instance.blueprintPanel.SetActive(true);
                                bpc = true;
                            }
                            else if (hit.collider.tag == "synthesis")
                            {
                               Synthesis.instance.synthesis(holdMaterial);
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
                                }
                            }
                            //click for game props
                            else if (hit.collider.tag == "itembox")
                            {
                                //after clicking, destroy the game prop (only master client destroy the networked object)
                                hit.collider.gameObject.GetComponentInParent<PhotonView>().RPC("destroyObject", RpcTarget.MasterClient);
                                gamePropsManager.instance.clickGameProps(team);
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
                            bpc = true;
                            bluePrint.instance.noticePointInfoPanel.SetActive(true);
                        }
                        else if (hit.collider.tag == "synthesis")
                        {
                            Debug.Log("show synthesis formula");
                            bpc = true;
                            bluePrint.instance.synthesisformulaPanel.SetActive(true);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                bluePrint.instance.noticePointInfoPanel.SetActive(false);
                bluePrint.instance.synthesisformulaPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                bpc = false;
            }
        }
    }
}
