using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonScript : MonoBehaviour
{
    public GameObject blueprintPanel;
    public GameObject optionPanel;
    public GameObject chatroomPanel;

    bool clicktimeBP;
    bool clicktimeOP;
    bool clicktimeCP;
    // Start is called before the first frame update
    void Start()
    {
        blueprintPanel.SetActive(false);
        optionPanel.SetActive(false);
        chatroomPanel.SetActive(false);
        /*LeanTween.scale(blueprintPanel, new Vector3(0, 0, 0), 0.1f);
        LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 415f, 0.1f);
        LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), -148f, 0.1f);*/
        clicktimeBP = false;
        clicktimeOP = false;
        clicktimeCP = false;
    }

    void Update()
    {
        if (PlayerInputActionMode.instance.state == 1 || PlayerInputActionMode.instance.state == 2)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                PlayerInputActionMode.instance.stateThree();
                blueprintPanel.SetActive(true);
                clicktimeBP = true;
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                PlayerInputActionMode.instance.stateThree();
                optionPanel.SetActive(true);
                clicktimeOP = true;
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                PlayerInputActionMode.instance.stateThree();
                chatroomPanel.SetActive(true);
                clicktimeCP = true;
            }
        }
        //state 3
        else if (PlayerInputActionMode.instance.state == 3)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayerInputActionMode.instance.stateOne();
                blueprintPanel.SetActive(false);
                optionPanel.SetActive(false);
                chatroomPanel.SetActive(false);
                clicktimeBP = false;
                clicktimeOP = false;
                clicktimeCP = false;
            }
        }
    }


    public void callBlueprintPanel()
    {
        if (clicktimeBP == false)
        {
            blueprintPanel.SetActive(true);
            /*LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 000f, 0.5f);
            LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), 000f, 0.5f);
            LeanTween.scale(blueprintPanel, new Vector3(1, 1, 1), 0.5f);*/
            clicktimeBP = true;
        }
        else if (clicktimeBP == true)
        {
            blueprintPanel.SetActive(false);
            /*LeanTween.scale(blueprintPanel, new Vector3(0, 0, 0), 0.5f);
            LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 415f, 0.5f);
            LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), -148f, 0.5f);*/
            clicktimeBP = false;
        }
    }

    public void callOptionPanel()
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

    public void callChatRoomPanel()
    {
        if (clicktimeCP == false)
        {
            chatroomPanel.SetActive(true);
            clicktimeCP = true;
        }
        else if (clicktimeCP == true)
        {
            chatroomPanel.SetActive(false);
            clicktimeCP = false;
        }
    }

    public void callUIChangeState()
    {
        if (clicktimeBP == true || clicktimeOP == true || clicktimeCP == true)
        {
            PlayerInputActionMode.instance.stateThree();
            Debug.Log("state 3");
        }
        else
        {
            PlayerInputActionMode.instance.stateOne();
            Debug.Log("state 1");
        }
    }
}
