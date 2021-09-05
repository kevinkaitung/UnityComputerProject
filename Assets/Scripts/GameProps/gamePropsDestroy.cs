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
    Color objcolor;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.init(3200);
        objcolor = gameObject.GetComponent<Renderer>().material.color;
        transform.Rotate(45, 0, 45);
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
        transform.Rotate(-0.75f, -0.75f, 0.75f);
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
}
