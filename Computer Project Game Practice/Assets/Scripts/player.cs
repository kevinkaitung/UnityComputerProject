using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public string holdMaterial;
    private Camera playerCam;
    // Start is called before the first frame update
    void Start()
    {
        //get player's camera
        playerCam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKey("up")) {  transform.Translate( 0, 0, 0.1f );  }
        // 按住 上鍵 時，物件每個 frame 朝自身 z 軸方向移動 0.1 公尺

        if ( Input.GetKey("down")) {  transform.Translate( 0, 0, -0.1f );  }
        // 按住 下鍵 時，物件每個 frame 朝自身 z 軸方向移動 -0.1 公尺

        if ( Input.GetKey("left")) {  transform.Rotate( 0, -3, 0 );  }
        // 按住 左鍵 時，物件每個 frame 以自身 y 軸為軸心旋轉 -3 度

        if ( Input.GetKey("right")) {  transform.Rotate( 0, 3, 0 );  }
        // 按住 右鍵 時，物件每個 frame 以自身 y 軸為軸心旋轉 3 度
        
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.transform.name);
                if(hit.collider.tag == "noticePoint")
                {
                    Debug.Log("click notice point");
                    //Get player clicked point's component to know clicked point's detail
                    noticePoint clickedPointInfo = hit.collider.gameObject.GetComponent<noticePoint>();
                    //Pass detail of clicked point and player's hold material to game controller
                    gameLogicController.instance.playerPutThingsOnPoint(clickedPointInfo, holdMaterial);
                }
            }
        }
    }
}
