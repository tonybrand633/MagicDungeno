using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : PT_MonoBehaviour
{
    public static CameraFollow singleton;
    public Transform targetTransform;
    public float camEasing = 0.1f;
    public Vector3 followOffset = new Vector3(0, 0, -2);


    void Awake()
    {
        singleton = this;    
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        pos = Vector3.Lerp(pos, targetTransform.transform.position + followOffset, camEasing);    
    }
}
