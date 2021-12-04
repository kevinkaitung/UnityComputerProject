using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class blackholeCollisionEffect : MonoBehaviourPunCallbacks
{
    float timer = 0.0f;
    [SerializeField]
    float durationTime = 10.0f;
    bool startTimer = false;
    Vector3 afterBlackholePos;
    bool isSmokeEffect = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
            if (timer > durationTime)
            {
                startTimer = false;
                //control playe action
                this.gameObject.GetComponent<PlayerClickActionforTeam>().isBlackholeEffectForClick = false;
                this.gameObject.GetComponent<PlayerCamera>().isBlackholeEffectForCam = false;
                this.gameObject.GetComponent<PlayerMovement>().isBlackholeEffectForMove = false;
                gamePropsManager.instance.disableBlackholeEffecttoPlayer();
                //解除限制移動和點擊動作(除了UI)...
            }
            //display countdown seconds for the player
            gamePropsManager.instance.blackholeEffectCountdown(durationTime - timer);
            this.gameObject.transform.position = afterBlackholePos;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if the player collide to blackhole, enable blackhole effect
        if (hit.collider.gameObject.tag == "blackholeObstacle" && startTimer == false)
        {
            startTimer = true;
            //control player action
            this.gameObject.GetComponent<PlayerClickActionforTeam>().isBlackholeEffectForClick = true;
            this.gameObject.GetComponent<PlayerCamera>().isBlackholeEffectForCam = true;
            this.gameObject.GetComponent<PlayerMovement>().isBlackholeEffectForMove = true;
            timer = 0.0f;
            gamePropsManager.instance.enableBlackholeEffecttoPlayer();
            //restart at the spawn position
            afterBlackholePos = gamePropsManager.instance.reSpawnPoints[gamePropsManager.instance.myRespawnPointIndex].position;
            //限制移動和點擊動作(除了UI)...
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine)
        {
            return;
        }
        //if player collide the smoke obstacle
        if (other.gameObject.tag == "smoke" && !isSmokeEffect)
        {
            isSmokeEffect = true;
            gamePropsManager.instance.smokeEffectPanel.SetActive(true);
            //alpha change not succeed, and not change the picture's alpha one by one
            StartCoroutine(smokeEffectDelay());
        }
    }

    IEnumerator smokeEffectDelay()
    {
        yield return new WaitForSeconds(3.1f);
        gamePropsManager.instance.smokeEffectPanel.SetActive(false);
        isSmokeEffect = false;
    }

    //when the game is end, disable the blackhole effect
    private void OnDestroy()
    {
        //如果gamePropsManager比玩家早destroy,會讀不到gamePropsManager(?)
        //gamePropsManager.instance.disableBlackholeEffecttoPlayer();
        //解除限制移動和點擊動作(除了UI)...
    }
}
