﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MPhase 
{
    idle,
    down,
    drag
}

[System.Serializable]

public enum ElementType 
{
    earth,
    water,
    air,
    fire,
    aether,
    none
}


public class MouseInfo 
{
    public Vector3 loc;
    public Vector3 screenLoc;
    public Ray ray;  //从鼠标变为3D射线的光束
    public float Time;  //本地的MouseInfo事件信息
    public RaycastHit hitInfo;  //被光束击中的信息
    public bool hit;

    //两种方法确认mouseRay是否击中内容
    public RaycastHit Raycast() 
    {
        hit = Physics.Raycast(ray, out hitInfo);
        return hitInfo;
    }

    public RaycastHit Raycast(int mask) 
    {
        hit = Physics.Raycast(ray, out hitInfo, mask);
        return hitInfo;
    }

}

public class Mage : PT_MonoBehaviour
{
    public static Mage singleton;
    



    public float activeScreenWidth = 1;
    public bool DEBUG = false;
    //鼠标信息
    public bool ___________;
    public MPhase mPhase = MPhase.idle;
    public List<MouseInfo> mouseInfos = new List<MouseInfo>();
    public float mTapTime = 0.1f;
    public float mDragDist = 5; //定义拖动的最小像素距离
    public bool isActiveArea;

    //移动代码
    public float Speed = 2;
    public bool walking = false;
    public Vector3 walkTarget;
    public Transform characterTrans;
    public Vector2 SpeedVec;

    //技能信息
    public GameObject[] elementPrefabs;
    public float elementRotDis = 0.5f;//旋转半径
    public float elementRotSpeed = 1f;
    public int maxNumElementSelect = 1;

    public List<Element> SelectElements = new List<Element>();

    //指示器
    public GameObject tapIndicatorPrefab;




    void Awake()
    {
        singleton = this;
        mPhase = MPhase.idle;

        characterTrans = transform.Find("CharacterTrans");

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //检测是否鼠标0-鼠标左键
        bool mouse0Up = Input.GetMouseButtonUp(0);        
        bool mouse0Down = Input.GetMouseButtonDown(0);


        //在这里处理所有的输入(物品栏除外)
        /*
         *只有以下几种可能的动作
         *1.单点地面的指定地点
         *2.没有施用法术时从地面拖到法师处
         *3.施放法术沿着地面拖动
         *4.在敌人面前单击操作，表现为普通攻击（推击）                   
        */

        isActiveArea = (float)Input.mousePosition.x / Screen.width < activeScreenWidth
            && ((float)Input.mousePosition.y / Screen.height)<activeScreenWidth
            && ((float)Input.mousePosition.y / Screen.height)>0;
        
        if (mPhase == MPhase.idle) 
        {
            if (mouse0Down&&isActiveArea) 
            {
                mouseInfos.Clear();
                AddMouseInfo();

                //如果鼠标有单击内容，则是无效的MouseDown
                if (mouseInfos[0].hit) 
                {
                    //Debug.Log("有东西被击中");
                    MouseDown();
                    mPhase = MPhase.down;
                }
            }
        }

        if (mPhase == MPhase.down)  //如果鼠标左键被按下
        {
            //Debug.Log((float)Input.mousePosition.x / Screen.width);
            //Debug.Log((float)Input.mousePosition.y / Screen.height);
            AddMouseInfo();
            if (mouse0Up)  //说明是单击
            {
                MouseTap();
                mPhase = MPhase.idle;
            } else if (Time.time - mouseInfos[0].Time>mTapTime) 
            {
                //如果按下的时间长度超过单击的长度，则为拖动
                float dragDist = (lastMouseInfo.screenLoc - mouseInfos[0].screenLoc).magnitude ;
                //Debug.Log("dragDist:" + dragDist);
                if (dragDist>mDragDist) 
                {                    
                    mPhase = MPhase.drag;
                }

                //道具代码
                if (SelectElements.Count==0) 
                {
                    mPhase = MPhase.drag;
                }


            }
        }

        if (mPhase == MPhase.drag) 
        {
            AddMouseInfo();
            if (mouse0Up)
            {
                MouseDragUp();
                mPhase = MPhase.idle;
            }
            else 
            {
                MouseDrag();
            }
        }
        //旋转道具代码
        OrbitSelectElements();

        //if (Input.GetMouseButtonDown(0)) 
        //{
        //    bool isActiveArea = (float)Input.mousePosition.x / Screen.width < activeScreenWidth;
        //    Debug.Log("isActiveArea:" + isActiveArea);
        //    Debug.Log("MousePosition.X:" + Input.mousePosition.x+" Screen.width"+Screen.width+ " Input.mousePosition.x / Screen.width"+ (float)Input.mousePosition.x / Screen.width); 
        //}        
    }

