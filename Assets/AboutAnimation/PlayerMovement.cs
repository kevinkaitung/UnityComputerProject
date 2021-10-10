using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerMovement : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public CharacterController controller;

    public float speed = 7.0f;
    public  float gravity = -9.81f;

    public Transform groundCheck;
    public float grounDistance = 0.4f;

    public LayerMask groundMask;

    public float JumpHeight = 3f;

    bool isGround;

    Vector3 velocity;
    private Animator anim;

    string myTeam;  //which team belong
    public int change = 1;  //change speed if required
    private float timerForChangeSpeedDuration = 0.0f;   //timer for change speed countdown
    [SerializeField]
    private float durationTimeForChangeSpeed = 10.0f;   //how long will change speed effect
    private bool startTimer = false;    //enable/disable timer
    
//register for raise event
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //receive raise event
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //receive if some one in the other team uses slow down prop
        if (eventCode == gamePropsManager.instance.slowdownEffectEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string raiseTeam = (string)data[0];
            string effect = (string)data[1];
            if (myTeam != raiseTeam)
            {
                //slow down speed effect
                change = 0;
                startTimer = true;
                timerForChangeSpeedDuration = 0.0f;
            }
        }
        //receive if some one in the other team uses speedup prop
        else if (eventCode == gamePropsManager.instance.speedupEffectEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string raiseTeam = (string)data[0];
            string effect = (string)data[1];
            if (myTeam == raiseTeam)
            {
                //speedup effect
                change = 2;
                //restart the timer
                startTimer = true;
                timerForChangeSpeedDuration = 0.0f;
            }
        }
    }

    void Start()
    {
        //get team
        object tmp;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("_pt", out tmp);
        if ((byte)tmp == 1)
        {
            myTeam = "blue";
        }
        else
        {
            myTeam = "red";
        }
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        //countdown for the change speed effect
        if (startTimer)
        {
            timerForChangeSpeedDuration += Time.deltaTime;
            if (timerForChangeSpeedDuration > durationTimeForChangeSpeed)
            {
                timerForChangeSpeedDuration = 0.0f;
                change = 1;
                startTimer = false;
            }
        }
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }
        if(!PlayerInputActionMode.instance.enablePlayerMovement)
        {
            return;
        }
        Move();
    }
    private void Move()
    {
        //anim.SetBool("Jump",false);
        isGround = Physics.CheckSphere(groundCheck.position,grounDistance,groundMask);
        if(isGround && velocity.y<0)
        {
            velocity.y = -2f;
        }
        float moveX  = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        anim.SetFloat("Vertical", moveZ);
        anim.SetFloat("Horizontal", moveX);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        if(isGround)
        {
            if(Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("Jump",true);
                Jump();
            }
                //moveDirection *= movespeed;
         }
         else if(!isGround)
         {
             anim.SetBool("Jump",false);
         }
        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void Jump()
    {
        velocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
    }
}
