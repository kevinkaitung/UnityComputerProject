using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
public class OptionPanelScript : MonoBehaviour
{
    public GameObject exit;
    public GameObject quesion;
    public GameObject setting;
    public GameObject optionPanel;
    public GameObject TeachPanel;
    public GameObject shutDownGamePanel;

    bool clicktime;

    // Start is called before the first frame update
    void Start()
    {
        exit.SetActive(false);
        quesion.SetActive(false);
        setting.SetActive(false);
        shutDownGamePanel.SetActive(false);
        clicktime = false;
    }

    public void doublecheck()
    {
        optionPanel.SetActive(false);
        exit.SetActive(true);
        clicktime = true;
    }
    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("launch");
        PhotonNetwork.JoinLobby();
    }
    public void Exitgame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
    }

    public void BacktoPanel()
    {
        optionPanel.SetActive(true);
        if(clicktime == true)
        {
            exit.SetActive(false);
            setting.SetActive(false);
            quesion.SetActive(false);
            shutDownGamePanel.SetActive(false);
            clicktime = false;
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
        //optionPanel.SetActive(false);
        TeachPanel.SetActive(true);
        clicktime = true;
    }

    public void shutDownGameWindow()
    {
        //optionPanel.SetActive(false);
        shutDownGamePanel.SetActive(true);
        clicktime = true;
    }

    public void shutDownGame()
    {
        Application.Quit();
    }

}
