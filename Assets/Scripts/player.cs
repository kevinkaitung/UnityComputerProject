using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using LitJson;
using MongoDB.Bson;
using MongoDB.Driver;

public class player : MonoBehaviourPun
{
    public string holdMaterial;
    //public GameObject synthesis;
    private Camera playerCam;
    public bool bpc;
    Button closeblueprint;
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
    Canvas Blueprint, obj;
    Text objname;
    // Start is called before the first frame update
    /*void Start()
    {
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
    }*/

    void Start()
    {
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
        //synthesis = GameObject.Find("Synthesis");
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        Blueprint = GameObject.Find("Blueprint").GetComponent<Canvas>();
        Blueprint.enabled = false;
        obj = GameObject.Find("obj").GetComponent<Canvas>();
        obj.enabled = false;
        objname = GameObject.Find("objname").GetComponent<Text>();
        closeblueprint = Blueprint.GetComponentInChildren<Button>();
        bpc = false;
        this.holdMaterial = "empty";
        this.gameObject.layer = 8;
        playerCam.cullingMask &= ~(1 << 8);
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
        if (!bpc)
        {
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
        }
        closeblueprint.onClick.AddListener(delegate{
            hideBlueprint();
        });
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
                if (hit.collider.tag == "blueprintcube")
                {
                    showBlueprint();
                }
                if (hit.collider.tag == "stone")
                {
                    holdMaterial = "stone";
                }
                if (hit.collider.tag == "brick")
                {
                    holdMaterial = "brick";
                }
                if (hit.collider.tag == "synthesis")
                {
                    if (Synthesis.instance.FirstInputItem == "empty" && holdMaterial != "empty")
                    {
                        Synthesis.instance.FirstInputItem = holdMaterial;
                    }
                    else if (Synthesis.instance.SecondInputItem == "empty" && Synthesis.instance.FirstInputItem != holdMaterial && holdMaterial != "empty")
                    {
                        Synthesis.instance.SecondInputItem = holdMaterial;
                    }
                    if (Synthesis.instance.FirstInputItem != "empty" && Synthesis.instance.SecondInputItem != "empty")
                    {
                        string result = Synthesis.instance.check(Synthesis.instance.FirstInputItem, Synthesis.instance.SecondInputItem);
                        if (result != "empty")
                        {
                            Synthesis.instance.FirstInputItem = "empty";
                            Synthesis.instance.SecondInputItem = "empty";
                            holdMaterial = result;
                        }
                    }
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
                    showNoticePointInfo(clickedPointInfo);
                    bpc = true;
                    obj.enabled = true;
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            obj.enabled = false;
            bpc = false;
        }
    }
    /*string check(string item1, string item2)
    {
        var client = new MongoClient("mongodb+srv://exriesz:unity00757014@exriesz.lxfdc.mongodb.net/unity?retryWrites=true&w=majority");
        var database = client.GetDatabase("unity"); //數據庫名稱
        var collection = database.GetCollection<BsonDocument>("synthesisData");//連接的表名
        var list = collection.Find(_ => true).ToList();
        list[0].Remove("_id");
        var datas = list[0].ToJson();          //File.ReadAllText(Application.dataPath + "/Resources/synthesisData.json");
        Debug.Log(datas);
        AllData allData;
        allData = JsonMapper.ToObject<AllData>(datas);
        Debug.Log(allData);
        foreach (var data in allData.synthesisDataNodes)
        {
            if (item1 == data.FirstInputItem && item2 == data.SecondInputItem)
            {
                return data.OutputItem;
            }
            if (item2 == data.FirstInputItem && item1 == data.SecondInputItem)
            {
                return data.OutputItem;
            }
        }
        return "empty";
    }*/
    void showBlueprint()
    {
        Blueprint.enabled = true;
        bpc = true;
    }
    void hideBlueprint()
    {
        Blueprint.enabled = false;
        bpc = false;
    }
    
    void showNoticePointInfo(noticePoint ntp)
    {
        int i = 0;
        while(true)
        {
            if(nodeManager.instance.dataRoot.gameDataNodes[i].position == ntp.pos)
            {
                objname.text = "The name of this notice point is " + ntp.objShap + ".\n";
                break;
            }
            else
            {
                i++;
            }
            if(i>100)
                break;
        }
    }

}
