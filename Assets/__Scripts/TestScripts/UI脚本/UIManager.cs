using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void UIDelegate();

public class UIManager:MonoBehaviour
{
    public Button[] Buttons;
    public GameObject Target;
    public BTools bTool;

    void Start()
    {
        Target = GameObject.Find("Target");
        bTool = Target.GetComponent<BTools>();
        foreach (var item in Buttons) 
        {
            item.onClick.AddListener(() => OnButtonClick(item));
        }    
    }

    public void OnButtonClick(Button btn) 
    {
        switch (btn.name) 
        {
            case "StartMoveBtn":
                if (bTool.time_Duration!=0) 
                {
                    bTool.StartMove(Time.time);
                }                
                break;
            default:
                Debug.Log("End");
                break;
        }
    }

}
