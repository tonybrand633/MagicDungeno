using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : PT_MonoBehaviour
{

    public string type;

    private string _tex;
    private Vector3 _pos;
    private int _height = 0;

    //一系列的get{}set{}
    new public Vector3 pos 
    {
        get { return _pos; }
        set 
        {
            _pos = value;
            transform.position = _pos;
            AdjustHeight();
        }
    }

    public int height 
    {
        get { return _height; }

        set 
        {
            _height = value;
            AdjustHeight();
        }
    }


    public string tex
    {
        get { return _tex; }
        set 
        {
            _tex = value;
            TileTex tileTex = LayOutTile.singleton.FindTexture(_tex);
            if (tileTex != null)
            {
                renderer.material.mainTexture = tileTex.tex;
                name = tileTex.texStr;

            }
            else 
            {
                Utils.trd("WE CAN'T Find Texture2D");
            }
        }
    }



    //尝试注释掉AdjustHeight看会发生什么
    public void AdjustHeight() 
    {
        Vector3 posOffset = Vector3.back * height;
        transform.position = _pos + posOffset;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
