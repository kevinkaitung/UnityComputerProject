using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class TeachingScript : MonoBehaviour
{
    [SerializeField]
    public GameObject TeachPanel;
    [SerializeField]
    public GameObject Exitbutton;
    [SerializeField]
    public GameObject NextPagebutton;
    [SerializeField]
    public GameObject FrontPagebutton;
    [SerializeField]
    public GameObject Billboard_one;
    [SerializeField]
    public GameObject Billboard_two;
    [SerializeField]
    public GameObject Billboard_three;
    [SerializeField]
    public GameObject Billboard_four;
    [SerializeField]
    public GameObject Billboard_five;
    [SerializeField]
    public GameObject Billboard_six;

    private bool firstTime = true;

    public int Clicktime = 0;  // 數字可更改

    // Start is called before the first frame update
    void Start()
    {
        TeachPanel.SetActive(true);
        Billboard_two.SetActive(false);
        Billboard_three.SetActive(false);

        if (Clicktime == 0)
            FrontPagebutton.SetActive(false);
        else
            FrontPagebutton.SetActive(true);

        if (Clicktime == 5)
            NextPagebutton.SetActive(false);
        else
            NextPagebutton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Clicktime == 0)
            FrontPagebutton.SetActive(false);
        else
            FrontPagebutton.SetActive(true);

        if (Clicktime == 5)
            NextPagebutton.SetActive(false);
        else
            NextPagebutton.SetActive(true);
    }

    public void GoNextPage()
    {
        Clicktime += 1;
        switch (Clicktime)
        {
            case 0:
                Billboard_one.SetActive(true);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 1:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(true);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 2:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(true);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 3:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(true);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 4:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(true);
                Billboard_six.SetActive(false);
                break;
            case 5:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(true);
                break;
        }

    }
    public void GoFrontPage()
    {
        Clicktime -= 1;
        switch (Clicktime)
        {
            case 0:
                Billboard_one.SetActive(true);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 1:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(true);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 2:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(true);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 3:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(true);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(false);
                break;
            case 4:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(true);
                Billboard_six.SetActive(false);
                break;
            case 5:
                Billboard_one.SetActive(false);
                Billboard_two.SetActive(false);
                Billboard_three.SetActive(false);
                Billboard_four.SetActive(false);
                Billboard_five.SetActive(false);
                Billboard_six.SetActive(true);
                break;
        }
    }

    public void ClosePanel()
    {
        Billboard_one.SetActive(true);
        Billboard_two.SetActive(false);
        Billboard_three.SetActive(false);
        Billboard_four.SetActive(false);
        Billboard_five.SetActive(false);
        Billboard_six.SetActive(false);
        TeachPanel.SetActive(false);
        Clicktime = 0;
        if (firstTime)
        {
            PlayerInputActionMode.instance.stateOne();
            firstTime = false;
        }
    }
}
