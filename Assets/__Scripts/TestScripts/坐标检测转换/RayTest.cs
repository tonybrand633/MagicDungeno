using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public Ray ray;
    public RaycastHit raycastHit;
    public bool hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector3 loc = Input.mousePosition;
            ray = Camera.main.ScreenPointToRay(loc);
            hit = Physics.Raycast(ray,out raycastHit);
        }    
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(ray.origin,ray.direction);
    }
}
