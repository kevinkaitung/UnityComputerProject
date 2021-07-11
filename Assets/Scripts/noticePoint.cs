using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class noticePoint : MonoBehaviourPun
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 sca;
    public string objShap;
    public string materialNam;
    public int stag;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //if this notice point be clicked, this point's part build done. Destroy this notice point
    //only master client do PhotonNetwork.Destroy
    [PunRPC]
    public void destroyNoticePoint()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}
