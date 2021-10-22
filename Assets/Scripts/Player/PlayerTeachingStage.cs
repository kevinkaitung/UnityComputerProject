using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeachingStage : MonoBehaviour
{
    private Camera playerCam;
    private float timer = 0.0f;
    private float durationn = 40.0f;
    // Start is called before the first frame update
    void Start()
    {
        playerCam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, LayerMask.NameToLayer("ground")))
        {
            if (Mathf.Pow(this.gameObject.transform.position.x - hit.point.x, 2) + Mathf.Pow(this.gameObject.transform.position.z - hit.point.z, 2) < 350)
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
                }
            }
            else
            {
                teachingStageController.instance.clickBuildingNotice.SetActive(false);
                teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
            }
        }
        else
        {
            teachingStageController.instance.clickBuildingNotice.SetActive(false);
            teachingStageController.instance.clickMaterialFieldNotice.SetActive(false);
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
