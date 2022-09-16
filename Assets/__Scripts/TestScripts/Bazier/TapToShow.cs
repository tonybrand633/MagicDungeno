using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyMouseInfo 
{
    public Vector3 loc;
    public Vector3 screenLoc;
    public Ray ray;
    public RaycastHit raycastHit;
    public bool hit;

    public bool detectHit() 
    {
        hit = Physics.Raycast(ray, out raycastHit);
        return hit; 
    }
}


public class TapToShow : MonoBehaviour
{
    public GameObject tapPrefab;
    GameObject tapGo;
    public List<MyMouseInfo> minfos = new List<MyMouseInfo>();

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            minfos.Clear();
            AddMouseInfo();
            TapShow();
            //if (tapGo == null) 
            //{
            //    TapShow();
            //}
        }

        //if (Input.GetMouseButtonUp(0)) 
        //{
        //    tapGo = null;
        //}

    }

    public void AddMouseInfo() 
    {
        MyMouseInfo info = new MyMouseInfo();
        info.screenLoc = Input.mousePosition;
        Vector3 tempLoc = info.screenLoc;
        tempLoc.z = -Camera.main.transform.position.z;
        
        info.loc = Camera.main.ScreenToWorldPoint(tempLoc);
        minfos.Add(info);
    }

    public void TapShow() 
    {        
        if (minfos.Count>0) 
        {
            tapGo = Instantiate(tapPrefab);
            tapGo.transform.position = lastMouseInfo.loc;
        }
    }

    public MyMouseInfo lastMouseInfo 
    {
        get
        {
            if (minfos.Count != 0)
            {
                return minfos[minfos.Count - 1];
            }
            else 
            {
                return null; 
            }
        }
    }
}


