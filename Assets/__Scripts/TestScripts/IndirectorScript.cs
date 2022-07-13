using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectorScript : MyBazierTool
{
    public float lifeTime = 0.4f;
    public float[] scales;
    public Color[] colors;

    void Awake()
    {
        //this.transform.localScale = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        BT_Loc bLoc;
        List<BT_Loc> bLocs = new List<BT_Loc>();

        Vector3 tPos = transform.position;
        tPos.z = -0.1f;

        for (int i = 0; i < scales.Length; i++)
        {
            bLoc = new BT_Loc();
            bLoc.scale = Vector3.one * scales[i];
            bLoc.pos = tPos;
            bLoc.color = colors[i];
            bLocs.Add(bLoc);
        }

        bt_callback = MyCallBackMethod;
        BT_StartMove(bLocs, lifeTime);


    }

    void MyCallBackMethod() 
    {
        Destroy(this.gameObject);
    }

    //继承了MyBazierTool的Update
}
