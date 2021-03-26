using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
public class node
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string objShape;
    public string materialName;
    public int stage;

    //constructor
    /*public node(Vector3 pos, string shape)
    {
        this.position = pos;
        this.objShape = shape;
        this.materialName = null;
        this.stage = -1;
    }*/
}

[System.Serializable]
public class nodeRoot
{
    public node[] gameDataNodes;
}