using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShowMaterialCube : MonoBehaviourPun
{
    private MeshRenderer holdMaterialMesh;
    // Start is called before the first frame update
    void Start()
    {
        holdMaterialMesh = this.gameObject.GetComponent<MeshRenderer>();
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //change texture by network
    [PunRPC]
    public void showPlayerHoldMaterialCube(string showMaterial)
    {
        //if hold material is empty
        if (showMaterial == "empty")
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            //show texture to hold material
            this.gameObject.SetActive(true);
            holdMaterialMesh.material = Resources.Load("materialTexture/Materials/" + showMaterial) as Material;
        }
    }
}
