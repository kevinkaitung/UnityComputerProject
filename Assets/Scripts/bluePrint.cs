using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bluePrint : MonoBehaviour
{
    private static bluePrint s_Instance = null;
    public static bluePrint instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(bluePrint)) as bluePrint;

                if (s_Instance == null)
                    Debug.Log("Could not locate a bluePrint " +
                              "object. \n You have to have exactly " +
                              "one bluePrint in the scene.");
            }
            return s_Instance;
        }
    }
    public GameObject noticePointInfoPanel, blueprintPanel;
    public Button closeBlueprintButton;
    public Text singleNoticePointInfoText, bluePrintText;
    // Start is called before the first frame update
    void Start()
    {
        noticePointInfoPanel.SetActive(false);
        blueprintPanel.SetActive(false);
    }
    public void showNoticePointInfo(noticePoint ntp)
    {
        singleNoticePointInfoText.text = "The name of this notice point is " + ntp.objShap + ".\n";
    }
}
