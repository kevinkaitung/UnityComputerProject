using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    //AudioController Singleton
    //Only create AudioController once
    private static AudioController s_Instance = null;
    public static AudioController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(AudioController)) as AudioController;

                if (s_Instance == null)
                    Debug.Log("Could not locate a AudioController " +
                              "object. \n You have to have exactly " +
                              "one AudioController in the scene.");
            }
            return s_Instance;
        }
    }
    public AudioSource ctrlAudioSource;
    [SerializeField]
    private AudioClip getItemAudioClip;
    [SerializeField]
    private AudioClip removeShapeAudioClip;
    [SerializeField]
    private AudioClip buildShapeAudioClip;
    [SerializeField]
    private AudioClip itemBoxGetAudioClip;
    [SerializeField]
    private AudioClip throwMaterialAudioClip;
    [SerializeField]
    private AudioClip winAudioClip;
    [SerializeField]
    private AudioClip loseAudioClip;
    public const string getItem = "getItem";
    public const string removeShape = "removeShape";
    public const string buildShape = "buildShape";
    public const string itemBoxGet = "itemBoxGet";
    public const string throwMaterial = "throwMaterial";
    public const string win = "win";
    public const string lose = "lose";
    // Start is called before the first frame update
    void Start()
    {
        ctrlAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //called by the player when the player do action
    public void actionPlaySound(string actionName)
    {
        switch (actionName)
        {
            case getItem:
                ctrlAudioSource.clip = getItemAudioClip;
                ctrlAudioSource.PlayOneShot(getItemAudioClip);
                break;
            case removeShape:
                ctrlAudioSource.clip = removeShapeAudioClip;
                ctrlAudioSource.PlayOneShot(removeShapeAudioClip);
                break;
            case buildShape:
                ctrlAudioSource.clip = buildShapeAudioClip;
                ctrlAudioSource.PlayOneShot(buildShapeAudioClip);
                break;
            case itemBoxGet:
                ctrlAudioSource.clip = itemBoxGetAudioClip;
                ctrlAudioSource.PlayOneShot(itemBoxGetAudioClip);
                break;
            case throwMaterial:
                ctrlAudioSource.clip = throwMaterialAudioClip;
                ctrlAudioSource.PlayOneShot(throwMaterialAudioClip);
                break;
            case win:
                ctrlAudioSource.clip = winAudioClip;
                ctrlAudioSource.PlayOneShot(winAudioClip);
                break;
            case lose:
                ctrlAudioSource.clip = loseAudioClip;
                ctrlAudioSource.PlayOneShot(loseAudioClip);
                break;
        }
    }
}
