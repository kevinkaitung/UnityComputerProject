using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        stateOne();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) && state == 1)
        {
            stateTwo();
        }
        else if(Input.GetKeyDown(KeyCode.LeftAlt) && state == 2)
        {
            stateOne();
        }
    }

    public void stateOne()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        enableCameraControl = true;
        enablePlayerMovement = true;
        enablePlayerClickAction = true;
        state = 1;
    }

    public void stateTwo()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        enableCameraControl = false;
        enablePlayerMovement = true;
        enablePlayerClickAction = true;
        state = 2;
    }

    public void stateThree()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        enableCameraControl = false;
        enablePlayerMovement = false;
        enablePlayerClickAction = false;
        state = 3;
    }
}
