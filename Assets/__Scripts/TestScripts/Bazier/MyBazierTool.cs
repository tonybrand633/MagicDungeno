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
    public string bt_easing = Easing.In;
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
        float u2 = u;

        //u<0说明没有在移动或者移动不正确？
        if (u < 0)
        {
            bt_state = BT_State.preMove;
            u = 0;
            //传递进来的bt_loc
            BT_SetLoc(bt_locs[0]);
        }
        //u>1代表运动已经结束
        else if (u > 1)
        {
            bt_state = BT_State.postMove;
            u = 1;
            BT_SetLoc(bt_locs[bt_locs.Count - 1]);
            if (bt_callback != null)
            {
                bt_callback();
            }
        }
        else 
        {
            bt_state = BT_State.move;
            u2 = MyEasing.Ease(u, bt_easing);
            //真正用来把u2和位置相关联的函数
            BT_Loc b_loc = BT_Loc.Bazier(u2, bt_locs);
            BT_SetLoc(b_loc);
        }
        bt_u = u;
        bt_u2 = u2;
        //Debug.Log(u);
    }

    public void BT_SetLoc(BT_Loc l) 
    {
        //这里的transform.position应该是子类的transform.position --- 验证了确实是这样
        transform.position = l.pos;
        transform.rotation = l.rot;
        transform.localScale = l.localScale;

        if (bt_mats == null)
        {
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            bt_mats = new Material[rends.Length];
            for (int i = 0; i < rends.Length; i++)
            {
                bt_mats[i] = rends[i].material;
            }
        }
        foreach (Material m in bt_mats)
        {
            m.color = l.color;
        }

        //if (bt_mats == null)
        //{
        //    Renderer[] renderers = GetComponentsInChildren<Renderer>();
        //    bt_mats = new Material[renderers.Length];
        //    for (int i = 0; i < renderers.Length; i++)
        //    {
        //        bt_mats[i] = renderers[i].material;
        //    }
        //}

        //if (bt_mats==null) 
        //{
        //    Renderer  renders = GetComponent<Renderer>();
        //    bt_mats = renders.material;
        //    bt_mats.color = l.color;            
        //}

        //foreach (Material mat in bt_mats)
        //{
        //    mat.color = l.color;
        //}

        //Debug.Log(this.gameObject.name);
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

    public static BT_Loc Bazier(float u,List<BT_Loc>locs) 
    {
        return Bazier(u, locs.ToArray());
    }

    public static BT_Loc Bazier(float u, BT_Loc[]locs,int iL = 0,int iR = -1) 
    {
        if (iR == -1) 
        {
            //3-1 = 2
            iR = locs.Length - 1;
        }

        if (iR == iL) 
        {
            //说明locs只有一个
            return locs[iR];
        }

        BT_Loc res = BT_Loc.Lerp(Bazier(u, locs, iL, iR - 1),Bazier(u,locs,iL+1,iR),u);
        return res;
    }

    public static BT_Loc Lerp(BT_Loc b1,BT_Loc b2,float u) 
    {
        BT_Loc b = new BT_Loc();
        b.pos = (1 - u) * b1.pos + u * b2.pos;
        b.rot = Quaternion.Slerp(b1.rot, b2.rot, u);
        b.scale = (1 - u) * b1.scale + u * b2.scale;
        b.color = (1 - u) * b1.color + u * b2.color;
        return b;
    }

}
