using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapIndicator : PT_Mover
{
    public float lifeTime = 0.4f;
    public float[] scales;
    public Color[] colors;

    void Awake()
    {
        //transform.localScale
        scale = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        //PT_Mover基于PT_Loc类工作
        //类似于转换器但是更简单，Unity不期望用户创建转换器
        PT_Loc pLoc;
        List<PT_Loc> pLocs = new List<PT_Loc>();

        //指示器位置在相同位置0.1f处
        Vector3 tPos = pos;
        tPos.z = -0.1f;

        //在检查器中必须保持相同的比例和颜色
        for (int i = 0; i < scales.Length; i++)
        {
            pLoc = new PT_Loc();
            pLoc.scale = Vector3.one * scales[i];
            pLoc.pos = tPos;
            pLoc.color = colors[i];
            pLocs.Add(pLoc);
        }

        callback = CallBackMethod;

        PT_StartMove(pLocs, lifeTime);
    }

    void CallBackMethod()
    {
        Destroy(gameObject);
    }
}


