using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MPhase 
{
    idle,
    down,
    drag
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
    public float eachRotDis = 0.2f;
    public float elementRotSpeed = 1f;
    public int maxNumElementSelect = 1;
    public string actionStartTag;   
    public List<Element> SelectElements = new List<Element>();

    //指示器
    public GameObject tapIndicatorPrefab;

    //LineRenderer
    public Color[] elementColors;
    List<Vector3> LinePts;
    protected LineRenderer liner;
    protected float LineZ = -0.1f;

    public float LineMinDelta = 0.1f;
    public float LineMaxDelta = 0.5f;
    public float LineMaxLength = 8f;

    public float totalLineLength;


    //法术
    public GameObject FireGround;
    public Transform SpellAnchor;


    //public 变量对所有类可见
    //private 变量对本类可见
    //protected 变量对本类或子类可见
    




    void Awake()
    {
        singleton = this;
        mPhase = MPhase.idle;

        //初始化LinePoints
        LinePts = new List<Vector3>();
        characterTrans = transform.Find("CharacterTrans");
        liner = GetComponent<LineRenderer>();
        liner.enabled = true;

        GameObject goSpellAnchor = new GameObject("Spell Anchor");
        SpellAnchor = goSpellAnchor.transform;
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
        SpeedVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        WalkByKeyBoard();


        //if (rigidbody.velocity == Vector3.zero)
        //{
        //    walking = false;
        //}

        //if (walking)
        //{
        //    //Debug.Log("walkTarget" + walkTarget);
        //    //Debug.Log("position" + pos);
        //    //Debug.Log("距离" + (walkTarget - pos).magnitude);
        //    //Debug.Log("比较量" + Speed * Time.fixedDeltaTime);
        //    //Debug.Log("速度" + rigidbody.velocity);

        //    if ((walkTarget - pos).magnitude < Speed * Time.fixedDeltaTime)
        //    {
        //        pos = walkTarget;
        //        StopWalking();
        //    }
        //    else
        //    {
        //        rigidbody.velocity = (walkTarget - pos).normalized * Speed;
        //    }
        //}
        //else
        //{
        //    rigidbody.velocity = Vector3.zero;
        //}
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
        //Debug.Log(rotPerElement);

        //基于时间来设置旋转基础角度（theta0）
        theta0 = elementRotSpeed * Time.time * tau;
        //Debug.Log("Theta0:"+theta0);

        for (int i = 0; i < SelectElements.Count; i++)
        {
            //确定每个道具的旋转度
            theta = theta0 + i * rotPerElement;
            //Debug.Log("Theta:"+theta+"i:"+i*rotPerElement);
            el = SelectElements[i];

            //使用"简单"的三角函数将角度转化为单位矢量-让它做圆周运动
            vec = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
            vec *= elementRotDis;
            //vec *= (i + 1) * eachRotDis;
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
        GameObject hitGo = mouseInfos[0].hitInfo.collider.gameObject;
        
        GameObject parentGo = Utils.FindTaggedParent(hitGo);
        //Debug.Log(parentGo.name);
        if (parentGo == null)
        {
            actionStartTag = "";
        }
        else 
        {
            actionStartTag = parentGo.tag;
        }
    }

    void MouseTap() 
    {
        if (DEBUG)
        {
            Debug.Log("MouseTap");
        }

        switch (actionStartTag) 
        {
            case "Mage":
                break;
            case "Ground":
                WalkTo(lastMouseInfo.loc);
                ShowTap(lastMouseInfo.loc);
                break;
        }
    }

    void MouseDrag()
    {
        if (DEBUG)
        {            
            Debug.Log("MouseDrag");
        }
        //只有从地面开始拖动才会有效
        if (actionStartTag!="Ground") 
        {
            return;
        }

        //如果没有道具被选中，玩家应该随着鼠标移动
        if (SelectElements.Count == 0)
        {
            //移动到当前mouseInfos的位置
            WalkTo(mouseInfos[mouseInfos.Count - 1].loc);
        }
        else 
        {
            AddPointToLiner(mouseInfos[mouseInfos.Count - 1].loc);
        }

    }

    void MouseDragUp()
    {
        StopWalking();
        if (DEBUG)
        {
            Debug.Log("MouseDragUp");
        }

        if (actionStartTag!="Ground") 
        {
            return;
        }
        if (SelectElements.Count == 0)
        {
            StopWalking();
        }        
        else 
        {
            CastGroundSpell();
            ClearLiner();
        }
    }

    public void ShowTap(Vector3 loc) 
    {
        GameObject go = Instantiate(tapIndicatorPrefab);
        go.transform.position = loc;
    }

    //=================LinerRendererd代码====================//

    //为线条添加新的坐标
    void AddPointToLiner(Vector3 pt) 
    {        
        pt.z = LineZ;
        if (LinePts.Count == 0) 
        {
            LinePts.Add(pt);
            totalLineLength = 0;
            return;
        }

        //超过最大长度就return
        if (totalLineLength>LineMaxLength) 
        {
            return;
        }

        Vector3 ptLast = LinePts[LinePts.Count - 1];
        Vector3 dir = ptLast - pt;
        float dirDelta = dir.magnitude;
        //Debug.Log(dirDelta);
        dir.Normalize();

        

        //加上两点的方向向量的模长  --   这个地方的dirDelta相当于每一个点的长度相加
        totalLineLength += dirDelta;

        //如果两点距离太近了，则放弃添加
        if (dirDelta<LineMinDelta) 
        {
            return; 
        }

        //如果大于最远距离则为附加坐标
        if (dirDelta>LineMaxDelta) 
        {
            //在两者之间添加附加坐标
            float NumToAdd = Mathf.Ceil(dirDelta / LineMinDelta);            
            float MidDelta = dirDelta / NumToAdd;
            Vector3 ptMid;
            for (int i = 1; i < NumToAdd; i++)
            {
                ptMid = ptLast + (dir * MidDelta * i);
                LinePts.Add(ptMid);
            }            
        }
        LinePts.Add(pt);
        UpdateLiner();
        //LinePts.Add(pt);        
        //UpdateLiner();
    }

    //使用坐标更新LinerRenderer
    void UpdateLiner() 
    {
        int el = (int)SelectElements[0].elementType;
        //基于颜色设定线条的颜色
        liner.startColor = elementColors[el];
        liner.endColor = elementColors[el];

        //更新将要释放的法术外观
        liner.positionCount = LinePts.Count;
        for (int i = 0; i < LinePts.Count; i++)
        {
            liner.SetPosition(i, LinePts[i]);
        }
        liner.enabled = true;
    }

    void CastGroundSpell() 
    {
        if (SelectElements.Count == 0) 
        {
            return;
        }

        ElementType type = SelectElements[0].elementType;
        switch (type) 
        {
            case ElementType.fire:
                foreach (Vector3 pt in LinePts) 
                {
                    GameObject go = Instantiate(FireGround) as GameObject;
                    go.transform.parent = SpellAnchor;
                    go.transform.position = pt;
                }
                break;
        }

        ClearElements();

    }

    public void ClearLiner() 
    {
        liner.enabled = false;
        LinePts.Clear();
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
