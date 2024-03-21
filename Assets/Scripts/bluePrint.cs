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

    public int myTeamStageNumber = 1;

    public int freecount = 1;

    [SerializeField]
    private Image Blueprint;

    [SerializeField]
    private Sprite one_front;
    [SerializeField]
    private Sprite one_left;
    [SerializeField]
    private Sprite one_right;
    [SerializeField]
    private Sprite one_back;
    [SerializeField]
    private Sprite three_front;
    [SerializeField]
    private Sprite three_left;
    [SerializeField]
    private Sprite three_right;
    [SerializeField]
    private Sprite three_back;

    [SerializeField]
    private Sprite five_front;
    [SerializeField]
    private Sprite five_left;
    [SerializeField]
    private Sprite five_right;
    [SerializeField]
    private Sprite five_back;
    [SerializeField]
    private Sprite six;
    [SerializeField]
    private Sprite six_plus;
    [SerializeField]
    private GameObject next_button;
    [SerializeField]
    private GameObject previous_button;


    // Start is called before the first frame update
    void Start()
    {
        Blueprint.sprite = one_front;
    }

    // Update is called once per frame
    void Update()
    {
        if(freecount == 1)
            previous_button.SetActive(false);
        else
            previous_button.SetActive(true);
        
        if(freecount == 6)
            next_button.SetActive(false);
        else
            next_button.SetActive(true);
    }

    public void autoSettoCurrentStageBluePrint(int stageInput)
    {
        myTeamStageNumber = stageInput;
        freecount = myTeamStageNumber;
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_front;
                break;
            case 2:
                Blueprint.sprite = one_front;
                break;
            case 3:
                Blueprint.sprite = three_front;
                break;
            case 4:
                Blueprint.sprite = three_front;
                break;
            case 5:
                Blueprint.sprite = five_front;
                break;
            case 6:
                Blueprint.sprite = six;
                break;
        }
    }


    public void NextStage()
    {
        freecount += 1;
        if (freecount > 6)
            freecount = 6;
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_front;
                break;
            case 2:
                Blueprint.sprite = one_front;
                break;
            case 3:
                Blueprint.sprite = three_front;
                break;
            case 4:
                Blueprint.sprite = three_front;
                break;
            case 5:
                Blueprint.sprite = five_front;
                break;
            case 6:
                Blueprint.sprite = six;
                break;
        }
        Debug.Log("freecount " + freecount);
    }

    public void previousStage()
    {
        freecount -= 1;
        if (freecount < 1)
            freecount = 1;
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_front;
                break;
            case 2:
                Blueprint.sprite = one_front;
                break;
            case 3:
                Blueprint.sprite = three_front;
                break;
            case 4:
                Blueprint.sprite = three_front;
                break;
            case 5:
                Blueprint.sprite = five_front;
                break;
            case 6:
                Blueprint.sprite = six;
                break;
        }
        Debug.Log("freecount " + freecount);
    }

    public void ChangeSightNorth()//目前有bug 下面數字不會動
    {
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_front;
                break;
            case 2:
                Blueprint.sprite = one_front;
                break;
            case 3:
                Blueprint.sprite = three_front;
                break;
            case 4:
                Blueprint.sprite = three_front;
                break;
            case 5:
                Blueprint.sprite = five_front;
                break;
        }
    }

    public void ChangeSightEast()
    {
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_left;
                break;
            case 2:
                Blueprint.sprite = one_left;
                break;
            case 3:
                Blueprint.sprite = three_left;
                break;
            case 4:
                Blueprint.sprite = three_left;
                break;
            case 5:
                Blueprint.sprite = five_left;
                break;
        }
    }

    public void ChangeSightWest()
    {
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_right;
                break;
            case 2:
                Blueprint.sprite = one_right;
                break;
            case 3:
                Blueprint.sprite = three_right;
                break;
            case 4:
                Blueprint.sprite = three_right;
                break;
            case 5:
                Blueprint.sprite = five_right;
                break;
            case 6:
                Blueprint.sprite = six_plus;
                break;
        }
    }

    public void ChangeSightSouth()
    {
        switch (freecount)
        {
            case 1:
                Blueprint.sprite = one_back;
                break;
            case 2:
                Blueprint.sprite = one_back;
                break;
            case 3:
                Blueprint.sprite = three_back;
                break;
            case 4:
                Blueprint.sprite = three_back;
                break;
            case 5:
                Blueprint.sprite = three_back;
                break;
        }
    }
}
