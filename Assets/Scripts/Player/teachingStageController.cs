using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teachingStageController : MonoBehaviour
{
    //teachingStageController Singleton
    //Only create NodeManager once
    private static teachingStageController s_Instance = null;
    public static teachingStageController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(teachingStageController)) as teachingStageController;

                if (s_Instance == null)
                    Debug.Log("Could not locate a teachingStageController " +
                              "object. \n You have to have exactly " +
                              "one teachingStageController in the scene.");
            }
            return s_Instance;
        }
    }
    [SerializeField]
    public GameObject clickBuildingNotice, clickMaterialFieldNotice;
    private int clickBuildingNoticeTimes = 0, clickMaterialFieldNoticeTimes = 0;
    private int noticeTimesBound = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
