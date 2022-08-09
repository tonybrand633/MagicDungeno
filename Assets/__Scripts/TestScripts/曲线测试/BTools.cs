using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BTools : MonoBehaviour
{
    public float time_Start;
    public float time_Duration;


    public float u;
    public string EasingWay;    
    public Transform endPoint;
    public MoveState moveState = MoveState.preMove;
    public bool startMove;

    //用来监听输入的文本lifeTime
    public InputField input;


    public Vector3 startPoint;

    void Awake()
    {
        startPoint = this.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        //startPoint = this.transform.position;
        //input.onValueChanged.AddListener(OnInputFieldValueChange);
        //input.onEndEdit.AddListener(OnInputFieldValueChange);
    }

    // Update is called once per frame
    void Update()
    {
        Text t = input.transform.Find("Text").GetComponent<Text>();
        float res;
        if (t.text!="") 
        {
            if (float.TryParse(t.text, out res)) 
            {
                time_Duration = res;
            }             
        }
        
        if (startMove) 
        {
            Move();
        }
    }

    //点击Button "开始运动"
    public void StartMove(float startTime) 
    {
        time_Start = startTime;
        startMove = true;        
    }

    public void Move()
    {
        u = (Time.time - time_Start) / time_Duration;

        if (u < 0)
        {
            moveState = MoveState.preMove;
            startMove = false;
        }
        else if (u > 1)
        {
            moveState = MoveState.endMove;
        }
        else
        {
            moveState = MoveState.Move;
            this.transform.position = (1 - u) * startPoint + u * endPoint.position;

        }
    }    
}

public enum MoveState 
{
    preMove,
    Move,
    endMove
}
