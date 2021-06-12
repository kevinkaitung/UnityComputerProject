using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class synthesisDataNodes : MonoBehaviour
{
    // Start is called before the first frame update
    public string firstInputItem;
    public string secondInputItem;
    public string outputItem;
    void Start()
    {
        firstInputItem = "empty";
        secondInputItem = "empty";
        outputItem = "empty";
    }
}

public class synthesisDataNodeClass
{
    public string firstInputItem;
    public string secondInputItem;
    public string outputItem;
}

public class AllData
{
    public List<synthesisDataNodeClass> synthesisDataNodes;
}