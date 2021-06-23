using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class nodeManager : MonoBehaviour
{
    //NodeManager Singleton
    //Only create NodeManager once
    private static nodeManager s_Instance = null;
    public static nodeManager instance
    {   
        get
        {   
            if (s_Instance == null)
            {   
                s_Instance = FindObjectOfType(typeof(nodeManager)) as nodeManager;
                 
                if (s_Instance == null)
                    Debug.Log("Could not locate a NodeManager " +
                              "object. \n You have to have exactly " +
                              "one NodeManager in the scene.");
            }
            return s_Instance;
        }
    }
    //serializable?
    public nodeRoot dataRoot;
    void Awake() 
    {
        createGameData();
    }

    void createGameData()
    {
        var client = new MongoClient("mongodb+srv://exriesz:unity00757014@exriesz.lxfdc.mongodb.net/unity?retryWrites=true&w=majority");
        var database = client.GetDatabase("unity"); //数据库名称
        var collection = database.GetCollection<BsonDocument>("gameData");//连接的表名
        var list = collection.Find(_ => true).ToList();
        list[0].Remove("_id");
        var datas = list[0].ToJson();
        Debug.Log(datas);
        dataRoot = JsonUtility.FromJson<nodeRoot>(datas);
        for(int i = 0; i < 8; i++)
        {
            Debug.Log(dataRoot.gameDataNodes[i].position + " " + dataRoot.gameDataNodes[i].objShape);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
