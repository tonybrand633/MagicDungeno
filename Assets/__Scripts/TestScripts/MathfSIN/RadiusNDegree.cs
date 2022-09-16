using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusNDegree : MonoBehaviour
{
    public float AngleInDegree;

    public float cos;
    public float sin;

    public float Speed;

    // Update is called once per frame
    void Update()
    {
        //Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * AngleInDegree), 0, Mathf.Cos(Mathf.Deg2Rad * AngleInDegree));
        //Debug.DrawLine(transform.position, direction,Color.red);
        //sin = Mathf.Sin(Mathf.Deg2Rad*AngleInDegree);
        //cos = Mathf.Cos(Mathf.Deg2Rad * (90 - AngleInDegree));

        //Vector3 direction = new Vector3(Mathf.Cos((90-AngleInDegree)*Mathf.Deg2Rad), Mathf.Sin((90-AngleInDegree)*Mathf.Deg2Rad), 0);
        Vector3 direction = new Vector3(Mathf.Sin(AngleInDegree * Mathf.Deg2Rad), Mathf.Cos(AngleInDegree * Mathf.Deg2Rad), 0);
        Vector2 SpeedVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Speed = SpeedVec.magnitude;
        Move();
        
    }


    public void Move() 
    {
        if (Speed>0) 
        {
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");

            Vector3 InputVec = new Vector3(Horizontal, Vertical, 0).normalized;
            Debug.DrawRay(transform.position, InputVec * 3, Color.red);

            float rZ = Mathf.Atan2(InputVec.y, InputVec.x) * Mathf.Rad2Deg;

            transform.Translate(InputVec * Time.deltaTime * 5, Space.World);
            Debug.Log(rZ);
            //transform.eulerAngles = Vector3.forward * rZ;
            transform.rotation = Quaternion.Euler(0, 0, rZ);
        }
        
    }
}
