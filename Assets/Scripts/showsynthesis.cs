using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showsynthesis : MonoBehaviour
{
    private MeshRenderer synthesisMaterialMesh;
    private static showsynthesis s_Instance = null;
    public static showsynthesis instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(showsynthesis)) as showsynthesis;

                if (s_Instance == null)
                    Debug.Log("Could not locate a showsynthesis " +
                              "object. \n You have to have exactly " +
                              "one showsynthesis in the scene.");
            }
            return s_Instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        synthesisMaterialMesh = this.gameObject.GetComponent<MeshRenderer>();
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void showSynthesisMaterialCube(string firstInputItem)
    {
        //if hold material is empty
        if (firstInputItem == "empty")
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("fkjshldf");
            //show texture to hold material
            this.gameObject.SetActive(true);
            synthesisMaterialMesh.material = Resources.Load("materialTexture/Materials/" + Synthesis.instance.firstInputItem) as Material;
        }
    }
}
