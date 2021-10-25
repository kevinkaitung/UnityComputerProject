using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputActionMode : MonoBehaviour
{
    //PlayerInputActionMode Singleton
    //Only create PlayerInputActionMode once
    private static PlayerInputActionMode s_Instance = null;
    public static PlayerInputActionMode instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(PlayerInputActionMode)) as PlayerInputActionMode;

                if (s_Instance == null)
                    Debug.Log("Could not locate a PlayerInputActionMode " +
                              "object. \n You have to have exactly " +
                              "one PlayerInputActionMode in the scene.");
            }
            return s_Instance;
        }
    }
    public int state;
    public bool enableCameraControl;
    public bool enablePlayerMovement;
    public bool enablePlayerClickAction;
    public Texture2D cursorCrossTexture;
    public Texture2D cursorPointTexture;
    [SerializeField]
    private GameObject fixedCenterCursor;
    bool exit = false, godcam = false;
    // Start is called before the first frame update
    void Start()
    {
        stateThree();
        //calculate for screen center
        fixedCenterCursor.GetComponent<RectTransform>().position = new Vector3Int(Screen.width / 2, Screen.height / 2, 0);
        fixedCenterCursor.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && state == 1)
        {
            stateTwo();
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt) && state == 2)
        {
            stateOne();
        }
        if(teamGameLogicController.instance.exitPanel.active == true)
        {
            stateTwo();
            exit = true;
        }
        if(teamGameLogicController.instance.exitPanel.active == false && exit)
        {
            stateOne();
            exit = false;
        }
        if(teamGameLogicController.instance.godcamera.active == true && !godcam)
        {
            stateTwo();
            godcam = true;
        }
        if(teamGameLogicController.instance.godcamera.active == false && godcam)
        {
            stateOne();
            godcam = false;
        }
    }

    public void stateOne()
    {
        float xspot = cursorCrossTexture.width / 2;
        float yspot = cursorCrossTexture.height / 2;
        Vector2 hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(cursorCrossTexture, hotSpot, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;
        fixedCenterCursor.SetActive(true);
        enableCameraControl = true;
        enablePlayerMovement = true;
        enablePlayerClickAction = true;
        state = 1;
    }

    public void stateTwo()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fixedCenterCursor.SetActive(false);
        Vector2 hotSpot = Vector2.zero;
        Cursor.SetCursor(cursorPointTexture, hotSpot, CursorMode.Auto);
        enableCameraControl = false;
        enablePlayerMovement = false;
        enablePlayerClickAction = false;
        state = 2;
    }

    public void stateThree()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fixedCenterCursor.SetActive(false);
        Vector2 hotSpot = Vector2.zero;
        Cursor.SetCursor(cursorPointTexture, hotSpot, CursorMode.Auto);
        enableCameraControl = false;
        enablePlayerMovement = false;
        enablePlayerClickAction = false;
        state = 3;
    }

    //game finish state
    public void stateFour()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fixedCenterCursor.SetActive(false);
        Vector2 hotSpot = Vector2.zero;
        Cursor.SetCursor(cursorPointTexture, hotSpot, CursorMode.Auto);
        enableCameraControl = false;
        enablePlayerMovement = false;
        enablePlayerClickAction = false;
        state = 4;
    }

    //cursor not use in game: color black
    public void fixedCenterCursorRest()
    {
        fixedCenterCursor.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
    }

    //cursor clickable in game: color green
    public void fixedCenterCursorDetected()
    {
        fixedCenterCursor.GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    }
}
