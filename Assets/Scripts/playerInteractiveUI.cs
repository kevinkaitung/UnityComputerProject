using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerInteractiveUI : MonoBehaviour
{
    private static playerInteractiveUI s_Instance = null;
    public static playerInteractiveUI instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(playerInteractiveUI)) as playerInteractiveUI;

                if (s_Instance == null)
                    Debug.Log("Could not locate a playerInteractiveUI " +
                              "object. \n You have to have exactly " +
                              "one playerInteractiveUI in the scene.");
            }
            return s_Instance;
        }
    }
    public GameObject noticePointInfoPanel, synthesisformulaPanel, materialFieldInfoPanel;
    public GameObject materialFieldInfoText, materialFieldInfoImage;
    public Text singleNoticePointInfoText, bluePrintText;
    // Start is called before the first frame update
    void Start()
    {
        noticePointInfoPanel.SetActive(false);
        synthesisformulaPanel.SetActive(false);
        materialFieldInfoPanel.SetActive(false);
    }
    public void showNoticePointInfo(noticePoint ntp)
    {
        singleNoticePointInfoText.text = "The name of this notice point is " + ntp.objShap + ".\n";
    }
}
