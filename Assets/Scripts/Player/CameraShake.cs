using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private PlayerCamera playerCamComp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void cameraShake()
    {
        /**************
        * Camera Shake
        **************/
        float height = 125.0f;
        float shakeAmt = height * 0.2f; // the degrees to shake the camera
        float shakePeriodTime = 0.42f; // The period of each shake
        float dropOffTime = 1.6f; // How long it takes the shaking to settle down to nothing
        LTDescr shakeTween = LeanTween.rotateAroundLocal(gameObject, Vector3.right, shakeAmt, shakePeriodTime)
        .setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
        .setLoopClamp()
        .setRepeat(4)
        .setOnComplete(finishShake);

        // Slow the camera shake down to zero
        LeanTween.value(gameObject, shakeAmt, 0f, dropOffTime).setOnUpdate(
            (float val) =>
            {
                shakeTween.setTo(this.transform.right * val);
            }
        ).setEase(LeanTweenType.easeOutQuad);
    }

    void finishShake()
    {
        playerCamComp.shakeNow = false;
    }

    //force to stop shake
    public void stopShake()
    {
        LeanTween.cancel(gameObject);
    }
}
