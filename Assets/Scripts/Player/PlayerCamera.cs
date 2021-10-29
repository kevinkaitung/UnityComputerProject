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
        //rotate about x-axis: rotate camera
        //rotate about y-axis: rotate character
        {
            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouselook.LookRotation(this.gameObject.transform, cam.transform);

            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            //m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            // if (m_axes == RotationAxes.MouseXAndY)
            // {
            //     float m_rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_sensitivityX;
            //     m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            //     m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            //     transform.localEulerAngles = new Vector3(0, m_rotationX, 0);
            //     playerCam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
            //     //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0/*transform.rotation.x*/, m_rotationX, 0/*transform.rotation.z*/), Time.deltaTime);
            //     //playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, Quaternion.Euler(-m_rotationY, 0/*playerCam.transform.rotation.y, playerCam.transform.rotation.z*/,0), Time.deltaTime);
            // }
            // else if (m_axes == RotationAxes.MouseX)
            // {
            //     transform.Rotate(0, Input.GetAxis("Mouse X") * m_sensitivityX, 0);
            //     //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0/*transform.rotation.x*/, Input.GetAxis("Mouse X") * m_sensitivityX, 0/*transform.rotation.z*/), Time.deltaTime);
            // }
            // else
            // {
            //     m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            //     m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            //     transform.localEulerAngles = new Vector3(0, playerCam.transform.localEulerAngles.y, 0);
            //     playerCam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
            //     //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0/*transform.rotation.x*/, playerCam.transform.localEulerAngles.y, 0/*transform.rotation.z*/), Time.deltaTime);
            //     //playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, Quaternion.Euler(-m_rotationY, 0/*playerCam.transform.rotation.y*/, 0/*playerCam.transform.rotation.z*/), Time.deltaTime);
            // }
        }
    }
}
