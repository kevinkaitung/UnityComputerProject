using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class gamePropsDestroy : MonoBehaviour
{
    float timer = 0.0f;
    //life time of the game prop existing in the scene
    [SerializeField]
    float durationTime = 20.0f;
    bool flash = false;
    bool rotatectl = true;
    Color objcolor;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.init(3200);
        objcolor = gameObject.GetComponentInChildren<Renderer>().material.color;
        if (this.gameObject.tag != "removalToolMyself" && this.gameObject.tag != "removalToolOther")
            transform.Rotate(35f, 30, 45f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > durationTime)
        {
            Destroy(this.gameObject);
            flash = false;
        }
        if (!flash)
        {
            if (durationTime - timer < 5.0f)
            {
                hide();
                flash = true;
            }
        }
        if(rotatectl)
        {
            if (this.gameObject.tag != "removalToolMyself" && this.gameObject.tag != "removalToolOther")
                rotate();
            rotatectl = false;
            //transform.Rotate(-0.75f, -0.75f, 0.75f);
        }//transform.RotateAround(transform.position, Vector3.up, 1.0f);
    }

    //if the game prop was clicked, destroy it (only master client do it) 
    [PunRPC]
    public void destroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    void hide()
    {
        LeanTween.alpha(gameObject, 0.0f, 0.15f).setOnComplete(show);
    }
    void show()
    {
        LeanTween.alpha(gameObject, 1.0f, 0.15f).setOnComplete(hide);
    }
    void rotate()
    {
        LeanTween.rotateAround(gameObject, Vector3.up, 360f, 1f).setOnComplete(rotate);
    }
}
