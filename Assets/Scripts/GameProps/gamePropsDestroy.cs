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
    bool tmp = true;
    Color objcolor;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.init(3200);
        objcolor = gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > durationTime)
        {
            Destroy(this.gameObject);
            tmp = true;
        }
        if (tmp)
        {
            if (durationTime - timer < 5.0f)
            {
                hide();
                tmp = false;
            }
        }
    }

    //if the game prop was clicked, destroy it (only master client do it) 
    [PunRPC]
    public void destroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    void hide()
    {
        LeanTween.color(gameObject, new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.25f).setOnComplete(show);
    }
    void show()
    {
        LeanTween.color(gameObject, objcolor, 0.25f).setOnComplete(hide);
    }
}
