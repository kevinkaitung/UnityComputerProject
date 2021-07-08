using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class allUI : MonoBehaviour
{
    private static allUI s_Instance = null;
    public static allUI instance
    {   
        get
        {   
            if (s_Instance == null)
            {   
                s_Instance = FindObjectOfType(typeof(allUI)) as allUI;
                 
                if (s_Instance == null)
                    Debug.Log("Could not locate a allUI " +
                              "object. \n You have to have exactly " +
                              "one allUI in the scene.");
            }
            return s_Instance;
        }
    }
    public GameObject obj, Blueprint;
    public Button closeblueprint;
    public Text objname, noticepointinfo;
    // Start is called before the first frame update
    void Start()
    {
        Blueprint.SetActive(false);
        obj.SetActive(false);
    }
    public void showNoticePointInfo(noticePoint ntp)
    {
        int i = 0;
        while(true)
        {
            if(nodeManager.instance.dataRoot.gameDataNodes[i].position == ntp.pos)
            {
                objname.text = "The name of this notice point is " + ntp.objShap + ".\n";
                break;
            }
            else
            {
                i++;
            }
            if(i>100)
                break;
        }
    }
}
