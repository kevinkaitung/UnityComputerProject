using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyChangeSceneAnim : MonoBehaviour
{
    //LobbyChangeSceneAnim Singleton
    //Only create LobbyChangeSceneAnim once
    private static LobbyChangeSceneAnim s_Instance = null;
    public static LobbyChangeSceneAnim instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(LobbyChangeSceneAnim)) as LobbyChangeSceneAnim;

                if (s_Instance == null)
                    Debug.Log("Could not locate a LobbyChangeSceneAnim " +
                              "object. \n You have to have exactly " +
                              "one LobbyChangeSceneAnim in the scene.");
            }
            return s_Instance;
        }
    }

    public GameObject blackPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
