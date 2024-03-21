using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodViewSceneInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject WaterField;
    [SerializeField]
    private GameObject WoodField;
    [SerializeField]
    private GameObject IronField;
    [SerializeField]
    private GameObject GravelField;
    [SerializeField]
    private GameObject FireField;
    [SerializeField]
    private GameObject CompositeField;
    [SerializeField]
    private GameObject RedBuildingSite;
    [SerializeField]
    private GameObject BlueBuildingSite;

    [SerializeField]
    private GameObject PlaceMarkPrefab;
    [SerializeField]
    private GameObject GodViewSceneInfoPanel;
    [SerializeField]
    private Camera GodCamera;
    // Start is called before the first frame update
    void Start()
    {
        //get material field pos in the scene, and then put place mark at the pos
        GameObject ironMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(IronField.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject waterMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(WaterField.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject compositeMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(CompositeField.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject woodMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(WoodField.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject fireMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(FireField.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject gravelMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(GravelField.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject redBuildingMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(RedBuildingSite.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);
        GameObject blueBuildingMark = Instantiate(PlaceMarkPrefab, GodCamera.WorldToScreenPoint(BlueBuildingSite.transform.position), Quaternion.identity, GodViewSceneInfoPanel.transform);   
        //set up the basic informations
        waterMark.GetComponent<SceneObjectInfo>().setup("water");
        woodMark.GetComponent<SceneObjectInfo>().setup("wood");
        ironMark.GetComponent<SceneObjectInfo>().setup("iron");
        gravelMark.GetComponent<SceneObjectInfo>().setup("gravel");
        fireMark.GetComponent<SceneObjectInfo>().setup("fire");
        compositeMark.GetComponent<SceneObjectInfo>().setup("composite");
        redBuildingMark.GetComponent<SceneObjectInfo>().setup("redBuilding");
        blueBuildingMark.GetComponent<SceneObjectInfo>().setup("blueBuilding");
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
