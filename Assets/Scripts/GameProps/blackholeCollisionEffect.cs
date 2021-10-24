using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class blackholeCollisionEffect : MonoBehaviour
{
    float timer = 0.0f;
    [SerializeField]
    float durationTime = 10.0f;
    bool startTimer = false;

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
                //control player click action
                this.gameObject.GetComponent<PlayerClickActionforTeam>().isBlackholeEffect = false;
                gamePropsManager.instance.disableBlackholeEffecttoPlayer();
                //解除限制移動和點擊動作(除了UI)...
                //PlayerClickActionforTeam.bpc = false;
            }
            //display countdown seconds for the player
            gamePropsManager.instance.blackholeEffectCountdown(durationTime - timer);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if the player collide to blackhole, enable blackhole effect
        if (hit.collider.gameObject.tag == "blackholeObstacle")
        {
            startTimer = true;
            //control player click action
            this.gameObject.GetComponent<PlayerClickActionforTeam>().isBlackholeEffect = true;
            timer = 0.0f;
            gamePropsManager.instance.enableBlackholeEffecttoPlayer();
            //restart at the spawn position
            this.gameObject.transform.position = gamePropsManager.instance.reSpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].position;
            //限制移動和點擊動作(除了UI)...
            //PlayerClickActionforTeam.bpc = true;
        }
    }

    //when the game is end, disable the blackhole effect
    private void OnDestroy()
    {
        //如果gamePropsManager比玩家早destroy,會讀不到gamePropsManager(?)
        gamePropsManager.instance.disableBlackholeEffecttoPlayer();
        //解除限制移動和點擊動作(除了UI)...
    }
}
