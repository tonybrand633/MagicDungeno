using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : MonoBehaviour
{
    public float duration = 4f;
    public float durationVariane = 0.5f;

    public float fadeTime = 1f;
    public float timeStart;


    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.time;
        duration = Random.Range(duration - durationVariane, duration + durationVariane);
        //3.5~4.5 duration之间
    }

    // Update is called once per frame
    void Update()
    {
        float u = (Time.time - timeStart)/duration;

        float fadePercent = 1 - (fadeTime / duration);
        if (u>fadePercent) 
        {
            //开始衰减
            float u2 = (u - fadePercent) / (1 - fadePercent);
            //u2是用于fadeTime的u;
            //Debug.Log(u2);
            Vector3 temPos = transform.position;
            temPos.z *= u2;
            transform.position = temPos;
        }
        if (u>1) 
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject goHit = Utils.FindTaggedParent(other.gameObject);
        if (goHit == null) 
        {
            goHit = other.gameObject;   
        }
        Utils.tr("Frame hit", goHit.name);
    }
}
