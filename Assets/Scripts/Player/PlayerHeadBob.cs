using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Characters.FirstPerson;
using Photon.Pun;

public class PlayerHeadBob : MonoBehaviourPun
{
    public Camera Camera;
    public CurveControlledBob motionBob = new CurveControlledBob();
    public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
    public RigidbodyFirstPersonController rigidbodyFirstPersonController;
    public PlayerMovement playerMovementController;
    public float StrideInterval;
    [Range(0f, 1f)] public float RunningStrideLengthen;

    // private CameraRefocus m_CameraRefocus;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;


    private void Start()
    {
        motionBob.Setup(Camera, StrideInterval);
        m_OriginalCameraPosition = Camera.transform.localPosition;
        //     m_CameraRefocus = new CameraRefocus(Camera, transform.root.transform, Camera.transform.localPosition);
    }


    private void Update()
    {
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }
        if(!PlayerInputActionMode.instance.enablePlayerMovement)
        {
            return;
        }
        //  m_CameraRefocus.GetFocusPoint();
        Vector3 newCameraPosition;
        if (playerMovementController.move.magnitude > 0 && playerMovementController.isGround)
        {
            //Camera.transform.localPosition = motionBob.DoHeadBob(playerMovementController.move.magnitude*4.0f*(/*rigidbodyFirstPersonController.Running*/ false ? RunningStrideLengthen : 1f));
            Camera.transform.localPosition = motionBob.DoHeadBob(playerMovementController.changedSpeedHeadBob);
            newCameraPosition = Camera.transform.localPosition;
            newCameraPosition.y = Camera.transform.localPosition.y - jumpAndLandingBob.Offset();
        }
        else
        {
            //when jumping, stop head bob(?
            newCameraPosition = Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - jumpAndLandingBob.Offset();
        }
        Camera.transform.localPosition = newCameraPosition;
        if (!m_PreviouslyGrounded && playerMovementController.isGround)
        {
            StartCoroutine(jumpAndLandingBob.DoBobCycle());
        }

        m_PreviouslyGrounded = playerMovementController.isGround;
        //  m_CameraRefocus.SetFocusPoint();
    }
}
