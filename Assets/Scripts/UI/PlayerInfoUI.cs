using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.EventSystems;

public class PlayerInfoUI : MonoBehaviourPun, IPointerEnterHandler
{
    [SerializeField]
    public Text PlayerNameText;
    //main camera(for UI)(god camera)
    private Camera cam;
    [SerializeField]
    private GameObject thisCharacter;
    public Vector2 currentPlayerPos;
    public string myTeam;

    public GameObject infoBackGround;
    public GameObject playerMark;
    public GameObject containerImage;
    //public Collider2D col;

    void setUpInfo()
    {
        //set self different color
        infoBackGround.GetComponent<Image>().color = new Color(1.0f, 0.8f, 0.52f, 1.0f);
        //prefab initialization after instantiate
        PlayerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        myTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;
        cam = GodViewPlayersInfo.instance.godCamera.GetComponent<Camera>();
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        this.gameObject.transform.SetParent(GodViewPlayersInfo.instance.GodViewPlayersInfoPanel.GetComponent<Transform>());
        this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (myTeam == "Red")
        {
            playerMark.GetComponent<Image>().color = Color.red;
        }
        else
        {
            playerMark.GetComponent<Image>().color = Color.blue;
        }
        //after initialization, call godview manager I'm ready
        GodViewPlayersInfo.instance.instantiateOk();
        //call other players same prefab to initialization
        photonView.RPC("otherSetUpInfo", RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, myTeam);
    }

    //other players initialization
    [PunRPC]
    void otherSetUpInfo(string nickName, string myteam)
    {
        PlayerNameText.text = nickName;
        myTeam = myteam;
        cam = GodViewPlayersInfo.instance.godCamera.GetComponent<Camera>();
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        this.gameObject.transform.SetParent(GodViewPlayersInfo.instance.GodViewPlayersInfoPanel.GetComponent<Transform>());
        //this.gameObject.transform.SetParent(GodViewPlayersInfo.instance.canvas.GetComponent<Transform>());
        this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (myTeam == "Red")
        {
            playerMark.GetComponent<Image>().color = Color.red;
        }
        else
        {
            playerMark.GetComponent<Image>().color = Color.blue;
        }
        //after initialization, call godview manager I'm ready
        GodViewPlayersInfo.instance.instantiateOk();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            setUpInfo();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform player position in world space to the coordinate on canvas
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(GodViewPlayersInfo.instance.canvas.GetComponent<RectTransform>(), cam.WorldToScreenPoint(thisCharacter.transform.position), null, out currentPlayerPos);
        transform.position = cam.WorldToScreenPoint(thisCharacter.transform.position);
    }

    //when mouse hover on the player info UI
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //let this player info UI show on the top
        this.transform.SetAsLastSibling();
    }

    void LateUpdate()
    {

    }
}
