using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTeachingStage : MonoBehaviourPun
{
    private Camera playerCam;
    private float timer = 0.0f;
    private float durationn = 60.0f;
    private float timerForRemainOnPoint = 0.0f;
    private float durationForRemainOnPoint = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        playerCam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!PlayerInputActionMode.instance.enablePlayerClickAction)
        {
            teachingStageController.instance.clickBuildingNotice.SetActive(false);
            teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
            timerForRemainOnPoint = 0.0f;
            return;
        }
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, LayerMask.NameToLayer("ground")))
        {
            if (Mathf.Pow(this.gameObject.transform.position.x - hit.point.x, 2) + Mathf.Pow(this.gameObject.transform.position.z - hit.point.z, 2) < 350)
            {
                //if already interact with scene objects, close the notice text and show again 3 secs later if inactive
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    teachingStageController.instance.clickBuildingNotice.SetActive(false);
                    teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
                    timerForRemainOnPoint = -2.0f;
                }
                timerForRemainOnPoint += Time.deltaTime;
                if (timerForRemainOnPoint >= durationForRemainOnPoint)
                {
                    if (hit.collider.tag == "noticePoint" /*&& clickBuildingNoticeTimes < noticeTimesBound*/)
                    {
                        teachingStageController.instance.clickBuildingNotice.SetActive(true);
                    }
                    else if ((hit.collider.tag == "wood" || hit.collider.tag == "gravel" || hit.collider.tag == "iron" || hit.collider.tag == "water" || hit.collider.tag == "fire") /*&& clickMaterialFieldNoticeTimes < noticeTimesBound*/)
                    {
                        teachingStageController.instance.clickMaterialFieldNotice.SetActive(true);
                    }
                    else
                    {
                        teachingStageController.instance.clickBuildingNotice.SetActive(false);
                        teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
                        timerForRemainOnPoint = 0.0f;
                    }
                }
            }
            else
            {
                teachingStageController.instance.clickBuildingNotice.SetActive(false);
                teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
                timerForRemainOnPoint = 0.0f;
            }
        }
        else
        {
            teachingStageController.instance.clickBuildingNotice.SetActive(false);
            teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
            timerForRemainOnPoint = 0.0f;
        }
        timer += Time.deltaTime;
        if (timer >= durationn)
        {
            this.gameObject.GetComponent<PlayerTeachingStage>().enabled = false;
            teachingStageController.instance.clickBuildingNotice.SetActive(false);
            teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
        }
    }
}
