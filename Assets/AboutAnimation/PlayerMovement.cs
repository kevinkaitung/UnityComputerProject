
using UnityEngine;
using System.Collections;
using Photon.Pun;

	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class PlayerMovement : MonoBehaviourPun
	{

		public float animSpeed = 1.5f;				// アニメーション再生速度設定
		public float lookSmoother = 3.0f;			// a smoothing setting for camera motion
		public bool useCurves = true;				// Mecanimでカーブ調整を使うか設定する
		// このスイッチが入っていないとカーブは使われない
		public float useCurvesHeight = 0.5f;		// カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）

		// 以下キャラクターコントローラ用パラメタ
		// 前進速度
		public float forwardSpeed = 7.0f;
		// 後退速度
		public float backwardSpeed = 2.0f;
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
		private Animator anim;							// キャラにアタッチされるアニメーターへの参照
		private AnimatorStateInfo currentBaseState;			// base layerで使われる、アニメーターの現在の状態の参照

		private GameObject cameraObject;	// メインカメラへの参照
		
		// アニメーター各ステートへの参照
		static int idleState = Animator.StringToHash ("Base Layer.Idle");
		static int locoState = Animator.StringToHash ("Base Layer.Locomotion");
		static int jumpState = Animator.StringToHash ("Base Layer.Jump");
		//static int restState = Animator.StringToHash ("Base Layer.Rest");

		// 初期化
		void Start ()
		{
			// Animatorコンポーネントを取得する
			anim = GetComponent<Animator> ();
			// CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
			col = GetComponent<CapsuleCollider> ();
			rb = GetComponent<Rigidbody> ();
			//メインカメラを取得する
			//cameraObject = GameObject.FindWithTag ("MainCamera");
			cameraObject = transform.GetChild(5).gameObject;
			// CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
			orgColHight = col.height;
			orgVectColCenter = col.center;
		}
		void FixedUpdate ()
		{
				//if not me, just return
			if (!photonView.IsMine)
			{
				return;
			}

			float h = Input.GetAxis ("Horizontal");				
			float v = Input.GetAxis ("Vertical");				
			anim.SetFloat ("Speed", v);							
			anim.SetFloat ("Direction", h); 						
			anim.speed = animSpeed;								
			currentBaseState = anim.GetCurrentAnimatorStateInfo (0);	
			rb.useGravity = true;
		
		
		
			
			velocity = new Vector3 (0, 0, v);	
			velocity = transform.TransformDirection (velocity);
		
			if (v > 0.1) {
				velocity *= forwardSpeed;		
			} else if (v < -0.1) {
				velocity *= backwardSpeed;	
			}
		
			if (Input.GetButtonDown ("Jump")) {	
				if (currentBaseState.nameHash == locoState) {
					
					if (!anim.IsInTransition (0)) {
						rb.AddForce (Vector3.up * jumpPower, ForceMode.VelocityChange);
						anim.SetBool ("Jump", true);		
					}
				}
			}
			transform.localPosition += velocity * Time.fixedDeltaTime;

			
			transform.Rotate (0, h * rotateSpeed, 0);	
			if (currentBaseState.nameHash == locoState) {
				if (useCurves) {
					resetCollider ();
				}
			}
		// JUMP中の処理
		// 現在のベースレイヤーがjumpStateの時
		else if (currentBaseState.nameHash == jumpState) {
				cameraObject.SendMessage ("setCameraPositionJumpView");	// ジャンプ中のカメラに変更
				// ステートがトランジション中でない場合
				if (!anim.IsInTransition (0)) {
				
					// 以下、カーブ調整をする場合の処理
					if (useCurves) {
						float jumpHeight = anim.GetFloat ("JumpHeight");
						float gravityControl = anim.GetFloat ("GravityControl"); 
						if (gravityControl > 0)
							rb.useGravity = false;	//ジャンプ中の重力の影響を切る
										
						// レイキャストをキャラクターのセンターから落とす
						Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit ();
						// 高さが useCurvesHeight 以上ある時のみ、コライダーの高さと中心をJUMP00アニメーションについているカーブで調整する
						if (Physics.Raycast (ray, out hitInfo)) {
							if (hitInfo.distance > useCurvesHeight) {
								col.height = orgColHight - jumpHeight;			// 調整されたコライダーの高さ
								float adjCenterY = orgVectColCenter.y + jumpHeight;
								col.center = new Vector3 (0, adjCenterY, 0);	// 調整されたコライダーのセンター
							} else {
								// 閾値よりも低い時には初期値に戻す（念のため）					
								resetCollider ();
							}
						}
					}
					// Jump bool値をリセットする（ループしないようにする）				
					anim.SetBool ("Jump", false);
				}
			}
	
		else if (currentBaseState.nameHash == idleState) {
				
				if (useCurves) {
					resetCollider ();
				}
				if (Input.GetButtonDown ("Jump")) {
					rb.AddForce (Vector3.up * jumpPower, ForceMode.VelocityChange);
					anim.SetBool ("Jump", true);
				}
			}
		
		void resetCollider ()
		{
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}