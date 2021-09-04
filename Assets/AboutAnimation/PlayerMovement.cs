
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement : MonoBehaviourPunCallbacks, IOnEventCallback
{
    string myTeam;  //which team belong
    public int change = 1;  //change speed if required
    private float timerForChangeSpeedDuration = 0.0f;   //timer for change speed countdown
    [SerializeField]
    private float durationTimeForChangeSpeed = 10.0f;   //how long will change speed effect
    private bool startTimer = false;    //enable/disable timer
    public float slower = 0.3f;
    public float speedup = 2.0f;
    public float animSpeed = 1.5f;              // アニメーション再生速度設定
    public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
    public bool useCurves = true;               // Mecanimでカーブ調整を使うか設定する
                                                // このスイッチが入っていないとカーブは使われない
    public float useCurvesHeight = 0.5f;        // カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）

    // 以下キャラクターコントローラ用パラメタ
    // 前進速度
    public float forwardSpeed = 12.0f;
    // 後退速度
    public float backwardSpeed = 5.0f;
    // 旋回速度
    public float rotateSpeed = 2.0f;
    // ジャンプ威力
    public float jumpPower = 3.0f;
    // キャラクターコントローラ（カプセルコライダ）の参照
    private CapsuleCollider col;
    private Rigidbody rb;
    // キャラクターコントローラ（カプセルコライダ）の移動量
    private Vector3 velocity;
    // CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
    private float orgColHight;
    private Vector3 orgVectColCenter;
    private Animator anim;                          // キャラにアタッチされるアニメーターへの参照
    private AnimatorStateInfo currentBaseState;         // base layerで使われる、アニメーターの現在の状態の参照

    private GameObject cameraObject;    // メインカメラへの参照

    // アニメーター各ステートへの参照
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    //static int restState = Animator.StringToHash ("Base Layer.Rest");

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

    // 初期化
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
        // Animatorコンポーネントを取得する
        anim = GetComponent<Animator>();
        // CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        //メインカメラを取得する
        //cameraObject = GameObject.FindWithTag ("MainCamera");
        //cameraObject = transform.GetChild(5).gameObject;
        // CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
        orgColHight = col.height;
        orgVectColCenter = col.center;
    }
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
    }

    void FixedUpdate()
    {
        //if not me, just return
        if (!photonView.IsMine)
        {
            return;
        }
        if(!PlayerInputActionMode.instance.enablePlayerMovement)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        anim.SetFloat("Speed", v);
        anim.SetFloat("Direction", h);
        anim.speed = animSpeed;
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        rb.useGravity = true;




        velocity = new Vector3(0, 0, v);
        velocity = transform.TransformDirection(velocity);

        //normal speed
        if (change == 1)
        {
            anim.SetBool("Slower", false);
            if (v > 0.1)
            {
                velocity *= forwardSpeed;
            }
            else if (v < -0.1)
            {
                velocity *= backwardSpeed;
            }
        }
        //slow down speed
        else if (change == 0)
        {
            anim.SetBool("Slower", true);
            if (v > 0.1)
            {
                velocity *= forwardSpeed;
                velocity *= slower;
            }
            else if (v < -0.1)
            {
                velocity *= backwardSpeed;
                velocity *= slower;
            }
        }
        //speed up
        else if (change == 2)
        {
            anim.SetBool("Slower", false);
            if (v > 0.1)
            {
                velocity *= forwardSpeed;
                velocity *= speedup;
            }
            else if (v < -0.1)
            {
                velocity *= backwardSpeed;
                velocity *= speedup;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (currentBaseState.nameHash == locoState)
            {
                if (!anim.IsInTransition(0))
                {
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    anim.SetBool("Jump", true);
                }
            }
        }
        transform.localPosition += velocity * Time.fixedDeltaTime;


        transform.Rotate(0, h * rotateSpeed, 0);
        if (currentBaseState.nameHash == locoState)
        {
            if (useCurves)
            {
                resetCollider();
            }
        }
        // JUMP中の処理
        // 現在のベースレイヤーがjumpStateの時
        else if (currentBaseState.nameHash == jumpState)
        {
            //cameraObject.SendMessage ("setCameraPositionJumpView");	// ジャンプ中のカメラに変更
            // ステートがトランジション中でない場合
            if (!anim.IsInTransition(0))
            {

                // 以下、カーブ調整をする場合の処理
                if (useCurves)
                {
                    float jumpHeight = anim.GetFloat("JumpHeight");
                    float gravityControl = anim.GetFloat("GravityControl");
                    if (gravityControl > 0)
                        rb.useGravity = false;  //ジャンプ中の重力の影響を切る

                    // レイキャストをキャラクターのセンターから落とす
                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo = new RaycastHit();
                    // 高さが useCurvesHeight 以上ある時のみ、コライダーの高さと中心をJUMP00アニメーションについているカーブで調整する
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {
                            col.height = orgColHight - jumpHeight;          // 調整されたコライダーの高さ
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3(0, adjCenterY, 0); // 調整されたコライダーのセンター
                        }
                        else
                        {
                            // 閾値よりも低い時には初期値に戻す（念のため）					
                            resetCollider();
                        }
                    }
                }
                // Jump bool値をリセットする（ループしないようにする）				
                anim.SetBool("Jump", false);
            }
        }

        else if (currentBaseState.nameHash == idleState)
        {

            if (useCurves)
            {
                resetCollider();
            }
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                anim.SetBool("Jump", true);
            }
        }

        void resetCollider()
        {
            col.height = orgColHight;
            col.center = orgVectColCenter;
        }
    }
}