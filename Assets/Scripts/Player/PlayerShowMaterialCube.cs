using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerShowMaterialCube : MonoBehaviourPun
{
    private MeshRenderer holdMaterialMesh;
    private Image holdMaterialImg; 
    // Start is called before the first frame update
    void Awake()
    {
        //holdMaterialMesh = this.gameObject.GetComponent<MeshRenderer>();
        holdMaterialImg = this.gameObject.GetComponent<Image>();
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
            //holdMaterialMesh.material = Resources.Load("materialTexture/Materials/" + showMaterial) as Material;
            holdMaterialImg.sprite = Resources.Load<Sprite>("materialSprite/" + showMaterial);
        }
    }
}
