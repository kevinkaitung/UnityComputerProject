using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using MongoDB.Bson;
using MongoDB.Driver;

public class Synthesis : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private static Synthesis s_Instance = null;
    public static Synthesis instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(Synthesis)) as Synthesis;

                if (s_Instance == null)
                    Debug.Log("Could not locate a Synthesis " +
                              "object. \n You have to have exactly " +
                              "one Synthesis in the scene.");
            }
            return s_Instance;
        }
    }
    public string firstInputItem;
    public string secondInputItem;
    public string outputItem;
    public TextAsset synthesisJsonFile;
    private string firstInputItemForBlueTeamKeyName = "synthesisFirstInputItemForBlueTeam"; //紀錄藍隊的合成台的第一項物品的 hashtable key
    private string firstInputItemForRedTeamKeyName = "synthesisFirstInputItemForRedTeam";   //紀錄紅隊的合成台的第一項物品的 hashtable key
    ExitGames.Client.Photon.Hashtable firstInputItemHashtableForBlueTeam;  //key-value hashtable 兩組key-value分別存藍隊和紅隊的第一項物品(算是要傳遞設置customproperties，傳遞參數的資料結構)
    ExitGames.Client.Photon.Hashtable firstInputItemHashtableForRedTeam;  //key-value hashtable 兩組key-value分別存藍隊和紅隊的第一項物品(算是要傳遞設置customproperties，傳遞參數的資料結構)
    private string myTeam;  //紀錄本地端玩家的隊伍，以修改相對應的第一項物品
    public GameObject showSynthesisMaterialCube;
    private MeshRenderer synthesisMaterialMesh;
    public GameObject synthesisPanel;
    public GameObject synthesisImage;
    void Start()
    {
        firstInputItem = "empty";
        secondInputItem = "empty";
        outputItem = "empty";
        myTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;    //隊伍名字分別是"Blue"和"Red"
        firstInputItemHashtableForBlueTeam = new ExitGames.Client.Photon.Hashtable();  //實例化hashtable
        firstInputItemHashtableForRedTeam = new ExitGames.Client.Photon.Hashtable();  //實例化hashtable
        firstInputItemHashtableForBlueTeam.Add(firstInputItemForBlueTeamKeyName, "empty"); //將hashtable新增一組key-value
        firstInputItemHashtableForRedTeam.Add(firstInputItemForRedTeamKeyName, "empty");  //將hashtable新增一組key-value
        synthesisMaterialMesh = showSynthesisMaterialCube.GetComponent<MeshRenderer>();
        showSynthesisMaterialCube.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(firstInputItemHashtableForBlueTeam); //第一次新增customproperties由房主新增就好(如果每個人都執行這個動作也不影響，只是沒有必要)
            PhotonNetwork.CurrentRoom.SetCustomProperties(firstInputItemHashtableForRedTeam); //第一次新增customproperties由房主新增就好(如果每個人都執行這個動作也不影響，只是沒有必要)
        }
    }

    public string synthesis(string holdMaterial)
    {
        if(holdMaterial == "removalToolMyself" || holdMaterial == "removalToolOther")
        {
            return holdMaterial;
        }
        if (firstInputItem == "empty" && holdMaterial != "empty")
        {
            firstInputItem = holdMaterial;
            if (myTeam == "Blue")
            {
                firstInputItemHashtableForBlueTeam[firstInputItemForBlueTeamKeyName] = holdMaterial;   //第一個物品被放上去之後，要修改customproperties前，先將hashtable相對應隊伍的value改成新的東西
                PhotonNetwork.CurrentRoom.SetCustomProperties(firstInputItemHashtableForBlueTeam);     //要set customproperties時，直接把hashtable當成參數傳入修改(執行此動作的玩家)
            }
            else if (myTeam == "Red")
            {
                firstInputItemHashtableForRedTeam[firstInputItemForRedTeamKeyName] = holdMaterial;
                PhotonNetwork.CurrentRoom.SetCustomProperties(firstInputItemHashtableForRedTeam);     //要set customproperties時，直接把hashtable當成參數傳入修改(執行此動作的玩家)
            }
            photonView.RPC("showSynthesisMaterial", RpcTarget.All, firstInputItem, myTeam);
            return "empty";
        }
        else if (secondInputItem == "empty" && firstInputItem != holdMaterial && holdMaterial != "empty")
        {
            secondInputItem = holdMaterial;
        }
        if (firstInputItem != "empty" && secondInputItem != "empty")
        {
            string result = check(firstInputItem, secondInputItem);
            if (myTeam == "Blue")
            {
                firstInputItemHashtableForBlueTeam[firstInputItemForBlueTeamKeyName] = "empty";   //第一個物品被放上去之後，要修改customproperties前，先將hashtable相對應隊伍的value改成新的東西
                PhotonNetwork.CurrentRoom.SetCustomProperties(firstInputItemHashtableForBlueTeam);     //要set customproperties時，直接把hashtable當成參數傳入修改(執行此動作的玩家)
            }
            else if (myTeam == "Red")
            {
                firstInputItemHashtableForRedTeam[firstInputItemForRedTeamKeyName] = "empty";
                PhotonNetwork.CurrentRoom.SetCustomProperties(firstInputItemHashtableForRedTeam);     //要set customproperties時，直接把hashtable當成參數傳入修改(執行此動作的玩家)
            }
            firstInputItem = "empty";
            secondInputItem = "empty";
            photonView.RPC("showSynthesisMaterial", RpcTarget.All, firstInputItem, myTeam);
            return result;
        }
        return "empty";
    }
    public string check(string item1, string item2)
    {
        /*var client = new MongoClient("mongodb+srv://exriesz:unity00757014@exriesz.lxfdc.mongodb.net/unity?retryWrites=true&w=majority");
        var database = client.GetDatabase("unity"); //數據庫名稱
        var collection = database.GetCollection<BsonDocument>("synthesisData");//連接的表名
        var list = collection.Find(_ => true).ToList();
        list[0].Remove("_id");
        var datas = list[0].ToJson();          //File.ReadAllText(Application.dataPath + "/Resources/synthesisData.json");
        Debug.Log(datas);*/
        string datas = synthesisJsonFile.text;
        AllData allData;
        allData = JsonMapper.ToObject<AllData>(datas);
        Debug.Log(allData);
        foreach (var data in allData.synthesisDataNodes)
        {
            if (item1 == data.firstInputItem && item2 == data.secondInputItem)
            {
                synthesisPanel.SetActive(true);
                synthesisPanel.GetComponentInChildren<Text>().text = "恭喜成功合成了:\n" + data.outputItem;
                synthesisImage.SetActive(true);
                synthesisImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("materialSprite/" + data.outputItem);
                StartCoroutine(showSynthesisPanel());
                return data.outputItem;
            }
            if (item2 == data.firstInputItem && item1 == data.secondInputItem)
            {
                synthesisPanel.SetActive(true);
                synthesisPanel.GetComponentInChildren<Text>().text = "恭喜成功合成了:\n" + data.outputItem;
                synthesisImage.SetActive(true);
                synthesisImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("materialSprite/" + data.outputItem);
                StartCoroutine(showSynthesisPanel());
                return data.outputItem;
            }
        }
        synthesisPanel.SetActive(true);
        synthesisImage.SetActive(false);
        synthesisPanel.GetComponentInChildren<Text>().text = "合成錯誤!\n請放入正確的材料!";
        StartCoroutine(showSynthesisPanel());
        return "empty";
    }

    //只要有其他人(或自己)有set customproperties，所有人都會收到這個callback function。收到後，下面的引數是修改過後的hashtable，再依隊伍指定給相對應隊伍的玩家的firstinputitem
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log(propertiesThatChanged[firstInputItemForBlueTeamKeyName]);
        Debug.Log(propertiesThatChanged[firstInputItemForRedTeamKeyName]);
        if (myTeam == "Blue")
        {
            if (propertiesThatChanged[firstInputItemForBlueTeamKeyName] != null)
            {
                firstInputItem = propertiesThatChanged[firstInputItemForBlueTeamKeyName].ToString();
            }
        }
        else if (myTeam == "Red")
        {
            if (propertiesThatChanged[firstInputItemForRedTeamKeyName] != null)
            {
                firstInputItem = propertiesThatChanged[firstInputItemForRedTeamKeyName].ToString();
            }
        }
    }

    [PunRPC]
    public void showSynthesisMaterial(string firstInputItem, string myteam)
    {
        if (myTeam == myteam)
        {
            //if hold material is empty
            if (firstInputItem == "empty")
            {
                showSynthesisMaterialCube.SetActive(false);
            }
            else
            {
                //show texture to hold material
                showSynthesisMaterialCube.SetActive(true);
                synthesisMaterialMesh.material = Resources.Load("materialTexture/Materials/" + firstInputItem) as Material;
            }
        }
    }

    public void closeSynthesisPanel()
    {
        synthesisPanel.SetActive(false);
    }
    IEnumerator showSynthesisPanel()
    {
        yield return new WaitForSeconds(2);
        synthesisPanel.SetActive(false);
    }

}
public class AllData
{
    public List<synthesisDataNode> synthesisDataNodes;
}
public class synthesisDataNode
{
    public string firstInputItem;
    public string secondInputItem;
    public string outputItem;
}

