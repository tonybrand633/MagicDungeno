using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotManager : MonoBehaviour
{
    public Vector3 dir;
    public float Speed;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }    

    private void Move()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        dir = new Vector3(Horizontal, Vertical);
        transform.position += dir*Speed*Time.deltaTime;
    }
}
