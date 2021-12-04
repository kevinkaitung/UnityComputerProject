using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoke_effect : MonoBehaviour
{
    [SerializeField]
    private GameObject[] smokeImg;

    System.Random Posrandom = new System.Random();

    // Start is called before the first frame update
    void OnEnable()
    {
        Debug.Log("smoke panel: OnEnable");
        for (int i = 0; i < 15; i++)
        {
            //move to range (random local position relative to canvas)
            //range 寬取500, 高取280，panel上的image會移動到隨機生成的位置座標上
            var range = new Vector3(Random.Range(-300f, 300f), Random.Range(-250f, 140f), 0.0f);
            //先把alpha值設成1(初始化)
            smokeImg[i].GetComponent<RectTransform>().LeanAlpha(1.0f, 0.0f);
            //延遲兩秒再淡出
            //改這邊的時間的話，也要改blackholeCollisionEffect.cs第75行(smokeEffectDelay())中coroutine的延遲秒數(目前最大總秒數為1+2秒)
            smokeImg[i].GetComponent<RectTransform>().LeanAlpha(0.0f, 1.0f).setDelay(2.0f).setEaseOutQuart();
            //同時移動到隨機產生的位置
            LeanTween.moveLocal(smokeImg[i], range, 1.5f).setEaseOutCirc();
            /*GameObject clone = Instantiate(Resources.Load("Assets/Resources/smokePanel", typeof(GameObject))) as GameObject;
            clone.transform.localPosition = new Vector2((int)Posrandom.Next(-200, 200), (int)Posrandom.Next(-100, 100));*/
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
