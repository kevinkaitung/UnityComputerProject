using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
public class PlayerCamera : MonoBehaviourPun
{
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
    public float m_minimumY = -30f;
    public float m_maximumY = 45f;

    float m_rotationY = 0f;
    bool jump = false;

    // Start is called before the first frame update
    void Start()
    {
        //get player's camera
        //playerCam = GetComponentInChildren<Camera>();
    }

    void Awake()
    {
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
        playerCam.gameObject.AddComponent<CinemachineVirtualCamera>();
        playerCam.gameObject.GetComponent<CinemachineVirtualCamera>().Follow = transform;
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
        if (m_axes == RotationAxes.MouseXAndY)
        {
            float m_rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_sensitivityX;
            m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            transform.localEulerAngles = new Vector3(0, m_rotationX, 0);
            playerCam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
        }
        else if (m_axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * m_sensitivityX, 0);
        }
        else
        {
            m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
            m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

            transform.localEulerAngles = new Vector3(0, playerCam.transform.localEulerAngles.y, 0);
            playerCam.transform.localEulerAngles = new Vector3(-m_rotationY, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LeanTween.moveY(playerCam.gameObject, playerCam.transform.position.y + 3.0f, 0.3f).setOnComplete(down);
        }
    }

    void down()
    {
        LeanTween.moveY(playerCam.gameObject, playerCam.transform.position.y - 3.0f, 0.3f);
    }
}
