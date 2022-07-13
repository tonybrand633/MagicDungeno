using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BT_State 
{
    idle,
    preMove,
    move,
    postMove
}

public delegate void BT_Callback();

public class MyBazierTool : MonoBehaviour
{
    public List<BT_Loc> bt_locs = null;
    public float bt_start;
    public float bt_duration;
    public BT_State bt_state = BT_State.idle;

    //只能被派生自MyBazierTool的类访问
    protected Material[] bt_mats;
    public float bt_u;
    public float bt_u2;
    public BT_Callback bt_callback;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update() 
    {
        //如果处于idle和postmove(移动后)的状态，则return
        if (bt_state == BT_State.idle||bt_state == BT_State.postMove) 
        {
            return;
        }

        //确定时间-余0.4说明u<0的时间为0.4f
        float u = (Time.time - bt_start) / bt_duration;
        Debug.Log(u);
    }

    public void BT_StartMove(List<BT_Loc>locs,float timeDuration = 1,float timeStart = float.NaN) 
    {
        if (float.IsNaN(timeStart)) 
        {
            timeStart = Time.time;
        }
        bt_start = timeStart;
        bt_duration = timeDuration;
        bt_locs = locs;
        //准备移动
        bt_state = BT_State.preMove;
    }
}

[System.Serializable]
public class BT_Loc 
{
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 scale = Vector3.one;
    public Color color = Color.white;

    public Vector3 pos 
    {
        get { return position; }
        set 
        {
            position = value;
        }
    }

    public Quaternion rot 
    {
        get { return rotation; }

        set 
        {
            rotation = value;
        }
    }

    public Vector3 localScale 
    {
        get { return scale; }
        set 
        {
            scale = value;
        }
    }

    //用BT_Loc来存储传入transformT的位置
    public BT_Loc(Transform t = null) 
    {
        if (t == null) 
        {
            return;
        }
        pos = t.position;
        rot = t.rotation;
        scale = t.localScale;
    }
}
