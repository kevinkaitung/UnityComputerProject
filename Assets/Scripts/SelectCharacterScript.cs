using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SelectCharacterScript : MonoBehaviour
{
    [SerializeField]
    private GameObject Character1;
    [SerializeField]
    private GameObject Character2;
    [SerializeField]
    private GameObject Character3;
    [SerializeField]
    private GameObject Character4;
    [SerializeField]
    private GameObject Character5;
    [SerializeField]
    private GameObject Character6;
    [SerializeField]
    private GameObject Character7;
    [SerializeField]
    private Text CharacterName;
    [SerializeField]
    private Image CharacterFace;

    [SerializeField]
    private Sprite one;
    [SerializeField]
    private Sprite two;
    [SerializeField]
    private Sprite three;
    [SerializeField]
    private Sprite four;
    [SerializeField]
    private Sprite five;
    [SerializeField]
    private Sprite six;
    [SerializeField]
    private Sprite seven;

    
    
    public void characterOne()
    {
        CharacterName.text = "1號";
        CharacterFace.sprite = one;
    }

    public void charactertwo()
    {
        CharacterName.text = "2號";
        CharacterFace.sprite = two;
    }

    public void characterthree()
    {
        CharacterName.text = "3號";
        CharacterFace.sprite = three;
    }

    public void characterfour()
    {
        CharacterName.text = "4號";
        CharacterFace.sprite = four;
    }

    public void characterfive()
    {
        CharacterName.text = "5號";
        CharacterFace.sprite = five;
    }

    public void charactersix()
    {
        CharacterName.text = "6號";
        CharacterFace.sprite = six;
    }

    public void characterseven()
    {
        CharacterName.text = "7號";
        CharacterFace.sprite = seven;
    }
}