    void FixedUpdate()
    {
        //用键盘来行走
        //SpeedVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        //WalkByKeyBoard();


        //if (rigidbody.velocity == Vector3.zero)
        //{
        //    walking = false;
        //}

        if (walking)
        {
            //Debug.Log("walkTarget" + walkTarget);
            //Debug.Log("position" + pos);
            //Debug.Log("距离" + (walkTarget - pos).magnitude);
            //Debug.Log("比较量" + Speed * Time.fixedDeltaTime);
            //Debug.Log("速度" + rigidbody.velocity);

            if ((walkTarget - pos).magnitude < Speed * Time.fixedDeltaTime)
            {
                pos = walkTarget;
                StopWalking();
            }
            else
            {
                rigidbody.velocity = (walkTarget - pos).normalized * Speed;
            }
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    public MouseInfo lastMouseInfo 
    {
        get 
        {
            if (mouseInfos.Count == 0) 
            {
                return null;
            }
            return mouseInfos[mouseInfos.Count - 1];
        }
    }

    MouseInfo AddMouseInfo() 
    {
        MouseInfo mInfo = new MouseInfo();
        mInfo.screenLoc = Input.mousePosition;

        //loc的来源,鼠标点击坐标转化为屏幕坐标   
        //Vector3 loc = Input.mousePosition;
        //loc.z = -Camera.main.transform.position.z;
        //loc = Camera.main.ScreenToWorldPoint(loc);
        //return (loc);
        mInfo.loc = Utils.mouseLoc;

        //ray的来源        
        //Vector3 loc = Input.mousePosition;
        //Ray r = Camera.main.ScreenPointToRay(loc);
        //return (r);        
        mInfo.ray = Utils.mouseRay;
        mInfo.Time = Time.time;
        //调用这方法，进行射线检测
        mInfo.Raycast();

        if (mouseInfos.Count==0) 
        {
            mouseInfos.Add(mInfo);
        }
        else
        {            
            float lastTime = mouseInfos[mouseInfos.Count - 1].Time;
            if (mInfo.Time!=lastTime) 
            {
                //当最后一个mouseInfo超时
                mouseInfos.Add(mInfo);
            }
        }
        return mInfo;
    }

    //道具代码
    //调用每个Update()方法使道具旋转

    public void SelectElement(ElementType elType) 
    {
        if (elType == ElementType.none) 
        {
            ClearElements();
            return;
        }

        if (maxNumElementSelect == 1) 
        {
            //清除以替换
            ClearElements();
        }
        if (SelectElements.Count>maxNumElementSelect) 
        {
            return;
        }
        //添加道具
        GameObject go = Instantiate(elementPrefabs[(int)elType]);
        Element el = go.GetComponent<Element>();
        el.transform.parent = this.transform;

        SelectElements.Add(el);
    }

    public void ClearElements() 
    {
        foreach (Element e in SelectElements) 
        {
            Destroy(e.gameObject);
        }
        SelectElements.Clear();
    }

    void OrbitSelectElements() 
    {
        //没有任何选择就返回
        if (SelectElements.Count == 0) return;

        Element el;
        Vector3 vec;
        float theta0, theta;
        float tau = Mathf.PI * 2; //tau in 360 in radians;

        //将圆圈划分到各个旋转道具
        float rotPerElement = tau/ SelectElements.Count;
        Debug.Log(rotPerElement);

        //基于时间来设置旋转基础角度（theta0）
        theta0 = elementRotSpeed * Time.time * tau;

        for (int i = 0; i < SelectElements.Count; i++)
        {
            //确定每个道具的旋转度
            theta = theta0 + i * rotPerElement;
            el = SelectElements[i];

            //使用"简单"的三角函数将角度转化为单位矢量
            vec = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
            vec *= elementRotDis;
            vec.z = -0.5f;
            el.transform.localPosition = vec;
        }

    }
    

    public void WalkByKeyBoard() 
    {
        if (SpeedVec.magnitude > 0)
        {
            walking = true;
            float rZ = Mathf.Atan2(SpeedVec.y, SpeedVec.x) * Mathf.Rad2Deg;
            rigidbody.velocity = SpeedVec * Speed;
            //Debug.Log(rZ);
            //characterTrans.rotation = Quaternion.Euler(0, 0, rZ); //可行
            characterTrans.eulerAngles = Vector3.forward * rZ; //不可行
            walking = true;
        }
        else 
        {
            rigidbody.velocity = Vector3.zero;
            walking = false;
        }        
    }

    public void WalkTo(Vector3 xTarget) 
    {
        walkTarget = xTarget;
        walkTarget.z = 0;
        walking = true;
        Face(walkTarget);    
    }

    public void Face(Vector3 poi) 
    {
        Vector3 delta = poi - pos;
        float rZ = Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x);
        characterTrans.rotation = Quaternion.Euler(0, 0, rZ);
    }

    public void StopWalking() 
    {
        walking = false;
        rigidbody.velocity = Vector3.zero;
    }



    void MouseDown()
    {
        if (DEBUG) 
        {
            Debug.Log("MouseDown");
        }
    }

    void MouseTap() 
    {
        WalkTo(lastMouseInfo.loc);
        ShowTap(lastMouseInfo.loc);
        if (DEBUG)
        {
            Debug.Log("MouseTap");
        }
    }

    void MouseDrag()
    {
        WalkTo(mouseInfos[mouseInfos.Count - 1].loc);
        if (DEBUG)
        {            
            Debug.Log("MouseDrag");
        }
    }

    void MouseDragUp()
    {
        StopWalking();
        if (DEBUG)
        {
            Debug.Log("MouseDragUp");
        }
    }

    public void ShowTap(Vector3 loc) 
    {
        GameObject go = Instantiate(tapIndicatorPrefab);
        go.transform.position = loc;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGo = collision.gameObject;
        Tile ti = otherGo.GetComponent<Tile>();
        if (ti!=null) 
        {
            if (ti.height>0) 
            {
                StopWalking();
            }
        }
    }

}