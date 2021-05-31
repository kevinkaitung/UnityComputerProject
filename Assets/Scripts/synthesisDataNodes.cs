using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class synthesisDataNodes : MonoBehaviour
{
    // Start is called before the first frame update
<<<<<<< HEAD
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
=======
    public string FirstInputItem;
    public string SecondInputItem;
    public string OutputItem;
    void Start()
    {
        FirstInputItem = "empty";
        SecondInputItem = "empty";
        OutputItem = "empty";
    }
}
public class AllData
{
    public List<synthesisDataNodes> synthesisDataNodes;
}
>>>>>>> remotes/origin/00757014
