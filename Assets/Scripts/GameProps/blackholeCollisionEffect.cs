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
    //reserve for block the player's click action and camera?
    public static bool isBlackholeEffect = false;
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
                gamePropsManager.instance.disableBlackholeEffecttoPlayer();
                //解除限制移動和點擊動作(除了UI)...
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //if the player collide to blackhole, enable blackhole effect
        if (other.collider.gameObject.tag == "blackholeObstacle")
        {
            startTimer = true;
            timer = 0.0f;
            gamePropsManager.instance.enableBlackholeEffecttoPlayer();
            //restart at the spawn position
            this.gameObject.transform.position = gamePropsManager.instance.reSpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].position;
           //限制移動和點擊動作(除了UI)...
        }
    }

    //when the game is end, disable the blackhole effect
    private void OnDestroy() {
        gamePropsManager.instance.disableBlackholeEffecttoPlayer();
        //解除限制移動和點擊動作(除了UI)...
    }
}
