using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanelScript : MonoBehaviour
{
    public GameObject exit;
    public GameObject quesion;
    public GameObject setting;
    public GameObject optionPanel;

    bool clicktime;

    // Start is called before the first frame update
    void Start()
    {
        exit.SetActive(false);
        quesion.SetActive(false);
        setting.SetActive(false);
        clicktime = false;
    }

    public void doublecheck()
    {
        optionPanel.SetActive(false);
        exit.SetActive(true);
        clicktime = true;
    }

    public void Exitgame()
    {

    }

    public void BacktoPanel()
    {
        optionPanel.SetActive(true);
        if(clicktime == true)
        {
            exit.SetActive(false);
            setting.SetActive(false);
            quesion.SetActive(false);
        }
        
    }

    public void settingwindow()
    {
        optionPanel.SetActive(false);
        setting.SetActive(true);
        clicktime = true;
    }

    public void queswindow()
    {
        optionPanel.SetActive(false);
        quesion.SetActive(true);
        clicktime = true;
    }

}
