using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeManager : MonoBehaviour
{
    //NodeManager Singleton
    //Only create NodeManager once
    private static nodeManager s_Instance = null;
    public static nodeManager instance
    {   
        get
        {   
            if (s_Instance == null)
            {   
                s_Instance = FindObjectOfType(typeof(nodeManager)) as nodeManager;
                 
                if (s_Instance == null)
                    Debug.Log("Could not locate a NodeManager " +
                              "object. \n You have to have exactly " +
                              "one NodeManager in the scene.");
            }
            return s_Instance;
        }
    }
    public TextAsset jsonFile;
    //serializable?
    public nodeRoot dataRoot;
    void Awake() 
    {
        createGameData();
    }

    void createGameData()
    {
        Debug.Log(jsonFile.text);
        dataRoot = JsonUtility.FromJson<nodeRoot>(jsonFile.text);
        for(int i = 0; i < 8; i++)
        {
            Debug.Log(dataRoot.gameDataNodes[i].position + " " + dataRoot.gameDataNodes[i].objShape);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
