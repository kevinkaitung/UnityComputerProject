using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerMovement : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public CharacterController controller;
    public float slower = 0.5f;

    public float speedup = 1.5f;

    public float speed = 15.0f;
    public float changedSpeedHeadBob = 5.0f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float grounDistance = 0.4f;

    public LayerMask groundMask;

    public float JumpHeight = 3f;

    public bool isGround;

    Vector3 velocity;
    private Animator anim;

    string myTeam;  //which team belong (every prefab belongs to different teams)
    public int change = 1;  //change speed if required
    private float timerForChangeSpeedDuration = 0.0f;   //timer for change speed countdown
    [SerializeField]
    private float durationTimeForChangeSpeed = 10.0f;   //how long will change speed effect
    private bool startTimer = false;    //enable/disable timer
    //black hole effect block player move action
    public bool isBlackholeEffectForMove = false;

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
                speed = 9.0f;
                changedSpeedHeadBob = 2.5f;
                startTimer = true;
                timerForChangeSpeedDuration = 0.0f;
                //change particle system's color (red for slow down)
                var main = GetComponent<ParticleSystem>().main;
                main.startColor = Color.red;
                GetComponent<ParticleSystem>().Play();
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
                Debug.Log("myteam:" + myTeam + "  " + raiseTeam);
                //speedup effect
                change = 2;
                speed = 20.0f;
                changedSpeedHeadBob = 7.5f;
                //restart the timer
                startTimer = true;
                timerForChangeSpeedDuration = 0.0f;
                //change particle system's color (blue for speed up)
                var main = GetComponent<ParticleSystem>().main;
                main.startColor = Color.white;
                GetComponent<ParticleSystem>().Play();
            }
        }
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            //set up team name of this player
            setUpTeamInfo();
        }
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void setUpTeamInfo()
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
        //call others to set up team name
        photonView.RPC("otherSetUpTeamInfo", RpcTarget.OthersBuffered, myTeam);
    }

    [PunRPC]
    void otherSetUpTeamInfo(string myteam)
    {
        myTeam = myteam;
    }

    public Vector3 move = new Vector3(0.0f, 0.0f, 0.0f);
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
                speed = 15.0f;
                changedSpeedHeadBob = 5.0f;
                startTimer = false;
                GetComponent<ParticleSystem>().Stop();
            }
        }
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }
        if (!PlayerInputActionMode.instance.enablePlayerMovement)
        {
            anim.SetFloat("Vertical", 0f);
            anim.SetFloat("Horizontal", 0f);
            return;
        }
        //if blackhole effect is active, block player move action
        if (isBlackholeEffectForMove)
        {
            return;
        }
        //normal speed

        Move();
    }

    private void Move()
    {
        //anim.SetBool("Jump",false);
        isGround = Physics.CheckSphere(groundCheck.position, grounDistance, groundMask);
        if (isGround && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        anim.SetFloat("Vertical", moveZ);
        anim.SetFloat("Horizontal", moveX);

        move = transform.right * moveX + transform.forward * moveZ;

        // SpeedChange();

        if (isGround)
        {
            anim.SetFloat("Velocity.y", velocity.y);

        }
        if (isGround && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        else if (!isGround)
        {
            anim.SetBool("Jump", false);
            anim.SetFloat("Velocity.y", velocity.y);
        }
        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void Jump()
    {
        anim.SetBool("Jump", true);
        velocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
    }

    /*
    private void SpeedChange()
    {
        if (change == 1)
        {
            //anim.SetBool("Slower", false);
            speed = 7.0f;
        }
        //slow down speed
        else if (change == 0)
        {
            //anim.SetBool("Slower", true);
            speed *= slower;

        }
        //speed up
        else if (change == 2)
        {
           // anim.SetBool("Slower", false);
            speed *= speedup;
        
        }
    }*/
}
