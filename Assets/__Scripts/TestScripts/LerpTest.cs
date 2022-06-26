using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTest : MonoBehaviour
{
    public bool isMoveStart;

    public float Speed;
    public float floatEasing;

    public Vector3 startPosition;
    public Transform positionA;
    

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveStart)
        {
            transform.position = Vector3.Lerp(transform.position, positionA.position, floatEasing);
        }
        else 
        {
            transform.position = startPosition;
        }
        
    }
}
