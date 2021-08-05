using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript: MonoBehaviour
{
    public GameObject blueprintPanel;
    public GameObject optionPanel;

    bool clicktimeBP;
    bool clicktimeOP;
    // Start is called before the first frame update
    void Start()
    {        
        blueprintPanel.SetActive(false);
        optionPanel.SetActive(false);
        LeanTween.scale(blueprintPanel, new Vector3(0, 0, 0), 0.1f);
        LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 415f, 0.1f);
        LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), -148f, 0.1f);
        clicktimeBP = false;
        clicktimeOP = false;
    }

    public void callblueprintPanel()
    {
        if(clicktimeBP == false)
        {
            blueprintPanel.SetActive(true);
            LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 000f, 0.5f);
            LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), 000f, 0.5f);
            LeanTween.scale(blueprintPanel, new Vector3(1, 1, 1), 0.5f);
            clicktimeBP = true;
        }
        else if (clicktimeBP == true)
        {
            LeanTween.scale(blueprintPanel, new Vector3(0, 0, 0), 0.5f);
            LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 415f, 0.5f);
            LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), -148f, 0.5f);
            clicktimeBP = false;              
        }

    }

    public void calloptionPanel()
    {
        if (clicktimeOP == false)
        {
            optionPanel.SetActive(true);
            clicktimeOP = true;
        }
        else if (clicktimeOP == true)
        {
            optionPanel.SetActive(false);
            clicktimeOP = false;
        }
    }

}
