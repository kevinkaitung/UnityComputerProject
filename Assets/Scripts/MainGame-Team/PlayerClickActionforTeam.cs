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
    public static bool bpc;     //for controlling freeze camera action
    private string team;    //which team belong to

    void Start()
    {
        //get player's camera(for raycast)
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
            if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
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
                        if (Synthesis.instance.firstInputItem == "empty" && holdMaterial != "empty")
                        {
                            Synthesis.instance.firstInputItem = holdMaterial;
                        }
                        else if (Synthesis.instance.secondInputItem == "empty" && Synthesis.instance.firstInputItem != holdMaterial && holdMaterial != "empty")
                        {
                            Synthesis.instance.secondInputItem = holdMaterial;
                        }
                        if (Synthesis.instance.firstInputItem != "empty" && Synthesis.instance.secondInputItem != "empty")
                        {
                            string result = Synthesis.instance.check(Synthesis.instance.firstInputItem, Synthesis.instance.secondInputItem);
                            if (result != "empty")
                            {
                                Synthesis.instance.firstInputItem = "empty";
                                Synthesis.instance.secondInputItem = "empty";
                                holdMaterial = result;
                                teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                            }
                        }
                    }
                    //click other material
                    else if (hit.collider.tag == "wood" || hit.collider.tag == "gravel" || hit.collider.tag == "iron" || hit.collider.tag == "water" || hit.collider.tag == "fire")
                    {
                        holdMaterial = hit.collider.tag;
                        teamGameLogicController.instance.showPlayerHandyMaterial(holdMaterial);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
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
