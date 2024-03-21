using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gamePropTypeText : MonoBehaviour
{
    //control Game Prop Effect Text tweening
    [SerializeField]
    private RectTransform gamePropEffectTextRectTransform;
    //Game Prop Effect Text and Image
    [SerializeField]
    private Text teamText;
    [SerializeField]
    private Text gamePropText;
    [SerializeField]
    private Image gamePropImage;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void activateTextEffect(string receiveAttackTeam, string receiveAttackEffect)
    {
        //change text color based on attacked team
        if (receiveAttackTeam == "red")
        {
            teamText.text = "Red Team,";
            teamText.color = Color.red;
            gamePropText.color = Color.red;
        }
        else
        {
            teamText.text = "Blue Team,";
            teamText.color = Color.blue;
            gamePropText.color = Color.blue;
        }
        gamePropText.text = receiveAttackEffect + " !";
        gamePropImage.sprite = Resources.Load<Sprite>("GamePropImg/" + receiveAttackEffect);
        //game prop effect tweening
        gamePropEffectTextRectTransform.LeanSetLocalPosX(250.0f);
        gamePropEffectTextRectTransform.LeanSetLocalPosY(-90.0f);
        gamePropEffectTextRectTransform.LeanMoveLocalX(190.0f, 0.5f).setEaseOutBack();

        gamePropEffectTextRectTransform.LeanMoveLocalY(-180.0f, 1.0f).setEaseOutExpo().setDelay(1.2f);
        //when completing the effect, destroy the prefab
        gamePropEffectTextRectTransform.LeanAlphaText(0.0f, 1.0f).setEaseOutExpo().setDelay(1.2f).setOnComplete(selfDestroy);
    }

    //destroy the prefab
    private void selfDestroy()
    {
        Destroy(this.gameObject);
    }
}
