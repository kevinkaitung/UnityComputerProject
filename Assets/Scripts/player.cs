using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using LitJson;

public class player : MonoBehaviourPun
{
    public string holdMaterial;
    public GameObject synthesis;
    private Camera playerCam;
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes m_axes = RotationAxes.MouseXAndY;
    public float m_sensitivityX = 10f;
    public float m_sensitivityY = 10f;

    // 水平方向的 镜头转向
    public float m_minimumX = -360f;
    public float m_maximumX = 360f;
    // 垂直方向的 镜头转向 (这里给个限度 最大仰角为45°)
    public float m_minimumY = -45f;
    public float m_maximumY = 45f;

    float m_rotationY = 0f;
    // Start is called before the first frame update
    /*void Start()
    {
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
    }*/


    public TextAsset jsonFileSynthesis;     //json file position

    void Start()
    {
        synthesis = GameObject.Find("SynthesisField");
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
        if (!photonView.IsMine)
        {
            //close other's camera to avoid rendering other's camera scene
            playerCam.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKey("up")) { transform.Translate(0, 0, 0.1f); }
        // 按住 上鍵 時，物件每個 frame 朝自身 z 軸方向移動 0.1 公尺

        if (Input.GetKey("down")) { transform.Translate(0, 0, -0.1f); }
        // 按住 下鍵 時，物件每個 frame 朝自身 z 軸方向移動 -0.1 公尺

        if (Input.GetKey("left")) { transform.Rotate(0, -0.3f, 0); }
        // 按住 左鍵 時，物件每個 frame 以自身 y 軸為軸心旋轉 -3 度

        if (Input.GetKey("right")) { transform.Rotate(0, 0.3f, 0); }
        // 按住 右鍵 時，物件每個 frame 以自身 y 軸為軸心旋轉 3 度

        if (m_axes == RotationAxes.MouseXAndY)
        {
            float m_rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_sensitivityX;
            m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            transform.localEulerAngles = new Vector3(-m_rotationY, m_rotationX, 0);
        }
        else if (m_axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * m_sensitivityX, 0);
        }
        else
        {
            m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            transform.localEulerAngles = new Vector3(-m_rotationY, transform.localEulerAngles.y, 0);
        }
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.transform.name);
                if (hit.collider.tag == "noticePoint")
                {
                    Debug.Log("click notice point");
                    //Get player clicked point's component to know clicked point's detail
                    noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                    //Pass detail of clicked point and player's hold material to game controller
                    gameLogicController.instance.playerPutThingsOnPoint(clickedPointInfo, holdMaterial);
                    //photonView.RPC("playerPutThingsOnPoint", RpcTarget.All, clickedPointInfo, holdMaterial);
                }
                if (hit.collider.tag == "stone")
                {
                    holdMaterial = "stone";
                    gameLogicController.instance.showPlayerHandyMaterial("stone");
                }
                else if (hit.collider.tag == "brick")
                {
                    holdMaterial = "brick";
                    gameLogicController.instance.showPlayerHandyMaterial("brick");
                }
                if (hit.collider.tag == "synthesis")
                {
                    if (synthesis.GetComponent<synthesisDataNodes>().firstInputItem == "empty")
                    {
                        synthesis.GetComponent<synthesisDataNodes>().firstInputItem = holdMaterial;
                    }
                    else if (synthesis.GetComponent<synthesisDataNodes>().secondInputItem == "empty" && synthesis.GetComponent<synthesisDataNodes>().firstInputItem != holdMaterial)
                    {
                        synthesis.GetComponent<synthesisDataNodes>().secondInputItem = holdMaterial;
                    }
                    if (synthesis.GetComponent<synthesisDataNodes>().firstInputItem != "empty" && synthesis.GetComponent<synthesisDataNodes>().secondInputItem != "empty")
                    {
                        string result = check(synthesis.GetComponent<synthesisDataNodes>().firstInputItem, synthesis.GetComponent<synthesisDataNodes>().secondInputItem);
                        if (result != "empty")
                        {
                            synthesis.GetComponent<synthesisDataNodes>().firstInputItem = "empty";
                            synthesis.GetComponent<synthesisDataNodes>().secondInputItem = "empty";
                            holdMaterial = result;
                            gameLogicController.instance.showPlayerHandyMaterial(result);
                        }
                    }
                }
            }
        }
    }
    string check(string item1, string item2)
    {
        string datas = jsonFileSynthesis.text;
        AllData allData;
        allData = JsonMapper.ToObject<AllData>(datas);
        //allData["synthesisDataNodes"][0]["firstInputItem"];
        foreach (var data in allData.synthesisDataNodes)
        {
            Debug.Log(data.firstInputItem);
            Debug.Log(data.secondInputItem);
            Debug.Log(data.outputItem);
            if (item1 == data.firstInputItem && item2 == data.secondInputItem)
            {
                return data.outputItem;
            }
            if (item2 == data.firstInputItem && item1 == data.secondInputItem)
            {
                return data.outputItem;
            }
        }
        return "empty";
    }
}
