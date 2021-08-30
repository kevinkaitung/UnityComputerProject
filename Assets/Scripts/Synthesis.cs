using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using MongoDB.Bson;
using MongoDB.Driver;

public class Synthesis : MonoBehaviour
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
    void Start()
    {
        firstInputItem = "empty";
        secondInputItem = "empty";
        outputItem = "empty";
    }
    public string synthesis(string holdMaterial)
    {
        if (firstInputItem == "empty" && holdMaterial != "empty")
        {
            firstInputItem = holdMaterial;
            return "empty";
        }
        else if (secondInputItem == "empty" && firstInputItem != holdMaterial && holdMaterial != "empty")
        {
            secondInputItem = holdMaterial;
        }
        if (firstInputItem != "empty" && secondInputItem != "empty")
        {
            string result = check(firstInputItem, secondInputItem);
            if (result != "empty")
            {
                firstInputItem = "empty";
                secondInputItem = "empty";
                //PlayerClickActionforTeam.instance.holdMaterial = result;
            }
            else
            {
                Synthesis.instance.firstInputItem = "empty";
                Synthesis.instance.secondInputItem = "empty";
                //PlayerClickActionforTeam.instance.holdMaterial = "empty";
            }
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
                return data.outputItem;
            }
            if (item2 == data.firstInputItem && item1 == data.secondInputItem)
            {
                return data.outputItem;
            }
        }
        return "empty";
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