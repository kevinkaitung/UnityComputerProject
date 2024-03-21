using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Cinemachine;
using UnityStandardAssets.Characters.FirstPerson;
public class PlayerCamera : MonoBehaviourPun
{
    public MouseLook mouselook = new MouseLook();
    public GameObject cam;
    private Camera playerCam;
    [SerializeField]
    private CameraShake camShak;
    public bool shakeNow = false;
    //black hole effect block player camera action
    public bool isBlackholeEffectForCam = false;
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes m_axes = RotationAxes.MouseXAndY;
    public float m_sensitivityX = 1f;
    public float m_sensitivityY = 1f;

    // 水平方向的 镜头转向
    public float m_minimumX = -360f;
    public float m_maximumX = 360f;
    // 垂直方向的 镜头转向 (这里给个限度 最大仰角为45°)
    public float m_minimumY = -30f;
    public float m_maximumY = 65f;
    private string myTeam;

    float m_rotationY = 0f;
    Vector3 temp;

    // Start is called before the first frame update
    void Start()
    {
        //get player's camera
        //playerCam = GetComponentInChildren<Camera>();
    }

    void Awake()
    {
        mouselook.Init(this.gameObject.transform, cam.transform);
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
        //playerCam.gameObject.AddComponent<CinemachineVirtualCamera>();
        //playerCam.gameObject.GetComponent<CinemachineVirtualCamera>().Follow = transform;
        myTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;
        temp = playerCam.transform.position;
        //playerCam.gameObject.GetComponent<CinemachineVirtualCamera>().LookAt = transform;
        if (!photonView.IsMine)
        {
            //close other's camera to avoid rendering other's camera scene
            playerCam.gameObject.SetActive(false);
        }
        /*for(int i = 0; i < 10; i++)
        {
            if(PhotonNetwork.NickName == teamGameLogicController.instance.playerlist[i])
            {
                PhotonNetwork.NickName = PhotonNetwork.NickName + "new";
                Debug.Log("hajsklsd");
                this.gameObject.layer = LayerMask.NameToLayer(teamGameLogicController.instance.layerlist[i]);
                foreach(Transform j in GetComponentsInChildren<Transform>())
                {
			        j.gameObject.layer = LayerMask.NameToLayer(teamGameLogicController.instance.layerlist[i]);//更改物體的Layer層
		        }
                playerCam.cullingMask =~(1 << i+10);
                break;
            }
        }*/
        if (photonView.IsMine)
        {
            this.gameObject.layer = LayerMask.NameToLayer("#0" + PhotonNetwork.LocalPlayer.ActorNumber.ToString());
            foreach (Transform j in GetComponentsInChildren<Transform>())
            {
                j.gameObject.layer = LayerMask.NameToLayer("#0" + PhotonNetwork.LocalPlayer.ActorNumber.ToString());//更改物體的Layer層
            }
            playerCam.cullingMask = ~(1 << PhotonNetwork.LocalPlayer.ActorNumber + 9);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Cursor.visible = false;
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }
        //if open blueprint or see notice point info, freeze camera action
        if (!PlayerInputActionMode.instance.enableCameraControl)
        {
            return;
        }
        //if blackhole effect is active, block player camera action
        if (isBlackholeEffectForCam)
        {
            //stop shake camera when affected by blackhole
            camShak.stopShake();
            shakeNow = false;
            return;
        }
        // if camera now is shaking, lock the camera from player's input
        if (shakeNow)
        {
            return;
        }
        //rotate about x-axis: rotate camera
        //rotate about y-axis: rotate character
        {
            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouselook.LookRotation(this.gameObject.transform, cam.transform);

            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(!photonView.IsMine)
        {
            return;
        }
        //if collide flame obstacle, shake camera
        if (hit.collider.tag == "flame")
        {
            if (!shakeNow)
            {
                shakeNow = true;
                //shake camera
                camShak.cameraShake();
            }
        }
    }
}
