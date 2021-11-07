using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneObjectInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject sceneObjectDetailPanel;
    [SerializeField]
    private Image sceneObjectImage;
    [SerializeField]
    private Text sceneObjectText;
    public string materialName;
    private float widthOffset, heightOffset;
    
    //set up image and text informations
    public void setup(string materialNametoPass)
    {
        materialName = materialNametoPass;
        if (materialName == "blueBuilding")
        {
            sceneObjectImage.sprite = Resources.Load<Sprite>("materialSprite/buildingSite");
            sceneObjectText.text = "籃隊建地";
        }
        else if (materialName == "redBuilding")
        {
            sceneObjectImage.sprite = Resources.Load<Sprite>("materialSprite/buildingSite");
            sceneObjectText.text = "紅隊建地";
        }
        else if (materialName == "composite")
        {
            sceneObjectImage.sprite = Resources.Load<Sprite>("materialSprite/" + materialName);
            sceneObjectText.text = "合成台";
        }
        else
        {
            sceneObjectImage.sprite = Resources.Load<Sprite>("materialSprite/" + materialName);
            sceneObjectText.text = "建材場：\n" + materialName;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //when mouse hover on the place mark
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        //Debug.Log("Cursor Entering " + name + " GameObject");
        sceneObjectDetailPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        //Debug.Log("Cursor Exiting " + name + " GameObject");
        sceneObjectDetailPanel.SetActive(false);
    }
}
