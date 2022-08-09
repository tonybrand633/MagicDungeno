using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreentToWorld : MonoBehaviour
{
    public Vector3 mouseLoc;
    public Vector3 gameLoc;
    public GameObject Cube;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouseLoc = Input.mousePosition;
        mouseLoc.z = 20;
        gameLoc = Camera.main.ScreenToWorldPoint(mouseLoc);
        Cube.transform.position = gameLoc;
    }
}
