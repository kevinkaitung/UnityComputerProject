﻿using System.Collections;
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
    public string check(string item1, string item2)
    {
        var client = new MongoClient("mongodb+srv://exriesz:unity00757014@exriesz.lxfdc.mongodb.net/unity?retryWrites=true&w=majority");
        var database = client.GetDatabase("unity"); //數據庫名稱
        var collection = database.GetCollection<BsonDocument>("synthesisData");//連接的表名
        var list = collection.Find(_ => true).ToList();
        list[0].Remove("_id");
        var datas = list[0].ToJson();          //File.ReadAllText(Application.dataPath + "/Resources/synthesisData.json");
        Debug.Log(datas);
        AllData allData;
        allData = JsonMapper.ToObject<AllData>(datas);
        Debug.Log(allData);
        foreach (var data in allData.synthesisDataNodes)
        {
            if (item1 == data.FirstInputItem && item2 == data.SecondInputItem)
            {
                return data.OutputItem;
            }
            if (item2 == data.FirstInputItem && item1 == data.SecondInputItem)
            {
                return data.OutputItem;
            }
        }
        return "empty";
    }
}
