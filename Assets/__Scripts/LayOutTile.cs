﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileTex 
{
    public string texStr;
    public Texture2D tex;
}


public class LayOutTile : MonoBehaviour
{
    public static LayOutTile singleton;

    public GameObject tilePrefab;
    public Transform tileAnchor;

    public string roomNumder;    
    public TextAsset roomAsset;
    public Tile[,] tiles = new Tile[100, 100];

    public TileTex[] tileTexs;

    

    //解构room
    PT_XMLReader XMLR;
    PT_XMLHashList roomList;

    void Awake()
    {
        singleton = this;    
    }



    // Start is called before the first frame update
    void Start()
    {
        XMLR = new PT_XMLReader();
        XMLR.Parse(roomAsset.text);
        roomList = XMLR.xml["xml"][0]["room"];

        GameObject tileAnchorGo = new GameObject("tileAnchor");
        tileAnchor = tileAnchorGo.transform;
        

        BuildRoom(roomNumder);
    }

    public TileTex FindTexture(string tString) 
    {
        foreach (TileTex t in tileTexs) 
        {
            if (t.texStr == tString) 
            {
                return t;
            }
        }
        return null;
    }

    public void BuildRoom(string roomNum) 
    {
        PT_XMLHashtable room = roomList[int.Parse(roomNum)];
        string floorStr = room.att("floor");
        string wallStr = room.att("wall");

        string roomDetails = room.text;
        string[]roomDetail = roomDetails.Split('\n');
        
        int rowCount = roomDetail.Length;
        for (int i = 0; i < rowCount; i++)
        {
            roomDetail[i] = roomDetail[i].Trim();
        }



        Tile t;
        string type,tileTexStr;
        int height;
        GameObject go;
        int maxY = rowCount;


        for (int y = 0; y < rowCount; y++)
        {
            string roomRow = roomDetail[y];
            for (int x = 0; x < roomRow.Length; x++)
            {
                //设置默认值
                height = 0;
                tileTexStr = "floor";

                type = roomRow[x].ToString();
                switch (type) 
                {
                    case " ":
                    case "_":
                        continue;
                    case ".":
                        height = 0;
                        tileTexStr = "floor";
                        break;
                    case "|":
                        height = 1;
                        tileTexStr = "wall";
                        break;
                    default:
                        height = 0;
                        tileTexStr = "floor";
                        break;
                }
                go = Instantiate(tilePrefab);
                go.transform.SetParent(tileAnchor);
                t = go.GetComponent<Tile>();
                t.height = height;
                t.tex = tileTexStr;
                t.pos = new Vector3(x, maxY - y, 0);
                tiles[x, y] = t;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
