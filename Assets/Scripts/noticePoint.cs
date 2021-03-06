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

    //if player build the part of building, change its texture to the player's handy Material
    //only master client do
    [PunRPC]
    public void buildToChangeTexture(string handyMaterial)
    {
        //set the texture to handyMaterial
        Renderer rend = GetComponent<Renderer>();
        rend.material = Resources.Load("standardMat") as Material;
        rend.material.mainTexture = Resources.Load("texture of building/" + handyMaterial) as Texture;
        this.gameObject.tag = "clickedNoticePoint";
    }
}
